# 아키텍처 추적성

## 목적

GDD, ADR, 통제 매니페스트, 앞으로 생성될 story가 서로 끊기지 않도록 현재 연결 상태를 기록한다.

## 추적 표

| 요구사항 | GDD | ADR | 통제 규칙 | 다음 증거 |
|---|---|---|---|---|
| 서버가 gameplay truth를 소유 | `authoritative-server-core.md` | ADR-0001, ADR-0002 | Server Core 필수 규칙 | FishNet smoke prototype |
| 전투 결과 서버 판정 | `combat-ai-core.md` | ADR-0001, ADR-0002 | Combat 필수 규칙 | hit/damage server log |
| 전리품 소유권 멱등 처리 | `inventory-looting-system.md` | ADR-0003 | Loot 필수 규칙 | duplicate RequestId test |
| 추출 성공만 결과 저장 | `raid-flow-system.md` | ADR-0002, ADR-0003 | Extraction 필수 규칙 | extraction commit log |
| 특성 선택 서버 검증 | `roguelike-run-build-system.md` | ADR-0002 | Server Core 필수 규칙 | trait choice event |
| 운영/포트폴리오 증거 | `retention-operations-system.md` | ADR-0001 | QA 필수 규칙 | smoke checklist |
| FishNet 컴포넌트 계약 가시성 | `authoritative-server-core.md` | ADR-0004 | Architecture 필수 규칙 | adapter/composition 정적 검증 |

## 현재 결론

문서 추적성은 Pre-Production 계획 기준으로 충분하다. 실제 story 추적성은 FishNet authority smoke prototype 이후 생성한다.
