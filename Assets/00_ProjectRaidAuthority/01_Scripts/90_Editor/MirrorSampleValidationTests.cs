#if UNITY_EDITOR
using System.Linq;
using Mirror;
using NUnit.Framework;
using ProjectRaidAuthority.EditorTools;
using ProjectRaidAuthority.Networking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectRaidAuthority.Tests.EditMode
{
    public sealed class MirrorSampleValidationTests
    {
        [Test]
        public void RuntimeScriptsUseExpectedMirrorBaseClasses()
        {
            Assert.IsTrue(typeof(NetworkRoomManager).IsAssignableFrom(typeof(MirrorSampleRoomManager)));
            Assert.IsTrue(typeof(NetworkRoomPlayer).IsAssignableFrom(typeof(MirrorSampleRoomPlayer)));
            Assert.IsTrue(typeof(NetworkBehaviour).IsAssignableFrom(typeof(MirrorSampleGamePlayer)));
        }

        [Test]
        public void BuilderGeneratesExpectedArtifactsAndBuildScenes()
        {
            MirrorSampleSceneBuilder.GenerateSample();

            Assert.NotNull(AssetDatabase.LoadAssetAtPath<SceneAsset>(MirrorSampleSceneBuilder.OfflineScenePath));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<SceneAsset>(MirrorSampleSceneBuilder.RoomScenePath));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<SceneAsset>(MirrorSampleSceneBuilder.GameplayScenePath));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(MirrorSampleSceneBuilder.RoomPlayerPrefabPath));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(MirrorSampleSceneBuilder.GamePlayerPrefabPath));

            string[] buildScenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
            CollectionAssert.Contains(buildScenePaths, MirrorSampleSceneBuilder.OfflineScenePath);
            CollectionAssert.Contains(buildScenePaths, MirrorSampleSceneBuilder.RoomScenePath);
            CollectionAssert.Contains(buildScenePaths, MirrorSampleSceneBuilder.GameplayScenePath);

            EditorSceneManager.OpenScene(MirrorSampleSceneBuilder.OfflineScenePath);
            Assert.IsTrue(Object.FindObjectsByType<Transport>(FindObjectsSortMode.None)
                .Any(transport => transport.GetType().FullName == "kcp2k.KcpTransport"));
            Assert.NotNull(Object.FindFirstObjectByType<MirrorSampleRoomManager>());
            Assert.NotNull(Object.FindFirstObjectByType<MirrorSampleOfflineMenu>());
            Assert.IsTrue(Object.FindObjectsByType<Text>(FindObjectsSortMode.None)
                .Any(text => text.text.Contains("Mirror Room Sample") || text.text.Contains("Offline")));

            EditorSceneManager.OpenScene(MirrorSampleSceneBuilder.RoomScenePath);
            Assert.NotNull(Object.FindFirstObjectByType<MirrorSampleRoomInstructions>());
            Assert.IsTrue(Object.FindObjectsByType<Text>(FindObjectsSortMode.None)
                .Any(text => text.text.Contains("NetworkRoomPlayer") || text.text.Contains("Ready")));

            EditorSceneManager.OpenScene(MirrorSampleSceneBuilder.GameplayScenePath);
            Assert.NotNull(Object.FindFirstObjectByType<MirrorSampleRoomInstructions>());

            GameObject gamePlayer = AssetDatabase.LoadAssetAtPath<GameObject>(MirrorSampleSceneBuilder.GamePlayerPrefabPath);
            Assert.NotNull(gamePlayer.GetComponent<NetworkIdentity>());
            Assert.NotNull(gamePlayer.GetComponent<NetworkTransformUnreliable>());
            Assert.NotNull(gamePlayer.GetComponent<MirrorSampleGamePlayer>());
            Assert.IsFalse(gamePlayer.GetComponents<MonoBehaviour>()
                .Any(component => component != null && component.GetType().Name.Contains("PlayerController")));

            GameObject roomPlayer = AssetDatabase.LoadAssetAtPath<GameObject>(MirrorSampleSceneBuilder.RoomPlayerPrefabPath);
            Assert.NotNull(roomPlayer.GetComponent<NetworkIdentity>());
            Assert.NotNull(roomPlayer.GetComponent<MirrorSampleRoomPlayer>());
        }
    }
}
#endif
