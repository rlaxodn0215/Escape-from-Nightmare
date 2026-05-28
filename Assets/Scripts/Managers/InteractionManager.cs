using UnityEngine;

namespace EscapeFromNightmare
{
    public class InteractionManager : Singleton<InteractionManager>
    {
        public void HandleClick(ClickableButton button)
        {
            if (button == null)
            {
                Debug.LogWarning("Clicked button is null.");
                return;
            }

            switch (button.ClickableType)
            {
                case ClickableType.ExamineImage:
                    HandleExamineImage(button);
                    break;
                case ClickableType.Puzzle:
                    HandlePuzzle(button);
                    break;
                case ClickableType.Door:
                    HandleDoor(button);
                    break;
                case ClickableType.HidePoint:
                    HandleHidePoint(button);
                    break;
                case ClickableType.PickupItem:
                    HandlePickupItem(button);
                    break;
                case ClickableType.UseItemTarget:
                    HandleUseItemTarget(button);
                    break;
                case ClickableType.FinalDoor:
                    HandleFinalDoor(button);
                    break;
                default:
                    Debug.LogWarning("Unhandled clickable type: " + button.ClickableType);
                    break;
            }
        }

        private void HandleExamineImage(ClickableButton button)
        {
            if (button == null)
            {
                return;
            }

            string clueId = button.LinkedClueImageId;
            if (string.IsNullOrEmpty(clueId))
            {
                clueId = button.TargetObjectId;
            }

            if (string.IsNullOrEmpty(clueId))
            {
                clueId = button.ClickableId;
            }

            if (string.IsNullOrEmpty(clueId))
            {
                Debug.LogWarning("ExamineImage clickable needs linkedClueImageId, targetObjectId, or clickableId.");
                return;
            }

            if (ClueImageManager.Instance == null)
            {
                Debug.LogWarning("ClueImageManager instance is missing.");
                return;
            }

            ClueImageManager.Instance.ShowClueImage(clueId);
        }

        private void HandlePuzzle(ClickableButton button)
        {
            if (string.IsNullOrEmpty(button.LinkedPuzzleId))
            {
                Debug.LogWarning("Puzzle clickable has an empty linkedPuzzleId: " + button.ClickableId);
                return;
            }

            Debug.Log("Handle puzzle: " + button.LinkedPuzzleId);

            if (PuzzleManager.Instance == null)
            {
                Debug.LogWarning("PuzzleManager instance is missing.");
                return;
            }

            PuzzleManager.Instance.OpenPuzzle(button.LinkedPuzzleId);
        }

        private void HandleDoor(ClickableButton button)
        {
            Debug.Log("Handle door: " + button.LinkedDoorId);

            if (LocationManager.Instance == null)
            {
                Debug.LogWarning("LocationManager instance is missing.");
                return;
            }

            if (!string.IsNullOrEmpty(button.LinkedDoorId))
            {
                LocationManager.Instance.MoveThroughDoor(button.LinkedDoorId);
                return;
            }

            if (!string.IsNullOrEmpty(button.LinkedLocationId))
            {
                LocationManager.Instance.SetLocation(button.LinkedLocationId, button.LinkedViewId);
                return;
            }

            Debug.LogWarning("Door clickable needs linkedDoorId or linkedLocationId: " + button.ClickableId);
        }

        private void HandleHidePoint(ClickableButton button)
        {
            if (button == null)
            {
                return;
            }

            HidePointController hidePointController = button.GetComponent<HidePointController>();
            if (hidePointController != null && !hidePointController.Usable)
            {
                Debug.Log("Hide point is not usable: " + hidePointController.HidePointId);
                return;
            }

            string hidePointId = hidePointController != null ? hidePointController.GetResolvedHidePointId() : string.Empty;
            if (string.IsNullOrEmpty(hidePointId))
            {
                hidePointId = button.TargetObjectId;
            }

            if (string.IsNullOrEmpty(hidePointId))
            {
                hidePointId = button.ClickableId;
            }

            if (string.IsNullOrEmpty(hidePointId))
            {
                hidePointId = button.gameObject.name;
            }

            if (HideManager.Instance == null)
            {
                Debug.LogWarning("HideManager instance is missing.");
                return;
            }

            Debug.Log("Handle hide point: " + hidePointId);
            HideManager.Instance.EnterHidePoint(hidePointId);
        }

