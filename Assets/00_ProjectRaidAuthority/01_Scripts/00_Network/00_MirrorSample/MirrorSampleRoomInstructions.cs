using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Lightweight in-scene instructions for the room/gameplay sample scenes.
    /// The actual lobby Ready button is provided by Mirror's NetworkRoomPlayer GUI.
    /// </summary>
    public sealed class MirrorSampleRoomInstructions : MonoBehaviour
    {
        [SerializeField] private string title = "Mirror Game Room";
        [TextArea(2, 5)]
        [SerializeField]
        private string body =
            "Mirror's NetworkRoomPlayer default GUI shows Ready during play mode. " +
            "When every player is ready, NetworkRoomManager loads MirrorGamePlay.";

        private void OnGUI()
        {
            const int width = 620;
            GUI.Box(new Rect(20f, 20f, width, 112f), GUIContent.none);
            GUI.Label(new Rect(36f, 36f, width - 32f, 28f), title);
            GUI.Label(new Rect(36f, 68f, width - 32f, 52f), body);
        }
    }
}
