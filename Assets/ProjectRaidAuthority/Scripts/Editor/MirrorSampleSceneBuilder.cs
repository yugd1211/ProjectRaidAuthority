using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
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
    /// Generates a runnable Mirror room sample matching the referenced blog flow.
    /// Use menu: Project Raid Authority/Mirror Sample/Generate Blog Sample.
    /// </summary>
    public static class MirrorSampleSceneBuilder
    {
        public const string SceneFolder = "Assets/ProjectRaidAuthority/Scenes/MirrorSample";
        public const string PrefabFolder = "Assets/ProjectRaidAuthority/Prefabs/Network/MirrorSample";
        public const string OfflineScenePath = SceneFolder + "/MirrorOffline.unity";
        public const string RoomScenePath = SceneFolder + "/MirrorGameRoom.unity";
        public const string GameplayScenePath = SceneFolder + "/MirrorGamePlay.unity";
        public const string RoomPlayerPrefabPath = PrefabFolder + "/MirrorRoomPlayer.prefab";
        public const string GamePlayerPrefabPath = PrefabFolder + "/MirrorGamePlayer.prefab";


        [InitializeOnLoadMethod]
        private static void GenerateOnceAfterScriptReload()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            if (SessionState.GetBool("ProjectRaidAuthority.MirrorSample.GeneratedThisSession", false))
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

            SessionState.SetBool("ProjectRaidAuthority.MirrorSample.GeneratedThisSession", true);
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                {
                    SessionState.SetBool("ProjectRaidAuthority.MirrorSample.GeneratedThisSession", false);
                    return;
                }

                GenerateSample();
            };
        }

        [MenuItem("Project Raid Authority/Mirror Sample/Generate Blog Sample")]
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
            Debug.Log("[MirrorSample] Generated Mirror sample scenes, prefabs, and build settings.");
        }

        private static void EnsureFolders()
        {
            Directory.CreateDirectory(SceneFolder);
            Directory.CreateDirectory(PrefabFolder);
        }

        private static GameObject CreateRoomPlayerPrefab()
        {
            GameObject root = new GameObject("MirrorRoomPlayer");
            root.AddComponent<NetworkIdentity>();
            MirrorSampleRoomPlayer roomPlayer = root.AddComponent<MirrorSampleRoomPlayer>();
            roomPlayer.showRoomGUI = true;

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, RoomPlayerPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return saved;
        }

        private static GameObject CreateGamePlayerPrefab()
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = "MirrorGamePlayer";
            root.transform.localScale = new Vector3(1f, 1.2f, 1f);
            root.AddComponent<NetworkIdentity>();
            NetworkTransformUnreliable transformSync = root.AddComponent<NetworkTransformUnreliable>();
            transformSync.syncDirection = SyncDirection.ClientToServer;
            root.AddComponent<MirrorSampleGamePlayer>();

            Renderer renderer = root.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = CreateMaterial("MirrorGamePlayer_Mat", new Color(0.1f, 0.65f, 1f));
            }

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, GamePlayerPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
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

        private static Type FindKcpTransportType()
        {
            Type directType = Type.GetType("kcp2k.KcpTransport, Mirror.Transports");
            if (directType != null)
            {
                return directType;
            }

            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type candidate = assembly.GetType("kcp2k.KcpTransport", false);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException("Mirror KCP transport type 'kcp2k.KcpTransport' was not found. Confirm Mirror Transports are imported.");
        }

        private static void CreateOfflineScene(GameObject roomPlayerPrefab, GameObject gamePlayerPrefab)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MirrorOffline";

            CreateCamera(new Vector3(0f, 2.5f, -10f), Quaternion.Euler(12f, 0f, 0f));
            CreateDirectionalLight();

            GameObject managerObject = new GameObject("RoomManager");
            Transport transport = (Transport)managerObject.AddComponent(FindKcpTransportType());
            MirrorSampleRoomManager manager = managerObject.AddComponent<MirrorSampleRoomManager>();
            manager.transport = transport;
            manager.offlineScene = OfflineScenePath;
            manager.onlineScene = RoomScenePath;
            manager.RoomScene = RoomScenePath;
            manager.GameplayScene = GameplayScenePath;
            manager.roomPlayerPrefab = roomPlayerPrefab.GetComponent<MirrorSampleRoomPlayer>();
            manager.playerPrefab = gamePlayerPrefab;
            manager.minPlayers = 1;
            manager.maxConnections = 8;
            manager.showRoomGUI = true;
            managerObject.AddComponent<NetworkManagerHUD>();

            Canvas canvas = CreateCanvas();
            MirrorSampleOfflineMenu menu = CreateMenuBridge(canvas.transform, manager);
            CreateText(canvas.transform, "Title", "Mirror Room Sample", new Vector2(0f, 150f), 34, TextAnchor.MiddleCenter);
            CreateText(canvas.transform, "Subtitle", "Blog flow: Offline -> Game Room -> Game Play", new Vector2(0f, 105f), 18, TextAnchor.MiddleCenter);
            CreateButton(canvas.transform, "HostButton", "Create Room (Host)", new Vector2(0f, 35f), menu.CreateRoom);
            CreateButton(canvas.transform, "JoinButton", "Join Localhost (Client)", new Vector2(0f, -25f), menu.JoinLocalhost);
            CreateText(canvas.transform, "Hint", "Host starts the room scene. In the room, press Ready to enter gameplay.", new Vector2(0f, -105f), 16, TextAnchor.MiddleCenter);

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, OfflineScenePath);
        }

        private static void CreateRoomScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MirrorGameRoom";

            CreateCamera(new Vector3(0f, 2f, -10f), Quaternion.identity);
            CreateDirectionalLight();
            Canvas canvas = CreateCanvas();
            CreateText(canvas.transform, "RoomTitle", "Mirror Game Room", new Vector2(0f, 95f), 32, TextAnchor.MiddleCenter);
            CreateText(canvas.transform, "RoomHint", "Mirror's NetworkRoomPlayer default GUI shows the Ready button during play mode.", new Vector2(0f, 35f), 18, TextAnchor.MiddleCenter);
            CreateText(canvas.transform, "RoomInstruction", "When all players are ready, NetworkRoomManager loads MirrorGamePlay.", new Vector2(0f, -15f), 16, TextAnchor.MiddleCenter);
            CreateInstructionOverlay(
                "Room Runtime Instructions",
                "Mirror Game Room",
                "Mirror's NetworkRoomPlayer default GUI shows Ready during play mode. When every player is ready, NetworkRoomManager loads MirrorGamePlay.");
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, RoomScenePath);
        }

        private static void CreateGameplayScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MirrorGamePlay";

            CreateCamera(new Vector3(0f, 7f, -9f), Quaternion.Euler(38f, 0f, 0f));
            CreateDirectionalLight();
            CreatePlane();
            CreateSpawnPoint("SpawnPoint_A", new Vector3(-2f, 0.1f, 0f));
            CreateSpawnPoint("SpawnPoint_B", new Vector3(2f, 0.1f, 0f));
            CreateSpawnPoint("SpawnPoint_C", new Vector3(0f, 0.1f, 3f));

            Canvas canvas = CreateCanvas();
            CreateText(canvas.transform, "GameplayHint", "Mirror Gameplay: local player moves with WASD/Arrow keys", new Vector2(0f, 185f), 18, TextAnchor.MiddleCenter);
            CreateInstructionOverlay(
                "Gameplay Runtime Instructions",
                "Mirror Game Play",
                "Local player moves with WASD/Arrow keys. Transform sync is handled by Mirror NetworkTransformUnreliable.");
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
                renderer.sharedMaterial = CreateMaterial("MirrorArenaFloor_Mat", new Color(0.2f, 0.28f, 0.22f));
            }
        }

        private static void CreateSpawnPoint(string name, Vector3 position)
        {
            GameObject spawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spawn.name = name;
            spawn.transform.position = position;
            spawn.transform.localScale = new Vector3(0.45f, 0.15f, 0.45f);
            spawn.AddComponent<NetworkStartPosition>();
            Renderer renderer = spawn.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = CreateMaterial("MirrorSpawnPoint_Mat", new Color(0.9f, 0.85f, 0.25f));
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

        private static MirrorSampleOfflineMenu CreateMenuBridge(Transform parent, MirrorSampleRoomManager manager)
        {
            GameObject menuObject = new GameObject("MirrorSampleOfflineMenu");
            menuObject.transform.SetParent(parent, false);
            MirrorSampleOfflineMenu menu = menuObject.AddComponent<MirrorSampleOfflineMenu>();
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

        private static MirrorSampleRoomInstructions CreateInstructionOverlay(string objectName, string title, string body)
        {
            GameObject instructionObject = new GameObject(objectName);
            MirrorSampleRoomInstructions instructions = instructionObject.AddComponent<MirrorSampleRoomInstructions>();
            SerializedObject serialized = new SerializedObject(instructions);
            serialized.FindProperty("title").stringValue = title;
            serialized.FindProperty("body").stringValue = body;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return instructions;
        }

        private static void CreateEventSystem()
        {
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() != null)
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
