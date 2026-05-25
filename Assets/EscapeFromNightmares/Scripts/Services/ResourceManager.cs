using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// Resources.Load 호출을 한 곳에 모으고, 누락된 스프라이트에는 안전한 fallback을 제공합니다.
    /// </summary>
    public sealed class ResourceManager
    {
        private Sprite fallbackSprite;

        public ResourceManager(ResourcePathCatalog catalog)
        {
            Catalog = catalog;
        }

        /// <summary>리소스 경로 카탈로그입니다.</summary>
        public ResourcePathCatalog Catalog { get; }

        /// <summary>스프라이트를 로드하고, 누락된 경우 어두운 fallback 스프라이트를 반환합니다.</summary>
        public Sprite LoadSprite(string path)
        {
            var sprite = Load<Sprite>(path);
            return sprite != null ? sprite : GetFallbackSprite();
        }

        /// <summary>AudioClip 리소스를 로드합니다. 누락되면 null을 반환합니다.</summary>
        public AudioClip LoadAudioClip(string path)
        {
            return Load<AudioClip>(path);
        }

        /// <summary>Prefab GameObject 리소스를 로드합니다. 누락되면 null을 반환합니다.</summary>
        public GameObject LoadPrefab(string path)
        {
            return Load<GameObject>(path);
        }

        /// <summary>지정한 Resources 경로에서 UnityEngine.Object 타입 리소스를 로드합니다.</summary>
        public T Load<T>(string path) where T : Object
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return Resources.Load<T>(path);
        }

        private Sprite GetFallbackSprite()
        {
            if (fallbackSprite != null)
            {
                return fallbackSprite;
            }

            var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            var pixels = new Color[16 * 16];
            for (var index = 0; index < pixels.Length; index++)
            {
                pixels[index] = new Color(0.04f, 0.04f, 0.045f, 1f);
            }

            texture.SetPixels(pixels);
            texture.Apply();
            fallbackSprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f));
            fallbackSprite.name = "Fallback Resource Sprite";
            return fallbackSprite;
        }
    }
}
