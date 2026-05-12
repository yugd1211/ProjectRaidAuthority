#if UNITY_EDITOR
using System.Linq;
using FishNet.Component.Scenes;
using FishNet.Component.Transforming;
using FishNet.Managing;
using FishNet.Object;
using NUnit.Framework;
using ProjectRaidAuthority.EditorTools;
using ProjectRaidAuthority.Networking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectRaidAuthority.Tests.EditMode
{
    public sealed class FishNetSampleValidationTests
    {
        [Test]
        public void RuntimeScriptsUseExpectedFishNetBaseClasses()
        {
            Assert.IsTrue(typeof(NetworkBehaviour).IsAssignableFrom(typeof(FishNetSampleGamePlayer)));
            Assert.IsTrue(typeof(NetworkBehaviour).IsAssignableFrom(typeof(FishNetSampleRoomPlayer)));
            Assert.IsTrue(typeof(MonoBehaviour).IsAssignableFrom(typeof(FishNetSampleRoomManager)));
        }

        [Test]
        public void BuilderGeneratesExpectedArtifactsAndBuildScenes()
        {
            FishNetSampleSceneBuilder.GenerateSample();

            Assert.NotNull(AssetDatabase.LoadAssetAtPath<SceneAsset>(FishNetSampleSceneBuilder.OfflineScenePath));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<SceneAsset>(FishNetSampleSceneBuilder.RoomScenePath));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<SceneAsset>(FishNetSampleSceneBuilder.GameplayScenePath));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<GameObject>(FishNetSampleSceneBuilder.RoomPlayerPrefabPath));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<GameObject>(FishNetSampleSceneBuilder.GamePlayerPrefabPath));

            string[] buildScenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
            CollectionAssert.Contains(buildScenePaths, FishNetSampleSceneBuilder.OfflineScenePath);
            CollectionAssert.Contains(buildScenePaths, FishNetSampleSceneBuilder.RoomScenePath);
            CollectionAssert.Contains(buildScenePaths, FishNetSampleSceneBuilder.GameplayScenePath);

            EditorSceneManager.OpenScene(FishNetSampleSceneBuilder.OfflineScenePath);
            Assert.NotNull(Object.FindAnyObjectByType<NetworkManager>());
            Assert.NotNull(Object.FindAnyObjectByType<DefaultScene>());
            Assert.NotNull(Object.FindAnyObjectByType<FishNetSampleRoomManager>());
            Assert.NotNull(Object.FindAnyObjectByType<FishNetSampleOfflineMenu>());
            Assert.IsTrue(Object.FindObjectsByType<Text>(FindObjectsInactive.Exclude)
                .Any(text => text.text.Contains("FishNet Room Sample") || text.text.Contains("Offline")));

            EditorSceneManager.OpenScene(FishNetSampleSceneBuilder.RoomScenePath);
            Assert.NotNull(Object.FindAnyObjectByType<FishNetSampleRoomInstructions>());
            Assert.IsTrue(Object.FindObjectsByType<Text>(FindObjectsInactive.Exclude)
                .Any(text => text.text.Contains("FishNet") || text.text.Contains("Ready")));

            EditorSceneManager.OpenScene(FishNetSampleSceneBuilder.GameplayScenePath);
            Assert.NotNull(Object.FindAnyObjectByType<FishNetSampleRoomInstructions>());

            GameObject gamePlayer = AssetDatabase.LoadAssetAtPath<GameObject>(FishNetSampleSceneBuilder.GamePlayerPrefabPath);
            Assert.NotNull(gamePlayer.GetComponent<NetworkObject>());
            Assert.NotNull(gamePlayer.GetComponent<NetworkTransform>());
            Assert.NotNull(gamePlayer.GetComponent<FishNetSampleGamePlayer>());
            Assert.IsFalse(gamePlayer.GetComponents<MonoBehaviour>()
                .Any(component => component != null && component.GetType().Name.Contains("PlayerController")));

            GameObject roomPlayer = AssetDatabase.LoadAssetAtPath<GameObject>(FishNetSampleSceneBuilder.RoomPlayerPrefabPath);
            Assert.NotNull(roomPlayer.GetComponent<NetworkObject>());
            Assert.NotNull(roomPlayer.GetComponent<FishNetSampleRoomPlayer>());
        }
    }
}
#endif
