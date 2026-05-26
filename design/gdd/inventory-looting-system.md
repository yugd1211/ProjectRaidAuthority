# 2. 인벤토리와 전리품

> **상태**: 역기획 마이그레이션 반영 / Prototype 수치 후보
> **작성 기준일**: 2026-05-19
> **구현 축**: 서버 권한 전리품 무결성 + 추출 전 임시 소유 판단
> **참고 source material**: `docs/source-material/ccgs/looting_reverse_design_duckov_tarkov.md`
> **주의**: 실제 수치는 Unity/FishNet smoke prototype과 MPPM runtime 검증 후 확정한다.

## 개요

인벤토리와 전리품 시스템은 아이템 인스턴스, 컨테이너, raid 중 임시 운반 상태, 추출 성공 후 stash 확정을 서버 멱등 transaction으로 처리한다. 이 시스템의 핵심은 “아이템을 주웠다”가 아니라 “추출 전까지 위험을 들고 다닌다”는 상태 전이다.

왜: ProjectRaidAuthority는 서버 권한 탑다운 PvE 추출 로그라이트 MVP다. 루팅이 즉시 소유 보상으로 끝나면 추출 판단과 서버 권한 commit boundary가 약해진다. 전리품을 추출 전까지 임시 소유로 두면 짧은 raid 안에서 더 챙길지, 버릴지, 탈출할지 판단이 발생한다.

## 플레이어 판타지

플레이어는 폐품과 장비를 챙기는 순간 보상을 얻었다고 느끼지만, 실제 소유는 아직 확정되지 않았다는 사실을 안다. 좋은 아이템을 들수록 생존 욕구가 커지고, 무게와 슬롯 압박 때문에 다음 선택이 달라진다.

왜: 원본 역기획의 핵심 질문은 “플레이어가 아이템을 주운 뒤 행동이 바뀌는가?”이다. ProjectRaidAuthority에서는 이 행동 변화를 서버가 확정하는 raid state와 연결해야 한다.

## 상세 규칙

1. 모든 lootable item에는 서버가 발급하거나 scene registry로 관리하는 `ItemInstanceId`가 있다.
2. 모든 loot 요청은 `RequestId`를 포함한다.
3. 같은 `RequestId` 중복 요청은 새 소유권 변경을 만들지 않는다.
4. 서버만 item state와 owner를 변경할 수 있다.
5. raid 중 획득한 아이템은 `CarriedRaid` 상태이며 stash 소유가 아니다.
6. 추출 성공 시에만 `ExtractCommitted`를 거쳐 `StashOwned`가 된다.
7. 사망, raid timeout, disconnect fail은 MVP에서 `LostOnFailure`로 처리한다.
8. 클라이언트 UI는 서버 snapshot/event만 표시한다.
9. MVP 아이템 용도는 4종으로 제한한다: 판매용, 제작 재료, 회복/소모품, 장비.
10. 퀘스트 아이템, 수집품, 보험, 거래소, 대형 인벤토리 테트리스는 MVP 필수 범위에서 제외한다.

### 상태 전이

```text
WorldAvailable
  └─ LootRequest(RequestId, ItemInstanceId)
ReservedForRequest
  ├─ validation failed → WorldAvailable + LootRejected
  └─ validation passed → CarriedRaid + LootCommitted
CarriedRaid
  ├─ Drop/Replace → WorldAvailable 또는 DroppedInRaid
  ├─ ExtractSuccess → ExtractCommitted → StashOwned
  └─ Death/Timeout/DisconnectFail → LostOnFailure
```

### 서버 검증 항목

| 검증 | 후보 값/규칙 | 실패 로그 | 왜 필요한가 |
|---|---:|---|---|
| 거리 | `loot_interaction_distance <= 2.5m` | `LootRejected: out_of_range` | 위치 spoofing 차단 |
| 상호작용 시간 | `0.35~0.8초` | `LootRejected: interrupted` | 전투 중 무방비 비용 생성 |
| raid phase | `Active`에서만 허용 | `LootRejected: invalid_phase` | 결과 commit 경계 보호 |
| 중복 요청 | 같은 `RequestId` 멱등 | `Duplicate LootRequest ignored` | 복제/재전송 악용 차단 |
| 이미 획득됨 | `state != WorldAvailable` 거절 | `LootRejected: already_looted` | 동시 획득 복제 차단 |
| 슬롯/무게 | slot/weight limit 이내 | `LootRejected: inventory_full` | 선택 비용 생성 |

왜: 모든 규칙은 “서버가 판정 가능한가”를 기준으로 남겼다. 플레이어 감정용 규칙이라도 서버 로그와 QA evidence로 확인되지 않으면 MVP 필수 규칙으로 보지 않는다.


### 아이템군 설계

