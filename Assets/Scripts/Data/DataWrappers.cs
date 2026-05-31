// -----------------------------------------------------------------------------
// Codex comment pass: Location Record List
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\DataWrappers.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Location Record List data container whose field names intentionally match the project data files.
    public class LocationRecordList
    {
        // Stores the locations value used by this script's runtime or editor workflow.
        public List<LocationRecord> locations = new List<LocationRecord>();
    }

    [Serializable]
    // Serializable Door Record List data container whose field names intentionally match the project data files.
    public class DoorRecordList
    {
        // Stores the doors value used by this script's runtime or editor workflow.
        public List<DoorRecord> doors = new List<DoorRecord>();
    }

    [Serializable]
    // Serializable Item Record List data container whose field names intentionally match the project data files.
    public class ItemRecordList
    {
        // Stores the items value used by this script's runtime or editor workflow.
        public List<ItemRecord> items = new List<ItemRecord>();
    }

    [Serializable]
    // Serializable Puzzle Record List data container whose field names intentionally match the project data files.
    public class PuzzleRecordList
    {
        // Stores the puzzles value used by this script's runtime or editor workflow.
        public List<PuzzleRecord> puzzles = new List<PuzzleRecord>();
    }

    [Serializable]
    // Serializable Puzzle Answer Record List data container whose field names intentionally match the project data files.
    public class PuzzleAnswerRecordList
    {
        // Stores the answers value used by this script's runtime or editor workflow.
        public List<PuzzleAnswerRecord> answers = new List<PuzzleAnswerRecord>();
    }

    [Serializable]
    // Serializable Clue Record List data container whose field names intentionally match the project data files.
    public class ClueRecordList
    {
        // Stores the clues value used by this script's runtime or editor workflow.
        public List<ClueRecord> clues = new List<ClueRecord>();
    }

    [Serializable]
    // Serializable Symbol Record List data container whose field names intentionally match the project data files.
    public class SymbolRecordList
    {
        // Stores the symbols value used by this script's runtime or editor workflow.
        public List<SymbolRecord> symbols = new List<SymbolRecord>();
    }

    [Serializable]
    // Serializable Ghost Rule Record List data container whose field names intentionally match the project data files.
    public class GhostRuleRecordList
    {
        // Stores the ghost Rules value used by this script's runtime or editor workflow.
        public List<GhostRuleRecord> ghostRules = new List<GhostRuleRecord>();
    }

    [Serializable]
    // Serializable Game Settings Wrapper data container whose field names intentionally match the project data files.
    public class GameSettingsWrapper
    {
        // Stores the settings value used by this script's runtime or editor workflow.
        public GameSettingsRecord settings = new GameSettingsRecord();
    }

    [Serializable]
    // Serializable Audio Settings Record data container whose field names intentionally match the project data files.
    public class AudioSettingsRecord
    {
        // Stores the music Volume value used by this script's runtime or editor workflow.
        public float musicVolume = 0.55f;
        // Stores the ambience Volume value used by this script's runtime or editor workflow.
        public float ambienceVolume = 0.22f;
        // Stores the sfx Volume value used by this script's runtime or editor workflow.
        public float sfxVolume = 0.85f;
        // Stores the ui Volume value used by this script's runtime or editor workflow.
        public float uiVolume = 0.75f;
        // Stores the fade Seconds value used by this script's runtime or editor workflow.
        public float fadeSeconds = 0.75f;

        // Stores the title Music Path value used by this script's runtime or editor workflow.
        public string titleMusicPath;
        // Stores the gameplay Music Path value used by this script's runtime or editor workflow.
        public string gameplayMusicPath;
        // Stores the chase Music Path value used by this script's runtime or editor workflow.
        public string chaseMusicPath;
        // Stores the ending Music Path value used by this script's runtime or editor workflow.
        public string endingMusicPath;
        // Stores the title Ambience Path value used by this script's runtime or editor workflow.
        public string titleAmbiencePath;
        // Stores the room Ambience Path value used by this script's runtime or editor workflow.
        public string roomAmbiencePath;

        // Stores the door Move Sfx Path value used by this script's runtime or editor workflow.
        public string doorMoveSfxPath;
        // Stores the door Unlock Sfx Path value used by this script's runtime or editor workflow.
        public string doorUnlockSfxPath;
        // Stores the door Locked Sfx Path value used by this script's runtime or editor workflow.
        public string doorLockedSfxPath;
        // Stores the item Pickup Sfx Path value used by this script's runtime or editor workflow.
        public string itemPickupSfxPath;
        // Stores the chase Start Sfx Path value used by this script's runtime or editor workflow.
        public string chaseStartSfxPath;
        // Stores the game Over Impact Sfx Path value used by this script's runtime or editor workflow.
        public string gameOverImpactSfxPath;

        // Stores the ui Click Sfx Path value used by this script's runtime or editor workflow.
        public string uiClickSfxPath;
        // Stores the ui Confirm Sfx Path value used by this script's runtime or editor workflow.
        public string uiConfirmSfxPath;
        // Stores the ui Fail Sfx Path value used by this script's runtime or editor workflow.
        public string uiFailSfxPath;
    }

    [Serializable]
    // Serializable Audio Settings Wrapper data container whose field names intentionally match the project data files.
    public class AudioSettingsWrapper
    {
        // Stores the audio value used by this script's runtime or editor workflow.
        public AudioSettingsRecord audio = new AudioSettingsRecord();
    }
}
