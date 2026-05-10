# Claude Code Game Studios -> Codex/OMX Port Matrix

This document tracks how Claude-native CCGS behaviors are being ported into Codex/OMX-compatible surfaces while preserving the upstream Claude files.

## Porting principles

1. **Preserve source assets** — `CLAUDE.md` and `.claude/**` remain authoritative.
2. **Add compatibility layers** — prefer `AGENTS.md`, `.codex/skills`, `.codex/agents`, and OMX state over destructive edits.
3. **Port by capability cluster** — do not translate everything at once.
4. **Score parity explicitly** — a capability is complete only when it reaches >= 0.90 practical parity.
5. **Document accepted deviations** — different native UX is acceptable if outcome and safety remain equivalent.

## Parity rubric

| Criterion | Weight | Question |
|---|---:|---|
| Trigger parity | 0.20 | Can the user discover and invoke it naturally in Codex/OMX? |
| Behavioral parity | 0.35 | Does the workflow achieve the same practical outcome? |
| Safety / governance parity | 0.20 | Are approvals, rules, guardrails, and audit expectations preserved? |
| Operator ergonomics | 0.15 | Is it maintainable, understandable, and pleasant to use? |
| Verification evidence | 0.10 | Is there a repeatable way to prove the port works? |

**Completion threshold:** total score >= **0.90**.

## Capability clusters

| Cluster | Claude source surface | Codex/OMX target surface | Status | Notes |
|---|---|---|---|---|
| Governance | `CLAUDE.md`, scoped `*/CLAUDE.md` | `AGENTS.md`, additive scoped overlays if needed | In progress | Root `AGENTS.md` added as compatibility overlay |
| Workflow routing | `.claude/skills/*`, workflow catalog | `.codex/skills/*`, AGENTS routing guidance | In progress | `ccgs-help`, `ccgs-project-stage-detect`, `ccgs-adopt`, `ccgs-gate-check`, and `ccgs-start` compatibility skills added |
| Agent system | `.claude/agents/*.md` | `.codex/agents/*`, OMX-native roles | In progress | Initial compatibility roles added for `technical-director`, `producer`, `lead-programmer`, and `gameplay-programmer` |
| Team orchestration | `team-*` skills, Task/subagent orchestration | OMX team / Codex subagents / orchestration skills | In progress | Team orchestration compatibility map added at `.codex/team/ccgs-team-orchestration-compatibility.md` |
| Hooks & automation | `.claude/settings.json`, `.claude/hooks/*`, `statusline.sh` | Codex hooks, OMX state/memory/notify surfaces | In progress | Hook/automation behavior map plus project-local `.codex/hooks.json` wiring added; OMX runtime surfaces already cover notify/compaction/subagent tracking partially or strongly |
| Rules & path standards | `.claude/rules/*` | AGENTS directives, scoped `CLAUDE.md`, review skills, hook checks | In progress | Rules/path compatibility map added at `.codex/rules/ccgs-rules-path-compatibility.md` |
| Templates & catalogs | `.claude/docs/templates/*`, workflow catalog | Reuse existing docs directly where possible | In progress | Template/catalog compatibility map added at `.codex/catalog/ccgs-templates-catalog-compatibility.md`; current strategy prefers direct reuse over duplication |
| Skill QA & upstream sync | `skill-test`, `skill-improve`, upgrade flows | Future Codex/OMX test/sync skills | In progress | Skill-QA/upstream-sync compatibility map added at `.codex/maintenance/ccgs-skill-qa-upstream-sync-compatibility.md` |

## Pilot capability: Claude `/help`

### Source behavior

- Skill: `.claude/skills/help/SKILL.md`
- Catalog: `.claude/docs/workflow-catalog.yaml`
- Supporting state/artifacts:
  - `production/stage.txt`
  - `production/session-state/active.md`
  - project artifact globs under `design/`, `docs/`, `production/`, and `src/`

### Codex/OMX target

- Skill: `.codex/skills/ccgs-help/SKILL.md`
- Governance surface: `AGENTS.md`
- Verification method: compare the Codex result against the Claude skill instructions on the same repo state

### Initial mapping

| Claude element | Codex/OMX adaptation |
|---|---|
| `/help` slash command | local Codex skill `ccgs-help` |
| workflow-catalog driven phase detection | reuse `.claude/docs/workflow-catalog.yaml` directly |
| lightweight read-only orientation | same behavior in Codex skill |
| uncataloged skill footer | inspect `.claude/skills/*` and `.codex/skills/*` |
| AskUserQuestion | plain Codex response unless/ until richer prompt UI is added |

### Accepted initial deviations

