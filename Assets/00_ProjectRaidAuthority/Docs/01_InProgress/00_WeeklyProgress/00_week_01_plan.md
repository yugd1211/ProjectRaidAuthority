# Week 01 Execution Plan — Planning Scaffold + MVP Definition

상태: InProgress  
문서 유형: 1주 실행 계획 / PM 운영 문서  
담당: PM/개발총괄 또는 단독 프로젝트 오너  
판단 주체: PM/개발총괄. 기술 결정은 Architect/Tech Owner 검토 후 PM이 채택 여부 기록  
주차 목표: CCGS-compatible planning scaffold의 최소 본문을 실제 작성 완료하고, 서버 권위 MVP의 실행 기준을 확정한다.  
시작일: 2026-05-11  
작성 기준: `$ralplan` consensus + `$ccgs-adopt` 개발총괄 관점

## 완료 또는 이동 조건

- [ ] Week 1 체크리스트 완료.
- [x] 미완 항목은 Week 2 backlog로 이관.
- [ ] 회고/결정 요약 작성 후 Complete 또는 다음 weekly plan으로 연결.

Complete closeout required fields:

- 완료일:
- 결정 요약:
- 후속 문서:
- 폐기 또는 대체 여부:

## Source-of-Truth Mapping Table — Asset Docs ↔ CCGS Standard Paths

| 정보 축 | Week 1에서 할 일 | Canonical 위치 |
|---|---|---|
| 제품 방향 / core loop | Asset Docs를 읽고 CCGS `game-concept.md` 최소 본문으로 요약 | 원문: Asset Docs, workflow artifact: `design/gdd/game-concept.md` |
| Phase/stage | `production/stage.txt`를 PM/개발총괄 권한으로 수동 생성 | `production/stage.txt` |
| Sprint/week 상태 | 이 문서를 기준으로 sprint-status 요약 생성 | 본문: Asset Docs, 상태 요약: `production/sprint-status.yaml` |
| 기술 결정 | 서버 권위 방향과 FishNet 네트워크 진입 흐름을 ADR/control-manifest 후보로 변환 | `docs/architecture/adr-*.md`, `docs/architecture/control-manifest.md`, `docs/architecture/fishnet-network-flow-architecture.html` |
| 요구사항 추적 | 핵심 TR-ID 후보를 등록할지 결정 | `docs/architecture/tr-registry.yaml` |
| 사업/시장 hold | 출시/BM/서버비/외부 데모 기준을 master plan에 반영 | `Assets/.../02_ProductPlan/01_master_development_plan.md` |

## 1. Week 1 Outcome

- [x] Week 1의 단일 목표가 “구현 착수”가 아니라 “개발 착수 가능한 최소 scaffold 실제 작성 완료”로 합의됨.
- [x] Master plan 초안이 `Plan` 상태로 작성됨.
- [x] CCGS-compatible scaffold 최소 세트가 목록 확정에 그치지 않고 최소 본문까지 작성 완료됨.
- [x] Week 2가 서버 데이터 모델/ADR/서버 코어 착수로 넘어갈 수 있는 조건이 명시됨.

## 2. Day-by-Day Checklist

### Day 1 — Baseline Audit + Scope Lock

- [x] 현재 문서 근거 4개(GameDesign/ProductPlan/TechOps/BusinessMarket)를 읽고 product one-liner를 고정.
- [x] MVP 필수 범위 표를 재확인.
- [x] MVP 제외 항목을 “하지 않을 일” 체크리스트로 고정.
- [x] CCGS artifact gap 목록 작성.
- [x] `production/stage.txt` 생성 권한을 PM/개발총괄로 확정하고, 초기 phase 값(`definition-adoption` 권장)과 기록 형식을 결정.
- [x] 출시 플랫폼/가격·BM hold/서버 운영비 추정/외부 데모/시장 검증 milestone 초안 작성.
- [x] Week 1 완료 기준 초안 작성.

### Day 2 — CCGS Planning Scaffold 설계

