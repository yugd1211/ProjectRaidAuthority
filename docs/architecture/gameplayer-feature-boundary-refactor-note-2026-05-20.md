# GamePlayer 기능 경계 리팩토링 메모 — partial, composition, service 패턴 비교

## 상태

참고 메모 / 실행 보류  
작성일: 2026-05-20  
용도: 지금 당장 리팩토링하지 않고, 이후 `GamePlayer` 기능이 커질 때 아키텍처 판단 근거로 다시 확인한다.

## 저장 위치 판단

이 문서는 `docs/architecture/`에 둔다.

이유:

- `docs/architecture/adr-0004-networkbehaviour-adapter-composition.md`가 이미 `NetworkBehaviour adapter + 순수 collaborator` 경계를 승인된 결정으로 보관하고 있다.
- 이번 메모는 새로운 구현 지시가 아니라, ADR-0004를 실제 `GamePlayer`/Loot 확장 상황에 적용할 때의 판단 보조 자료다.
- `.omx/plans/`에는 관련 실행 계획이 이미 있지만, `.omx/`는 런타임/작업 상태 성격이 강해서 장기 아키텍처 참고 문서로는 부적합하다.
- `Assets/00_ProjectRaidAuthority/Docs/`는 PM, 주차 계획, 기획/사업/TechOps 운영 문서 성격이 강하고, 현재 문서는 코드 구조 리팩토링 판단 기준에 가깝다.

관련 문서:

- `docs/architecture/adr-0004-networkbehaviour-adapter-composition.md`
- `.omx/plans/prd-gameplayer-composition-refactor.md`
- `.omx/plans/test-spec-gameplayer-composition-refactor.md`
- `.omx/plans/ralplan-handoff-gameplayer-composition-refactor.md`

## 배경

현재 `GamePlayer`는 FishNet `NetworkBehaviour`이며, 일부 기능이 `partial class`로 분리되어 있다.

예시:

```text
Shared/GamePlayer.cs
30_Loot/Client/GamePlayer.LootClient.cs
30_Loot/Server/GamePlayer.LootServer.cs
```

현재 구조는 파일을 나눠 읽기 쉽게 만들지만, 컴파일 후 실제 객체는 하나의 `GamePlayer`다. 따라서 기능이 계속 늘어나면 다음 위험이 생긴다.

- `GamePlayer`가 모든 플레이어 기능을 소유하는 God Object가 될 수 있다.
- `Update()` 또는 lifecycle 메서드에 기능별 호출이 계속 추가될 수 있다.
- private field와 helper가 partial 파일 사이에서 암묵적으로 공유된다.
- 파일은 나뉘었지만 객체 책임은 분리되지 않는다.
- 신규 작업자가 RPC, SyncVar, serialized field, Unity lifecycle의 실제 소유 위치를 추적해야 한다.

반대로, `LootClient`, `LootAuthorityService` 같은 객체를 만들어 composition으로 조립하면 책임은 더 명확해진다. 하지만 FishNet/Unity 문맥 전달 비용이 생긴다.

## 핵심 이해 정리

### partial 방식

```text
GamePlayer
├─ GamePlayer.cs
├─ GamePlayer.LootClient.cs
└─ GamePlayer.LootServer.cs
```

특징:

- 파일만 나뉘고 실제 객체는 하나다.
- `OwnerId`, `IsOwner`, `IsServerStarted`, `transform`, `[ServerRpc]`에 직접 접근하기 쉽다.
- FishNet/Unity adapter 코드를 빠르게 작성하기 좋다.
- 그러나 기능이 늘면 `GamePlayer`의 책임이 계속 커진다.

### composition 방식

```text
GamePlayer
├─ LootClientInteractor
├─ LootAuthorityService
└─ Movement/Combat/Extraction collaborators
```

특징:

