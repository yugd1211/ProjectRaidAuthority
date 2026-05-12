using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 룸과 게임플레이 샘플 씬에서 보여 주는 가벼운 안내문입니다.
    /// </summary>
    public sealed class FishNetSampleRoomInstructions : MonoBehaviour
    {
        [SerializeField] private string title = "FishNet 게임룸";
        [TextArea(2, 5)]
        [SerializeField]
        private string body =
            "FishNet 샘플 룸 플레이어의 Ready 버튼을 누르세요. " +
            "모든 플레이어가 준비되면 샘플 룸 매니저가 게임플레이 씬을 로드합니다.";

        private void OnGUI()
        {
            const int width = 660;
            GUI.Box(new Rect(20f, 20f, width, 112f), GUIContent.none);
            GUI.Label(new Rect(36f, 36f, width - 32f, 28f), title);
            GUI.Label(new Rect(36f, 68f, width - 32f, 52f), body);
        }
    }
}
