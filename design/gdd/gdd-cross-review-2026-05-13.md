# GDD 교차 리뷰 — 2026-05-13

## 판정

**조건부 통과.** MVP 핵심 GDD 6개는 Pre-Production 아키텍처 계획 기준으로 충분하다. 실제 구현 스토리로 넘기기 전에는 Unity/FishNet smoke prototype 검증이 필요하다.

## 확인한 문서

- `authoritative-server-core.md`
- `combat-ai-core.md`
- `inventory-looting-system.md`
- `raid-flow-system.md`
- `roguelike-run-build-system.md`
- `retention-operations-system.md`
- `systems-index.md`

## 강점

- 서버 권한 원칙이 모든 핵심 시스템에 반복 적용되어 있다.
- 전리품, 추출, 결과 저장의 commit boundary가 분리되어 있다.
- 1인 개발 MVP 범위가 명확하다.
- 아트 방향이 “러프하지만 의도된” 제작 방식과 맞는다.

## 우려

- 수치와 tick/snapshot 값은 아직 후보이며 프로토타입 측정이 필요하다.
- UI stack과 FishNet serialization/RPC 제약은 구현 전 확인해야 한다.
- 오디오/피드백 문서는 아직 없다.

## 권장 다음 작업

FishNet authority smoke prototype을 먼저 만들고, 그 검증 결과를 기준으로 epic/story를 생성한다.
