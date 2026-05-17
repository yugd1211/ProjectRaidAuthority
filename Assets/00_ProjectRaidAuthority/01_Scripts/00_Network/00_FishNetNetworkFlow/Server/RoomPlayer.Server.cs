using FishNet.Object;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class RoomPlayer
    {
        [ServerRpc]
        private void ServerSetReadyToBegin(bool ready)
        {
            readyToBegin.Value = ready;
        }

        private void OnReadyChanged(bool previous, bool next, bool asServer)
        {
            Debug.Log($"[NetworkFlow] RoomPlayer 준비 상태 변경: {previous} -> {next}");

            if (asServer)
            {
                FindAnyObjectByType<FishNetNetworkFlowController>()?.NotifyRoomPlayerReadyChanged();
            }
        }
    }
}
