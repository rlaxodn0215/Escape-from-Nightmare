// -----------------------------------------------------------------------------
// Codex comment pass: Interaction Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\InteractionManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Interaction Manager system, keeping shared state and events behind one access point.
    public class InteractionManager : Singleton<InteractionManager>
    {
        // Performs the Handle Click operation while keeping its implementation details inside this script.
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

        // Performs the Handle Examine Image operation while keeping its implementation details inside this script.
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
                PlayUiFail();
                Debug.LogWarning("ExamineImage clickable needs linkedClueImageId, targetObjectId, or clickableId.");
                return;
            }

            if (ClueImageManager.Instance == null)
            {
                PlayUiFail();
                Debug.LogWarning("ClueImageManager instance is missing.");
                return;
            }

            ClueImageManager.Instance.ShowClueImage(clueId);
        }

        // Performs the Handle Puzzle operation while keeping its implementation details inside this script.
        private void HandlePuzzle(ClickableButton button)
        {
            if (GameDataManager.Instance != null && GameDataManager.Instance.DisablePuzzles)
            {
                Debug.Log("Puzzle click ignored because puzzles are disabled for layout testing: " + button.ClickableId);
                return;
            }

            if (string.IsNullOrEmpty(button.LinkedPuzzleId))
            {
                PlayUiFail();
                Debug.LogWarning("Puzzle clickable has an empty linkedPuzzleId: " + button.ClickableId);
                return;
            }

            Debug.Log("Handle puzzle: " + button.LinkedPuzzleId);

            if (PuzzleManager.Instance == null)
            {
                PlayUiFail();
                Debug.LogWarning("PuzzleManager instance is missing.");
                return;
            }

            PuzzleManager.Instance.OpenPuzzle(button.LinkedPuzzleId);
        }

        // Performs the Handle Door operation while keeping its implementation details inside this script.
        private void HandleDoor(ClickableButton button)
        {
            Debug.Log("Handle door: " + button.LinkedDoorId);

            if (LocationManager.Instance == null)
            {
                PlayUiFail();
                Debug.LogWarning("LocationManager instance is missing.");
                return;
            }

            if (!string.IsNullOrEmpty(button.LinkedDoorId))
            {
                PlayMovementTransition(() =>
                {
                    if (LocationManager.Instance.MoveThroughDoor(button.LinkedDoorId))
                    {
                        PlaySfx(AudioCue.DoorMove);
                    }
                    else
                    {
                        PlaySfx(AudioCue.DoorLocked);
                    }
                });

                return;
            }

            if (!string.IsNullOrEmpty(button.LinkedLocationId))
            {
                PlayMovementTransition(() =>
                {
                    LocationManager.Instance.SetLocation(button.LinkedLocationId, button.LinkedViewId);
                    PlaySfx(AudioCue.DoorMove);
                });
                return;
            }

            PlayUiFail();
            Debug.LogWarning("Door clickable needs linkedDoorId or linkedLocationId: " + button.ClickableId);
        }

        // Performs the Handle Hide Point operation while keeping its implementation details inside this script.
        private void HandleHidePoint(ClickableButton button)
        {
            if (GameDataManager.Instance != null && GameDataManager.Instance.DisableHiding)
            {
                Debug.Log("Hide point ignored because hiding is disabled for layout testing: " + (button != null ? button.ClickableId : string.Empty));
                return;
            }

            if (button == null)
            {
                return;
            }

            HidePointController hidePointController = button.GetComponent<HidePointController>();
            if (hidePointController != null && !hidePointController.Usable)
            {
                PlayUiFail();
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
                PlayUiFail();
                Debug.LogWarning("HideManager instance is missing.");
                return;
            }

            Debug.Log("Handle hide point: " + hidePointId);
            PlayMovementTransition(() => HideManager.Instance.EnterHidePoint(hidePointId));
        }

        // Performs the Handle Pickup Item operation while keeping its implementation details inside this script.
        private void HandlePickupItem(ClickableButton button)
        {
            if (button == null)
            {
                return;
            }

            string itemId = button.LinkedItemId;
            if (string.IsNullOrEmpty(itemId))
            {
                PlayUiFail();
                Debug.LogWarning("PickupItem clickable has an empty linkedItemId: " + button.ClickableId);
                return;
            }

            Debug.Log("Handle pickup item: " + itemId);

            if (InventoryManager.Instance == null)
            {
                PlayUiFail();
                Debug.LogWarning("InventoryManager instance is missing.");
                return;
            }

            bool added = InventoryManager.Instance.TryAddItem(itemId);
            PickupItemController pickupItemController = button.GetComponent<PickupItemController>();

            if (added)
            {
                PlaySfx(AudioCue.ItemPickup);

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
            PlayUiFail();
            if (pickupItemController != null)
            {
                pickupItemController.RefreshVisibility();
            }
        }

        // Performs the Handle Use Item Target operation while keeping its implementation details inside this script.
        private void HandleUseItemTarget(ClickableButton button)
        {
            if (button == null)
            {
                return;
            }

            string requiredItemId = button.RequiredItemId;
            if (string.IsNullOrEmpty(requiredItemId))
            {
                PlayUiFail();
                Debug.LogWarning("UseItemTarget clickable has an empty requiredItemId: " + button.ClickableId);
                return;
            }

            if (InventoryManager.Instance == null)
            {
                PlayUiFail();
                Debug.LogWarning("InventoryManager instance is missing.");
                return;
            }

            if (!InventoryManager.Instance.TryUseSelectedItem(requiredItemId))
            {
                PlayUiFail();
                Debug.Log("No selected item or selected item does not match required item: " + requiredItemId);
                return;
            }

            PlayUiConfirm();

            if (!string.IsNullOrEmpty(button.LinkedPuzzleId))
            {
                if (PuzzleManager.Instance != null)
                {
                    PuzzleManager.Instance.OpenPuzzle(button.LinkedPuzzleId);
                }
                else
                {
                    PlayUiFail();
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
                    PlayUiFail();
                    Debug.LogWarning("ClueImageManager instance is missing.");
                }
            }

            if (!string.IsNullOrEmpty(button.LinkedDoorId))
            {
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.MarkDoorOpened(button.LinkedDoorId);
                    PlaySfx(AudioCue.DoorUnlock);
                }
                else
                {
                    PlayUiFail();
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

        // Performs the Handle Final Door operation while keeping its implementation details inside this script.
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
                    PlayUiFail();
                    Debug.LogWarning("InventoryManager instance is missing.");
                    return;
                }

                if (!InventoryManager.Instance.TryUseSelectedItem(button.RequiredItemId))
                {
                    PlaySfx(AudioCue.DoorLocked);
                    Debug.Log("Final door requirement is not satisfied: " + button.RequiredItemId);
                    return;
                }

                PlaySfx(AudioCue.DoorUnlock);
            }

            if (!string.IsNullOrEmpty(button.LinkedPuzzleId))
            {
                if (PuzzleManager.Instance != null)
                {
                    PuzzleManager.Instance.OpenPuzzle(button.LinkedPuzzleId);
                }
                else
                {
                    PlayUiFail();
                    Debug.LogWarning("PuzzleManager instance is missing.");
                }

                return;
            }

            if (!string.IsNullOrEmpty(button.LinkedDoorId))
            {
                if (LocationManager.Instance != null)
                {
                    PlayMovementTransition(() =>
                    {
                        if (LocationManager.Instance.MoveThroughDoor(button.LinkedDoorId))
                        {
                            PlaySfx(AudioCue.DoorMove);
                        }
                        else
                        {
                            PlaySfx(AudioCue.DoorLocked);
                        }
                    });
                }
                else
                {
                    PlayUiFail();
                    Debug.LogWarning("LocationManager instance is missing.");
                }

                return;
            }

            if (GameManager.Instance != null)
            {
                PlayUiConfirm();
                GameManager.Instance.EnterEnding();
            }
            else
            {
                PlayUiFail();
                Debug.LogWarning("GameManager instance is missing.");
            }
        }

        // Performs the Play Sfx operation while keeping its implementation details inside this script.
        private void PlaySfx(AudioCue cue)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(cue);
            }
        }

        private void PlayMovementTransition(System.Action action)
        {
            if (ScreenFadeManager.Instance != null)
            {
                ScreenFadeManager.Instance.PlayTransition(action);
                return;
            }

            if (action != null)
            {
                action();
            }
        }

        // Performs the Play Ui Confirm operation while keeping its implementation details inside this script.
        private void PlayUiConfirm()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiConfirm);
            }
        }

        // Performs the Play Ui Fail operation while keeping its implementation details inside this script.
        private void PlayUiFail()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiFail);
            }
        }
    }
}