- [x] `design/gdd/game-concept.md` 최소 본문 작성.
- [x] `design/gdd/systems-index.md` MVP 시스템 목록 최소 본문 작성.
- [x] 핵심 system GDD 후보 선정: Raid Flow, Server Authority, Combat, Loot/Inventory, Extraction, Trait/Synergy/Curse, Meta Progression, Operations/Telemetry.
- [x] ADR 후보 3개 선정 및 각 ADR의 최소 context/decision/consequence 문장 작성.
  - [x] ADR: Unity + FishNet dedicated authoritative server.
  - [x] ADR: Server-authoritative combat/loot/extraction commit boundary.
  - [ ] ADR: RaidEvent append-only log + Docker operations direction.
- [x] control-manifest가 담아야 할 개발 규칙 목록 초안 작성.
- [x] `production/stage.txt`와 `production/sprint-status.yaml` 최소 scaffold 작성.

### Day 3 — Master Plan Draft

- [x] `Assets/00_ProjectRaidAuthority/Docs/00_Plan/02_ProductPlan/01_master_development_plan.md` 목차 작성.
- [x] 6개월 phase gate를 체크리스트화.
- [x] 각 phase에 CCGS adoption checkpoint를 배치.
- [x] Source-of-Truth Mapping Table과 Business/PM Axis를 포함.
- [x] 서버 권위 검증 항목을 각 phase gate에 연결.
- [x] Plan/InProgress/Complete lifecycle 이동 규칙 작성.

### Day 4 — Week Plan Draft + Sprint Tracking

- [x] `Assets/00_ProjectRaidAuthority/Docs/01_InProgress/00_WeeklyProgress/00_week_01_plan.md` 목차 작성.
- [x] Week 1 day-by-day 체크리스트 작성.
- [x] `production/sprint-status.yaml`에 들어갈 필드 후보 정의.
- [x] Week 2 backlog 후보 작성.
- [x] 금요일 검증/회고 양식 작성.

### Day 5 — Review / Gate / Handoff

- [x] CCGS adopt 관점으로 blocking/high/medium/low gap 재분류.
- [x] Master plan acceptance criteria 검토.
- [x] Week 1 acceptance criteria 검토.
- [x] 실제 작성된 최소 scaffold 파일들이 Week 1 exit criteria를 충족하는지 검토.
- [x] Week 2 진입 조건 판정.
- [x] 미완 항목을 Week 2 backlog로 이관.
- [ ] Week 1 회고와 다음 액션 기록.

## 3. Week 1 CCGS Adoption Checkpoint

### BLOCKING / HIGH 우선 후보

- [x] `.claude/docs/technical-preferences.md`의 Engine/Language/Rendering/Physics 설정 필요.
- [x] `production/stage.txt` 생성 필요: PM/개발총괄이 `production/` 디렉터리 아래 수동 생성, 초기 phase와 갱신일 기록.
- [x] `design/gdd/game-concept.md` 최소 본문 작성 필요.
- [x] `design/gdd/systems-index.md` 최소 본문 작성 필요.
- [x] `docs/architecture/adr-*.md` 최소 3개 context/decision/consequence 작성 필요.
- [x] `docs/architecture/control-manifest.md` 최소 규칙 본문 작성 필요.

### MEDIUM / Tracking 후보

- [x] `production/sprint-status.yaml` 생성 필요.
- [x] `docs/architecture/tr-registry.yaml`에 실제 TR-ID 등록 필요.
- [x] `docs/architecture/architecture-traceability.md` 필요 여부 결정.
- [x] Asset Docs와 CCGS 표준 경로 간 중복/참조 규칙 필요.

## 4. Week 1 Exit Criteria

- [x] Master plan draft가 `Plan` 상태로 준비됨.
- [x] Week 1 plan이 `InProgress` 상태로 운영 가능함.
- [x] CCGS-compatible scaffold 생성 목록과 우선순위가 확정되고 최소 본문이 실제 작성됨.
- [x] Business/PM hold criteria가 master plan에 반영됨.
- [x] Week 2에서 서버 구조 설계로 넘어가기 위한 prerequisite가 명시됨.
- [x] 미완 항목이 backlog로 분류됨.

