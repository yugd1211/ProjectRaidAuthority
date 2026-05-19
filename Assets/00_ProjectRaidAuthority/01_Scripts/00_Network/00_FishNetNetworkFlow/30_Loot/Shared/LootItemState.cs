namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 서버 권한 Loot 스모크에서 아이템 소유권 확정 전후를 표현하는 최소 상태입니다.
    /// </summary>
    public enum LootItemState
    {
        /// <summary>아직 어떤 플레이어도 획득하지 않은 상태입니다.</summary>
        Available = 0,

        /// <summary>서버가 획득을 확정해 더 이상 획득할 수 없는 상태입니다.</summary>
        Looted = 1,
    }
}
