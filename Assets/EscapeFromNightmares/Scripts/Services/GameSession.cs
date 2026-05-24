using System.Collections.Generic;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public sealed class GameSession
    {
        private readonly HashSet<string> inventory = new HashSet<string>();
        private readonly HashSet<string> flags = new HashSet<string>();
        private readonly HashSet<string> solvedPuzzleIds = new HashSet<string>();
        private readonly HashSet<string> usedInteractableIds = new HashSet<string>();

        public string CurrentRoomId { get; private set; }
        public RoomFaceDirection CurrentFaceDirection { get; private set; } = RoomFaceDirection.North;
        public string SelectedItemId { get; private set; }
        public bool MonsterEnabled { get; set; }

        public IReadOnlyCollection<string> InventoryItems => inventory;
        public IReadOnlyCollection<string> Flags => flags;
        public IReadOnlyCollection<string> SolvedPuzzleIds => solvedPuzzleIds;
        public IReadOnlyCollection<string> UsedInteractableIds => usedInteractableIds;

        public void Start(StageDefinition stage)
        {
            inventory.Clear();
            flags.Clear();
            solvedPuzzleIds.Clear();
            usedInteractableIds.Clear();
            SelectedItemId = string.Empty;
            MonsterEnabled = false;
            CurrentRoomId = stage.startRoomId;
            CurrentFaceDirection = RoomFaceDirection.North;
        }

        public void MoveTo(string roomId)
        {
            CurrentRoomId = roomId;
            CurrentFaceDirection = RoomFaceDirection.North;
        }

        public void RotateFace(int offset)
        {
            CurrentFaceDirection = (RoomFaceDirection)(((int)CurrentFaceDirection + offset + 4) % 4);
        }

        public void SetFaceDirection(RoomFaceDirection direction)
        {
            CurrentFaceDirection = direction;
        }

        public bool AddItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            return inventory.Add(itemId);
        }

        public bool RemoveItem(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && inventory.Remove(itemId);
        }

        public bool HasItem(string itemId)
        {
            return string.IsNullOrWhiteSpace(itemId) || inventory.Contains(itemId);
        }

        public void SelectItem(string itemId)
        {
            SelectedItemId = inventory.Contains(itemId) ? itemId : string.Empty;
        }

        public void SetFlag(string flagId)
        {
            if (!string.IsNullOrWhiteSpace(flagId))
            {
                flags.Add(flagId);
            }
        }

        public void ClearFlag(string flagId)
        {
            if (!string.IsNullOrWhiteSpace(flagId))
            {
                flags.Remove(flagId);
            }
        }

        public bool HasFlag(string flagId)
        {
            return string.IsNullOrWhiteSpace(flagId) || flags.Contains(flagId);
        }

        public void MarkPuzzleSolved(string puzzleId)
        {
            if (!string.IsNullOrWhiteSpace(puzzleId))
            {
                solvedPuzzleIds.Add(puzzleId);
            }
        }

        public bool HasSolvedPuzzle(string puzzleId)
        {
            return string.IsNullOrWhiteSpace(puzzleId) || solvedPuzzleIds.Contains(puzzleId);
        }

        public void MarkInteractableUsed(string interactableId)
        {
            if (!string.IsNullOrWhiteSpace(interactableId))
            {
                usedInteractableIds.Add(interactableId);
            }
        }

        public bool HasUsedInteractable(string interactableId)
        {
            return !string.IsNullOrWhiteSpace(interactableId) && usedInteractableIds.Contains(interactableId);
        }
    }

    public sealed class FlagService
    {
        private readonly GameSession session;

        public FlagService(GameSession session)
        {
            this.session = session;
        }

        public void Set(string flagId)
        {
            session.SetFlag(flagId);
        }

        public void Clear(string flagId)
        {
            session.ClearFlag(flagId);
        }

        public bool Has(string flagId)
        {
            return session.HasFlag(flagId);
        }

        public bool AllFlags(params string[] flagIds)
        {
            return All(flagIds, session.HasFlag);
        }

        public bool AnyFlag(params string[] flagIds)
        {
            if (flagIds == null || flagIds.Length == 0)
            {
                return true;
            }

            foreach (var flagId in flagIds)
            {
                if (session.HasFlag(flagId))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ConditionsMet(ConditionDefinition conditions)
        {
            if (conditions == null)
            {
                return true;
            }

            return All(conditions.requiredItemIds, session.HasItem)
                && All(conditions.requiredFlagIds, session.HasFlag)
                && AnyFlag(conditions.anyFlagIds)
                && None(conditions.forbiddenFlagIds, session.HasFlag)
                && All(conditions.solvedPuzzleIds, session.HasSolvedPuzzle);
        }

        private static bool All(string[] ids, System.Func<string, bool> predicate)
        {
            if (ids == null)
            {
                return true;
            }

            foreach (var id in ids)
            {
                if (!predicate(id))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool None(string[] ids, System.Func<string, bool> predicate)
        {
            if (ids == null)
            {
                return true;
            }

            foreach (var id in ids)
            {
                if (!string.IsNullOrWhiteSpace(id) && predicate(id))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
