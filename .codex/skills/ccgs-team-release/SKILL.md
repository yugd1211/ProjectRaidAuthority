---
name: ccgs-team-release
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-release workflow. Translates release coordination, QA, deployment, and sign-off into an OMX $team launch recommendation."
---

# CCGS Team Release (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-release/SKILL.md`

## Purpose

Preserve the Claude `team-release` workflow's release-coordination intent while using Codex/OMX team/runtime surfaces.

## Staffing translation

Claude intent centers on:
- release-manager
- qa-lead
- devops-engineer
- producer

Codex/OMX translation:
- release coordination lane
- QA/sign-off lane
- build/deploy lane

## Recommended launch shape

Example launch hint:
- `$team 3:executor "release candidate: [build/version]"`

Recommended phase ordering:
1. release candidate review
2. QA regression and sign-off evidence
3. build/deploy readiness
4. go / no-go summary

## Behavioral parity notes

- Release orchestration must preserve explicit no-go / blocked paths.
- QA evidence and deployment readiness both need independent attention.
- Avoid collapsing release into a generic “ship it” workflow with no sign-off owner.

## Verification guidance

A good result should:
- preserve explicit release roles
- separate QA from deployment readiness
- end with a go/no-go style recommendation and concrete blockers if any