- Claude's exact slash-command UI is replaced by Codex skill invocation / AGENTS routing.
- No Claude-specific `AskUserQuestion` UI requirement for the initial pilot.

### Pilot verification checklist

- [x] Read workflow catalog successfully
- [x] Determine current phase from `production/stage.txt` or artifact inference
- [x] Surface current/active task from `production/session-state/active.md` when present
- [x] Identify the next required step
- [x] List optional steps when relevant
- [x] List relevant uncataloged skills
- [x] Produce a concise, user-facing next-step report
- [x] Score parity against rubric

### Current pilot parity scorecard (working estimate)

| Criterion | Weight | Score | Notes |
|---|---:|---:|---|
| Trigger parity | 0.20 | 0.18 | `ccgs-help` is available as a named Codex skill and routed by the overlay docs. |
| Behavioral parity | 0.35 | 0.31 | The Codex skill preserves the same read-only orientation flow, phase detection inputs, and next-step framing. |
| Safety / governance parity | 0.20 | 0.17 | The skill stays advisory and non-destructive, but Claude widget UX is replaced by plain Codex questioning. |
| Operator ergonomics | 0.15 | 0.13 | The compatibility surface is lightweight and readable, but downstream ports are still incomplete. |
| Verification evidence | 0.10 | 0.09 | Mapping, checklist, and a repo-state-based `/help` parity transcript now exist at `.omx/wiki/help-parity-transcript-2026-04-21.md`. |

**Current total:** `0.88 / 1.00` -> **not yet at the 0.90 completion threshold**.

**Primary remaining gaps for `/help`:**
- Validate that Codex output matches Claude guidance on the same repo state in a live interactive run, not just a repo-state transcript.
- Finish more adjacent workflow ports so the recommendations point to native Codex/OMX surfaces more often.
- Optionally add a second transcript for a non-empty project state (for example with active sprint/session artifacts present).
- Validate a project-scope auth strategy so live `codex exec` / interactive runs can use the local `.codex/` wiring without hitting 401.

## Current workflow-routing coverage

| Claude source skill | Codex/OMX compatibility surface | Status | Notes |
|---|---|---|---|
| `/help` | `.codex/skills/ccgs-help/SKILL.md` | Added | Pilot compatibility surface |
| `/project-stage-detect` | `.codex/skills/ccgs-project-stage-detect/SKILL.md` | Added | Broader audit / stage analysis |
| `/adopt` | `.codex/skills/ccgs-adopt/SKILL.md` | Added | Brownfield format-compatibility audit + migration plan guidance |
| `/gate-check` | `.codex/skills/ccgs-gate-check/SKILL.md` | Added | Advisory phase-transition readiness verdict |
| `/start` | `.codex/skills/ccgs-start/SKILL.md` | Added | First-time onboarding and workflow routing |

**Accepted workflow-routing deviations (current state)**

- Claude `AskUserQuestion` widgets are replaced by concise Codex questions or runtime-native prompt UI when available.
- Director-panel / multi-reviewer flows are treated as optional higher-parity lanes until explicit delegation/runtime support is available.
- Downstream recommendations may still reference canonical Claude workflow names when a native Codex/OMX compatibility skill has not yet been added.

## Current agent-system coverage

| Claude source agent | Codex/OMX compatibility surface | Status | Notes |
|---|---|---|---|
| `technical-director` | `.codex/agents/technical-director.md` | Added | Strategy / architecture / risk framing |
| `producer` | `.codex/agents/producer.md` | Added | Planning / sequencing / scope coordination |
| `lead-programmer` | `.codex/agents/lead-programmer.md` | Added | Code-level architecture and review framing |
| `gameplay-programmer` | `.codex/agents/gameplay-programmer.md` | Added | Gameplay implementation discipline and escalation paths |

**Accepted agent-system deviations (current state)**

- The first Codex/OMX agent pass is intentionally thin: it captures role intent, responsibilities, and output conventions before deeper runtime/tool parity work.
- Claude-specific widget flows are described as behavior, but Codex/OMX execution follows repo-wide AGENTS autonomy and escalation rules.
- Wider specialist coverage across the full Claude roster is still pending.

## Current hooks & automation coverage

