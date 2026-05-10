---
name: ccgs-team-combat
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-combat workflow. Translates combat-team staffing and phase ordering into an OMX $team launch recommendation plus verification lane expectations."
---

# CCGS Team Combat (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-combat/SKILL.md`

## Purpose

Preserve the Claude `team-combat` workflow's staffing and phased execution intent while using Codex/OMX team surfaces.

## Staffing translation

Claude intent:
- game-designer
- gameplay-programmer
- ai-programmer
- technical-artist
- sound-designer
- engine specialist
- qa-tester

Codex/OMX translation:
- use OMX `$team` for durable parallel lanes when the work is large
- keep one verification lane active
- use repo role compatibility docs to preserve domain ownership

## Recommended launch shape

For substantial combat work, prefer a team run with:
- implementation lane
- engine/architecture validation lane
- QA/verification lane

Example launch hint:
- `$team 3:executor "combat feature: [feature description]"`

Then staff/brief the lanes according to the Claude source workflow's phases:
1. design grounding
2. architecture + engine validation
3. implementation
4. integration
5. QA validation
6. sign-off

## Behavioral parity notes

- Preserve combat-specific staffing and QA expectations.
- Do not collapse QA into implementation-only work.
- Escalate cross-domain conflicts upward instead of letting workers freeload across domains.

## Verification guidance

A good result should:
- preserve combat-specific staffing intent
- keep a verification lane
- surface blockers explicitly
- end with a clear COMPLETE / BLOCKED style status
