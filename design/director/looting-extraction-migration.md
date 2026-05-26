# 1. 루팅/추출 마이그레이션 기획서 — ProjectRaidAuthority

> **상태**: Director draft v2 / Senior Game Director 재리뷰 기준 통과
> **작성 기준일**: 2026-05-19
> **원본 source material**: `docs/source-material/ccgs/looting_reverse_design_duckov_tarkov.md`
> **적용 대상**: `design/gdd/inventory-looting-system.md`, `design/gdd/raid-flow-system.md`, `docs/architecture/adr-0003-authoritative-loot-transaction.md`
> **우선 제약**: Unity 6000.4.5f1, FishNet authoritative dedicated server, 탑다운 PvE-first, 1인 6개월 MVP

## 1. 컨셉 한 줄

**서버 권한 탑다운 PvE 추출 로그라이트에서, 전리품은 즉시 보상이 아니라 추출 전까지 플레이어에게 붙는 “이동 가능한 위험 부채”다.**

왜: ProjectRaidAuthority의 현재 차별점은 콘텐츠 양이 아니라 서버가 전투·loot·추출 결과를 확정한다는 신뢰다. 루팅을 단순 획득 버튼으로 두면 서버 권한 smoke의 의미가 약해진다. 반대로 전리품을 추출 전까지 확정되지 않은 부채로 만들면, 짧은 raid 안에서 “더 챙길지 / 빠질지” 판단이 매번 발생한다.

## 2. 구조 전복 포인트

### 전복할 핵심 규칙

- 일반 RPG 규칙: `아이템 줍기 → 즉시 소유 → 보상 종료`
- ProjectRaidAuthority 규칙: `아이템 발견 → 서버 임시 점유 → 운반 리스크 증가 → 추출 성공 시 소유 확정`

이 전복은 기능 추가가 아니라 보상 확정 시점의 이동이다. 플레이어는 아이템을 얻는 순간 강해지는 것이 아니라, 그 아이템 때문에 더 느려지고 더 조심하게 된다.

왜: 1인 개발 MVP에서 복잡한 경제/상점/거래소를 만들 여력은 없다. 대신 소유 확정 시점과 서버 상태 전이를 명확히 만들면 적은 콘텐츠로도 반복 판단을 만들 수 있다.

### 상태 전이

```text
WorldAvailable
  └─ 서버 거리/시야/잠금/중복 RequestId 검증 성공
ReservedForRequest
  └─ server loot transaction commit
CarriedRaid
  ├─ ExtractSuccess → ExtractCommitted → StashOwned
  ├─ PlayerDeath / RaidTimeout / DisconnectFail → LostOnFailure
  └─ Drop / Replace → WorldAvailable 또는 DroppedInRaid
```

## 3. 핵심 루프

### Core Loop — 8~12분 raid

| 단계 | 시간 후보 | 서버 확정 데이터 | 플레이어 판단 | 왜 필요한가 |
|---|---:|---|---|---|
| 준비 | 30~90초 | loadout snapshot | 가방/무기/회복템 선택 | 초기 위험량을 플레이어가 직접 정하게 한다 |
| 진입 | 0초 | raid seed, spawn point | 안전 루트/고가치 루트 선택 | 맵 지식이 전투력만큼 가치 있게 만든다 |
| 탐색/전투 | 2~7분 | enemy hit, loot availability | 교전 지속/회피 | loot와 전투를 분리하지 않는다 |
| 선별 | 매 loot 0.35~0.8초 | RequestId, item state, carry state | 지금 들 것/버릴 것 | 아이템을 판단 비용으로 만든다 |
| 추출 판단 | 남은 2~4분 | extract zone, hold timer | 더 돌기/탈출 | 탐욕 제어를 만든다 |
| 결과 확정 | 5~15초 | ExtractCommitted/LostOnFailure | 결과 학습 | 서버 로그 기반 QA와 반복 동기를 만든다 |

