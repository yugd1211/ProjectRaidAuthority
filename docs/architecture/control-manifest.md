# 통제 매니페스트 — ProjectRaidAuthority

- **매니페스트 버전**: 2026-05-20-unity-fishnet-authority-v3
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
| Architecture | FishNet `NetworkBehaviour`는 단일 adapter가 계약을 소유하고, 계산/정책은 순수 C# collaborator로 분리한다. |
| FishNet Server Guard | 서버 권한으로 상태를 변경하는 `NetworkBehaviour` 메서드에는 `[Server]`를 붙인다. 클라이언트 요청 진입점은 `[ServerRpc]`로 받고, 그 내부에서 거리/소유권/상태/중복 요청을 반드시 검증한다. |

## 금지 규칙

- 클라이언트가 HP, item ownership, extraction success, trait result를 직접 기록하는 것
- 중복 요청 방지 없는 loot/result commit
- 서버 권한 smoke prototype 전 콘텐츠 범위 확장
- 구현 스토리에서 관련 GDD/ADR/통제 매니페스트 참조를 생략하는 것
- Unity/FishNet 계약을 숨긴 채 `partial` 파일 분산으로 읽기 경계를 흐리는 것

## 가드레일

- 새 gameplay story는 최소 하나의 GDD 요구사항과 하나의 ADR 또는 매니페스트 규칙을 참조한다.
- FishNet API 확정 전에는 pseudo contract를 실제 코드 계약처럼 취급하지 않는다.
- 새 `NetworkBehaviour` 설계는 `docs/architecture/adr-0004-networkbehaviour-adapter-composition.md`의 adapter/composition 경계를 먼저 검토한다.
- `[Server]`는 네트워크 메시지를 보내는 기능이 아니라 서버가 아닌 실행 환경을 차단하는 제한자다. 단순 계산/검증 helper까지 강박적으로 붙일 필요는 없지만, HP/loot/inventory/extraction/result처럼 gameplay truth를 바꾸는 서버 함수에는 기본 가드로 사용한다.
- `[ServerRpc]`는 클라이언트가 서버에 요청을 전달하는 입구일 뿐 신뢰 경계가 아니다. 모든 `[ServerRpc]`는 hostile client 입력을 가정하고 서버 데이터 기준으로 검증한 뒤 `[Server]` 처리 함수로 넘긴다.
- 프로토타입 완료 증거 없이 `prototypes/*/README.md`를 만들지 않는다.
