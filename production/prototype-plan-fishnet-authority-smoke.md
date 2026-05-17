# FishNet Authority Smoke Prototype 계획

## 상태

**계획됨. 아직 구현되지 않음.**

이 문서는 프로토타입 계획이며 `/prototype 완료` 증거가 아니다. 실제 구현과 검증이 끝나기 전에는 `prototypes/fishnet-authority-smoke/README.md`를 만들지 않는다.

## 목표

Unity/FishNet dedicated server가 게임플레이 진실 상태를 소유할 수 있음을 가장 작은 범위에서 증명한다.

## 최소 시나리오

1. 헤드리스 또는 server mode 실행
2. 클라이언트 1–2개 접속
3. 서버 소유 더미 entity 생성
4. 클라이언트 input 전송
5. 서버가 위치 또는 상태를 확정
6. 클라이언트는 snapshot/event를 표시
7. 서버 로그에 검증 가능한 event 기록

## 선택 확장

- 서버 판정 damage
- RequestId 기반 loot transaction
- extraction hold와 commit event

## 코드 구조 원칙

서버 권한 스모크 프로토타입은 한 Unity/FishNet 프로젝트 안에서 서버와 클라이언트를 함께 빌드·실행하되, 코드 배치는 기능 도메인별로 `Client` / `Server` / `Shared` 하위 폴더를 둔다.

```text
Assets/00_ProjectRaidAuthority/01_Scripts/
  00_Network/
    Bootstrap/
    Shared/

  10_Raid/
    Shared/
    Server/
    Client/

  20_Player/
    Shared/
    Server/
    Client/

  30_Loot/
    Shared/
    Server/
    Client/
```

### 의미

- `Client`: 입력 수집, UI, 카메라, VFX, 로컬 표시, `ServerRpc` 요청 전송을 담당한다. 결과를 직접 확정하지 않는다.
- `Server`: 이동·전투·루팅·탈출 같은 게임플레이 결과를 검증하고 확정한다. 서버 권한 상태 변경과 검증 로그를 담당한다.
- `Shared`: 서버와 클라이언트가 함께 알아야 하는 enum, DTO, request payload, state id, 설정값을 둔다. 확정 로직은 두지 않는다.
- `Bootstrap`: MPPM 태그, dedicated server, host/client 시작처럼 실행 역할을 선택하는 시작 코드를 둔다. 서버/클라이언트 시작 API가 함께 있어도 게임플레이 확정 로직과 분리한다.

### 의도

이 구조는 “한 프로젝트에서 서버와 클라이언트를 모두 사용한다”는 FishNet 개발 방식을 유지하면서도, 게임플레이 책임이 섞이는 것을 막기 위한 경계다. 기능을 고칠 때는 `Raid`, `Player`, `Loot` 같은 도메인 안에서 관련 서버·클라이언트·공유 코드를 함께 찾을 수 있고, 서버 권한 위반 여부는 `Client` 코드가 결과를 직접 변경하는지 확인하는 방식으로 검토한다.

스모크 프로토타입의 최소 흐름은 다음 경계를 따른다.

```text
Client 입력 코드
  → ServerRpc 요청
Server 권한 코드
  → 검증 / 상태 확정 / 로그
Shared 상태·payload
  → SyncVar 또는 ObserversRpc로 Client 표시
Client 표현 코드
  → UI/VFX/로컬 표시만 수행
```

## 완료 기준

- [ ] 서버와 클라이언트가 분리 실행된다.
- [ ] 클라이언트 직접 state write가 불가능하다.
- [ ] 서버 확정 snapshot/event가 클라이언트에 표시된다.
- [ ] 로그 또는 테스트 결과가 남는다.
- [ ] 결과를 `prototypes/fishnet-authority-smoke/README.md`에 기록한다.
