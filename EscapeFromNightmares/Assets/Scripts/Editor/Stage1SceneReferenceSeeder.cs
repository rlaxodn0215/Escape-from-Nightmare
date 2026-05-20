using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1SceneReferenceSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string StageDefinitionPath = "Assets/ScriptableObjects/Stage1/Stage1Definition.asset";

        [MenuItem("Escape From Nightmares/Seed Stage1 Scene References")]
        public static void Seed()
        {
            StageDefinition stageDefinition = AssetDatabase.LoadAssetAtPath<StageDefinition>(StageDefinitionPath);
            if (stageDefinition == null)
            {
                Debug.LogError($"Missing StageDefinition at {StageDefinitionPath}");
                return;
            }

            BindRoomSprites(stageDefinition);
            BindItemIcons(stageDefinition);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject stageRoot = GameObject.Find("StageRoot");
            GameObject systems = GameObject.Find("Systems");
            GameObject roomView = GameObject.Find("RoomView");

            if (stageRoot == null || systems == null || roomView == null)
            {
                Debug.LogError("Stage1 scene is missing StageRoot, Systems, or RoomView.");
                return;
            }

            GameBootstrap bootstrap = stageRoot.GetComponent<GameBootstrap>();
            if (bootstrap == null)
            {
                bootstrap = stageRoot.AddComponent<GameBootstrap>();
            }

            RoomSystem roomSystem = systems.GetComponent<RoomSystem>();
            if (roomSystem == null)
            {
                roomSystem = systems.AddComponent<RoomSystem>();
            }

            RoomViewPresenter roomViewPresenter = roomView.GetComponent<RoomViewPresenter>();
            if (roomViewPresenter == null)
            {
                roomViewPresenter = roomView.AddComponent<RoomViewPresenter>();
            }

            SpriteRenderer spriteRenderer = roomView.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = roomView.AddComponent<SpriteRenderer>();
            }

            spriteRenderer.sortingOrder = 0;
            spriteRenderer.drawMode = SpriteDrawMode.Simple;

            SetObjectReference(bootstrap, "stageDefinition", stageDefinition);
            SetObjectReference(roomSystem, "bootstrap", bootstrap);
            SetObjectReference(roomSystem, "roomViewPresenter", roomViewPresenter);
            SetObjectReference(roomViewPresenter, "backgroundRenderer", spriteRenderer);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded Stage1 scene room runtime references.");
        }

        private static void BindRoomSprites(StageDefinition stageDefinition)
        {
            foreach (RoomDefinition room in stageDefinition.Rooms)
            {
                if (room == null || string.IsNullOrWhiteSpace(room.RoomId))
                {
                    continue;
                }

                string spritePath = $"Assets/Sprites/Rooms/room_{ToResourceRoomName(room.RoomId)}.png";
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sprite == null)
                {
                    Debug.LogWarning($"Missing room sprite for {room.RoomId}: {spritePath}");
                    continue;
                }

                SetObjectReference(room, "backgroundSprite", sprite);
            }
        }

        private static void BindItemIcons(StageDefinition stageDefinition)
        {
            foreach (ItemDefinition item in stageDefinition.Items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.ItemId))
                {
                    continue;
                }

                string iconName = item.ItemId == "fuse_holder" ? "item_electric_part" : $"item_{item.ItemId}";
                string iconPath = $"Assets/Sprites/Items/{iconName}.png";
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
                if (sprite == null)
                {
                    Debug.LogWarning($"Missing item icon for {item.ItemId}: {iconPath}");
                    continue;
                }

                SetObjectReference(item, "icon", sprite);
            }
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static string ToResourceRoomName(string roomId)
        {
            return roomId switch
            {
                "first_floor_hallway" => "1f_hallway",
                "first_floor_bathroom" => "1f_bathroom",
                "first_floor_storage" => "1f_storage",
                "second_floor_hallway" => "2f_hallway",
                "second_floor_bathroom" => "2f_bathroom",
                "family_photo_room" => "family_photo",
                _ => roomId
            };
        }
    }
}
