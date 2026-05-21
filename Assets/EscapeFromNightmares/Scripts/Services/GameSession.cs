using System.Collections.Generic;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public sealed class GameSession
    {
        private readonly HashSet<string> inventory = new HashSet<string>();
        private readonly HashSet<string> flags = new HashSet<string>();

        public string CurrentRoomId { get; private set; }
        public string SelectedItemId { get; private set; }
        public bool MonsterEnabled { get; set; }

        public IReadOnlyCollection<string> InventoryItems => inventory;
        public IReadOnlyCollection<string> Flags => flags;

        public void Start(StageDefinition stage)
        {
            inventory.Clear();
            flags.Clear();
            SelectedItemId = string.Empty;
            MonsterEnabled = false;
            CurrentRoomId = stage.startRoomId;
        }

        public void MoveTo(string roomId)
        {
            CurrentRoomId = roomId;
        }

        public bool AddItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            return inventory.Add(itemId);
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

        public bool HasFlag(string flagId)
        {
            return string.IsNullOrWhiteSpace(flagId) || flags.Contains(flagId);
        }
    }
}
