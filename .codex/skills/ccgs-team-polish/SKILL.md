---
name: ccgs-team-polish
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-polish workflow. Translates polish-team staffing and verification priorities into an OMX $team launch recommendation."
---

# CCGS Team Polish (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-polish/SKILL.md`

## Purpose

Preserve the Claude `team-polish` workflow's multi-discipline polish pass while using Codex/OMX team surfaces.

## Staffing translation

Claude intent centers on:
- performance-analyst
- technical-artist
- sound-designer
- qa-tester

Codex/OMX translation:
- performance/analysis lane
- presentation/polish lane
- QA/verification lane

## Recommended launch shape

Example launch hint:
- `$team 3:executor "polish pass: [feature/build area]"`

Recommended phase ordering:
1. baseline analysis and defects list
2. polish fixes and tuning
3. QA re-verification and sign-off

## Behavioral parity notes

- Preserve polish as a quality-focused pass, not a vague cleanup bucket.
- Keep performance and QA explicit.
- Require re-verification after polish changes.

## Verification guidance

A good result should:
- preserve analysis -> polish -> re-verify ordering
- keep QA explicit
- end with a clear status and remaining risks
