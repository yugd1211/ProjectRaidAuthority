# 아키텍처 리뷰 — 2026-05-13

## 판정

**조건부 통과.** 문서상 아키텍처는 Technical Setup과 프로토타입 준비에 충분하다. 실제 구현 story 완료 판정에는 Unity/FishNet smoke prototype 증거가 필요하다.

## 확인한 항목

- Unity + FishNet 서버 권한 방향
- 서버 소유 raid state
- 멱등 전리품 transaction
- 통제 매니페스트
- GDD와 ADR 추적성

## 승인 근거

- 클라이언트/서버 권한 경계가 명확하다.
- loot/extraction/result commit이 분리되어 있다.
- 프로토타입 전 완료 처리 금지 규칙이 문서화되어 있다.

## 남은 우려

- FishNet API 사용 방식은 아직 실제 코드로 검증되지 않았다.
- tick/snapshot/AOI 수치는 후보값이다.
- dedicated server/Docker 절차는 구현 후 업데이트해야 한다.

## 다음 필수 작업

FishNet 서버 권한 스모크 프로토타입을 만들고, 검증 로그를 남긴 뒤 epic/story를 생성한다.
