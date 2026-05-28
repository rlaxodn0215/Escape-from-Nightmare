using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
    public class LocationManager : Singleton<LocationManager>
    {
        [SerializeField] private Transform locationRoot;
        [SerializeField] private List<LocationController> locationControllers = new List<LocationController>();
        [SerializeField] private bool collectLocationsFromRoot = true;
        [SerializeField] private string startingLocationId;
        [SerializeField] private string startingViewId;

        private readonly Dictionary<string, LocationController> locationMap = new Dictionary<string, LocationController>();
        private LocationController currentLocationController;
        private bool hasInitialized;
        private bool suppressChaseMoveRegistration;

        public string CurrentLocationId;
        public string CurrentViewId;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                return;
            }

            BuildLocationMap();
            DeactivateAllLocations();
        }

        private void Start()
        {
            if (!hasInitialized)
            {
                ApplySavedPositionOrStartingLocation();
            }
        }

        public void RegisterLocation(LocationController controller)
        {
            if (controller == null)
            {
                return;
            }

            if (!locationControllers.Contains(controller))
            {
                locationControllers.Add(controller);
            }

            if (string.IsNullOrEmpty(controller.LocationId))
            {
                Debug.LogWarning("Cannot register LocationController with empty locationId: " + controller.name, controller);
                return;
            }

            if (!locationMap.ContainsKey(controller.LocationId))
            {
                locationMap.Add(controller.LocationId, controller);
            }
            else if (locationMap[controller.LocationId] != controller)
            {
                Debug.LogWarning("Duplicate locationId ignored: " + controller.LocationId, controller);
            }
        }

        public void BuildLocationMap()
        {
            if (collectLocationsFromRoot)
            {
                CollectLocationsFromRoot();
            }

            locationMap.Clear();

            for (int i = 0; i < locationControllers.Count; i++)
            {
                LocationController controller = locationControllers[i];
                if (controller == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(controller.LocationId))
                {
                    Debug.LogWarning("LocationController has an empty locationId: " + controller.name, controller);
                    continue;
                }

                if (locationMap.ContainsKey(controller.LocationId))
                {
                    Debug.LogWarning("Duplicate locationId ignored: " + controller.LocationId, controller);
                    continue;
                }

                locationMap.Add(controller.LocationId, controller);
            }
        }

        public void CollectLocationsFromRoot()
        {
            if (locationRoot == null)
            {
                return;
            }

            LocationController[] foundControllers = locationRoot.GetComponentsInChildren<LocationController>(true);
            for (int i = 0; i < foundControllers.Length; i++)
            {
                if (foundControllers[i] != null && !locationControllers.Contains(foundControllers[i]))
                {
                    locationControllers.Add(foundControllers[i]);
                }
            }
        }

        public void InitializeStartingLocation()
        {
            if (hasInitialized)
            {
                return;
            }

            bool previousSuppressChaseMoveRegistration = suppressChaseMoveRegistration;
            suppressChaseMoveRegistration = true;

            string resolvedLocationId = null;
            string resolvedViewId = null;

            if (!string.IsNullOrEmpty(startingLocationId))
            {
                resolvedLocationId = startingLocationId;
                resolvedViewId = startingViewId;
            }
            else if (GameDataManager.Instance != null && GameDataManager.Instance.Locations != null && GameDataManager.Instance.Locations.Count > 0)
            {
                LocationRecord firstLocation = GameDataManager.Instance.Locations[0];
                if (firstLocation != null && !string.IsNullOrEmpty(firstLocation.locationId))
                {
                    resolvedLocationId = firstLocation.locationId;
                    resolvedViewId = firstLocation.defaultViewId;
                }
            }
            else
            {
                for (int i = 0; i < locationControllers.Count; i++)
                {
                    if (locationControllers[i] != null && !string.IsNullOrEmpty(locationControllers[i].LocationId))
                    {
                        resolvedLocationId = locationControllers[i].LocationId;
                        resolvedViewId = locationControllers[i].DefaultViewId;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(resolvedLocationId))
            {
                Debug.LogWarning("No starting location could be resolved.");
                suppressChaseMoveRegistration = previousSuppressChaseMoveRegistration;
                return;
            }

            SetLocation(resolvedLocationId, resolvedViewId);
            hasInitialized = !string.IsNullOrEmpty(CurrentLocationId);
            suppressChaseMoveRegistration = previousSuppressChaseMoveRegistration;
        }

        public void ApplySavedPositionOrStartingLocation()
        {
            if (hasInitialized)
            {
                return;
            }

            if (TryApplySavedPosition())
            {
                return;
            }

            InitializeStartingLocation();
        }

        public bool TryApplySavedPosition()
        {
            if (SaveManager.Instance == null)
            {
                return false;
            }

            string locationId = SaveManager.Instance.CurrentLocationId;
            if (string.IsNullOrEmpty(locationId))
            {
                return false;
            }

            if (GetLocationController(locationId) == null)
            {
                Debug.LogWarning("Saved location cannot be applied because no LocationController exists: " + locationId);
                return false;
            }

            bool previousSuppressChaseMoveRegistration = suppressChaseMoveRegistration;
            suppressChaseMoveRegistration = true;
            SetLocation(locationId, SaveManager.Instance.CurrentViewId);
            suppressChaseMoveRegistration = previousSuppressChaseMoveRegistration;

            bool applied = CurrentLocationId == locationId;
            hasInitialized = applied;
            return applied;
        }

        public void SetLocation(string locationId, string viewId = null)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                Debug.LogWarning("Cannot set an empty locationId.");
                return;
            }

            LocationController targetController = GetLocationController(locationId);
            if (targetController == null)
            {
                Debug.LogWarning("LocationController not found: " + locationId);
                return;
            }

            if (currentLocationController != null && currentLocationController != targetController)
            {
                currentLocationController.SetLocationActive(false);
            }

            targetController.SetLocationActive(true);

            string targetViewId = !string.IsNullOrEmpty(viewId) ? viewId : GetDefaultViewIdForLocation(locationId, targetController);
            bool wasSamePosition = IsSameCurrentPosition(locationId, targetViewId);
            bool activated = false;

            if (!string.IsNullOrEmpty(targetViewId))
            {
                activated = targetController.ActivateView(targetViewId);
            }

            if (!activated)
            {
                activated = targetController.ActivateDefaultView();
            }

            if (!activated)
            {
                Debug.LogWarning("Location activated without a valid view: " + locationId);
            }

            currentLocationController = targetController;
            CurrentLocationId = locationId;
            CurrentViewId = targetController.CurrentViewId;

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SetCurrentPosition(CurrentLocationId, CurrentViewId);
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            RefreshActiveView();

            if (!wasSamePosition)
            {
                RegisterChaseMoveIfNeeded();
            }
        }

        public void SetView(string viewId)
        {
            if (currentLocationController == null)
            {
                Debug.LogWarning("Cannot set view because there is no current location.");
                return;
            }

            if (string.IsNullOrEmpty(viewId))
            {
                Debug.LogWarning("Cannot set an empty viewId.");
                return;
            }

            if (currentLocationController.ActivateView(viewId))
            {
                string previousViewId = CurrentViewId;
                CurrentViewId = currentLocationController.CurrentViewId;

                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SetCurrentPosition(CurrentLocationId, CurrentViewId);
                }
                else
                {
                    Debug.LogWarning("SaveManager instance is missing.");
                }

                if (previousViewId != CurrentViewId)
                {
                    RegisterChaseMoveIfNeeded();
                }
            }
        }

        public void RotateLeft()
        {
            if (currentLocationController == null)
            {
                Debug.LogWarning("Cannot rotate left because there is no current location.");
                return;
            }

            if (currentLocationController.RotateView(-1))
            {
                string previousViewId = CurrentViewId;
                CurrentViewId = currentLocationController.CurrentViewId;

                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SetCurrentPosition(CurrentLocationId, CurrentViewId);
                }
                else
                {
                    Debug.LogWarning("SaveManager instance is missing.");
                }

                if (previousViewId != CurrentViewId)
                {
                    RegisterChaseMoveIfNeeded();
                }
            }
        }

        public void RotateRight()
        {
            if (currentLocationController == null)
            {
                Debug.LogWarning("Cannot rotate right because there is no current location.");
                return;
            }

            if (currentLocationController.RotateView(1))
            {
                string previousViewId = CurrentViewId;
                CurrentViewId = currentLocationController.CurrentViewId;

                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SetCurrentPosition(CurrentLocationId, CurrentViewId);
                }
                else
                {
                    Debug.LogWarning("SaveManager instance is missing.");
                }

                if (previousViewId != CurrentViewId)
                {
                    RegisterChaseMoveIfNeeded();
                }
            }
        }

        public void MoveThroughDoor(string doorId)
        {
            if (string.IsNullOrEmpty(doorId))
            {
                Debug.LogWarning("Cannot move through an empty doorId.");
                return;
            }

            if (GameDataManager.Instance == null)
            {
                Debug.LogWarning("GameDataManager instance is missing.");
                return;
            }

            DoorRecord door = GameDataManager.Instance.GetDoorById(doorId);
            if (door == null)
            {
                Debug.LogWarning("Door not found: " + doorId);
                return;
            }

            if (!string.IsNullOrEmpty(door.fromLocationId) && door.fromLocationId != CurrentLocationId)
            {
                Debug.LogWarning("Door cannot be used from this location. Door: " + doorId + ", Current: " + CurrentLocationId);
                return;
            }

            if (!string.IsNullOrEmpty(door.fromViewId) && door.fromViewId != CurrentViewId)
            {
                Debug.LogWarning("Door cannot be used from this view. Door: " + doorId + ", Current: " + CurrentViewId);
                return;
            }

            if (!string.IsNullOrEmpty(door.toLocationId) && GetLocationController(door.toLocationId) == null)
            {
                Debug.LogWarning("Target LocationController not found for door: " + doorId + ", Target: " + door.toLocationId);
                return;
            }

            if (!CanMoveThroughDoor(door))
            {
                Debug.Log("Door is locked or requirements are not satisfied: " + doorId);
                return;
            }

            SetLocation(door.toLocationId, door.toViewId);

            if (door.staysUnlockedAfterOpen)
            {
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.MarkDoorOpened(door.doorId);
                }
                else
                {
                    Debug.LogWarning("SaveManager instance is missing.");
                }
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }
        }

        public void RefreshActiveView()
        {
            if (currentLocationController != null)
            {
                currentLocationController.RefreshViewVisibility();
            }
        }

        private bool CanMoveThroughDoor(DoorRecord door)
        {
            if (door == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(door.toLocationId))
            {
                Debug.LogWarning("Door has an empty toLocationId: " + door.doorId);
                return false;
            }

            if (SaveManager.Instance != null && SaveManager.Instance.IsDoorOpened(door.doorId))
            {
                return true;
            }

            bool hasItemRequirement = !string.IsNullOrEmpty(door.requiredItemId);
            bool hasPuzzleRequirement = !string.IsNullOrEmpty(door.requiredPuzzleId);

            if (!door.startsLocked && !hasItemRequirement && !hasPuzzleRequirement)
            {
                return true;
            }

            return IsDoorRequirementSatisfied(door);
        }

        private bool IsDoorRequirementSatisfied(DoorRecord door)
        {
            if (door == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(door.requiredPuzzleId))
            {
                if (SaveManager.Instance == null)
                {
                    Debug.LogWarning("SaveManager instance is missing.");
                    return false;
                }

                if (!SaveManager.Instance.IsPuzzleCompleted(door.requiredPuzzleId))
                {
                    Debug.Log("Required puzzle is not completed: " + door.requiredPuzzleId);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(door.requiredItemId))
            {
                if (InventoryManager.Instance == null)
                {
                    Debug.LogWarning("InventoryManager instance is missing.");
                    return false;
                }

                if (!InventoryManager.Instance.HasItem(door.requiredItemId))
                {
                    Debug.Log("Required item is not owned: " + door.requiredItemId);
                    return false;
                }

                if (!InventoryManager.Instance.TryUseSelectedItem(door.requiredItemId))
                {
                    Debug.Log("Required item is not selected or could not be used: " + door.requiredItemId);
                    return false;
                }
            }

            return true;
        }

        private void DeactivateAllLocations()
        {
            for (int i = 0; i < locationControllers.Count; i++)
            {
                if (locationControllers[i] != null)
                {
                    locationControllers[i].SetLocationActive(false);
                }
            }
        }

        private LocationController GetLocationController(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                return null;
            }

            if (locationMap.Count == 0)
            {
                BuildLocationMap();
            }

            LocationController controller;
            if (locationMap.TryGetValue(locationId, out controller))
            {
                return controller;
            }

            return null;
        }

        private string GetDefaultViewIdForLocation(string locationId, LocationController controller)
        {
            if (GameDataManager.Instance != null)
            {
                LocationRecord record = GameDataManager.Instance.GetLocationById(locationId);
                if (record != null && !string.IsNullOrEmpty(record.defaultViewId))
                {
                    return record.defaultViewId;
                }
            }

            if (controller != null)
            {
                if (!string.IsNullOrEmpty(controller.DefaultViewId))
                {
                    return controller.DefaultViewId;
                }

                if (controller.Views != null && controller.Views.Count > 0 && controller.Views[0] != null)
                {
                    return controller.Views[0].ViewId;
                }
            }

            return null;
        }

        private bool IsSameCurrentPosition(string locationId, string viewId)
        {
            return CurrentLocationId == locationId && CurrentViewId == viewId;
        }

        private void RegisterChaseMoveIfNeeded()
        {
            if (ChaseManager.Instance == null || !ChaseManager.Instance.IsChasing)
            {
                return;
            }

            if (suppressChaseMoveRegistration)
            {
                return;
            }

            if (HideManager.Instance != null && HideManager.Instance.IsHiding)
            {
                return;
            }

            ChaseManager.Instance.RegisterMove();
        }
    }
}
