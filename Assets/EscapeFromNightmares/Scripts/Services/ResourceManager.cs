using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Services
{
    public sealed class ResourceManager
    {
        private Sprite fallbackSprite;

        public ResourceManager(ResourcePathCatalog catalog)
        {
            Catalog = catalog;
        }

        public ResourcePathCatalog Catalog { get; }

        public Sprite LoadSprite(string path)
        {
            var sprite = Load<Sprite>(path);
            return sprite != null ? sprite : GetFallbackSprite();
        }

        public AudioClip LoadAudioClip(string path)
        {
            return Load<AudioClip>(path);
        }

        public GameObject LoadPrefab(string path)
        {
            return Load<GameObject>(path);
        }

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
