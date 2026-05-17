# Master Development Plan — ProjectRaidAuthority MVP

상태: Plan  
문서 유형: PM / 개발총괄 Master Plan  
대상 기간: 6개월 MVP  
대상 제품: Unity + FishNet authoritative dedicated server 기반 탑다운 로그라이크 익스트랙션 MVP  
작성일: 2026-05-11  
작성 기준: `$ralplan` consensus + `$ccgs-adopt` 개발총괄 관점

## Source of Truth

- Product overview: `../01_GameDesign/00_project_vision_gdd_lite.md`
- Roadmap baseline: `00_6month_mvp_roadmap.md`
- Tech/Ops baseline: `../03_TechOps/00_authoritative_server_direction.md`
- FishNet network flow architecture: `../../../../../../docs/architecture/fishnet-network-flow-architecture.html`
- Business/market baseline: `../04_BusinessMarket/00_market_business_validation.md`

## Lifecycle

| 상태 | 의미 | 이동 조건 |
|---|---|---|
| `Plan` | 개발 기준으로 검토 중 | 아직 특정 phase/week의 실행 기준으로 채택되지 않음 |
| `InProgress` | 현재 phase/week 실행 기준 | 특정 기간의 실제 PM/개발총괄 판단 기준으로 채택됨 |
| `Complete` | 승인/대체/폐기 기록 완료 | phase 종료 후 결정 요약과 후속 문서를 기록함 |

Complete closeout required fields:

- 완료일:
- 결정 요약:
- 후속 문서:
- 폐기 또는 대체 여부:

## 1. Product North Star

**한 줄 정의:** 서버가 전투·루팅·탈출·결과 저장을 확정하는 탑다운 로그라이크 익스트랙션 멀티플레이 MVP.

- [ ] 한 줄 정의가 서버 권위 탑다운 로그라이크 익스트랙션 MVP로 고정됨.
- [ ] Core loop가 `레이드 입장 → 탐색/루팅 → 교전 → 특성 선택 → 탈출 선택 → 결과 저장 → 메타 해금`으로 고정됨.
- [ ] MVP 필수 범위와 제외 범위가 별도 표로 분리됨.
- [ ] “서버 권위 런 계약 게임” 전복 포인트가 제품 차별점으로 채택/보류 결정됨.

## 2. MVP Scope Contract

### MVP 필수 범위

- [ ] 플레이어 범위: 1~4인 우선, 4~8인은 확장 목표로 명시.
- [ ] 맵 범위: 1개 맵 MVP로 고정.
- [ ] 세션 범위: 10~15분 레이드로 고정.
- [ ] 전투 범위: 무기 2종, 일반 적 2종, 엘리트 1종.
- [ ] 로그라이크 범위: 특성 8개, 시너지 4개, 저주 3개.
- [ ] 운영 범위: Docker 실행, 이벤트 로그, QA 리포트.

### MVP 제외 범위

- [ ] 정식 PvP 제외.
- [ ] 거래소/경제 시스템 제외.
- [ ] 다중 맵 제외.
- [ ] 심화 무기 개조 제외.
- [ ] 과금/BM 시스템 구현 제외.

## 3. Source-of-Truth Mapping Table — Asset Docs ↔ CCGS Standard Paths