| 아이템군 | MVP 예시 | raid 중 판단 | 추출 후 용도 | 기획 의도 |
|---|---|---|---|---|
| 생존 소모품 | 붕대, 응급 주사, 탄약 팩 | 지금 쓰면 생존, 들고 가면 판매/재사용 | 다음 raid 준비 | 현재 생존과 미래 가치 충돌 |
| 판매용 경량품 | 금속 태그, 고장난 렌즈, 희귀 코인 | 가볍게 챙기기 좋음 | stash value 상승 | 추출 욕구 강화 |
| 제작 재료 | 나사, 전선, 폐회로판, 접착제 | 당장은 약하지만 버리기 아까움 | 5~8개 recipe 재료 | 잡템의 장기 가치 부여 |
| 장비/모듈 | 조준기, 방탄판, 작은 가방 | 장착/운반/교체 선택 | 다음 raid 성능 보정 | loot가 다음 run build에 연결 |
| 위험 부채 아이템 | 무거운 엔진 코어, 불안정 배터리 | 무겁고 느려지지만 고가치 | 큰 판매/제작 목표 | 획득 직후 탈출 판단 유도 |

왜: 아이템군은 가격표가 아니라 의사결정 역할로 나눈다. 판매가만 다르면 플레이어는 가장 비싼 것만 줍지만, 즉시 생존/가벼운 가치/장기 제작/장비/무거운 고가치가 섞이면 상황별 선택이 생긴다.

### 루팅 장소 설계

| 위치 유형 | 기본 보상 | 위험 장치 | 반복 방지 장치 |
|---|---|---|---|
| 시작 주변 | 저급 소모품, 가벼운 재료 | 거의 없음 | 가치 상한 낮음 |
| 일반 건물 | 제작 재료, 판매 잡화 | 약한 AI patrol | container seed 변동 |
| 고장난 창고 | 무거운 고가치품 | 느린 이동/퇴로 노출 | 1~2개 후보 spawn |
| 군용 잔해 | 탄약, 방어 장비 | 강한 AI, 소음 유발 | 접근 루트 2개 이상 |
| 탈출구 주변 | 낮은 보상, 정보성 item | 마지막 교전 가능성 | 고가치 loot 배치 금지 |

왜: 루팅 장소는 보상 배치표가 아니라 동선 설계 도구다. 고가치 장소는 플레이어를 위험으로 끌어들이고, 탈출구 주변은 마지막 욕심을 줄이거나 확인 행동을 만들기 위한 곳이어야 한다.

### 플레이어 판단 순간

| 순간 | 조건 | UI 질문 | 기대 행동 |
|---|---|---|---|
| 컨테이너 열기 전 | 적 소리/시야 위험 | “지금 열면 무방비인가?” | 주변 확인 후 열기 |
| inventory 70% | weight warning | “계속 챙길 것인가?” | 경량 고가치 선별 |
| 고가치 획득 | 상위 15% value item | “이제 빠질 것인가?” | 탐색자 → 생존자 전환 |
| 더 좋은 아이템 발견 | slot full | “무엇을 버릴 것인가?” | 가치/무게/목표 비교 |
| 추출 실패 | LostOnFailure | “다음엔 어떤 route를 줄일 것인가?” | 실패를 route 학습으로 전환 |

왜: 이 표는 구현자에게 UI 버튼을 늘리라는 뜻이 아니다. 각 순간에 플레이어가 해야 할 판단이 보이는지 확인하는 설계 체크리스트다.

### 루팅 피드백 규칙

| 피드백 | 조건 | 표현 | 실패 시 문제 |
|---|---|---|---|
| `Raid Carry` 배지 | `CarriedRaid` 상태 | 아이템 카드/인벤토리 상단 | 추출 전 미확정 상태를 모름 |
| 고가치 획득 강조 | item value 상위 15% | 짧은 음향, 테두리 색 | 행동 전환점이 약함 |
| 무게 경고 | `weight_ratio >= 0.70` | HUD 경고, 이동 아이콘 | 과적 리스크를 늦게 이해 |
| 교체 비교 | slot full | 후보/기존 value와 weight 표시 | inventory 스트레스 증가 |
| 손실 요약 | 실패 결과 | `획득했지만 잃은 것` 그룹 | 실패 학습이 안 됨 |

왜: 정보 부족은 루팅 판단을 깊게 만드는 것이 아니라 불쾌하게 만든다. MVP UI는 예쁘기보다 추출 전 미확정, 무게 위험, 고가치 획득, 손실 이유를 빠르게 설명해야 한다.

## 수식

```text
loot_weight_total = sum(item.weight for carried_raid_items)
loot_slot_used = sum(item.slot_size for carried_raid_items)
extract_value = sum(item.value for extract_committed_items)
weight_ratio = loot_weight_total / weight_limit
move_speed_multiplier = 1.0                  if weight_ratio < 0.70
move_speed_multiplier = 0.85                 if 0.70 <= weight_ratio < 1.00
move_speed_multiplier = 0.65                 if weight_ratio >= 1.00
raid_profit_delta = extract_value - loadout_cost_lost
item_decision_score = item.value / max(1, item.weight) + goal_bonus + survival_bonus
risk_debt_score = carried_high_value_count + weight_penalty + distance_to_extract_penalty
```

