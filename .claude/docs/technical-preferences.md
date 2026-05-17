# 기술 선호와 프로젝트 기준

이 문서는 `Assets/00_ProjectRaidAuthority/Docs/00_Plan`의 기존 기획 문서를 CCGS 구조로 옮기며 설정한 현재 기술 기준이다. 모든 새 문서와 주석은 한국어로 작성한다.

## 엔진과 언어

- **엔진**: Unity 6000.4.5f1
- **언어**: C#
- **렌더링**: Universal Render Pipeline 17.4.0
- **물리**: 탑다운 플레이에 필요한 Unity 물리 사용. 서버 권한 게임 상태는 클라이언트 물리 결과를 신뢰하지 않는다.
- **네트워킹**: FishNet authoritative dedicated server. 클라이언트는 입력과 표시만 담당한다.

## 입력과 플랫폼

- **우선 플랫폼**: PC standalone / Steam 데모
- **입력 방식**: 키보드/마우스 우선, 게임패드는 핵심 루프 검증 후 선택
- **모바일/콘솔/WebGL**: MVP에서는 제외
- **운영 목표**: 1인 개발 6개월 포트폴리오/데모. Linux dedicated server 빌드와 Docker 실행 증거가 기술 검증의 일부다.

## 이름 규칙

- **클래스**: PascalCase, 가능하면 공개 타입 하나당 파일 하나
- **변수**: 지역 변수와 필드는 camelCase. 비공개 직렬화 필드는 모듈 안에서 일관되면 `_camelCase` 허용
- **이벤트/신호**: PascalCase. 서버 이벤트는 `DamageApplied`, `LootCommitted`, `RaidStarted`처럼 과거형 도메인명 사용
- **파일**: C# 스크립트는 주요 타입명과 일치. Markdown 산출물은 kebab-case
- **Scene/Prefab**: `Assets/00_ProjectRaidAuthority/` 아래 PascalCase 그룹 사용

## 성능 기준 후보

- **클라이언트 목표**: 60 FPS
- **클라이언트 프레임 예산**: 16.6ms
- **서버 tick 후보**: 20–30Hz
- **snapshot 후보**: 10–20Hz
- **AOI 반경 후보**: 25–45m 상당
- **네트워크 원칙**: 필요 없는 엔티티를 보내지 않고, AOI/interest management를 조기에 검토한다.

## 테스트 기준

- **프레임워크**: Unity Test Framework 1.6.0
- **필수 검증**: 서버 권한 이동, 발사, 피격, 피해, 상태이상, 전리품 멱등성, 추출 결과 확정, 특성 선택 권한, raid event log
- **커버리지 숫자 게이트**: 아직 없음. 대신 서버 권한 불변식마다 자동 또는 스크립트 검증 하나 이상이 필요하다.

## 금지 패턴

- 클라이언트가 전투, 인벤토리, 추출, 특성, 프로필, 메타 진행 상태를 직접 확정하는 구조
- 클라이언트가 보고한 피격, 아이템 소유, 추출 성공, 특성 선택을 서버 검증 없이 신뢰하는 구조
- 요청 ID 또는 동등한 재처리 방지가 없는 전리품/결과 요청
- 서버 핵심, 전투 권한, 전리품 무결성, 한 번의 완전한 raid 검증 전 콘텐츠 범위를 넓히는 것
- 진행 중인 생산 스토리를 삭제 재생성하는 것. 필요한 경우 additive retrofit을 사용한다.

## 허용 라이브러리와 패키지

- Unity Input System 1.19.0
- Unity Multiplayer Play Mode 2.0.2
- Newtonsoft Json 3.2.2
- Universal Render Pipeline 17.4.0
- FishNet (`Assets/10_FishNet`)

## 아키텍처 결정 로그

- `docs/architecture/adr-0001-unity-fishnet-authoritative-server.md` — Unity + FishNet 서버 권한 기반
- `docs/architecture/adr-0002-server-owned-raid-state.md` — 서버 소유 raid 상태와 snapshot/event 흐름
- `docs/architecture/adr-0003-authoritative-loot-transaction.md` — 멱등적 서버 권한 전리품 거래

## 엔진 전문가 라우팅

- **주 담당**: Unity gameplay/networking specialist
- **언어/테스트**: C# / Unity Test Framework specialist
- **셰이더**: URP / Shader Graph specialist
- **UI**: Unity UI Toolkit 또는 uGUI specialist. 최종 선택은 UX 검증 후 결정
- **추가 전문가**: FishNet networking, dedicated server/Docker operations, QA automation
- **라우팅 메모**: 서버 권한 게임플레이 변경은 스토리 완료 전 아키텍처/QA 리뷰가 필요하다.

## 파일 유형별 담당

| 파일 유형 | 담당 |
|---|---|
| 게임 코드 | C# / Unity gameplay-networking specialist |
| 셰이더/머티리얼 | URP / Shader Graph specialist |
| UI/화면 | Unity UI specialist |
| Scene/Prefab/Level | Unity scene/prefab specialist |
| Native extension/plugin | C# / Unity package specialist |
| 일반 아키텍처 리뷰 | Unity technical director |
