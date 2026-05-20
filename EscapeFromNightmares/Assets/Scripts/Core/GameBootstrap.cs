using UnityEngine;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private StageDefinition stageDefinition;
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private SceneFlowController sceneFlowController;
        [SerializeField] private InputRouter inputRouter;
        [SerializeField] private SaveManager saveManager;

        public StageDefinition StageDefinition => stageDefinition;
        public GameStateManager GameStateManager => gameStateManager;
        public SceneFlowController SceneFlowController => sceneFlowController;
        public InputRouter InputRouter => inputRouter;
        public SaveManager SaveManager => saveManager;

        private void Awake()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            sceneFlowController ??= FindFirstObjectByType<SceneFlowController>();
            inputRouter ??= FindFirstObjectByType<InputRouter>();
            saveManager ??= FindFirstObjectByType<SaveManager>();

            saveManager?.Load();
            gameStateManager?.SetState(GameRunState.Boot);
        }
    }
}
