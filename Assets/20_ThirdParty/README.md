# ThirdParty

수동으로 관리하는 외부 에셋을 보관하는 선택적 폴더입니다.
Unity Package Manager로 설치하기 어렵거나, 자체 vendor 폴더를 유지하지 않는 외부 에셋을 이곳에 둘 수 있습니다.

## 기본 원칙

- Unity Package Manager로 설치 가능한 패키지는 루트의 `Packages/manifest.json`과 `Packages/packages-lock.json`에서 관리합니다.
- Mirror처럼 자체 폴더를 가진 Asset Store/vendor 플러그인은 기존 vendor 위치를 유지합니다.
  - 예: `Assets/10_Mirror`
- 프로젝트 전용 코드, 래퍼, 샘플, 게임 로직은 이 폴더가 아니라 `Assets/00_ProjectRaidAuthority` 아래에 작성합니다.
- 외부 플러그인/에셋 원본은 가능한 직접 수정하지 않습니다.
- 불가피하게 vendor 코드를 수정했다면 수정 이유와 범위를 별도 문서나 커밋 메시지에 남깁니다.

## 사용 예시

```text
Assets/
├─ 00_ProjectRaidAuthority/       # 우리 프로젝트 전용 코드와 에셋
├─ 10_Mirror/                     # Mirror 외부 플러그인
└─ 20_ThirdParty/                 # 수동 관리 외부 에셋
   └─ SomeExternalAsset/
```

## 이 폴더에 넣는 경우

- UPM으로 설치할 수 없는 외부 에셋
- vendor가 명확하지만 import 시 고정 폴더가 없는 에셋
- 직접 다운로드한 사운드, 폰트, 임시 테스트 에셋
- 추후 교체 가능성이 있는 외부 리소스

## 이 폴더에 넣지 않는 경우

- 프로젝트 고유 코드
- 게임플레이 스크립트
- Mirror 연동 코드
- 프로젝트 씬/프리팹
- Unity Package Manager로 관리되는 패키지

## 주의사항

- 에셋을 이동할 때는 `.meta` 파일도 함께 이동해야 Unity GUID 참조가 유지됩니다.
- 외부 에셋과 프로젝트 전용 에셋을 섞지 마세요.
- 외부 에셋을 실제 게임에 사용하기 전에는 라이선스와 사용 범위를 확인하세요.
