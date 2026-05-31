// -----------------------------------------------------------------------------
// Codex comment pass: Location Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\LocationManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Location Manager system, keeping shared state and events behind one access point.
    public class LocationManager : Singleton<LocationManager>
    {
        [SerializeField] private Transform locationRoot;
        [SerializeField] private List<LocationController> locationControllers = new List<LocationController>();
        [SerializeField] private bool collectLocationsFromRoot = true;
        [SerializeField] private string startingLocationId;
        [SerializeField] private string startingViewId;

        private readonly Dictionary<string, LocationController> locationMap = new Dictionary<string, LocationController>();
        // Stores the current Location Controller value used by this script's runtime or editor workflow.
        private LocationController currentLocationController;
        // Stores the has Initialized value used by this script's runtime or editor workflow.
        private bool hasInitialized;
        // Stores the suppress Chase Move Registration value used by this script's runtime or editor workflow.
        private bool suppressChaseMoveRegistration;

        // Stores the Current Location Id value used by this script's runtime or editor workflow.
        public string CurrentLocationId;
        // Stores the Current View Id value used by this script's runtime or editor workflow.
        public string CurrentViewId;

        // Caches required component references and prepares this object before other startup code runs.
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

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            if (!hasInitialized)
            {
                ApplySavedPositionOrStartingLocation();
            }
        }

        // Performs the Register Location operation while keeping its implementation details inside this script.
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

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
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

        // Performs the Collect Locations From Root operation while keeping its implementation details inside this script.
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

        // Initializes local UI and state from an external record before the player can interact with it.
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

        // Applies calculated settings to Unity components or runtime state.
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

        // Returns the scene to its authored first playable location without reading saved position data.
        public void ResetToStartingLocation()
        {
            bool previousSuppressChaseMoveRegistration = suppressChaseMoveRegistration;
            suppressChaseMoveRegistration = true;

            if (currentLocationController != null)
            {
                currentLocationController.SetLocationActive(false);
            }

            currentLocationController = null;
            CurrentLocationId = string.Empty;
            CurrentViewId = string.Empty;
            hasInitialized = false;

            BuildLocationMap();
            DeactivateAllLocations();
            InitializeStartingLocation();

            suppressChaseMoveRegistration = previousSuppressChaseMoveRegistration;
        }

        // Performs the Try Apply Saved Position operation while keeping its implementation details inside this script.
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

        // Stores an incoming value and updates any dependent visual or runtime state.
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

        // Stores an incoming value and updates any dependent visual or runtime state.
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

        // Performs the Rotate Left operation while keeping its implementation details inside this script.
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

        // Performs the Rotate Right operation while keeping its implementation details inside this script.
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

        // Performs the Move Through Door operation while keeping its implementation details inside this script.
        public bool MoveThroughDoor(string doorId)
        {
            if (string.IsNullOrEmpty(doorId))
            {
                Debug.LogWarning("Cannot move through an empty doorId.");
                return false;
            }

            if (GameDataManager.Instance == null)
            {
                Debug.LogWarning("GameDataManager instance is missing.");
                return false;
            }

            DoorRecord door = GameDataManager.Instance.GetDoorById(doorId);
            if (door == null)
            {
                Debug.LogWarning("Door not found: " + doorId);
                return false;
            }

            if (!string.IsNullOrEmpty(door.fromLocationId) && door.fromLocationId != CurrentLocationId)
            {
                Debug.LogWarning("Door cannot be used from this location. Door: " + doorId + ", Current: " + CurrentLocationId);
                return false;
            }

            if (!string.IsNullOrEmpty(door.fromViewId) && door.fromViewId != CurrentViewId)
            {
                Debug.LogWarning("Door cannot be used from this view. Door: " + doorId + ", Current: " + CurrentViewId);
                return false;
            }

            if (!string.IsNullOrEmpty(door.toLocationId) && GetLocationController(door.toLocationId) == null)
            {
                Debug.LogWarning("Target LocationController not found for door: " + doorId + ", Target: " + door.toLocationId);
                return false;
            }

            if (!CanMoveThroughDoor(door))
            {
                Debug.Log("Door is locked or requirements are not satisfied: " + doorId);
                return false;
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

            return true;
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        public void RefreshActiveView()
        {
            if (currentLocationController != null)
            {
                currentLocationController.RefreshViewVisibility();
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Performs the Deactivate All Locations operation while keeping its implementation details inside this script.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsSameCurrentPosition(string locationId, string viewId)
        {
            return CurrentLocationId == locationId && CurrentViewId == viewId;
        }

        // Performs the Register Chase Move If Needed operation while keeping its implementation details inside this script.
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
