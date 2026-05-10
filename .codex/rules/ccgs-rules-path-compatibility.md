# CCGS Rules & Path Standards — Codex/OMX Compatibility Map

Source surfaces:

- `.claude/rules/*`
- `.claude/docs/rules-reference.md`
- root `CLAUDE.md`
- scoped `CLAUDE.md` files:
  - `docs/CLAUDE.md`
  - `design/CLAUDE.md`
  - `src/CLAUDE.md`
  - `CCGS Skill Testing Framework/CLAUDE.md`

## Compatibility posture

Claude-native CCGS uses two overlapping enforcement layers:

1. **path-specific rule files** under `.claude/rules/`
2. **scoped directory instructions** via nested `CLAUDE.md`

Current Codex/OMX parity is achieved primarily through:

- repo-wide `AGENTS.md`
- AGENTS-required reading of scoped `CLAUDE.md` before edits in `docs/**`, `design/**`, `src/**`, and `CCGS Skill Testing Framework/**`
- compatibility hook entrypoints under `.codex/hooks/` for commit/push/asset/skill validation
- documentation that maps Claude rule intent to current Codex/OMX enforcement surfaces

## Rule-file to Codex/OMX mapping

| Claude rule file | Source path pattern | Current Codex/OMX enforcement surface | Current parity |
|---|---|---|---|
| `.claude/rules/gameplay-code.md` | `src/gameplay/**` | root `CLAUDE.md`, `src/CLAUDE.md`, AGENTS verification rules, commit hook hardcoded-value checks | Partial-to-strong |
| `.claude/rules/engine-code.md` | `src/core/**` | root `CLAUDE.md`, `src/CLAUDE.md`, AGENTS requirement to check pinned engine refs | Partial |
| `.claude/rules/ai-code.md` | `src/ai/**` | root `CLAUDE.md`, `src/CLAUDE.md`, general code-review/verification discipline | Partial |
| `.claude/rules/network-code.md` | `src/networking/**` | root `CLAUDE.md`, `src/CLAUDE.md`, AGENTS verification and security review posture | Partial |
| `.claude/rules/ui-code.md` | `src/ui/**` | root `CLAUDE.md`, `src/CLAUDE.md`, UX/accessibility guidance via workflow docs | Partial |
| `.claude/rules/design-docs.md` | `design/gdd/**` | `design/CLAUDE.md`, `docs/CLAUDE.md`, AGENTS required-reading and verification rules, commit/design checks | Strong |
| `.claude/rules/narrative.md` | `design/narrative/**` | root `CLAUDE.md`, `design/CLAUDE.md`, workflow-led design/narrative review norms | Partial |
| `.claude/rules/data-files.md` | `assets/data/**` | `.codex/hooks/validate-assets-compatibility.sh` + AGENTS verification | Partial-to-strong |
| `.claude/rules/test-standards.md` | `tests/**` | root `CLAUDE.md`, `src/CLAUDE.md`, AGENTS verify-before-complete requirement | Partial |
| `.claude/rules/prototype-code.md` | `prototypes/**` | workflow guidance in `docs/WORKFLOW-GUIDE.md` + AGENTS verification discipline | Partial |
| `.claude/rules/shader-code.md` | `assets/shaders/**` | root `CLAUDE.md` engine-specialist routing + manual review discipline | Partial |

## Scoped directory standards mapping

| Scoped Claude file | Covered subtree | Current Codex/OMX compatibility behavior |
|---|---|---|
| `docs/CLAUDE.md` | `docs/**` | Must be read before doc edits; architecture docs and engine-reference guidance remain source-of-truth |
| `design/CLAUDE.md` | `design/**` | Must be read before design edits; preserves 8-section GDD contract and design workflow order |
| `src/CLAUDE.md` | `src/**` | Must be read before code edits; preserves engine-reference checks, data-driven values, and verification-driven development |
| `CCGS Skill Testing Framework/CLAUDE.md` | `CCGS Skill Testing Framework/**` | Must be read before testing-framework edits; preserves catalog/spec/rubric path conventions |

## Accepted deviations

- Claude path rules are not yet re-expressed as first-class Codex-native path matchers; current parity relies on AGENTS routing, scoped `CLAUDE.md`, and targeted hook entrypoints.
- Some specialized rule files (AI, networking, shaders, prototypes) are currently represented as review/behavior expectations rather than executable Codex validations.
- The strongest current parity is around design-doc standards, scoped directory guidance, and asset/data validation.

## Recommended next slices

1. Add dedicated Codex-native validations for code-path standards that are currently only documented.
2. Decide whether path-specific rule enforcement should live in hook config, review skills, or both.
3. Capture a verification transcript showing scoped `CLAUDE.md` + hook enforcement working together on representative paths.
