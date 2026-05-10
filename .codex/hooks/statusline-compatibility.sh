#!/usr/bin/env bash
# Codex/OMX compatibility surface for the Claude CCGS statusline.
# Source of truth: .claude/statusline.sh
# Receives JSON on stdin and prints a single-line status summary.

set -u

input=$(cat)

if command -v jq >/dev/null 2>&1; then
  model=$(echo "$input" | jq -r '.model.display_name // "Unknown"')
  used_pct=$(echo "$input" | jq -r '.context_window.used_percentage // empty')
  cwd=$(echo "$input" | jq -r '.workspace.current_dir // .cwd // ""')
else
  model=$(echo "$input" | grep -oE '"display_name"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | sed 's/.*: *"//;s/"//')
  used_pct=$(echo "$input" | grep -oE '"used_percentage"[[:space:]]*:[[:space:]]*[0-9]+' | head -1 | sed 's/.*: *//')
  cwd=$(echo "$input" | grep -oE '"current_dir"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | sed 's/.*: *"//;s/"//')
  [ -z "$model" ] && model="Unknown"
fi

cwd=$(echo "$cwd" | sed 's|\\|/|g')
[ -z "$cwd" ] && cwd="."

if [ -n "${used_pct:-}" ]; then
  ctx_label="ctx: ${used_pct}%"
else
  ctx_label="ctx: --"
fi

stage_file="$cwd/production/stage.txt"
stage=""
if [ -f "$stage_file" ]; then
  stage=$(head -1 "$stage_file" | tr -d '\r\n')
fi

if [ -z "$stage" ]; then
  concept_file="$cwd/design/gdd/game-concept.md"
  systems_file="$cwd/design/gdd/systems-index.md"
  tech_prefs="$cwd/.claude/docs/technical-preferences.md"

  has_concept=false
  has_systems=false
  engine_configured=false
  src_count=0
  has_adrs=false

  [ -f "$concept_file" ] && has_concept=true
  [ -f "$systems_file" ] && has_systems=true

  if [ -f "$tech_prefs" ]; then
    engine_line=$(grep -m1 '^\*\*Engine\*\*:' "$tech_prefs" 2>/dev/null || true)
    if [ -n "$engine_line" ] && ! echo "$engine_line" | grep -q 'TO BE CONFIGURED'; then
      engine_configured=true
    fi
  fi

  if [ -d "$cwd/src" ]; then
    src_count=$(find "$cwd/src" -type f \( -name '*.gd' -o -name '*.cs' -o -name '*.cpp' -o -name '*.h' -o -name '*.py' -o -name '*.rs' -o -name '*.lua' -o -name '*.tscn' -o -name '*.tres' \) 2>/dev/null | wc -l | tr -d ' ')
  fi

  if ls "$cwd/docs/architecture/"adr-*.md >/dev/null 2>&1; then
    has_adrs=true
  fi

  if [ "$src_count" -ge 10 ] 2>/dev/null; then
    stage='Production'
  elif [ "$has_adrs" = true ]; then
    stage='Pre-Production'
  elif [ "$engine_configured" = true ]; then
    stage='Technical Setup'
  elif [ "$has_systems" = true ]; then
    stage='Systems Design'
  elif [ "$has_concept" = true ]; then
    stage='Concept'
  else
    stage='Concept'
  fi
fi

breadcrumb=""
if [ "$stage" = 'Production' ] || [ "$stage" = 'Polish' ] || [ "$stage" = 'Release' ]; then
  state_file="$cwd/production/session-state/active.md"
  if [ -f "$state_file" ]; then
    in_block=false
    epic='' ; feature='' ; task=''
    while IFS= read -r line; do
      case "$line" in
        *'<!-- STATUS -->'*) in_block=true; continue ;;
        *'<!-- /STATUS -->'*) break ;;
      esac
      if [ "$in_block" = true ]; then
        case "$line" in
          Epic:*) epic=$(echo "$line" | sed 's/^Epic: *//') ;;
          Feature:*) feature=$(echo "$line" | sed 's/^Feature: *//') ;;
          Task:*) task=$(echo "$line" | sed 's/^Task: *//') ;;
        esac
      fi
    done < "$state_file"

    parts=''
    [ -n "$epic" ] && parts="$epic"
    [ -n "$feature" ] && parts="${parts:+$parts > }$feature"
    [ -n "$task" ] && parts="${parts:+$parts > }$task"
    [ -n "$parts" ] && breadcrumb=" | $parts"
  fi
fi

printf '%s' "${ctx_label} | ${model} | ${stage}${breadcrumb}"
