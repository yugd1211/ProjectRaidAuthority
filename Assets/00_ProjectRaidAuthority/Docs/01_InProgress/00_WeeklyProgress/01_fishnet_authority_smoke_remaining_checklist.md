# FishNet 서버 권한 스모크 프로토타입 남은 할 일

## 목적

현재 `00_FishNetSample`은 룸/Ready/게임플레이 씬 전환과 플레이어 스폰까지 상당 부분 구현되어 있다. 남은 목표는 이 샘플을 **서버 권한 스모크 프로토타입**으로 바꾸는 것이다.

핵심은 클라이언트가 결과를 직접 확정하지 않고, 서버가 입력을 검증한 뒤 위치/상태/전리품 결과를 확정한다는 것을 증명하는 것이다.

---

## 1. 현재 샘플 실행 확인

- [ ] `FishNetOffline` 씬에서 Host 실행 확인
- [ ] 로컬 Client 참가 확인
- [ ] `FishNetGameRoom` 씬 로드 확인
- [ ] Room Player 스폰 확인
- [ ] Ready 버튼 동작 확인
- [ ] 모든 플레이어 Ready 후 `FishNetGamePlay` 씬 이동 확인
- [ ] Game Player 스폰 확인
- [ ] 현재 이동 동기화가 되는지 확인

---

## 2. 이동을 서버 권한 구조로 변경

- [ ] `FishNetSampleGamePlayer`에서 클라이언트 직접 `transform.position` 변경 제거
- [ ] 클라이언트는 이동 입력만 읽도록 변경
- [ ] 이동 입력을 `ServerRpc`로 서버에 전송
- [ ] 서버에서 입력값 clamp / 검증
- [ ] 서버에서 위치와 회전 계산
- [ ] 서버만 `transform.position`, `transform.rotation` 변경
- [ ] `FishNetGamePlayer.prefab`의 `NetworkTransform`을 서버 권한 설정으로 변경
- [ ] 서버 이동 적용 로그 추가
- [ ] 클라이언트에서 서버 결과가 동기화되는지 확인

---

## 3. 서버 권한 Loot 스모크 추가

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

## 가장 먼저 할 일

```text
FishNetSampleGamePlayer 이동을 서버 권한 구조로 변경
```

현재 가장 중요한 남은 작업은 클라이언트 직접 이동을 제거하고, 클라이언트 입력 → 서버 검증 → 서버 위치 확정 → 클라이언트 표시 흐름으로 바꾸는 것이다.
