#!/usr/bin/env bash
# Codex/OMX compatibility surface for the Claude CCGS validate-commit hook.
# Source of truth: .claude/hooks/validate-commit.sh
#
# Input schema (compatible with Claude PreToolUse Bash hook payload):
# { "tool_name": "Bash", "tool_input": { "command": "git commit -m ..." } }
#
# Exit 0 = allow / advisory warnings only
# Exit 2 = block

set -u

INPUT=$(cat)

parse_command() {
  if command -v jq >/dev/null 2>&1; then
    echo "$INPUT" | jq -r '.tool_input.command // empty'
  else
    echo "$INPUT" | grep -oE '"command"[[:space:]]*:[[:space:]]*"[^"]*"' | sed 's/"command"[[:space:]]*:[[:space:]]*"//;s/"$//'
  fi
}

COMMAND=$(parse_command)

if ! echo "$COMMAND" | grep -qE '^git[[:space:]]+commit'; then
  exit 0
fi

STAGED=$(git diff --cached --name-only --diff-filter=ACM 2>/dev/null)
if [ -z "$STAGED" ]; then
  exit 0
fi

WARNINGS=""

DESIGN_FILES=$(echo "$STAGED" | grep -E '^design/gdd/.*\.md$' || true)
if [ -n "$DESIGN_FILES" ]; then
  while IFS= read -r file; do
    [ -f "$file" ] || continue
    for section in "Overview" "Player Fantasy" "Detailed" "Formulas" "Edge Cases" "Dependencies" "Tuning Knobs" "Acceptance Criteria"; do
      if ! grep -qi "$section" "$file"; then
        WARNINGS="$WARNINGS\nDESIGN: $file missing required section: $section"
      fi
    done
  done <<< "$DESIGN_FILES"
fi

DATA_FILES=$(echo "$STAGED" | grep -E '^assets/data/.*\.json$' || true)
if [ -n "$DATA_FILES" ]; then
  PYTHON_CMD=""
  for cmd in python python3 py; do
    if command -v "$cmd" >/dev/null 2>&1; then
      PYTHON_CMD="$cmd"
      break
    fi
  done

  while IFS= read -r file; do
    [ -f "$file" ] || continue
    if [ -n "$PYTHON_CMD" ]; then
      if ! "$PYTHON_CMD" -m json.tool "$file" >/dev/null 2>&1; then
        echo "BLOCKED: $file is not valid JSON" >&2
        exit 2
      fi
    else
      WARNINGS="$WARNINGS\nJSON: Cannot validate JSON (python not found): $file"
    fi
  done <<< "$DATA_FILES"
fi

GAMEPLAY_FILES=$(echo "$STAGED" | grep -E '^src/gameplay/' || true)
if [ -n "$GAMEPLAY_FILES" ]; then
  while IFS= read -r file; do
    [ -f "$file" ] || continue
    if grep -nE '(damage|health|speed|rate|chance|cost|duration)[[:space:]]*[:=][[:space:]]*[0-9]+' "$file" >/dev/null 2>&1; then
      WARNINGS="$WARNINGS\nCODE: $file may contain hardcoded gameplay values. Use data files."
    fi
  done <<< "$GAMEPLAY_FILES"
fi

SRC_FILES=$(echo "$STAGED" | grep -E '^src/' || true)
if [ -n "$SRC_FILES" ]; then
  while IFS= read -r file; do
    [ -f "$file" ] || continue
    if grep -nE '(TODO|FIXME|HACK)[^(]' "$file" >/dev/null 2>&1; then
      WARNINGS="$WARNINGS\nSTYLE: $file has TODO/FIXME without owner tag. Use TODO(name) format."
    fi
  done <<< "$SRC_FILES"
fi

if [ -n "$WARNINGS" ]; then
  echo -e "=== Codex/OMX Commit Validation Warnings ===$WARNINGS\n============================================" >&2
fi

exit 0
