using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class GamePlayer
    {
        private const float LootServerAcceptRadius = 2.5f;

        private readonly HashSet<string> handledLootRequestIds = new();

        [ServerRpc]
        private void ServerRequestLoot(string requestIdValue, string itemIdValue)
        {
            LootRequest request = new(new LootRequestId(requestIdValue), new LootItemIdentity(itemIdValue));
            if (handledLootRequestIds.Contains(request.RequestId.Value))
            {
                Debug.Log($"[FishNet Authority Smoke] Duplicate LootRequest ignored: owner={OwnerId}, requestId={request.RequestId}, itemId={request.ItemId}");
                return;
            }

            if (!request.IsValid)
            {
                RejectLootRequest(request, "Invalid requestId or itemId");
                return;
            }

            handledLootRequestIds.Add(request.RequestId.Value);

            if (!TryFindLootItem(request.ItemId, out LootItem item))
            {
                RejectLootRequest(request, "Loot item not found");
                return;
            }

            if (!IsLootItemInRange(item))
            {
                RejectLootRequest(request, "Loot item out of range");
                return;
            }

            if (!item.IsAvailable)
            {
                RejectLootRequest(request, "Loot item already looted");
                return;
            }

            item.ServerMarkLooted(OwnerId);
            LootResult result = new(request.RequestId, request.ItemId, true, OwnerId, item.State, "Committed");
            Debug.Log($"[FishNet Authority Smoke] LootCommitted: owner={OwnerId}, requestId={result.RequestId}, itemId={result.ItemId}, state={result.State}");
        }

        private bool TryFindLootItem(LootItemIdentity itemId, out LootItem foundItem)
        {
            LootItem[] items = FindObjectsByType<LootItem>();
            foreach (LootItem item in items)
            {
                if (!item.ItemId.Equals(itemId))
                {
                    continue;
                }

                foundItem = item;
                return true;
            }

            foundItem = default;
            return false;
        }

        private bool IsLootItemInRange(LootItem item)
        {
            float distanceSq = (item.transform.position - transform.position).sqrMagnitude;
            return distanceSq <= LootServerAcceptRadius * LootServerAcceptRadius;
        }

        private void RejectLootRequest(LootRequest request, string reason)
        {
            LootResult result = new(request.RequestId, request.ItemId, false, -1, LootItemState.Available, reason);
            Debug.Log($"[FishNet Authority Smoke] LootRejected: owner={OwnerId}, requestId={result.RequestId}, itemId={result.ItemId}, reason={result.Reason}");
        }
    }
}
