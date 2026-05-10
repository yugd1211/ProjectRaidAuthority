---
name: ccgs-adopt
description: "Codex/OMX-compatible adaptation of the Claude CCGS /adopt workflow. Audits an existing project's artifacts for template-format compatibility and produces a prioritized migration plan."
---

# CCGS Adopt (Codex/OMX Compatibility Skill)

Use this skill when the user asks for brownfield onboarding, template adoption, migration planning, or whether existing artifacts will actually work with the CCGS workflow.

This is the Codex/OMX-compatible adaptation of `.claude/skills/adopt/SKILL.md`.

## Purpose

Audit **format compatibility**, not just artifact existence.

- `ccgs-project-stage-detect` answers: what exists?
- `ccgs-adopt` answers: will those artifacts work correctly with the workflow?

## Inputs to read

- `.claude/skills/adopt/SKILL.md`
- `.claude/docs/workflow-catalog.yaml`
- `production/stage.txt` if present
- `design/gdd/*.md`
- `design/gdd/systems-index.md`
- `docs/architecture/adr-*.md`
- `docs/architecture/control-manifest.md`
- `docs/architecture/tr-registry.yaml`
- `production/epics/**/*.md`
- `production/sprint-status.yaml`
- `.claude/docs/technical-preferences.md`
- `docs/engine-reference/**`
- prior `docs/adoption-plan-*.md` files if any

## Optional audit focus

If the user narrows scope, support the same high-level focus modes as the Claude source workflow:

- `full`
- `gdds`
- `adrs`
- `stories`
- `infra`

## Workflow

### Phase 1 — Detect project state

Gather evidence first and infer whether the project is truly brownfield:

- authoritative phase from `production/stage.txt` when present
- otherwise infer from artifacts using the same broad order as the Claude workflow
- count GDDs, ADRs, stories, and relevant infrastructure files
- detect whether engine configuration and engine reference docs exist
- note the latest prior adoption plan if present

If the project looks effectively fresh, do not continue the full migration audit. Redirect the user toward `ccgs-start` or a manual artifact-location check.

### Phase 2 — Audit format compatibility

Audit the artifact types in scope and classify concrete gaps.

#### GDD checks

For each GDD, inspect whether the Claude workflow's expected sections and status metadata exist:

- Overview
- Player Fantasy
- Detailed Rules / Detailed Design / Core Rules
- Formulas
- Edge Cases
- Dependencies
- Tuning Knobs
- Acceptance Criteria
- `**Status**:` header value

Also call out obvious placeholder-only sections.

#### ADR checks

For each ADR, verify the presence of the source workflow's key sections:

- `## Status`
- `## ADR Dependencies`
- `## Engine Compatibility`
- `## GDD Requirements Addressed`
- `## Performance Implications`

Treat missing `## Status` as blocking.

#### Systems index checks

If `design/gdd/systems-index.md` exists, inspect:

- parenthetical status values that break exact-match workflow logic
- invalid or non-canonical status values
- missing required columns such as system name, layer, priority, and status

#### Story checks

For each story file, verify whether it includes:

- readable status field
- acceptance-criteria checkbox list
- ADR reference
- TR-ID reference
- manifest version metadata

#### Infrastructure checks

Inspect the higher-leverage support artifacts:

- `docs/architecture/tr-registry.yaml`
- `docs/architecture/control-manifest.md`
- manifest version stamp in control manifest
- `production/sprint-status.yaml`
- `production/stage.txt`
- engine reference docs
- architecture traceability index

#### Technical preferences checks

Read `.claude/docs/technical-preferences.md` and flag unconfigured core fields such as engine, language, rendering, and physics.

### Phase 3 — Classify severity

Bucket findings into the same practical tiers as the Claude workflow:

- **BLOCKING** — workflow will malfunction or silently mis-evaluate now
- **HIGH** — unsafe or unreliable to continue core pipeline work
- **MEDIUM** — quality / tracking degradation
- **LOW** — advisory cleanup

If there are zero blocking and zero high-severity gaps, explicitly say the project appears template-compatible.

### Phase 4 — Build the migration plan

Produce a numbered, dependency-aware plan that preserves the source workflow's ordering logic:

1. blocking gaps first
2. high-priority infrastructure before downstream content fixes
3. medium gaps in dependency order (GDD -> ADR -> story)
4. low-priority cleanup last

For each item include:

- the problem
- exact file(s) affected
- canonical next workflow or manual fix path
- rough effort estimate
- checkbox-ready wording when writing a plan file

Preserve the Claude workflow's special handling for:

- parenthetical systems-index statuses
- ADRs missing `## Status`
- GDDs missing Acceptance Criteria
- infrastructure bootstrap ordering
- the warning not to regenerate in-progress or completed stories

### Phase 5 — Presentation and writing behavior

First present a compact in-chat summary:

- detected phase
- engine configured or not
- audit counts
- severity counts
- short gap preview

Then either:

- keep the result in chat, or
- write `docs/adoption-plan-[date].md` if the user explicitly wants the artifact

When writing, preserve the Claude workflow's structure and sequencing, but use a plain Codex confirmation question instead of Claude-specific widget UX.

## Behavioral parity notes

- This compatibility skill keeps the Claude workflow's **audit intent and ordering**, while replacing `AskUserQuestion` widgets with plain Codex confirmation / follow-up questions.
- Do not auto-rewrite existing GDDs, ADRs, or stories. Audit first, recommend second.
- Prefer additive fixes and retrofit paths over regeneration.

## Verification guidance

A good result should:

- distinguish existence gaps from format-compatibility gaps
- identify concrete blocking/high-risk problems
- produce a migration plan in dependency order
- preserve the source workflow's bias against silently rewriting existing work
