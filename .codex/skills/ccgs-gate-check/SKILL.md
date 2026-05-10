---
name: ccgs-gate-check
description: "Codex/OMX-compatible adaptation of the Claude CCGS /gate-check workflow. Produces an advisory PASS / CONCERNS / FAIL readiness verdict for a phase transition with explicit blockers and next actions."
---

# CCGS Gate Check (Codex/OMX Compatibility Skill)

Use this skill when the user asks whether the project is ready to advance to the next phase or wants a formal readiness check.

This is the Codex/OMX-compatible adaptation of `.claude/skills/gate-check/SKILL.md`.

## Purpose

Produce a **formal, advisory** gate verdict:

- `ccgs-project-stage-detect` answers: where are we?
- `ccgs-gate-check` answers: are we ready to advance?

## Inputs to read

- `.claude/skills/gate-check/SKILL.md`
- `.claude/docs/workflow-catalog.yaml`
- `production/stage.txt` if present
- `production/review-mode.txt` if present
- `docs/consistency-failures.md` if present
- phase-relevant artifacts under `design/`, `docs/architecture/`, `production/`, `prototypes/`, `src/`, and `tests/`
- any recent review or playtest reports that materially affect the target gate

## Inputs / arguments

Support the same practical inputs as the Claude source workflow:

- explicit target phase (`systems-design`, `technical-setup`, `pre-production`, `production`, `polish`, `release`)
- optional review mode override (`full`, `lean`, `solo`)

If no target phase is supplied, detect the current stage first and ask the user to confirm the inferred transition before continuing.

## Workflow

### Phase 1 — Resolve the target gate

Use this order:

1. explicit target phase from the user
2. otherwise detect the current stage from `production/stage.txt` or artifact evidence
3. confirm the inferred transition before issuing a verdict

### Phase 2 — Resolve review mode

Use this order:

1. explicit review mode override
2. `production/review-mode.txt`
3. default `lean`

Surface the resolved review mode in the report so the user understands what level of scrutiny was applied.

### Phase 3 — Run artifact and quality checks

For the target transition, reuse the Claude workflow's practical checks:

- required artifact existence
- meaningful content, not empty stubs
- quality and coherence checks
- code / test / report evidence where relevant
- cross-reference checks between design, architecture, production, and implementation

Do not auto-pass unverifiable items. Mark them as manual checks.

### Phase 4 — Manual-check handling

When something cannot be verified from repo evidence alone, ask concise follow-up questions instead of assuming success. Examples include:

- whether the core loop is genuinely fun
- whether informal playtests occurred without a saved report
- whether a human has validated ambiguous readiness items

### Phase 5 — Director / specialist review lane

Higher-parity mode may include additional specialist review perspectives mirroring the Claude source workflow's director panel.

In Codex/OMX compatibility mode:

- use those extra review lanes only when delegation is explicitly allowed or the runtime supports it
- otherwise perform a single-agent gate analysis and clearly label that the multi-director panel was not run
- treat this as an accepted parity gap, not a hidden omission

### Phase 6 — Verdict and chain-of-verification

Output an advisory report with:

- target transition
- date and review mode
- required artifacts status
- quality check status
- explicit blockers
- recommended next actions
- final verdict: `PASS`, `CONCERNS`, or `FAIL`

Before finalizing, challenge the draft verdict with a short chain-of-verification pass:

- identify the weakest evidence
- re-check assumptions that could overturn the verdict
- revise the verdict if the re-check changes the conclusion

### Phase 7 — Stage update behavior

If the verdict is `PASS`, do **not** auto-advance the project state.
Only write `production/stage.txt` after the user explicitly confirms they want to advance.

### Phase 8 — Handoff

After the verdict, recommend the single most logical next workflow or remediation step.
Never auto-run it.

## Behavioral parity notes

- The core gate rubric, advisory verdict shape, and manual-check posture are preserved.
- Claude's director-panel behavior becomes an optional higher-parity lane in Codex/OMX rather than a default requirement.
- Stage advancement still requires explicit user confirmation before writing `production/stage.txt`.

## Verification guidance

A good result should:

- correctly identify the target gate
- show concrete evidence for pass/fail reasoning
- separate blockers from recommendations
- preserve advisory, user-decides semantics
- make any missing multi-director review evidence explicit
