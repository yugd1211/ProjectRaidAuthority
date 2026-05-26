# 서버 도메인 데이터 모델 초안

상태: Plan  
작성 기준일: 2026-05-26  
원천: `Assets/00_ProjectRaidAuthority/Docs/00_Plan/03_TechOps/00_authoritative_server_direction.md`, `.omx/plans/server-domain-layering-consensus-plan.md`  
목적: 현재 FishNet Game Server에서 바로 사용할 수 있으면서, 이후 Backend server와 Database로 옮기기 쉬운 canonical 서버 도메인 모델을 먼저 고정한다.

## 1. 계층 개요

```text
Future Database layer
  - PlayerProfile / Stash / RaidEventArchive / Audit 저장
  - 실제 DB schema, migration, ORM은 현재 만들지 않는다.

Future Backend layer
  - Match/Profile/Inventory/RaidResult service contract
  - HTTP, gRPC, 외부 인증, 실제 백엔드 프로세스는 현재 만들지 않는다.

Current FishNet Game Server layer
  - 실시간 raid authority
  - FishNet NetworkBehaviour, ServerRpc, SyncVar, NetworkObject를 adapter/projection으로 사용한다.
```

현재 구현 순서는 “Database를 실제로 만든다”가 아니라, **Database에 저장 가능한 의미를 먼저 문서화하고 FishNet 서버 내부의 순수 service/in-memory repository로 작게 검증한다**는 뜻이다.

## 2. 경계 원칙

| 원칙 | 현재 의미 | 향후 이전 의미 |
|---|---|---|
| Adapter owns FishNet types | `NetworkBehaviour`, `NetworkObject`, `OwnerId`, `SyncVar`, `ServerRpc`, `Transform`은 FishNet adapter에만 둔다. | Backend/DB로 이전할 때 adapter만 교체한다. |
| Domain owns pure ids/states | `PlayerId`, `RaidId`, `ItemInstanceId`, `ItemInstanceState`, `LootDecision`은 FishNet을 참조하지 않는다. | DB row key, Backend DTO, replay event로 재사용한다. |
| Application service owns rules | 중복 request, already looted, 소유권 commit 같은 규칙은 service가 판단한다. | FishNet 밖 Backend service로 올릴 수 있다. |
| FishNet server owns real-time validation | 거리, line-of-sight, scene object 존재, connection authority는 FishNet/Unity adapter가 검증하거나 command 입력으로 변환한다. | dedicated raid server 또는 authoritative simulation service에 남긴다. |
| SyncVar owns projection | 클라이언트 표시와 late snapshot 후보를 위한 복제 상태다. | DB 저장 모델이 아니라 presentation/snapshot projection이다. |

## 3. Canonical IDs

| ID | 의미 | 현재 소유자 | Future owner | 비고 |
|---|---|---|---|---|
| `PlayerId` | 계정/프로필 기준 플레이어 식별자 | FishNet 세션에서 임시 발급 또는 개발용 고정값 | Backend profile/auth service | `ConnectionId`와 다르다. |
| `ConnectionId` | FishNet 접속 식별자 | FishNet adapter | Dedicated server runtime | 재접속/서버 이전 시 안정 ID로 쓰지 않는다. |
| `RaidId` | 하나의 raid session 식별자 | FishNet Game Server bootstrap | Match/Raid orchestration service | 로그, 결과 commit, replay의 루트 키다. |
| `EntityId` | 플레이어/몬스터/상호작용 오브젝트의 서버 발급 식별자 | FishNet Game Server | Raid simulation service | `NetworkObjectId`와 1:1이라고 가정하지 않는다. |
| `ItemDefinitionId` | 포션/코인/무기 template 식별자 | 콘텐츠 데이터 | Backend content/profile service | 수량과 무관한 “종류” ID다. |
| `ItemInstanceId` | 거래/소유권 추적이 필요한 아이템 단위 식별자 | Loot service/in-memory repository | Inventory DB / Raid item table | stack 내부 개별 단위를 모두 ID로 추적할지 여부는 아이템 정책에 따른다. |
| `InventoryStackId` | 인벤토리 슬롯/스택 식별자 | Inventory projection 또는 future service | Inventory DB | `ItemInstanceId` 배열을 가질 수도 있고, currency처럼 aggregate quantity만 가질 수도 있다. |
| `ContainerId` | world loot, chest, player inventory, stash 같은 보관 위치 식별자 | FishNet raid runtime | Inventory/Profile DB | 컨테이너 자체가 NetworkObject일 필요는 없다. |
| `RaidEventSeq` | raid event append 순번 | FishNet Game Server | Raid event archive | 재현/감사 로그의 정렬 키다. |
| `RequestId` | 클라이언트 요청 멱등성 키 | Client 생성 + server 기록 | Backend/API idempotency key | 같은 요청 재전송을 중복 적용하지 않는다. |

