# FishNet Dedicated Server 온보딩

> 목적: 현재 프로젝트에서 FishNet 기반 데디케이트 서버를 만들기 전에, “게임 서버가 무엇을 책임지는지”, “PUN2와 FishNet이 어떻게 다른지”, “TCP/UDP/Transport/쿠버네티스/호스팅을 어떤 순서로 이해해야 하는지”를 빠르게 잡기 위한 입문 문서입니다.

> 문서 경계: 이 문서는 온보딩/학습 자료입니다. 최종 기술 선택, 운영 아키텍처, 호스팅 전략을 확정하는 ADR이 아니며, 구속력 있는 결정은 `docs/architecture/`의 ADR로 분리해야 합니다.

## 1. 이미지에서 읽은 요청 요약

이미지의 핵심 요청은 다음과 같습니다.

- 현재 프로젝트에서 `FishNet`을 사용해 데디케이트 서버를 만들고 개발하려고 한다.
- 본인은 게임 서버를 잘 모르고, FishNet도 잘 모른다.
- 원래는 `PUN2` 같은 더 관리형에 가까운 네트워크 솔루션을 생각했지만, 현재 프로젝트의 기획·확장성·서버 권위 구조를 생각하면 서버 자체를 직접 다루는 쪽이 필요하다고 판단했다.
- 무작정 만들기보다 먼저 흐름을 이해하고 시작하고 싶다.
- `PUN2`, 개발된 서버, `TCP`, `Kubernetes`, 서버 개념 등을 2주 정도 학습한 사람도 따라올 수 있게 FishNet + 데디케이트 서버 지식과 튜토리얼형 문서를 만들어 달라는 요청이다.

## 2. 한 줄 결론

`PUN2`는 “Photon이 운영하는 서버 생태계에 클라이언트가 붙는 방식”에 가깝고, `FishNet + Dedicated Server`는 “우리 Unity 게임 빌드가 서버 프로세스로도 실행되어 게임 상태를 권위 있게 판단하는 방식”에 가깝습니다.

이 프로젝트가 서버 권위(authoritative server), 확장성, 장기 운영을 고민한다면 FishNet은 좋은 방향이지만, 대신 다음 책임을 직접 설계해야 합니다.

- 서버 빌드 생성
- 서버 실행/종료
- 포트와 방화벽
- 접속 주소 배포
- 매치메이킹 또는 방 배정
- 서버 권위 로직
- 배포/모니터링/로그
- 필요 시 컨테이너와 오케스트레이션

## 3. PUN2와 FishNet의 차이

| 구분 | PUN2 | FishNet + Dedicated Server |
|---|---|---|
| 기본 관점 | Photon Cloud/Photon Server에 클라이언트가 접속 | Unity 게임 인스턴스가 서버 또는 호스트로 실행 |
| 빠른 시작 | 쉬움 | Network Flow는 쉽지만 운영 서버는 직접 설계 필요 |
| 서버 로직 통제 | Photon Cloud 사용 시 제한적, 자체 Photon Server면 가능 | Unity C# 코드로 직접 통제 |
| 권위 모델 | 룸/마스터 클라이언트/Photon 서버 구조 이해 필요 | FishNet 기본은 서버 권위 중심 |
| 운영 부담 | Cloud 사용 시 낮음 | 서버 빌드, 호스팅, 배포, 모니터링 부담 증가 |
| 적합한 상황 | 빠른 멀티플레이 프로토타입, 관리형 서비스 선호 | 서버 판정, 치트 대응, 커스텀 세션/운영 구조 필요 |

Photon 공식 문서 기준으로 PUN은 Photon 서버에 연결되고, Photon Cloud는 호스팅/로드밸런싱된 서버를 제공합니다. 반면 FishNet 기반 데디케이트 서버는 Unity 서버 빌드 안에서 프로젝트가 직접 권위 로직, 연결 흐름, 세션 운영을 설계해야 합니다.

## 4. FishNet에서 말하는 서버/호스트/클라이언트

