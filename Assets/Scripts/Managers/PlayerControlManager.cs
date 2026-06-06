using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	// 방 안에서 플레이어가 바라보는 방향입니다.
	public enum RoomDirection
	{
		Front,
		Back
	}

	// 현재 화면의 이동 버튼이 어느 방과 방향으로 이어지는지 저장합니다.
	[Serializable]
	public class RoomExit
	{
		public string exitId;
		public string fromRoomId;
		public RoomDirection fromDirection = RoomDirection.Front;
		public string targetRoomId;
		public RoomDirection targetDirection = RoomDirection.Front;
		public Button button;
		public string requiredFlagId;
		public string requiredItemId;
		public string requiredSolvedPuzzleId;
	}

	[Serializable]
	public class RoomHidePoint
	{
		public string hidePointId;
		public string roomId;
		public RoomDirection direction = RoomDirection.Front;
		public Button button;
		public Sprite insideSprite;
	}

	// 1인칭 방 이동을 관리하며 현재 방과 방향 화면 하나만 활성화합니다.
	public class PlayerControlManager : Singleton<PlayerControlManager>
	{
		private const string DefaultStartRoomId = "Bedroom";
		private const string LocationRootName = "LocationRoot";

		[SerializeField] private Transform locationRoot;
		[SerializeField] private string startRoomId = DefaultStartRoomId;
		[SerializeField] private RoomDirection startDirection = RoomDirection.Front;
		[SerializeField] private RoomNavigationSceneDatabase navigationDatabase;
		[SerializeField] private Button rotateLeftButton;
		[SerializeField] private Button rotateRightButton;
		[SerializeField] private GameObject hideRoot;
		[SerializeField] private Image hideImage;
		[SerializeField] private Button exitHideButton;
		[SerializeField] private ScreenFadeManager screenFadeManager;

		private readonly Dictionary<string, Transform> roomRoots = new Dictionary<string, Transform>(StringComparer.Ordinal);
		private readonly Dictionary<RoomViewKey, GameObject> roomViews = new Dictionary<RoomViewKey, GameObject>();
		private readonly List<BoundExitButton> boundExitButtons = new List<BoundExitButton>();
		private readonly List<BoundHidePointButton> boundHidePointButtons = new List<BoundHidePointButton>();

		private string currentRoomId;
		private RoomDirection currentDirection;
		private string currentHidePointId;

		public string CurrentRoomId => currentRoomId;
		public RoomDirection CurrentDirection => currentDirection;
		public bool IsHidden => !string.IsNullOrEmpty(currentHidePointId);
		public string CurrentHidePointId => currentHidePointId;
		public bool IsInputBlocked => (screenFadeManager != null && screenFadeManager.IsTransitioning)
			|| (PuzzleManager.Instance != null && PuzzleManager.Instance.IsPuzzleOpen);

		protected override bool DontDestroy => false;
		private IReadOnlyList<RoomExit> Exits => navigationDatabase != null ? navigationDatabase.Exits : Array.Empty<RoomExit>();
		private IReadOnlyList<RoomHidePoint> HidePoints => navigationDatabase != null ? navigationDatabase.HidePoints : Array.Empty<RoomHidePoint>();

		protected override void Awake()
		{
			base.Awake();

			if (Instance != this)
			{
				return;
			}

			RefreshRoomViewsFromLocationRoot();
			FindScreenFadeManagerIfNeeded();
			FindNavigationDatabaseIfNeeded();
			BindRotationButtons();
			BindExitButtons();
			BindHidePointButtons();
			BindExitHideButton();
			SetHideRootActive(false);
			InitializeStartLocation();
		}

		protected override void OnDestroy()
		{
			UnbindExitHideButton();
			UnbindHidePointButtons();
			UnbindRotationButtons();
			UnbindExitButtons();
			base.OnDestroy();
		}

		// 위치 루트 하위의 방 오브젝트와 앞/뒤 화면을 자동 등록합니다.
		public void RefreshRoomViewsFromLocationRoot()
		{
			roomRoots.Clear();
			roomViews.Clear();
			FindLocationRootIfNeeded();

			if (locationRoot == null)
			{
				Debug.LogWarning("PlayerControlManager has no LocationRoot assigned.", this);
				return;
			}

			foreach (Transform roomRoot in locationRoot)
			{
				if (roomRoot == null || string.IsNullOrWhiteSpace(roomRoot.name))
				{
					continue;
				}

				string roomId = roomRoot.name;
				if (roomRoots.ContainsKey(roomId))
				{
					Debug.LogWarning($"PlayerControlManager found duplicate room id '{roomId}'. Only the first room will be used.", roomRoot);
					continue;
				}

				roomRoots.Add(roomId, roomRoot);
				RegisterDirectionView(roomId, roomRoot, RoomDirection.Front);
				RegisterDirectionView(roomId, roomRoot, RoomDirection.Back);
			}
		}

		private void FindLocationRootIfNeeded()
		{
			if (locationRoot != null)
			{
				return;
			}

			GameObject foundLocationRoot = GameObject.Find(LocationRootName);
			if (foundLocationRoot != null)
			{
				locationRoot = foundLocationRoot.transform;
			}
		}

		private void FindNavigationDatabaseIfNeeded()
		{
			if (navigationDatabase != null)
			{
				return;
			}

			navigationDatabase = GetComponent<RoomNavigationSceneDatabase>();
			if (navigationDatabase == null)
			{
				navigationDatabase = UnityEngine.Object.FindFirstObjectByType<RoomNavigationSceneDatabase>();
			}
		}

		// 좌측 회전 버튼에서 호출되며, 두 방향 구조에서는 앞뒤를 토글합니다.
		public void RotateLeft()
		{
			if (IsInputBlocked)
			{
				return;
			}

			if (IsHidden)
			{
				Debug.LogWarning("PlayerControlManager cannot rotate while hidden.", this);
				return;
			}

			ToggleDirection();
		}

		// 우측 회전 버튼에서 호출되며, 두 방향 구조에서는 앞뒤를 토글합니다.
		public void RotateRight()
		{
			if (IsInputBlocked)
			{
				return;
			}

			if (IsHidden)
			{
				Debug.LogWarning("PlayerControlManager cannot rotate while hidden.", this);
				return;
			}

			ToggleDirection();
		}

		// 지정한 출구 식별자에 해당하는 방 이동을 시도합니다.
		public void MoveToRoom(string exitId)
		{
			if (IsInputBlocked)
			{
				return;
			}

			if (IsHidden)
			{
				Debug.LogWarning("PlayerControlManager cannot move while hidden.", this);
				return;
			}

			if (string.IsNullOrWhiteSpace(exitId))
			{
				Debug.LogWarning("PlayerControlManager cannot move with an empty exit id.", this);
				return;
			}

			RoomExit exit = FindExit(exitId);
			if (exit == null)
			{
				Debug.LogWarning($"PlayerControlManager could not find RoomExit '{exitId}'.", this);
				return;
			}

			if (!IsExitAvailable(exit))
			{
				Debug.LogWarning($"RoomExit '{exitId}' is not available from '{currentRoomId}/{currentDirection}'.", this);
				if (AudioManager.Instance != null)
				{
					AudioManager.Instance.PlaySfx(AudioSoundId.DoorLocked);
				}

				return;
			}

			MoveToLocation(exit.targetRoomId, exit.targetDirection, true);
		}

		public bool MoveToLocation(string roomId, RoomDirection direction, bool useFade)
		{
			if (useFade)
			{
				if (IsInputBlocked)
				{
					return false;
				}

				FindScreenFadeManagerIfNeeded();
				if (screenFadeManager != null)
				{
					screenFadeManager.PlayTransition(() => TrySetLocation(roomId, direction));
					return true;
				}
			}

			return TrySetLocation(roomId, direction);
		}

		public bool EnterHidePoint(string hidePointId)
		{
			if (IsInputBlocked)
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(hidePointId))
			{
				Debug.LogWarning("PlayerControlManager cannot hide with an empty hide point id.", this);
				return false;
			}

			RoomHidePoint hidePoint = FindHidePoint(hidePointId);
			if (hidePoint == null)
			{
				Debug.LogWarning($"PlayerControlManager could not find hide point '{hidePointId}'.", this);
				return false;
			}

			if (!IsHidePointAvailable(hidePoint))
			{
				Debug.LogWarning($"Hide point '{hidePointId}' is not available from '{currentRoomId}/{currentDirection}'.", this);
				return false;
			}

			if (hideRoot == null || hideImage == null)
			{
				Debug.LogWarning("PlayerControlManager has no HideRoot or HideImage assigned.", this);
				return false;
			}

			PlayFade(() =>
			{
				currentHidePointId = hidePoint.hidePointId;
				hideImage.sprite = hidePoint.insideSprite;
				SetHideRootActive(true);
			});
			return true;
		}

		public void ExitHidePoint()
		{
			if (!IsHidden)
			{
				SetHideRootActive(false);
				return;
			}

			if (IsInputBlocked)
			{
				return;
			}

			PlayFade(ExitHidePointImmediate);
		}

		private void InitializeStartLocation()
		{
			currentRoomId = string.Empty;
			currentDirection = startDirection;
			TrySetLocation(startRoomId, startDirection);
		}

		private void ToggleDirection()
		{
			RoomDirection nextDirection = currentDirection == RoomDirection.Front ? RoomDirection.Back : RoomDirection.Front;
			MoveToLocation(currentRoomId, nextDirection, true);
		}

		private void FindScreenFadeManagerIfNeeded()
		{
			if (screenFadeManager == null)
			{
				screenFadeManager = ScreenFadeManager.EnsureInstance();
			}
		}

		private void PlayFade(Action onHidden)
		{
			FindScreenFadeManagerIfNeeded();
			if (screenFadeManager != null)
			{
				screenFadeManager.PlayTransition(onHidden);
			}
			else
			{
				onHidden?.Invoke();
			}
		}

		private bool TrySetLocation(string roomId, RoomDirection direction)
		{
			if (string.IsNullOrWhiteSpace(roomId))
			{
				Debug.LogWarning("PlayerControlManager cannot move to an empty room id.", this);
				return false;
			}

			if (!roomRoots.ContainsKey(roomId))
			{
				Debug.LogWarning($"PlayerControlManager could not find room '{roomId}'.", this);
				return false;
			}

			RoomViewKey key = new RoomViewKey(roomId, direction);
			if (!roomViews.ContainsKey(key))
			{
				Debug.LogWarning($"PlayerControlManager could not find view '{roomId}/{direction}'.", this);
				return false;
			}

			currentRoomId = roomId;
			currentDirection = direction;
			ExitHidePointImmediate();
			ApplyActiveView();
			return true;
		}

		// 현재 위치 상태에 맞춰 방과 방향 오브젝트의 활성화를 적용합니다.
		private void ApplyActiveView()
		{
			foreach (KeyValuePair<string, Transform> roomRoot in roomRoots)
			{
				if (roomRoot.Value != null)
				{
					roomRoot.Value.gameObject.SetActive(roomRoot.Key == currentRoomId);
				}
			}

			foreach (KeyValuePair<RoomViewKey, GameObject> roomView in roomViews)
			{
				if (roomView.Value != null)
				{
					bool isCurrentView = roomView.Key.RoomId == currentRoomId && roomView.Key.Direction == currentDirection;
					roomView.Value.SetActive(isCurrentView);
				}
			}
		}

		private void RegisterDirectionView(string roomId, Transform roomRoot, RoomDirection direction)
		{
			Transform directionView = roomRoot.Find(direction.ToString());
			if (directionView == null)
			{
				return;
			}

			RoomViewKey key = new RoomViewKey(roomId, direction);
			if (roomViews.ContainsKey(key))
			{
				Debug.LogWarning($"PlayerControlManager found duplicate view '{roomId}/{direction}'. Only the first view will be used.", directionView);
				return;
			}

			roomViews.Add(key, directionView.gameObject);
		}

		private RoomExit FindExit(string exitId)
		{
			foreach (RoomExit exit in Exits)
			{
				if (exit != null && string.Equals(exit.exitId, exitId, StringComparison.Ordinal))
				{
					return exit;
				}
			}

			return null;
		}

		private bool IsExitAvailable(RoomExit exit)
		{
			if (!string.Equals(exit.fromRoomId, currentRoomId, StringComparison.Ordinal) || exit.fromDirection != currentDirection)
			{
				return false;
			}

			if (PuzzleManager.Instance == null)
			{
				return string.IsNullOrWhiteSpace(exit.requiredFlagId)
					&& string.IsNullOrWhiteSpace(exit.requiredItemId)
					&& string.IsNullOrWhiteSpace(exit.requiredSolvedPuzzleId);
			}

			return PuzzleManager.Instance.MeetsConditions(
				ToConditionArray(exit.requiredFlagId),
				ToConditionArray(exit.requiredSolvedPuzzleId),
				ToConditionArray(exit.requiredItemId));
		}

		private RoomHidePoint FindHidePoint(string hidePointId)
		{
			foreach (RoomHidePoint hidePoint in HidePoints)
			{
				if (hidePoint != null && string.Equals(hidePoint.hidePointId, hidePointId, StringComparison.Ordinal))
				{
					return hidePoint;
				}
			}

			return null;
		}

		private bool IsHidePointAvailable(RoomHidePoint hidePoint)
		{
			return string.Equals(hidePoint.roomId, currentRoomId, StringComparison.Ordinal)
				&& hidePoint.direction == currentDirection;
		}

		private static string[] ToConditionArray(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? Array.Empty<string>() : new[] { value };
		}

		private void BindExitButtons()
		{
			UnbindExitButtons();

			foreach (RoomExit exit in Exits)
			{
				if (exit == null || exit.button == null || string.IsNullOrWhiteSpace(exit.exitId))
				{
					continue;
				}

				string capturedExitId = exit.exitId;
				UnityAction action = () => MoveToRoom(capturedExitId);
				exit.button.onClick.AddListener(action);
				boundExitButtons.Add(new BoundExitButton(exit.button, action));
			}
		}

		private void UnbindExitButtons()
		{
			foreach (BoundExitButton boundButton in boundExitButtons)
			{
				if (boundButton.Button != null && boundButton.Action != null)
				{
					boundButton.Button.onClick.RemoveListener(boundButton.Action);
				}
			}

			boundExitButtons.Clear();
		}

		private void BindHidePointButtons()
		{
			UnbindHidePointButtons();

			foreach (RoomHidePoint hidePoint in HidePoints)
			{
				if (hidePoint == null || hidePoint.button == null || string.IsNullOrWhiteSpace(hidePoint.hidePointId))
				{
					continue;
				}

				string capturedHidePointId = hidePoint.hidePointId;
				UnityAction action = () => EnterHidePoint(capturedHidePointId);
				hidePoint.button.onClick.AddListener(action);
				boundHidePointButtons.Add(new BoundHidePointButton(hidePoint.button, action));
			}
		}

		private void UnbindHidePointButtons()
		{
			foreach (BoundHidePointButton boundButton in boundHidePointButtons)
			{
				if (boundButton.Button != null && boundButton.Action != null)
				{
					boundButton.Button.onClick.RemoveListener(boundButton.Action);
				}
			}

			boundHidePointButtons.Clear();
		}

		private void BindExitHideButton()
		{
			if (exitHideButton != null)
			{
				exitHideButton.onClick.AddListener(ExitHidePoint);
			}
		}

		private void UnbindExitHideButton()
		{
			if (exitHideButton != null)
			{
				exitHideButton.onClick.RemoveListener(ExitHidePoint);
			}
		}

		private void SetHideRootActive(bool active)
		{
			if (hideRoot != null)
			{
				hideRoot.SetActive(active);
			}

			if (exitHideButton != null)
			{
				exitHideButton.gameObject.SetActive(active);
			}

			if (!active && hideImage != null)
			{
				hideImage.sprite = null;
			}
		}

		private void ExitHidePointImmediate()
		{
			currentHidePointId = string.Empty;
			SetHideRootActive(false);
		}

		private void BindRotationButtons()
		{
			if (rotateLeftButton != null)
			{
				rotateLeftButton.onClick.AddListener(RotateLeft);
			}

			if (rotateRightButton != null)
			{
				rotateRightButton.onClick.AddListener(RotateRight);
			}
		}

		private void UnbindRotationButtons()
		{
			if (rotateLeftButton != null)
			{
				rotateLeftButton.onClick.RemoveListener(RotateLeft);
			}

			if (rotateRightButton != null)
			{
				rotateRightButton.onClick.RemoveListener(RotateRight);
			}
		}

		private readonly struct RoomViewKey : IEquatable<RoomViewKey>
		{
			public RoomViewKey(string roomId, RoomDirection direction)
			{
				RoomId = roomId;
				Direction = direction;
			}

			public string RoomId { get; }
			public RoomDirection Direction { get; }

			public bool Equals(RoomViewKey other)
			{
				return string.Equals(RoomId, other.RoomId, StringComparison.Ordinal) && Direction == other.Direction;
			}

			public override bool Equals(object obj)
			{
				return obj is RoomViewKey other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((RoomId != null ? StringComparer.Ordinal.GetHashCode(RoomId) : 0) * 397) ^ (int)Direction;
				}
			}
		}

		private readonly struct BoundExitButton
		{
			public BoundExitButton(Button button, UnityAction action)
			{
				Button = button;
				Action = action;
			}

			public Button Button { get; }
			public UnityAction Action { get; }
		}

		private readonly struct BoundHidePointButton
		{
			public BoundHidePointButton(Button button, UnityAction action)
			{
				Button = button;
				Action = action;
			}

			public Button Button { get; }
			public UnityAction Action { get; }
		}
	}
}
