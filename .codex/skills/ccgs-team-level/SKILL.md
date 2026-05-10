---
name: ccgs-team-level
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-level workflow. Translates level-team staffing and phase ordering into an OMX $team launch recommendation."
---

# CCGS Team Level (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-level/SKILL.md`

## Purpose

Preserve the Claude `team-level` workflow's cross-disciplinary level-design orchestration while using Codex/OMX team surfaces.

## Staffing translation

Claude intent centers on:
- level-designer
- narrative-director
- world-builder
- art-director
- systems-designer
- qa-tester

Codex/OMX translation:
- level/narrative design lane
- implementation/content lane
- QA/validation lane

## Recommended launch shape

Example launch hint:
- `$team 3:executor "level feature: [feature description]"`

Recommended phase ordering:
1. level goals and narrative/art constraints
2. encounter/system/content planning
3. implementation/integration
4. QA validation and sign-off

## Behavioral parity notes

- Preserve cross-discipline level constraints rather than treating the level as only content placement.
- Keep art and narrative inputs visible early.
- QA should remain a first-class lane because level work often hides integration bugs.

## Verification guidance

A good result should:
- preserve level-specific staffing intent
- keep cross-discipline constraints explicit
- end with a clear status, blockers, and next step
