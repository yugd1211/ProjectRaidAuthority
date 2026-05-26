# Claude Code Game Studios 템플릿 업그레이드 메모

이 파일은 상위 Claude Code Game Studios 템플릿을 새 버전으로 가져올 때 참고하는 요약 문서다. ProjectRaidAuthority는 이미 Unity/FishNet 기준으로 설정되어 있으므로, 템플릿을 갱신할 때도 프로젝트 문서와 한국어 언어 규칙을 우선 보존한다.

## 업그레이드 전 원칙

- `AGENTS.md`, `CLAUDE.md`, `.claude/docs/technical-preferences.md`, `design/**`, `docs/**`, `production/**`의 프로젝트 결정 내용을 먼저 백업하거나 diff로 확인한다.
- 새 대화와 Markdown 문서/주석은 한국어로 유지한다.
- 상위 템플릿 변경을 가져오더라도 Unity/FishNet authoritative server 방향을 덮어쓰지 않는다.
- 프로토타입 완료 증거 없이 `prototypes/*/README.md`를 만들지 않는다.

## 추천 방식

상위 템플릿을 git remote로 등록한 뒤 필요한 변경만 병합한다.

```bash
git remote add template https://github.com/Donchitos/Claude-Code-Game-Studios.git
git fetch template main
git merge template/main --allow-unrelated-histories
```

충돌이 나면 다음 순서로 판단한다.

1. ProjectRaidAuthority의 게임/기술 결정 유지
2. 한국어 문서 규칙 유지
3. Claude/CCGS 구조 개선은 가능한 범위에서 수용
4. Codex/OMX 호환 문서는 `docs/codex-omx-port-matrix.md`에 차이를 기록

## 특히 조심할 파일

- `CLAUDE.md`
- `AGENTS.md`
- `.claude/docs/technical-preferences.md`
- `docs/CLAUDE.md`
- `design/CLAUDE.md`
- `.codex/skills/**`
- `.claude/skills/**`

## 수동 복사 방식

zip 다운로드나 수동 복사를 사용할 경우, 상위 템플릿 파일을 바로 덮어쓰지 말고 별도 디렉터리에 받은 뒤 필요한 부분만 비교 병합한다.

## 현재 프로젝트 우선 결정

- Unity 6000.4.5f1
- URP 17.4.0
- FishNet authoritative dedicated server
- Broken Toy Raid Punk 아트 방향
- Pre-Production / Prototype Readiness 단계
- 다음 필수 작업: 실제 FishNet authority smoke prototype 구현
