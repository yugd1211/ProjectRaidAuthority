using Mirror;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Lobby player used by NetworkRoomManager. Mirror's base implementation
    /// already provides the simple Ready GUI used by the blog sample.
    /// </summary>
    public sealed class MirrorSampleRoomPlayer : NetworkRoomPlayer
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log($"[MirrorSample] Room player joined. index={index}, ready={readyToBegin}");
        }
    }
}