        private void HandlePickupItem(ClickableButton button)
        {
            if (button == null)
            {
                return;
            }

            string itemId = button.LinkedItemId;
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogWarning("PickupItem clickable has an empty linkedItemId: " + button.ClickableId);
                return;
            }

            Debug.Log("Handle pickup item: " + itemId);

            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryManager instance is missing.");
                return;
            }

            bool added = InventoryManager.Instance.TryAddItem(itemId);
            PickupItemController pickupItemController = button.GetComponent<PickupItemController>();

            if (added)
            {
                if (pickupItemController != null)
                {
                    pickupItemController.MarkPickedUp();
                }
                else
                {
                    button.gameObject.SetActive(false);
                }

                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SaveGame();
                }
                else
                {
                    Debug.LogWarning("SaveManager instance is missing.");
                }

                return;
            }

            Debug.Log("Item was not added, possibly already owned: " + itemId);
            if (pickupItemController != null)
            {
                pickupItemController.RefreshVisibility();
            }
        }

        private void HandleUseItemTarget(ClickableButton button)
        {
            if (button == null)
            {
                return;
            }

            string requiredItemId = button.RequiredItemId;
            if (string.IsNullOrEmpty(requiredItemId))
            {
                Debug.LogWarning("UseItemTarget clickable has an empty requiredItemId: " + button.ClickableId);
                return;
            }

            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryManager instance is missing.");
                return;
            }

            if (!InventoryManager.Instance.TryUseSelectedItem(requiredItemId))
            {
                Debug.Log("No selected item or selected item does not match required item: " + requiredItemId);
                return;
            }

            if (!string.IsNullOrEmpty(button.LinkedPuzzleId))
            {
                if (PuzzleManager.Instance != null)
                {
                    PuzzleManager.Instance.OpenPuzzle(button.LinkedPuzzleId);
                }
                else
                {
                    Debug.LogWarning("PuzzleManager instance is missing.");
                }
            }

            if (!string.IsNullOrEmpty(button.LinkedClueImageId))
            {
                if (ClueImageManager.Instance != null)
                {
                    ClueImageManager.Instance.UnlockClue(button.LinkedClueImageId);
                    ClueImageManager.Instance.ShowClueImage(button.LinkedClueImageId);
                }
                else
                {
                    Debug.LogWarning("ClueImageManager instance is missing.");
                }
            }

            if (!string.IsNullOrEmpty(button.LinkedDoorId))
            {
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.MarkDoorOpened(button.LinkedDoorId);
                }
                else
                {
                    Debug.LogWarning("SaveManager instance is missing.");
                }
            }

            if (!string.IsNullOrEmpty(button.LinkedItemId) && button.LinkedItemId != requiredItemId)
            {
                InventoryManager.Instance.TryAddItem(button.LinkedItemId);
            }

            if (!string.IsNullOrEmpty(button.TargetObjectId))
            {
                Debug.Log("TODO: Apply item-use result to target object: " + button.TargetObjectId);
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

        private void HandleFinalDoor(ClickableButton button)
        {
            if (button == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(button.RequiredItemId))
            {
                if (InventoryManager.Instance == null)
                {
                    Debug.LogWarning("InventoryManager instance is missing.");
                    return;
                }

                if (!InventoryManager.Instance.TryUseSelectedItem(button.RequiredItemId))
                {
                    Debug.Log("Final door requirement is not satisfied: " + button.RequiredItemId);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(button.LinkedPuzzleId))
            {
                if (PuzzleManager.Instance != null)
                {
                    PuzzleManager.Instance.OpenPuzzle(button.LinkedPuzzleId);
                }
                else
                {
                    Debug.LogWarning("PuzzleManager instance is missing.");
                }

                return;
            }

            if (!string.IsNullOrEmpty(button.LinkedDoorId))
            {
                if (LocationManager.Instance != null)
                {
                    LocationManager.Instance.MoveThroughDoor(button.LinkedDoorId);
                }
                else
                {
                    Debug.LogWarning("LocationManager instance is missing.");
                }

                return;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnterEnding();
            }
            else
            {
                Debug.LogWarning("GameManager instance is missing.");
            }
        }
    }
}