- 객체 책임이 실제로 분리된다.
- 테스트성과 유지보수성이 좋아진다.
- 하지만 초기화, context 전달, 참조 연결 비용이 생긴다.
- `GamePlayer`의 `OwnerId`, `transform`, `ServerRpc` 같은 문맥을 직접 쓰기 어렵다.
- Unity component로 나누면 prefab reference, `GetComponent`, 누락 검증, lifecycle 순서 문제가 추가된다.

## partial과 composition의 트레이드오프

| 기준 | partial `GamePlayer.*` | composition/collaborator |
|---|---|---|
| 실제 객체 분리 | 낮음 | 높음 |
| 파일 탐색성 | 단기적으로 좋음 | 설계가 좋으면 좋음 |
| 책임 격리 | 중간 이하 | 높음 |
| FishNet RPC 접근 | 매우 쉬움 | adapter를 거쳐야 함 |
| `OwnerId`/`transform` 접근 | 직접 접근 | context 전달 필요 |
| 초기 구현 속도 | 빠름 | 느릴 수 있음 |
| 테스트성 | 낮음~중간 | 높음 |
| prefab/Unity wiring 비용 | 낮음 | component 방식이면 증가 |
| God Object 위험 | 높음 | 낮음 |
| 장기 확장성 | partial만 계속 쓰면 낮음 | 높음 |

## 현재 코드에 대한 판단

현재 단계에서는 `partial GamePlayer`를 즉시 제거할 필요는 없다.

이유:

1. 현재 프로젝트는 아직 FishNet 서버 권한 스모크 프로토타입 검증 단계다.
2. `GamePlayer`는 `NetworkBehaviour`이므로 RPC, ownership, transform, Unity lifecycle과 강하게 묶여 있다.
3. Loot 기능은 아직 `E 입력 → ServerRpc → 서버 거리/상태 검증 → 로그` 수준이라 전면 composition은 과할 수 있다.
4. 이미 `GamePlayerMovementRules`, `InputRateLimiter`, `LootRequest`, `LootResult`처럼 일부 값/규칙이 분리되어 있어 점진 분리 방향이 열려 있다.

따라서 단기 기본값은 다음이다.

```text
partial 유지 가능
단, partial은 FishNet/Unity adapter 경계로 제한한다.
순수 규칙, 정책, 판정, 트랜잭션은 점진적으로 collaborator/service/rules로 이동한다.
```

## 추천 패턴

### 1. Network Adapter + Domain Service

가장 우선 추천하는 패턴이다.

```text
GamePlayer partial
  - FishNet/Unity lifecycle
  - [ServerRpc]
  - OwnerId, IsOwner, IsServerStarted
  - transform, Camera, InputAction 접근
  - domain service 호출

LootAuthorityService
  - 루팅 요청 검증
  - 거리/상태/중복 판정
  - commit 결과 생성
```

도입 이유:

- FishNet RPC와 Unity lifecycle은 `GamePlayer`에 남겨 안정성을 유지한다.
- 실제 게임 규칙은 service로 빼 테스트성과 책임 분리를 얻는다.
- 완전 DI/composition보다 이관 비용이 낮다.

주의점:

- service가 `GamePlayer` 전체를 참조하지 않도록 한다.
- service에는 필요한 값만 넘긴다.
- service가 `Transform`, `NetworkObject`, `Time`, `Debug.Log`, `OwnerId` 같은 runtime 객체를 직접 소유하지 않게 한다.

### 2. Command / Request-Result 패턴

서버 권한 게임에 잘 맞는다.

```text
Client
  → LootRequest
Server
  → Validate
  → LootResult
```

확장 예:

```text
MoveInputCommand
AttackCommand
LootCommand
InteractCommand
ExtractCommand
TraitSelectCommand
```

도입 이유:

- 클라이언트는 결과가 아니라 요청만 보낸다는 서버 권한 원칙과 맞다.
- requestId 기반 멱등성, 재전송, 로그 추적에 유리하다.
- 서버 확정 결과를 `RaidEvent`로 연결하기 쉽다.

### 3. Rules / Policy 패턴

판정 규칙을 순수 C#으로 분리한다.

예시:

