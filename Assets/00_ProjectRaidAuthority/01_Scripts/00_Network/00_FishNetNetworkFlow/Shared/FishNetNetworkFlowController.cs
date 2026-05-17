using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// FishNet Match Room 흐름의 공유 설정과 생명주기를 보관하는 NetworkFlowController입니다.
    /// Client 폴더는 접속 진입점, Server 폴더는 씬 전환/스폰 확정 로직을 담당합니다.
    /// </summary>
    public sealed partial class FishNetNetworkFlowController : MonoBehaviour
    {
        [Header("FishNet")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private FishNet.Object.NetworkObject roomPlayerPrefab;
        [SerializeField] private FishNet.Object.NetworkObject gamePlayerPrefab;
        [SerializeField] private string offlineBootstrapScene = "Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/OfflineBootstrap.unity";
        [SerializeField] private string matchRoomScene = "Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/MatchRoom.unity";
        [SerializeField] private string gameplayScene = "Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/Gameplay.unity";
        [SerializeField] private string clientAddress = "localhost";
        [SerializeField] private bool showMatchRoomGui = true;

        [Header("네트워크 흐름 메모")]
        [TextArea(2, 5)]
        [SerializeField]
        private string flowDescription =
            "FishNet 연결/씬/스폰 기능으로 구성한 Match Room 네트워크 흐름입니다. " +
            "Offline 씬에서 역할을 판정하고, 클라이언트는 단일 서버 매칭 후 Match Room을 거쳐 Gameplay 씬으로 전환합니다.";

        private bool gameplayLoadRequested;

        public string FlowDescription => flowDescription;
        public bool ShowMatchRoomGui => showMatchRoomGui;
        public string OfflineBootstrapScene => offlineBootstrapScene;
        public string MatchRoomScene => matchRoomScene;
        public string GameplayScene => gameplayScene;

        private NetworkManager NetworkManagerInstance
        {
            get
            {
                if (networkManager == null)
                {
                    networkManager = GetComponent<NetworkManager>() ?? FindAnyObjectByType<NetworkManager>();
                }

                return networkManager;
            }
        }

        public void Configure(
            NetworkManager manager,
            FishNet.Object.NetworkObject roomPrefab,
            FishNet.Object.NetworkObject gamePrefab,
            string offlineBootstrapScenePath,
            string matchRoomScenePath,
            string gameplayScenePath,
            string address = "localhost")
        {
            networkManager = manager;
            roomPlayerPrefab = roomPrefab;
            gamePlayerPrefab = gamePrefab;
            offlineBootstrapScene = offlineBootstrapScenePath;
            matchRoomScene = matchRoomScenePath;
            gameplayScene = gameplayScenePath;
            clientAddress = string.IsNullOrWhiteSpace(address) ? "localhost" : address;
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void Start()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void StopNetwork()
        {
            NetworkManager manager = NetworkManagerInstance;
            if (manager == null)
            {
                return;
            }

            if (manager.ClientManager.Started)
            {
                manager.ClientManager.StopConnection();
            }

            if (manager.IsServerStarted)
            {
                manager.ServerManager.StopConnection(true);
            }
        }

        private void Subscribe()
        {
            NetworkManager manager = NetworkManagerInstance;
            if (manager == null || manager.SceneManager == null)
            {
                return;
            }

            manager.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientLoadedStartScenes;
            manager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;
            manager.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
            manager.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        }

        private void Unsubscribe()
        {
            NetworkManager manager = networkManager;
            if (manager == null || manager.SceneManager == null)
            {
                return;
            }

            manager.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientLoadedStartScenes;
            manager.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
        }
    }
}