FishNet에서 자주 나오는 용어는 다음처럼 이해하면 됩니다.

- **Client**: 플레이어가 실행하는 게임 앱입니다.
- **Server**: 여러 Client가 접속하는 게임 상태의 중심입니다.
- **Host**: 한 프로세스가 Client와 Server 역할을 동시에 합니다. 로컬 테스트와 빠른 개발에 편합니다.
- **Dedicated Server**: 플레이어가 직접 조작하지 않는 서버 전용 프로세스입니다. 보통 그래픽 없이 실행하고, 게임 상태 판단과 동기화를 담당합니다.

처음부터 Dedicated Server만 붙잡기보다, 다음 순서가 안전합니다.

1. Editor에서 Host/Client로 흐름 이해
2. 같은 PC에서 ServerOnly + Client 접속 테스트
3. LAN 또는 다른 PC에서 접속 테스트
4. Linux Dedicated Server 빌드 생성
5. VPS/클라우드에 서버 배포
6. 컨테이너화
7. Kubernetes 또는 게임 서버 오케스트레이션 검토

## 5. 이 프로젝트에서 먼저 볼 위치

현재 repo에는 FishNet vendor 코드와 ProjectRaidAuthority용 FishNet Network Flow이 모두 있습니다.

### FishNet 패키지/엔진 코드

- `Assets/10_FishNet/`
- `Assets/10_FishNet/Runtime/Managing/NetworkManager.cs`
- `Assets/10_FishNet/Runtime/Managing/Server/ServerManager.cs`
- `Assets/10_FishNet/Runtime/Managing/Client/ClientManager.cs`
- `Assets/10_FishNet/Runtime/Managing/Scened/SceneManager.cs`
- `Assets/10_FishNet/Runtime/Transporting/`
- `Assets/10_FishNet/Runtime/Transporting/Transports/Tugboat/`

FishNet에는 바로 쓰는 내장 NetworkFlowController가 없으므로, 현재 프로젝트에서는 `FishNetNetworkFlowController`가 연결 시작, 룸 씬 로드, Ready 상태 확인, 게임플레이 씬 로드, 플레이어 스폰 흐름을 담당합니다.

### 프로젝트 FishNet Network Flow

- `Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/OfflineBootstrap.unity`
- `Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/MatchRoom.unity`
- `Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow/Gameplay.unity`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Shared/FishNetNetworkFlowController.cs`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Client/FishNetNetworkFlowController.Client.cs`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Server/FishNetNetworkFlowController.Server.cs`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Shared/RoomPlayer.cs`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Client/RoomPlayer.Client.cs`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Server/RoomPlayer.Server.cs`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Shared/GamePlayer.cs`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Client/GamePlayer.Client.cs`
- `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow/Server/GamePlayer.Server.cs`
- `Assets/00_ProjectRaidAuthority/02_Prefabs/00_Network/00_FishNetNetworkFlow/`

이 문서를 읽은 뒤에는 위 Network Flow를 “서버/룸/게임플레이 흐름을 보는 실험실”로 쓰면 됩니다.

## 6. TCP, UDP, Transport를 게임 서버 관점에서 이해하기

### TCP

TCP는 패킷 순서와 재전송을 보장합니다. 채팅, 로그인, 상점, 인벤토리 저장처럼 “정확성이 우선”인 곳에 잘 맞습니다. 하지만 실시간 위치 동기화처럼 최신 상태가 중요한 데이터에는 지연이 누적될 수 있습니다.

### UDP

UDP는 TCP처럼 순서/재전송을 기본 보장하지 않습니다. 대신 지연이 낮고 실시간 데이터에 유리합니다. 실시간 게임에서는 “오래된 위치 패킷을 늦게 받는 것”보다 “최신 위치를 빨리 받는 것”이 더 중요할 때가 많습니다.

### Transport

