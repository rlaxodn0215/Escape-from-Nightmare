using System.Collections.Generic;
using System.Linq;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Fast ID-based lookup view over a loaded stage.
    /// </summary>
    public sealed class StageLookup
    {
        private readonly Dictionary<string, RoomDefinition> rooms = new Dictionary<string, RoomDefinition>();
        private readonly Dictionary<string, ItemDefinition> items = new Dictionary<string, ItemDefinition>();
        private readonly Dictionary<string, PuzzleDefinition> puzzles = new Dictionary<string, PuzzleDefinition>();

        public StageLookup(StageDefinition stage)
        {
            Stage = stage;
            if (stage == null)
            {
                return;
            }

            AddRooms(stage.rooms);
            AddItems(stage.items);
            AddPuzzles(stage.puzzles);
        }

        public StageDefinition Stage { get; }

        public bool TryGetRoom(string roomId, out RoomDefinition room)
        {
            return rooms.TryGetValue(roomId, out room);
        }

        public bool TryGetItem(string itemId, out ItemDefinition item)
        {
            return items.TryGetValue(itemId, out item);
        }

        public bool TryGetPuzzle(string puzzleId, out PuzzleDefinition puzzle)
        {
            return puzzles.TryGetValue(puzzleId, out puzzle);
        }

        public RoomDefinition RequireRoom(string roomId)
        {
            return rooms[roomId];
        }

        public InteractableDefinition FindInteractable(string interactableId)
        {
            if (string.IsNullOrWhiteSpace(interactableId))
            {
                return null;
            }

            foreach (var room in rooms.Values)
            {
                var found = room.faces?
                    .SelectMany(face => face.interactables ?? new InteractableDefinition[0])
                    .FirstOrDefault(interactable => interactable != null && interactable.interactableId == interactableId);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private void AddRooms(IEnumerable<RoomDefinition> definitions)
        {
            if (definitions == null)
            {
                return;
            }

            foreach (var room in definitions)
            {
                if (room != null && !string.IsNullOrWhiteSpace(room.roomId))
                {
                    rooms[room.roomId] = room;
                }
            }
        }

        private void AddItems(IEnumerable<ItemDefinition> definitions)
        {
            if (definitions == null)
            {
                return;
            }

            foreach (var item in definitions)
            {
                if (item != null && !string.IsNullOrWhiteSpace(item.itemId))
                {
                    items[item.itemId] = item;
                }
            }
        }

        private void AddPuzzles(IEnumerable<PuzzleDefinition> definitions)
        {
            if (definitions == null)
            {
                return;
            }

            foreach (var puzzle in definitions)
            {
                if (puzzle != null && !string.IsNullOrWhiteSpace(puzzle.puzzleId))
                {
                    puzzles[puzzle.puzzleId] = puzzle;
                }
            }
        }
    }
}
