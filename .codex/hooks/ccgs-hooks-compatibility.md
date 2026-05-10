# CCGS Hooks & Automation — Codex/OMX Compatibility Map

Source surfaces:

- `.claude/settings.json`
- `.claude/hooks/*`
- `.claude/statusline.sh`
- `.claude/docs/hooks-reference.md`
- `.claude/docs/hooks-reference/**`

Codex/OMX compatibility intent:

- preserve the **behavioral purpose** of Claude-native hooks
- reuse existing OMX runtime/state surfaces where possible
- document explicit parity gaps instead of hiding them
- prefer additive compatibility layers over re-implementing every shell hook immediately

## Current compatibility posture

This repository now ships executable Codex-native compatibility scripts under `.codex/hooks/` plus a project-local `.codex/hooks.json` wiring file.
The present compatibility layer is a combination of:

- repo-wide `AGENTS.md` operating/verification rules
- OMX runtime state and automation surfaces under `.omx/`
- Codex/OMX workflow skills (`ccgs-help`, `ccgs-project-stage-detect`, `ccgs-adopt`, `ccgs-gate-check`, `ccgs-start`)
- `.codex/hooks.json`, which wires OMX-native hooks plus CCGS compatibility checks at project scope
- this mapping document, which records where Claude hook intent currently lands in Codex/OMX

## Event / behavior mapping

| Claude event / hook | Source behavior | Current Codex/OMX compatibility surface | Current parity |
|---|---|---|---|
| `SessionStart` -> `session-start.sh` | Load session/project context at startup | OMX session state (`.omx/state/session.json`), runtime overlay in `AGENTS.md`, existing project context files | Partial |
| `SessionStart` -> `detect-gaps.sh` | Detect fresh-project / missing-artifact gaps and suggest next workflow | `ccgs-start`, `ccgs-help`, `ccgs-project-stage-detect`, `ccgs-adopt` | Strong |
| `PreToolUse` -> `validate-commit.sh` | Validate commit-time safety and content quality | `.codex/hooks/validate-commit-compatibility.sh` + repo `AGENTS.md` verification rules | Partial-to-strong |
| `PreToolUse` -> `validate-push.sh` | Warn on risky/protected pushes | `.codex/hooks/validate-push-compatibility.sh` as a callable compatibility entrypoint; full hook wiring still pending | Partial |
| `PostToolUse` -> `validate-assets.sh` | Validate assets after writes | `.codex/hooks/validate-assets-compatibility.sh` as a callable compatibility entrypoint | Partial |
| `PostToolUse` -> `validate-skill-change.sh` | Advise testing after skill edits | `.codex/hooks/validate-skill-change-compatibility.sh` + skill-focused review discipline | Partial |
| `Notification` -> `notify.sh` | User/system notification delivery | OMX notify fallback runtime (`.omx/state/notify-fallback-state.json`, `.omx/state/notify-fallback.pid`, notify logs) | Strong |
| `PreCompact` -> `pre-compact.sh` | Preserve critical session state before compaction | AGENTS compaction protocol, `.omx/notepad.md`, OMX state tools, context snapshots under `.omx/context/` | Strong |
| `PostCompact` -> `post-compact.sh` | Remind/restore session state after compaction | AGENTS compaction protocol + persistent OMX state/notepad artifacts | Strong |
| `Stop` -> `session-stop.sh` | Clean stop and session summary | OMX stop-state tracking (`.omx/state/native-stop-state.json`) and mode cleanup via state tools / `cancel` skill | Partial |
| `SubagentStart` -> `log-agent.sh` | Audit subagent start | OMX subagent tracking (`.omx/state/subagent-tracking.json`) | Strong |
| `SubagentStop` -> `log-agent-stop.sh` | Audit subagent stop | OMX subagent tracking (`.omx/state/subagent-tracking.json`) | Strong |
| `statusLine.command` -> `.claude/statusline.sh` | Show context %, model, stage, and breadcrumb | `.codex/hooks/statusline-compatibility.sh` + OMX HUD/status runtime (`.omx/hud-config.json`, session `hud-state.json`) + repo phase artifacts | Partial-to-strong |

## Statusline / HUD notes

Claude's statusline script computes:

- context usage
- model name
- project stage
- optional production breadcrumb from `production/session-state/active.md`

Current Codex/OMX parity is **partial**:

- OMX already maintains HUD configuration/state
- project phase artifacts like `production/stage.txt` remain reusable
- a callable Codex/OMX statusline renderer now exists at `.codex/hooks/statusline-compatibility.sh`, and project-local `.codex/config.toml` now enables the built-in TUI status line; exact custom renderer wiring still remains separate from the built-in status items

## Accepted deviations

- Claude shell hooks are not being mirrored 1:1 yet; current parity is achieved through OMX runtime state, AGENTS guardrails, and explicit workflow skills.
- Some Claude behaviors that were event-driven are currently **operator- or workflow-driven** in Codex/OMX.
- Notification, compaction, and subagent-audit behaviors already have stronger OMX-native counterparts than the original shell-hook pattern.
- Commit, push, asset, and skill-change validation now have callable compatibility entrypoints and a project-local hook wiring file, but runtime adoption/merge semantics should still be verified in a real Codex session.

## Recommended next implementation slices

1. Verify real-session adoption/merge semantics for `.codex/hooks.json` and `.codex/config.toml` alongside user-scope Codex config.
2. Decide whether the custom statusline compatibility script should remain documentation-only or be invoked through a future native statusline surface.
3. Capture a verification transcript showing validation + notification + compaction + subagent tracking parity in practice.
4. Decide whether any checks should stay advisory-only versus blocking in Codex/OMX runtime.
