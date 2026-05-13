using System.Collections.Generic;
using System.IO;
using FishNet.Component.Scenes;
using FishNet.Component.Transforming;
using FishNet.Managing;
using FishNet.Object;
using ProjectRaidAuthority.Networking;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectRaidAuthority.EditorTools
{
    /// <summary>
    /// Generates a runnable FishNet room sample.
    /// Use menu: Project Raid Authority/FishNet Sample/Generate Room Sample.
    /// </summary>
    public static class FishNetSampleSceneBuilder
    {
        public const string SceneFolder = "Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetSample";
        public const string PrefabFolder = "Assets/00_ProjectRaidAuthority/02_Prefabs/00_Network/00_FishNetSample";
        public const string OfflineScenePath = SceneFolder + "/FishNetOffline.unity";
        public const string RoomScenePath = SceneFolder + "/FishNetGameRoom.unity";
        public const string GameplayScenePath = SceneFolder + "/FishNetGamePlay.unity";
        public const string RoomPlayerPrefabPath = PrefabFolder + "/FishNetRoomPlayer.prefab";
        public const string GamePlayerPrefabPath = PrefabFolder + "/FishNetGamePlayer.prefab";

        [InitializeOnLoadMethod]
        private static void GenerateOnceAfterScriptReload()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            if (SessionState.GetBool("ProjectRaidAuthority.FishNetSample.GeneratedThisSession", false))
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(OfflineScenePath) != null &&
                AssetDatabase.LoadAssetAtPath<SceneAsset>(RoomScenePath) != null &&
                AssetDatabase.LoadAssetAtPath<SceneAsset>(GameplayScenePath) != null &&
                AssetDatabase.LoadAssetAtPath<GameObject>(RoomPlayerPrefabPath) != null &&
                AssetDatabase.LoadAssetAtPath<GameObject>(GamePlayerPrefabPath) != null)
            {
                return;
            }

            SessionState.SetBool("ProjectRaidAuthority.FishNetSample.GeneratedThisSession", true);
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                {
                    SessionState.SetBool("ProjectRaidAuthority.FishNetSample.GeneratedThisSession", false);
                    return;
                }

                GenerateSample();
            };
        }

        [MenuItem("Project Raid Authority/FishNet Sample/Generate Room Sample")]
        public static void GenerateSample()
        {
            EnsureFolders();
            GameObject roomPlayerPrefab = CreateRoomPlayerPrefab();
            GameObject gamePlayerPrefab = CreateGamePlayerPrefab();

            CreateOfflineScene(roomPlayerPrefab, gamePlayerPrefab);
            CreateRoomScene();
            CreateGameplayScene();
            RegisterBuildScenes();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[FishNetSample] Generated FishNet sample scenes, prefabs, and build settings.");
        }

        private static void EnsureFolders()
        {
            Directory.CreateDirectory(SceneFolder);
            Directory.CreateDirectory(PrefabFolder);
        }

        private static GameObject CreateRoomPlayerPrefab()
        {
            GameObject root = new GameObject("FishNetRoomPlayer");
            root.AddComponent<NetworkObject>();
            root.AddComponent<FishNetSampleRoomPlayer>();

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, RoomPlayerPrefabPath);
            Object.DestroyImmediate(root);
            return saved;
        }

        private static GameObject CreateGamePlayerPrefab()
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = "FishNetGamePlayer";
            root.transform.localScale = new Vector3(1f, 1.2f, 1f);
            root.AddComponent<NetworkObject>();
            root.AddComponent<NetworkTransform>();
            root.AddComponent<FishNetSampleGamePlayer>();

            Renderer renderer = root.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = CreateMaterial("FishNetGamePlayer_Mat", new Color(0.1f, 0.65f, 1f));
            }

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, GamePlayerPrefabPath);
            Object.DestroyImmediate(root);
            return saved;
        }

        private static Material CreateMaterial(string name, Color color)
        {
            string path = PrefabFolder + "/" + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void CreateOfflineScene(GameObject roomPlayerPrefab, GameObject gamePlayerPrefab)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "FishNetOffline";

            CreateCamera(new Vector3(0f, 2.5f, -10f), Quaternion.Euler(12f, 0f, 0f));
            CreateDirectionalLight();

            GameObject managerObject = new GameObject("FishNetRoomManager");
            NetworkManager networkManager = managerObject.AddComponent<NetworkManager>();
            DefaultScene defaultScene = managerObject.AddComponent<DefaultScene>();
            defaultScene.SetOfflineScene(OfflineScenePath);
            defaultScene.SetOnlineScene(RoomScenePath);

            FishNetSampleRoomManager manager = managerObject.AddComponent<FishNetSampleRoomManager>();
            manager.Configure(
                networkManager,
                roomPlayerPrefab.GetComponent<NetworkObject>(),
                gamePlayerPrefab.GetComponent<NetworkObject>(),
                OfflineScenePath,
                RoomScenePath,
                GameplayScenePath);

            Canvas canvas = CreateCanvas();
            FishNetSampleOfflineMenu menu = CreateMenuBridge(canvas.transform, manager);
            CreateText(canvas.transform, "Title", "FishNet 룸 샘플", new Vector2(0f, 150f), 34, TextAnchor.MiddleCenter);
            CreateText(canvas.transform, "Subtitle", "흐름: 오프라인 -> 게임룸 -> 게임플레이", new Vector2(0f, 105f), 18, TextAnchor.MiddleCenter);
            CreateButton(canvas.transform, "HostButton", "방 만들기 (호스트)", new Vector2(0f, 35f), menu.CreateRoom);
            CreateButton(canvas.transform, "JoinButton", "로컬호스트 참가 (클라이언트)", new Vector2(0f, -25f), menu.JoinLocalhost);
            CreateText(canvas.transform, "Hint", "호스트를 시작하면 FishNet 룸 씬으로 이동합니다. 룸에서 준비를 누르면 게임플레이로 진입합니다.", new Vector2(0f, -105f), 16, TextAnchor.MiddleCenter);

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, OfflineScenePath);
        }

        private static void CreateRoomScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "FishNetGameRoom";

            CreateCamera(new Vector3(0f, 2f, -10f), Quaternion.identity);
            CreateDirectionalLight();
            Canvas canvas = CreateCanvas();
            CreateText(canvas.transform, "RoomTitle", "FishNet 게임룸", new Vector2(0f, 95f), 32, TextAnchor.MiddleCenter);
            CreateText(canvas.transform, "RoomHint", "플레이 모드에서 FishNet 샘플 룸 플레이어 조작 화면에서 준비 상태를 전환하세요.", new Vector2(0f, 35f), 18, TextAnchor.MiddleCenter);
            CreateText(canvas.transform, "RoomInstruction", "생성된 모든 룸 플레이어가 준비되면 샘플 매니저가 게임플레이 씬을 불러옵니다.", new Vector2(0f, -15f), 16, TextAnchor.MiddleCenter);
            CreateInstructionOverlay(
                "Room Runtime Instructions",
                "FishNet 게임룸",
                "플레이 모드에서 FishNet 샘플 룸 플레이어 조작 화면에서 준비 상태를 전환하세요. 모든 플레이어가 준비되면 게임플레이 씬을 불러옵니다.");
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, RoomScenePath);
        }

        private static void CreateGameplayScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "FishNetGamePlay";

            CreateCamera(new Vector3(0f, 7f, -9f), Quaternion.Euler(38f, 0f, 0f));
            CreateDirectionalLight();
            CreatePlane();
            CreateSpawnPoint("SpawnPoint_A", new Vector3(-2f, 0.1f, 0f));
            CreateSpawnPoint("SpawnPoint_B", new Vector3(2f, 0.1f, 0f));
            CreateSpawnPoint("SpawnPoint_C", new Vector3(0f, 0.1f, 3f));

            Canvas canvas = CreateCanvas();
            CreateText(canvas.transform, "GameplayHint", "FishNet 게임플레이: 로컬 플레이어는 WASD/화살표 키로 이동합니다", new Vector2(0f, 185f), 18, TextAnchor.MiddleCenter);
            CreateInstructionOverlay(
                "Gameplay Runtime Instructions",
                "FishNet 게임플레이",
                "로컬 플레이어는 WASD/화살표 키로 이동합니다. 위치 동기화는 FishNet 위치 동기화 컴포넌트가 처리합니다.");
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, GameplayScenePath);
        }

        private static Camera CreateCamera(Vector3 position, Quaternion rotation)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetPositionAndRotation(position, rotation);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            cameraObject.AddComponent<AudioListener>();
            return camera;
        }

        private static void CreateDirectionalLight()
        {
            GameObject lightObject = new GameObject("Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.8f;
        }

        private static void CreatePlane()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Sample Arena Floor";
            plane.transform.localScale = new Vector3(1.5f, 1f, 1.5f);
            Renderer renderer = plane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = CreateMaterial("FishNetArenaFloor_Mat", new Color(0.2f, 0.28f, 0.22f));
            }
        }

        private static void CreateSpawnPoint(string name, Vector3 position)
        {
            GameObject spawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spawn.name = name;
            spawn.transform.position = position;
            spawn.transform.localScale = new Vector3(0.45f, 0.15f, 0.45f);
            Renderer renderer = spawn.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = CreateMaterial("FishNetSpawnPoint_Mat", new Color(0.9f, 0.85f, 0.25f));
            }
        }

        private static Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static FishNetSampleOfflineMenu CreateMenuBridge(Transform parent, FishNetSampleRoomManager manager)
        {
            GameObject menuObject = new GameObject("FishNetSampleOfflineMenu");
            menuObject.transform.SetParent(parent, false);
            FishNetSampleOfflineMenu menu = menuObject.AddComponent<FishNetSampleOfflineMenu>();
            menu.Configure(manager);
            return menu;
        }

        private static Text CreateText(Transform parent, string name, string text, Vector2 anchoredPosition, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            Text label = textObject.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = fontSize;
            label.alignment = alignment;
            label.color = Color.white;

            RectTransform rect = label.rectTransform;
            rect.sizeDelta = new Vector2(720f, 52f);
            rect.anchoredPosition = anchoredPosition;
            return label;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction action)
        {
            GameObject buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.15f, 0.22f, 0.32f, 0.92f);
            Button button = buttonObject.AddComponent<Button>();
            UnityEventTools.AddPersistentListener(button.onClick, action);

            RectTransform rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(260f, 44f);
            rect.anchoredPosition = anchoredPosition;

            Text text = CreateText(buttonObject.transform, "Text", label, Vector2.zero, 18, TextAnchor.MiddleCenter);
            text.color = Color.white;
            text.rectTransform.sizeDelta = rect.sizeDelta;
            return button;
        }

        private static FishNetSampleRoomInstructions CreateInstructionOverlay(string objectName, string title, string body)
        {
            GameObject instructionObject = new GameObject(objectName);
            FishNetSampleRoomInstructions instructions = instructionObject.AddComponent<FishNetSampleRoomInstructions>();
            SerializedObject serialized = new SerializedObject(instructions);
            serialized.FindProperty("title").stringValue = title;
            serialized.FindProperty("body").stringValue = body;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return instructions;
        }

        private static void CreateEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        private static void RegisterBuildScenes()
        {
            string[] generatedScenes = { OfflineScenePath, RoomScenePath, GameplayScenePath };
            HashSet<string> seen = new HashSet<string>(generatedScenes);
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

            foreach (string scene in generatedScenes)
            {
                scenes.Add(new EditorBuildSettingsScene(scene, true));
            }

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene == null || string.IsNullOrWhiteSpace(scene.path) || seen.Contains(scene.path))
                {
                    continue;
                }

                scenes.Add(scene);
                seen.Add(scene.path);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
