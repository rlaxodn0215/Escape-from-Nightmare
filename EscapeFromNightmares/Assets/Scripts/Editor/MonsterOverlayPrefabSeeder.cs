using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class MonsterOverlayPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/Monster/MonsterOverlay.prefab";
        private const string SilhouetteSpritePath = "Assets/Sprites/Monster/monster_silhouette_window.png";
        private const string NearDetectionSpritePath = "Assets/Sprites/Monster/monster_near_detection.png";
        private const string ChaseSpritePath = "Assets/Sprites/Monster/monster_chase_overlay.png";
        private const string GameOverSpritePath = "Assets/Sprites/Monster/monster_gameover_shadow.png";

        [MenuItem("Escape From Nightmares/Seed Monster Overlay Prefab")]
        public static void Seed()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Prefabs/Monster");

            GameObject prefabRoot = BuildMonsterOverlay();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject monsterLayer = GameObject.Find("MonsterLayer");
            if (monsterLayer == null)
            {
                Debug.LogError("Stage1 scene is missing MonsterLayer.");
                return;
            }

            GameObject existing = FindChild(monsterLayer, "MonsterOverlay");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, monsterLayer.transform);
            instance.name = "MonsterOverlay";
            instance.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded MonsterOverlay prefab and Stage1 scene instance.");
        }

        private static GameObject BuildMonsterOverlay()
        {
            GameObject root = new GameObject("MonsterOverlay");
            MonsterOverlay overlay = root.AddComponent<MonsterOverlay>();

            GameObject visual = new GameObject("OverlayVisual");
            visual.transform.SetParent(root.transform, false);
            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite(SilhouetteSpritePath);
            renderer.sortingOrder = 50;
            renderer.enabled = false;

            SerializedObject serialized = new SerializedObject(overlay);
            serialized.FindProperty("overlayRenderer").objectReferenceValue = renderer;
            serialized.FindProperty("silhouetteSprite").objectReferenceValue = LoadSprite(SilhouetteSpritePath);
            serialized.FindProperty("nearDetectionSprite").objectReferenceValue = LoadSprite(NearDetectionSpritePath);
            serialized.FindProperty("chaseSprite").objectReferenceValue = LoadSprite(ChaseSpritePath);
            serialized.FindProperty("gameOverSprite").objectReferenceValue = LoadSprite(GameOverSpritePath);
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static Sprite LoadSprite(string path)
        {
            EnsureSpriteImport(path);

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                return sprite;
            }

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (Object asset in assets)
            {
                if (asset is Sprite nestedSprite)
                {
                    return nestedSprite;
                }
            }

            Debug.LogWarning($"Missing monster overlay sprite at {path}");
            return null;
        }

        private static void EnsureSpriteImport(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            if (importer.textureType == TextureImporterType.Sprite && importer.spriteImportMode == SpriteImportMode.Single)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string parent = System.IO.Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folder = System.IO.Path.GetFileName(folderPath);
            AssetDatabase.CreateFolder(parent, folder);
        }

        private static GameObject FindChild(GameObject parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            Transform[] children = parent.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child != parent.transform && child.name == childName)
                {
                    return child.gameObject;
                }
            }

            return null;
        }
    }
}
