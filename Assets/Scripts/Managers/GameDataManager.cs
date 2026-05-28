using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EscapeFromNightmare
{
    public class GameDataManager : Singleton<GameDataManager>
    {
        private List<LocationRecord> locations = new List<LocationRecord>();
        private List<DoorRecord> doors = new List<DoorRecord>();
        private List<ItemRecord> items = new List<ItemRecord>();
        private List<PuzzleRecord> puzzles = new List<PuzzleRecord>();
        private List<PuzzleAnswerRecord> puzzleAnswers = new List<PuzzleAnswerRecord>();
        private List<ClueRecord> clues = new List<ClueRecord>();
        private List<SymbolRecord> symbols = new List<SymbolRecord>();
        private List<GhostRuleRecord> ghostRules = new List<GhostRuleRecord>();
        private GameSettingsRecord settings = new GameSettingsRecord();

        public IReadOnlyList<LocationRecord> Locations
        {
            get { return locations; }
        }

        public IReadOnlyList<DoorRecord> Doors
        {
            get { return doors; }
        }

        public IReadOnlyList<ItemRecord> Items
        {
            get { return items; }
        }

        public IReadOnlyList<PuzzleRecord> Puzzles
        {
            get { return puzzles; }
        }

        public IReadOnlyList<PuzzleAnswerRecord> PuzzleAnswers
        {
            get { return puzzleAnswers; }
        }

        public IReadOnlyList<ClueRecord> Clues
        {
            get { return clues; }
        }

        public IReadOnlyList<SymbolRecord> Symbols
        {
            get { return symbols; }
        }

        public IReadOnlyList<GhostRuleRecord> GhostRules
        {
            get { return ghostRules; }
        }

        public GameSettingsRecord Settings
        {
            get { return settings; }
        }

        private void Start()
        {
            LoadAllData();
        }

        public void LoadAllData()
        {
            LocationRecordList locationList = LoadJson<LocationRecordList>("locations.json");
            DoorRecordList doorList = LoadJson<DoorRecordList>("doors.json");
            ItemRecordList itemList = LoadJson<ItemRecordList>("items.json");
            PuzzleRecordList puzzleList = LoadJson<PuzzleRecordList>("puzzles.json");
            PuzzleAnswerRecordList puzzleAnswerList = LoadJson<PuzzleAnswerRecordList>("puzzle_answers.json");
            ClueRecordList clueList = LoadJson<ClueRecordList>("clues.json");
            SymbolRecordList symbolList = LoadJson<SymbolRecordList>("symbols.json");
            GhostRuleRecordList ghostRuleList = LoadJson<GhostRuleRecordList>("ghost_rules.json");
            GameSettingsWrapper settingsWrapper = LoadJson<GameSettingsWrapper>("game_settings.json");

            locations = locationList != null && locationList.locations != null ? locationList.locations : new List<LocationRecord>();
            doors = doorList != null && doorList.doors != null ? doorList.doors : new List<DoorRecord>();
            items = itemList != null && itemList.items != null ? itemList.items : new List<ItemRecord>();
            puzzles = puzzleList != null && puzzleList.puzzles != null ? puzzleList.puzzles : new List<PuzzleRecord>();
            puzzleAnswers = puzzleAnswerList != null && puzzleAnswerList.answers != null ? puzzleAnswerList.answers : new List<PuzzleAnswerRecord>();
            clues = clueList != null && clueList.clues != null ? clueList.clues : new List<ClueRecord>();
            symbols = symbolList != null && symbolList.symbols != null ? symbolList.symbols : new List<SymbolRecord>();
            ghostRules = ghostRuleList != null && ghostRuleList.ghostRules != null ? ghostRuleList.ghostRules : new List<GhostRuleRecord>();
            settings = settingsWrapper != null && settingsWrapper.settings != null ? settingsWrapper.settings : new GameSettingsRecord();
        }

        public T LoadJson<T>(string fileName) where T : class
        {
            string path = GetDataPath(fileName);

            if (!File.Exists(path))
            {
                Debug.LogWarning("Data file not found: " + path);
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        public string GetDataPath(string fileName)
        {
            return Path.Combine(Application.streamingAssetsPath, "Data", fileName);
        }

        public PuzzleRecord GetPuzzleById(string puzzleId)
        {
            if (string.IsNullOrEmpty(puzzleId) || puzzles == null)
            {
                return null;
            }

            for (int i = 0; i < puzzles.Count; i++)
            {
                if (puzzles[i] != null && puzzles[i].puzzleId == puzzleId)
                {
                    return puzzles[i];
                }
            }

            return null;
        }

        public ItemRecord GetItemById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || items == null)
            {
                return null;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].itemId == itemId)
                {
                    return items[i];
                }
            }

            return null;
        }

        public LocationRecord GetLocationById(string locationId)
        {
            if (string.IsNullOrEmpty(locationId) || locations == null)
            {
                return null;
            }

            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i] != null && locations[i].locationId == locationId)
                {
                    return locations[i];
                }
            }

            return null;
        }

        public DoorRecord GetDoorById(string doorId)
        {
            if (string.IsNullOrEmpty(doorId) || doors == null)
            {
                return null;
            }

            for (int i = 0; i < doors.Count; i++)
            {
                if (doors[i] != null && doors[i].doorId == doorId)
                {
                    return doors[i];
                }
            }

            return null;
        }

        public ClueRecord GetClueById(string clueId)
        {
            if (string.IsNullOrEmpty(clueId) || clues == null)
            {
                return null;
            }

            for (int i = 0; i < clues.Count; i++)
            {
                if (clues[i] != null && clues[i].clueId == clueId)
                {
                    return clues[i];
                }
            }

            return null;
        }

        public bool HasClue(string clueId)
        {
            return GetClueById(clueId) != null;
        }

        public IReadOnlyList<ClueRecord> GetCluesByLocation(string locationId)
        {
            List<ClueRecord> result = new List<ClueRecord>();
            if (string.IsNullOrEmpty(locationId) || clues == null)
            {
                return result;
            }

            for (int i = 0; i < clues.Count; i++)
            {
                if (clues[i] != null && clues[i].locationId == locationId)
                {
                    result.Add(clues[i]);
                }
            }

            return result;
        }

        public PuzzleAnswerRecord GetPuzzleAnswerByVariableName(string answerVariableName)
        {
            if (string.IsNullOrEmpty(answerVariableName) || puzzleAnswers == null)
            {
                return null;
            }

            for (int i = 0; i < puzzleAnswers.Count; i++)
            {
                if (puzzleAnswers[i] != null && puzzleAnswers[i].answerVariableName == answerVariableName)
                {
                    return puzzleAnswers[i];
                }
            }

            return null;
        }

        public PuzzleAnswerRecord GetPuzzleAnswerByPuzzleId(string puzzleId)
        {
            if (string.IsNullOrEmpty(puzzleId) || puzzleAnswers == null)
            {
                return null;
            }

            for (int i = 0; i < puzzleAnswers.Count; i++)
            {
                if (puzzleAnswers[i] != null && puzzleAnswers[i].puzzleId == puzzleId)
                {
                    return puzzleAnswers[i];
                }
            }

            return null;
        }

        public PuzzleAnswerRecord GetPuzzleAnswer(PuzzleRecord puzzle)
        {
            if (puzzle == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(puzzle.answerVariableName))
            {
                PuzzleAnswerRecord answer = GetPuzzleAnswerByVariableName(puzzle.answerVariableName);
                if (answer != null)
                {
                    return answer;
                }
            }

            if (!string.IsNullOrEmpty(puzzle.puzzleId))
            {
                return GetPuzzleAnswerByPuzzleId(puzzle.puzzleId);
            }

            return null;
        }

        public string GetAnswerText(PuzzleRecord puzzle)
        {
            PuzzleAnswerRecord answer = GetPuzzleAnswer(puzzle);
            return answer != null ? answer.answerText : null;
        }

        public string[] GetAnswerSequence(PuzzleRecord puzzle)
        {
            PuzzleAnswerRecord answer = GetPuzzleAnswer(puzzle);
            return answer != null ? answer.answerSequence : null;
        }

        public SymbolRecord GetSymbolById(string symbolId)
        {
            if (string.IsNullOrEmpty(symbolId) || symbols == null)
            {
                return null;
            }

            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i] != null && symbols[i].symbolId == symbolId)
                {
                    return symbols[i];
                }
            }

            return null;
        }

        public GhostRuleRecord GetGhostRuleById(string ruleId)
        {
            if (string.IsNullOrEmpty(ruleId) || ghostRules == null)
            {
                return null;
            }

            for (int i = 0; i < ghostRules.Count; i++)
            {
                if (ghostRules[i] != null && ghostRules[i].ruleId == ruleId)
                {
                    return ghostRules[i];
                }
            }

            return null;
        }

        public GhostRuleRecord GetGhostRuleForLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId) || ghostRules == null)
            {
                return null;
            }

            for (int i = 0; i < ghostRules.Count; i++)
            {
                if (ghostRules[i] != null && ghostRules[i].locationId == locationId)
                {
                    return ghostRules[i];
                }
            }

            return null;
        }

        public bool HasLocation(string locationId)
        {
            return GetLocationById(locationId) != null;
        }

        public bool HasDoor(string doorId)
        {
            return GetDoorById(doorId) != null;
        }
    }
}
