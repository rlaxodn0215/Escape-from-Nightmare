namespace EscapeFromNightmares.Services
{
    public sealed class InventoryService
    {
        private readonly GameSession session;

        public InventoryService(GameSession session)
        {
            this.session = session;
        }

        public bool Acquire(string itemId)
        {
            return session.AddItem(itemId);
        }

        public bool CanUse(string itemId)
        {
            return session.HasItem(itemId);
        }

        public void Select(string itemId)
        {
            session.SelectItem(itemId);
        }
    }
}
