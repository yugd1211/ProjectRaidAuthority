using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Match Room 씬에서 사용하는 per-connection RoomPlayer의 공유 상태입니다.
    /// Client 폴더는 준비 UI/요청, Server 폴더는 준비 확정/씬 진행 통지를 담당합니다.
    /// </summary>
    public sealed partial class RoomPlayer : NetworkBehaviour
    {
        private readonly SyncVar<bool> readyToBegin = new(false);

        public bool IsReadyToBegin => readyToBegin.Value;

        private void Awake()
        {
            readyToBegin.OnChange += OnReadyChanged;
        }

        private void OnDestroy()
        {
            readyToBegin.OnChange -= OnReadyChanged;
        }
    }
}
