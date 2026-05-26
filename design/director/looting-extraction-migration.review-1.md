# Senior Game Director Review 1 — Looting Extraction Migration

> **대상**: `design/director/looting-extraction-migration.md` 초안
> **리뷰 기준**: `.codex/skills/senior-game-director/SKILL.md`
> **결과**: NEEDS REVISION → 범위 절단 피드백 반영 필요

## 구조 전복 검토

- 통과: “전리품은 즉시 보상이 아니라 추출 전까지 위험 부채”라는 단일 전복은 명확하다.
- 이유: 기능을 늘리는 대신 소유 확정 시점을 바꾸므로 현재 서버 권한 prototype과 결합된다.

## 차단/수정 피드백

1. **MVP 아이템 용도가 과하다**
   - 판매/제작/퀘스트/장비/수집 5종을 그대로 가져오면 1인 6개월 범위를 넘는다.
   - 반영 지시: MVP 필수는 판매/제작 재료/소모품/장비 4종으로 제한하고, 퀘스트/수집품은 optional 또는 removed로 내려라.

2. **회수/보험이 너무 빨리 들어온다**
   - PvE 완화 장치로 타당하지만, 먼저 실패 규칙과 손실 로그가 검증되어야 한다.
   - 반영 지시: 1회 회수 표식은 optional로 내리고, MVP 필수는 `LostOnFailure` 확정으로 제한하라.

3. **시장/BM이 추상적이다**
   - “Tarkov/Duckov 참고”만으로는 누구에게 팔지 약하다.
   - 반영 지시: target persona와 Steam 유료 데모 → Early Access 경로를 명시하라.

4. **abuse vector를 서버 로그와 연결하라**
   - 클라이언트 spoofing, duplicate request, disconnect 회피가 각각 어떤 로그/거절로 남는지 명시하라.

## 반영 확인 계획

- 본문 v2에서 required/optional/removed scope가 분리되어야 한다.
- 2차 리뷰는 Senior Game Director 8항목 전체와 1인 6개월 MVP 생존성을 다시 확인한다.
