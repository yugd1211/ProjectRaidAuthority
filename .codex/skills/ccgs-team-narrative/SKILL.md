---
name: ccgs-team-narrative
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-narrative workflow. Translates narrative-team staffing and phase ordering into an OMX $team launch recommendation."
---

# CCGS Team Narrative (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-narrative/SKILL.md`

## Purpose

Preserve the Claude `team-narrative` workflow's narrative-focused staffing and sequencing while using Codex/OMX team surfaces.

## Staffing translation

Claude intent centers on:
- narrative-director
- writer
- world-builder
- level-designer
- related implementation / review support when needed

Codex/OMX translation:
- narrative/design lane
- writing/worldbuilding lane
- implementation/validation lane

## Recommended launch shape

Example launch hint:
- `$team 3:executor "narrative feature: [feature description]"`

Recommended phase ordering:
1. narrative framing and constraints
2. world / character / content development
3. implementation integration
4. review / QA / sign-off

## Behavioral parity notes

- Narrative intent and canon consistency should remain explicit throughout execution.
- Preserve the distinction between story structure work and implementation work.
- Surface lore/design conflicts early instead of letting implementation silently choose.

## Verification guidance

A good result should:
- preserve narrative-specific staffing intent
- keep canon / consistency concerns explicit
- end with a clear status and unresolved blockers if any
