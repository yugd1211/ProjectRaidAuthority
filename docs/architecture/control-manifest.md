# 통제 매니페스트 — ProjectRaidAuthority

- **매니페스트 버전**: 2026-05-14-unity-fishnet-authority-v1
- **상태**: Pre-Production 기준 승인
- **적용 범위**: 서버 권한 raid loop 구현 전반

## 필수 규칙

| 계층 | 필수 규칙 |
|---|---|
| Client | 입력 전송과 표시/보간만 담당한다. |
| Server Core | 위치, HP, 상태, raid phase, item ownership, trait, result를 확정한다. |
| Combat | hit/damage/status는 서버에서 계산한다. |
| Loot | 소유권 변경은 RequestId 기반 멱등 transaction을 사용한다. |
| Extraction | 성공 추출만 loot/profile commit을 허용한다. |
| UI | 서버 event/snapshot을 표시하고 gameplay truth를 소유하지 않는다. |
| QA | 모든 핵심 event는 재현 가능한 로그나 체크리스트 증거를 남긴다. |

## 금지 규칙

- 클라이언트가 HP, item ownership, extraction success, trait result를 직접 기록하는 것
- 중복 요청 방지 없는 loot/result commit
- 서버 권한 smoke prototype 전 콘텐츠 범위 확장
- 구현 스토리에서 관련 GDD/ADR/통제 매니페스트 참조를 생략하는 것

## 가드레일

- 새 gameplay story는 최소 하나의 GDD 요구사항과 하나의 ADR 또는 매니페스트 규칙을 참조한다.
- FishNet API 확정 전에는 pseudo contract를 실제 코드 계약처럼 취급하지 않는다.
- 프로토타입 완료 증거 없이 `prototypes/*/README.md`를 만들지 않는다.