Transport는 FishNet이 서버/클라이언트 간 실제 패킷 송수신에 사용하는 전송 계층입니다. 현재 FishNet 패키지에는 Tugboat 같은 Transport 구현이 포함되어 있으며, 실제 포트와 프로토콜은 선택한 Transport 설정을 기준으로 확인해야 합니다.

FishNet으로 시작할 때는 일단 프로젝트에 붙어 있는 Transport 컴포넌트를 기준으로 다음을 확인합니다.

- 서버 포트: 기본 `7777`
- 클라이언트가 접속할 서버 주소
- 방화벽/보안 그룹 인바운드 규칙
- 로컬/원격에서 접속 가능한지
- 서버 로그에 접속/해제 이벤트가 찍히는지

## 7. FishNet Dedicated Server 튜토리얼 흐름

### Step 0. 목표를 작게 잡기

처음 목표는 “완성된 매치메이킹 서버”가 아닙니다.

첫 목표는 다음 3개입니다.

1. 서버 프로세스가 켜진다.
2. 클라이언트가 서버에 붙는다.
3. 플레이어 오브젝트 또는 간단한 상태가 동기화된다.

### Step 1. Network Flow 씬을 이해한다

먼저 다음 순서로 Network Flow를 봅니다.

1. `OfflineBootstrap.unity`
   - 메뉴/진입점
   - Host/Client 시작 버튼 흐름
2. `MatchRoom.unity`
   - 룸 플레이어
   - Ready 상태
   - 게임 씬으로 넘어가기 전 대기 흐름
3. `Gameplay.unity`
   - 실제 플레이어 프리팹
   - 이동 또는 상태 동기화

확인할 질문은 다음입니다.

- `NetworkManager` 또는 `FishNetNetworkFlowController`는 어느 GameObject에 붙어 있는가?
- Transport는 무엇인가?
- Offline Scene과 Online Scene은 무엇으로 설정되어 있는가?
- Player Prefab과 Room Player Prefab은 어디를 바라보는가?
- Host로 시작했을 때와 Client로 시작했을 때 로그 차이는 무엇인가?

### Step 2. Host 모드로 먼저 성공시킨다

Editor에서 바로 Dedicated Server부터 만들지 말고, Host 모드로 먼저 성공시킵니다.

- Host 시작
- 같은 Editor/빌드 또는 별도 클라이언트에서 접속
- 룸 진입
- Ready
- 게임 씬 진입
- 플레이어 생성 확인

Host 모드는 서버와 클라이언트가 같은 프로세스에 있으므로 디버깅이 쉽습니다.

### Step 3. ServerOnly 모드를 테스트한다

다음은 서버만 켜고 별도 클라이언트가 붙는 흐름입니다.

- 서버 인스턴스: `NetworkManager.ServerManager.StartConnection()`
- 클라이언트 인스턴스: 서버 주소로 `NetworkManager.ClientManager.StartConnection(address)`
- 같은 PC에서는 `localhost`
- 다른 PC/VPS에서는 실제 IP 또는 도메인

이 단계에서 중요한 것은 “서버가 화면에 보이는가”가 아니라 “로그와 네트워크 상태로 서버가 살아 있음을 확인하는가”입니다.

### Step 4. Dedicated Server 빌드를 만든다

Unity 공식 문서 기준으로 Dedicated Server 빌드는 Editor, 스크립트, 커맨드라인으로 만들 수 있습니다. 커맨드라인에서는 `-standaloneBuildSubtarget Server` 인자를 사용합니다.

개념적으로는 다음 형태입니다.

```bash
Unity \
  -batchmode \
  -quit \
  -projectPath <project-path> \
  -buildTarget Linux64 \
  -standaloneBuildSubtarget Server
```

실제 프로젝트에서는 빌드 메서드, 빌드 경로, CI 환경에 맞춰 별도 스크립트를 두는 것이 좋습니다.

### Step 5. 로컬에서 서버 빌드를 실행한다

서버 빌드가 만들어지면 먼저 로컬에서 실행합니다.

확인할 것:

