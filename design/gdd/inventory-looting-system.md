# 인벤토리와 전리품

> **상태**: 아키텍처 계획 기준 승인  
> **작성 기준일**: 2026-05-13  
> **구현 축**: 서버 권한 전리품 무결성  
> **주의**: 실제 수치는 Unity/FishNet smoke prototype 측정 전까지 후보값이다.

## 개요

아이템 인스턴스, 컨테이너, 플레이어 소유권 변경을 서버 멱등 거래로 처리하는 시스템이다.

## 플레이어 판타지

플레이어는 위험을 감수해 얻은 전리품이 사라지거나 복제되지 않는다고 믿어야 한다.

## 상세 규칙

1. 모든 아이템에는 서버가 발급한 `ItemInstanceId`가 있다.
2. loot 요청은 `RequestId`를 포함하고 멱등 처리된다.
3. 소유권 변경은 서버 transaction으로만 확정된다.
4. 추출 성공 전 raid loot는 임시 상태이며 실패 시 정상 보존되지 않는다.
5. UI는 서버가 확정한 inventory snapshot만 표시한다.

## 수식

```text
loot_weight_total = sum(item.weight for carried_items)
extract_value = sum(item.value for extracted_items)
```

수식은 구현 전 검증용 기준이다. 최종 타입과 직렬화 방식은 Unity/FishNet API 확인 후 결정한다.

## 예외 상황

- 같은 아이템을 두 플레이어가 동시에 요청하면 서버 transaction 순서가 승자를 정한다.
- 중복 요청은 기존 결과를 재전송하고 새 소유권 변경을 만들지 않는다.
- 컨테이너 상태가 바뀌었으면 요청을 거절하고 이유를 보낸다.

## 의존성

- 서버 권한 핵심
- Raid 흐름과 추출
- 결과 화면
- 운영 로그

## 조정값

- inventory_slots
- weight_limit
- loot_rarity_weights
- request_timeout_seconds

## 수용 기준

- [ ] 동시 loot 요청에서 아이템이 복제되지 않는다.
- [ ] 중복 RequestId가 동일 결과를 반환한다.
- [ ] 추출 실패 시 raid loot가 stash에 저장되지 않는다.
- [ ] loot commit/reject 로그가 남는다.
