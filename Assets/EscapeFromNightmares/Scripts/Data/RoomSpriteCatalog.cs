using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Room Sprite Catalog")]
    public sealed class RoomSpriteCatalog : ScriptableObject
    {
        [SerializeField] private List<SpriteEntry> sprites = new List<SpriteEntry>();

        public IReadOnlyList<SpriteEntry> Sprites => sprites;

        public bool TryFind(string spriteId, out Sprite sprite)
        {
            foreach (var entry in sprites)
            {
                if (entry != null && entry.spriteId == spriteId && entry.sprite != null)
                {
                    sprite = entry.sprite;
                    return true;
                }
            }

            sprite = null;
            return false;
        }

        public void SetSprites(IEnumerable<SpriteEntry> entries)
        {
            sprites.Clear();
            if (entries == null)
            {
                return;
            }

            foreach (var entry in entries)
            {
                if (entry != null)
                {
                    sprites.Add(entry);
                }
            }
        }
    }

    [Serializable]
    public sealed class SpriteEntry
    {
        public string spriteId;
        public Sprite sprite;
    }
}
