# ADR-0004: FishNet NetworkBehaviour adapter와 순수 composition 경계

## 상태

승인 — FishNet authority smoke prototype 이후에도 같은 판단 기준을 기본값으로 사용한다.

## 배경

`GamePlayer`와 `RoomPlayer`는 처음에 `Shared/Client/Server/Camera` partial 파일로 분리되어 있었다. 이 방식은 빠르게 역할을 나누는 데 유리하지만, 실제 클래스 경계는 하나라서 `NetworkBehaviour` 상속 위치, override 가능한 FishNet lifecycle, `SyncVar`, `[ServerRpc]`, private 필드 소유자가 여러 파일로 흩어진다. 결과적으로 신규 작업자는 “왜 이 메서드가 override 가능한가”, “어떤 파일이 네트워크 계약을 소유하는가”, “순수 규칙과 Unity adapter가 어디서 갈라지는가”를 매번 추적해야 한다.

## 결정

FishNet/Unity 컴포넌트는 **단일 adapter/orchestrator 파일**이 계약을 소유한다. 계산·정책·검증 규칙은 가능한 한 **순수 C# collaborator**로 분리한다.

기본 규칙은 다음과 같다.

1. `NetworkBehaviour` 상속, `SyncVar`, `[ServerRpc]`, FishNet lifecycle override, serialized field, `IsOwner`/`IsServerStarted`/`OwnerId` 접근은 adapter 파일에 둔다.
2. 이동 clamp, 입력 rate limit, 값 sanitize, 서버 권한 정책처럼 Unity lifecycle 없이 검증 가능한 규칙은 순수 C# 객체로 분리한다.
3. 순수 collaborator는 `UnityEngine`, FishNet namespace, `Time`, `Vector2`/`Vector3`, `Transform`, `Camera`, `NetworkObject`에 의존하지 않는다.
4. 작은 `RoomPlayer`류 객체는 정책 분기나 테스트 케이스가 늘기 전까지 과분리하지 않는다.
5. `partial`은 Unity/FishNet 계약 가시성을 해치지 않는 생성 코드 또는 매우 좁은 이유가 있을 때만 사용한다.

## 결과와 영향

### 긍정적 영향

- 네트워크 계약의 소유 파일이 명확해져 디버깅 시작점이 빨라진다.
- 순수 collaborator는 EditMode 테스트로 고정할 수 있다.
- FishNet codegen과 Unity serialization에 영향을 주는 요소가 한 파일에 모인다.
- future refactor에서 “partial로 파일만 나누기”보다 “adapter와 규칙 경계 분리”를 우선 검토하게 된다.

### 비용과 위험

- adapter 파일이 커질 수 있다.
- collaborator가 Unity 타입을 다시 참조하면 경계가 무너진다.
- 작은 기능까지 무리하게 객체화하면 코드량이 늘고 읽기 비용이 커진다.

이를 막기 위해 adapter 파일은 다음 순서를 따른다.

1. serialized refs/settings
2. SyncVar/state
3. Unity/FishNet lifecycle
4. owner input adapter
5. server RPC/apply
6. camera adapter
7. private helpers

## 거절한 대안

- `partial` 유지: 변경 위험은 낮지만 상속/RPC/필드 출처 분산 문제를 해결하지 못한다.
- 단일 거대 파일 회귀: 계약은 잘 보이지만 계산·정책 테스트성이 낮고 God object 위험이 크다.
- 완전 DI/composition: 테스트성은 가장 좋지만 현재 1인 MVP 프로토타입 단계에서는 이관 비용이 과하다.

## ADR 의존성

- `docs/architecture/adr-0001-unity-fishnet-authoritative-server.md`
- `docs/architecture/control-manifest.md`
- `.omx/plans/prd-gameplayer-composition-refactor.md`
- `.omx/plans/test-spec-gameplayer-composition-refactor.md`

## 엔진 호환성

- Unity 6000.4.5f1
- URP 17.4.0
- FishNet under `Assets/10_FishNet`
- Unity Test Framework 1.6.0

## 연결된 GDD 요구사항

- `design/gdd/authoritative-server-core.md`
- `design/gdd/raid-flow-system.md`

## 검증 기준

- [ ] FishNet 계약은 adapter 파일에서 바로 찾을 수 있다.
- [ ] 순수 collaborator는 Unity/FishNet 타입을 참조하지 않는다.
- [ ] collaborator는 EditMode 테스트로 최소 sanitize/rate-limit/권한 정책을 검증한다.
- [ ] 2클라이언트 스모크에서 owner 입력, 서버 이동/회전, ready 흐름이 회귀하지 않는다.
