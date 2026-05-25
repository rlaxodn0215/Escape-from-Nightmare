namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// GameSession의 인벤토리 상태를 감싸 아이템 획득과 선택을 담당하는 얇은 서비스입니다.
    /// </summary>
    public sealed class InventoryService
    {
        private readonly GameSession session;

        public InventoryService(GameSession session)
        {
            this.session = session;
        }

        /// <summary>아이템 ID를 인벤토리에 추가합니다. 이미 있거나 잘못된 ID면 실패합니다.</summary>
        public bool Acquire(string itemId)
        {
            return session.AddItem(itemId);
        }

        /// <summary>현재 인벤토리에 해당 아이템을 사용할 수 있는지 확인합니다.</summary>
        public bool CanUse(string itemId)
        {
            return session.HasItem(itemId);
        }

        /// <summary>인벤토리에서 사용할 아이템을 선택합니다.</summary>
        public void Select(string itemId)
        {
            session.SelectItem(itemId);
        }
    }
}