### Meta Loop — 작고 검증 가능한 성장

- `ExtractCommitted` 아이템은 stash에 들어간다.
- MVP 아이템 용도는 4종만 둔다: 판매용, 제작 재료, 회복/소모품, 장비.
- 제작은 6개월 MVP에서 5~8개 recipe만 허용한다.
- 은신처/상인 평판/장비 개조는 문서상 future scope로 남기고, vertical slice에는 넣지 않는다.

왜: 원본 역기획서의 장기 성장은 강하지만, 그대로 가져오면 1인 개발 범위를 초과한다. MVP는 “전리품이 다음 raid 준비에 영향을 준다”는 최소 연결만 증명한다.

### Retention Structure — 숫자로 제한한 반복 이유

| 주기 | 목표 | 수치 후보 | 검증 지표 |
|---|---|---:|---|
| 1 raid | loot 3~8개 중 1~3개만 의미 있게 선택 | carry slots 12~18, weight warning 70% | extract 성공/실패 로그 |
| 1 session | 3~5회 raid 반복 | 25~45분 | stash value 변화 |
| 1주 | recipe/장비 1개 해금 | 5~8 recipe | 반복 loot route 편향 |

왜: retention은 live-ops 이벤트가 아니라 “이번 raid에서 잃은 것을 다음 raid에서 회복할 수 있다”는 작고 측정 가능한 복구 루프에서 시작한다.


### 플레이어 의사결정 설계 — 루팅 순간의 5가지 질문

| 질문 | 발생 조건 | 좋은 선택지 | 나쁜 선택지 | 왜 필요한가 |
|---|---|---|---|---|
| 지금 열 것인가? | 컨테이너 상호작용 0.35~0.8초 | 주변 정리 후 탐색 | 적이 근처인데 열기 | 루팅 자체에 노출 비용을 붙인다 |
| 지금 들 것인가? | 슬롯/무게가 70%를 넘기 시작 | 고가치/목표 재료 선별 | 값싼 무거운 잡템 과적 | 아이템 가치 판단을 만든다 |
| 지금 쓸 것인가? | 체력/탄약 부족 | 회복템/탄약 즉시 사용 | 추출 전까지 아끼다 사망 | 생존템의 현재 가치를 만든다 |
| 지금 버릴 것인가? | 더 좋은 아이템 발견 | 낮은 가치/무거운 아이템 교체 | 기존 loot 집착 | 인벤토리를 “작은 경매장”으로 만든다 |
| 지금 빠질 것인가? | 고가치 아이템 획득 후 | 추출로 소유 확정 | 욕심내다 LostOnFailure | extraction 장르의 핵심 판단을 만든다 |

왜: 기획의 중심은 loot table 수량이 아니라 플레이어의 행동 변화다. 같은 전리품이라도 체력, 남은 시간, 현재 무게, 추출구 거리, 제작 목표에 따라 가치가 달라져야 한다.

### MVP 아이템군 — 20~30개로 만드는 선택 압박

| 아이템군 | 예시 | 즉시 가치 | 추출 후 가치 | 설계 의도 |
|---|---|---:|---:|---|
| 생존 소모품 | 붕대, 진통제, 임시 배터리 | 높음 | 낮음 | “팔 것인가, 지금 살 것인가”를 만든다 |
| 판매용 경량품 | 고장난 렌즈, 금속 태그 | 낮음 | 높음 | 추출 욕구를 강화한다 |
| 제작 재료 | 나사 묶음, 전선, 폐회로판 | 낮음~중간 | 중간~높음 | 잡템을 장기 목표와 연결한다 |
| 장비/모듈 | 저급 방탄판, 조준기, 가방 | 중간 | 중간 | 다음 raid 난이도 완화 |
| 위험 부채 아이템 | 무거운 엔진 코어, 신호 송신기 | 낮음 | 매우 높음 | 들고 있으면 느려지거나 추적 위험이 생긴다 |

