using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Plain runtime object graph shared by GameDirector collaborators.
    /// </summary>
    public sealed class RuntimeContext
    {
        public StageDefinition Stage { get; set; }
        public StageLookup StageLookup { get; set; }
        public GameSession Session { get; set; }
        public FlagService Flags { get; set; }
        public InventoryService Inventory { get; set; }
        public PuzzleService Puzzles { get; set; }
        public DangerSystem Danger { get; set; }
        public HidingSystem Hiding { get; set; }
        public MonsterAIController MonsterAI { get; set; }
        public SettingsSaveService SaveService { get; set; }
        public SoundManager Sound { get; set; }
        public EscapeActionResolver ActionResolver { get; set; }
        public EscapeActionExecutor ActionExecutor { get; set; }
    }
}
