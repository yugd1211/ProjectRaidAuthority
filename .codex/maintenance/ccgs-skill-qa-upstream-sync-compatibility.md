# CCGS Skill QA & Upstream Sync — Codex/OMX Compatibility Map

Source surfaces:

- `.claude/skills/skill-test/SKILL.md`
- `.claude/skills/skill-improve/SKILL.md`
- `CCGS Skill Testing Framework/README.md`
- `CCGS Skill Testing Framework/catalog.yaml`
- `CCGS Skill Testing Framework/quality-rubric.md`
- `CCGS Skill Testing Framework/templates/skill-test-spec.md`
- `.claude/docs/templates/skill-test-spec.md`
- `UPGRADING.md`
- `.claude/docs/workflow-catalog.yaml`

Current Codex/OMX maintenance surfaces consulted:

- `.codex/hooks/validate-skill-change-compatibility.sh`
- `.codex/catalog/ccgs-templates-catalog-compatibility.md`
- `docs/codex-omx-port-matrix.md`
- `.omx/plans/prd-claude-to-omx-port-20260420T102525Z.md`
- `.omx/plans/test-spec-claude-to-omx-port-20260420T102525Z.md`

## Compatibility posture

This cluster currently has three layers:

1. **direct source reuse** for skill QA logic and testing framework artifacts
2. **compatibility guidance** in Codex surfaces (for example skill-change hook advice and the port matrix)
3. **planned future automation** for upstream diff -> classify -> port -> score parity

Codex/OMX does not yet ship dedicated repo-local wrappers such as `ccgs-skill-test` or `ccgs-sync-port`, but the canonical sources are preserved and already usable as references for maintenance work.

## Skill-QA mapping

| Claude source surface | Current Codex/OMX compatibility surface | Current parity |
|---|---|---|
| `.claude/skills/skill-test/SKILL.md` | direct source reuse + `.codex/hooks/validate-skill-change-compatibility.sh` guidance + maintenance mapping docs | Partial-to-strong |
| `.claude/skills/skill-improve/SKILL.md` | direct source reuse + maintenance mapping docs | Partial |
| `CCGS Skill Testing Framework/catalog.yaml` | canonical test inventory for future maintenance work; preserved by reference | Strong |
| `CCGS Skill Testing Framework/quality-rubric.md` | canonical category metrics for future maintenance work; preserved by reference | Strong |
| `CCGS Skill Testing Framework/templates/skill-test-spec.md` + `.claude/docs/templates/skill-test-spec.md` | canonical spec-template surfaces; preserved by reference | Strong |

## Upgrade / upstream-sync mapping

| Claude source surface | Current Codex/OMX compatibility surface | Current parity |
|---|---|---|
| `UPGRADING.md` | canonical upgrade/change-history source reused by maintenance planning docs | Strong |
| `.claude/docs/workflow-catalog.yaml` | used to anchor change classification by workflow cluster | Strong |
| port-matrix future sync section | `docs/codex-omx-port-matrix.md` | Partial-to-strong |
| upstream-sync plan/test criteria | `.omx/plans/prd-claude-to-omx-port-20260420T102525Z.md`, `.omx/plans/test-spec-claude-to-omx-port-20260420T102525Z.md` | Strong |

## Current maintenance loop (documented)

1. inspect upstream Claude changes (`UPGRADING.md`, git diff, changed `.claude/**` surfaces)
2. classify the change by cluster
3. identify impacted Codex/OMX compatibility surfaces
4. update `.codex/**` compatibility layers or docs
5. update `docs/codex-omx-port-matrix.md`
6. gather fresh verification evidence

## Accepted deviations

- There is not yet a dedicated repo-local Codex wrapper skill for `/skill-test`, `/skill-improve`, or upstream sync automation.
- Current parity depends on **documented runbook + direct source reuse**, not a finished native maintenance workflow.
- The strongest parity in this cluster is the preservation of the canonical test framework and upgrade ledger, not a new execution surface.

## Recommended next slices

1. Add a repo-local `ccgs-skill-qa` wrapper for the highest-value maintenance actions.
2. Add a repo-local `ccgs-sync-port` workflow that operationalizes the future upstream-sync section in the port matrix.
3. Capture a verification transcript for one real upstream-diff -> port-matrix-update cycle.
