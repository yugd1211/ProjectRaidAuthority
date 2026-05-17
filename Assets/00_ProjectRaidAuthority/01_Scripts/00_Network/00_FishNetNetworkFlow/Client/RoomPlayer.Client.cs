using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class RoomPlayer
    {
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
    }
}
