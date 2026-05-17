# Codex/OMX 호환 운영 지침 — ProjectRaidAuthority

이 저장소는 Claude Code Game Studios 템플릿을 원본으로 사용하고, Codex/OMX가 같은 흐름을 실행할 수 있도록 얇은 호환 계층을 더한 프로젝트다.

## 최우선 언어 규칙

- 앞으로 사용자와의 모든 대화는 한국어로 작성한다.
- 새로 작성하거나 수정하는 Markdown 문서, 문서 주석, 설계 메모, 운영 기록은 한국어로 작성한다.
- Unity, FishNet, URP, API 이름, 파일 경로, 클래스명, 명령어, 로그 원문처럼 고유명사나 코드 식별자는 원문을 유지할 수 있다.
- 영어 원문을 보존해야 하는 외부 인용, 패키지명, 프로토콜명은 번역하지 말고 설명만 한국어로 붙인다.

## 원본 보존 규칙

- `CLAUDE.md`, `.claude/**`, 범위별 `*/CLAUDE.md`는 Claude-native 워크플로의 원천 자료다.
- 가능한 한 원본을 덮어쓰기보다 참조와 적응을 우선한다.
- 사용자가 명시적으로 요청하지 않은 한 Claude 원본 파일을 삭제하거나 조용히 교체하지 않는다.
- OMX 실행 상태는 `.omx/` 안에만 둔다. 제품 문서나 게임 콘텐츠로 취급하지 않는다.

## Codex/OMX 포팅 표면

Claude-native 동작을 Codex/OMX로 옮길 때 우선순위는 다음과 같다.

1. `AGENTS.md` — 저장소 전체 오케스트레이션과 라우팅 지침
2. `.codex/skills/` — 슬래시 명령/워크플로 대응
3. `.codex/agents/` — 에이전트 역할 대응
4. Codex/OMX hook/config — 자동화와 검증
5. `docs/codex-omx-port-matrix.md` — 포팅 현황, 차이, 남은 작업 기록

## 편집 전 필수 확인

아래 영역을 편집할 때는 해당 범위의 Claude 지침을 먼저 읽는다.

- `docs/**` → `docs/CLAUDE.md`
- `design/**` → `design/CLAUDE.md`
- `src/**` → `src/CLAUDE.md`
- `CCGS Skill Testing Framework/**` → `CCGS Skill Testing Framework/CLAUDE.md`

여러 영역을 동시에 건드리면 모든 관련 지침을 만족해야 한다.

## 실행 방식

- 사용자가 “현재 무엇을 해야 하나?”라고 묻는 경우, `.codex/skills/ccgs-help/SKILL.md`를 우선 사용한다.
- brownfield/포팅 작업은 `docs/codex-omx-port-matrix.md`에 상태와 차이를 기록한다.
- 완성 판정은 UX가 완전히 같다는 뜻이 아니라 실무 결과가 동등한지로 판단한다.
- 진행 보고는 결과 우선으로 짧게 작성한다: 목표 결과, 제약, 검증 증거, 멈춘 이유.

## Unity MCP 운영 안전선

- `.omx/unity-mcp/umcp`, `broker.py`, `relay_win.exe --mcp`는 장기 연결 승인과 묶여 있으므로 평소에는 중지/재시작하지 않는다.
- Unity MCP 호출이 실패하면 먼저 `.omx/unity-mcp/umcp status`, `.omx/unity-mcp/umcp logs`로 상태를 확인한다.
- Direct MCP 재시작은 Unity가 연결을 새로 분류해 권한/용량 문제가 생길 수 있으므로 사용자가 명시적으로 요청할 때만 수행한다.
- Unity Editor 자체를 확인하는 MCP tool 호출(`mcp__unity_mcp__.*`, 예: `Unity_GetConsoleLogs`, `Unity_RunCommand`, 화면 캡처 등)은 반드시 현재 사용자 대화를 오케스트레이션하는 **루트 OMX 세션**에서만 수행한다.
- Codex native subagent, OMX worker/team pane, 새로 만든 보조 Codex 세션, MPPM clone 확인용 세션은 Unity MCP tool을 직접 호출하지 않는다. 필요한 경우 루트 세션에 “Unity MCP 확인 요청”을 보고하고 루트가 단일 연결에서 실행한다.
- Unity MCP 승인이 흔들릴 때는 새 세션을 만들어 재시도하지 않는다. 세션 재생성은 Unity가 연결을 새 앱/새 승인 대상으로 분류해 `Connection revoked` 루프를 만들 수 있으므로, 루트 세션에서만 상태/로그를 확인하고 사용자가 명시적으로 허가할 때만 재시작한다.

## 포팅 완료 기준

포팅된 기능은 다음 항목에서 약 90% 이상의 실무 동등성을 보여야 완료로 본다.

- 트리거/발견 가능성
- 실제 동작
- 안전/거버넌스
- 작업자 사용성
- 검증 증거

## 변경 전략

- 얇은 호환 계층부터 시작한다.
- 대표 기능 하나를 끝까지 검증한 뒤 확장한다.
- 되돌릴 수 있고 리뷰 가능한 변경을 선호한다.
- 허용한 차이는 숨기지 말고 기록한다.

## 현재 다음 작업

현재 프로젝트는 사전 제작 / 프로토타입 준비 상태다. 다음 필수 작업은 실제 Unity/FishNet 서버 권한 스모크 프로토타입 구현과 검증이다. 프로토타입이 실제로 검증되기 전에는 `/prototype 완료` 또는 `prototypes/*/README.md` 완료 아티팩트를 만들지 않는다.

## 보고 규칙

실질적인 포팅/문서 변경을 할 때는 다음을 보고한다.

- 참고한 Claude 원본 파일
- 추가/수정한 Codex/OMX 표면
- 현재 동등성 추정
- 남은 차이와 다음 권장 작업
