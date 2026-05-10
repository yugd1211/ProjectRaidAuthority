---
name: ccgs-team-ui
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-ui workflow. Translates UI/UX team staffing and gates into an OMX $team launch recommendation."
---

# CCGS Team UI (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-ui/SKILL.md`

## Purpose

Preserve the Claude `team-ui` workflow's UX-first and accessibility-aware team orchestration while using Codex/OMX team surfaces.

## Staffing translation

Claude intent centers on:
- ux-designer
- ui-programmer
- art-director
- accessibility-specialist
- QA validation

Codex/OMX translation:
- one UX/spec lane
- one UI implementation lane
- one validation/accessibility lane

## Recommended launch shape

Example launch hint:
- `$team 3:executor "ui feature: [feature description]"`

Recommended phase ordering:
1. UX/spec grounding
2. accessibility/design validation
3. UI implementation
4. integration
5. QA/accessibility verification

## Behavioral parity notes

- UX validation should remain a hard upstream concern, not an afterthought.
- Accessibility requirements stay in scope through verification.
- Preserve the source workflow's bias against skipping design review before implementation.

## Verification guidance

A good result should:
- keep UX and accessibility explicit
- preserve design-before-implementation ordering
- end with a clear status and next action
