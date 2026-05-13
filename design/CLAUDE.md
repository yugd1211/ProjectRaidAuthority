# design 디렉터리 지침

이 디렉터리의 설계 문서를 작성하거나 수정할 때는 아래 기준을 따른다.

## 공통 언어 규칙

- 설계 문서, 표, 체크리스트, 주석은 한국어로 작성한다.
- Unity, FishNet, API명, 클래스명, 파일 경로, 수식 변수명은 원문을 유지할 수 있다.

## GDD 파일 (`design/gdd/`)

모든 GDD는 아래 8개 필수 항목을 같은 순서로 포함한다.

1. 개요 — 한 문단 요약
2. 플레이어 판타지 — 의도한 감정과 경험
3. 상세 규칙 — 모호하지 않은 메커닉
4. 수식 — 변수와 계산식
5. 예외 상황 — 특수 상황 처리
6. 의존성 — 연결 시스템
7. 조정값 — 설정 가능한 값
8. 수용 기준 — 검증 가능한 완료 조건

파일명은 시스템 슬러그를 사용한다. 예: `combat-ai-core.md`.

`design/gdd/systems-index.md`는 새 GDD가 추가될 때 함께 갱신한다.

## 설계 순서

기반 → 핵심 → 기능 → 표현 → 폴리시 순서로 설계한다.

## UX 문서 (`design/ux/`)

- HUD 설계: `design/ux/hud.md`
- 상호작용 패턴: `design/ux/interaction-patterns.md`
- 결과 화면: `design/ux/raid-result-screen.md`
- 접근성 요구사항: `design/accessibility-requirements.md`

UX 문서는 구현 지시서가 아니라 구현 전 기준선이다. UI 스택은 실제 Unity 프로토타입 검증 후 확정한다.