| 결정/정보 축 | Asset Docs 근거 | CCGS 표준 경로/대상 | 운영 규칙 |
|---|---|---|---|
| Product vision / core loop | `Assets/00_ProjectRaidAuthority/Docs/00_Plan/01_GameDesign/00_project_vision_gdd_lite.md` | `design/gdd/game-concept.md` | Asset Docs를 원문 근거로 삼고 CCGS GDD에는 요약/참조를 작성한다. |
| 6개월 roadmap / phase gates | `Assets/00_ProjectRaidAuthority/Docs/00_Plan/02_ProductPlan/00_6month_mvp_roadmap.md` | `production/stage.txt`, `production/sprint-status.yaml` | roadmap은 Asset Docs가 원문, CCGS production 파일은 현재 phase/week 운영 상태를 기록한다. |
| Server authority / tech ops | `Assets/00_ProjectRaidAuthority/Docs/00_Plan/03_TechOps/00_authoritative_server_direction.md`, `docs/architecture/fishnet-network-flow-architecture.html` | `docs/architecture/adr-*.md`, `docs/architecture/control-manifest.md`, `design/gdd/systems-index.md` | 기술 방향은 Asset Docs가 기준, FishNet 흐름 HTML은 현재 제안 ADR/설명회 자료, ADR/control-manifest는 승인된 결정과 개발 규칙만 고정한다. |
| Business / market validation | `Assets/00_ProjectRaidAuthority/Docs/00_Plan/04_BusinessMarket/00_market_business_validation.md` | master plan의 Business/PM Axis, 추후 PRD/market validation appendix | BM 구현은 MVP 제외로 유지하되 플랫폼/가격/서버비/외부시연/시장검증 hold criteria를 master plan에 반영한다. |
| Weekly execution status | `Assets/00_ProjectRaidAuthority/Docs/01_InProgress/00_WeeklyProgress/*.md` | `production/sprint-status.yaml` | 주간 계획 본문은 Asset Docs에 작성하고 sprint-status는 상태/담당/판단/다음 gate만 요약한다. |
| Completed decisions/archive | `Assets/00_ProjectRaidAuthority/Docs/02_Complete/**` | CCGS ADR/production 상태 파일의 후속 링크 | Complete 이동 시 완료일/결정 요약/후속 문서/폐기 또는 대체 여부를 반드시 남긴다. |

## 4. CCGS-compatible Planning Scaffold Checkpoint

`$ccgs-adopt` 관점에서 현재 프로젝트는 Unity 프로젝트와 Asset Docs는 존재하지만 CCGS 표준 planning artifact가 부족하다. 아래 항목은 Week 1에서 최소 본문까지 작성되어야 한다.

- [ ] `production/stage.txt`가 현재 phase를 기록함.
- [ ] `production/stage.txt` 생성 권한/방법: PM/개발총괄이 `production/` 경로에 수동 생성하고, 초기 값은 `definition-adoption` 또는 동등한 Phase 0 명칭으로 기록함.
- [ ] `.claude/docs/technical-preferences.md`가 Unity/C#/FishNet 기준으로 설정됨.
- [ ] `design/gdd/game-concept.md`가 Product North Star를 반영함.
- [ ] `design/gdd/systems-index.md`가 MVP 시스템 목록과 status를 포함함.
- [ ] 핵심 GDD 파일이 `design/gdd/*.md`에 작성됨.
- [ ] `docs/architecture/adr-*.md` 최소 3개가 생성됨.
- [ ] `docs/architecture/control-manifest.md`가 Accepted ADR 기준으로 생성됨.
- [ ] `docs/architecture/tr-registry.yaml`에 실제 요구사항 ID가 등록됨.
- [ ] `production/sprint-status.yaml`이 현재 sprint/week 상태를 기록함.
- [ ] Asset Docs와 CCGS 표준 artifact의 source-of-truth 규칙이 각 문서에 명시됨.

## 5. Business / PM Axis — Hold Criteria and Validation Milestones