| Claude source surface | Codex/OMX compatibility surface | Status | Notes |
|---|---|---|---|
| `.claude/settings.json` hook registrations | `.codex/hooks/ccgs-hooks-compatibility.md`, `.codex/hooks/README.md`, `.codex/hooks.json`, `.codex/config.toml` | In progress | Documents event-to-surface mapping and adds project-local native hook + config wiring |
| `.claude/hooks/session-start.sh` + `detect-gaps.sh` | OMX session state + `ccgs-start` / `ccgs-help` / `ccgs-project-stage-detect` / `ccgs-adopt` | Partial-to-strong | Workflow intent largely preserved |
| `.claude/hooks/notify.sh` | OMX notify fallback runtime under `.omx/state/notify-fallback-*` | Strong | Runtime-backed parity already exists |
| `.claude/hooks/pre-compact.sh` + `post-compact.sh` | AGENTS compaction protocol + `.omx/notepad.md` + OMX state/context artifacts | Strong | Behavior preserved through OMX-native persistence |
| `.claude/hooks/log-agent*.sh` | `.omx/state/subagent-tracking.json` | Strong | Subagent audit trail already present |
| `.claude/statusline.sh` | `.codex/hooks/statusline-compatibility.sh` + `.codex/config.toml` + OMX HUD/tmux runtime + phase artifacts | In progress | Callable renderer plus project-local TUI status line config added; exact custom renderer wiring still pending |
| commit/push/asset/skill validation hooks | `.codex/hooks/validate-commit-compatibility.sh`, `.codex/hooks/validate-push-compatibility.sh`, `.codex/hooks/validate-assets-compatibility.sh`, `.codex/hooks/validate-skill-change-compatibility.sh`, plus `.codex/hooks.json` and AGENTS rules | In progress | Callable validation entrypoints plus project-local hook wiring added; real-session adoption semantics still need verification |

**Accepted hooks & automation deviations (current state)**

- Hook parity is currently documented as a behavior map, not a 1:1 shell-hook reimplementation.
- Event-driven Claude checks may currently land as AGENTS rules, OMX runtime behavior, or workflow-driven checks in Codex/OMX.
- Notification, compaction, and subagent auditing already map well to OMX-native runtime/state surfaces.
- Commit, push, asset, and skill-change validation now have callable compatibility entrypoints plus project-local hook wiring, and project-local statusline config is present. Real-session adoption semantics are partially clarified: `CODEX_HOME=.codex` loads project-local config/skills, but a live `codex exec` hit `401 Unauthorized`, indicating auth is also scoped to `CODEX_HOME` and needs a project-scope strategy for higher parity.

## Current rules & path-standards coverage

| Claude source surface | Codex/OMX compatibility surface | Status | Notes |
|---|---|---|---|
| `.claude/rules/*` path rules | `.codex/rules/ccgs-rules-path-compatibility.md` | Added | Documents rule-to-path mapping and current enforcement surfaces |
| `.claude/docs/rules-reference.md` | `.codex/rules/ccgs-rules-path-compatibility.md` | Added | Preserves canonical rule-file/path mapping |
| scoped `CLAUDE.md` files (`docs/`, `design/`, `src/`, `CCGS Skill Testing Framework/`) | `AGENTS.md` required-reading gate + scoped source files | In progress | Behavioral parity depends on agents actually loading the scoped files before edits |
| path-sensitive checks for design/data/code | `.codex/hooks/*.sh` + AGENTS verification rules | In progress | Asset/data and some commit-time checks are wired; broader code-path validation still pending |

**Accepted rules & path-standards deviations (current state)**

- Claude path rules are currently mapped mainly as source-of-truth guidance plus selective hook validations, not full Codex-native path matcher logic.
- Stronger parity exists for `design/gdd/**`, `assets/data/**`, and scoped directory instructions than for AI/network/shader/prototype specialties.
- Real parity depends on preserving the repo rule that scoped `CLAUDE.md` files must be read before edits in covered subtrees.

## Current team-orchestration coverage

| Claude source surface | Codex/OMX compatibility surface | Status | Notes |
|---|---|---|---|
| `.claude/skills/team-*` domain workflows | `.codex/team/ccgs-team-orchestration-compatibility.md` + installed OMX `$team` / `worker` skills + repo-local `ccgs-team-*` wrappers | In progress | Mapping docs plus wrapper skills added for combat/UI/QA/release/narrative/audio/level/polish/live-ops |
| `.claude/docs/coordination-rules.md` | `AGENTS.md` + `.codex/team/ccgs-team-orchestration-compatibility.md` | In progress | Hierarchy/escalation rules are mapped, but not all domain wrappers exist yet |
| `.claude/docs/agent-coordination-map.md` | role compatibility docs + OMX team runtime | In progress | Staffing intent preserved at documentation level |
| director gate / review checkpoints during team flow | AGENTS escalation/verification rules + existing gate compatibility docs | Partial | Explicit repo-local `ccgs-team-*` wrappers still pending |

**Accepted team-orchestration deviations (current state)**

