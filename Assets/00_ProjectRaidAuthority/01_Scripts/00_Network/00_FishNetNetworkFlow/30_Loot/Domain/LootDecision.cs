using System;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 루팅 transaction 처리 결과입니다. FishNet adapter는 이 값을 로그와 projection 갱신에 사용합니다.
    /// </summary>
    [Serializable]
    public readonly struct LootDecision
    {
        /// <summary>판정 종류입니다.</summary>
        public LootDecisionKind Kind { get; }

        /// <summary>판정 대상 requestId입니다.</summary>
        public LootRequestId RequestId { get; }

        /// <summary>판정 대상 itemId입니다.</summary>
        public LootItemIdentity ItemId { get; }

        /// <summary>성공 여부입니다.</summary>
        public bool Succeeded => Kind == LootDecisionKind.Committed;

        /// <summary>중복 요청으로 무시된 결과인지 여부입니다.</summary>
        public bool IsDuplicate => Kind == LootDecisionKind.DuplicateIgnored;

        /// <summary>판정 후 소유자 connection id입니다. 실패 시 -1일 수 있습니다.</summary>
        public int OwnerId { get; }

        /// <summary>판정 후 아이템 상태입니다.</summary>
        public LootItemState State { get; }

        /// <summary>로그/검증용 사유입니다.</summary>
        public string Reason { get; }

        /// <summary>루팅 transaction 결과를 생성합니다.</summary>
        public LootDecision(
            LootDecisionKind kind,
            LootRequestId requestId,
            LootItemIdentity itemId,
            int ownerId,
            LootItemState state,
            string reason)
        {
            Kind = kind;
            RequestId = requestId;
            ItemId = itemId;
            OwnerId = ownerId;
            State = state;
            Reason = reason;
        }
    }
}