왜: MVP에서 아이템 100개를 만들 수 없다. 대신 20~30개 안에서도 “가볍고 비싼 것”, “무겁지만 필요한 것”, “지금 쓰면 살고 팔면 돈이 되는 것”처럼 역할 차이를 만든다.

### 공간/위험-보상 설계

| 지역 | 보상 성격 | 위험 | 플레이어 행동 유도 |
|---|---|---|---|
| 시작 주변 | 저급 소모품, 가벼운 재료 | 낮음 | 초보자 회복/학습 |
| 일반 건물 | 제작 재료, 판매용 잡화 | 낮음~중간 | 루트 선택 연습 |
| 고장난 창고 | 무거운 고가치품 | 중간 | 과적 후 추출 판단 |
| 군용 잔해 | 탄약, 방어 장비 | 높음 | 전투 준비와 위험 보상 |
| 탈출구 주변 | 낮은 보상, 안전 확인 정보 | 중간 | 마지막 욕심 억제 |

왜: 맵이 작아도 지역별 보상 성격이 다르면 반복 루트가 생긴다. 고가치 지역은 단순히 아이템 가격이 높은 곳이 아니라, 들고 나온 뒤 이동/전투 판단이 바뀌는 곳이어야 한다.

### UX/피드백 설계

| 피드백 | 표시 방식 | 기준 | 이유 |
|---|---|---:|---|
| 획득 확정 전 상태 | 아이템 카드에 `Raid Carry` 배지 | 추출 전 | “아직 내 것이 아니다”를 명확히 한다 |
| 무게 경고 | HUD 색상/발소리 둔화/속도 아이콘 | 70% 이상 | 숫자보다 몸으로 위험을 느끼게 한다 |
| 고가치 획득 | 짧은 음향 + 로그 강조 | 상위 15% 가치 | 탈출 판단 전환점을 만든다 |
| 교체 압박 | 현재 아이템 vs 후보 아이템 가치/무게 비교 | inventory full | 버리기 결정을 빠르게 한다 |
| 손실 요약 | 결과 화면에서 `획득했지만 잃은 것` 분리 | 실패 시 | 실패를 다음 목표로 바꾼다 |

왜: 기술적으로 서버가 맞아도 플레이어가 상태를 이해하지 못하면 루팅 판단은 스트레스가 된다. UI는 모든 정보를 보여주는 것이 아니라 지금 결정에 필요한 정보만 빠르게 보여줘야 한다.

### 예시 raid 시나리오

1. 2분 지점: 플레이어가 일반 건물에서 `폐회로판` 2개와 `붕대` 1개를 얻는다. 아직 가볍기 때문에 탐색을 계속한다.
2. 5분 지점: 고장난 창고에서 `무거운 엔진 코어`를 얻는다. 가치가 높지만 weight warning이 켜지고 이동 속도가 85%로 떨어진다.
3. 6분 지점: 군용 잔해 쪽에서 총성이 들린다. 플레이어는 더 좋은 장비를 노릴 수 있지만, 엔진 코어를 잃으면 이번 raid 목표가 사라진다.
4. 7분 지점: 추출구까지 가는 길에 소모품을 하나 사용한다. 즉시 생존템의 가치는 판매가보다 높아진다.
5. 9분 지점: 추출 성공 시 `ExtractCommitted`로 엔진 코어가 stash에 들어가고, 실패 시 결과 화면에 `획득했지만 잃은 고가치품`으로 남는다.

왜: 이 시나리오는 새 시스템을 추가하지 않는다. 같은 loot, weight, enemy, extract 규칙만으로 플레이어 행동이 탐색자에서 생존자로 바뀌는지 확인하는 테스트 케이스다.

## 4. 기술 구조

### Unity/FishNet 적합성

