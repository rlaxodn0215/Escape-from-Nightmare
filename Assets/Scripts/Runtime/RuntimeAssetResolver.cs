using System.Collections.Generic;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;
using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Resolves runtime sprites and sounds from direct bindings, catalog assets, and stage data.
    /// </summary>
    public sealed class RuntimeAssetResolver
    {
        private readonly HashSet<string> missingSpriteWarnings = new HashSet<string>();
        private readonly Object warningContext;
        private readonly string defaultResourceCatalogPath;
        private ResourcePathCatalog resourceCatalog;
        private SpriteBinding[] spriteBindings;
        private SoundEntry[] soundBindings;
        private IEnumerable<SoundEntry> stageSounds;
        private SoundManager soundManager;

        public RuntimeAssetResolver(
            ResourcePathCatalog resourceCatalog,
            SpriteBinding[] spriteBindings,
            SoundEntry[] soundBindings,
            IEnumerable<SoundEntry> stageSounds,
            SoundManager soundManager,
            Object warningContext,
            string defaultResourceCatalogPath)
        {
            this.resourceCatalog = resourceCatalog;
            this.spriteBindings = spriteBindings ?? System.Array.Empty<SpriteBinding>();
            this.soundBindings = soundBindings ?? System.Array.Empty<SoundEntry>();
            this.stageSounds = stageSounds;
            this.soundManager = soundManager;
            this.warningContext = warningContext;
            this.defaultResourceCatalogPath = defaultResourceCatalogPath;
        }

        public ResourcePathCatalog ResourceCatalog => resourceCatalog;

        public void SetSoundManager(SoundManager manager)
        {
            soundManager = manager;
        }

        public void SetBindings(SpriteBinding[] sprites, SoundEntry[] sounds, IEnumerable<SoundEntry> stageSoundEntries)
        {
            spriteBindings = sprites ?? System.Array.Empty<SpriteBinding>();
            soundBindings = sounds ?? System.Array.Empty<SoundEntry>();
            stageSounds = stageSoundEntries;
        }

        public void EnsureResourceCatalog()
        {
            if (resourceCatalog != null)
            {
                return;
            }

            resourceCatalog = Resources.Load<ResourcePathCatalog>(defaultResourceCatalogPath);
            if (resourceCatalog == null)
            {
                Debug.LogWarning("ResourcePathCatalog not found at Resources/" + defaultResourceCatalogPath, warningContext);
            }
        }

        public Sprite ResolveSprite(string resourcePathOrId)
        {
            var spriteId = SpriteId(resourcePathOrId);
            if (BindingLookup.TryFindSprite(spriteBindings, spriteId, out var sprite))
            {
                return sprite;
            }

            EnsureResourceCatalog();
            if (resourceCatalog != null && resourceCatalog.TryFindSprite(spriteId, out sprite))
            {
                return sprite;
            }

            WarnMissingSprite(resourcePathOrId, spriteId);
            return null;
        }

        public void PlaySoundById(string soundId)
        {
            if (soundManager == null)
            {
                return;
            }

            if (BindingLookup.TryFindSound(soundBindings, soundId, out var entry)
                || (resourceCatalog != null && resourceCatalog.TryFindSound(soundId, out entry))
                || BindingLookup.TryFindSound(stageSounds, soundId, out entry))
            {
                soundManager.Play(entry);
            }
        }

        public static string SpriteId(string resourcePathOrId)
        {
            if (string.IsNullOrWhiteSpace(resourcePathOrId))
            {
                return string.Empty;
            }

            var slashIndex = resourcePathOrId.LastIndexOf('/');
            return slashIndex >= 0 ? resourcePathOrId.Substring(slashIndex + 1) : resourcePathOrId;
        }

        private void WarnMissingSprite(string resourcePathOrId, string spriteId)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (string.IsNullOrWhiteSpace(spriteId) || !missingSpriteWarnings.Add(spriteId))
            {
                return;
            }

            Debug.LogWarning("Sprite binding not found: " + resourcePathOrId + " (id: " + spriteId + ")", warningContext);
#endif
        }
    }
}
