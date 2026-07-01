using NUnit.Framework;
using ProjectRaidAuthority.Networking;

namespace ProjectRaidAuthority.Tests.EditMode
{
    public sealed class LootTransactionServiceTests
    {
        [Test]
        public void TryCommitLoot_WhenWorldItemAvailable_CommitsOwnership()
        {
            LootTransactionService service = CreateServiceWithItem("loot_item_1", LootItemState.Available, -1);
            LootCommand command = CreateCommand("req-1", "loot_item_1", ownerId: 11, isDistanceValid: true);

            LootDecision decision = service.TryCommitLoot(command);

            Assert.That(decision.Kind, Is.EqualTo(LootDecisionKind.Committed));
            Assert.That(decision.Succeeded, Is.True);
            Assert.That(decision.OwnerId, Is.EqualTo(11));
            Assert.That(decision.State, Is.EqualTo(LootItemState.Looted));
        }

        [Test]
        public void TryCommitLoot_WhenSameRequestIdRepeated_ReturnsDuplicateIgnoredWithoutNewCommit()
        {
            LootTransactionService service = CreateServiceWithItem("loot_item_1", LootItemState.Available, -1);
            LootCommand command = CreateCommand("req-dup", "loot_item_1", ownerId: 11, isDistanceValid: true);

            LootDecision first = service.TryCommitLoot(command);
            LootDecision second = service.TryCommitLoot(command);

            Assert.That(first.Kind, Is.EqualTo(LootDecisionKind.Committed));
            Assert.That(second.Kind, Is.EqualTo(LootDecisionKind.DuplicateIgnored));
            Assert.That(second.IsDuplicate, Is.True);
            Assert.That(second.OwnerId, Is.EqualTo(11));
            Assert.That(second.State, Is.EqualTo(LootItemState.Looted));
        }

        [Test]
        public void TryCommitLoot_WhenAlreadyLootedWithNewRequest_RejectsAlreadyLooted()
        {
            LootTransactionService service = CreateServiceWithItem("loot_item_1", LootItemState.Looted, 7);
            LootCommand command = CreateCommand("req-2", "loot_item_1", ownerId: 11, isDistanceValid: true);

            LootDecision decision = service.TryCommitLoot(command);

            Assert.That(decision.Kind, Is.EqualTo(LootDecisionKind.RejectedAlreadyLooted));
            Assert.That(decision.Succeeded, Is.False);
            Assert.That(decision.OwnerId, Is.EqualTo(7));
            Assert.That(decision.Reason, Is.EqualTo("Loot item already looted"));
        }

        [Test]
        public void TryCommitLoot_WhenItemNotFound_RejectsNotFound()
        {
            LootTransactionService service = new();
            LootCommand command = CreateCommand("req-3", "missing_item", ownerId: 11, isDistanceValid: true);

            LootDecision decision = service.TryCommitLoot(command);

            Assert.That(decision.Kind, Is.EqualTo(LootDecisionKind.RejectedNotFound));
            Assert.That(decision.Reason, Is.EqualTo("Loot item not found"));
        }

        [Test]
        public void TryCommitLoot_WhenAdapterCannotFindTrackedProjection_RejectsNotFound()
        {
            LootTransactionService service = CreateServiceWithItem("loot_item_1", LootItemState.Available, -1);
            LootCommand command = CreateCommand("req-missing-projection", "loot_item_1", ownerId: 11, isDistanceValid: true, isItemPresent: false);

            LootDecision decision = service.TryCommitLoot(command);

            Assert.That(decision.Kind, Is.EqualTo(LootDecisionKind.RejectedNotFound));
            Assert.That(decision.Succeeded, Is.False);
        }

        [Test]
        public void TryCommitLoot_WhenDistanceInvalid_RejectsInvalidDistance()
        {
            LootTransactionService service = CreateServiceWithItem("loot_item_1", LootItemState.Available, -1);
            LootCommand command = CreateCommand("req-4", "loot_item_1", ownerId: 11, isDistanceValid: false);

            LootDecision decision = service.TryCommitLoot(command);

            Assert.That(decision.Kind, Is.EqualTo(LootDecisionKind.RejectedInvalidDistance));
            Assert.That(decision.Reason, Is.EqualTo("Loot item out of range"));
        }

        [Test]
        public void TryCommitLoot_WhenRequestInvalid_RejectsWithoutRecordingDuplicate()
        {
            LootTransactionService service = new();
            LootCommand invalid = CreateCommand(string.Empty, "loot_item_1", ownerId: 11, isDistanceValid: true);

            LootDecision first = service.TryCommitLoot(invalid);
            LootDecision second = service.TryCommitLoot(invalid);

            Assert.That(first.Kind, Is.EqualTo(LootDecisionKind.RejectedInvalidRequest));
            Assert.That(second.Kind, Is.EqualTo(LootDecisionKind.RejectedInvalidRequest));
        }

        private static LootTransactionService CreateServiceWithItem(string itemId, LootItemState state, int ownerId)
        {
            LootTransactionService service = new();
            service.TrackItem(new ItemInstanceRecord(new LootItemIdentity(itemId), state, ownerId));
            return service;
        }

        private static LootCommand CreateCommand(string requestId, string itemId, int ownerId, bool isDistanceValid, bool isItemPresent = true)
        {
            LootRequest request = new(new LootRequestId(requestId), new LootItemIdentity(itemId));
            return new LootCommand(request, ownerId, isDistanceValid, isItemPresent);
        }
    }
}
