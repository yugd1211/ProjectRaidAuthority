using System;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 서버가 Loot 요청을 판정한 뒤 남길 최소 결과 값입니다.
    /// 성공/실패 로그는 이 값으로 requestId, itemId, owner, 상태, 사유를 일관되게 출력합니다.
    /// </summary>
    [Serializable]
    public readonly struct LootResult
    {
        /// <summary>판정 대상 요청 식별자입니다.</summary>
        public LootRequestId RequestId { get; }

        /// <summary>판정 대상 전리품 인스턴스 식별자입니다.</summary>
        public LootItemIdentity ItemId { get; }

        /// <summary>서버 판정 성공 여부입니다.</summary>
        public bool Succeeded { get; }

        /// <summary>서버가 확정한 소유자 connection id입니다. 실패 시 -1입니다.</summary>
        public int OwnerId { get; }

        /// <summary>판정 후 전리품 상태입니다.</summary>
        public LootItemState State { get; }

        /// <summary>로그/검증용 결과 사유입니다.</summary>
        public string Reason { get; }

        /// <summary>Loot 결과 값을 생성합니다.</summary>
        /// <param name="requestId">판정 대상 요청 식별자입니다.</param>
        /// <param name="itemId">판정 대상 전리품 인스턴스 식별자입니다.</param>
        /// <param name="succeeded">서버 판정 성공 여부입니다.</param>
        /// <param name="ownerId">서버가 확정한 소유자 connection id입니다.</param>
        /// <param name="state">판정 후 전리품 상태입니다.</param>
        /// <param name="reason">로그/검증용 결과 사유입니다.</param>
        public LootResult(
            LootRequestId requestId,
            LootItemIdentity itemId,
            bool succeeded,
            int ownerId,
            LootItemState state,
            string reason)
        {
            RequestId = requestId;
            ItemId = itemId;
            Succeeded = succeeded;
            OwnerId = ownerId;
            State = state;
            Reason = reason;
        }
    }
}
