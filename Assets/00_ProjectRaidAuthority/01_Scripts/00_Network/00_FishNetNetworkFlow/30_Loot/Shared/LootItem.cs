using FishNet.Object;
using FishNet.Object.Synchronizing;
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

        private readonly SyncVar<LootItemState> state = new(LootItemState.Available);
        private readonly SyncVar<int> ownerId = new(-1);

        private Renderer[] presentationRenderers = System.Array.Empty<Renderer>();
        private Collider[] presentationColliders = System.Array.Empty<Collider>();

        private void Awake()
        {
            state.OnChange += OnLootStateChanged;
            presentationRenderers = GetComponentsInChildren<Renderer>(true);
            presentationColliders = GetComponentsInChildren<Collider>(true);
        }

        private void OnDestroy()
        {
            state.OnChange -= OnLootStateChanged;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            ApplyLootPresentationState(state.Value);
        }

        /// <summary>전리품 인스턴스 식별자입니다. 소유권 확정에는 사용하지 않습니다.</summary>
        public LootItemIdentity ItemId => new(itemId);

        /// <summary>스모크 확인용 표시 이름입니다.</summary>
        public string DisplayName => displayName;

        /// <summary>현재 전리품 상태입니다. 클라이언트가 이 값으로 소유권을 확정하지 않습니다.</summary>
        public LootItemState State => state.Value;

        /// <summary>서버가 확정한 소유자 connection id입니다. 미획득 상태에서는 -1입니다.</summary>
        public int LootOwnerId => ownerId.Value;

        /// <summary>아직 획득되지 않은 아이템인지 반환합니다.</summary>
        public bool IsAvailable => state.Value == LootItemState.Available;

        /// <summary>
        /// 서버 측 스모크/후속 RPC 구현에서만 호출할 소유권 확정 진입점입니다.
        /// </summary>
        /// <param name="serverOwnerId">서버가 검증 후 확정한 소유자 connection id입니다.</param>
        [Server]
        public void ServerMarkLooted(int serverOwnerId)
        {
            if (state.Value == LootItemState.Looted)
            {
                return;
            }

            ownerId.Value = serverOwnerId;
            state.Value = LootItemState.Looted;
        }

        private void OnLootStateChanged(LootItemState previous, LootItemState next, bool asServer)
        {
            ApplyLootPresentationState(next);
        }

        /// <summary>
        /// 획득된 전리품을 화면과 상호작용 후보에서만 제거합니다.
        /// 루트 GameObject, NetworkObject, LootItem은 유지해 서버의 already-looted 판정 근거를 보존합니다.
        /// </summary>
        private void ApplyLootPresentationState(LootItemState next)
        {
            bool shouldShow = next == LootItemState.Available;

            foreach (Renderer presentationRenderer in presentationRenderers)
            {
                presentationRenderer.enabled = shouldShow;
            }

            foreach (Collider presentationCollider in presentationColliders)
            {
                presentationCollider.enabled = shouldShow;
            }
        }
    }
}