- [ ] 출시 플랫폼 hold criteria: Month 2 종료 전 `PC standalone/Steam demo 우선` 여부를 결정하고, WebGL/모바일/콘솔은 MVP 제외 또는 별도 근거 필요로 표시.
- [ ] 가격/BM hold criteria: MVP에는 BM 구현 없음. Month 4까지 무료 데모/유료 Early Access/포트폴리오 전용 중 하나를 후보로 좁히되, retention 지표와 서버비 추정 전 최종 확정 금지.
- [ ] 서버 운영비 추정: Month 1 Docker headless server 기준으로 동시 인원/인스턴스당 CPU·RAM·트래픽 가정을 작성하고, Month 5에 월 운영비 rough order estimate를 갱신.
- [ ] 포트폴리오/외부 데모 criteria: 외부 시연은 10~15분 한 판 완주, 서버 권위 전투/루팅/탈출, RaidEvent 로그, Docker 실행 증거, 3분 설명 영상 또는 문서가 모두 있을 때만 허용.
- [ ] 시장 검증 milestones: Month 0 경쟁/페르소나 기준 확정 → Month 3 내부 플레이 루프 검증 → Month 4 반복 선택 재미 검증 → Month 5 소규모 외부 피드백 → Month 6 포트폴리오/데모 패키지 검증.

## 6. Phase Roadmap and Checklists

### Phase 0 — Definition + Adoption Readiness

목표: 구현 착수 전 PM/기획/기술/CCGS scaffold를 실제 실행 가능한 기준으로 만든다.

- [ ] PRD v0.1 작성.
- [ ] MVP 제외 항목 승인.
- [ ] 사업/BM hold 항목 승인: 출시 플랫폼, 가격/BM 보류 기준, 서버비 추정 방식, 외부 데모 조건.
- [ ] 서버 확정 항목 승인.
- [ ] CCGS scaffold 최소 세트 완료.
- [ ] Phase 1 진입 gate: “서버 코어 구현을 시작해도 되는 상태” 판정.

### Phase 1 — Month 1: Server Core

목표: FishNet dedicated server가 실제로 실행되고, 서버 기준으로 더미 엔티티 동기화가 가능해야 한다.

구조 원칙:

- 한 Unity/FishNet 프로젝트를 유지하되, 게임플레이 코드는 `Lobby`, `Match/Raid`, `Player`, `Skill/Loot` 같은 기능 도메인으로 나눈다.
- 각 도메인은 `Client` / `Server` / `Shared` 책임을 분리한다.
- `Client`는 입력·표시·요청, `Server`는 검증·상태 확정·로그, `Shared`는 enum/DTO/state id만 담당한다.
- 이 원칙은 빌드 분리보다 먼저 책임 경계를 명확히 하기 위한 Phase 1 코드 배치 기준이다.

네트워크 진입 원칙:

- FishNet은 Transport, NetworkObject, SceneManager, ownership을 담당한다.
- Photon식 Lobby/Matchmaking은 FishNet이 제공한다고 가정하지 않고 프로젝트 레벨에서 추상화한다.
- 프로젝트는 `IMatchmakingService` / `MatchRequest` / `MatchAssignment`를 경계로 로비·방 선택·매칭을 분리한다.
- 현재 서버가 하나뿐인 동안은 `SingleServerMatchmaker`가 고정 서버 주소와 개발용 room 정보를 반환한다.
- 이후 Steam Lobby, PlayFab, Nakama, Unity Matchmaker, 직접 백엔드 구현체로 교체한다.

씬/객체 원칙:

- `OfflineBootstrap`은 부트스트랩·역할 판정·매칭 요청 단계다.
- `MatchRoom`은 Match Room 단계다.
- `Gameplay`는 실제 레이드/게임플레이 단계다.
- `RoomPlayer`는 FishNet 내장 Lobby가 아니라 Match Room의 per-connection 임시 `NetworkObject`로 유지한다.
- `RoomPlayer`는 Ready/선택/로드 상태까지만 담당한다.
- 전투·HP·루팅·탈출·영속 상태는 `GamePlayer` 또는 서버 도메인 서비스가 확정한다.

