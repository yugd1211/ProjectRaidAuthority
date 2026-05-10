---
name: ccgs-team-audio
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-audio workflow. Translates audio-team staffing and phase ordering into an OMX $team launch recommendation."
---

# CCGS Team Audio (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-audio/SKILL.md`

## Purpose

Preserve the Claude `team-audio` workflow's audio-direction and implementation intent while using Codex/OMX team surfaces.

## Staffing translation

Claude intent centers on:
- audio-director
- sound-designer
- technical-artist
- gameplay-programmer

Codex/OMX translation:
- audio direction lane
- implementation/integration lane
- validation/polish lane

## Recommended launch shape

Example launch hint:
- `$team 3:executor "audio feature: [feature description]"`

Recommended phase ordering:
1. audio direction and event plan
2. implementation / integration
3. polish / validation

## Behavioral parity notes

- Preserve the difference between audio direction and concrete implementation tasks.
- Keep integration with gameplay and technical-art concerns visible.
- Do not drop validation just because audio work can feel subjective.

## Verification guidance

A good result should:
- preserve explicit audio ownership
- keep implementation and polish distinct
- end with a clear status and next action
