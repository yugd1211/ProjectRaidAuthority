using System.Collections.Generic;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 현재 FishNet Game Server 안에서만 사용하는 임시 ItemInstance 저장소입니다.
    /// 향후 DB-backed repository로 교체될 수 있도록 FishNet/Unity 타입을 참조하지 않습니다.
    /// </summary>
    public sealed class InMemoryItemInstanceRepository
    {
        private readonly Dictionary<LootItemIdentity, ItemInstanceRecord> records = new();

        /// <summary>아이템 레코드를 추가하거나 최신 projection 상태로 갱신합니다.</summary>
        public void Upsert(ItemInstanceRecord record)
        {
            if (record.ItemId.IsEmpty)
            {
                return;
            }

            records[record.ItemId] = record;
        }

        /// <summary>아이템 레코드 조회를 시도합니다.</summary>
        public bool TryGet(LootItemIdentity itemId, out ItemInstanceRecord record)
        {
            return records.TryGetValue(itemId, out record);
        }
    }
}
