namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// 닫힌 서랍이나 단서 확대 화면처럼 상호작용 오브젝트가 보여줄 수 있는 확대 상태입니다.
    /// </summary>
    public enum CloseUpState
    {
        /// <summary>아직 열리지 않았거나 기본 닫힘 이미지를 보여주는 상태입니다.</summary>
        Closed,
        /// <summary>보상 아이템이 남아 있는 열린 상태입니다.</summary>
        OpenWithItem,
        /// <summary>이미 아이템을 획득해 비어 있는 열린 상태입니다.</summary>
        OpenEmpty
    }
}
