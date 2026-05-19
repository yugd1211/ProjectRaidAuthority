using FishNet.Object;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 서버 권한 Loot 스모크의 테스트용 전리품 네트워크 adapter입니다.
    /// 획득 판정은 GamePlayer의 서버 권한 Loot RPC가 수행하고, 이 컴포넌트는 서버 확정 상태를 보관합니다.
    /// </summary>
    public sealed class LootItem : NetworkBehaviour
    {
        [Header("Smoke Identity")]
        [SerializeField] private string itemId = "loot-smoke-item-001";
        [SerializeField] private string displayName = "Smoke Loot Item";

        [Header("Server-owned State")]
        [SerializeField] private LootItemState state = LootItemState.Available;
        [SerializeField] private int ownerId = -1;

        /// <summary>전리품 인스턴스 식별자입니다. 소유권 확정에는 사용하지 않습니다.</summary>
        public LootItemIdentity ItemId => new(itemId);

        /// <summary>스모크 확인용 표시 이름입니다.</summary>
        public string DisplayName => displayName;

        /// <summary>현재 전리품 상태입니다. 클라이언트가 이 값으로 소유권을 확정하지 않습니다.</summary>
        public LootItemState State => state;

        /// <summary>서버가 확정한 소유자 connection id입니다. 미획득 상태에서는 -1입니다.</summary>
        public int LootOwnerId => ownerId;

        /// <summary>아직 획득되지 않은 아이템인지 반환합니다.</summary>
        public bool IsAvailable => state == LootItemState.Available;

        /// <summary>
        /// 서버 측 스모크/후속 RPC 구현에서만 호출할 소유권 확정 진입점입니다.
        /// </summary>
        /// <param name="serverOwnerId">서버가 검증 후 확정한 소유자 connection id입니다.</param>
        [Server]
        public void ServerMarkLooted(int serverOwnerId)
        {
            if (state == LootItemState.Looted)
            {
                return;
            }

            ownerId = serverOwnerId;
            state = LootItemState.Looted;
        }
    }
}