```text
GamePlayerMovementRules
InputRateLimiter
LootValidationRules
CombatHitRules
DamageRules
ExtractionRules
```

도입 이유:

- 서버 권한 게임에서 자주 바뀌는 것은 규칙이다.
- 규칙을 Unity/FishNet 밖으로 빼면 EditMode 테스트로 고정하기 쉽다.
- 밸런스나 정책 변경 시 영향 범위가 줄어든다.

### 4. Context Object 패턴

service/rules에 넘기는 파라미터가 많아질 때 사용한다.

나쁜 방향:

```csharp
TryCommit(ownerId, playerPosition, itemId, requestId, currentTime, raidId, inventoryId, items);
```

나은 방향:

```csharp
LootRequestContext context;
LootResult result = lootAuthority.TryCommit(context);
```

도입 이유:

- composition의 단점인 파라미터 전달 비용을 줄인다.
- `GamePlayer` 전체 참조를 넘기지 않고 필요한 값만 묶을 수 있다.

주의점:

- context가 너무 커지면 또 다른 God Object가 된다.
- context는 해당 use case에 필요한 값만 담는다.

### 5. State Machine

전투, 사망, 탈출, 레이드 phase가 들어오면 필요해질 가능성이 높다.

예시:

```text
PlayerState
  - Spawning
  - Alive
  - Downed
  - Dead
  - Extracting
  - Extracted

RaidState
  - Waiting
  - InProgress
  - ExtractionOpen
  - Completed
```

도입 이유:

- 서버가 “지금 이 행동을 허용해도 되는가?”를 명확히 판단할 수 있다.
- 죽은 플레이어 루팅, 레이드 종료 후 RPC, 탈출 중 공격 같은 예외를 관리하기 쉽다.

### 6. Domain Event / RaidEvent

서버가 확정한 결과만 이벤트로 남긴다.

예시:

```text
LootCommitted
DamageApplied
PlayerDowned
ExtractionStarted
ExtractionCompleted
TraitSelected
```

도입 이유:

- 서버 권한 증거를 남기기 좋다.
- QA, 디버깅, 운영 로그, 복구/리플레이 확장에 유리하다.
- 문서의 append-only `RaidEvent` 방향과 일치한다.

### 7. Feature Component 패턴

기능이 매우 커진 뒤 검토한다.

```text
GamePlayer
├─ PlayerMovementFeature
├─ PlayerLootFeature
├─ PlayerCombatFeature
└─ PlayerCameraFeature
```

도입 이유:

- 전투, 루팅, 인벤토리, 상태이상, 탈출이 모두 붙으면 partial만으로는 한계가 온다.
- 기능별 책임과 팀 작업 경계를 나누기 쉽다.

주의점:

- 지금 단계에서는 과할 수 있다.
- Unity component로 쪼개면 prefab wiring, 초기화 순서, FishNet RPC 위치가 복잡해질 수 있다.

## 도입 우선순위

현재 프로젝트에 맞는 우선순위는 다음이다.

1. `GamePlayer` partial은 당분간 유지하되 adapter로 제한한다.
2. 커지는 판정 로직은 `Rules / Policy`로 먼저 뺀다.
3. 루팅/전투/탈출처럼 서버 확정이 필요한 흐름은 `Command / Request-Result`로 정리한다.
4. 파라미터가 많아지면 `Context Object`를 도입한다.
5. domain service가 필요할 정도로 로직이 커지면 `Network Adapter + Domain Service` 구조로 이동한다.
6. 전투/사망/탈출 phase가 복잡해지면 `State Machine`을 추가한다.
7. 서버 확정 결과가 운영/QA 증거가 되어야 하면 `Domain Event / RaidEvent`를 붙인다.
8. 기능 수가 많아져 adapter가 감당하기 어려우면 `Feature Component`를 검토한다.

## 리팩토링 트리거

아래 조건 중 2개 이상이 발생하면 partial 유지에서 service/collaborator 분리를 검토한다.

