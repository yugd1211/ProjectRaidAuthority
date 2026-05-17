# Unity MCP 세션 운영 정책

상태: 활성  
작성일: 2026-05-17  
적용 범위: ProjectRaidAuthority의 Codex/OMX 기반 Unity Editor 확인 작업

## 2026-05-16 기본 경로 변경

Unity AI Assistant 공식 MCP는 구독/credits 및 승인 게이트에 묶여 있어 기본 경로에서 제외했다. 현재 기본 MCP 경로는 CoplayDev 오픈소스 **MCP for Unity**다.

- Unity 패키지: `com.coplaydev.unity-mcp`
- Codex MCP 이름: `mcp_for_unity`
- Codex transport: streamable HTTP
- 기본 endpoint: `http://localhost:8080/mcp`
- WSL 서버 기동 명령:

```bash
uvx --from mcpforunityserver mcp-for-unity --transport http --http-url http://127.0.0.1:8080 --project-scoped-tools
```

기존 `.omx/unity-mcp` direct broker와 Unity AI `relay_win.exe --mcp` 경로는 롤백/참고용으로만 남긴다. 새 작업에서는 기존 `unity-mcp` 이름을 재사용하지 말고 `mcp_for_unity`를 사용한다.

## 배경

Unity MCP 승인 화면에서 연결이 `Accepted`로 보이더라도, Codex/OMX가 다른 세션 또는 새로 생성된 보조 세션에서 Unity MCP tool을 호출하면 Unity가 해당 연결을 새 승인 대상으로 재분류할 수 있다. 이 경우 `Connection revoked`가 반복되고, 사용자가 Allow를 다시 켜도 다음 호출에서 다시 꺼지는 루프가 발생할 수 있다.

이번 프로젝트에서는 `relay_win.exe`, `wsl.exe`, Unity named pipe 연결이 장기 승인 상태와 묶여 있으므로 Unity MCP 호출 주체를 하나로 고정한다.

## 결정

Unity Editor 자체를 확인하는 모든 Unity MCP tool 호출은 반드시 **현재 사용자 대화를 오케스트레이션하는 루트 OMX 세션**에서만 수행한다.

루트 OMX 세션이란 다음 조건을 만족하는 세션이다.

- 사용자와 직접 대화하는 leader/orchestrator 세션이다.
- 현재 작업의 `.omx/state/sessions/<session-id>/` 상태를 소유한다.
- Unity MCP 승인 상태를 확인하고 실패/재시작 여부를 최종 판단하는 단일 책임자다.

## 금지 사항

다음 실행 주체는 Unity MCP tool을 직접 호출하지 않는다.

- Codex native subagent
- OMX worker/team pane
- 새로 만든 보조 Codex 세션
- MPPM clone 또는 테스트 인스턴스 확인용 별도 세션
- Unity MCP 승인 문제를 우회하기 위해 임시로 만든 세션

금지되는 호출 예시는 다음과 같다.

- `mcp__unity_mcp__.Unity_GetConsoleLogs`
- `mcp__unity_mcp__.Unity_RunCommand`
- `mcp__unity_mcp__.Unity_Camera_Capture`
- `mcp__unity_mcp__.Unity_SceneView_Capture2DScene`
- 기타 Unity Editor 상태를 직접 조회/변경하는 Unity MCP tool

## 허용되는 대체 흐름

보조 에이전트나 워커가 Unity 확인이 필요하다고 판단하면 다음처럼 처리한다.

1. 직접 Unity MCP tool을 호출하지 않는다.
2. 필요한 검증 목적, 호출 후보, 예상 위험을 루트 세션에 보고한다.
3. 루트 세션이 `.omx/unity-mcp/umcp status`와 `.omx/unity-mcp/umcp logs`를 먼저 확인한다.
4. 루트 세션만 필요한 최소 Unity MCP 호출을 1회 단위로 수행한다.
5. 실패하면 같은 보조 세션을 새로 만들지 않고, 루트 세션에서 실패 로그와 다음 조치를 판단한다.

## 재시작 정책

Unity MCP 승인 또는 연결이 불안정할 때는 새 세션 생성으로 해결하려고 하지 않는다.

- `.omx/unity-mcp/umcp`, `broker.py`, `relay_win.exe --mcp`는 장기 연결 승인과 묶여 있으므로 평소에는 중지/재시작하지 않는다.
- Unity MCP 호출 실패 시 먼저 `.omx/unity-mcp/umcp status`, `.omx/unity-mcp/umcp logs`를 확인한다.
- Direct MCP 재시작은 Unity가 연결을 새 승인 대상으로 분류할 수 있으므로 사용자가 명시적으로 요청할 때만 수행한다.

## 운영 체크리스트

Unity MCP로 Unity 자체를 확인하기 전 루트 세션은 다음을 확인한다.

- 현재 호출자가 사용자-facing 루트 OMX 세션인가?
- 보조 에이전트나 새 세션에서 대신 호출하려는 상태는 아닌가?
- Unity MCP 승인 화면에서 대상 연결이 현재 루트 세션의 `wsl.exe`/`relay_win.exe` 조합인가?
- 실패 직후 반복 호출이 아니라 `status`/`logs` 확인을 먼저 했는가?
- MPPM 테스트가 목적이라면 MCP 없이 Unity Editor에서 직접 Play로 확인할 수 있는가?

## 기대 효과

- Unity MCP approval이 세션별로 흔들리는 상황을 줄인다.
- `Connection revoked` 재현 루프를 피한다.
- Unity 확인 책임이 루트 OMX 세션으로 모여 로그와 판단이 분산되지 않는다.
