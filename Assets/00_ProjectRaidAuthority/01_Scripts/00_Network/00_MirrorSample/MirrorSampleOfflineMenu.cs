using Mirror;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Offline-scene UI bridge. Buttons call these methods, matching the blog's
    /// StartHost entry point while also offering a localhost client button.
    /// </summary>
    public sealed class MirrorSampleOfflineMenu : MonoBehaviour
    {
        [SerializeField] private NetworkRoomManager roomManager;
        [SerializeField] private string clientAddress = "localhost";
        [SerializeField] private bool showRuntimeControls = true;

        private NetworkRoomManager Manager => roomManager != null
            ? roomManager
            : NetworkManager.singleton as NetworkRoomManager;

        public void Configure(NetworkRoomManager manager, string address = "localhost")
        {
            roomManager = manager;
            clientAddress = string.IsNullOrWhiteSpace(address) ? "localhost" : address;
        }

        public void CreateRoom()
        {
            NetworkRoomManager manager = Manager;
            if (manager == null)
            {
                Debug.LogError("[MirrorSample] Cannot start host: NetworkRoomManager not found.");
                return;
            }

            manager.StartHost();
        }

        public void JoinLocalhost()
        {
            NetworkRoomManager manager = Manager;
            if (manager == null)
            {
                Debug.LogError("[MirrorSample] Cannot start client: NetworkRoomManager not found.");
                return;
            }

            manager.networkAddress = string.IsNullOrWhiteSpace(clientAddress) ? "localhost" : clientAddress;
            manager.StartClient();
        }

        private void OnGUI()
        {
            if (!showRuntimeControls)
            {
                return;
            }

            GUI.Box(new Rect(20f, 20f, 280f, 154f), "Mirror Room Sample");
            GUI.Label(new Rect(36f, 48f, 248f, 24f), "Offline -> GameRoom -> GamePlay");

            if (GUI.Button(new Rect(36f, 78f, 248f, 32f), "Create Room (Host)"))
            {
                CreateRoom();
            }

            if (GUI.Button(new Rect(36f, 118f, 248f, 32f), "Join Localhost (Client)"))
            {
                JoinLocalhost();
            }
        }

        public void QuitNetwork()
        {
            NetworkRoomManager manager = Manager;
            if (manager == null)
            {
                return;
            }

            if (NetworkServer.active && NetworkClient.isConnected)
            {
                manager.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                manager.StopClient();
            }
            else if (NetworkServer.active)
            {
                manager.StopServer();
            }
        }
    }
}
