# 시스템 인덱스

> **상태**: Pre-Production / Prototype Readiness 기준  
> **마지막 갱신**: 2026-05-14  
> **언어 규칙**: 새 문서와 주석은 한국어로 작성한다.

## 시스템 목록

| 번호 | 시스템 | 범주 | 우선순위 | 상태 | 문서 | 의존성 |
|---:|---|---|---|---|---|---|
| 1 | 서버 권한 핵심 | 기반 | MVP | 승인 | `design/gdd/authoritative-server-core.md` | Unity, FishNet |
| 2 | 전투와 AI 핵심 | 핵심 | MVP | 승인 | `design/gdd/combat-ai-core.md` | 서버 권한 핵심 |
| 3 | 인벤토리와 전리품 | 핵심 | MVP | 승인 | `design/gdd/inventory-looting-system.md` | 서버 권한 핵심, Raid 흐름 |
| 4 | Raid 흐름과 추출 | 핵심 | MVP | 승인 | `design/gdd/raid-flow-system.md` | 서버 권한 핵심, 전투, 전리품 |
| 5 | Roguelike Run Build | 기능 | Vertical Slice | 승인 | `design/gdd/roguelike-run-build-system.md` | 전투, 전리품, Raid 흐름 |
| 6 | 유지/운영 증거 | 운영 | MVP | 승인 | `design/gdd/retention-operations-system.md` | 서버 로그, 프로토타입 |
| 7 | HUD / 결과 UI | 표현 | Vertical Slice | 설계 중 | `design/ux/hud.md`, `design/ux/raid-result-screen.md` | 서버 snapshot/event |
| 8 | 오디오/피드백 | 표현 | Vertical Slice | 미시작 | — | 전투, Raid 흐름 |
| 9 | 접근성 기준 | 품질 | MVP | 설계 중 | `design/accessibility-requirements.md` | HUD, 상호작용 |

## 설계 순서

1. 서버 권한 핵심
2. Raid 흐름과 추출
3. 인벤토리와 전리품
4. 전투와 AI 핵심
5. Roguelike Run Build
6. HUD/결과 화면
7. 운영/QA 증거

## 고위험 항목

| 위험 | 영향 | 대응 |
|---|---|---|
| FishNet API 사용 방식 미검증 | 구현 지연, 잘못된 동기화 구조 | smoke prototype에서 먼저 검증 |
| 서버 권한 범위 과대 | 1인 개발 범위 초과 | MVP는 핵심 불변식만 자동 검증 |
| 전리품 중복/소실 | 게임 신뢰도 붕괴 | RequestId 기반 멱등 transaction 사용 |
| 추출 결과 순서 모호 | 결과 저장 버그 | 서버 event order와 commit boundary 명시 |
| 아트 품질 과투자 | 구현 시간 부족 | Broken Toy Raid Punk로 러프 자산을 의도화 |

## 진행 현황

| 항목 | 수량/상태 |
|---|---|
| 전체 추적 시스템 | 9개 |
| MVP 핵심 GDD | 6개 승인 |
| UX 문서 | 4개 작성 |
| 접근성 기준 | 작성됨 |
| 아트 바이블 | 작성됨 |
| ADR | 3개 승인 |
| 실제 프로토타입 | 아직 미구현 |

## 다음 단계

1. 실제 Unity/FishNet authority smoke prototype을 구현한다.
2. 서버가 입력 검증, 더미 엔티티, 피해/loot/추출 중 최소 핵심 흐름을 소유하는지 확인한다.
3. 검증이 끝난 뒤에만 `prototypes/fishnet-authority-smoke/README.md`를 만든다.
4. 그 다음 epic/story/sprint plan을 생성한다.
