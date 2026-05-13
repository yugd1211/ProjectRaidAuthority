# 서버 권한 핵심

> **상태**: 아키텍처 계획 기준 승인  
> **작성 기준일**: 2026-05-13  
> **구현 축**: 서버 진실 우선  
> **주의**: 실제 수치는 Unity/FishNet smoke prototype 측정 전까지 후보값이다.

## 개요

전용 서버가 월드 상태, 엔티티 식별자, tick 시뮬레이션, 검증된 클라이언트 입력, snapshot/event를 소유하는 기반 시스템이다.

## 플레이어 판타지

플레이어는 피격, 전리품, 추출, 결과가 공정하고 조작되지 않았다고 느껴야 한다.

## 상세 규칙

1. 서버가 최종 위치, 회전, HP, 상태, 아이템 소유, 특성, 추출, 결과 저장을 소유한다.
2. 클라이언트는 `ClientInput(tick, seq, moveVec, aimDir, fire, reload, interact)`만 보낸다.
3. 서버 tick 후보는 20–30Hz, snapshot 후보는 10–20Hz로 시작하고 프로토타입에서 측정한다.
4. 모든 네트워크 엔티티는 서버가 발급한 안정적인 `EntityId`를 가진다.
5. AOI/interest management는 초기부터 고려한다.

## 수식

```text
tick_duration_seconds = 1 / server_tick_rate
snapshot_interval_seconds = 1 / snapshot_rate
```

수식은 구현 전 검증용 기준이다. 최종 타입과 직렬화 방식은 Unity/FishNet API 확인 후 결정한다.

## 예외 상황

- 클라이언트가 불가능한 속도를 보내면 clamp/reject 후 로그를 남긴다.
- 중복 입력 sequence는 무시하거나 멱등 처리한다.
- snapshot payload가 커지면 콘텐츠 추가보다 AOI와 엔티티 수를 먼저 줄인다.

## 의존성

- Unity/FishNet
- 전투와 AI 핵심
- 인벤토리와 전리품
- Raid 흐름과 추출

## 조정값

- server_tick_rate: 20–30Hz 후보
- snapshot_rate: 10–20Hz 후보
- aoi_radius: 25–45m 후보
- lag_compensation_window: 0–150ms 후보

## 수용 기준

- [ ] Headless dedicated server가 켜지고 최소 2개 클라이언트를 받는다.
- [ ] 서버 소유 더미 엔티티가 서버 tick으로 이동하고 클라이언트에 보인다.
- [ ] 클라이언트가 위치/HP/상태를 직접 확정할 수 없다.
- [ ] tick/snapshot rate가 smoke test 로그에 남는다.
