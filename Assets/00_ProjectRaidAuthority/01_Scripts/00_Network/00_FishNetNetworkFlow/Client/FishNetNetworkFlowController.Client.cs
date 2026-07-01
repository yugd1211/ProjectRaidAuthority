using FishNet.Managing;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class FishNetNetworkFlowController
    {
        /// <summary>기존 로컬 FishNet 연결을 정리한 뒤 호스트 연결 시작을 요청합니다.</summary>
        public void StartHost()
        {
            NetworkManager manager = NetworkManagerInstance;
            if (manager == null)
            {
                Debug.LogError("[NetworkFlow] NetworkManager가 없어 호스트를 시작할 수 없습니다.");
                return;
            }

            ResetLocalNetworkStateForFreshStart(manager);
            Subscribe();

            manager.ServerManager.StartConnection();
            manager.ClientManager.StartConnection(clientAddress);
        }

        /// <summary>기존 로컬 FishNet 연결을 정리한 뒤 지정 주소로 클라이언트 연결 시작을 요청합니다.</summary>
        public void StartClient(string address = null)
        {
            NetworkManager manager = NetworkManagerInstance;
            if (manager == null)
            {
                Debug.LogError("[NetworkFlow] NetworkManager가 없어 클라이언트를 시작할 수 없습니다.");
                return;
            }

            ResetLocalNetworkStateForFreshStart(manager);
            Subscribe();
            clientAddress = string.IsNullOrWhiteSpace(address) ? clientAddress : address;

            manager.ClientManager.StartConnection(clientAddress);
        }
    }
}
