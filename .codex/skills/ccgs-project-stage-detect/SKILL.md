---
name: ccgs-project-stage-detect
description: "Codex/OMX-compatible adaptation of the Claude CCGS /project-stage-detect workflow. Performs a broader project audit than ccgs-help and identifies stage, major gaps, and next actions."
---

# CCGS Project Stage Detect (Codex/OMX Compatibility Skill)

Use this skill when the user asks questions such as:
- where are we in development?
- what stage is this project in?
- give me a full project audit
- what is missing before the next milestone?

This is the Codex/OMX-compatible adaptation of `.claude/skills/project-stage-detect/SKILL.md`.

## Purpose

Produce a broader diagnostic than `ccgs-help` by scanning design, code, architecture, production, prototype, and test artifacts, then classifying the current stage and surfacing the most important gaps.

## Inputs to read

- `.claude/skills/project-stage-detect/SKILL.md`
- `.claude/docs/workflow-catalog.yaml`
- `production/stage.txt` if present
- `design/**`
- `src/**`
- `docs/architecture/**`
- `production/**`
- `prototypes/**`
- `tests/**`

## Optional role focus

If the user supplies a role filter, bias the recommendations toward that role:
- `programmer`
- `designer`
- `producer`
- otherwise `general`

## Workflow

### Phase 1 — Scan key project surfaces

Gather high-signal facts only.

#### Design
- count GDD files in `design/gdd/`
- check for `game-concept.md`, `systems-index.md`, and other foundational docs
- note whether design structure looks sparse, partial, or substantial

#### Source
- count source files and identify major source directories
- note whether the repo is mostly template, prototype, or active implementation

#### Architecture
- count ADRs or architecture docs in `docs/architecture/`
- note missing overview or missing decision records

#### Production
- check for sprint plans, milestones, epics, release docs, and session state

#### Prototypes
- count prototype directories and whether they are documented

#### Tests
- count test files if the `tests/` tree exists
- call out missing or very light test structure

### Phase 2 — Determine current stage

Use this order:

1. `production/stage.txt` if present and trustworthy
2. otherwise infer using the same broad heuristics as the Claude skill:
   - `concept`
   - `systems-design`
   - `technical-setup`
   - `pre-production`
   - `production`
   - `polish` (explicit only)
   - `release` (explicit only)

When inference is ambiguous, say so explicitly and distinguish evidence from inference.

### Phase 3 — Identify gaps collaboratively

Do not just dump a missing-file list.
Frame gaps as meaningful development questions, for example:
- code exists but no design docs for that area
- design exists but no systems index
- architecture decisions exist but no overview
- work appears active but no production tracking artifacts exist
- prototypes exist but are undocumented

Prefer 3-7 high-value gaps over exhaustive noise.

### Phase 4 — Produce a concise stage report

Use a structure like:

```md
# Project Stage Analysis

**Stage:** [stage]
**Confidence:** [high / medium / low]

## Evidence
- ...

## Gaps
1. ...
2. ...

## Recommended next steps
1. ...
2. ...
3. ...
```

If the user asked for a role-specific analysis, tailor the gaps and next steps accordingly.

### Phase 5 — Writing behavior

By default, provide the analysis in-chat.
Only write a report file if the user explicitly wants an artifact written.
If writing is requested, a suitable default output path is:
- `production/project-stage-report.md`

## Relationship to other skills

- Use `ccgs-help` for lightweight "what next?" orientation.
- Use this skill for deeper stage audits and gap analysis.
- Common follow-ups include:
  - workflow decomposition / systems mapping
  - reverse-documentation of code into design or architecture docs
  - sprint planning or milestone review

## Verification guidance

A good result should:
- correctly classify the project stage or explain ambiguity
- identify the highest-value missing artifacts
- recommend the next few actions in priority order
- stay concise enough to drive action