## 5. Week 2 Backlog Seed

- [ ] Entity/Player/Raid/ItemInstance/RaidEvent 데이터 모델 상세화.
- [x] FishNet dedicated server 접속 실험 계획.
- [x] `IMatchmakingService` / `MatchAssignment` / `SingleServerMatchmaker`로 단일 서버 자동 매칭 추상화 구현 계획.
- [x] `OfflineBootstrap → MatchRoom → Gameplay` MPPM 스모크 테스트 계획.
- [x] `RoomPlayer`와 `GamePlayer` 생명주기/책임 경계 검증 계획.
- [ ] Tick/Snapshot/AOI 초기값 결정 ADR 작성.
- [ ] 서버 권한 smoke prototype 완료 후 Prediction/Reconcile 도입 여부 판단.
- [x] 서버 확정 항목을 control-manifest 규칙으로 변환.
- [ ] 첫 번째 server-core story/sprint 후보 작성.
- [x] 출시 플랫폼/BM hold/서버 운영비 추정 assumptions를 Week 2 PM backlog로 유지.

## 6. Acceptance Criteria

- [x] 문서 상단에 `상태: InProgress`가 명시되어 있다.
- [x] Week 1의 단일 목표가 명확하다.
- [x] day-by-day 체크리스트가 있다.
- [x] CCGS adopt 관점의 compatibility checkpoint가 있다.
- [x] Week 1 exit criteria가 체크리스트로 있다.
- [x] Week 1 header에 담당/판단 주체가 있다.
- [x] Week 1 scope가 목록 확인이 아니라 최소 scaffold 실제 작성 완료로 고정되어 있다.
- [x] Week 2 backlog seed가 있다.
- [x] 구현 착수보다 planning scaffold 우선이라는 제한이 명확하다.

## 7. 진행 확인 메모 — 2026-05-18

체크 기준: 파일 존재와 문서 본문으로 확인 가능한 항목만 완료 처리했다. 실제 Unity/FishNet 실행 검증, loot 스모크, 프로토타입 완료 README는 아직 완료 처리하지 않는다.

확인한 완료 근거:

- `production/stage.txt`: `pre-production`
- `production/sprint-status.yaml`: prototype-readiness 상태와 `prototype_built: false` 기록
- `.claude/docs/technical-preferences.md`: Unity 6000.4.5f1, C#, URP 17.4.0, FishNet authoritative dedicated server 기준 기록
- `design/gdd/game-concept.md`, `design/gdd/systems-index.md`, MVP 핵심 GDD 6개 존재
- `docs/architecture/adr-0001..0003`, `docs/architecture/control-manifest.md`, `docs/architecture/tr-registry.yaml`, `docs/architecture/architecture-traceability.md` 존재
- `production/prototype-plan-fishnet-authority-smoke.md`와 `01_fishnet_authority_smoke_remaining_checklist.md`가 다음 미완 작업을 서버 권한 스모크 프로토타입으로 이관

미완으로 유지하는 핵심 항목:

- Week 1 Complete closeout: 완료일/결정 요약/후속 문서/폐기 또는 대체 여부 미작성
- `ADR: RaidEvent append-only log + Docker operations direction`: 동일 제목 ADR은 아직 없음. 운영 증거 요구사항은 GDD/TR/control-manifest에 분산 반영됨
- Tick/Snapshot/AOI 초기값 ADR: 후보값은 있으나 별도 ADR은 아직 없음
- 실제 FishNet 서버 권한 스모크 검증과 `prototypes/fishnet-authority-smoke/README.md` 작성

## 8. 운영 메모

- 이 문서는 `01_InProgress`에 있으므로 현재 주차 실행 기준이다.
- 단순 기능 구현 문서처럼 즉시 Complete로 넘기지 않는다.
- Week 1 종료 시 `완료일 / 결정 요약 / 후속 문서 / 폐기 또는 대체 여부`를 채운 뒤 `02_Complete` 이동 여부를 판단한다.
