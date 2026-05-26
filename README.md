# ProjectRaidAuthority

ProjectRaidAuthority는 Unity + FishNet 기반의 탑다운 PvE 추출 로그라이트 MVP 프로젝트다. 현재 목표는 많은 콘텐츠를 만드는 것이 아니라, 전투·전리품·추출·결과 저장이 서버 권한으로 검증되는 작은 raid loop를 먼저 증명하는 것이다.

## 현재 단계

**사전 제작 / 프로토타입 준비**

- 기존 기획 문서(`Assets/00_ProjectRaidAuthority/Docs/00_Plan`)를 CCGS 구조로 이전했다.
- Unity 6000.4.5f1 + FishNet authoritative dedicated server 방향을 확정했다.
- Broken Toy Raid Punk 아트 방향을 정했다.
- 실제 FishNet 서버 권한 스모크 프로토타입은 아직 구현 전이다.

## 기술 스택

| 항목 | 내용 |
|---|---|
| 엔진 | Unity 6000.4.5f1 |
| 언어 | C# |
| 렌더링 | URP 17.4.0 |
| 네트워킹 | FishNet |
| 테스트 | Unity Test Framework |
| 우선 플랫폼 | PC standalone / Steam 데모 |

## 핵심 원칙

- 클라이언트는 입력과 표시를 담당한다.
- 서버는 게임플레이 진실 상태를 소유한다.
- 전투, 전리품, 추출, 특성, 결과 저장은 서버 검증 없이 확정하지 않는다.
- 프로토타입 검증 전에는 `/prototype 완료`로 표시하지 않는다.
- 새 대화와 Markdown 문서/주석은 한국어로 작성한다.

## 프로젝트 구조

```text
Assets/00_ProjectRaidAuthority/   프로젝트 전용 Unity 자산과 코드
Assets/10_FishNet/                외부 FishNet 플러그인
design/                           GDD, UX, 아트, 접근성 문서
docs/architecture/                ADR, 아키텍처, 통제 매니페스트
production/                       현재 단계, 세션 상태, 프로토타입 계획
```

## 주요 문서

- `design/gdd/game-concept.md` — 게임 콘셉트
- `design/gdd/systems-index.md` — 시스템 목록과 진행 상태
- `design/art/art-bible.md` — Broken Toy Raid Punk 아트 기준
- `docs/architecture/architecture.md` — MVP 아키텍처
- `docs/architecture/control-manifest.md` — 구현 통제 규칙
- `production/prototype-plan-fishnet-authority-smoke.md` — 다음 프로토타입 계획

## 다음 작업

1. FishNet 서버 권한 스모크 프로토타입 구현
2. 서버/클라이언트 분리 실행 검증
3. 서버 소유 entity, 입력 검증, snapshot/event 확인
4. loot 또는 extraction commit 중 하나 이상 검증
5. 검증 후 `prototypes/fishnet-authority-smoke/README.md` 생성
6. 그 다음 epic/story/sprint plan 생성
