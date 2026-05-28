using System;
using System.Collections.Generic;

namespace EscapeFromNightmare
{
    [Serializable]
    public class LocationRecordList
    {
        public List<LocationRecord> locations = new List<LocationRecord>();
    }

    [Serializable]
    public class DoorRecordList
    {
        public List<DoorRecord> doors = new List<DoorRecord>();
    }

    [Serializable]
    public class ItemRecordList
    {
        public List<ItemRecord> items = new List<ItemRecord>();
    }

    [Serializable]
    public class PuzzleRecordList
    {
        public List<PuzzleRecord> puzzles = new List<PuzzleRecord>();
    }

    [Serializable]
    public class PuzzleAnswerRecordList
    {
        public List<PuzzleAnswerRecord> answers = new List<PuzzleAnswerRecord>();
    }

    [Serializable]
    public class ClueRecordList
    {
        public List<ClueRecord> clues = new List<ClueRecord>();
    }

    [Serializable]
    public class SymbolRecordList
    {
        public List<SymbolRecord> symbols = new List<SymbolRecord>();
    }

    [Serializable]
    public class GhostRuleRecordList
    {
        public List<GhostRuleRecord> ghostRules = new List<GhostRuleRecord>();
    }

    [Serializable]
    public class GameSettingsWrapper
    {
        public GameSettingsRecord settings = new GameSettingsRecord();
    }
}
