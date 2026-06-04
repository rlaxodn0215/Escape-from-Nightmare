using System.Collections.Generic;
using System.Text;
using EscapeFromNightmare;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare.Editor
{
	public static class InventorySceneSetup
	{
		private const string MenuPath = "Tools/Escape From Nightmare/Setup Inventory";
		private const string MainScenePath = "Assets/Scenes/Main.unity";

		[MenuItem(MenuPath)]
		public static void SetupInventory()
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

			List<ItemDefinition> items = CreateItemDefinitions(report);
			Canvas canvas = Object.FindFirstObjectByType<Canvas>();
			if (canvas == null)
			{
				Debug.LogError("Setup Inventory failed because Main scene has no Canvas.");
				return;
			}

			GameObject managers = GameObject.Find("Managers");
			if (managers == null)
			{
				managers = new GameObject("Managers");
			}

			SetupSaveManager(report);
			InventoryManager inventoryManager = SetupInventoryManager(managers.transform, items, report);
			SetupInventoryBar(canvas.transform, inventoryManager, report);
			SetupPickups(canvas.transform, items, report);

			EditorUtility.SetDirty(canvas);
			EditorUtility.SetDirty(managers);
			AssetDatabase.SaveAssets();
			EditorSceneManager.MarkSceneDirty(scene);
			EditorSceneManager.SaveScene(scene);
			Debug.Log(report.ToString());
		}

		private static List<ItemDefinition> CreateItemDefinitions(StringBuilder report)
		{
			ItemSpec[] itemSpecs =
			{
				new ItemSpec("BasementFuse", "Basement Fuse", "Assets/Sprites/Items/BasementFuse.png"),
				new ItemSpec("FrontDoorKey", "Front Door Key", "Assets/Sprites/Items/FrontDoorKey.png"),
				new ItemSpec("OldDrawerKey", "Old Drawer Key", "Assets/Sprites/Items/OldDrawerKey.png"),
				new ItemSpec("SmallClockworkDevice", "Small Clockwork Device", "Assets/Sprites/Items/SmallClockworkDevice.png"),
				new ItemSpec("ModifiedClockworkDevice", "Modified Clockwork Device", "Assets/Sprites/Items/ModifiedClockworkDevice.png")
			};

			List<ItemDefinition> items = new List<ItemDefinition>();
			foreach (ItemSpec spec in itemSpecs)
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
				items.Add(item);
			}

			return items;
		}

		private static void SetupSaveManager(StringBuilder report)
		{
			GameObject saveManagerObject = GameObject.Find("SaveManager");
			if (saveManagerObject == null)
			{
				saveManagerObject = new GameObject("SaveManager");
				report.AppendLine("Created SaveManager object.");
			}

			if (saveManagerObject.transform.parent != null)
			{
				saveManagerObject.transform.SetParent(null, false);
				report.AppendLine("Moved SaveManager to the scene root.");
			}

			SaveManager saveManager = saveManagerObject.GetComponent<SaveManager>();
			if (saveManager == null)
			{
				saveManagerObject.AddComponent<SaveManager>();
				report.AppendLine("Added SaveManager component.");
			}
		}

		private static InventoryManager SetupInventoryManager(Transform managers, List<ItemDefinition> items, StringBuilder report)
		{
			GameObject managerObject = FindOrCreateChild(managers, "InventoryManager", false);
			InventoryManager inventoryManager = managerObject.GetComponent<InventoryManager>();
			if (inventoryManager == null)
			{
				inventoryManager = managerObject.AddComponent<InventoryManager>();
				report.AppendLine("Added InventoryManager component.");
			}

			SerializedObject serializedObject = new SerializedObject(inventoryManager);
			serializedObject.FindProperty("maxItems").intValue = 6;
			SerializedProperty catalog = serializedObject.FindProperty("itemCatalog");
			catalog.arraySize = items.Count;
			for (int i = 0; i < items.Count; i++)
			{
				catalog.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
			}

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(inventoryManager);
			return inventoryManager;
		}

		private static void SetupInventoryBar(Transform canvas, InventoryManager inventoryManager, StringBuilder report)
		{
			RectTransform inventoryRoot = FindOrCreateRectChild(canvas, "InventoryRoot");
			SetStretchFull(inventoryRoot);
			inventoryRoot.SetAsLastSibling();

			Sprite emptySlotSprite = LoadSprite("Assets/Sprites/UI/Panels/InventorySlotEmpty.png", report);
			Sprite selectedSlotSprite = LoadSprite("Assets/Sprites/UI/Panels/InventorySlotSelected.png", report);

			RectTransform toggleButtonRect = FindOrCreateRectChild(inventoryRoot, "InventoryToggleButton");
			SetAnchored(toggleButtonRect, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-34f, -34f), new Vector2(96f, 96f));
			Image toggleImage = EnsureComponent<Image>(toggleButtonRect.gameObject);
			toggleImage.sprite = emptySlotSprite;
			toggleImage.type = Image.Type.Sliced;
			toggleImage.color = Color.white;
			toggleImage.raycastTarget = true;
			Button toggleButton = EnsureComponent<Button>(toggleButtonRect.gameObject);
			toggleButton.targetGraphic = toggleImage;

			RectTransform windowRoot = FindOrCreateRectChild(inventoryRoot, "InventoryWindow");
			SetAnchored(windowRoot, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-34f, -146f), new Vector2(620f, 360f));
			windowRoot.gameObject.SetActive(false);
			MoveLegacyInventoryPanel(inventoryRoot, windowRoot);

			RectTransform panel = FindOrCreateRectChild(windowRoot, "InventoryPanel");
			SetAnchored(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 360f));

			Image panelImage = EnsureComponent<Image>(panel.gameObject);
			panelImage.sprite = LoadSprite("Assets/Sprites/UI/Panels/InventoryBarPanel.png", report);
			panelImage.type = Image.Type.Sliced;
			panelImage.color = Color.white;
			panelImage.raycastTarget = false;

			UIInventoryBar inventoryBar = inventoryRoot.GetComponent<UIInventoryBar>();
			if (inventoryBar == null)
			{
				inventoryBar = inventoryRoot.gameObject.AddComponent<UIInventoryBar>();
				report.AppendLine("Added UIInventoryBar component.");
			}

			RemoveExtraSlots(panel, 6);
			List<UIInventorySlot> slots = new List<UIInventorySlot>();
			float startX = -150f;
			float startY = 70f;

			for (int i = 0; i < 6; i++)
			{
				RectTransform slotRect = FindOrCreateRectChild(panel, $"Slot_{i + 1}");
				int column = i % 3;
				int row = i / 3;
				SetAnchored(slotRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(startX + column * 150f, startY - row * 150f), new Vector2(108f, 108f));

				Image slotImage = EnsureComponent<Image>(slotRect.gameObject);
				slotImage.sprite = emptySlotSprite;
				slotImage.type = Image.Type.Sliced;
				slotImage.color = Color.white;
				slotImage.raycastTarget = true;

				Button button = EnsureComponent<Button>(slotRect.gameObject);
				button.targetGraphic = slotImage;

				RectTransform iconRect = FindOrCreateRectChild(slotRect, "Icon");
				SetAnchored(iconRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(76f, 76f));
				Image iconImage = EnsureComponent<Image>(iconRect.gameObject);
				iconImage.color = Color.white;
				iconImage.preserveAspect = true;
				iconImage.raycastTarget = false;
				iconImage.enabled = false;

				UIInventorySlot slot = slotRect.GetComponent<UIInventorySlot>();
				if (slot == null)
				{
					slot = slotRect.gameObject.AddComponent<UIInventorySlot>();
				}

				SerializedObject slotObject = new SerializedObject(slot);
				slotObject.FindProperty("button").objectReferenceValue = button;
				slotObject.FindProperty("frameImage").objectReferenceValue = slotImage;
				slotObject.FindProperty("iconImage").objectReferenceValue = iconImage;
				slotObject.FindProperty("emptySprite").objectReferenceValue = emptySlotSprite;
				slotObject.FindProperty("selectedSprite").objectReferenceValue = selectedSlotSprite;
				slotObject.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(slot);
				slots.Add(slot);
			}

			SerializedObject barObject = new SerializedObject(inventoryBar);
			barObject.FindProperty("inventoryManager").objectReferenceValue = inventoryManager;
			barObject.FindProperty("toggleButton").objectReferenceValue = toggleButton;
			barObject.FindProperty("windowRoot").objectReferenceValue = windowRoot.gameObject;
			SerializedProperty slotArray = barObject.FindProperty("slots");
			slotArray.arraySize = slots.Count;
			for (int i = 0; i < slots.Count; i++)
			{
				slotArray.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
			}
			barObject.FindProperty("openOnStart").boolValue = false;

			barObject.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(inventoryBar);
		}

		private static void SetupPickups(Transform canvas, List<ItemDefinition> items, StringBuilder report)
		{
			PickupSpec[] pickupSpecs =
			{
				new PickupSpec("OldDrawerKey", "LocationRoot/Bedroom/Front/Buttons", new Vector2(-510f, -255f), new Vector2(120f, 80f)),
				new PickupSpec("SmallClockworkDevice", "LocationRoot/Study/Front/Buttons", new Vector2(250f, -250f), new Vector2(130f, 90f)),
				new PickupSpec("ModifiedClockworkDevice", "LocationRoot/Study/Back/Buttons", new Vector2(-280f, -210f), new Vector2(130f, 90f)),
				new PickupSpec("BasementFuse", "LocationRoot/BasementStorage/Front/Buttons", new Vector2(430f, -200f), new Vector2(130f, 90f)),
				new PickupSpec("FrontDoorKey", "LocationRoot/Kitchen/Back/Buttons", new Vector2(-390f, -230f), new Vector2(120f, 80f))
			};

			foreach (PickupSpec spec in pickupSpecs)
			{
				Transform parent = canvas.Find(spec.ParentPath);
				if (parent == null)
				{
					report.AppendLine($"Skipped pickup '{spec.ItemId}' because parent was not found: {spec.ParentPath}");
					continue;
				}

				RectTransform pickupRect = FindOrCreateRectChild(parent, $"Pickup_{spec.ItemId}");
				SetAnchored(pickupRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), spec.Position, spec.Size);

				Image pickupImage = EnsureComponent<Image>(pickupRect.gameObject);
				pickupImage.color = new Color(1f, 1f, 1f, 0.02f);
				pickupImage.raycastTarget = true;

				Button pickupButton = EnsureComponent<Button>(pickupRect.gameObject);
				pickupButton.targetGraphic = pickupImage;

				PickupItemButton pickup = pickupRect.GetComponent<PickupItemButton>();
				if (pickup == null)
				{
					pickup = pickupRect.gameObject.AddComponent<PickupItemButton>();
				}

				SerializedObject pickupObject = new SerializedObject(pickup);
				pickupObject.FindProperty("item").objectReferenceValue = items.Find(item => item != null && item.ItemId == spec.ItemId);
				pickupObject.FindProperty("button").objectReferenceValue = pickupButton;
				pickupObject.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(pickup);
				report.AppendLine($"Configured pickup: {spec.ItemId}");
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
			Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
			if (sprite != null)
			{
				return sprite;
			}

			Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
			foreach (Object asset in assets)
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
			T component = target.GetComponent<T>();
			return component != null ? component : target.AddComponent<T>();
		}

		private static void SetStretchBottom(RectTransform rect, float left, float right, float bottom, float height)
		{
			rect.anchorMin = new Vector2(0f, 0f);
			rect.anchorMax = new Vector2(1f, 0f);
			rect.pivot = new Vector2(0.5f, 0f);
			rect.anchoredPosition = new Vector2((left - right) * 0.5f, bottom);
			rect.sizeDelta = new Vector2(-(left + right), height);
			rect.localScale = Vector3.one;
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

		private static void RemoveExtraSlots(RectTransform panel, int slotCount)
		{
			List<GameObject> extraSlots = new List<GameObject>();
			foreach (Transform child in panel)
			{
				if (!child.name.StartsWith("Slot_"))
				{
					continue;
				}

				string indexText = child.name.Substring("Slot_".Length);
				if (!int.TryParse(indexText, out int slotIndex) || slotIndex < 1 || slotIndex > slotCount)
				{
					extraSlots.Add(child.gameObject);
				}
			}

			foreach (GameObject extraSlot in extraSlots)
			{
				Object.DestroyImmediate(extraSlot);
			}
		}

		private static void MoveLegacyInventoryPanel(RectTransform inventoryRoot, RectTransform windowRoot)
		{
			Transform legacyPanel = inventoryRoot.Find("InventoryPanel");
			if (legacyPanel == null)
			{
				return;
			}

			Transform existingWindowPanel = windowRoot.Find("InventoryPanel");
			if (existingWindowPanel != null && existingWindowPanel != legacyPanel)
			{
				Object.DestroyImmediate(legacyPanel.gameObject);
				return;
			}

			legacyPanel.SetParent(windowRoot, false);
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

		private readonly struct PickupSpec
		{
			public PickupSpec(string itemId, string parentPath, Vector2 position, Vector2 size)
			{
				ItemId = itemId;
				ParentPath = parentPath;
				Position = position;
				Size = size;
			}

			public string ItemId { get; }
			public string ParentPath { get; }
			public Vector2 Position { get; }
			public Vector2 Size { get; }
		}
	}
}
