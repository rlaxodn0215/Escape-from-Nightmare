using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Stage-specific progression side effects that should not live in UI presenters.
    /// </summary>
    public static class ProgressionFlow
    {
        public const string KitchenFirstAppearanceEventFlag = "event_kitchen_first_appearance";

        public static void ApplyStudySafeUnlockAndOpenState(GameSession gameSession)
        {
            gameSession.SetFlag(StudySafePuzzleRules.UnlockedFlag);
            gameSession.SetFlag(StudySafePuzzleRules.OpenedFlag);
            gameSession.SetFlag(StudySafePuzzleRules.SuccessEventId);
        }

        public static void ApplyFinalChaseStartState(GameSession gameSession, MonsterAIController monsterAIController, FlagService flags)
        {
            gameSession.MonsterEnabled = true;
            flags.Set("final_chase_started");
            monsterAIController.ForceChase();
        }

        public static void ApplyLaundryStorageBoxMonsterStartState(GameSession gameSession, MonsterAIController monsterAIController, FlagService flags, DangerSystem danger)
        {
            gameSession.MonsterEnabled = true;
            monsterAIController.Enable();
            if (!flags.Has(KitchenFirstAppearanceEventFlag))
            {
                flags.Set(KitchenFirstAppearanceEventFlag);
                danger.AddNoise(20f);
            }
        }

        public static void ApplyStageClearState(GameSession gameSession, StageDefinition stageDefinition, SettingsSaveService settingsSaveService, MonsterAIController monsterAIController)
        {
            gameSession.SetFlag(stageDefinition.clearFlag);
            var records = settingsSaveService.LoadClearRecords();
            if (stageDefinition.stageId == StageRepository.Stage1Id)
            {
                records.stage1Clear = true;
            }

            settingsSaveService.SaveClearRecords(records);
            monsterAIController.Reset();
        }
    }
}