- 프로세스가 바로 종료되지 않는가?
- 서버 빌드에서 서버 시작 부트스트랩 코드가 실행되는가?
- 포트가 열렸는가?
- 클라이언트가 `localhost` 또는 LAN IP로 붙는가?
- 서버 로그에 접속 이벤트가 찍히는가?

### Step 6. VPS 또는 클라우드에 올린다

가장 단순한 운영 실험은 Kubernetes가 아니라 VPS 한 대입니다.

1. Linux 서버 준비
2. 서버 빌드 업로드
3. 실행 권한 부여
4. 포트 오픈
5. 서버 실행
6. 클라이언트에서 공인 IP로 접속
7. 로그 확인

이 단계에서 실패하면 대부분 다음 문제입니다.

- 포트가 막힘
- 클라이언트 주소가 잘못됨
- 서버가 자동 시작되지 않음
- 빌드에 필요한 씬이 빠짐
- Linux 실행 권한 문제
- 방화벽/보안 그룹 설정 누락

### Step 7. 컨테이너와 Kubernetes는 나중에 붙인다

Kubernetes는 서버를 “잘 만드는 방법”이 아니라, 이미 서버가 잘 실행될 때 “여러 서버 인스턴스를 배포/스케일/복구하는 방법”입니다.

따라서 순서는 다음이 좋습니다.

1. 로컬 ServerOnly 성공
2. Linux Dedicated Server 빌드 성공
3. VPS 수동 실행 성공
4. Docker 이미지화
5. 단일 컨테이너 실행 성공
6. 여러 서버 인스턴스 필요성이 생겼을 때 Kubernetes 또는 게임 서버 오케스트레이션 검토

처음 2주차 학습 단계에서는 Kubernetes보다 “서버 빌드가 켜지고 클라이언트가 붙는 것”이 먼저입니다.

## 8. 서버 권위 로직을 어디에 둘 것인가

FishNet을 쓰는 이유가 Dedicated Server라면 핵심은 서버 권위입니다.

나쁜 방향:

- 클라이언트가 “나 데미지 100 줬어”라고 말하면 서버가 그대로 믿는다.
- 클라이언트가 위치를 마음대로 보내고 서버가 그대로 반영한다.
- 아이템 획득/스킬 판정/매치 결과를 클라이언트가 결정한다.

좋은 방향:

- 클라이언트는 입력 또는 요청만 보낸다.
- 서버가 위치, 충돌, 데미지, 아이템 획득, 승패를 검증한다.
- 서버 상태가 클라이언트로 동기화된다.
- 클라이언트 예측은 가능하지만 최종 권위는 서버에 둔다.

처음 구현할 최소 예시는 다음입니다.

- 클라이언트: 이동 입력 전송
- 서버: 이동 가능 여부 판단
- 서버: 최종 위치 갱신
- 클라이언트들: 서버 위치를 받아 표시

## 9. 최소 실험 체크리스트

### 로컬 Host 체크

- [ ] `OfflineBootstrap.unity`에서 Host 시작 가능
- [ ] 룸 또는 게임 씬으로 이동 가능
- [ ] 플레이어 프리팹 생성 확인
- [ ] Console에 FishNet 오류 없음

### 로컬 ServerOnly + Client 체크

- [ ] 서버 인스턴스가 서버 모드로 시작
- [ ] 클라이언트가 `localhost`로 접속
- [ ] 접속/해제 로그 확인
- [ ] 플레이어 생성/동기화 확인

### Dedicated Server 빌드 체크

- [ ] Dedicated Server 빌드 타겟 또는 `-standaloneBuildSubtarget Server` 사용
- [ ] 필요한 씬이 Build Settings에 포함됨
- [ ] 서버 빌드 실행 시 서버 시작 부트스트랩 코드가 실행됨
- [ ] 포트 `7777` 또는 설정 포트가 열림
- [ ] 클라이언트가 서버 IP로 접속

### 운영 실험 체크

