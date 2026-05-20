using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class InteractionSystem : MonoBehaviour
    {
        [SerializeField] private RoomSystem roomSystem;
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private PuzzleSystem puzzleSystem;
        [SerializeField] private EventRuntimeSystem eventRuntimeSystem;
        [SerializeField] private SoundRuntimeSystem soundRuntimeSystem;
        [SerializeField] private HidingRuntimeSystem hidingRuntimeSystem;

        private void Awake()
        {
            roomSystem ??= FindFirstObjectByType<RoomSystem>();
            inventorySystem ??= FindFirstObjectByType<InventorySystem>();
            puzzleSystem ??= FindFirstObjectByType<PuzzleSystem>();
            eventRuntimeSystem ??= FindFirstObjectByType<EventRuntimeSystem>();
            soundRuntimeSystem ??= FindFirstObjectByType<SoundRuntimeSystem>();
            hidingRuntimeSystem ??= FindFirstObjectByType<HidingRuntimeSystem>();
        }

        public void HandleInteractableClicked(InteractableDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            if (!RequirementsAreMet(definition))
            {
                return;
            }

            switch (definition.InteractableType)
            {
                case InteractableType.Door:
                case InteractableType.ScreenEdge:
                case InteractableType.LockedDoor:
                case InteractableType.EscapeDoor:
                    if (definition.PuzzleDefinition != null)
                    {
                        puzzleSystem?.OpenPuzzle(definition.PuzzleDefinition);
                        break;
                    }

                    TryMove(definition);
                    break;
                case InteractableType.ItemPickup:
                    TryAcquireItem(definition);
                    break;
                case InteractableType.PuzzleObject:
                    if (definition.PuzzleDefinition != null)
                    {
                        puzzleSystem?.OpenPuzzle(definition.PuzzleDefinition);
                    }
                    else
                    {
                        eventRuntimeSystem?.ExecuteEvent(definition.EventId);
                    }

                    break;
                case InteractableType.HideSpot:
                    hidingRuntimeSystem?.EnterHideSpot(definition.InteractableId);
                    break;
                default:
                    Debug.Log($"Clicked interactable '{definition.InteractableId}' of type '{definition.InteractableType}'.");
                    eventRuntimeSystem?.ExecuteEvent(definition.EventId);
                    break;
            }
        }

        private void TryMove(InteractableDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(definition.TargetRoomId))
            {
                Debug.Log($"Interactable '{definition.InteractableId}' has no target room yet.");
                return;
            }

            roomSystem?.ChangeRoom(definition.TargetRoomId);
            soundRuntimeSystem?.PlayDoorOpen();
            eventRuntimeSystem?.ExecuteEvent(definition.EventId);
        }

        private void TryAcquireItem(InteractableDefinition definition)
        {
            if (definition.ItemReward == null)
            {
                Debug.Log($"Interactable '{definition.InteractableId}' has no item reward yet.");
                return;
            }

            if (inventorySystem == null)
            {
                Debug.LogWarning("Cannot acquire item because InventorySystem is missing.");
                return;
            }

            if (inventorySystem.AddItem(definition.ItemReward))
            {
                Debug.Log($"Acquired item '{definition.ItemReward.ItemId}'.");
                soundRuntimeSystem?.PlayItemPickup();
                eventRuntimeSystem?.ExecuteEvent(definition.EventId);
            }
        }

        private bool RequirementsAreMet(InteractableDefinition definition)
        {
            bool hasItemRequirements = false;
            bool selectedRequiredItem = false;

            foreach (InteractableRequirement requirement in definition.Requirements)
            {
                if (!string.IsNullOrWhiteSpace(requirement.requiredItemId))
                {
                    hasItemRequirements = true;

                    if (inventorySystem == null || !inventorySystem.HasItem(requirement.requiredItemId))
                    {
                        Debug.Log($"Interactable '{definition.InteractableId}' requires item '{requirement.requiredItemId}'.");
                        return false;
                    }

                    if (inventorySystem.IsSelected(requirement.requiredItemId))
                    {
                        selectedRequiredItem = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(requirement.requiredFlag))
                {
                    if (eventRuntimeSystem == null || !eventRuntimeSystem.HasFlag(requirement.requiredFlag))
                    {
                        Debug.Log($"Interactable '{definition.InteractableId}' requires flag '{requirement.requiredFlag}'.");
                        return false;
                    }
                }
            }

            if (hasItemRequirements && !selectedRequiredItem)
            {
                Debug.Log($"Interactable '{definition.InteractableId}' requires selecting one of its required items.");
                return false;
            }

            ConsumeSelectedRequirement(definition);
            return true;
        }

        private void ConsumeSelectedRequirement(InteractableDefinition definition)
        {
            if (inventorySystem == null || inventorySystem.SelectedItem == null)
            {
                return;
            }

            foreach (InteractableRequirement requirement in definition.Requirements)
            {
                if (requirement.consumeItem && inventorySystem.IsSelected(requirement.requiredItemId))
                {
                    inventorySystem.TryConsumeSelected(requirement.requiredItemId);
                    return;
                }

            }
        }
    }
}
