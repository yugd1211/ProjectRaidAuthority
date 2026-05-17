using FishNet.Managing;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class FishNetNetworkFlowController
    {
        public void StartHost()
        {
            NetworkManager manager = NetworkManagerInstance;
            if (manager == null)
            {
                Debug.LogError("[NetworkFlow] NetworkManager가 없어 호스트를 시작할 수 없습니다.");
                return;
            }

            Subscribe();

            if (!manager.IsServerStarted)
            {
                manager.ServerManager.StartConnection();
            }

            if (!manager.ClientManager.Started)
            {
                manager.ClientManager.StartConnection(clientAddress);
            }
        }

        public void StartClient(string address = null)
        {
            NetworkManager manager = NetworkManagerInstance;
            if (manager == null)
            {
                Debug.LogError("[NetworkFlow] NetworkManager가 없어 클라이언트를 시작할 수 없습니다.");
                return;
            }

            Subscribe();
            clientAddress = string.IsNullOrWhiteSpace(address) ? clientAddress : address;

            if (manager.ClientManager.Started)
            {
                Debug.Log("[NetworkFlow] 클라이언트가 이미 시작되어 접속 요청을 건너뜁니다.");
                return;
            }

            manager.ClientManager.StartConnection(clientAddress);
        }
    }
}
