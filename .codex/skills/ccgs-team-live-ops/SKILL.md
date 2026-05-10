---
name: ccgs-team-live-ops
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-live-ops workflow. Translates live-ops staffing and rollout concerns into an OMX $team launch recommendation."
---

# CCGS Team Live-Ops (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-live-ops/SKILL.md`

## Purpose

Preserve the Claude `team-live-ops` workflow's multi-role coordination for seasonal/event/live-economy work while using Codex/OMX team surfaces.

## Staffing translation

Claude intent centers on:
- live-ops-designer
- economy-designer
- community-manager
- analytics-engineer

Codex/OMX translation:
- live-ops/economy design lane
- rollout/comms lane
- analytics/verification lane

## Recommended launch shape

Example launch hint:
- `$team 3:executor "live-ops initiative: [event/season description]"`

Recommended phase ordering:
1. event/economy design framing
2. rollout/comms and implementation coordination
3. analytics/readiness verification
4. launch or blocked summary

## Behavioral parity notes

- Preserve ethics, economy, and player-communication concerns explicitly.
- Do not reduce live-ops work to only implementation tasks.
- Keep analytics as a distinct verification lens.

## Verification guidance

A good result should:
- preserve live-ops-specific staffing intent
- keep economy/comms/analytics visible
- end with a launch-readiness or blocker summary
