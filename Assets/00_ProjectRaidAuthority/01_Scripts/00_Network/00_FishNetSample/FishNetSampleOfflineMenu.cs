using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// FishNet 샘플 오프라인 씬의 UI 연결 컴포넌트입니다.
    /// 버튼 입력을 샘플 룸 매니저로 전달해 호스트 또는 로컬호스트 클라이언트를 시작합니다.
    /// </summary>
    public sealed class FishNetSampleOfflineMenu : MonoBehaviour
    {
        [SerializeField] private FishNetSampleRoomManager roomManager;
        [SerializeField] private string clientAddress = "localhost";
        [SerializeField] private bool showRuntimeControls = true;

        private FishNetSampleRoomManager Manager => roomManager != null
            ? roomManager
            : FindAnyObjectByType<FishNetSampleRoomManager>();

        public void Configure(FishNetSampleRoomManager manager, string address = "localhost")
        {
            roomManager = manager;
            clientAddress = string.IsNullOrWhiteSpace(address) ? "localhost" : address;
        }

        public void CreateRoom()
        {
            FishNetSampleRoomManager manager = Manager;
            if (manager == null)
            {
                Debug.LogError("[FishNet샘플] 호스트를 시작할 수 없습니다: 샘플 룸 매니저를 찾을 수 없습니다.");
                return;
            }

            manager.StartHost();
        }

        public void JoinLocalhost()
        {
            FishNetSampleRoomManager manager = Manager;
            if (manager == null)
            {
                Debug.LogError("[FishNet샘플] 클라이언트를 시작할 수 없습니다: 샘플 룸 매니저를 찾을 수 없습니다.");
                return;
            }

            manager.StartClient(clientAddress);
        }

        public void QuitNetwork()
        {
            Manager?.StopNetwork();
        }

        private void OnGUI()
        {
            if (!showRuntimeControls)
            {
                return;
            }

            GUI.Box(new Rect(20f, 20f, 280f, 154f), "FishNet 룸 샘플");
            GUI.Label(new Rect(36f, 48f, 248f, 24f), "오프라인 -> 게임룸 -> 게임플레이");

            if (GUI.Button(new Rect(36f, 78f, 248f, 32f), "방 만들기(호스트)"))
            {
                CreateRoom();
            }

            if (GUI.Button(new Rect(36f, 118f, 248f, 32f), "로컬호스트 참가(클라이언트)"))
            {
                JoinLocalhost();
            }
        }
    }
}
