# FishNet 서버 권한 스모크 프로토타입 남은 할 일

## 목적

현재 `00_FishNetNetworkFlow`은 룸/Ready/게임플레이 씬 전환과 플레이어 스폰까지 상당 부분 구현되어 있다. 남은 목표는 이 Network Flow를 **서버 권한 스모크 프로토타입**으로 검증하는 것이다.

핵심은 클라이언트가 결과를 직접 확정하지 않고, 서버가 입력을 검증한 뒤 위치/상태/전리품 결과를 확정한다는 것을 증명하는 것이다.

## 코드 배치 원칙

서버와 클라이언트는 같은 Unity/FishNet 프로젝트를 사용하지만, 게임플레이 코드는 기능 도메인별로 `Client` / `Server` / `Shared`를 나눠 배치한다. 이 구분은 빌드 분리가 아니라 책임 분리를 먼저 달성하기 위한 규칙이다.

```text
Assets/00_ProjectRaidAuthority/01_Scripts/
  00_Network/
    Bootstrap/          # MPPM 태그, server/client/host 시작, 주소 선택
    Shared/             # 네트워크 공통 상수와 실행 역할 enum

  10_Raid/
    Client/             # 레이드 UI/표시, 클라이언트 요청 진입점
    Server/             # 레이드 phase, 서버 확정 이벤트, 로그
    Shared/             # RaidState, RaidEvent payload

  20_Player/
    Client/             # 입력 수집, 카메라, 로컬 표시
    Server/             # 이동 검증, 위치/회전 확정
    Shared/             # 입력 DTO, PlayerState, 팀/식별자

  30_Loot/
    Client/             # 획득 키 입력, 획득 UI/피드백
    Server/             # 거리 검증, requestId 멱등 처리, owner 확정
    Shared/             # LootRequest, LootResult, item id/state
```

### 의미와 의도

- `Client` 코드는 입력과 표현만 담당한다. `transform.position`, loot owner, HP, 보상 같은 결과를 직접 확정하지 않는다.
- `Server` 코드는 게임플레이 진실 상태를 소유한다. 검증, 확정, 거절, 중복 요청 방지, 로그를 담당한다.
- `Shared` 코드는 양쪽이 함께 읽는 타입과 payload만 둔다. 서버 판정 로직이나 클라이언트 UI 로직을 넣지 않는다.
- `Bootstrap`은 실행 역할 선택 영역이다. `StartServerOnly`, `StartClient(address)`, `StartHost` 같은 시작 코드가 함께 있어도 괜찮지만, 실제 전투/이동/루팅 확정 로직과 분리한다.

이 구조의 검토 기준은 “기능 도메인을 한곳에서 찾을 수 있는가”와 “클라이언트 폴더에 서버 확정 로직이 들어가지 않았는가”이다.

---

## 1. 현재 Network Flow 실행 확인

- [ ] `OfflineBootstrap` 씬에서 Host 실행 확인
- [ ] 로컬 Client 참가 확인
- [ ] `MatchRoom` 씬 로드 확인
- [ ] Room Player 스폰 확인
- [ ] Ready 버튼 동작 확인
- [ ] 모든 플레이어 Ready 후 `Gameplay` 씬 이동 확인
- [ ] Game Player 스폰 확인
- [ ] 현재 이동 동기화가 되는지 확인

---

## 2. 이동을 서버 권한 구조로 변경

- [x] `20_Player/Shared`에 이동 입력 payload와 서버 확정 상태 타입 배치 — 현재 스모크 범위에서는 `00_Network/00_FishNetNetworkFlow/Shared/GamePlayer.cs`의 공유 상태로 기능 등가 구현
- [x] `20_Player/Client`에 클라이언트 입력 수집과 `ServerRpc` 요청 진입점 배치 — 현재 스모크 범위에서는 `00_Network/00_FishNetNetworkFlow/Client/GamePlayer.Client.cs`에 기능 등가 구현
- [x] `20_Player/Server`에 입력 clamp / 검증 / 위치·회전 확정 로직 배치 — 현재 스모크 범위에서는 `00_Network/00_FishNetNetworkFlow/Server/GamePlayer.Server.cs`에 기능 등가 구현
- [x] `GamePlayer`에서 클라이언트 직접 `transform.position` 변경 제거
- [x] 클라이언트는 이동 입력만 읽도록 변경
- [x] 이동 입력을 `ServerRpc`로 서버에 전송
- [x] 서버에서 입력값 clamp / 검증
- [x] 서버에서 위치와 회전 계산
- [x] 서버만 `transform.position`, `transform.rotation` 변경
- [x] `GamePlayer.prefab`의 `NetworkTransform`을 서버 권한 설정으로 변경
- [x] 서버 이동 적용 로그 추가
- [x] 클라이언트에서 서버 결과가 동기화되는지 확인

---

## 3. 서버 권한 Loot 스모크 추가

