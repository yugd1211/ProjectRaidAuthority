# Server Core Story 001 — LootTransactionService 경계 분리

## 상태

Draft — FishNet authority smoke 완료 이후 첫 server-core story 후보  
작성 기준일: 2026-05-26

## 목표

이 story에서 `Service`는 루팅 기능 전체 관리자가 아니라 backend/application 계층의 contract 관습을 따른 이름이다. 혼동을 줄이기 위해 넓은 `LootService` 대신 `LootTransactionService`를 사용한다.

현재 FishNet loot smoke 동작을 유지하면서, loot 소유권 commit 규칙을 FishNet `NetworkBehaviour` 내부 구현에서 순수 application transaction 경계로 분리한다. 이 story는 미래 Backend/Database 이전을 준비하지만 실제 DB, Backend process, HTTP/gRPC API는 만들지 않는다.

## 근거 문서

- `.omx/plans/server-domain-layering-consensus-plan.md`
- `Assets/00_ProjectRaidAuthority/Docs/00_Plan/03_TechOps/01_server_domain_data_model.md`
- `Assets/00_ProjectRaidAuthority/Docs/00_Plan/03_TechOps/00_authoritative_server_direction.md`
- `docs/architecture/architecture.md`
- `docs/architecture/control-manifest.md`
- `docs/architecture/adr-0004-networkbehaviour-adapter-composition.md`
- `design/gdd/authoritative-server-core.md`
- `production/prototype-plan-fishnet-authority-smoke.md`
- `prototypes/fishnet-authority-smoke/README.md`

## 사용자 가치

개발자는 FishNet Game Server 안에서 바로 검증 가능한 loot transaction을 유지하면서, 이후 Backend inventory service나 DB-backed repository로 옮길 때 `LootTransactionService` contract만 유지하고 adapter/repository를 교체할 수 있다.

## 범위

### 포함

- 순수 C# `LootTransactionService.TryCommitLoot(LootCommand) -> LootDecision` 설계/구현.
- in-memory `ItemInstanceRepository` 또는 동등한 repository 경계 설계/구현.
- `GamePlayer.LootServer.cs`가 FishNet/Unity 데이터를 `LootCommand`로 변환하도록 정리.
- `LootItem`은 `ItemInstance`의 획득 가능 상태를 표시하는 projection으로 유지.
- 기존 로그 패턴 유지: `LootCommitted`, `LootRejected`, `Duplicate LootRequest ignored`.
- duplicate `RequestId`, already-looted item, invalid distance/state에 대한 명시적 결과 분기.

### 제외

- 실제 Database schema, migration, ORM.
- REST/gRPC Backend server.
- 전체 `Player/Raid/Entity/Inventory` 구현.
- 영구 stash 저장과 extraction result commit.
- late join/reconnect snapshot 복원 정책 구현.
- 대량 loot despawn/pooling 최적화.

## 설계 방향

```text
Client input
  -> FishNet [ServerRpc] request transport
  -> GamePlayer.LootServer.cs adapter
      - connection/owner 검증
      - scene distance 검증
      - NetworkObject/LootItem 조회
      - LootCommand 생성
  -> LootTransactionService.TryCommitLoot(command)
      - RequestId 멱등성
      - item state 전이
      - ownership commit
      - RaidEvent 후보 생성
  -> InMemoryItemInstanceRepository
  -> FishNet projection update
      - LootItem SyncVar state
      - Renderer/Collider presentation 비활성화
      - server log 유지
```

## Acceptance Criteria

### AC-01 — 서버 권한 입력 경계

- 클라이언트는 `itemId`/`requestId`/상호작용 요청 payload만 보낸다.
- 서버는 요청값을 sanitize/check한 뒤 loot 상태와 소유권을 확정한다.
- 클라이언트 코드가 item ownership, stash/profile result, loot availability를 직접 확정하지 않는다.

### AC-02 — FishNet 계약 가시성

- `[ServerRpc]`, `[Server]`, `SyncVar`, `NetworkBehaviour` lifecycle이 어느 adapter 파일에 있는지 리뷰어가 한 번에 찾을 수 있다.
- 현재 `partial` 구조를 유지한다면, story 완료 보고에 예외 사유와 계약 소유 파일 표를 남긴다.
- 순수 domain/service 파일에는 FishNet namespace, `NetworkObject`, `OwnerId`, `Transform`, `SyncVar`가 들어가지 않는다.

### AC-03 — LootTransactionService 순수 규칙

