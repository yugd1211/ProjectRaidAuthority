using System.IO;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// FishNet 룸 흐름 샘플 매니저입니다.
    /// FishNet에는 직접 대응되는 내장 룸 매니저가 없으므로,
    /// 이 컴포넌트가 FishNet 연결, 씬, 스폰 API를 사용해 3개 씬 샘플 흐름을 유지합니다.
    /// </summary>
    public sealed class FishNetSampleRoomManager : MonoBehaviour
    {
        [Header("FishNet")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private NetworkObject roomPlayerPrefab;
        [SerializeField] private NetworkObject gamePlayerPrefab;
        [SerializeField] private string offlineScene = "Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetSample/FishNetOffline.unity";
        [SerializeField] private string roomScene = "Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetSample/FishNetGameRoom.unity";
        [SerializeField] private string gameplayScene = "Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetSample/FishNetGamePlay.unity";
        [SerializeField] private string clientAddress = "localhost";
        [SerializeField] private bool showRoomGui = true;

        [Header("샘플 메모")]
        [TextArea(2, 5)]
        [SerializeField]
        private string sampleDescription =
            "FishNet 연결/씬/스폰 기능으로 구성한 룸 흐름 샘플입니다. " +
            "오프라인 씬에서 호스트/클라이언트를 시작하고, 룸 씬에서 준비를 누르면 게임플레이 씬으로 전환합니다.";

        private bool gameplayLoadRequested;

        public string SampleDescription => sampleDescription;
        public bool ShowRoomGui => showRoomGui;
        public string OfflineScene => offlineScene;
        public string RoomScene => roomScene;
        public string GameplayScene => gameplayScene;

        private NetworkManager Manager
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
            NetworkObject roomPrefab,
            NetworkObject gamePrefab,
            string offlineScenePath,
            string roomScenePath,
            string gameplayScenePath,
            string address = "localhost")
        {
            networkManager = manager;
            roomPlayerPrefab = roomPrefab;
            gamePlayerPrefab = gamePrefab;
            offlineScene = offlineScenePath;
            roomScene = roomScenePath;
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

        public void StartHost()
        {
            NetworkManager manager = Manager;
            if (manager == null)
            {
                Debug.LogError("[FishNet샘플] NetworkManager가 없어 호스트를 시작할 수 없습니다.");
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
            NetworkManager manager = Manager;
            if (manager == null)
            {
                Debug.LogError("[FishNet샘플] NetworkManager가 없어 클라이언트를 시작할 수 없습니다.");
                return;
            }

            Subscribe();
            clientAddress = string.IsNullOrWhiteSpace(address) ? clientAddress : address;
            manager.ClientManager.StartConnection(clientAddress);
        }

        public void StopNetwork()
        {
            NetworkManager manager = Manager;
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

        public void NotifyRoomReadyChanged()
        {
            if (!IsServerReadyToLoadGameplay())
            {
                return;
            }

            LoadGameplayScene();
        }

        private void Subscribe()
        {
            NetworkManager manager = Manager;
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

        private void SceneManager_OnClientLoadedStartScenes(NetworkConnection connection, bool asServer)
        {
            if (!asServer || roomPlayerPrefab == null || connection == null || !connection.IsValid)
            {
                return;
            }

            if (connection.FirstObject != null)
            {
                return;
            }

            NetworkObject roomPlayer = Manager.GetPooledInstantiated(roomPlayerPrefab, Vector3.zero, Quaternion.identity, true);
            Manager.ServerManager.Spawn(roomPlayer, connection);
            Debug.Log($"[FishNet샘플] 룸 플레이어를 스폰했습니다. clientId={connection.ClientId}");
        }

        private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs args)
        {
            if (!gameplayLoadRequested || !args.QueueData.AsServer)
            {
                return;
            }

            gameplayLoadRequested = false;
            SpawnGameplayPlayers();
        }

        private bool IsServerReadyToLoadGameplay()
        {
            NetworkManager manager = Manager;
            if (manager == null || !manager.IsServerStarted || gameplayLoadRequested)
            {
                return false;
            }

            FishNetSampleRoomPlayer[] roomPlayers = FindObjectsByType<FishNetSampleRoomPlayer>(FindObjectsInactive.Exclude);
            if (roomPlayers.Length == 0)
            {
                return false;
            }

            foreach (FishNetSampleRoomPlayer player in roomPlayers)
            {
                if (!player.IsReadyToBegin)
                {
                    return false;
                }
            }

            return true;
        }

        private void LoadGameplayScene()
        {
            NetworkManager manager = Manager;
            if (manager == null)
            {
                return;
            }

            gameplayLoadRequested = true;
            SceneLoadData loadData = new(GetSceneName(gameplayScene))
            {
                ReplaceScenes = ReplaceOption.All
            };
            manager.SceneManager.LoadGlobalScenes(loadData);
            Debug.Log("[FishNet샘플] 모든 룸 플레이어가 준비되었습니다. 게임플레이 씬을 로드합니다.");
        }

        private void SpawnGameplayPlayers()
        {
            NetworkManager manager = Manager;
            if (manager == null || gamePlayerPrefab == null || !manager.IsServerStarted)
            {
                return;
            }

            int index = 0;
            foreach (NetworkConnection connection in manager.ServerManager.Clients.Values)
            {
                if (connection == null || !connection.IsValid)
                {
                    continue;
                }

                Vector3 position = GetSpawnPosition(index++);
                NetworkObject player = manager.GetPooledInstantiated(gamePlayerPrefab, position, Quaternion.identity, true);
                manager.ServerManager.Spawn(player, connection);
            }
        }

        private static Vector3 GetSpawnPosition(int index)
        {
            return new Vector3((index % 4 - 1.5f) * 2f, 1f, (index / 4) * 2f);
        }

        private static string GetSceneName(string scenePath)
        {
            return Path.GetFileNameWithoutExtension(scenePath);
        }
    }
}