- [ ] FishNet dedicated server 실행 기준 확정.
- [ ] 서버 Tick 20~30Hz 후보값 결정.
- [ ] Snapshot 10~20Hz 후보값 결정.
- [ ] Entity/Player/Raid/RaidEvent 최소 데이터 모델 확정.
- [ ] 도메인별 `Client` / `Server` / `Shared` 코드 배치 기준 확정.
- [ ] `IMatchmakingService`, `MatchRequest`, `MatchAssignment`, `SingleServerMatchmaker` 최소 인터페이스 설계.
- [ ] `OfflineBootstrap → MatchAssignment → MatchRoom → Gameplay` 전환 흐름 검증.
- [ ] `RoomPlayer`와 `GamePlayer`의 생명주기/책임 경계 검증.
- [ ] 클라이언트 1~2개 접속 테스트 완료.
- [ ] 더미 엔티티 서버 Tick 처리 완료.
- [ ] 서버 기준 위치 동기화 검증 완료.
- [ ] 반복 Play/Stop 시 활성 `AudioListener`가 1개만 유지되는지 검증.
- [ ] Phase 1 gate: 클라이언트가 서버에 접속하고 더미/플레이어 엔티티가 서버 기준으로 동기화됨.

### Phase 2 — Month 2: Combat Core

목표: 클라이언트가 아니라 서버가 이동/발사/히트/피해/HP를 확정한다.

- [ ] 서버 기준 이동 검증.
- [ ] 서버 기준 발사/히트 판정.
- [ ] 서버 기준 피해량/HP 감소.
- [ ] 기본 AI 탐지/추적/공격.
- [ ] 상태이상 Tick 처리.
- [ ] Phase 2 gate: 서버 기준 전투가 성립함.

### Phase 3 — Month 3: Inventory + Raid Flow

목표: 아이템 소유권, 루팅 중복 방지, 탈출 결과 커밋이 서버 기준으로 성립한다.

- [ ] ItemInstance 소유권 모델 구현 기준 확정.
- [ ] LootRequest `requestId` 멱등 처리 기준 확정.
- [ ] 동시 루팅 중복 0건 테스트 기준 확정.
- [ ] 탈출 성공/실패 판정 기준 확정.
- [ ] 성공 탈출만 결과 저장.
- [ ] Phase 3 gate: 한 판 완주 가능.

### Phase 4 — Month 4: Roguelike Layer

목표: 한 판마다 선택과 빌드 변화가 체감되고, 선택 적용은 서버에서 확정된다.

- [ ] TraitState 모델 확정.
- [ ] 3지선다 선택 흐름 확정.
- [ ] 특성 8개 적용.
- [ ] 시너지 4개 적용.
- [ ] 저주 3개 적용.
- [ ] Phase 4 gate: 런마다 선택 변화가 체감됨.

### Phase 5 — Month 5: Retention + Operations

목표: 반복 플레이 동기와 운영 증거가 MVP 수준으로 확인된다.

- [ ] 메타 해금 v1.
- [ ] 고위험 탈출 v1.
- [ ] append-only RaidEvent 운영 로그.
- [ ] Docker 실행.
- [ ] 멀티 인스턴스 운영 초안.
- [ ] 서버 운영비 rough order estimate 갱신.
- [ ] Phase 5 gate: 반복 플레이 구조와 운영 구조가 확인됨.

### Phase 6 — Month 6: Stabilization + Portfolio

목표: 외부 시연 가능한 품질과 설명 가능한 기술 증거를 갖춘다.

- [ ] QA 체크리스트 완료.
- [ ] 서버/클라 안정화.
- [ ] 성능/스냅샷 부하 점검.
- [ ] 외부 시연 빌드.
- [ ] 포트폴리오/외부 데모 기준 충족 여부 판정.
- [ ] 포트폴리오 문서/영상/아키텍처 설명.
- [ ] Phase 6 gate: 외부 시연 가능한 MVP.

## 7. Weekly PM Operating Rhythm

