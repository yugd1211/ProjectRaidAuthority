using FishNet.Object;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class GamePlayer
    {
        private const float LootServerAcceptRadius = 2.5f;

        private readonly LootTransactionService lootTransactions = new();

        [ServerRpc]
        private void ServerRequestLoot(string requestIdValue, string itemIdValue)
        {
            LootRequest request = new(new LootRequestId(requestIdValue), new LootItemIdentity(itemIdValue));
            bool itemFound = TryFindLootItem(request.ItemId, out LootItem item);
            bool isDistanceValid = !itemFound || IsLootItemInRange(item);

            if (itemFound)
            {
                lootTransactions.TrackItem(new ItemInstanceRecord(item.ItemId, item.State, item.LootOwnerId));
            }

            LootCommand command = new(request, OwnerId, isDistanceValid, itemFound);
            LootDecision decision = lootTransactions.TryCommitLoot(command);
            ApplyLootDecision(decision, itemFound ? item : null);
        }

        private void ApplyLootDecision(LootDecision decision, LootItem item)
        {
            if (decision.Kind == LootDecisionKind.DuplicateIgnored)
            {
                Debug.Log($"[FishNet Authority Smoke] Duplicate LootRequest ignored: owner={OwnerId}, requestId={decision.RequestId}, itemId={decision.ItemId}");
                return;
            }

            if (!decision.Succeeded)
            {
                RejectLootRequest(decision);
                return;
            }

            if (item == null)
            {
                RejectLootRequest(new LootDecision(
                    LootDecisionKind.RejectedNotFound,
                    decision.RequestId,
                    decision.ItemId,
                    -1,
                    LootItemState.Available,
                    "Loot item not found"));
                return;
            }

            item.ServerMarkLooted(decision.OwnerId);
            LootResult result = new(decision.RequestId, decision.ItemId, true, decision.OwnerId, item.State, decision.Reason);
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

        private void RejectLootRequest(LootDecision decision)
        {
            LootResult result = new(decision.RequestId, decision.ItemId, false, -1, decision.State, decision.Reason);
            Debug.Log($"[FishNet Authority Smoke] LootRejected: owner={OwnerId}, requestId={result.RequestId}, itemId={result.ItemId}, reason={result.Reason}");
        }
    }
}
