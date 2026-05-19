using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class GamePlayer
    {
        private const float LootClientSearchRadius = 2.25f;

        private int nextLootRequestSequence;

        private void SendOwnedLootInput()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame)
            {
                return;
            }

            string itemId = FindNearestLootItemId(LootClientSearchRadius);
            LootRequestId requestId = NextLootRequestId();
            ServerRequestLoot(requestId.Value, itemId);
        }

        private LootRequestId NextLootRequestId()
        {
            nextLootRequestSequence++;
            return new LootRequestId($"loot-{OwnerId}-{nextLootRequestSequence}-{Guid.NewGuid():N}");
        }

        private string FindNearestLootItemId(float searchRadius)
        {
            LootItem[] items = FindObjectsByType<LootItem>();
            float bestDistanceSq = searchRadius * searchRadius;
            string bestItemId = string.Empty;

            foreach (LootItem item in items)
            {
                float distanceSq = (item.transform.position - transform.position).sqrMagnitude;
                if (distanceSq > bestDistanceSq || !item.IsAvailable)
                {
                    continue;
                }

                bestDistanceSq = distanceSq;
                bestItemId = item.ItemId.Value;
            }

            return bestItemId;
        }
    }
}
