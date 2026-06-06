using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
	[Serializable]
	public class PuzzleReward
	{
		public ItemDefinition[] grantItems = Array.Empty<ItemDefinition>();
		public string[] consumeItemIds = Array.Empty<string>();
		public string[] setFlagIds = Array.Empty<string>();
	}

	[Serializable]
	public class PuzzleDefinition
	{
		public string puzzleId;
		public PuzzleReward reward = new PuzzleReward();
	}

	public class PuzzleManager : Singleton<PuzzleManager>
	{
		public const string FlagBedroomDrawerOpened = "BedroomDrawerOpened";
		public const string FlagBedroomDoorUnlocked = "BedroomDoorUnlocked";
		public const string FlagChildRoomUnlocked = "ChildRoomUnlocked";
		public const string FlagStudyKeyAvailable = "StudyKeyAvailable";
		public const string FlagFirstFloorHallUnlocked = "FirstFloorHallUnlocked";
		public const string FlagLivingKitchenUnlocked = "LivingKitchenUnlocked";
		public const string FlagKitchenCabinetHintKnown = "KitchenCabinetHintKnown";
		public const string FlagBasementMetalPieceInstalled = "BasementMetalPieceInstalled";
		public const string FlagBasementFuseInstalled = "BasementFuseInstalled";
		public const string FlagRangeKnobPuzzleActive = "RangeKnobPuzzleActive";
		public const string FlagBasementPowerRestored = "BasementPowerRestored";
		public const string FlagFinalPieceOrderHintKnown = "FinalPieceOrderHintKnown";
		public const string FlagFinalFrameActive = "FinalFrameActive";
		public const string FlagFrontDoorUnlocked = "FrontDoorUnlocked";
		public const string FlagEscaped = "Escaped";

		public const string ItemFramePiece1 = "FramePiece1";
		public const string ItemFramePiece2 = "FramePiece2";
		public const string ItemFramePiece3 = "FramePiece3";
		public const string ItemFramePiece4 = "FramePiece4";
		public const string ItemBedroomDoorKey = "BedroomDoorKey";
		public const string ItemStudyKey = "StudyKey";
		public const string ItemBasementMetalPiece = "BasementMetalPiece";
		public const string ItemBasementKey = "BasementKey";
		public const string ItemFrontDoorKey = "FrontDoorKey";

		[SerializeField] private PuzzleDefinition[] puzzleDefinitions = Array.Empty<PuzzleDefinition>();
		[SerializeField] private UIPuzzleOverlay puzzleOverlay;

		private readonly HashSet<string> solvedPuzzleIds = new HashSet<string>(StringComparer.Ordinal);
		private readonly HashSet<string> flagIds = new HashSet<string>(StringComparer.Ordinal);
		private readonly Dictionary<string, PuzzleDefinition> definitionsById = new Dictionary<string, PuzzleDefinition>(StringComparer.Ordinal);

		private bool isLoaded;

		public event Action PuzzleStateChanged;
		public event Action<string> PuzzleSolved;
		public event Action<string> FlagChanged;

		public bool IsPuzzleOpen => puzzleOverlay != null && puzzleOverlay.IsOpen;

		protected override bool DontDestroy => false;

		protected override void Awake()
		{
			base.Awake();

			if (Instance != this)
			{
				return;
			}

			BuildDefinitionLookup();
			FindOverlayIfNeeded();
			LoadPuzzleState();
			EvaluateDerivedFlags(false);
		}

		private void Start()
		{
			if (InventoryManager.Instance != null)
			{
				InventoryManager.Instance.InventoryChanged -= HandleInventoryChanged;
				InventoryManager.Instance.InventoryChanged += HandleInventoryChanged;
			}

			EvaluateDerivedFlags(true);
		}

		protected override void OnDestroy()
		{
			if (InventoryManager.Instance != null)
			{
				InventoryManager.Instance.InventoryChanged -= HandleInventoryChanged;
			}

			base.OnDestroy();
		}

		public bool OpenPuzzle(string puzzleId)
		{
			if (string.IsNullOrWhiteSpace(puzzleId))
			{
				Debug.LogWarning("PuzzleManager cannot open an empty puzzle id.", this);
				return false;
			}

			FindOverlayIfNeeded();
			if (puzzleOverlay == null)
			{
				Debug.LogWarning($"PuzzleManager cannot open '{puzzleId}' because no UIPuzzleOverlay exists.", this);
				return false;
			}

			return puzzleOverlay.OpenPuzzle(puzzleId);
		}

		public bool SolvePuzzle(string puzzleId)
		{
			if (string.IsNullOrWhiteSpace(puzzleId))
			{
				return false;
			}

			if (!solvedPuzzleIds.Add(puzzleId))
			{
				return false;
			}

			ApplyReward(puzzleId);
			EvaluateDerivedFlags(false);
			SavePuzzleState();
			PuzzleSolved?.Invoke(puzzleId);
			PuzzleStateChanged?.Invoke();
			return true;
		}

		public bool HasSolved(string puzzleId)
		{
			return !string.IsNullOrWhiteSpace(puzzleId) && solvedPuzzleIds.Contains(puzzleId);
		}

		public bool HasFlag(string flagId)
		{
			return !string.IsNullOrWhiteSpace(flagId) && flagIds.Contains(flagId);
		}

		public bool SetFlag(string flagId)
		{
			if (string.IsNullOrWhiteSpace(flagId) || !flagIds.Add(flagId))
			{
				return false;
			}

			SavePuzzleState();
			FlagChanged?.Invoke(flagId);
			PuzzleStateChanged?.Invoke();
			return true;
		}

		public bool MeetsConditions(string[] requiredFlagIds, string[] requiredSolvedPuzzleIds, string[] requiredItemIds)
		{
			if (!HasAllFlags(requiredFlagIds) || !HasAllSolved(requiredSolvedPuzzleIds))
			{
				return false;
			}

			if (requiredItemIds == null || requiredItemIds.Length == 0)
			{
				return true;
			}

			if (InventoryManager.Instance == null)
			{
				return false;
			}

			foreach (string itemId in requiredItemIds)
			{
				if (!string.IsNullOrWhiteSpace(itemId) && !InventoryManager.Instance.HasItem(itemId))
				{
					return false;
				}
			}

			return true;
		}

		private bool HasAllFlags(string[] requiredFlagIds)
		{
			if (requiredFlagIds == null)
			{
				return true;
			}

			foreach (string flagId in requiredFlagIds)
			{
				if (!string.IsNullOrWhiteSpace(flagId) && !HasFlag(flagId))
				{
					return false;
				}
			}

			return true;
		}

		private bool HasAllSolved(string[] requiredSolvedPuzzleIds)
		{
			if (requiredSolvedPuzzleIds == null)
			{
				return true;
			}

			foreach (string puzzleId in requiredSolvedPuzzleIds)
			{
				if (!string.IsNullOrWhiteSpace(puzzleId) && !HasSolved(puzzleId))
				{
					return false;
				}
			}

			return true;
		}

		private void BuildDefinitionLookup()
		{
			definitionsById.Clear();
			foreach (PuzzleDefinition definition in puzzleDefinitions)
			{
				if (definition == null || string.IsNullOrWhiteSpace(definition.puzzleId))
				{
					continue;
				}

				definitionsById[definition.puzzleId] = definition;
			}
		}

		private void FindOverlayIfNeeded()
		{
			if (puzzleOverlay == null)
			{
				puzzleOverlay = FindFirstObjectByType<UIPuzzleOverlay>(FindObjectsInactive.Include);
			}
		}

		private void LoadPuzzleState()
		{
			if (isLoaded)
			{
				return;
			}

			isLoaded = true;
			solvedPuzzleIds.Clear();
			flagIds.Clear();

			if (SaveManager.Instance == null)
			{
				return;
			}

			PuzzleSaveData saveData = SaveManager.Instance.LoadPuzzleData();
			AddNonEmpty(saveData.solvedPuzzleIds, solvedPuzzleIds);
			AddNonEmpty(saveData.flagIds, flagIds);
		}

		private void SavePuzzleState()
		{
			if (SaveManager.Instance == null)
			{
				return;
			}

			SaveManager.Instance.SavePuzzleData(new PuzzleSaveData
			{
				solvedPuzzleIds = ToArray(solvedPuzzleIds),
				flagIds = ToArray(flagIds)
			});
		}

		private void ApplyReward(string puzzleId)
		{
			if (!definitionsById.TryGetValue(puzzleId, out PuzzleDefinition definition) || definition.reward == null)
			{
				return;
			}

			PuzzleReward reward = definition.reward;
			if (InventoryManager.Instance != null)
			{
				if (reward.consumeItemIds != null)
				{
					foreach (string itemId in reward.consumeItemIds)
					{
						InventoryManager.Instance.RemoveItem(itemId);
					}
				}

				if (reward.grantItems != null)
				{
					foreach (ItemDefinition item in reward.grantItems)
					{
						InventoryManager.Instance.AddItem(item);
					}
				}
			}

			if (reward.setFlagIds != null)
			{
				foreach (string flagId in reward.setFlagIds)
				{
					if (!string.IsNullOrWhiteSpace(flagId))
					{
						flagIds.Add(flagId);
					}
				}
			}
		}

		private void EvaluateDerivedFlags(bool save)
		{
			bool changed = false;
			changed |= AddFlagIf(HasInventoryItems(ItemFramePiece1, ItemFramePiece2), FlagStudyKeyAvailable);
			changed |= AddFlagIf(HasFlag(FlagBasementMetalPieceInstalled) && HasFlag(FlagBasementFuseInstalled), FlagRangeKnobPuzzleActive);
			changed |= AddFlagIf(HasInventoryItems(ItemFramePiece1, ItemFramePiece2, ItemFramePiece3, ItemFramePiece4), FlagFinalFrameActive);

			if (changed && save)
			{
				SavePuzzleState();
				PuzzleStateChanged?.Invoke();
			}
		}

		private void HandleInventoryChanged()
		{
			EvaluateDerivedFlags(true);
		}

		private bool AddFlagIf(bool condition, string flagId)
		{
			return condition && !string.IsNullOrWhiteSpace(flagId) && flagIds.Add(flagId);
		}

		private bool HasInventoryItems(params string[] itemIds)
		{
			if (InventoryManager.Instance == null)
			{
				return false;
			}

			foreach (string itemId in itemIds)
			{
				if (!InventoryManager.Instance.HasItem(itemId))
				{
					return false;
				}
			}

			return true;
		}

		private static void AddNonEmpty(string[] values, HashSet<string> target)
		{
			if (values == null)
			{
				return;
			}

			foreach (string value in values)
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					target.Add(MapLegacyPuzzleId(value));
				}
			}
		}

		private static string MapLegacyPuzzleId(string puzzleId)
		{
			if (string.Equals(puzzleId, "Bedroom_MirrorFrameDirection", StringComparison.Ordinal))
			{
				return "Bedroom_FrontStorageCode";
			}

			if (string.Equals(puzzleId, "Bedroom_SmallBox", StringComparison.Ordinal))
			{
				return "Bedroom_BackStorageCode";
			}

			return puzzleId;
		}

		private static string[] ToArray(HashSet<string> source)
		{
			string[] values = new string[source.Count];
			source.CopyTo(values);
			return values;
		}
	}
}
