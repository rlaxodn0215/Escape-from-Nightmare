using System.Collections.Generic;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Systems;
using EscapeFromNightmares.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1MapRuntimeSeeder
    {
        private const string MenuPath = "Escape From Nightmares/Seed Stage1 Map Runtime";
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";

        private static readonly Dictionary<string, Vector2> MarkerPositions = new Dictionary<string, Vector2>
        {
            { "entrance", new Vector2(178f, -112f) },
            { "first_floor_hallway", new Vector2(8f, 10f) },
            { "living_room", new Vector2(-168f, 20f) },
            { "dining_room", new Vector2(-36f, -92f) },
            { "kitchen", new Vector2(-150f, -78f) },
            { "laundry_room", new Vector2(96f, -78f) },
            { "first_floor_bathroom", new Vector2(-222f, -70f) },
            { "first_floor_storage", new Vector2(98f, 88f) },
            { "garage", new Vector2(222f, -116f) },
            { "family_photo_room", new Vector2(-224f, 92f) },
            { "stairwell_1f", new Vector2(222f, 90f) },
            { "child_room", new Vector2(-214f, 84f) },
            { "second_floor_hallway", new Vector2(2f, 78f) },
            { "master_bedroom", new Vector2(-86f, 138f) },
            { "dressing_room", new Vector2(-150f, 12f) },
            { "guest_room", new Vector2(-224f, 14f) },
            { "second_floor_bathroom", new Vector2(44f, 16f) },
            { "study", new Vector2(86f, 118f) },
            { "mirror_room", new Vector2(160f, 52f) },
            { "stairwell_2f", new Vector2(222f, 90f) },
            { "basement_entry", new Vector2(-170f, 72f) },
            { "basement_main", new Vector2(-14f, 44f) },
            { "basement_storage", new Vector2(106f, 86f) },
            { "altar_room", new Vector2(178f, -70f) },
            { "attic_main", new Vector2(-80f, 52f) },
            { "attic_toy_storage", new Vector2(140f, -36f) },
            { "attic_album_storage", new Vector2(70f, 78f) }
        };

        [MenuItem(MenuPath)]
        public static void Seed()
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            GameObject stageRoot = GameObject.Find("StageRoot");
            GameObject mapObject = FindSceneObject("MapUI");

            if (systems == null || stageRoot == null || mapObject == null)
            {
                Debug.LogError("Stage1 scene is missing Systems, StageRoot, or MapUI.");
                return;
            }

            MapRuntimeSystem mapRuntimeSystem = systems.GetComponent<MapRuntimeSystem>();
            if (mapRuntimeSystem == null)
            {
                mapRuntimeSystem = systems.AddComponent<MapRuntimeSystem>();
            }

            GameBootstrap bootstrap = stageRoot.GetComponent<GameBootstrap>();
            RoomSystem roomSystem = systems.GetComponent<RoomSystem>();
            MapUI mapUI = mapObject.GetComponent<MapUI>();

            SerializedObject serializedMap = new SerializedObject(mapRuntimeSystem);
            serializedMap.FindProperty("bootstrap").objectReferenceValue = bootstrap;
            serializedMap.FindProperty("roomSystem").objectReferenceValue = roomSystem;
            serializedMap.FindProperty("mapUI").objectReferenceValue = mapUI;
            FillMarkers(serializedMap.FindProperty("markers"), bootstrap != null ? bootstrap.StageDefinition : null);
            serializedMap.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(mapRuntimeSystem);

            mapObject.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded Stage1 map runtime.");
        }

        private static void FillMarkers(SerializedProperty property, StageDefinition stageDefinition)
        {
            if (stageDefinition == null)
            {
                property.arraySize = 0;
                return;
            }

            property.arraySize = stageDefinition.Rooms.Length;
            for (int index = 0; index < stageDefinition.Rooms.Length; index++)
            {
                RoomDefinition room = stageDefinition.Rooms[index];
                string roomId = room != null ? room.RoomId : "";
                Vector2 markerPosition = MarkerPositions.TryGetValue(roomId, out Vector2 position) ? position : Vector2.zero;
                SerializedProperty marker = property.GetArrayElementAtIndex(index);
                marker.FindPropertyRelative("roomId").stringValue = roomId;
                marker.FindPropertyRelative("markerAnchoredPosition").vector2Value = markerPosition;
            }
        }

        private static GameObject FindSceneObject(string name)
        {
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == name)
                    {
                        return child.gameObject;
                    }
                }
            }

            return null;
        }
    }
}