## 4. Canonical data models

### 4.1 PlayerProfile

| 필드 | 의미 | 현재 범위 | Future owner |
|---|---|---|---|
| `playerId` | 안정 플레이어 ID | 개발용 placeholder | Profile DB |
| `displayName` | 표시 이름 | 선택 | Profile DB |
| `stashContainerId` | 영구 stash 컨테이너 | 비범위 | Inventory DB |
| `progressionFlags` | 해금/진행 상태 | 비범위 | Profile DB |

현재 FishNet smoke에서는 `OwnerId`/connection으로 플레이어를 식별하지만, 이것을 영구 `PlayerId`로 고정하지 않는다.

### 4.2 RaidSession

| 필드 | 의미 | 현재 범위 | Future owner |
|---|---|---|---|
| `raidId` | raid session ID | 개발용 생성 | Match/Raid service |
| `phase` | `Lobby`, `Loading`, `Active`, `Extracting`, `Completed`, `Failed` | 일부만 사용 | Raid service |
| `seed` | spawn/loot/AI seed | 후보 | Raid orchestration |
| `participants` | `PlayerId`/connection 매핑 | FishNet runtime | Match service |
| `serverTick` | 서버 simulation tick | 후보 | Raid simulation |
| `eventSeq` | 마지막 event 순번 | 후보 | Raid event archive |

### 4.3 EntityState

| 필드 | 의미 | 현재 범위 | Future owner |
|---|---|---|---|
| `entityId` | 서버 발급 엔티티 ID | 후보 | Raid simulation |
| `entityType` | Player, Enemy, LootContainer 등 | 후보 | Raid simulation |
| `position`/`rotation` | 서버 확정 transform 값 | FishNet server authority | Raid simulation |
| `hp`/`statusBits` | 전투 상태 | 비범위 | Combat service |
| `ownerPlayerId` | 플레이어 소유 엔티티면 소유자 | 후보 | Raid simulation |

Unity `Transform` 자체는 canonical model이 아니다. 값만 command/snapshot으로 변환한다.

### 4.4 ItemInstance

| 필드 | 의미 | 현재 범위 | Future owner |
|---|---|---|---|
| `itemInstanceId` | 소유권 변경 추적 단위 | loot smoke item에 사용 | Inventory/Raid DB |
| `itemDefinitionId` | 아이템 종류 | `loot-smoke-item` 계열 | Content/Profile DB |
| `state` | `WorldAvailable`, `Reserved`, `InventoryOwned`, `StashOwned`, `Consumed`, `Destroyed` | `WorldAvailable`/`InventoryOwned`/`Looted projection` | Inventory/Raid DB |
| `containerId` | 현재 위치/소유 컨테이너 | world/player 후보 | Inventory DB |
| `ownerPlayerId` | 소유 플레이어 | loot commit 시 확정 | Inventory DB |
| `quantity` | aggregate 가능한 아이템 수량 | currency/stack policy에서 사용 | Inventory DB |
| `lastRequestId` | 마지막 commit 요청 키 | 멱등성 검증 후보 | Audit/Event log |

