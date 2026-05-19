using System.IO;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class FishNetNetworkFlowController
    {
        public void StartServerOnly()
        {
            NetworkManager manager = NetworkManagerInstance;
            if (manager == null)
            {
                Debug.LogError("[NetworkFlow] NetworkManager가 없어 서버 전용 모드를 시작할 수 없습니다.");
                return;
            }

            Subscribe();

            if (manager.IsServerStarted)
            {
                Debug.Log("[NetworkFlow] 서버가 이미 시작되어 서버 전용 시작 요청을 건너뜁니다.");
                return;
            }

            manager.ServerManager.StartConnection();
            Debug.Log("[NetworkFlow] 서버 전용 모드를 시작했습니다. 클라이언트는 함께 시작하지 않습니다.");
        }

        public void NotifyRoomPlayerReadyChanged()
        {
            if (!AreAllRoomPlayersReady())
            {
                return;
            }

            LoadGameplayScene();
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

            NetworkObject roomPlayer = NetworkManagerInstance.GetPooledInstantiated(roomPlayerPrefab, Vector3.zero, Quaternion.identity, true);
            NetworkManagerInstance.ServerManager.Spawn(roomPlayer, connection);
            Debug.Log($"[NetworkFlow] RoomPlayer를 스폰했습니다. clientId={connection.ClientId}");
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

        private bool AreAllRoomPlayersReady()
        {
            NetworkManager manager = NetworkManagerInstance;
            if (manager == null || !manager.IsServerStarted || gameplayLoadRequested)
            {
                return false;
            }

            RoomPlayer[] roomPlayers = FindObjectsByType<RoomPlayer>();
            if (roomPlayers.Length == 0)
            {
                return false;
            }

            foreach (RoomPlayer player in roomPlayers)
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
            NetworkManager manager = NetworkManagerInstance;
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
            Debug.Log("[NetworkFlow] 모든 RoomPlayer가 준비되었습니다. Gameplay 씬을 로드합니다.");
        }

        private void SpawnGameplayPlayers()
        {
            NetworkManager manager = NetworkManagerInstance;
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

            SpawnSmokeLootItem(manager);
        }

        private void SpawnSmokeLootItem(NetworkManager manager)
        {
            NetworkObject lootItem = manager.GetPooledInstantiated(lootItemPrefab, lootItemSpawnPosition, Quaternion.identity, true);
            manager.ServerManager.Spawn(lootItem);
            Debug.Log($"[FishNet Authority Smoke] LootItem 스폰: itemId=loot-smoke-item-001, position={lootItemSpawnPosition}");
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
