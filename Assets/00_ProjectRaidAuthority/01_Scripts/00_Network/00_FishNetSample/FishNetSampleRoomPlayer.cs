using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 샘플 룸 씬에서 사용하는 FishNet 로비 플레이어입니다.
    /// 소유 클라이언트만 준비 상태를 바꿀 수 있으며,
    /// 모든 룸 플레이어가 준비되면 서버 측 샘플 매니저가 게임플레이 씬으로 진행합니다.
    /// </summary>
    public sealed class FishNetSampleRoomPlayer : NetworkBehaviour
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

        public override void OnStartClient()
        {
            base.OnStartClient();
        
            Debug.Log($"[FishNet샘플] 룸 플레이어가 참가했습니다. ownerId={OwnerId}, ready={readyToBegin.Value}");
        }

        private void OnGUI()
        {
            if (!IsOwner)
            {
                return;
            }

            GUI.Box(new Rect(20f, 190f, 280f, 96f), "FishNet 게임룸");
            GUI.Label(new Rect(36f, 220f, 248f, 24f), readyToBegin.Value ? "준비 상태입니다." : "준비를 눌러 게임을 시작하세요.");

            if (GUI.Button(new Rect(36f, 248f, 248f, 28f), readyToBegin.Value ? "준비 취소" : "준비"))
            {
                ServerSetReady(!readyToBegin.Value);
            }
        }

        [ServerRpc]
        private void ServerSetReady(bool ready)
        {
            readyToBegin.Value = ready;
        }

        private void OnReadyChanged(bool previous, bool next, bool asServer)
        {
            Debug.Log($"[FishNet샘플] 룸 플레이어 준비 상태 변경: {previous} -> {next}");

            if (asServer)
            {
                FindAnyObjectByType<FishNetSampleRoomManager>()?.NotifyRoomReadyChanged();
            }
        }
    }
}
