using System;
using System.Collections;
using System.IO;
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

        private bool started;

        public enum MppmRole
        {
            ServerOnly,
            ClientOnly
        }

        private FishNetNetworkFlowController Controller => networkFlowController != null
            ? networkFlowController
            : FindAnyObjectByType<FishNetNetworkFlowController>();

        public void Configure(
            FishNetNetworkFlowController controller,
            string address = "localhost",
            bool autoStart = true)
        {
            networkFlowController = controller;
            serverAddress = string.IsNullOrWhiteSpace(address) ? "localhost" : address;
            autoStartOnPlay = autoStart;
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
            if (started)
            {
                Debug.Log("[NetworkFlow] MPPM 태그 자동 시작이 이미 처리되어 재호출을 건너뜁니다.");
                return;
            }

#if UNITY_EDITOR
            object tags = ReadCurrentPlayerTags();
            MppmRole role = ResolveRole(tags);
            Debug.Log($"[NetworkFlow] MPPM 태그 판정: tag={FormatTags(tags)}, role={role}");
            ApplyRole(role, StartServerOnly, StartClientOnly, serverAddress);
            started = true;
#else
            Debug.Log("[NetworkFlow] MPPM 태그 자동 시작은 Unity Editor 전용입니다.");
#endif
        }

        public static MppmRole ResolveRole(string tag)
        {
            return string.Equals(tag?.Trim(), "server", StringComparison.OrdinalIgnoreCase)
                ? MppmRole.ServerOnly
                : MppmRole.ClientOnly;
        }

        public static MppmRole ResolveRole(IEnumerable tags)
        {
            if (tags == null)
            {
                return MppmRole.ClientOnly;
            }

            foreach (object tag in tags)
            {
                if (ResolveRole(tag?.ToString()) == MppmRole.ServerOnly)
                {
                    return MppmRole.ServerOnly;
                }
            }

            return MppmRole.ClientOnly;
        }

        public static MppmRole ResolveRole(object rawTags)
        {
            return rawTags switch
            {
                string tag => ResolveRole(tag),
                IEnumerable tags => ResolveRole(tags),
                _ => ResolveRole(rawTags?.ToString())
            };
        }

        public static void ApplyRole(MppmRole role, Action startServerOnly, Action<string> startClientOnly, string address)
        {
            if (role == MppmRole.ServerOnly)
            {
                startServerOnly?.Invoke();
                return;
            }

            startClientOnly?.Invoke(string.IsNullOrWhiteSpace(address) ? "localhost" : address);
        }

        private void StartServerOnly()
        {
            FishNetNetworkFlowController controller = Controller;
            if (controller == null)
            {
                Debug.LogError("[NetworkFlow] MPPM 서버 역할을 시작할 수 없습니다: Match Room 매니저를 찾을 수 없습니다.");
                return;
            }

            Debug.Log("[NetworkFlow] MPPM 태그가 server이므로 서버 전용 역할을 시작합니다.");
            controller.StartServerOnly();
        }

        private void StartClientOnly(string address)
        {
            FishNetNetworkFlowController controller = Controller;
            if (controller == null)
            {
                Debug.LogError("[NetworkFlow] MPPM 클라이언트 역할을 시작할 수 없습니다: Match Room 매니저를 찾을 수 없습니다.");
                return;
            }

            Debug.Log($"[NetworkFlow] MPPM 태그가 server가 아니므로 클라이언트 접속을 시도합니다. address={address}");
            controller.StartClient(address);
        }

        private static object ReadCurrentPlayerTags()
        {
#if UNITY_EDITOR
            object currentPlayerTags = Unity.Multiplayer.PlayMode.CurrentPlayer.Tags;
            if (HasAnyTag(currentPlayerTags as IEnumerable))
            {
                return currentPlayerTags;
            }

            string fallbackTags = ReadTagsFromMppmSystemData(Unity.Multiplayer.PlayMode.CurrentPlayer.IsMainEditor);
            return string.IsNullOrWhiteSpace(fallbackTags) ? currentPlayerTags : fallbackTags;
#else
            return null;
#endif
        }

        private static bool HasAnyTag(IEnumerable tags)
        {
            if (tags == null)
            {
                return false;
            }

            foreach (object tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag?.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        private static string ReadTagsFromMppmSystemData(bool isMainEditor)
        {
            string systemDataPath = FindMppmSystemDataPath();
            if (string.IsNullOrWhiteSpace(systemDataPath))
            {
                return string.Empty;
            }

            string json;
            try
            {
                json = File.ReadAllText(systemDataPath);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[NetworkFlow] MPPM 태그 fallback 파일을 읽지 못했습니다: {ex.Message}");
                return string.Empty;
            }

            string virtualProjectId = FindVirtualProjectIdFromDataPath();
            return ExtractTagsFromMppmSystemData(json, virtualProjectId, isMainEditor);
        }

        private static string FindMppmSystemDataPath()
        {
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            while (directory != null)
            {
                string candidate = Path.Combine(directory.FullName, "Library", "VP", "SystemData.json");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                candidate = Path.Combine(directory.FullName, "VP", "SystemData.json");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            return string.Empty;
        }

        private static string FindVirtualProjectIdFromDataPath()
        {
            string normalizedPath = Application.dataPath.Replace('\\', '/');
            Match match = Regex.Match(
                normalizedPath,
                @"/Library/VP/mppm(?<id>[^/]+)/Assets$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            return match.Success ? match.Groups["id"].Value : string.Empty;
        }

        public static string ExtractTagsFromMppmSystemData(string json, string virtualProjectId, bool isMainEditor)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return string.Empty;
            }

            int markerIndex = -1;
            if (!string.IsNullOrWhiteSpace(virtualProjectId))
            {
                markerIndex = json.IndexOf($"\"m_Id\": \"{virtualProjectId}\"", StringComparison.OrdinalIgnoreCase);
            }

            if (markerIndex < 0 && (isMainEditor || string.IsNullOrWhiteSpace(virtualProjectId)))
            {
                markerIndex = json.IndexOf("\"Name\": \"Main Editor\"", StringComparison.OrdinalIgnoreCase);
            }

            if (markerIndex < 0)
            {
                return string.Empty;
            }

            string entry = ExtractJsonObjectAround(json, markerIndex);
            return ExtractTagsArray(entry);
        }

        private static string ExtractJsonObjectAround(string json, int markerIndex)
        {
            int entryLineIndex = json.LastIndexOf("\n        \"", markerIndex, StringComparison.Ordinal);
            if (entryLineIndex < 0)
            {
                entryLineIndex = json.LastIndexOf("\n    \"", markerIndex, StringComparison.Ordinal);
            }

            int objectStart = entryLineIndex < 0
                ? json.LastIndexOf('{', markerIndex)
                : json.IndexOf('{', entryLineIndex);
            if (objectStart < 0)
            {
                return string.Empty;
            }

            int depth = 0;
            bool inString = false;
            bool escaped = false;
            for (int i = objectStart; i < json.Length; i++)
            {
                char current = json[i];
                if (inString)
                {
                    escaped = current == '\\' && !escaped;
                    if (current == '"' && !escaped)
                    {
                        inString = false;
                    }
                    else if (current != '\\')
                    {
                        escaped = false;
                    }

                    continue;
                }

                if (current == '"')
                {
                    inString = true;
                    escaped = false;
                    continue;
                }

                if (current == '{')
                {
                    depth++;
                }
                else if (current == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return json.Substring(objectStart, i - objectStart + 1);
                    }
                }
            }

            return string.Empty;
        }

        private static string ExtractTagsArray(string jsonObject)
        {
            int tagsIndex = jsonObject.IndexOf("\"Tags\"", StringComparison.OrdinalIgnoreCase);
            if (tagsIndex < 0)
            {
                return string.Empty;
            }

            int arrayStart = jsonObject.IndexOf('[', tagsIndex);
            int arrayEnd = arrayStart < 0 ? -1 : jsonObject.IndexOf(']', arrayStart);
            if (arrayStart < 0 || arrayEnd < 0)
            {
                return string.Empty;
            }

            string arrayText = jsonObject.Substring(arrayStart, arrayEnd - arrayStart + 1);
            MatchCollection matches = Regex.Matches(
                arrayText,
                "\"(?<tag>(?:\\\\.|[^\"])*)\"",
                RegexOptions.CultureInvariant);

            string label = string.Empty;
            foreach (Match match in matches)
            {
                string tag = Regex.Unescape(match.Groups["tag"].Value);
                if (string.IsNullOrWhiteSpace(tag))
                {
                    continue;
                }

                label = string.IsNullOrEmpty(label) ? tag : $"{label},{tag}";
            }

            return label;
        }
#endif

        private static string FormatTags(object rawTags)
        {
            if (rawTags == null)
            {
                return "무태그";
            }

            if (rawTags is string tag)
            {
                return string.IsNullOrWhiteSpace(tag) ? "무태그" : tag;
            }

            if (rawTags is IEnumerable tags)
            {
                string label = string.Empty;
                foreach (object item in tags)
                {
                    string value = item?.ToString();
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }

                    label = string.IsNullOrEmpty(label) ? value : $"{label},{value}";
                }

                return string.IsNullOrWhiteSpace(label) ? "무태그" : label;
            }

            string text = rawTags.ToString();
            return string.IsNullOrWhiteSpace(text) ? "무태그" : text;
        }
    }
}