- [ ] VPS/클라우드 방화벽 인바운드 포트 오픈
- [ ] 서버 로그 파일 또는 콘솔 로그 확보
- [ ] 서버 재시작 방법 확보
- [ ] 빌드 버전과 클라이언트 버전 일치
- [ ] 동시 접속 수를 작게라도 측정

## 10. 지금 당장 만들면 좋은 학습 산출물

처음부터 완성 서버를 만들기보다 다음 산출물을 차례로 만들면 좋습니다.

1. **FishNet Network Flow 실행 노트**
   - 어떤 씬에서 어떤 버튼을 누르면 어떤 상태가 되는지 기록
2. **Host/Client 로그 비교표**
   - Host, ServerOnly, ClientOnly에서 찍히는 로그 차이 기록
3. **서버 권위 미니 테스트**
   - 서버만 점수/데미지를 바꿀 수 있게 만들기
4. **Dedicated Server 빌드 스크립트 초안**
   - Linux64 Server 빌드 자동화
5. **VPS 접속 실험 기록**
   - IP, 포트, 방화벽, 접속 성공/실패 원인 기록
6. **운영 방향 결정 문서**
   - 매치메이킹을 직접 만들지, Edgegap/Multiplay/기타 호스팅을 쓸지 결정

## 11. 추천 학습 순서

### 1일차: 용어 정리

- Server, Client, Host, Dedicated Server
- Authority, Spawn, NetworkObject, NetworkBehaviour, ServerRpc/ObserversRpc
- Transport, Port, Address

### 2~3일차: FishNet Network Flow 실행

- Host 실행
- Client 접속
- Room 흐름 확인
- Gameplay 씬 동기화 확인

### 4~5일차: 서버 권위 미니 기능

- 서버가 점수 관리
- 클라이언트 요청 검증
- 잘못된 요청 무시

### 6~7일차: ServerOnly 흐름

- 서버 인스턴스와 클라이언트 인스턴스 분리
- 로그 기반으로 상태 확인
- 포트/주소 문제 해결

### 8~10일차: Dedicated Server 빌드

- Linux Server 빌드
- 로컬 실행
- 클라이언트 접속

### 11~14일차: 원격 서버 실험

- VPS 배포
- 방화벽/포트 설정
- 접속 테스트
- 재시작/로그 확인
- Docker/Kubernetes 필요성 판단

## 12. 이 문서의 판단 기준

이 문서는 “최종 아키텍처 결정”이 아니라 온보딩 문서입니다. 따라서 지금의 정답은 다음입니다.

- 먼저 FishNet Network Flow을 실행해서 Host/Client/ServerOnly 차이를 몸으로 확인한다.
- Dedicated Server는 Unity 서버 빌드 타겟과 FishNet 서버 시작 부트스트랩이 맞아야 한다.
- Transport/포트/방화벽은 실제 접속 실패를 해결할 때 반드시 이해해야 한다.
- Kubernetes는 초기 학습 주제가 아니라, 서버 빌드와 수동 배포가 성공한 뒤의 운영 주제다.
- PUN2보다 FishNet이 더 “직접 다루는” 구조이므로 서버 권위 로직과 운영 책임을 프로젝트가 가져간다.

## 13. 참고 링크

- FishNet 공식 문서 — General: <https://fish-networking.gitbook.io/docs/>
- FishNet 공식 문서 — Getting Started: <https://fish-networking.gitbook.io/docs/tutorials/simple/starting-fishnets-connections>
- FishNet 공식 문서 — Cloud Hosting Guides: <https://fish-networking.gitbook.io/docs/guides/features/hosting>
- Unity 공식 문서 — Dedicated Server 빌드: <https://docs.unity3d.com/kr/2023.1/Manual/dedicated-server-build.html>
- Photon PUN2 API 문서 — General Documentation: <https://doc-api.photonengine.com/en/pun/current/general.html>
- Photon Fusion 공식 문서 — Dedicated Servers: <https://doc.photonengine.com/fusion/v2/concepts-and-patterns/dedicated-server-overview>