`ItemInstanceId`는 “아이템 종류 ID”가 아니다. `potion_small` 16개가 한 슬롯에 있을 때 다음 두 정책 중 하나를 선택할 수 있다.

- **개별 추적 정책**: 각 포션 단위가 durability, roll, bind, audit이 필요하면 16개의 `ItemInstanceId`를 가진다. 스택은 16개 ID를 묶는 `InventoryStackId`다.
- **수량 집계 정책**: 포션이 완전히 동일하고 개별 이력이 중요하지 않으면 하나의 stack record가 `itemDefinitionId=potion_small`, `quantity=16`만 가진다. 이때 `ItemInstanceId`는 world/drop/거래 commit 단위 또는 stack lot 단위로만 둘 수 있다.

MVP 기본값은 **거래/소유권 경계에서는 `ItemInstanceId`를 유지하되, 대량 currency나 완전 동일 소모품은 future stack policy에서 aggregate를 허용**한다.

### 4.5 InventoryStack

| 필드 | 의미 | 현재 범위 | Future owner |
|---|---|---|---|
| `inventoryStackId` | 슬롯/스택 ID | 비범위 | Inventory DB |
| `containerId` | player inventory/stash 등 | 비범위 | Inventory DB |
| `itemDefinitionId` | 스택 내 아이템 종류 | 비범위 | Inventory DB |
| `quantity` | 표시/수량 계산값 | 비범위 | Inventory DB |
| `itemInstanceIds` | 개별 추적 정책일 때 포함 | 비범위 | Inventory DB |
| `maxStack` | 슬롯 최대 수량 | content data 후보 | Content/Profile DB |

`InventoryStackId`는 UI 슬롯과 저장 record를 추적하기 위한 ID이고, `ItemInstanceId`는 소유권/거래 무결성 단위다. 둘은 같은 ID가 아니다.

### 4.6 RaidEvent

| 필드 | 의미 | 현재 범위 | Future owner |
|---|---|---|---|
| `raidId` | 이벤트가 속한 raid | 후보 | Raid event archive |
| `seq` | append-only 순번 | 후보 | Raid event archive |
| `eventType` | `LootCommitted`, `LootRejected`, `DuplicateLootRequestIgnored` 등 | 로그 패턴 존재 | Raid event archive |
| `payload` | 이벤트별 데이터 | 로그 문자열 중심 | Event archive |
| `serverTick` | 발생 tick | 후보 | Raid simulation |
| `requestId` | 멱등성 연결 키 | loot smoke에서 사용 | Audit/Event log |

`RaidEvent`는 현재 runtime log/event이면서, 이후 append-only audit log 후보다.

## 5. 상태 전이

### RaidPhase

```text
Created -> Loading -> Active -> Extracting -> Completed
                         └──────-> Failed
```

- 현재 smoke는 MatchRoom ready 후 Gameplay 진입과 Active 흐름만 검증했다.
- extraction/result commit은 후속 범위다.

### PlayerLifeState

```text
Spawning -> Alive -> Downed -> Dead
                 └-> Extracted
```

- 현재 이동/loot smoke에서는 `Alive` 중심으로 본다.
- HP/피해/부활은 combat story에서 확정한다.

### ItemInstanceState

```text
WorldAvailable -> Reserved -> InventoryOwned -> StashOwned
       └───────────────-> Destroyed
InventoryOwned -> Consumed
```

- `WorldAvailable`: raid world에서 획득 가능하다.
- `Reserved`: 서버가 transaction 처리 중이며 경쟁 요청을 막는다.
- `InventoryOwned`: raid 중 플레이어 inventory에 들어갔다.
- `StashOwned`: 성공 추출 후 영구 stash에 commit됐다.
- `Consumed`: 포션처럼 사용으로 사라졌다.
- `Destroyed`: 실패 추출/월드 정리/운영 삭제 등으로 제거됐다.

현재 `LootItemState.Looted`는 FishNet presentation projection이며, canonical state로는 `InventoryOwned` 또는 `WorldAvailable이 아님`에 해당한다.

## 6. Application service contracts

### 6.1 LootService.TryCommitLoot