- Unity 씬에는 `LootItem` network prefab과 server-owned container만 둔다.
- FishNet dedicated server가 `RequestId`, `ItemInstanceId`, 거리, raid phase, extract result를 확정한다.
- 클라이언트는 입력과 선택 의도만 보낸다: `requestId`, `targetItemId`, `containerId`, `requestedAction`.
- 서버는 inventory snapshot/event를 내려준다. UI는 snapshot을 표시만 한다.

왜: 기존 ADR-0003은 RequestId 기반 멱등 loot transaction을 이미 결정했다. 이 기획은 그 결정을 “추출 전 임시 소유” 상태 전이까지 확장한다.

### 서버 권한 검증 규칙

| 요청 | 서버 검증 | 거절 예시 | 로그 |
|---|---|---|---|
| LootRequest | 거리 ≤ 2.5m, item available, raid phase active, RequestId 신규/멱등 | 거리 초과, 이미 획득, 잘못된 phase | `LootCommitted`, `LootRejected`, `Duplicate LootRequest ignored` |
| Drop/Replace | carried item 소유자, 슬롯/무게 계산 | owner 불일치 | `LootDropped`, `LootReplaceRejected` |
| ExtractCommit | extract zone 유지 시간, 생존 상태, raid result order | hold 중단, 사망 우선 | `ExtractCommitted`, `ExtractRejected` |

### Abuse vectors / abuse vector 검토

| 악용 | 방어 | MVP 수용선 |
|---|---|---|
| 클라이언트가 item owner 직접 변경 | 서버 transaction 외 소유 변경 금지 | smoke test에서 직접 write 거절 확인 |
| 같은 RequestId 반복 | 멱등 결과 재사용 또는 중복 무시 | 중복 로그 필수 |
| itemId spoofing | 서버 scene registry에 없는 item 거절 | registry lookup 실패 로그 |
| 거리/텔레포트 조작 | 서버 위치 기준 거리 검증 | 클라이언트 좌표 신뢰 금지 |
| disconnect로 손실 회피 | MVP는 disconnect 실패 처리 | reconnect/보험은 removed scope |

### 대략 트래픽/서버 가정

- MVP 동시 플레이: 1~4인 co-op 후보.
- tick/상태 전송은 기존 movement smoke 측정 전까지 보수적으로 유지한다.
- loot 요청은 플레이어당 raid당 10~30회 수준으로 가정한다.
- payload는 request/result 중심이라 전투 snapshot보다 작다.

왜: 루팅은 cheat-sensitive이지만 high-frequency action이 아니다. 서버 권한 처리 비용보다 상태 일관성/로그 추적성이 더 중요하다.

## 5. 시장/수익 모델

| 경쟁/참고작 | 가져올 점 | 버릴 점 | 이유 |
|---|---|---|---|
| Escape from Tarkov | 추출 전 소유 미확정, 위험-보상 압박 | PvP 중심 경제, 하드코어 진입장벽 | 1인 MVP와 cheat/balance 비용 초과 |
| Escape From Duckov | PvE 완화, 회수/성장 구조 | 대량 제작/은신처 콘텐츠 | PvE-first 방향과 맞지만 콘텐츠 양 제한 필요 |
| ZERO Sievert | 탑다운 extraction, 작은 맵 반복 | 복잡한 싱글 경제 확장 | 탑다운 readability 참고, 서버 권한은 별도 차별점 |

### Target persona

- 25~40세 PC 플레이어.
- Tarkov류의 판단 압박은 좋아하지만 PvP 스트레스/시간 비용은 부담스러운 유저.
- 20~40분 세션 안에 2~4번의 작은 성공/실패 로그를 보고 싶어 하는 co-op 지향 유저.

### BM 선택

- Steam 유료 데모 → 저가 Early Access가 1차 후보.
- F2P, 거래소, 시즌 배틀패스는 removed scope.

왜: 서버 권한 co-op은 운영 비용이 생긴다. 초반 BM은 복잡한 live economy보다 구매 전환이 명확한 유료/EA가 1인 개발에 맞다.

