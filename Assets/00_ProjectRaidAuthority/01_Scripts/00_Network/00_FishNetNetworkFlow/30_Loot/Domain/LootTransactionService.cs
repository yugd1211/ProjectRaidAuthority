using System.Collections.Generic;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// RequestId 기반 멱등성, ItemInstance 상태 전이, 소유권 commit/reject를 담당하는 순수 루팅 transaction 경계입니다.
    /// 입력/UI/FishNet RPC/월드 오브젝트 표시는 담당하지 않습니다.
    /// </summary>
    public sealed class LootTransactionService
    {
        private readonly InMemoryItemInstanceRepository repository;
        private readonly HashSet<LootRequestId> handledRequestIds = new();

        /// <summary>기본 in-memory 저장소를 사용하는 transaction service를 생성합니다.</summary>
        public LootTransactionService()
            : this(new InMemoryItemInstanceRepository())
        {
        }

        /// <summary>지정한 저장소를 사용하는 transaction service를 생성합니다.</summary>
        public LootTransactionService(InMemoryItemInstanceRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>FishNet adapter가 관찰한 최신 item projection을 transaction 저장소에 반영합니다.</summary>
        public void TrackItem(ItemInstanceRecord record)
        {
            repository.Upsert(record);
        }

        /// <summary>루팅 요청을 서버 권한 transaction으로 commit/reject/duplicate 판정합니다.</summary>
        public LootDecision TryCommitLoot(LootCommand command)
        {
            LootRequest request = command.Request;
            if (!request.IsValid)
            {
                return Reject(LootDecisionKind.RejectedInvalidRequest, request, -1, LootItemState.Available, "Invalid requestId or itemId");
            }

            if (handledRequestIds.Contains(request.RequestId))
            {
                return Duplicate(request);
            }

            handledRequestIds.Add(request.RequestId);

            if (!command.IsItemPresent)
            {
                return Reject(LootDecisionKind.RejectedNotFound, request, -1, LootItemState.Available, "Loot item not found");
            }

            if (!repository.TryGet(request.ItemId, out ItemInstanceRecord record))
            {
                return Reject(LootDecisionKind.RejectedNotFound, request, -1, LootItemState.Available, "Loot item not found");
            }

            if (!command.IsDistanceValid)
            {
                return Reject(LootDecisionKind.RejectedInvalidDistance, request, record.OwnerId, record.State, "Loot item out of range");
            }

            if (record.State == LootItemState.Looted)
            {
                return Reject(LootDecisionKind.RejectedAlreadyLooted, request, record.OwnerId, record.State, "Loot item already looted");
            }

            if (record.State != LootItemState.Available)
            {
                return Reject(LootDecisionKind.RejectedInvalidState, request, record.OwnerId, record.State, "Loot item state is not lootable");
            }

            ItemInstanceRecord committed = record.MarkLooted(command.OwnerId);
            repository.Upsert(committed);
            return new LootDecision(
                LootDecisionKind.Committed,
                request.RequestId,
                request.ItemId,
                committed.OwnerId,
                committed.State,
                "Committed");
        }

        private LootDecision Duplicate(LootRequest request)
        {
            int ownerId = -1;
            LootItemState state = LootItemState.Available;
            if (repository.TryGet(request.ItemId, out ItemInstanceRecord record))
            {
                ownerId = record.OwnerId;
                state = record.State;
            }

            return new LootDecision(
                LootDecisionKind.DuplicateIgnored,
                request.RequestId,
                request.ItemId,
                ownerId,
                state,
                "Duplicate LootRequest ignored");
        }

        private static LootDecision Reject(
            LootDecisionKind kind,
            LootRequest request,
            int ownerId,
            LootItemState state,
            string reason)
        {
            return new LootDecision(kind, request.RequestId, request.ItemId, ownerId, state, reason);
        }
    }
}
