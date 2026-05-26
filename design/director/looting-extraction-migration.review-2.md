# Senior Game Director Review 2 — Looting Extraction Migration

> **대상**: `design/director/looting-extraction-migration.md` v2
> **리뷰 기준**: `.codex/skills/senior-game-director/SKILL.md`
> **결과**: APPROVED

## 1. 컨셉 한 줄

APPROVED. 탑다운 PvE 추출 로그라이트와 서버 권한, “위험 부채” 전복이 한 문장에 들어 있다.

## 2. 구조 전복 포인트

APPROVED. 전복은 보상 확정 시점 하나에 집중되어 있고 feature pile이 아니다.

## 3. 핵심 루프

APPROVED. Core/Meta/Retention loop가 8~12분 raid, 12~18 slots, 70% weight warning, 3~5회 session 같은 후보 수치로 제한되어 있다.

## 4. 기술 구조

APPROVED. Unity/FishNet 전제, 서버 권한 RequestId transaction, validation, abuse vectors, rough traffic 가정이 있다.

## 5. 시장/수익 모델

APPROVED. Tarkov, Duckov, ZERO Sievert 비교와 PvP 스트레스는 부담스럽지만 extraction 판단은 원하는 PC/co-op 유저 persona가 명시되어 있다. BM은 Steam paid demo/Early Access로 1인 개발에 맞다.

## 6. 리스크

APPROVED. 기술, 밸런스, 운영, 비용, 감정 리스크가 mitigation과 연결되어 있다.

## 7. MVP 6개월 압축안

APPROVED. required/optional/removed scope가 분리되어 있고, PvP 경제/거래소/대형 인벤토리/보험/live-ops가 removed scope로 고정되어 있다.

## 8. 디렉터 최종 판단

APPROVED. 이 방향의 최대 실패 지점을 scope creep으로 보고, loot transaction → carried raid state → extract commit → result screen 순서로 자른 판단이 타당하다.

## 남은 비차단 주의

- 모든 수치는 smoke prototype 이후 조정해야 한다.
- 실제 플레이 감정 검증은 Unity/MPPM runtime 로그와 20~40분 세션 테스트가 필요하다.
