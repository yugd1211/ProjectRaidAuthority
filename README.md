# ProjectRaidAuthority

Unity + FishNet 기반의 멀티플레이 게임 개발 프로젝트입니다.
현재는 FishNet 룸/게임플레이 샘플을 기준으로 네트워크 구조를 잡고, 이후 실제 게임 시스템을 확장하는 단계입니다.

## 프로젝트 목표

- Unity에서 동작하는 멀티플레이 게임 제작
- FishNet 기반 Host/Client, Room, Player 동기화 구조 구축
- 외부 플러그인과 프로젝트 전용 코드를 분리한 유지보수 가능한 구조 유지
- 추후 서버, 매치메이킹, 캐릭터, 전투/액션, UI, 에셋을 단계적으로 확장

## 기술 스택

| 항목 | 내용 |
|---|---|
| Engine | Unity 6000.4.5f1 |
| Language | C# |
| Networking | FishNet |
| Transport | FishNet Transport 계열 |
| Test | Unity Test Framework |
| Target | 우선 PC/Editor 테스트, 이후 모바일/기타 플랫폼 검토 |

## 현재 구현 상태

- FishNet 패키지 import 완료
- FishNet 기반 샘플 씬 생성 완료
- Offline → Room → Gameplay 흐름 구성
- Room Player / Game Player 프리팹 구성
- 간단한 플레이어 이동 및 NetworkTransform 동기화 예제 구성
- 프로젝트 전용 코드와 FishNet 외부 플러그인 코드 분리 완료
- `00_`, `01_` 방식의 중요도/역할순 디렉토리 prefix 정리 완료

## Assets 구조

```text
Assets/
├─ 00_ProjectRaidAuthority/       # 프로젝트 전용 에셋/코드 루트
├─ 10_FishNet/                     # 외부 FishNet 플러그인, 직접 수정 지양
├─ 20_ThirdParty/                 # 수동 관리 외부 에셋 보관용
├─ 80_TutorialInfo/               # Unity 템플릿/튜토리얼 에셋
├─ 90_ScriptTemplates/            # Unity 스크립트 템플릿
└─ 99_Readme.asset                # Unity 템플릿 README 에셋
```

## 프로젝트 내부 구조

```text
Assets/00_ProjectRaidAuthority/
├─ 00_Scenes/                     # 프로젝트 씬
│  └─ 00_FishNetSample/            # FishNet 샘플 씬
├─ 01_Scripts/                    # 프로젝트 C# 코드
│  ├─ 00_Network/00_FishNetSample/ # FishNet 샘플 런타임 코드
│  └─ 90_Editor/                  # 에디터 빌더/검증 코드
├─ 02_Prefabs/                    # 프로젝트 프리팹
│  └─ 00_Network/00_FishNetSample/ # 네트워크 프리팹
├─ 03_Settings/                   # 입력/렌더링 등 프로젝트 설정 에셋
├─ 04_Tests/                      # EditMode / PlayMode 테스트
├─ 10_Art/                        # 스프라이트, 텍스처, 머티리얼, 모델
├─ 11_UI/                         # UI 에셋
├─ 12_Audio/                      # BGM/SFX
├─ 13_VFX/                        # 이펙트
├─ 20_Animations/                 # 애니메이션
└─ 30_ScriptableObjects/          # 데이터/설정 ScriptableObject
```

## FishNet 샘플 씬

| 씬 | 역할 |
|---|---|
| `FishNetOffline.unity` | Host/Client 시작용 오프라인 메뉴 씬 |
| `FishNetGameRoom.unity` | Room Player가 Ready 상태를 관리하는 룸 씬 |
| `FishNetGamePlay.unity` | Game Player가 생성되고 이동 동기화를 테스트하는 게임 씬 |

경로:

```text
Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetSample/
```

## 외부 플러그인 관리 원칙

- FishNet 같은 외부 플러그인은 `Assets/10_FishNet`처럼 vendor 폴더를 유지합니다.
- 프로젝트 전용 코드, 래퍼, 샘플, 게임 로직은 `Assets/00_ProjectRaidAuthority` 안에 작성합니다.
- 외부 플러그인 소스는 가능한 직접 수정하지 않습니다.
- Unity Package Manager로 관리 가능한 패키지는 `Packages/manifest.json`에서 관리합니다.
- 수동으로 추가하는 외부 에셋은 필요 시 `Assets/20_ThirdParty/<AssetName>`에 둡니다.

## 개발 규칙

- 게임 코드 네임스페이스는 `ProjectRaidAuthority` 기준으로 작성합니다.
- 네트워크 관련 코드는 `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network` 하위에 둡니다.
- 에디터 전용 코드는 `Assets/00_ProjectRaidAuthority/01_Scripts/90_Editor` 하위에 둡니다.
- 씬/프리팹/스크립트를 이동할 때는 `.meta` 파일을 함께 유지합니다.
- FishNet 플러그인 내부 코드를 수정하기보다 프로젝트 쪽에서 확장/래핑합니다.

## 다음 작업 TODO

- [ ] 실제 게임 장르와 핵심 플레이 루프 정의
- [ ] FishNet Room 흐름을 실제 게임 로비/매치 구조로 확장
- [ ] 서버 권한 구조 결정: Host 기반 / Dedicated Server / Hybrid
- [ ] 플레이어 데이터, 닉네임, 캐릭터 선택 구조 추가
- [ ] 네트워크 스폰/동기화 규칙 정리
- [ ] UI 흐름 작성: 메인 메뉴, 로비, 룸, 게임 결과
- [ ] EditMode / PlayMode 테스트 보강
- [ ] 빌드 타깃과 배포 파이프라인 결정

## 참고 문서

- `Assets/00_ProjectRaidAuthority/README.md` - 프로젝트 에셋 구조와 외부 플러그인 정책
- `Assets/20_ThirdParty/README.md` - 수동 외부 에셋 관리 정책
- `Assets/00_ProjectRaidAuthority/01_Scripts/90_Editor/FishNetSampleSceneBuilder.cs` - FishNet 샘플 씬 생성 코드