- `GamePlayer`의 특정 partial 파일이 150줄 이상으로 커진다.
- `Update()` 또는 lifecycle에 기능별 호출이 4개 이상 누적된다.
- 같은 기능의 server/client/shared state가 3개 이상 파일에 흩어진다.
- 해당 기능을 Unity PlayMode 없이 EditMode 테스트하고 싶어진다.
- 루팅이 단일 `LootItem`에서 container/inventory/stack/ownership transaction으로 확장된다.
- requestId 결과 캐싱, ack, retry, duplicate replay 처리가 필요해진다.
- 전투/피해/상태이상/탈출처럼 서버 판정 규칙이 독립적으로 커진다.
- `GamePlayer` 전체를 참조하지 않고도 계산 가능한 로직이 늘어난다.

## 유지할 경계 규칙

`GamePlayer` partial 또는 adapter에 남길 것:

```text
- NetworkBehaviour 상속
- FishNet lifecycle override
- [ServerRpc] / [ObserversRpc] / [TargetRpc]
- SyncVar 선언과 값 변경
- serialized field
- OwnerId, IsOwner, IsServerStarted 접근
- Transform, Camera, InputAction, Time, Debug.Log 접근
- domain service/rules 호출 glue code
```

`GamePlayer` 밖으로 빼야 할 것:

```text
- 거리/사거리 판정
- 중복 requestId 정책
- 루팅 가능 여부 검증
- 아이템 소유권 commit 규칙
- 피해량/히트 판정
- 탈출 성공/실패 조건
- 상태이상 Tick 정책
- RaidEvent 생성 규칙
```

## 피해야 할 방향

### partial만 계속 늘리기

```text
GamePlayer.LootServer.cs
GamePlayer.CombatServer.cs
GamePlayer.InventoryServer.cs
GamePlayer.SkillServer.cs
GamePlayer.ExtractionServer.cs
...
```

파일은 나뉘지만 객체 책임은 계속 `GamePlayer` 하나에 쌓인다.

### 모든 것을 Unity component로 쪼개기

지금 단계에서는 prefab wiring, 초기화 순서, FishNet RPC 위치가 복잡해질 수 있다.

### 상속으로 기능 나누기

```csharp
LootGamePlayer : GamePlayer
CombatGamePlayer : LootGamePlayer
```

Unity/FishNet 환경에서 기능 조합과 lifecycle 추적이 어려워지므로 피한다.

## 다음에 실제 리팩토링할 때의 실행 순서 초안

1. 현재 `GamePlayer` 관련 파일의 RPC, SyncVar, serialized field, lifecycle 위치를 기준선으로 기록한다.
2. 변경 전 EditMode 테스트 또는 수동 스모크 체크리스트를 고정한다.
3. partial을 즉시 제거하지 말고, 먼저 adapter와 순수 규칙의 경계를 표시한다.
4. 작은 규칙부터 `Rules / Policy`로 추출한다.
5. Loot가 커지면 `LootAuthorityService`와 `LootRequestContext`를 추가한다.
6. `GamePlayer`의 RPC 메서드는 얇게 유지하고 service 호출로 위임한다.
7. Unity compile, FishNet codegen, prefab missing reference, Host/Client 스모크를 확인한다.
8. 충분히 안정화된 뒤 `partial` 제거 또는 단일 adapter 파일 병합 여부를 다시 판단한다.

## 현재 결론

지금은 `partial GamePlayer`를 즉시 문제로 보지 않는다.

다만 이 구조를 최종 아키텍처로 확정하지 않는다. 현재의 올바른 해석은 다음이다.

```text
partial GamePlayer = FishNet/Unity adapter를 기능별로 임시 분리한 형태
장기 목표 = adapter는 얇게 유지하고, 규칙/정책/트랜잭션은 순수 collaborator/service로 분리
```

즉, 지금 당장은 partial이 실용적이다.  
하지만 기능이 늘어나는 순간에는 `Network Adapter + Domain Service`, `Rules / Policy`, `Command / Request-Result` 조합으로 점진 이전한다.