- [ ] `30_Loot/Shared`에 `LootRequest`, `LootResult`, item id/state 타입 배치
- [ ] `30_Loot/Client`에 획득 입력과 요청 전송 코드 배치
- [ ] `30_Loot/Server`에 거리 검증, 소유권 확정, `requestId` 멱등 처리 배치
- [ ] 테스트용 Loot Item 오브젝트 또는 prefab 생성
- [ ] Loot Item에 `NetworkObject` 추가
- [ ] Loot Item에 `itemId` 추가
- [ ] Loot Item에 `looted / ownerId` 상태 추가
- [ ] 플레이어가 `E` 키로 loot 요청 보내기
- [ ] loot 요청을 `ServerRpc`로 서버에 전송
- [ ] 요청마다 `requestId` 포함
- [ ] 서버에서 플레이어와 아이템 거리 확인
- [ ] 서버에서 이미 획득된 아이템인지 확인
- [ ] 서버에서 item owner 확정
- [ ] 같은 `requestId` 중복 요청 시 중복 처리 방지
- [ ] 성공 시 `LootCommitted` 로그 출력
- [ ] 실패 시 `LootRejected` 로그 출력
- [ ] 중복 요청 시 `Duplicate LootRequest ignored` 로그 출력

---

## 4. 서버 권한 검증 로그 정리

- [ ] 서버 시작 로그 확인
- [ ] 클라이언트 접속 로그 확인
- [ ] Room Player 스폰 로그 확인
- [ ] Game Player 스폰 로그 확인
- [ ] 이동 입력 수신 로그 확인
- [ ] 이동 입력 검증 로그 확인
- [ ] 서버 위치 적용 로그 확인
- [ ] LootRequest 수신 로그 확인
- [ ] LootCommitted 로그 확인
- [ ] LootRejected 로그 확인
- [ ] Duplicate LootRequest 로그 확인

---

## 5. 실제 검증

- [ ] Host 1개 실행
- [ ] Client 1개 이상 접속
- [ ] Ready 후 게임플레이 씬 이동
- [ ] Player 이동 확인
- [ ] 이동이 서버 적용 결과로 동기화되는지 확인
- [ ] Loot Item 근처에서 획득 요청
- [ ] Loot 성공 확인
- [ ] 같은 요청 중복 처리 확인
- [ ] 이미 획득된 아이템 재획득 거절 확인
- [ ] 서버 로그로 전체 흐름 추적 가능 확인

---

## 6. 완료 증거 문서 작성

- [ ] `prototypes/fishnet-authority-smoke/` 디렉터리 생성
- [ ] `prototypes/fishnet-authority-smoke/README.md` 작성
- [ ] 실행한 씬 기록
- [ ] 실행 방법 기록
- [ ] 검증한 서버 권한 항목 기록
- [ ] 성공 로그 샘플 기록
- [ ] 남은 한계 기록
- [ ] 다음 단계로 가져갈 결정 기록

---

## 7. 후속 판단 항목 — Prediction/Reconcile

- [ ] 서버 권한 스모크 프로토타입 완료 후 Prediction/Reconcile 도입 여부를 판단한다.
- [x] 현재 단계에서는 클라이언트 예측, rollback, reconcile buffer, tick alignment를 구현하지 않는다.
- [ ] 도입 판단 기준은 이동/사격/피격 체감 지연, 서버 snapshot 보정 흔들림, latency 테스트 결과가 실제 문제로 확인되는 경우로 제한한다.
- [x] 도입 시점은 서버 권한 이동, loot commit, 기본 전투 판정이 먼저 검증된 이후로 둔다.

---

## 진행 확인 메모 — 2026-05-19

체크 기준: 현재 파일/코드/프리팹으로 확인 가능한 구현 완료 항목만 체크했다. `## 1. 현재 Network Flow 실행 확인`, `## 4. 서버 권한 검증 로그 정리`, `## 5. 실제 검증`은 실제 Host/Client 실행 로그나 수동 검증 증거가 필요하므로 이번 확인에서는 완료 처리하지 않았다.

확인한 구현 근거:

- `GamePlayer.Client.cs`: owner 입력 수집, 카메라 기준 이동 방향 계산, `ServerSetMovementInput(moveDirection, lookDirection)` 호출
- `GamePlayer.Server.cs`: `[ServerRpc] ServerSetMovementInput`, `SanitizeDirection` 기반 clamp/검증, 서버 위치/회전 적용, 서버 이동/회전 로그
- `GamePlayer.cs`: `IsServerStarted`일 때만 서버 이동 적용, `IsOwner`일 때 입력 전송
- `GamePlayer.prefab`: `NetworkTransform`의 `_clientAuthoritative: 0`으로 서버 권한 설정

아직 미구현/미검증으로 남긴 범위:

- `30_Loot/*` 코드와 Loot Item prefab은 아직 없음
- `LootCommitted`, `LootRejected`, `Duplicate LootRequest ignored` 로그는 아직 없음
- 실제 Host/Client 실행 검증과 `prototypes/fishnet-authority-smoke/README.md` 완료 증거는 아직 없음

---

## 가장 먼저 할 일

```text
서버 권한 Loot 스모크 추가
```

현재 가장 중요한 남은 작업은 테스트용 Loot Item을 만들고, 클라이언트 E 키 입력 → ServerRpc 요청 → 서버 거리/중복/requestId 검증 → LootCommitted 또는 LootRejected 로그 흐름을 구현하는 것이다.
