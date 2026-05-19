using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Unity Multiplayer Play Mode 태그로 FishNet Network Flow의 서버/클라이언트 역할을 자동 시작합니다.
    /// `server` 태그만 서버 전용으로 처리하고, 그 외 태그와 무태그 인스턴스는 클라이언트 접속을 시도합니다.
    /// </summary>
    public sealed class FishNetMppmRoleBootstrapper : MonoBehaviour
    {
        [SerializeField] private FishNetNetworkFlowController networkFlowController;
        [SerializeField] private string serverAddress = "localhost";
        [SerializeField] private bool autoStartOnPlay = true;

        public enum AppRole
        {
            Server,
            Client
        }


        private void Start()
        {
            if (autoStartOnPlay)
            {
                StartFromCurrentPlayerTag();
            }
        }

        public void StartFromCurrentPlayerTag()
        {
#if UNITY_EDITOR
			IReadOnlyList<string> tagList = Unity.Multiplayer.PlayMode.CurrentPlayer.Tags;
            AppRole role = ResolveRole(tagList);
            ApplyRole(role, StartServer, StartClient, serverAddress);
#else
            // 에디터가 아닌 환경에서 빌드셋팅등으로 서버와 클라구분해야함
#endif
        }

        public static AppRole ResolveRole(IReadOnlyList<string> rawTags)
        {
            if (rawTags.Any(tag => string.Equals(tag?.Trim(), "server", StringComparison.OrdinalIgnoreCase)))
            {
                return AppRole.Server;
            }
            return AppRole.Client;
        }

        public static void ApplyRole(AppRole role, Action startServerOnly, Action<string> startClientOnly, string address)
        {
            if (role == AppRole.Server)
            {
                startServerOnly?.Invoke();
                return;
            }

            startClientOnly?.Invoke(string.IsNullOrWhiteSpace(address) ? "localhost" : address);
        }

        private void StartServer()
        {
            networkFlowController.StartServerOnly();
        }

        private void StartClient(string address)
        {
            networkFlowController.StartClient(address);
        }
    }
}