```text
LootService.TryCommitLoot(LootCommand command) -> LootDecision
```

`LootCommand` 후보 필드:

| 필드 | 의미 |
|---|---|
| `raidId` | raid session ID |
| `requestId` | 멱등성 키 |
| `playerId` | 요청 플레이어 canonical ID |
| `connectionId` | 현재 FishNet 연결 ID |
| `itemInstanceId` | 획득 대상 |
| `sourceContainerId` | world/chest 등 출처 |
| `targetContainerId` | player inventory 후보 |
| `serverTick` | 요청 처리 tick |
| `validatedDistance` | adapter가 scene 기준으로 검증한 거리 결과 |

`LootDecision` 후보 결과:

| 결과 | 의미 |
|---|---|
| `Committed` | 소유권 변경 성공, `LootCommitted` event 생성 |
| `DuplicateIgnored` | 같은 `RequestId` 재전송, 상태 변경 없음 |
| `RejectedAlreadyLooted` | 이미 다른 소유/상태로 바뀜 |
| `RejectedInvalidDistance` | adapter distance 검증 실패 |
| `RejectedNotFound` | 대상 item 없음 |
| `RejectedInvalidState` | 상태 전이가 허용되지 않음 |

### 6.2 Future services

| Service | 현재 상태 | Future owner | 비고 |
|---|---|---|---|
| `InventoryService.ApplyStackChange(...)` | 비범위 | Backend inventory service | stack merge/split/consume 정책 담당 |
| `RaidResultService.CommitExtraction(...)` | 비범위 | Backend profile/result service | 성공 추출만 stash/profile commit |
| `MatchAssignmentService.Assign(...)` | 개발용 `SingleServerMatchmaker` 방향 | Backend match service | FishNet 접속 주소/토큰 배정 |

## 7. FishNet adapter mapping

| FishNet/Unity 표면 | 도메인 매핑 | 규칙 |
|---|---|---|
| `GamePlayer.LootServer.cs` | `LootCommand` adapter | `ServerRpc` payload와 scene 검증을 순수 command로 변환한다. |
| `LootItem` | `ItemInstance` availability projection | world object와 item instance가 항상 1:1이라고 가정하지 않는다. |
| `SyncVar` | client presentation/snapshot projection | DB 저장 모델이 아니다. UI/후보 제외 상태만 동기화한다. |
| `[ServerRpc]` | request transport | 신뢰 경계가 아니며 내부에서 반드시 서버 검증을 통과해야 한다. |
| `[Server]` | server-only truth mutation guard | item ownership, 위치, HP, result 같은 truth 변경 메서드에 기본 적용한다. |
| `NetworkObject`/`OwnerId` | runtime transport identity | canonical `EntityId`/`PlayerId`와 분리한다. |

## 8. MVP 정책

- 지금은 실제 DB를 만들지 않는다.
- 지금은 HTTP/gRPC Backend process를 만들지 않는다.
- 첫 구현은 in-memory repository만 사용한다.
- 첫 service는 `LootService.TryCommitLoot` 하나로 제한한다.
- `Player/Raid/Entity/Inventory` 전체 구현은 후속 story로 분리한다.
- 기존 FishNet smoke 로그인 `LootCommitted`, `LootRejected`, `Duplicate LootRequest ignored`는 유지한다.

## 9. 문서 수용 기준 체크

- [x] DB-ready / application contract / FishNet adapter 섹션을 모두 가진다.
- [x] 실제 DB/Backend 구현을 하지 않는다고 명시한다.
- [x] `ItemInstanceId`와 `InventoryStackId`의 차이를 명확히 적는다.
- [x] `LootItem`과 `ItemInstance`가 항상 1:1이 아닐 수 있음을 적는다.
- [x] `RaidEvent`가 runtime event이자 future append-only audit log 후보임을 적는다.
- [x] 첫 story가 `LootService` 하나로 제한된다고 적는다.
- [x] `Adapter owns FishNet types, Domain owns pure ids/states` 규칙을 포함한다.
