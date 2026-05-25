using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// 방 배경이나 특수 스프라이트를 ID로 찾아오기 위한 ScriptableObject 카탈로그입니다.
    /// </summary>
    [CreateAssetMenu(menuName = "Escape From Nightmares/Room Sprite Catalog")]
    public sealed class RoomSpriteCatalog : ScriptableObject
    {
        [SerializeField] private List<SpriteEntry> sprites = new List<SpriteEntry>();

        /// <summary>현재 카탈로그에 등록된 스프라이트 항목입니다.</summary>
        public IReadOnlyList<SpriteEntry> Sprites => sprites;

        /// <summary>
        /// 지정한 spriteId와 일치하는 스프라이트를 찾습니다.
        /// </summary>
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

        /// <summary>
        /// 에디터 빌더가 생성한 항목으로 카탈로그 내용을 교체합니다.
        /// </summary>
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

    /// <summary>
    /// 코드에서 사용하는 스프라이트 ID와 실제 Unity Sprite 참조를 묶는 항목입니다.
    /// </summary>
    [Serializable]
    public sealed class SpriteEntry
    {
        public string spriteId;
        public Sprite sprite;
    }
}
