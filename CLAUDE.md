# Claude Code Game Studios — ProjectRaidAuthority 운영 지침

ProjectRaidAuthority는 Unity 기반 탑다운 PvE 추출 로그라이트 MVP다. 이 문서는 Claude-native 게임 스튜디오 워크플로에서 프로젝트 전체 기준으로 읽힌다.

## 최우선 언어 규칙

- 모든 대화, 설계 문서, Markdown 주석, 진행 기록은 한국어로 작성한다.
- Unity, FishNet, C#, URP, API명, 파일 경로, 클래스명, 명령어, 로그 원문은 고유 식별자로 보고 원문을 유지할 수 있다.
- 사용자가 별도로 요청하지 않는 한 새 문서를 영어로 만들지 않는다.

## 기술 스택

- **엔진**: Unity 6000.4.5f1
- **언어**: C#
- **렌더링**: Universal Render Pipeline 17.4.0
- **네트워킹**: FishNet authoritative dedicated server
- **버전 관리**: Git, 작은 단위 변경과 명확한 검증 기록
- **자산 방향**: Synty 계열 저폴리 키트배싱, Broken Toy Raid Punk 아트 방향

## 프로젝트 구조

- `Assets/00_ProjectRaidAuthority/` — Unity 프로젝트 자산과 프로젝트 문서
- `design/` — GDD, UX, 아트, 접근성 설계
- `docs/architecture/` — ADR, 아키텍처, 통제 매니페스트, 추적성
- `docs/engine-reference/unity/` — 현재 Unity 기준 메모
- `production/` — 현재 단계, 세션 상태, 스프린트 상태, 프로토타입 계획

## 엔진 기준

현재 엔진 기준은 다음 파일을 우선한다.

- `.claude/docs/technical-preferences.md`
- `docs/engine-reference/unity/VERSION.md`

## 협업 원칙

- 사용자가 명확한 작업을 요청하면 안전하고 되돌릴 수 있는 범위에서는 바로 진행한다.
- 파괴적 변경, 외부 배포, 인증/결제/프로덕션 접근, 큰 범위 변경은 확인한다.
- 완료를 주장하기 전에 실제 파일과 검증 결과를 확인한다.
- 프로토타입은 실제 Unity/FishNet 동작 검증 전까지 완료 처리하지 않는다.

## 설계/구현 게이트

1. Concept / Definition 산출물 정리
2. Pre-Production 설계와 아키텍처 기준 승인
3. FishNet 서버 권한 스모크 프로토타입 실제 구현
4. 프로토타입 검증 증거 생성
5. Epic / Story / Sprint 계획 생성
6. 구현과 QA 루프

현재 상태는 2단계 완료, 3단계 진입 전이다.

## 코딩 기준

- 서버 권한이 필요한 전투, 인벤토리, 추출, 특성, 결과 저장은 클라이언트가 직접 확정하지 않는다.
- 클라이언트는 입력과 표시를 담당하고, 서버는 검증·시뮬레이션·결과 확정을 담당한다.
- 새 기능은 GDD 요구사항, ADR, 통제 매니페스트, 검증 방법과 연결되어야 한다.