- `LootTransactionService.TryCommitLoot`는 Unity/FishNet 타입 없이 테스트할 수 있다.
- 같은 `RequestId`의 재시도는 두 번째부터 상태 변경 없이 `DuplicateIgnored` 또는 동등한 결과를 반환한다.
- 이미 획득된 item에 대한 새 요청은 소유권을 바꾸지 않고 reject 결과를 반환한다.
- state transition은 `WorldAvailable -> InventoryOwned` 또는 story에서 정의한 동등 전이만 허용한다.

### AC-04 — Repository 교체 가능성

- 첫 구현은 in-memory repository만 사용한다.
- repository contract는 future DB-backed implementation으로 교체 가능하도록 FishNet 타입을 참조하지 않는다.
- 저장 대상 후보인 `ItemInstanceId`, `OwnerPlayerId`, `ContainerId`, `RequestId`, `RaidEventSeq` 의미를 문서와 맞춘다.

### AC-05 — 기존 smoke 회귀 금지

- FishNet smoke에서 이미 확인한 서버 이동/loot 흐름을 깨지 않는다.
- 성공 loot 후 다른 클라이언트에서도 loot item이 획득 불가/보이지 않게 projection된다.
- 루트 `NetworkObject`를 즉시 despawn하지 않는 현 정책을 유지한다.

### AC-06 — 로그/이벤트 증거

- 성공 시 `LootCommitted` 또는 동등한 서버 확정 로그가 남는다.
- 이미 획득된 item 요청은 `LootRejected` 또는 동등한 거절 로그가 남는다.
- 같은 `RequestId` 반복 요청은 `Duplicate LootRequest ignored` 또는 동등한 멱등 로그가 남는다.

### AC-07 — Tick/Snapshot/AOI 추적

- 이 story에서 tick/snapshot/AOI 값을 확정하지 않더라도, 완료 보고에 현재 후보값 또는 “별도 ADR 필요”를 명시한다.
- `design/gdd/authoritative-server-core.md`의 tick/snapshot/AOI 후보와 충돌하는 runtime 상수를 추가하지 않는다.

### AC-08 — Late join/reconnect 추적

- raid 중 신규 gameplay 참가자의 looted snapshot 복원은 이번 story 비범위로 둔다.
- 단, 완료 보고에 후속 story로 추적해야 한다는 문장을 남긴다.

## 구현 체크리스트

- [ ] `LootCommand`, `LootDecision` 후보 타입을 FishNet 타입 없이 정의한다.
- [ ] `ItemInstance`/repository 후보 타입을 FishNet 타입 없이 정의한다.
- [ ] `LootTransactionService.TryCommitLoot`가 duplicate/already-looted/not-found/invalid-state를 분기한다.
- [ ] `GamePlayer.LootServer.cs`가 scene/FishNet 검증 후 `LootCommand`를 생성한다.
- [ ] `LootItem` `SyncVar` projection과 presentation 비활성화 동작을 유지한다.
- [ ] 기존 smoke 로그 문자열 또는 동등한 검색 가능한 로그를 유지한다.
- [ ] story 완료 보고에 `partial` 구조 예외 여부와 계약 소유 파일을 기록한다.

## Verification Evidence 템플릿

완료 보고에는 아래 블록을 채운다.

```md
## Verification Evidence
- PASS/FAIL — Unity EditMode tests: `<Unity Test Runner 또는 명령>` → `<결과 요약>`
- PASS/FAIL — LootTransactionService pure tests: `<테스트명>` → `Committed/Duplicate/AlreadyLooted/InvalidState 결과`
- PASS/FAIL — Static server authority scan: `grep -RIn "[ServerRpc]\|[Server]\|SyncVar" Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow` → `<검토 결과>`
- PASS/FAIL — Client truth ownership scan: `<명령 또는 리뷰 범위>` → `<클라이언트 직접 확정 없음/발견 내용>`
- PASS/FAIL — FishNet manual smoke: `<scene/role/client count>` → `<서버 이동/LootCommitted/LootRejected 로그>`
- PASS/FAIL — Documentation traceability: `<GDD/ADR/manifest/story 링크>` → `<연결 확인>`
- NOT TESTED — `<미실행 항목>` → `<사유와 후속 작업>`
```

## 완료 조건

- `LootTransactionService` 경계가 FishNet adapter와 분리되어 있다.
- 현재 smoke에서 검증한 loot 획득/거절/중복요청 로그가 유지된다.
- 실제 DB/Backend를 만들지 않았다는 비범위가 지켜진다.
- `Assets/00_ProjectRaidAuthority/Docs/00_Plan/03_TechOps/01_server_domain_data_model.md`와 traceability가 맞는다.
