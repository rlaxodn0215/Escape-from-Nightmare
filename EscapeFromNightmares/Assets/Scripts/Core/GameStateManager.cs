using System;
using EscapeFromNightmares.Systems;
using UnityEngine;

namespace EscapeFromNightmares.Core
{
    public enum GameRunState
    {
        Boot,
        Title,
        Playing,
        Paused,
        GameOver,
        Ending
    }

    public sealed class GameStateManager : MonoBehaviour
    {
        [SerializeField] private GameRunState currentState = GameRunState.Boot;
        [SerializeField] private RoomSystem roomSystem;
        [SerializeField] private string currentRoomId = "child_room";
        [SerializeField] private bool stage1Clear;

        public event Action<GameRunState> StateChanged;
        public event Action Stage1RunStarted;

        public GameRunState CurrentState => currentState;
        public string CurrentRoomId => currentRoomId;
        public bool Stage1Clear => stage1Clear;

        private void Awake()
        {
            roomSystem ??= FindFirstObjectByType<RoomSystem>();
        }

        public void SetState(GameRunState nextState)
        {
            if (currentState == nextState)
            {
                return;
            }

            currentState = nextState;
            StateChanged?.Invoke(currentState);
        }

        public void StartStage1Run()
        {
            currentRoomId = "child_room";
            roomSystem ??= FindFirstObjectByType<RoomSystem>();
            roomSystem?.ChangeRoom(currentRoomId);
            Stage1RunStarted?.Invoke();
            SetState(GameRunState.Playing);
        }

        public void SetCurrentRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                Debug.LogWarning("Ignored empty room id.");
                return;
            }

            currentRoomId = roomId;
        }

        public void SetPaused(bool paused)
        {
            SetState(paused ? GameRunState.Paused : GameRunState.Playing);
        }

        public void TriggerGameOver()
        {
            currentRoomId = "child_room";
            SetState(GameRunState.GameOver);
        }

        public void MarkStage1Clear()
        {
            stage1Clear = true;
            SetState(GameRunState.Ending);
        }
    }
}
