using System;
using System.Collections.Generic;
using System.Text;
using EscapeFromNightmare;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare.Editor
{
	public static class PuzzleSceneSetup
	{
		private const string MenuPath = "Tools/Escape From Nightmare/Setup Puzzle System";
		private const string MainScenePath = "Assets/Scenes/Main.unity";
		private const string DisabledLegacyPickupFlag = "DisabledLegacyPickup";

		[MenuItem(MenuPath)]
		public static void SetupPuzzleSystem()
		{
			StringBuilder report = new StringBuilder();
			UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene();
			if (scene.name != "Main")
			{
				scene = EditorSceneManager.OpenScene(MainScenePath);
			}

			AssetDatabase.Refresh();
			EnsureFolder("Assets/ScriptableObjects");
			EnsureFolder("Assets/ScriptableObjects/Items");

			Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
			if (canvas == null)
			{
				Debug.LogError("Setup Puzzle System failed because Main scene has no Canvas.");
				return;
			}

			GameObject managers = GameObject.Find("Managers");
			if (managers == null)
			{
				managers = new GameObject("Managers");
			}

			Dictionary<string, ItemDefinition> items = CreateItemDefinitions(report);
			InventoryManager inventoryManager = SetupInventoryManager(managers.transform, items, report);
			UIPuzzleOverlay overlay = SetupOverlay(canvas.transform, report);
			SetupPuzzleManager(managers.transform, overlay, items, report);
			SetupPuzzleHotspots(canvas.transform, items, report);
			DisableLegacyBedroomPuzzleObjects(canvas.transform, report);
			SetupStudyKeyPickup(canvas.transform, items, report);
			DisableLegacyPickups(canvas.transform, report);
			SetupDoorRequirements(report);

			EditorUtility.SetDirty(canvas);
			EditorUtility.SetDirty(managers);
			if (inventoryManager != null)
			{
				EditorUtility.SetDirty(inventoryManager);
			}

			AssetDatabase.SaveAssets();
			EditorSceneManager.MarkSceneDirty(scene);
			EditorSceneManager.SaveScene(scene);
			Debug.Log(report.ToString());
		}

		private static Dictionary<string, ItemDefinition> CreateItemDefinitions(StringBuilder report)
		{
			ItemSpec[] specs =
			{
				new ItemSpec("BasementFuse", "Basement Fuse", "Assets/Sprites/Items/BasementFuse.png"),
				new ItemSpec("FrontDoorKey", "Front Door Key", "Assets/Sprites/Items/FrontDoorKey.png"),
				new ItemSpec("OldDrawerKey", "Old Drawer Key", "Assets/Sprites/Items/OldDrawerKey.png"),
				new ItemSpec("SmallClockworkDevice", "Small Clockwork Device", "Assets/Sprites/Items/SmallClockworkDevice.png"),
				new ItemSpec("ModifiedClockworkDevice", "Modified Clockwork Device", "Assets/Sprites/Items/ModifiedClockworkDevice.png"),
				new ItemSpec(PuzzleManager.ItemFramePiece1, "Frame Piece 1", "Assets/Sprites/Symbols/Symbol_01.png"),
				new ItemSpec(PuzzleManager.ItemFramePiece2, "Frame Piece 2", "Assets/Sprites/Symbols/Symbol_02.png"),
				new ItemSpec(PuzzleManager.ItemFramePiece3, "Frame Piece 3", "Assets/Sprites/Symbols/Symbol_03.png"),
				new ItemSpec(PuzzleManager.ItemFramePiece4, "Frame Piece 4", "Assets/Sprites/Symbols/Symbol_04.png"),
				new ItemSpec(PuzzleManager.ItemBedroomDoorKey, "Bedroom Door Key", "Assets/Sprites/Items/FrontDoorKey.png"),
				new ItemSpec(PuzzleManager.ItemStudyKey, "Study Key", "Assets/Sprites/Items/FrontDoorKey.png"),
				new ItemSpec(PuzzleManager.ItemBasementMetalPiece, "Basement Metal Piece", "Assets/Sprites/Symbols/Symbol_05.png"),
				new ItemSpec(PuzzleManager.ItemBasementKey, "Basement Key", "Assets/Sprites/Items/FrontDoorKey.png")
			};

			Dictionary<string, ItemDefinition> items = new Dictionary<string, ItemDefinition>(StringComparer.Ordinal);
			foreach (ItemSpec spec in specs)
			{
				string assetPath = $"Assets/ScriptableObjects/Items/{spec.Id}.asset";
				ItemDefinition item = AssetDatabase.LoadAssetAtPath<ItemDefinition>(assetPath);
				if (item == null)
				{
					item = ScriptableObject.CreateInstance<ItemDefinition>();
					AssetDatabase.CreateAsset(item, assetPath);
					report.AppendLine($"Created item asset: {assetPath}");
				}

				SerializedObject serializedObject = new SerializedObject(item);
				serializedObject.FindProperty("itemId").stringValue = spec.Id;
				serializedObject.FindProperty("displayName").stringValue = spec.DisplayName;
				serializedObject.FindProperty("icon").objectReferenceValue = LoadSprite(spec.IconPath, report);
				serializedObject.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(item);
				items[spec.Id] = item;
			}

			return items;
		}

		private static InventoryManager SetupInventoryManager(Transform managers, Dictionary<string, ItemDefinition> items, StringBuilder report)
		{
			GameObject managerObject = FindOrCreateChild(managers, "InventoryManager", false);
			InventoryManager inventoryManager = managerObject.GetComponent<InventoryManager>();
			if (inventoryManager == null)
			{
				inventoryManager = managerObject.AddComponent<InventoryManager>();
				report.AppendLine("Added InventoryManager component.");
			}

			SerializedObject serializedObject = new SerializedObject(inventoryManager);
			serializedObject.FindProperty("maxItems").intValue = 12;
			SerializedProperty catalog = serializedObject.FindProperty("itemCatalog");
			List<ItemDefinition> itemList = new List<ItemDefinition>(items.Values);
			catalog.arraySize = itemList.Count;
			for (int i = 0; i < itemList.Count; i++)
			{
				catalog.GetArrayElementAtIndex(i).objectReferenceValue = itemList[i];
			}

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(inventoryManager);
			return inventoryManager;
		}

		private static UIPuzzleOverlay SetupOverlay(Transform canvas, StringBuilder report)
		{
			RectTransform overlayRoot = FindOrCreateRectChild(canvas, "PuzzleOverlay");
			SetStretchFull(overlayRoot);
			overlayRoot.SetAsLastSibling();

			Image dimImage = EnsureComponent<Image>(overlayRoot.gameObject);
			dimImage.color = new Color(0.02f, 0.018f, 0.016f, 0.94f);
			dimImage.raycastTarget = true;

			UIPuzzleOverlay overlay = EnsureComponent<UIPuzzleOverlay>(overlayRoot.gameObject);

			RectTransform titleRect = FindOrCreateRectChild(overlayRoot, "TitleText");
			SetAnchored(titleRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -36f), new Vector2(760f, 60f));
			Text titleText = EnsureText(titleRect.gameObject, 28, TextAnchor.MiddleCenter);

			RectTransform messageRect = FindOrCreateRectChild(overlayRoot, "MessageText");
			SetAnchored(messageRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 34f), new Vector2(820f, 52f));
			Text messageText = EnsureText(messageRect.gameObject, 22, TextAnchor.MiddleCenter);

			RectTransform closeRect = FindOrCreateRectChild(overlayRoot, "CloseButton");
			SetAnchored(closeRect, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-34f, -34f), new Vector2(92f, 58f));
			Image closeImage = EnsureComponent<Image>(closeRect.gameObject);
			closeImage.color = new Color(0.28f, 0.23f, 0.2f, 0.98f);
			Button closeButton = EnsureComponent<Button>(closeRect.gameObject);
			closeButton.targetGraphic = closeImage;
			Text closeText = EnsureText(FindOrCreateRectChild(closeRect, "Text").gameObject, 20, TextAnchor.MiddleCenter);
			closeText.text = "Close";
			SetStretchFull(closeText.GetComponent<RectTransform>());

			RectTransform contentRoot = FindOrCreateRectChild(overlayRoot, "ContentRoot");
			SetStretchFull(contentRoot);

			List<PuzzleViewBase> views = new List<PuzzleViewBase>
			{
				SetupStorageView<BedroomFrontStorageCodePuzzleView>(contentRoot, "Bedroom_FrontStorageCode", "Front Storage Lock", "Assets/Sprites/Puzzles/Bedroom/Bedroom_FrontStorage_Closeup.png", report),
				SetupStorageView<BedroomBackStorageCodePuzzleView>(contentRoot, "Bedroom_BackStorageCode", "Back Storage Lock", "Assets/Sprites/Puzzles/Bedroom/Bedroom_BackStorage_Closeup.png", report),
				SetupFrameClueView(contentRoot, "Bedroom_FrontFrame_LeftTable_Front5", "Framed Photograph", "Assets/Sprites/Puzzles/Bedroom/Bedroom_FrontFrame_LeftTable_Front5.png", "Front storage clue: 5 / Symbol_04", report),
				SetupFrameClueView(contentRoot, "Bedroom_FrontFrame_CenterTable_Back1", "Framed Photograph", "Assets/Sprites/Puzzles/Bedroom/Bedroom_FrontFrame_CenterTable_Back1.png", "Back storage clue: 1 / Symbol_05", report),
				SetupFrameClueView(contentRoot, "Bedroom_FrontFrame_RightTable_Front2", "Framed Photograph", "Assets/Sprites/Puzzles/Bedroom/Bedroom_FrontFrame_RightTable_Front2.png", "Front storage clue: 2 / Symbol_03", report),
				SetupFrameClueView(contentRoot, "Bedroom_FrontFrame_RightWall_Back0", "Framed Photograph", "Assets/Sprites/Puzzles/Bedroom/Bedroom_FrontFrame_RightWall_Back0.png", "Back storage clue: 0 / Symbol_02", report),
				SetupFrameClueView(contentRoot, "Bedroom_BackFrame_LeftWallLarge_Front6", "Framed Photograph", "Assets/Sprites/Puzzles/Bedroom/Bedroom_BackFrame_LeftWallLarge_Front6.png", "Front storage clue: 6 / Symbol_01", report),
				SetupFrameClueView(contentRoot, "Bedroom_BackFrame_LeftWallSmall_Back6", "Framed Photograph", "Assets/Sprites/Puzzles/Bedroom/Bedroom_BackFrame_LeftWallSmall_Back6.png", "Back storage clue: 6 / Symbol_01", report),
				SetupFrameClueView(contentRoot, "Bedroom_BackFrame_TableLeft_Front7", "Framed Photograph", "Assets/Sprites/Puzzles/Bedroom/Bedroom_BackFrame_TableLeft_Front7.png", "Front storage clue: 7 / Symbol_02", report),
				SetupFrameClueView(contentRoot, "Bedroom_BackFrame_TableRight_Back5", "Framed Photograph", "Assets/Sprites/Puzzles/Bedroom/Bedroom_BackFrame_TableRight_Back5.png", "Back storage clue: 5 / Symbol_06", report),
				SetupView<HallPortraitGazePuzzleView>(contentRoot, "Hall_PortraitGazeCross", "Crossing Gazes"),
				SetupView<ChildRoomCardOrderPuzzleView>(contentRoot, "ChildRoom_CardOrder", "Card Pattern Order"),
				SetupView<DollhouseWindowPuzzleView>(contentRoot, "ChildRoom_DollhouseWindows", "Dollhouse Windows"),
				SetupView<StudyBookshelfSymbolPuzzleView>(contentRoot, "Study_BookshelfSymbol", "Bookshelf Symbols"),
				SetupView<StudyDeskLockPuzzleView>(contentRoot, "Study_DeskLockPassword", "Desk Lock"),
				SetupView<LivingRoomClockCandlePuzzleView>(contentRoot, "LivingRoom_ClockCandleOrder", "Clock and Candles"),
				SetupView<KitchenCabinetPuzzleView>(contentRoot, "Kitchen_CabinetArrangement", "Cabinet Arrangement"),
				SetupView<KitchenTableclothPuzzleView>(contentRoot, "Kitchen_TableclothSymbol", "Folded Tablecloth"),
				SetupView<KitchenBasementLockPuzzleView>(contentRoot, "Kitchen_BasementDoorLock", "Basement Door Lock"),
				SetupView<KitchenRangeKnobPuzzleView>(contentRoot, "Kitchen_RangeKnobDirection", "Range Knobs"),
				SetupView<BasementGeneratorPuzzleView>(contentRoot, "Basement_GeneratorRedLight", "Generator Lights"),
				SetupView<BasementShadowPuzzleView>(contentRoot, "Basement_ShadowBottleBox", "Shelf Shadows"),
				SetupView<BasementWorkbenchPuzzleView>(contentRoot, "Basement_WorkbenchClueBox", "Workbench Box"),
				SetupView<FinalFramePuzzleView>(contentRoot, "Final_FramePieceOrder", "The Large Frame")
			};

			SerializedObject overlayObject = new SerializedObject(overlay);
			overlayObject.FindProperty("rootPanel").objectReferenceValue = overlayRoot.gameObject;
			overlayObject.FindProperty("contentRoot").objectReferenceValue = contentRoot;
			overlayObject.FindProperty("titleText").objectReferenceValue = titleText;
			overlayObject.FindProperty("messageText").objectReferenceValue = messageText;
			overlayObject.FindProperty("closeButton").objectReferenceValue = closeButton;
			SerializedProperty viewArray = overlayObject.FindProperty("puzzleViews");
			viewArray.arraySize = views.Count;
			for (int i = 0; i < views.Count; i++)
			{
				viewArray.GetArrayElementAtIndex(i).objectReferenceValue = views[i];
			}

			overlayObject.ApplyModifiedPropertiesWithoutUndo();
			overlayRoot.gameObject.SetActive(false);
			EditorUtility.SetDirty(overlay);
			report.AppendLine("Configured PuzzleOverlay.");
			return overlay;
		}

		private static T SetupView<T>(RectTransform contentRoot, string puzzleId, string title) where T : PuzzleViewBase
		{
			RectTransform viewRect = FindOrCreateRectChild(contentRoot, puzzleId);
			SetStretchFull(viewRect);
			T view = EnsurePuzzleView<T>(viewRect.gameObject);
			SerializedObject serializedObject = new SerializedObject(view);
			serializedObject.FindProperty("puzzleId").stringValue = puzzleId;
			serializedObject.FindProperty("title").stringValue = title;
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			viewRect.gameObject.SetActive(false);
			EditorUtility.SetDirty(view);
			return view;
		}

		private static T EnsurePuzzleView<T>(GameObject target) where T : PuzzleViewBase
		{
			GameObjectUtility.RemoveMonoBehavioursWithMissingScript(target);
			T selected = null;
			PuzzleViewBase[] views = target.GetComponents<PuzzleViewBase>();
			foreach (PuzzleViewBase view in views)
			{
				if (view is T typedView && selected == null)
				{
					selected = typedView;
					continue;
				}

				UnityEngine.Object.DestroyImmediate(view);
			}

			return selected != null ? selected : target.AddComponent<T>();
		}

		private static T SetupStorageView<T>(RectTransform contentRoot, string puzzleId, string title, string imagePath, StringBuilder report) where T : PuzzleViewBase
		{
			T view = SetupView<T>(contentRoot, puzzleId, title);
			SerializedObject serializedObject = new SerializedObject(view);
			serializedObject.FindProperty("closeupImage").objectReferenceValue = LoadSprite(imagePath, report);
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(view);
			return view;
		}

		private static BedroomFrameCluePuzzleView SetupFrameClueView(RectTransform contentRoot, string puzzleId, string title, string imagePath, string description, StringBuilder report)
		{
			BedroomFrameCluePuzzleView view = SetupView<BedroomFrameCluePuzzleView>(contentRoot, puzzleId, title);
			SerializedObject serializedObject = new SerializedObject(view);
			serializedObject.FindProperty("clueImage").objectReferenceValue = LoadSprite(imagePath, report);
			serializedObject.FindProperty("clueDescription").stringValue = description;
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(view);
			return view;
		}

		private static void SetupPuzzleManager(Transform managers, UIPuzzleOverlay overlay, Dictionary<string, ItemDefinition> items, StringBuilder report)
		{
			GameObject managerObject = FindOrCreateChild(managers, "PuzzleManager", false);
			PuzzleManager puzzleManager = EnsureComponent<PuzzleManager>(managerObject);
			SerializedObject serializedObject = new SerializedObject(puzzleManager);
			serializedObject.FindProperty("puzzleOverlay").objectReferenceValue = overlay;

			PuzzleConfig[] configs =
			{
				new PuzzleConfig("Bedroom_FrontStorageCode", new[] { PuzzleManager.ItemBedroomDoorKey }, null, new[] { PuzzleManager.FlagBedroomDoorUnlocked }),
				new PuzzleConfig("Bedroom_BackStorageCode", new[] { PuzzleManager.ItemFramePiece1 }, null, null),
				new PuzzleConfig("Hall_PortraitGazeCross", null, null, new[] { PuzzleManager.FlagChildRoomUnlocked }),
				new PuzzleConfig("ChildRoom_CardOrder", new[] { "SmallClockworkDevice" }, null, null),
				new PuzzleConfig("ChildRoom_DollhouseWindows", new[] { PuzzleManager.ItemFramePiece2 }, null, null),
				new PuzzleConfig("Study_BookshelfSymbol", null, null, new[] { "StudyDeskDrawerOpened", PuzzleManager.FlagFirstFloorHallUnlocked }),
				new PuzzleConfig("Study_DeskLockPassword", new[] { PuzzleManager.ItemFramePiece3 }, null, new[] { PuzzleManager.FlagLivingKitchenUnlocked }),
				new PuzzleConfig("LivingRoom_ClockCandleOrder", null, null, new[] { PuzzleManager.FlagKitchenCabinetHintKnown }),
				new PuzzleConfig("Kitchen_CabinetArrangement", new[] { PuzzleManager.ItemBasementMetalPiece }, null, null),
				new PuzzleConfig("Kitchen_TableclothSymbol", new[] { "BasementFuse" }, null, null),
				new PuzzleConfig("Kitchen_BasementDoorLock", null, new[] { PuzzleManager.ItemBasementMetalPiece, "BasementFuse" }, new[] { PuzzleManager.FlagBasementMetalPieceInstalled, PuzzleManager.FlagBasementFuseInstalled }),
				new PuzzleConfig("Kitchen_RangeKnobDirection", new[] { PuzzleManager.ItemBasementKey }, null, null),
				new PuzzleConfig("Basement_GeneratorRedLight", null, null, new[] { PuzzleManager.FlagBasementPowerRestored }),
				new PuzzleConfig("Basement_ShadowBottleBox", null, null, new[] { PuzzleManager.FlagFinalPieceOrderHintKnown }),
				new PuzzleConfig("Basement_WorkbenchClueBox", new[] { PuzzleManager.ItemFramePiece4 }, null, null),
				new PuzzleConfig("Final_FramePieceOrder", new[] { PuzzleManager.ItemFrontDoorKey }, null, new[] { PuzzleManager.FlagFrontDoorUnlocked })
			};

			SerializedProperty definitions = serializedObject.FindProperty("puzzleDefinitions");
			definitions.arraySize = configs.Length;
			for (int i = 0; i < configs.Length; i++)
			{
				SerializedProperty definition = definitions.GetArrayElementAtIndex(i);
				definition.FindPropertyRelative("puzzleId").stringValue = configs[i].PuzzleId;
				SerializedProperty reward = definition.FindPropertyRelative("reward");
				SetItemArray(reward.FindPropertyRelative("grantItems"), configs[i].GrantItemIds, items);
				SetStringArray(reward.FindPropertyRelative("consumeItemIds"), configs[i].ConsumeItemIds);
				SetStringArray(reward.FindPropertyRelative("setFlagIds"), configs[i].SetFlagIds);
			}

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(puzzleManager);
			report.AppendLine("Configured PuzzleManager.");
		}

		private static void SetupPuzzleHotspots(Transform canvas, Dictionary<string, ItemDefinition> items, StringBuilder report)
		{
			PuzzleHotspotSpec[] specs =
			{
				new PuzzleHotspotSpec("Bedroom_FrontStorageCode", "LocationRoot/Bedroom/Front/Buttons", new Vector2(130f, -95f), new Vector2(280f, 150f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_BackStorageCode", "LocationRoot/Bedroom/Back/Buttons", new Vector2(170f, -85f), new Vector2(280f, 150f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_FrontFrame_LeftTable_Front5", "LocationRoot/Bedroom/Front/Buttons", new Vector2(-15f, 35f), new Vector2(86f, 105f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_FrontFrame_CenterTable_Back1", "LocationRoot/Bedroom/Front/Buttons", new Vector2(35f, 34f), new Vector2(74f, 100f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_FrontFrame_RightTable_Front2", "LocationRoot/Bedroom/Front/Buttons", new Vector2(140f, 34f), new Vector2(108f, 112f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_FrontFrame_RightWall_Back0", "LocationRoot/Bedroom/Front/Buttons", new Vector2(885f, 220f), new Vector2(145f, 225f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_BackFrame_LeftWallLarge_Front6", "LocationRoot/Bedroom/Back/Buttons", new Vector2(-670f, 235f), new Vector2(230f, 335f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_BackFrame_LeftWallSmall_Back6", "LocationRoot/Bedroom/Back/Buttons", new Vector2(-470f, 145f), new Vector2(115f, 190f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_BackFrame_TableLeft_Front7", "LocationRoot/Bedroom/Back/Buttons", new Vector2(-720f, -180f), new Vector2(205f, 170f), null, null, null),
				new PuzzleHotspotSpec("Bedroom_BackFrame_TableRight_Back5", "LocationRoot/Bedroom/Back/Buttons", new Vector2(-505f, -150f), new Vector2(135f, 165f), null, null, null),
				new PuzzleHotspotSpec("Hall_PortraitGazeCross", "LocationRoot/SecondFloorHallway/Front/Buttons", new Vector2(0f, 110f), new Vector2(420f, 160f), null, null, null),
				new PuzzleHotspotSpec("ChildRoom_CardOrder", "LocationRoot/ChildRoom/Front/Buttons", new Vector2(-220f, -50f), new Vector2(240f, 150f), null, null, null),
				new PuzzleHotspotSpec("ChildRoom_DollhouseWindows", "LocationRoot/ChildRoom/Back/Buttons", new Vector2(230f, 0f), new Vector2(240f, 180f), null, null, new[] { "SmallClockworkDevice" }),
				new PuzzleHotspotSpec("Study_BookshelfSymbol", "LocationRoot/Study/Back/Buttons", new Vector2(-230f, 40f), new Vector2(260f, 200f), null, null, null),
				new PuzzleHotspotSpec("Study_DeskLockPassword", "LocationRoot/Study/Front/Buttons", new Vector2(170f, -80f), new Vector2(250f, 150f), null, new[] { "Study_BookshelfSymbol" }, null),
				new PuzzleHotspotSpec("LivingRoom_ClockCandleOrder", "LocationRoot/LivingRoom/Front/Buttons", new Vector2(0f, -10f), new Vector2(280f, 180f), null, null, null),
				new PuzzleHotspotSpec("Kitchen_CabinetArrangement", "LocationRoot/Kitchen/Front/Buttons", new Vector2(-260f, 55f), new Vector2(260f, 160f), new[] { PuzzleManager.FlagKitchenCabinetHintKnown }, null, null),
				new PuzzleHotspotSpec("Kitchen_TableclothSymbol", "LocationRoot/Kitchen/Front/Buttons", new Vector2(185f, -120f), new Vector2(260f, 135f), null, null, null),
				new PuzzleHotspotSpec("Kitchen_BasementDoorLock", "LocationRoot/Kitchen/Back/Buttons", new Vector2(260f, -40f), new Vector2(240f, 180f), null, null, new[] { PuzzleManager.ItemBasementMetalPiece, "BasementFuse" }),
				new PuzzleHotspotSpec("Kitchen_RangeKnobDirection", "LocationRoot/Kitchen/Front/Buttons", new Vector2(40f, 20f), new Vector2(220f, 140f), new[] { PuzzleManager.FlagRangeKnobPuzzleActive }, null, null),
				new PuzzleHotspotSpec("Basement_GeneratorRedLight", "LocationRoot/BasementStorage/Front/Buttons", new Vector2(-250f, 10f), new Vector2(260f, 180f), null, null, null),
				new PuzzleHotspotSpec("Basement_ShadowBottleBox", "LocationRoot/BasementStorage/Back/Buttons", new Vector2(0f, 70f), new Vector2(340f, 170f), new[] { PuzzleManager.FlagBasementPowerRestored }, null, null),
				new PuzzleHotspotSpec("Basement_WorkbenchClueBox", "LocationRoot/BasementStorage/Front/Buttons", new Vector2(260f, -100f), new Vector2(260f, 150f), new[] { PuzzleManager.FlagFinalPieceOrderHintKnown }, null, null),
				new PuzzleHotspotSpec("Final_FramePieceOrder", "LocationRoot/LivingRoom/Back/Buttons", new Vector2(0f, 55f), new Vector2(360f, 210f), new[] { PuzzleManager.FlagFinalFrameActive, PuzzleManager.FlagFinalPieceOrderHintKnown }, null, null)
			};

			foreach (PuzzleHotspotSpec spec in specs)
			{
				Transform parent = canvas.Find(spec.ParentPath);
				if (parent == null)
				{
					report.AppendLine($"Skipped puzzle hotspot '{spec.PuzzleId}' because parent was not found: {spec.ParentPath}");
					continue;
				}

				RectTransform hotspot = FindOrCreateRectChild(parent, "Puzzle_" + spec.PuzzleId);
				SetAnchored(hotspot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), spec.Position, spec.Size);
				Image image = EnsureComponent<Image>(hotspot.gameObject);
				image.color = new Color(1f, 1f, 1f, 0.025f);
				image.raycastTarget = true;
				Button button = EnsureComponent<Button>(hotspot.gameObject);
				button.targetGraphic = image;
				PuzzleOpenButton openButton = EnsureComponent<PuzzleOpenButton>(hotspot.gameObject);
				SerializedObject serializedObject = new SerializedObject(openButton);
				serializedObject.FindProperty("puzzleId").stringValue = spec.PuzzleId;
				serializedObject.FindProperty("button").objectReferenceValue = button;
				SetStringArray(serializedObject.FindProperty("requiredFlagIds"), spec.RequiredFlagIds);
				SetStringArray(serializedObject.FindProperty("requiredSolvedPuzzleIds"), spec.RequiredSolvedPuzzleIds);
				SetStringArray(serializedObject.FindProperty("requiredItemIds"), spec.RequiredItemIds);
				serializedObject.FindProperty("hideWhenSolved").boolValue = false;
				serializedObject.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(openButton);
			}

			report.AppendLine("Configured puzzle hotspots.");
		}

		private static void SetupStudyKeyPickup(Transform canvas, Dictionary<string, ItemDefinition> items, StringBuilder report)
		{
			Transform parent = canvas.Find("LocationRoot/SecondFloorHallway/Back/Buttons");
			if (parent == null || !items.TryGetValue(PuzzleManager.ItemStudyKey, out ItemDefinition studyKey))
			{
				report.AppendLine("Skipped StudyKey pickup.");
				return;
			}

			RectTransform pickupRect = FindOrCreateRectChild(parent, "Pickup_StudyKey");
			SetAnchored(pickupRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(270f, -120f), new Vector2(135f, 90f));
			Image image = EnsureComponent<Image>(pickupRect.gameObject);
			image.color = new Color(1f, 1f, 1f, 0.025f);
			image.raycastTarget = true;
			Button button = EnsureComponent<Button>(pickupRect.gameObject);
			button.targetGraphic = image;
			PickupItemButton pickup = EnsureComponent<PickupItemButton>(pickupRect.gameObject);
			SerializedObject serializedObject = new SerializedObject(pickup);
			serializedObject.FindProperty("item").objectReferenceValue = studyKey;
			serializedObject.FindProperty("button").objectReferenceValue = button;
			SetStringArray(serializedObject.FindProperty("requiredFlagIds"), new[] { PuzzleManager.FlagStudyKeyAvailable });
			SetStringArray(serializedObject.FindProperty("requiredSolvedPuzzleIds"), null);
			SetStringArray(serializedObject.FindProperty("requiredItemIds"), null);
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(pickup);
			report.AppendLine("Configured StudyKey pickup.");
		}

		private static void DisableLegacyBedroomPuzzleObjects(Transform canvas, StringBuilder report)
		{
			string[] disablePaths =
			{
				"LocationRoot/Bedroom/Front/Buttons/Puzzle_Bedroom_MirrorFrameDirection",
				"LocationRoot/Bedroom/Front/Buttons/Puzzle_Bedroom_SmallBox"
			};

			foreach (string path in disablePaths)
			{
				Transform target = canvas.Find(path);
				if (target == null)
				{
					continue;
				}

				GameObjectUtility.RemoveMonoBehavioursWithMissingScript(target.gameObject);
				target.gameObject.SetActive(false);
				EditorUtility.SetDirty(target.gameObject);
			}

			string[] destroyPaths =
			{
				"PuzzleOverlay/ContentRoot/Bedroom_MirrorFrameDirection",
				"PuzzleOverlay/ContentRoot/Bedroom_SmallBox"
			};

			foreach (string path in destroyPaths)
			{
				Transform target = canvas.Find(path);
				if (target == null)
				{
					continue;
				}

				UnityEngine.Object.DestroyImmediate(target.gameObject);
			}

			report.AppendLine("Disabled legacy bedroom mirror/small-box puzzle objects.");
		}

		private static void DisableLegacyPickups(Transform canvas, StringBuilder report)
		{
			string[] legacyPickupNames =
			{
				"Pickup_OldDrawerKey",
				"Pickup_SmallClockworkDevice",
				"Pickup_ModifiedClockworkDevice",
				"Pickup_BasementFuse",
				"Pickup_FrontDoorKey"
			};

			foreach (PickupItemButton pickup in canvas.GetComponentsInChildren<PickupItemButton>(true))
			{
				if (pickup == null || Array.IndexOf(legacyPickupNames, pickup.gameObject.name) < 0)
				{
					continue;
				}

				SerializedObject serializedObject = new SerializedObject(pickup);
				SetStringArray(serializedObject.FindProperty("requiredFlagIds"), new[] { DisabledLegacyPickupFlag });
				SetStringArray(serializedObject.FindProperty("requiredSolvedPuzzleIds"), null);
				SetStringArray(serializedObject.FindProperty("requiredItemIds"), null);
				serializedObject.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(pickup);
			}

			report.AppendLine("Disabled legacy direct pickups that conflict with puzzle rewards.");
		}

		private static void SetupDoorRequirements(StringBuilder report)
		{
			RoomNavigationSceneDatabase database = UnityEngine.Object.FindFirstObjectByType<RoomNavigationSceneDatabase>();
			if (database == null)
			{
				report.AppendLine("Skipped door requirements because RoomNavigationSceneDatabase was not found.");
				return;
			}

			Dictionary<string, ExitRequirement> requirements = new Dictionary<string, ExitRequirement>(StringComparer.Ordinal)
			{
				{ "Door_Bedroom_SecondFloorHallway", new ExitRequirement(PuzzleManager.FlagBedroomDoorUnlocked, null, null) },
				{ "Door_SecondFloorHallway_ChildRoom", new ExitRequirement(PuzzleManager.FlagChildRoomUnlocked, null, null) },
				{ "Door_SecondFloorHallway_Study", new ExitRequirement(null, PuzzleManager.ItemStudyKey, null) },
				{ "Door_SecondFloorHallway_FirstFloorHall", new ExitRequirement(PuzzleManager.FlagFirstFloorHallUnlocked, null, null) },
				{ "Door_FirstFloorHall_LivingRoom", new ExitRequirement(PuzzleManager.FlagLivingKitchenUnlocked, null, null) },
				{ "Door_FirstFloorHall_Kitchen", new ExitRequirement(PuzzleManager.FlagLivingKitchenUnlocked, null, null) },
				{ "Door_Kitchen_BasementStorage", new ExitRequirement(null, PuzzleManager.ItemBasementKey, null) }
			};

			SerializedObject serializedObject = new SerializedObject(database);
			SerializedProperty exits = serializedObject.FindProperty("exits");
			for (int i = 0; i < exits.arraySize; i++)
			{
				SerializedProperty exit = exits.GetArrayElementAtIndex(i);
				string exitId = exit.FindPropertyRelative("exitId").stringValue;
				if (!requirements.TryGetValue(exitId, out ExitRequirement requirement))
				{
					continue;
				}

				exit.FindPropertyRelative("requiredFlagId").stringValue = requirement.RequiredFlagId ?? string.Empty;
				exit.FindPropertyRelative("requiredItemId").stringValue = requirement.RequiredItemId ?? string.Empty;
				exit.FindPropertyRelative("requiredSolvedPuzzleId").stringValue = requirement.RequiredSolvedPuzzleId ?? string.Empty;
			}

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(database);
			report.AppendLine("Configured door requirements.");
		}

		private static void SetStringArray(SerializedProperty property, string[] values)
		{
			if (values == null)
			{
				values = Array.Empty<string>();
			}

			property.arraySize = values.Length;
			for (int i = 0; i < values.Length; i++)
			{
				property.GetArrayElementAtIndex(i).stringValue = values[i] ?? string.Empty;
			}
		}

		private static void SetItemArray(SerializedProperty property, string[] itemIds, Dictionary<string, ItemDefinition> items)
		{
			if (itemIds == null)
			{
				itemIds = Array.Empty<string>();
			}

			property.arraySize = itemIds.Length;
			for (int i = 0; i < itemIds.Length; i++)
			{
				items.TryGetValue(itemIds[i], out ItemDefinition item);
				property.GetArrayElementAtIndex(i).objectReferenceValue = item;
			}
		}

		private static void EnsureFolder(string path)
		{
			if (AssetDatabase.IsValidFolder(path))
			{
				return;
			}

			int slash = path.LastIndexOf('/');
			string parent = path.Substring(0, slash);
			string name = path.Substring(slash + 1);
			EnsureFolder(parent);
			AssetDatabase.CreateFolder(parent, name);
		}

		private static Sprite LoadSprite(string path, StringBuilder report)
		{
			TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
			if (importer != null && importer.textureType != TextureImporterType.Sprite)
			{
				importer.textureType = TextureImporterType.Sprite;
				importer.spriteImportMode = SpriteImportMode.Single;
				importer.SaveAndReimport();
			}

			Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
			if (sprite != null)
			{
				return sprite;
			}

			UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
			foreach (UnityEngine.Object asset in assets)
			{
				if (asset is Sprite candidate)
				{
					return candidate;
				}
			}

			report.AppendLine($"Missing sprite: {path}");
			return null;
		}

		private static GameObject FindOrCreateChild(Transform parent, string name, bool rect)
		{
			Transform existing = parent.Find(name);
			if (existing != null)
			{
				return existing.gameObject;
			}

			GameObject child = rect ? new GameObject(name, typeof(RectTransform)) : new GameObject(name);
			child.transform.SetParent(parent, false);
			return child;
		}

		private static RectTransform FindOrCreateRectChild(Transform parent, string name)
		{
			Transform existing = parent.Find(name);
			if (existing != null && existing.TryGetComponent(out RectTransform existingRect))
			{
				return existingRect;
			}

			GameObject child = new GameObject(name, typeof(RectTransform));
			child.layer = 5;
			child.transform.SetParent(parent, false);
			return child.GetComponent<RectTransform>();
		}

		private static T EnsureComponent<T>(GameObject target) where T : Component
		{
			GameObjectUtility.RemoveMonoBehavioursWithMissingScript(target);
			T[] components = target.GetComponents<T>();
			if (components.Length > 0)
			{
				for (int i = 1; i < components.Length; i++)
				{
					UnityEngine.Object.DestroyImmediate(components[i]);
				}

				return components[0];
			}

			return target.AddComponent<T>();
		}

		private static Text EnsureText(GameObject target, int fontSize, TextAnchor alignment)
		{
			Text text = EnsureComponent<Text>(target);
			text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			if (text.font == null)
			{
				text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}

			text.fontSize = fontSize;
			text.alignment = alignment;
			text.color = new Color(0.93f, 0.89f, 0.78f, 1f);
			text.raycastTarget = false;
			return text;
		}

		private static void SetStretchFull(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;
			rect.localScale = Vector3.one;
		}

		private static void SetAnchored(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
		{
			rect.anchorMin = anchorMin;
			rect.anchorMax = anchorMax;
			rect.pivot = pivot;
			rect.anchoredPosition = position;
			rect.sizeDelta = size;
			rect.localScale = Vector3.one;
		}

		private readonly struct ItemSpec
		{
			public ItemSpec(string id, string displayName, string iconPath)
			{
				Id = id;
				DisplayName = displayName;
				IconPath = iconPath;
			}

			public string Id { get; }
			public string DisplayName { get; }
			public string IconPath { get; }
		}

		private readonly struct PuzzleConfig
		{
			public PuzzleConfig(string puzzleId, string[] grantItemIds, string[] consumeItemIds, string[] setFlagIds)
			{
				PuzzleId = puzzleId;
				GrantItemIds = grantItemIds;
				ConsumeItemIds = consumeItemIds;
				SetFlagIds = setFlagIds;
			}

			public string PuzzleId { get; }
			public string[] GrantItemIds { get; }
			public string[] ConsumeItemIds { get; }
			public string[] SetFlagIds { get; }
		}

		private readonly struct PuzzleHotspotSpec
		{
			public PuzzleHotspotSpec(string puzzleId, string parentPath, Vector2 position, Vector2 size, string[] requiredFlagIds, string[] requiredSolvedPuzzleIds, string[] requiredItemIds)
			{
				PuzzleId = puzzleId;
				ParentPath = parentPath;
				Position = position;
				Size = size;
				RequiredFlagIds = requiredFlagIds;
				RequiredSolvedPuzzleIds = requiredSolvedPuzzleIds;
				RequiredItemIds = requiredItemIds;
			}

			public string PuzzleId { get; }
			public string ParentPath { get; }
			public Vector2 Position { get; }
			public Vector2 Size { get; }
			public string[] RequiredFlagIds { get; }
			public string[] RequiredSolvedPuzzleIds { get; }
			public string[] RequiredItemIds { get; }
		}

		private readonly struct ExitRequirement
		{
			public ExitRequirement(string requiredFlagId, string requiredItemId, string requiredSolvedPuzzleId)
			{
				RequiredFlagId = requiredFlagId;
				RequiredItemId = requiredItemId;
				RequiredSolvedPuzzleId = requiredSolvedPuzzleId;
			}

			public string RequiredFlagId { get; }
			public string RequiredItemId { get; }
			public string RequiredSolvedPuzzleId { get; }
		}
	}
}
