# CCGS Templates & Catalogs — Codex/OMX Compatibility Map

Source surfaces:

- `.claude/docs/workflow-catalog.yaml`
- `.claude/docs/skills-reference.md`
- `.claude/docs/agent-roster.md`
- `.claude/docs/agent-coordination-map.md`
- `.claude/docs/hooks-reference.md`
- `.claude/docs/rules-reference.md`
- `.claude/docs/templates/*`
- `.claude/docs/templates/collaborative-protocols/*`

## Compatibility posture

For templates and catalogs, the preferred Codex/OMX strategy is:

1. keep Claude files as the canonical source
2. reference/reuse them directly from Codex compatibility surfaces
3. add thin adapters only when Codex/OMX needs a different interaction shape
4. avoid copying template bodies unless a native Codex artifact truly requires a fork

This cluster therefore has stronger parity through **direct reuse** than some other clusters that require runtime adaptation.

## Catalog reuse mapping

| Claude source catalog | Current Codex/OMX reuse surface | Current parity |
|---|---|---|
| `.claude/docs/workflow-catalog.yaml` | `ccgs-help`, `ccgs-project-stage-detect`, `ccgs-adopt`, `ccgs-gate-check`, `AGENTS.md`, `docs/codex-omx-port-matrix.md` | Strong |
| `.claude/docs/skills-reference.md` | `AGENTS.md`, compatibility planning/docs, template/catalog mapping docs | Partial-to-strong |
| `.claude/docs/agent-roster.md` | role/agent compatibility planning, `.codex/agents/*`, team orchestration mapping docs | Partial-to-strong |
| `.claude/docs/agent-coordination-map.md` | team orchestration mapping docs, planning context | Partial |
| `.claude/docs/hooks-reference.md` | `.codex/hooks/ccgs-hooks-compatibility.md`, planning docs | Partial-to-strong |
| `.claude/docs/rules-reference.md` | `.codex/rules/ccgs-rules-path-compatibility.md` | Partial-to-strong |

## Template reuse mapping

| Claude template surface | Current Codex/OMX reuse strategy | Current parity |
|---|---|---|
| `.claude/docs/templates/architecture-decision-record.md` | referenced as canonical ADR template by `docs/CLAUDE.md` and preserved as source-of-truth | Strong |
| `.claude/docs/templates/accessibility-requirements.md` | referenced by gate/start/guidance docs as the canonical accessibility template | Strong |
| `.claude/docs/templates/post-mortem.md` | referenced by workflow docs as the canonical post-mortem template | Strong |
| `.claude/docs/templates/skill-test-spec.md` | remains canonical alongside `CCGS Skill Testing Framework/**` | Strong |
| broader `.claude/docs/templates/*` corpus | currently reused by reference rather than copied into `.codex/` | Partial-to-strong |
| collaborative protocol templates | preserved as Claude-native source material for future Codex/OMX wrapper work | Partial |

## Why reuse beats duplication here

Direct reuse avoids three classes of drift:

- template body drift
- catalog enumeration drift
- naming/ordering drift between Claude and Codex surfaces

This is especially important for:

- workflow phase ordering
- template-required sections
- agent roster / skill roster names
- audit/review artifacts that depend on stable path names

## Accepted deviations

- Codex/OMX does not yet expose dedicated repo-local wrappers for every reusable template/catalog artifact; many are intentionally consumed by reference.
- Some catalog docs (`skills-reference`, `agent-roster`, `agent-coordination-map`) currently inform planning/mapping more than end-user native Codex UX.
- Collaborative-protocol templates remain source material rather than active Codex-native protocol wrappers.

## Recommended next slices

1. Add repo-local wrapper docs only for the highest-value templates that need Codex-specific invocation guidance.
2. Capture a verification transcript that shows a Codex compatibility skill successfully using a Claude catalog/template source without copying it.
3. Add upstream-sync automation so template/catalog changes are re-diffed and reflected in the port matrix quickly.
