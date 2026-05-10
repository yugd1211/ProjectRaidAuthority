#!/usr/bin/env bash
# Codex/OMX compatibility surface for the Claude CCGS validate-push hook.
# Source of truth: .claude/hooks/validate-push.sh
#
# Input schema (compatible with Claude PreToolUse Bash hook payload):
# { "tool_name": "Bash", "tool_input": { "command": "git push origin main" } }
#
# Exit 0 = allow / advisory warning
# Exit 2 = block (currently unused to preserve Claude behavior)

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

if ! echo "$COMMAND" | grep -qE '^git[[:space:]]+push'; then
  exit 0
fi

CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || true)
MATCHED_BRANCH=""

for branch in develop main master; do
  if [ "$CURRENT_BRANCH" = "$branch" ]; then
    MATCHED_BRANCH="$branch"
    break
  fi
  if echo "$COMMAND" | grep -qE "[[:space:]]${branch}([[:space:]]|$)"; then
    MATCHED_BRANCH="$branch"
    break
  fi
done

if [ -n "$MATCHED_BRANCH" ]; then
  echo "Push to protected branch '$MATCHED_BRANCH' detected." >&2
  echo "Reminder: Ensure build passes, unit tests pass, and no S1/S2 bugs exist." >&2
fi

exit 0
