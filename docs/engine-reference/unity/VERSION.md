# Unity 버전 기준

## 현재 고정 버전

- **Unity**: 6000.4.5f1
- **URP**: 17.4.0
- **Input System**: 1.19.0
- **Unity Test Framework**: 1.6.0
- **Multiplayer Play Mode**: 2.0.2
- **Networking**: FishNet (`Assets/10_FishNet`)

## 프로젝트 사용 방식

ProjectRaidAuthority는 Unity client와 FishNet dedicated server를 분리해 검증한다. 클라이언트는 입력과 표시를 담당하고, 서버는 gameplay truth를 소유한다.

## 구현 전 확인해야 할 것

- FishNet server/client bootstrap 방식
- headless dedicated server 빌드 설정
- NetworkObject/NetworkBehaviour 사용 제약
- RPC/serialization 제약
- scene loading과 multi-client local test 방식
- Unity Test Framework에서 서버 권한 흐름을 검증하는 방법

## 현재 상태

이 문서는 버전 고정과 검증 체크리스트다. 실제 API 세부 내용은 smoke prototype 구현 중 로컬 패키지/공식 문서 확인 결과로 갱신한다.
