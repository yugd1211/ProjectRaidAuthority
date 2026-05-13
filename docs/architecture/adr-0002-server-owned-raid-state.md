# ADR-0002: 서버 소유 Raid 상태

## 상태

승인 — 단, 실제 API/수치 검증은 FishNet authority smoke prototype 이후 확정한다.

## 배경

ProjectRaidAuthority는 서버 권한 추출 로그라이트다. 전투, 전리품, 추출, 특성, 결과 저장은 조작 가능성이 낮고 재현 가능한 서버 결과여야 한다. 1인 개발 6개월 MVP이므로 복잡한 범용 아키텍처보다 작고 검증 가능한 권한 경계를 우선한다.

## 결정

Raid phase, timer, entity state, extraction state, event order는 서버가 소유한다.

## 결과와 영향

### 긍정적 영향

- 핵심 게임 결과가 서버 로그와 QA 증거로 검증된다.
- 클라이언트 조작으로 전투/loot/추출 결과를 확정하기 어렵다.
- 구현 스토리가 명확한 권한 경계를 가진다.

### 비용과 위험

- 전용 서버 실행, 로컬 다중 클라이언트 테스트, FishNet API 학습 비용이 있다.
- latency와 예측/보간 처리가 필요하다.
- 프로토타입 전까지 tick/snapshot 수치는 후보에 머문다.

## 거절한 대안

클라이언트가 raid state를 직접 확정하면 추출/사망/전리품 결과가 서로 어긋날 수 있다.

## ADR 의존성

- `docs/architecture/adr-0001-unity-fishnet-authoritative-server.md`
- `docs/architecture/adr-0002-server-owned-raid-state.md`
- `docs/architecture/adr-0003-authoritative-loot-transaction.md`

## 엔진 호환성

- Unity 6000.4.5f1
- URP 17.4.0
- FishNet under `Assets/10_FishNet`
- Unity Test Framework 1.6.0

## 연결된 GDD 요구사항

- `design/gdd/authoritative-server-core.md`
- `design/gdd/combat-ai-core.md`
- `design/gdd/inventory-looting-system.md`
- `design/gdd/raid-flow-system.md`

## 검증 기준

- [ ] headless server가 실행된다.
- [ ] 최소 두 클라이언트가 접속한다.
- [ ] 클라이언트 직접 state write가 거절된다.
- [ ] 관련 서버 이벤트가 로그에 남는다.
- [ ] smoke prototype 결과가 문서에 반영된다.
