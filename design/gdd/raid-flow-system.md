# Raid 흐름과 추출

> **상태**: 아키텍처 계획 기준 승인  
> **작성 기준일**: 2026-05-13  
> **구현 축**: 한 판의 위험-보상 루프  
> **주의**: 실제 수치는 Unity/FishNet smoke prototype 측정 전까지 후보값이다.

## 개요

준비, 진입, 탐색, 전투, 추출, 결과 확정까지 한 번의 raid 생명주기를 서버 상태 기계로 관리한다.

## 플레이어 판타지

플레이어는 언제 더 욕심낼지, 언제 빠져나갈지 긴장감 있게 판단해야 한다.

## 상세 규칙

1. 서버가 raid phase와 timer를 소유한다.
2. 추출 구역 진입과 유지 시간은 서버가 판정한다.
3. 성공 추출만 전리품과 결과를 profile/stash에 commit한다.
4. 사망/시간초과/연결 종료 정책은 MVP에서 명확히 하나로 정한다.
5. 결과 화면은 서버 결과 이벤트만 표시한다.

## 수식

```text
raid_time_remaining = raid_time_limit - elapsed_server_time
extract_progress = time_inside_extract_zone / required_extract_hold_time
```

수식은 구현 전 검증용 기준이다. 최종 타입과 직렬화 방식은 Unity/FishNet API 확인 후 결정한다.

## 예외 상황

- 추출 직전 사망하면 서버 event order가 결과를 결정한다.
- 연결 끊김은 MVP에서 실패 처리하거나 별도 reconnect 정책을 명시한다.
- 서버 timer가 drift하면 서버 시간을 우선한다.

## 의존성

- 서버 권한 핵심
- 전투와 AI
- 인벤토리와 전리품
- 결과 화면

## 조정값

- raid_time_limit
- extract_hold_time
- extract_zone_count
- disconnect_policy

## 수용 기준

- [ ] 서버 phase 전환이 로그로 검증된다.
- [ ] 추출 성공/실패 결과가 서버에서만 확정된다.
- [ ] 성공 추출만 loot commit을 호출한다.
- [ ] 결과 화면 입력 데이터가 서버 이벤트와 일치한다.
