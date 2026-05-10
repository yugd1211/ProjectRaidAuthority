using Mirror;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Blog-style Mirror room manager sample.
    ///
    /// The referenced article uses NetworkRoomManager for a three-scene flow:
    /// Offline Scene -> Game Room Scene -> Game Play Scene. This subclass keeps
    /// the sample explicit while leaving Mirror's built-in room/ready behavior intact.
    /// </summary>
    public sealed class MirrorSampleRoomManager : NetworkRoomManager
    {
        [Header("Sample Notes")]
        [TextArea(2, 5)]
        [SerializeField]
        private string sampleDescription =
            "Mirror sample based on the blog flow: Offline scene starts host/client, " +
            "Room scene uses NetworkRoomPlayer ready UI, Gameplay scene spawns GamePlayer.";

        public string SampleDescription => sampleDescription;

        public override void OnRoomServerPlayersReady()
        {
            Debug.Log("[MirrorSample] All room players are ready. Loading gameplay scene.");
            base.OnRoomServerPlayersReady();
        }

        public override void OnRoomClientEnter()
        {
            base.OnRoomClientEnter();
            Debug.Log("[MirrorSample] Entered Mirror room scene. Use the Ready button to continue.");
        }
    }
}