## 6. 리스크

| 리스크 | 실패 양상 | 완화 |
|---|---|---|
| 기술 | FishNet server authority/scene spawn/loot transaction이 늦게 검증됨 | loot smoke prototype을 먼저 끝내고 GDD 수치 확정 |
| 밸런스 | 고가치 루트만 반복 | loot table 20~30개 이하, spawn seed/위험 배치 로그로 편향 확인 |
| 운영 | 서버 비용/빌드 배포 부담 | 초기 1~4인 local/headless 검증, public matchmaking 보류 |
| 비용 | 1인 개발이 UI/경제/콘텐츠를 과하게 만듦 | removed scope를 문서에 고정 |
| 감정 | 사망 손실이 과해 이탈 | PvE-first는 1회 회수 표식을 optional로 두되 MVP에서는 실패 규칙부터 검증 |

왜: 이 시스템의 가장 큰 실패는 “Tarkov를 작게 만들기”가 아니라 “작게 만들 수 없는 Tarkov를 따라 하기”다. 서버 권한과 추출 commit만 남기고 경제 규모는 잘라야 한다.

## 7. MVP 6개월 압축안

### Required scope

- 1개 gameplay map.
- loot item 20~30개.
- container 3종: loose loot, crate, corpse/drop placeholder.
- inventory: 슬롯 12~18 + 단순 weight.
- raid timer 8~12분.
- extraction zone 1~2개, hold 3~5초.
- server logs: `LootCommitted`, `LootRejected`, `Duplicate LootRequest ignored`, `ExtractCommitted`, `LostOnFailure`.
- result screen: carried / extracted / lost summary.

### Optional scope

- PvE 사망 지점 1회 회수 표식.
- 잠긴 container 1종.
- 제작 recipe 5~8개.
- 희귀 아이템 sound/UI 강조.

### Removed scope

- PvP 경제.
- 복잡한 시장/거래소.
- 대형 인벤토리 테트리스.
- 보험/우편 회수 시스템.
- 은신처 다단계 업그레이드.
- 동적 이벤트/야간/폭풍 고위험 보상.
- public matchmaking과 live-ops season.

### 월별 milestone

| 월 | 목표 | 완료 증거 |
|---:|---|---|
| 1 | FishNet authority smoke: player, loot, server logs | Unity/MPPM 로그 |
| 2 | raid phase/timer/extract commit | `ExtractCommitted`/failure 로그 |
| 3 | inventory snapshot/result screen | 서버 snapshot과 UI 일치 |
| 4 | loot table 20~30개, container 3종 | spawn/획득/손실 QA 표 |
| 5 | combat/AI와 loot route 연결 | 위험 지역별 success/fail 로그 |
| 6 | vertical slice polish, Steam demo candidate | 20~40분 플레이 세션 검증 |

왜: 문서상 장기 경제보다 서버 권한 smoke와 결과 화면이 먼저다. 이 두 가지가 없으면 루팅 설계는 검증되지 않는다.

## 8. 디렉터 최종 판단

이 방향은 유지할 가치가 있다. 이유는 세 가지다.

1. 기존 프로젝트의 서버 권한 핵심과 직접 연결된다.
2. 원본 역기획서의 강점인 “소유 확정 지연”을 PvE-first 탑다운 MVP에 맞게 축소한다.
3. 적은 콘텐츠로도 raid마다 판단을 만들 수 있다.

가장 큰 실패 지점은 scope creep이다. “아이템 용도 5종”, “회수”, “제작”, “은신처”, “잠긴 방”을 모두 MVP에 넣으면 6개월 안에 서버 권한 검증이 밀린다. 대응은 명확하다. MVP는 loot transaction, carried raid state, extract commit, result screen까지만 필수로 둔다. 나머지는 로그가 안정된 뒤 추가한다.