- [ ] 월요일: 이번 주 목표 1개 확정.
- [ ] 화~수요일: 핵심 산출물/기능 진행.
- [ ] 목요일: 통합, 예외, 로그, 문서 정리.
- [ ] 금요일: 플레이/검증/회고/다음 주 우선순위 조정.
- [ ] 매주 서버 권위 위반 여부 점검.
- [ ] 매주 MVP 필수 범위 이탈 여부 점검.
- [ ] 매주 CCGS artifact와 Asset Docs의 source-of-truth 충돌 여부 점검.

## 8. Risk Register

| Risk | Impact | Mitigation | Checklist |
|---|---|---|---|
| Scope creep | 6개월 MVP 지연 | MVP 제외 항목을 phase gate에서 재확인 | - [ ] 매 phase gate마다 제외 항목 재확인 |
| Server/client responsibility drift | 서버 권위 가치 약화 | 서버 확정 항목 표를 control-manifest로 고정 | - [ ] client-only 확정 로직 금지 규칙화 |
| Loot duplication | 경제/레이드 결과 신뢰도 붕괴 | item lock + requestId + event log를 Phase 3 gate로 고정 | - [ ] 동시 루팅 중복 0건 테스트 |
| Performance risk | 서버 비용/동기화 품질 악화 | Tick/Snapshot/AOI 후보값을 Phase 1~2에서 검증 | - [ ] Tick/Snapshot 테스트 결과 기록 |
| CCGS workflow mismatch | gate/story/ADR 흐름 오판 | Week 1 adoption checkpoint 완료 전 story/sprint 구현 착수 금지 | - [ ] 최소 scaffold 완료 확인 |
| FishNet Lobby/Room 용어 혼동 | Photon식 Lobby와 프로젝트 Match Room을 혼동해 책임이 섞임 | `IMatchmakingService`는 로비/매칭 경계, `RoomPlayer`는 Match Room 임시 객체로 문서와 코드에서 분리 | - [ ] Phase 1에서 SingleServerMatchmaker와 RoomPlayer/GamePlayer 경계 검증 |
| BM/출시 판단 방치 | MVP 이후 외부 데모/운영비/가격 판단 지연 | Business/PM Axis를 milestone gate로 관리 | - [ ] Month 2/4/5 BM hold 검토 |

## 9. Acceptance Criteria

- [ ] 문서 상단에 `상태: Plan`이 명시되어 있다.
- [ ] 6개월 MVP 전체 흐름이 phase별 checklist로 되어 있다.
- [ ] 각 phase에 통과 조건이 있다.
- [ ] 서버 권위 검증 항목이 phase gate에 포함되어 있다.
- [ ] CCGS adoption checkpoint가 별도 섹션으로 포함되어 있다.
- [ ] Plan/InProgress/Complete lifecycle 이동 조건이 포함되어 있다.
- [ ] MVP 제외 항목이 명확히 포함되어 scope creep 방지 기준으로 사용 가능하다.
- [ ] Week 1 plan과 연결되는 다음 액션이 있다.
- [ ] Source-of-Truth Mapping Table이 Asset Docs와 CCGS 표준 경로를 연결한다.
- [ ] Business/PM Axis가 출시 플랫폼, 가격/BM hold, 서버 운영비, 포트폴리오/외부 데모, 시장 검증 milestones를 포함한다.
- [ ] Phase 1이 `IMatchmakingService`/`SingleServerMatchmaker` 기반의 로비·매칭 추상화와 `RoomPlayer`/`GamePlayer` 책임 경계를 포함한다.

## 10. 다음 실행 연결

이 문서가 채택되면 다음 문서가 현재 실행 기준이 된다.

- Week 1 plan: `../../01_InProgress/00_WeeklyProgress/00_week_01_plan.md`

Week 1 종료 후에는 다음 둘 중 하나로 이동한다.

- 완료/승인: `Assets/00_ProjectRaidAuthority/Docs/02_Complete/00_Accepted/`
- 대체/폐기: `Assets/00_ProjectRaidAuthority/Docs/02_Complete/01_Rejected/`
