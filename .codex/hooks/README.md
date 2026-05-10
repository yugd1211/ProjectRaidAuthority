# Codex/OMX Hook Compatibility Surfaces

This directory contains **thin compatibility surfaces** for Claude-native CCGS hooks.

Source-of-truth behavior remains in:

- `.claude/settings.json`
- `.claude/hooks/*`
- `.claude/statusline.sh`
- `.claude/docs/hooks-reference/**`

## Current files

- `ccgs-hooks-compatibility.md` — event-to-surface behavior map
- `omx-native-hook-wrapper.sh` — portable wrapper that resolves the installed OMX native hook entrypoint
- `validate-commit-compatibility.sh` — Codex/OMX-compatible commit validation entrypoint
- `validate-push-compatibility.sh` — Codex/OMX-compatible push validation entrypoint
- `validate-assets-compatibility.sh` — Codex/OMX-compatible asset/data validation entrypoint
- `validate-skill-change-compatibility.sh` — Codex/OMX-compatible advisory entrypoint for skill changes
- `statusline-compatibility.sh` — Codex/OMX-compatible statusline renderer entrypoint

## Invocation contract

The validation scripts intentionally accept the same stdin JSON payload shape used by Claude's Bash PreToolUse hook events.

Example:

```bash
printf '{"tool_name":"Bash","tool_input":{"command":"git push origin main"}}' \
  | ./.codex/hooks/validate-push-compatibility.sh
```

## Current parity boundary

These scripts are **callable compatibility entrypoints**, and this repository now also includes a project-local `.codex/hooks.json` wiring file that targets them.
They exist so that:

1. the validation behavior has a concrete Codex/OMX surface
2. future hook/config wiring has stable targets to call
3. parity work can be verified without reusing Claude hook paths directly
4. commit/push/assets/skill-change checks can be exercised independently in temporary repos or test harnesses

- `.codex/hooks.json` — project-local native hook wiring that preserves OMX native hooks and layers CCGS compatibility checks on top

- `.codex/config.toml` — project-local Codex config enabling `codex_hooks` and built-in TUI status line for OMX/CCGS parity
