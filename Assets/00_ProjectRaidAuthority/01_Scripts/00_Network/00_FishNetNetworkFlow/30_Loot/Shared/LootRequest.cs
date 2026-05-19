using System;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 클라이언트가 서버에 보낼 Loot 획득 요청의 최소 공유 계약입니다.
    /// ServerRpc는 이 값 계약을 바탕으로 거리, 소유권, requestId 멱등성을 판정합니다.
    /// </summary>
    [Serializable]
    public readonly struct LootRequest
    {
        /// <summary>요청 중복 처리를 위한 식별자입니다.</summary>
        public LootRequestId RequestId { get; }

        /// <summary>획득하려는 전리품 인스턴스 식별자입니다.</summary>
        public LootItemIdentity ItemId { get; }

        /// <summary>Loot 요청 값을 생성합니다.</summary>
        /// <param name="requestId">요청 중복 처리를 위한 식별자입니다.</param>
        /// <param name="itemId">획득하려는 전리품 인스턴스 식별자입니다.</param>
        public LootRequest(LootRequestId requestId, LootItemIdentity itemId)
        {
            RequestId = requestId;
            ItemId = itemId;
        }

        /// <summary>요청에 필요한 식별자가 모두 채워졌는지 반환합니다.</summary>
        public bool IsValid => !RequestId.IsEmpty && !ItemId.IsEmpty;
    }
}
