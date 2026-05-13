using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// FishNet lobby player used by the sample room scene. The owning client can toggle Ready,
    /// and the server-side sample manager advances to gameplay when every room player is ready.
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
