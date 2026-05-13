# 아키텍처 — ProjectRaidAuthority MVP

## 문서 상태

- **버전**: 0.2
- **마지막 갱신**: 2026-05-14
- **상태**: 기술 설정 기준 승인
- **엔진**: Unity 6000.4.5f1
- **네트워킹**: FishNet authoritative dedicated server

## 1. 아키텍처 의도

ProjectRaidAuthority는 전용 서버가 전투, 전리품, 추출, 특성 선택, 결과 저장, 이벤트 로그의 진실을 소유하는 탑다운 PvE 추출 로그라이트 MVP다. 목표는 콘텐츠 양이 아니라 1인 개발자가 6개월 안에 보여줄 수 있는 신뢰도 높은 서버 권한 raid loop다.

## 2. 절대 조건

1. 클라이언트는 입력을 보내고 표시/보간만 한다.
2. 서버는 위치, HP, 상태, 아이템 소유, raid phase, 추출 성공, 특성 상태, 결과 저장을 확정한다.
3. 모든 아이템 소유 변경은 멱등적인 서버 transaction을 거친다.
4. 성공 추출만 raid loot를 stash/profile에 저장한다.
5. Unity/FishNet API 사용은 구현 전 로컬 패키지와 공식 문서로 검증한다.
6. 서버 권한 루프가 증명되기 전에는 MVP 콘텐츠 범위를 넓히지 않는다.

## 3. 시스템 계층

| 계층 | 시스템 | 책임 |
|---|---|---|
| 기반 | 서버 권한 핵심 | dedicated server, tick, 검증된 입력, 엔티티 registry, snapshot/event |
| 핵심 게임플레이 | 전투와 AI, 전리품, Raid 흐름 | 서버 판정 전투, 아이템 소유권, raid state, 결과 commit |
| 기능 | Roguelike Run Build | trait, synergy, curse, run modifier |
| 운영 | 유지/운영 증거 | event log, Docker/server 증거, QA 체크 |
| 표현 | HUD/결과 UI, 오디오 | 확정 상태 표시. 진실 소유 금지 |

## 4. 런타임 구조

```text
Unity Client
  -> ClientInput(tick, seq, move, aim, fire, reload, interact)
  -> FishNet Dedicated Raid Server
      -> 입력 검증
      -> 서버 tick 시뮬레이션
      -> Entity / Raid / Inventory / Trait 상태
      -> 확정 Event + World Snapshot
  -> Unity Client 표시/보간
```

## 5. 데이터 소유권

| 상태 | 소유자 | 읽는 쪽 | 쓰기 규칙 |
|---|---|---|---|
| 플레이어 최종 위치/회전 | 서버 핵심 | UI, 전투, AI | 서버만 기록 |
| HP/피해/상태 | 서버 전투 시스템 | UI, Raid 흐름, Trait | 서버만 기록 |
| 아이템 소유 | 서버 전리품 시스템 | UI, 결과 화면, QA log | transaction으로만 기록 |
| raid phase/추출 | 서버 Raid 흐름 | UI, 전리품, 운영 | 서버만 기록 |
| trait/synergy/curse | 서버 Run Build | 전투, UI, 결과 화면 | 유효 선택 후 서버만 기록 |
| profile/stash 결과 | 서버 결과 commit | 운영, future lobby | commit boundary에서만 기록 |

## 6. 후보 인터페이스

```csharp
record ClientInput(int Tick, int Sequence, Vector2 Move, Vector2 Aim, bool Fire, bool Reload, bool Interact);
record EntitySnapshot(string EntityId, Vector2 Position, float Rotation, int Hp, int StatusBits);
record RaidEvent(string RaidId, long Sequence, string EventType, object Payload, int ServerTick);
record LootRequest(string ItemInstanceId, string ContainerId, string RequestId);
record LootCommitted(string ItemInstanceId, string FromContainerId, string ToPlayerId, string RequestId, int ServerTick);
```

위 형태는 아키텍처 수준의 후보이며 최종 코드 계약이 아니다. 구현 스토리는 Unity/FishNet serialization/RPC 제약을 먼저 확인해야 한다.

## 7. 검증 순서

1. 헤드리스 server 실행
2. 두 클라이언트 접속
3. 서버 소유 더미 엔티티 snapshot 확인
4. 클라이언트 직접 state write 거절 확인
5. 서버 판정 피해 또는 loot transaction 확인
6. 추출 성공/실패 결과 commit 확인

## 8. 다음 구현 전제

아키텍처 문서는 승인되었지만 실제 production story는 아직 만들지 않는다. 먼저 FishNet 서버 권한 스모크 프로토타입이 필요하다.
