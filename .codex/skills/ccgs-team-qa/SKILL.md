---
name: ccgs-team-qa
description: "Codex/OMX-compatible wrapper for the Claude CCGS /team-qa workflow. Translates QA planning/execution/sign-off into an OMX $team launch recommendation with a strong verification lane."
---

# CCGS Team QA (Codex/OMX Compatibility Skill)

Source of truth: `.claude/skills/team-qa/SKILL.md`

## Purpose

Preserve the Claude `team-qa` workflow's coordinated QA cycle using Codex/OMX team/runtime surfaces.

## Staffing translation

Claude intent centers on:
- qa-lead
- qa-tester
- gameplay-programmer or relevant implementer
- producer / coordination

Codex/OMX translation:
- test-strategy lane
- execution/evidence lane
- fix/triage lane

## Recommended launch shape

Example launch hint:
- `$team 3:executor "qa cycle: [feature/sprint/build]"`

Recommended phase ordering:
1. QA scope and plan
2. test execution and evidence capture
3. fix/triage for discovered issues
4. sign-off or blocked report

## Behavioral parity notes

- QA must remain a first-class lane, not folded into a generic “done” claim.
- Evidence and sign-off matter as much as bug discovery.
- Surface BLOCKED states immediately when release/sign-off is not justified.

## Verification guidance

A good result should:
- preserve evidence-driven QA flow
- separate findings from fixes
- end with explicit sign-off state or blocker report
