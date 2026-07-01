using System;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// FishNet adapter가 scene 검증을 마친 뒤 순수 루팅 transaction 경계로 넘기는 command입니다.
    /// </summary>
    [Serializable]
    public readonly struct LootCommand
    {
        /// <summary>클라이언트가 보낸 requestId와 itemId입니다.</summary>
        public LootRequest Request { get; }

        /// <summary>서버가 확정하려는 소유자 connection id입니다.</summary>
        public int OwnerId { get; }

        /// <summary>FishNet/Unity adapter가 현재 scene에서 대상 아이템 projection을 찾았는지 여부입니다.</summary>
        public bool IsItemPresent { get; }

        /// <summary>FishNet/Unity adapter가 거리 검증을 통과했다고 판단했는지 여부입니다.</summary>
        public bool IsDistanceValid { get; }

        /// <summary>루팅 transaction command를 생성합니다.</summary>
        public LootCommand(LootRequest request, int ownerId, bool isDistanceValid, bool isItemPresent = true)
        {
            Request = request;
            OwnerId = ownerId;
            IsItemPresent = isItemPresent;
            IsDistanceValid = isDistanceValid;
        }
    }
}
