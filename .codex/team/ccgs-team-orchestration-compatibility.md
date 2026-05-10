# CCGS Team Orchestration — Codex/OMX Compatibility Map

Source surfaces:

- `.claude/skills/team-combat/SKILL.md`
- `.claude/skills/team-narrative/SKILL.md`
- `.claude/skills/team-ui/SKILL.md`
- `.claude/skills/team-level/SKILL.md`
- `.claude/skills/team-audio/SKILL.md`
- `.claude/skills/team-polish/SKILL.md`
- `.claude/skills/team-release/SKILL.md`
- `.claude/skills/team-live-ops/SKILL.md`
- `.claude/skills/team-qa/SKILL.md`
- `.claude/docs/coordination-rules.md`
- `.claude/docs/agent-coordination-map.md`
- `.claude/docs/director-gates.md`

Codex/OMX runtime surfaces consulted:

- `~/.codex/skills/team/SKILL.md`
- `~/.codex/skills/worker/SKILL.md`
- `AGENTS.md`
- `.omx/tmux-hook.json`
- `.omx/state/team-leader-nudge.json`
- `.omx/state/subagent-tracking.json`
- `.omx/state/skill-active-state.json`

## Compatibility posture

Claude-native CCGS team orchestration is **domain-first**:

- each `team-*` skill names a concrete cross-functional team
- the skill runs a phased orchestration pipeline
- subagents are spawned with explicit roles
- phase transitions require review/approval

Current Codex/OMX parity is **runtime-first**:

- OMX `$team` provides durable tmux/team-state orchestration
- OMX `worker` defines worker ACK/mailbox/task lifecycle
- repo `AGENTS.md` defines team/worker governance and escalation rules
- repo compatibility layers currently document how domain-specific Claude team flows should be staffed and translated into OMX launches

## Team-skill to Codex/OMX mapping

| Claude source skill | Source team intent | Current Codex/OMX compatibility surface | Current parity |
|---|---|---|---|
| `team-combat` | coordinate combat design + code + AI + VFX + audio + QA | OMX `$team` runtime + role compatibility docs (`lead-programmer`, `gameplay-programmer`, `technical-director`, etc.) + native subagents when bounded in-session fanout is enough | Partial |
| `team-narrative` | coordinate narrative design/content implementation across writing/gameplay/QA | OMX `$team` runtime + role/staffing translation via AGENTS and Claude source docs | Partial |
| `team-ui` | coordinate UX/UI implementation with design + programming + QA | OMX `$team` runtime + `ui`/UX-related role guidance + scoped `design/CLAUDE.md` / `src/CLAUDE.md` | Partial |
| `team-level` | coordinate level design, content, implementation, and test | OMX `$team` runtime + role/staffing translation | Partial |
| `team-audio` | coordinate audio direction, events, implementation, and validation | OMX `$team` runtime + role/staffing translation | Partial |
| `team-polish` | coordinate performance, art, audio, UX, and QA polish | OMX `$team` runtime + verification-oriented lanes | Partial |
| `team-release` | coordinate release-manager, QA, build/deploy, and sign-off | OMX `$team` runtime + release/QA/devops-style staffing translation | Partial |
| `team-live-ops` | coordinate live-ops design, economy, community, analytics, and release tasks | OMX `$team` runtime + role/staffing translation | Partial |
| `team-qa` | orchestrate QA planning/execution/sign-off | OMX `$team` runtime + verification lanes + QA-oriented worker protocol | Partial |

## Coordination-rules mapping

| Claude coordination behavior | Current Codex/OMX surface |
|---|---|
| vertical delegation by hierarchy | `AGENTS.md` leader/worker protocol + role catalog + team skill runtime |
| horizontal consultation without binding cross-domain decisions | `AGENTS.md` delegation rules and role boundaries |
| conflict escalation to shared parent / director | `AGENTS.md` escalation rules + role compatibility docs |
| no unilateral cross-domain changes | `AGENTS.md` + worker-scope rules + review/verification expectations |
| parallel spawn for independent lanes | OMX `$team` runtime + Codex native subagents for bounded parallel work |

## Practical translation rule

When porting or emulating a Claude `team-*` skill in Codex/OMX, preserve these invariants:

1. **domain-specific staffing** still matters
   - do not reduce everything to generic workers without recording intended roles
2. **phase ordering** still matters
   - design / architecture / implementation / integration / validation / sign-off
3. **escalation ownership** still matters
   - cross-domain conflicts go upward, not sideways
4. **verification lane** must always exist
   - QA/review cannot disappear just because runtime is more generic

## Accepted deviations

- Claude has many named `team-*` workflow entry points; Codex/OMX currently has one generic `$team` runtime plus repo-specific compatibility docs, not a full set of repo-local `ccgs-team-*` skills yet.
- Claude `Task`-based in-session orchestration and OMX tmux-team runtime are not identical execution models; parity is achieved by preserving staffing, sequencing, escalation, and verification intent rather than duplicating the exact UX.
- Domain-specific team orchestration is currently **documented and staffable**, but not yet exposed through dedicated repo-local Codex skill wrappers for each `team-*` variant.

## Recommended next slices

1. Add repo-local `ccgs-team-*` wrappers for the highest-value team workflows (`combat`, `ui`, `qa`, `release`).
2. Add staffing templates that translate each Claude `team-*` skill into an OMX `$team N:role` launch recommendation.
3. Capture a verification transcript from a real `$team` run to prove mailbox/ACK/task-state behavior against one representative CCGS team workflow.
