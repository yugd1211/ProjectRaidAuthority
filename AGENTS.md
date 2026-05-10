# Codex/OMX Compatibility Overlay for Claude Code Game Studios

This repository's original operating system is Claude-native.
`CLAUDE.md`, `.claude/`, and the scoped `*/CLAUDE.md` files remain the source of truth for the game-studio workflow, agent roster, hooks, rules, and slash-command behaviors.

This `AGENTS.md` adds a **Codex/OMX compatibility layer** so Codex can interpret and port those behaviors without rewriting the original Claude assets.

## Primary goals

1. Preserve the upstream Claude template as-is whenever possible.
2. Add Codex/OMX-native surfaces under `AGENTS.md`, `.codex/`, and additive docs.
3. Port Claude behaviors one capability at a time and validate practical parity.
4. Keep future upstream sync work tractable by minimizing duplicate logic.

## Source-of-truth rules

- Treat `CLAUDE.md` and `.claude/**` as authoritative for existing behavior.
- Prefer **reference + adaptation** over copy-paste rewrites.
- Do not delete or silently replace Claude-native files unless explicitly asked.
- Keep OMX runtime/session state inside `.omx/` only; it is local workflow state, not product content.

## Porting surfaces

When translating a Claude-native behavior into Codex/OMX, prefer this mapping order:

1. `AGENTS.md` for repo-wide orchestration and routing guidance
2. `.codex/skills/` for slash-command/workflow equivalents
3. `.codex/agents/` for agent-role equivalents
4. Codex/OMX hook/config surfaces for automation and validation behavior
5. `docs/codex-omx-port-matrix.md` for parity tracking, gaps, and status

## Required reading before edits

Before editing files in these areas, read the scoped Claude instructions first:

- `docs/**` -> `docs/CLAUDE.md`
- `design/**` -> `design/CLAUDE.md`
- `src/**` -> `src/CLAUDE.md`
- `CCGS Skill Testing Framework/**` -> `CCGS Skill Testing Framework/CLAUDE.md`

If a task touches multiple scoped areas, satisfy all applicable scoped `CLAUDE.md` files.

## Codex/OMX execution guidance

- If a user asks for the current game-studio workflow, inspect `.claude/docs/workflow-catalog.yaml`, `.claude/docs/skills-reference.md`, and the relevant skill files.
- If a user asks "what should I do next?", prefer the Codex-compatible help skill in `.codex/skills/ccgs-help/SKILL.md` once available.
- For brownfield/parity work, update `docs/codex-omx-port-matrix.md` as capabilities are analyzed or ported.
- Validate parity by practical effect, not exact UX duplication. Equivalent outcome is acceptable when Codex/OMX uses a different native interaction surface.

## Unity MCP operational guardrail

- Unity MCP broker/relay approval is tied to the long-lived connection. Do **not** stop, kill, or restart `.omx/unity-mcp/umcp`, `broker.py`, or `relay_win.exe --mcp` during normal work.
- If Unity MCP tool calls fail, inspect status/logs first with `.omx/unity-mcp/umcp status` and `.omx/unity-mcp/umcp logs`; do not run stop/start as a reflex.
- Restarting Direct MCP can cause Unity to reclassify the connection and revoke/CapacityLimit it. Only restart when the user explicitly requests it, and use the explicit override flags documented by `.omx/unity-mcp/umcp`.

## Parity standard

A ported capability should not be considered complete until it reaches roughly **90% practical parity** across:

- trigger/discovery parity
- behavioral parity
- safety/governance parity
- operator ergonomics
- verification evidence

Use `docs/codex-omx-port-matrix.md` as the stable in-repo ledger for this work.

## Change strategy

- Start with thin compatibility layers.
- Pilot one representative capability end-to-end before scaling a cluster.
- Prefer reversible, reviewable additions.
- Record accepted deviations instead of hiding them.

## Current pilot target

The first Codex/OMX pilot capability is the Claude `/help` workflow:

- source behavior: `.claude/skills/help/SKILL.md`
- supporting catalog: `.claude/docs/workflow-catalog.yaml`
- Codex target: `.codex/skills/ccgs-help/SKILL.md`

## Commit and reporting expectations

When making substantive porting changes:

- list the Claude source files consulted
- list the Codex/OMX target surfaces added or updated
- state the current parity estimate
- note remaining gaps and the next recommended slice
