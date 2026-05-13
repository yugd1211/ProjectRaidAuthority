# 유지/운영과 포트폴리오 증거

> **상태**: 아키텍처 계획 기준 승인  
> **작성 기준일**: 2026-05-13  
> **구현 축**: 검증 가능한 작은 데모  
> **주의**: 실제 수치는 Unity/FishNet smoke prototype 측정 전까지 후보값이다.

## 개요

이벤트 로그, Docker/server 실행 증거, QA 체크, 데모 패키징을 관리해 1인 MVP의 신뢰도를 높이는 시스템이다.

## 플레이어 판타지

플레이어보다 개발자/평가자가 “이 데모는 서버 권한과 운영 증거가 있다”고 확인할 수 있어야 한다.

## 상세 규칙

1. 핵심 raid event는 파일 또는 콘솔 로그로 남긴다.
2. 서버 실행, 클라이언트 접속, 전투, loot, 추출의 smoke test 절차를 유지한다.
3. MVP 범위를 넘는 live-ops 기능은 추적만 하고 구현하지 않는다.
4. 포트폴리오 빌드는 재현 가능한 실행 순서를 가진다.

## 수식

```text
demo_readiness = required_smoke_checks_passed / required_smoke_checks_total
```

수식은 구현 전 검증용 기준이다. 최종 타입과 직렬화 방식은 Unity/FishNet API 확인 후 결정한다.

## 예외 상황

- 로그가 너무 많으면 핵심 event type만 남긴다.
- Docker가 지연되면 headless local server 증거를 먼저 확보한다.
- 분석/수익화는 MVP 이후로 명확히 미룬다.

## 의존성

- 서버 권한 핵심
- Raid 흐름
- 인벤토리와 전리품
- 프로토타입 계획

## 조정값

- log_level
- smoke_check_count
- demo_content_budget
- server_instance_count

## 수용 기준

- [ ] 서버 실행 절차가 문서화된다.
- [ ] 주요 raid event가 로그에 남는다.
- [ ] smoke test 체크리스트가 재현 가능하다.
- [ ] 포트폴리오 데모 범위가 1맵/제한 콘텐츠로 고정된다.
