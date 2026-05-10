---
name: ccgs-start
description: "Codex/OMX-compatible adaptation of the Claude CCGS /start workflow. Handles first-time onboarding, asks where the user is starting from, and routes them to the right next workflow without assumptions."
---

# CCGS Start (Codex/OMX Compatibility Skill)

Use this skill when the user is new to the workflow, asks how to get started, or needs onboarding without assumptions.

This is the Codex/OMX-compatible adaptation of `.claude/skills/start/SKILL.md`.

## Purpose

Orient the user, identify where they are starting from, and recommend the right next workflow.
Do not auto-run the next step.

## Inputs to read

- `.claude/skills/start/SKILL.md`
- `.claude/docs/technical-preferences.md`
- `design/gdd/game-concept.md` if present
- `design/gdd/` overview state
- `src/` implementation footprint
- `prototypes/` contents if present
- production planning artifacts if present
- `production/review-mode.txt` if present

## Workflow

### Phase 1 — Detect project state silently

Before asking the user anything, gather enough context to tailor onboarding:

- whether the engine is configured
- whether a game concept exists
- whether source code already exists
- whether prototypes exist
- whether design docs exist
- whether production artifacts exist

Use these findings to validate and refine the user's self-assessment.

### Phase 2 — Ask where the user is starting from

Ask one concise onboarding question that maps to the same four branches as the Claude workflow:

1. no idea yet
2. vague idea
3. clear concept
4. existing work

In Codex App / plain Codex sessions, use a short plain-text question or compact numbered options instead of Claude-specific question widgets.

### Phase 3 — Route to the right next workflow

Preserve the source workflow's branch logic:

- **No idea yet** -> guide toward brainstorm / concept formation
- **Vague idea** -> guide toward structured brainstorming around the hint
- **Clear concept** -> offer formalize-first vs jump-into-setup paths
- **Existing work** -> distinguish between early-stage setup gaps and true brownfield adoption

For existing work, prefer the same sequence as the Claude workflow:

1. `ccgs-project-stage-detect` for stage / gap inventory
2. `ccgs-adopt` for format-compatibility audit
3. setup or retrofit workflows as needed

When downstream workflows are not yet ported to Codex/OMX, reference the canonical Claude workflow name or the relevant source skill path plainly instead of pretending a native Codex equivalent already exists.

### Phase 3b — Review mode setup

If `production/review-mode.txt` does not exist, ask the user to choose a review mode:

- `full`
- `lean`
- `solo`

After the user chooses, write `production/review-mode.txt` as the direct consequence of that choice.
If it already exists, surface the current mode briefly and do not ask again.

### Phase 4 — Confirm the first next step

After presenting the recommended path, ask which first step the user wants.
Keep it to one primary recommendation plus an alternate path.
Never auto-run the chosen workflow.

### Phase 5 — Hand off cleanly

Once the user picks the next step, hand off with a concise instruction that names the next workflow or artifact to open.

## Behavioral parity notes

- The onboarding branch logic is preserved.
- Claude widget prompts are replaced by concise Codex questions.
- Review-mode persistence is preserved via the same `production/review-mode.txt` artifact.
- The skill remains orienting and non-destructive: it routes, but does not auto-execute downstream work.

## Verification guidance

A good result should:

- correctly recognize whether the repo looks fresh or brownfield
- ask the right onboarding question before routing
- recommend the right next workflow for the user's starting state
- preserve explicit handoff instead of auto-running the next step
