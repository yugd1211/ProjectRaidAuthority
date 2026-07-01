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
        [SerializeField] private FishNet.Object.NetworkObject lootItemPrefab;
        [SerializeField] private Vector3 lootItemSpawnPosition = new(0f, 0.5f, 3f);
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

        /// <summary>현재 FishNet 네트워크 흐름 설명을 반환합니다.</summary>
        public string FlowDescription => flowDescription;

        /// <summary>Match Room 임시 GUI 표시 여부를 반환합니다.</summary>
        public bool ShowMatchRoomGui => showMatchRoomGui;

        /// <summary>오프라인 부트스트랩 씬 경로를 반환합니다.</summary>
        public string OfflineBootstrapScene => offlineBootstrapScene;

        /// <summary>매치 룸 씬 경로를 반환합니다.</summary>
        public string MatchRoomScene => matchRoomScene;

        /// <summary>게임플레이 씬 경로를 반환합니다.</summary>
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

        /// <summary>테스트나 부트스트랩 코드에서 FishNet 흐름 의존성을 주입합니다.</summary>
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
            if (Application.isPlaying)
            {
                StopNetwork();
            }

            Unsubscribe();
        }

        private void OnApplicationQuit()
        {
            StopNetwork();
        }

        /// <summary>현재 NetworkManager의 로컬 서버와 클라이언트 연결을 정리합니다.</summary>
        public void StopNetwork()
        {
            StopNetwork(NetworkManagerInstance);
        }

        private static void ResetLocalNetworkStateForFreshStart(NetworkManager targetManager)
        {
            foreach (NetworkManager manager in NetworkManager.Instances)
            {
                StopNetwork(manager);
            }

            StopNetwork(targetManager);
        }

        private static void StopNetwork(NetworkManager manager)
        {
            if (manager == null)
            {
                return;
            }

            if (manager.ClientManager != null && manager.ClientManager.Started)
            {
                manager.ClientManager.StopConnection();
            }

            if (manager.ServerManager != null && manager.IsServerStarted)
            {
                manager.ServerManager.StopConnection(true);
            }

            manager.TransportManager?.Transport?.Shutdown();
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
