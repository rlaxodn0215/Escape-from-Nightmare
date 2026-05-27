using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Direct Unity asset references keyed by the resource-style ids used in stage data.
    /// </summary>
    [CreateAssetMenu(fileName = "ResourcePathCatalog", menuName = "Escape From Nightmares/Resource Path Catalog")]
    public sealed class ResourcePathCatalog : ScriptableObject
    {
        [SerializeField] private SpriteBinding[] spriteBindings = Array.Empty<SpriteBinding>();
        [SerializeField] private SoundEntry[] soundBindings = Array.Empty<SoundEntry>();

        public IReadOnlyList<SpriteBinding> SpriteBindings => spriteBindings;
        public IReadOnlyList<SoundEntry> SoundBindings => soundBindings;

        public bool TryFindSprite(string spriteId, out Sprite sprite)
        {
            return BindingLookup.TryFindSprite(spriteBindings, spriteId, out sprite);
        }

        public bool TryFindSound(string soundId, out SoundEntry entry)
        {
            return BindingLookup.TryFindSound(soundBindings, soundId, out entry);
        }

        public void SetSpriteBindings(IEnumerable<SpriteBinding> bindings)
        {
            spriteBindings = ToArray(bindings);
        }

        public void SetSoundBindings(IEnumerable<SoundEntry> bindings)
        {
            soundBindings = ToArray(bindings);
        }

        private static T[] ToArray<T>(IEnumerable<T> entries)
        {
            if (entries == null)
            {
                return Array.Empty<T>();
            }

            var list = new List<T>();
            foreach (var entry in entries)
            {
                if (entry != null)
                {
                    list.Add(entry);
                }
            }

            return list.ToArray();
        }
    }
}
