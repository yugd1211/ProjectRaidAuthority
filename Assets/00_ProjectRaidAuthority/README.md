# ProjectRaidAuthority 에셋 구조

`Assets/00_ProjectRaidAuthority`는 프로젝트 전용 에셋/코드 루트입니다.
게임플레이 코드, 씬, 프리팹, 아트, 오디오, UI, 설정, 테스트는 기본적으로 이 폴더 아래에 둡니다.
단, Unity가 특정 폴더명을 요구하는 경우에는 Unity 규칙을 우선합니다.

## Prefix 규칙

- `00_` ~ `09_`: 핵심 개발/런타임 자산
- `10_` ~ `19_`: 콘텐츠 제작 자산
- `20_` ~ `39_`: 데이터/보조 자산
- `90_`: 에디터 전용 또는 낮은 우선순위 보조 폴더

## 프로젝트 전용 폴더

| 경로 | 용도 |
|---|---|
| `00_Scenes/` | 프로젝트 씬 |
| `00_Scenes/00_FishNetNetworkFlow/` | FishNet Network Flow 씬 |
| `01_Scripts/` | 프로젝트 C# 코드 |
| `01_Scripts/00_Network/` | 네트워크/서버/멀티플레이 관련 코드 |
| `02_Prefabs/` | 재사용 GameObject 프리팹 |
| `02_Prefabs/00_Network/` | 네트워크 프리팹 |
| `03_Settings/` | 프로젝트 설정 에셋 |
| `03_Settings/00_Input/` | 입력 설정 |
| `03_Settings/01_Rendering/` | 렌더링/URP 설정 |
| `04_Tests/` | EditMode / PlayMode 테스트 |
| `10_Art/` | 스프라이트, 텍스처, 머티리얼, 모델 |
| `11_UI/` | UI 폰트, 아이콘, UI 전용 에셋 |
| `12_Audio/` | BGM/SFX |
| `13_VFX/` | 파티클, VFX, 이펙트 전용 셰이더 |
| `20_Animations/` | 애니메이션 클립/컨트롤러 |
| `30_ScriptableObjects/` | 데이터/설정 ScriptableObject |

## Unity 템플릿에서 이동된 프로젝트 자산

- 기본 씬: `Assets/00_ProjectRaidAuthority/00_Scenes/SampleScene.unity`
- 입력 액션: `Assets/00_ProjectRaidAuthority/03_Settings/00_Input/InputSystem_Actions.inputactions`
- URP/렌더링 설정: `Assets/00_ProjectRaidAuthority/03_Settings/01_Rendering`

## 외부 패키지/플러그인 정책

- Unity Package Manager 의존성은 루트 `Packages/manifest.json`에서 관리합니다.
- `Packages/packages-lock.json`은 패키지 버전 고정을 위해 유지합니다.
- 외부 vendor/plugin 루트는 `Assets/00_ProjectRaidAuthority` 밖에 둡니다.
  - 현재 FishNet은 `Assets/10_FishNet`에 있습니다.
- 수동 관리 외부 에셋은 `Assets/20_ThirdParty/<VendorOrAssetName>`에 둘 수 있습니다.
- 외부 플러그인 원본은 가능한 직접 수정하지 않습니다.
- 프로젝트 전용 어댑터, 래퍼, Network Flow 씬, 게임별 통합 코드는 `Assets/00_ProjectRaidAuthority` 아래에 작성합니다.
- `Editor`, `Resources`, `StreamingAssets`, `Plugins`, `Gizmos`, `Editor Default Resources`처럼 Unity가 예약한 폴더명은 Unity 규칙을 우선합니다.
- 파일/폴더를 이동할 때는 `.meta` 파일도 함께 이동해 GUID 참조를 유지합니다.

## 현재 FishNet Network Flow 위치

- Scenes: `Assets/00_ProjectRaidAuthority/00_Scenes/00_FishNetNetworkFlow`
- Prefabs: `Assets/00_ProjectRaidAuthority/02_Prefabs/00_Network/00_FishNetNetworkFlow`
- Runtime scripts: `Assets/00_ProjectRaidAuthority/01_Scripts/00_Network/00_FishNetNetworkFlow`

FishNet 플러그인 자체는 `Assets/10_FishNet`에 있으며, third-party/vendor 코드로 취급합니다.
