using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Maps stable sound IDs to Resources paths and playback categories.
    /// </summary>
    [CreateAssetMenu(menuName = "Escape From Nightmares/Sound Catalog")]
    public sealed class SoundCatalog : ScriptableObject
    {
        public List<SoundEntry> entries = new List<SoundEntry>();

        public bool TryFind(string soundId, out SoundEntry entry)
        {
            foreach (var candidate in entries)
            {
                if (candidate != null && candidate.soundId == soundId)
                {
                    entry = candidate;
                    return true;
                }
            }

            entry = null;
            return false;
        }
    }

    [Serializable]
    public sealed class SoundEntry
    {
        public string soundId;
        public string resourcePath;
        public SoundCategory category = SoundCategory.Sfx;
        public bool loop;
    }
}
