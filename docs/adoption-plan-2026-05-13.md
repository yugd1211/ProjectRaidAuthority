# 기존 기획 문서 채택 계획 — 2026-05-13

## 요약

`Assets/00_ProjectRaidAuthority/Docs/00_Plan` 아래 기존 Markdown 기획 문서를 CCGS 구조로 옮겼다. 이전 상태는 Concept / Definition-Adoption에 가까웠고, Ralph 진행 후 현재는 사전 제작 / 프로토타입 준비 상태다.

## 읽은 원본 문서

- `00_Source/00_pm_development_plan.md`
- `00_Source/01_deep_research_report.md`
- `01_GameDesign/00_project_vision_gdd_lite.md`
- `01_GameDesign/01_director_framework.md`
- `02_ProductPlan/00_6month_mvp_roadmap.md`
- `02_ProductPlan/01_master_development_plan.md`
- `03_TechOps/00_authoritative_server_direction.md`
- `04_BusinessMarket/00_market_business_validation.md`
- `README.md`

## 채택한 결정

- Unity 6000.4.5f1 + FishNet authoritative dedicated server를 기준으로 삼는다.
- MVP는 1인 개발 6개월 범위의 탑다운 PvE 추출 로그라이트다.
- 서버는 전투, 전리품, 추출, 특성, 결과 저장의 진실을 소유한다.
- 아트 방향은 Synty 기반 러프 자산을 허용하는 Broken Toy Raid Punk로 정했다.

## 생성/갱신한 산출물

- `.claude/docs/technical-preferences.md`
- `design/gdd/game-concept.md`
- `design/gdd/systems-index.md`
- `design/gdd/*core*.md` 및 MVP 시스템 GDD
- `design/art/art-bible.md`
- `design/ux/*.md`
- `design/accessibility-requirements.md`
- `docs/architecture/*.md`
- `docs/architecture/tr-registry.yaml`
- `production/*.md`, `production/*.yaml`, `production/stage.txt`

## 의도적으로 미룬 것

- 실제 FishNet 서버 권한 스모크 프로토타입 구현
- 프로토타입 완료 README 생성
- epic/story/sprint plan 생성
- 오디오 세부 사양

## 다음 권장 작업

1. Unity에서 FishNet 서버 권한 스모크 프로토타입을 구현한다.
2. 서버 실행, 클라이언트 접속, 서버 소유 상태, 전리품/추출 중 최소 하나의 commit을 검증한다.
3. 검증 후 `prototypes/fishnet-authority-smoke/README.md`를 만든다.
4. 그 다음 epic/story/sprint plan을 생성한다.
