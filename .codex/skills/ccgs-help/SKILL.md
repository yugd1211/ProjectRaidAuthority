---
name: ccgs-help
description: "Codex/OMX-compatible replacement for the Claude CCGS /help workflow. Reads the Claude workflow catalog and current project artifacts, then tells the user the next required step."
---

# CCGS Help (Codex/OMX Compatibility Skill)

Use this skill when the user asks any variant of:

- what should I do next?
- where am I in the workflow?
- what comes after this?
- I am stuck / lost / not sure what to do

This is the Codex/OMX-compatible adaptation of `.claude/skills/help/SKILL.md`.

## Purpose

Provide a lightweight, read-only orientation report based on the existing Claude-native workflow assets, without rewriting them.

## Inputs to read

1. `.claude/docs/workflow-catalog.yaml`
2. `production/stage.txt` if present
3. `production/session-state/active.md` if present
4. Relevant project artifacts referenced by the current phase
5. Installed workflow surfaces under:
   - `.claude/skills/*/SKILL.md`
   - `.codex/skills/*/SKILL.md`

## Workflow

### Phase 1 — Read the authoritative workflow catalog

Read `.claude/docs/workflow-catalog.yaml`.
Treat it as the canonical definition of:
- phases
- required vs optional steps
- repeatable steps
- artifact checks
- next-phase progression

### Phase 2 — Determine the current phase

Use this order:

1. `production/stage.txt` if it exists and maps clearly to a catalog phase
2. otherwise infer from artifacts, using the most advanced matching phase

Preferred inference order:
- substantial `src/` implementation -> `production`
- `production/epics/` or story artifacts -> `pre-production`
- `docs/architecture/adr-*.md` -> `technical-setup`
- `design/gdd/systems-index.md` -> `systems-design`
- `design/gdd/game-concept.md` -> `concept`
- otherwise `concept`

### Phase 3 — Read active session context

If `production/session-state/active.md` exists, extract:
- most recently worked-on task
- in-progress item
- blockers or open questions
- epic / feature / task breadcrumb if present

Surface this briefly at the top of the response.

### Phase 4 — Check completion of steps in the current phase

For each step in the current phase:
- if the catalog provides an artifact glob, check for matching files
- if it provides `min_count`, enforce it
- if it provides an artifact pattern, verify the pattern exists in the file
- if only a note exists, mark the step as manual / not auto-detectable
- if no artifact metadata exists, mark it as unknown or ongoing

For repeatable steps, do not overclaim completion; explain detected evidence and note that more work may still remain.

### Phase 5 — Find the next required action

Compute:
1. last confirmed completed required step
2. first incomplete required step -> this is the main next action
3. optional steps available now
4. upcoming required steps after the blocker

If the user explicitly says they just finished a step, use that as a tie-breaker when artifact evidence is ambiguous.

### Phase 6 — Show uncataloged but relevant skills

Compare:
- catalog command entries from `.claude/docs/workflow-catalog.yaml`
- available Claude-native skill folders in `.claude/skills/*`
- available Codex skills in `.codex/skills/*`

If there are skills not represented in the catalog, show only the most relevant ones as an `Also available` footer.

### Phase 7 — Output format

Keep the answer concise and operational.
Use this structure when possible:

```md
## Where You Are: [Phase]

**In progress:** [active task, if any]

### Done
- ...

### Next up (required)
**[step name]** — [why it is next]
Command / workflow: `[recommended skill or action]`

### Also available (optional)
- ...

### Coming up after that
- ...
```

Formatting rules:
- show only one primary required next step
- distinguish confirmed evidence from inference
- if a step is manual-only, say so plainly
- if the user sounds confused, add a short escalation footer with deeper analysis options

## Behavioral parity notes

This skill intentionally preserves the Claude `/help` workflow's purpose while adapting the interaction style to Codex/OMX.
Equivalent outcome matters more than identical UI mechanics.

## Verification guidance

To verify parity against Claude `/help`, compare whether both surfaces:
- identify the same current phase
- identify the same next required step
- surface the same major blocker or in-progress item
- recommend substantially similar next actions