- Claude exposes many named `team-*` entry points; Codex/OMX currently relies on one generic `$team` runtime plus repo-local mapping docs.
- Parity is currently achieved through staffing/phase/escalation mapping, not by cloning every team UX verbatim.
- Wrapper skills now exist for combat/UI/QA/release/narrative/audio/level/polish/live-ops; remaining work is deeper runtime evidence, not just wrapper presence.

## Current templates & catalogs coverage

| Claude source surface | Codex/OMX compatibility surface | Status | Notes |
|---|---|---|---|
| `.claude/docs/workflow-catalog.yaml` | `ccgs-help`, `ccgs-project-stage-detect`, `ccgs-adopt`, `ccgs-gate-check`, `AGENTS.md` | Strong | Direct reuse already active in multiple compatibility surfaces |
| `.claude/docs/skills-reference.md`, `.claude/docs/agent-roster.md` | `.codex/catalog/ccgs-templates-catalog-compatibility.md`, planning docs, `.codex/agents/*` | In progress | Reused mainly by reference and mapping, not yet via dedicated UI wrappers |
| `.claude/docs/templates/*` | `.codex/catalog/ccgs-templates-catalog-compatibility.md`, scoped `CLAUDE.md`, workflow docs | In progress | Canonical templates are preserved by reference instead of copied |
| `CCGS Skill Testing Framework/**` + `.claude/docs/templates/skill-test-spec.md` | skill-testing guidance and source-of-truth references | Partial-to-strong | Canonical testing templates/spec corpus remains directly reusable |

**Accepted templates & catalogs deviations (current state)**

- Many Claude templates/catalogs are intentionally reused by reference rather than wrapped as repo-local Codex-native files.
- Strong parity here comes from minimizing duplication, not from mirroring every source file into `.codex/`.
- The main remaining work is better wrapper guidance and upstream-sync automation, not body-level porting.

## Current skill-QA & upstream-sync coverage

| Claude source surface | Codex/OMX compatibility surface | Status | Notes |
|---|---|---|---|
| `.claude/skills/skill-test/SKILL.md` / `.claude/skills/skill-improve/SKILL.md` | `.codex/maintenance/ccgs-skill-qa-upstream-sync-compatibility.md` + direct source reuse | In progress | Canonical QA flows preserved by reference; native wrappers still pending |
| `CCGS Skill Testing Framework/**` | direct source reuse + maintenance mapping docs | Strong | Canonical testing framework retained unchanged |
| `UPGRADING.md` | direct source reuse + maintenance mapping docs | Strong | Canonical change-history / upgrade ledger retained unchanged |
| future upstream-sync workflow | `docs/codex-omx-port-matrix.md` + `.omx/plans/prd-claude-to-omx-port-20260420T102525Z.md` + `.omx/plans/test-spec-claude-to-omx-port-20260420T102525Z.md` | In progress | Planned automation path is documented, not yet wrapped in a repo-local skill |

**Accepted skill-QA & upstream-sync deviations (current state)**

- Current parity comes mainly from direct reuse of the canonical testing framework and upgrade ledger, not from new repo-local Codex wrapper skills.
- The remaining gap is operational automation: native wrappers and a repeatable sync transcript.
- This cluster is closer to a documented runbook than to a finished Codex-native workflow.

## Planned execution order

1. Root Codex/OMX governance overlay (`AGENTS.md`) — done
2. `/help` pilot skill — done
3. Port matrix refinement + parity scorecard for pilot — in progress (scorecard added; transcript evidence still pending)
4. Expand to adjacent workflow skills — in progress
   - `project-stage-detect` — compatibility skill added
   - `adopt` — compatibility skill added
   - `gate-check` — compatibility skill added
   - `start` — compatibility skill added
5. Port core agent roles — in progress
   - `technical-director` — compatibility agent added
   - `producer` — compatibility agent added
   - `lead-programmer` — compatibility agent added
   - `gameplay-programmer` — compatibility agent added
6. Port hooks and automation — in progress
   - hook/automation behavior map added
   - notify / compaction / subagent tracking mapped to existing OMX runtime surfaces
   - commit / push / asset / skill-change validation entrypoints added
   - project-local native hook wiring added
   - project-local statusline config added
   - real-session hook/config adoption verification still pending
7. Add upstream sync / diff / port skill
8. Continue rules/path and team-orchestration parity hardening
   - repo-local `ccgs-team-*` wrappers added for combat/UI/QA/release/narrative/audio/level/polish/live-ops

## Future upstream sync workflow (target)

A future Codex/OMX maintenance skill should:

1. fetch/pull upstream Claude template changes
2. diff `CLAUDE.md`, `.claude/**`, and supporting docs against the previous baseline
3. classify changes by capability cluster
4. identify existing ports affected by the change
5. propose additive Codex/OMX updates
6. update this matrix with status, gaps, and parity notes
