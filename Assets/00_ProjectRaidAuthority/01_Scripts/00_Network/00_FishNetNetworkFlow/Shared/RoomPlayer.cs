using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Match Room 씬에서 사용하는 per-connection RoomPlayer의 FishNet/Unity adapter입니다.
    /// ready 흐름이 작게 유지되는 동안 단일 파일로 네트워크 계약과 UI 요청 경로를 함께 보여줍니다.
    /// </summary>
    public sealed class RoomPlayer : NetworkBehaviour
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

            Debug.Log($"[NetworkFlow] RoomPlayer가 참가했습니다. ownerId={OwnerId}, ready={readyToBegin.Value}");
        }

        private void OnGUI()
        {
            if (!IsOwner)
            {
                return;
            }

            GUI.Box(new Rect(20f, 190f, 280f, 96f), "FishNet Match Room");
            GUI.Label(new Rect(36f, 220f, 248f, 24f), readyToBegin.Value ? "준비 상태입니다." : "준비를 눌러 게임을 시작하세요.");

            if (GUI.Button(new Rect(36f, 248f, 248f, 28f), readyToBegin.Value ? "준비 취소" : "준비"))
            {
                ServerSetReadyToBegin(!readyToBegin.Value);
            }
        }

        [ServerRpc]
        private void ServerSetReadyToBegin(bool ready)
        {
            readyToBegin.Value = ready;
        }

        private void OnReadyChanged(bool previous, bool next, bool asServer)
        {
            Debug.Log($"[NetworkFlow] RoomPlayer 준비 상태 변경: {previous} -> {next}");

            if (asServer)
            {
                FindAnyObjectByType<FishNetNetworkFlowController>()?.NotifyRoomPlayerReadyChanged();
            }
        }
    }
}
