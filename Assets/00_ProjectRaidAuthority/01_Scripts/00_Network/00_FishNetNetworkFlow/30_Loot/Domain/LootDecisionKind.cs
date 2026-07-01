namespace ProjectRaidAuthority.Networking
{
    /// <summary>루팅 transaction의 서버 판정 종류입니다.</summary>
    public enum LootDecisionKind
    {
        Committed,
        DuplicateIgnored,
        RejectedInvalidRequest,
        RejectedNotFound,
        RejectedInvalidDistance,
        RejectedAlreadyLooted,
        RejectedInvalidState
    }
}