### 후보 조정값

| 값 | 후보 | 이유 |
|---|---:|---|
| `inventory_slots` | 12~18 | 1인 MVP UI에서 관리 가능한 범위 |
| `weight_limit` | 25~35 units | 경고/감속을 느낄 수 있는 단순 기준 |
| `weight_warning_ratio` | 0.70 | “더 챙기면 위험”을 일찍 알려줌 |
| `loot_interaction_seconds` | 0.35~0.8초 | 전투 중 즉시 줍기 방지 |
| `raid_time_limit` 연동 | 8~12분 | 짧은 추출 루프 유지 |
| `request_timeout_seconds` | 2~5초 | 지연/재전송 처리 후보 |
| `high_value_percentile` | 상위 15% | 고가치 획득 피드백 기준 |
| `starter_safe_loot_count` | 2~4개 | 초반 폐사 완화용 시작 주변 보상 |

왜: 수치는 경제 시뮬레이션이 아니라 smoke prototype 검증용이다. 값보다 중요한 것은 서버가 같은 계산을 재현하고 로그로 남기는 것이다.

## 예외 상황

- 두 플레이어가 같은 아이템을 동시에 요청하면 서버 transaction 순서가 승자를 정한다.
- 이미 처리된 `RequestId`는 새 소유권 변경 없이 기존 결과를 재사용하거나 중복 무시 로그를 남긴다.
- 컨테이너 상태가 바뀌었으면 요청을 거절하고 이유를 보낸다.
- 추출 직전 사망과 추출 성공이 같은 tick에 가까우면 `raid-flow-system`의 서버 event order가 우선한다.
- disconnect는 MVP에서 실패 처리한다. reconnect/보험/우편 회수는 removed scope다.
- PvE 완화용 1회 회수 표식은 optional이다. `LostOnFailure` 검증 전에는 필수로 넣지 않는다.

왜: 루팅 시스템의 신뢰는 예외 상황에서 깨진다. 특히 동시 요청, 중복 요청, 추출 경계, disconnect는 클라이언트 편의보다 서버 판정 일관성을 우선한다.

## 의존성

- `design/gdd/authoritative-server-core.md`: 서버 권한 원칙과 클라이언트 입력 신뢰 금지.
- `design/gdd/raid-flow-system.md`: raid phase, timer, extract commit boundary.
- `design/gdd/combat-ai-core.md`: 전투 중 loot interaction interruption, 위험 지역 배치.
- `design/ux/interaction-patterns.md`: loot interaction prompt, drag/drop, discard/replace UX.
- `design/ux/raid-result-screen.md`: extracted/lost summary 표시.
- `docs/architecture/adr-0003-authoritative-loot-transaction.md`: RequestId 기반 멱등 loot transaction.

왜: 이 문서는 단독 경제 설계가 아니다. 전리품은 raid flow가 성공해야 stash에 들어가며, UI는 서버 결과를 설명해야 한다.

## 조정값

| 조정값 | MVP 기본 | Optional/Future | Removed |
|---|---:|---|---|
| item_count | 20~30 | 50+ | 대량 economy sim |
| container_types | 3 | 잠긴 container 1종 | 복잡한 key economy |
| item_roles | 판매/재료/소모품/장비 | 퀘스트/수집품 | 거래소 투기품 |
| extraction_commit | 성공 추출만 | PvE 회수 표식 | 사망해도 대부분 보존 |
| inventory_shape | 단순 slot + weight | category filter | 대형 테트리스 |
| recovery | 없음 | 사망 지점 1회 회수 | 보험/우편/market recovery |

왜: 조정값 표는 scope cut 장치다. 기획 중 “좋아 보이는 기능”을 바로 MVP에 올리지 않고 optional/removed로 분리한다.

## 수용 기준

- [ ] 동시 loot 요청에서 아이템이 복제되지 않는다.
- [ ] 중복 `RequestId`가 새 소유권 변경을 만들지 않는다.
- [ ] 추출 실패 시 `CarriedRaid` loot가 stash에 저장되지 않는다.
- [ ] 성공 추출 시에만 `ExtractCommitted`가 발생한다.
- [ ] `LootCommitted`, `LootRejected`, `Duplicate LootRequest ignored` 로그가 남는다.
- [ ] 클라이언트가 item owner/state를 직접 확정할 수 없다.
- [ ] `weight_ratio >= 0.70`에서 UI 경고 또는 이동 페널티 후보가 확인된다.
- [ ] result screen이 extracted/lost item summary를 서버 이벤트와 일치하게 표시한다.
- [ ] PvP 경제, 복잡한 시장/거래소, 대형 인벤토리 테트리스, 보험/우편 회수는 MVP removed scope에 남아 있다.

왜: 완료 판정은 “문서상 재미있다”가 아니라 서버 로그와 상태 전이로 검증 가능한가에 둔다.
