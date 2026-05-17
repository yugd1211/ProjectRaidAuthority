using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// FishNet Network Flow 씬 전환 중 여러 씬의 카메라가 잠시 공존해도 활성 AudioListener를 하나로 정규화합니다.
    /// </summary>
    public sealed class NetworkFlowAudioListenerGuard : MonoBehaviour
    {
        private const string GuardObjectName = "NetworkFlowAudioListenerGuard";
        private static readonly HashSet<string> NetworkFlowSceneNames = new()
        {
            "OfflineBootstrap",
            "MatchRoom",
            "Gameplay"
        };

        private static string preferredSceneName;
        private static NetworkFlowAudioListenerGuard instance;
        private AudioListener guardListener;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntimeState()
        {
            instance = null;
            preferredSceneName = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RemoveStaleGuardsBeforeSceneLoad()
        {
            DestroyExistingGuards();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstallAfterSceneLoad()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (IsNetworkFlowScene(activeScene))
            {
                preferredSceneName = activeScene.name;
            }

            EnsureGuardExists();
        }

        private static void EnsureGuardExists()
        {
            if (!Application.isPlaying || !IsAnyNetworkFlowSceneLoaded())
            {
                return;
            }

            NetworkFlowAudioListenerGuard existingGuard = GetExistingGuard();
            if (existingGuard != null)
            {
                NormalizeActiveListeners();
                return;
            }

            GameObject guardObject = new GameObject(GuardObjectName);
            DontDestroyOnLoad(guardObject);
            guardObject.AddComponent<NetworkFlowAudioListenerGuard>();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                DisableListenersOn(gameObject);
                DestroyGuardObject(gameObject);
                return;
            }

            instance = this;
            guardListener = GetComponent<AudioListener>();
            if (guardListener == null)
            {
                guardListener = gameObject.AddComponent<AudioListener>();
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            NormalizeActiveListeners();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            if (instance == this)
            {
                instance = null;
            }
        }

        private void LateUpdate()
        {
            if (!IsAnyNetworkFlowSceneLoaded())
            {
                Destroy(gameObject);
                return;
            }

            FollowPreferredCamera();
            NormalizeActiveListeners();
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (IsNetworkFlowScene(scene))
            {
                preferredSceneName = scene.name;
            }

            EnsureGuardExists();
        }

        public static int NormalizeActiveListeners()
        {
            AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Exclude);
            NetworkFlowAudioListenerGuard guard = GetExistingGuard();
            if (guard != null)
            {
                guard.FollowPreferredCamera();
                return NormalizeListeners(listeners, guard.guardListener);
            }

            return NormalizeListeners(listeners, GetPreferredSceneListener(listeners, preferredSceneName));
        }

        private static NetworkFlowAudioListenerGuard GetExistingGuard()
        {
            if (instance != null)
            {
                return instance;
            }

            NetworkFlowAudioListenerGuard[] guards = Resources.FindObjectsOfTypeAll<NetworkFlowAudioListenerGuard>();
            for (int i = 0; i < guards.Length; i++)
            {
                NetworkFlowAudioListenerGuard guard = guards[i];
                if (guard != null && guard.gameObject.scene.isLoaded)
                {
                    instance = guard;
                    return guard;
                }
            }

            return null;
        }

        private static void DestroyExistingGuards()
        {
            NetworkFlowAudioListenerGuard[] guards = Resources.FindObjectsOfTypeAll<NetworkFlowAudioListenerGuard>();
            for (int i = 0; i < guards.Length; i++)
            {
                NetworkFlowAudioListenerGuard guard = guards[i];
                if (guard == null)
                {
                    continue;
                }

                DisableListenersOn(guard.gameObject);
                DestroyGuardObject(guard.gameObject);
            }

            instance = null;
        }

        private static void DestroyGuardObject(GameObject guardObject)
        {
            if (guardObject == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
#if UNITY_EDITOR
                DestroyImmediate(guardObject);
#else
                Destroy(guardObject);
#endif
            }
            else
            {
                DestroyImmediate(guardObject);
            }
        }

        private static void DisableListenersOn(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            AudioListener[] listeners = target.GetComponents<AudioListener>();
            for (int i = 0; i < listeners.Length; i++)
            {
                if (listeners[i] != null)
                {
                    listeners[i].enabled = false;
                }
            }
        }

        public static int NormalizeListeners(IReadOnlyList<AudioListener> listeners, AudioListener preferredListener)
        {
            AudioListener listenerToKeep = SelectListenerToKeep(listeners, preferredListener);
            if (listenerToKeep == null)
            {
                return 0;
            }

            int enabledCount = 0;
            for (int i = 0; i < listeners.Count; i++)
            {
                AudioListener listener = listeners[i];
                if (listener == null)
                {
                    continue;
                }

                bool shouldEnable = listener == listenerToKeep;
                if (listener.enabled != shouldEnable)
                {
                    listener.enabled = shouldEnable;
                }

                if (shouldEnable)
                {
                    enabledCount++;
                }
            }

            return enabledCount;
        }

        private static AudioListener SelectListenerToKeep(IReadOnlyList<AudioListener> listeners, AudioListener preferredListener)
        {
            if (IsUsableListener(preferredListener))
            {
                return preferredListener;
            }

            for (int i = 0; i < listeners.Count; i++)
            {
                AudioListener listener = listeners[i];
                if (IsUsableListener(listener) && listener.enabled)
                {
                    return listener;
                }
            }

            for (int i = 0; i < listeners.Count; i++)
            {
                AudioListener listener = listeners[i];
                if (IsUsableListener(listener))
                {
                    return listener;
                }
            }

            return null;
        }

        private void FollowPreferredCamera()
        {
            Transform target = GetPreferredCameraTransform();
            if (target == null)
            {
                return;
            }

            transform.SetPositionAndRotation(target.position, target.rotation);
        }

        private static Transform GetPreferredCameraTransform()
        {
            if (!string.IsNullOrWhiteSpace(preferredSceneName))
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (!IsNetworkFlowScene(scene) || scene.name != preferredSceneName)
                    {
                        continue;
                    }

                    GameObject[] roots = scene.GetRootGameObjects();
                    for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
                    {
                        Camera camera = roots[rootIndex].GetComponentInChildren<Camera>(false);
                        if (camera != null)
                        {
                            return camera.transform;
                        }
                    }
                }
            }

            Camera mainCamera = Camera.main;
            return mainCamera != null ? mainCamera.transform : null;
        }

        private static AudioListener GetPreferredSceneListener(IReadOnlyList<AudioListener> listeners, string sceneName)
        {
            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                for (int i = 0; i < listeners.Count; i++)
                {
                    AudioListener listener = listeners[i];
                    if (IsUsableListener(listener) && listener.gameObject.scene.name == sceneName)
                    {
                        return listener;
                    }
                }
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null && mainCamera.TryGetComponent(out AudioListener mainListener))
            {
                return mainListener;
            }

            return null;
        }

        private static bool IsUsableListener(AudioListener listener)
        {
            return listener != null && listener.gameObject.activeInHierarchy;
        }

        private static bool IsAnyNetworkFlowSceneLoaded()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (IsNetworkFlowScene(SceneManager.GetSceneAt(i)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsNetworkFlowScene(Scene scene)
        {
            return scene.isLoaded && NetworkFlowSceneNames.Contains(scene.name);
        }
    }
}
