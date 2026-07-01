using System;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// FishNet 타입 없이 루팅 소유권 transaction에서 다루는 최소 ItemInstance 상태입니다.
    /// </summary>
    [Serializable]
    public readonly struct ItemInstanceRecord
    {
        /// <summary>비어 있는 아이템 레코드입니다.</summary>
        public static readonly ItemInstanceRecord Empty = new(LootItemIdentity.Empty, LootItemState.Available, -1);

        /// <summary>루팅 transaction 대상 아이템 식별자입니다.</summary>
        public LootItemIdentity ItemId { get; }

        /// <summary>현재 서버 확정 아이템 상태입니다.</summary>
        public LootItemState State { get; }

        /// <summary>서버가 확정한 소유자 connection id입니다. 미소유 상태는 -1입니다.</summary>
        public int OwnerId { get; }

        /// <summary>아이템 인스턴스 레코드를 생성합니다.</summary>
        public ItemInstanceRecord(LootItemIdentity itemId, LootItemState state, int ownerId)
        {
            ItemId = itemId;
            State = state;
            OwnerId = ownerId;
        }

        /// <summary>소유권이 확정된 새 레코드를 반환합니다.</summary>
        public ItemInstanceRecord MarkLooted(int ownerId) => new(ItemId, LootItemState.Looted, ownerId);
    }
}
