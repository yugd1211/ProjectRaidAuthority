#!/usr/bin/env bash
# Codex/OMX compatibility surface for the Claude CCGS validate-skill-change hook.
# Source of truth: .claude/hooks/validate-skill-change.sh
#
# Input schema (compatible with Claude PostToolUse Write/Edit payload):
# { "tool_name": "Write", "tool_input": { "file_path": "..." } }
#
# Exit 0 = advisory only

set -u

INPUT=$(cat)

parse_file_path() {
  if command -v jq >/dev/null 2>&1; then
    echo "$INPUT" | jq -r '.tool_input.file_path // empty'
  else
    echo "$INPUT" | grep -oE '"file_path"[[:space:]]*:[[:space:]]*"[^"]*"' | sed 's/"file_path"[[:space:]]*:[[:space:]]*"//;s/"$//'
  fi
}

FILE_PATH=$(parse_file_path)
FILE_PATH=$(echo "$FILE_PATH" | sed 's|\\|/|g')

if echo "$FILE_PATH" | grep -qE '(^|/)\.claude/skills/'; then
  SKILL_NAME=$(echo "$FILE_PATH" | grep -oE '\.claude/skills/[^/]+' | sed 's|\.claude/skills/||')
  if [ -n "$SKILL_NAME" ]; then
    echo "=== Skill Modified: $SKILL_NAME ===" >&2
    echo "Run /skill-test static $SKILL_NAME to validate structural compliance." >&2
    echo "====================================" >&2
  fi
  exit 0
fi

if echo "$FILE_PATH" | grep -qE '(^|/)\.codex/skills/'; then
  SKILL_NAME=$(echo "$FILE_PATH" | grep -oE '\.codex/skills/[^/]+' | sed 's|\.codex/skills/||')
  if [ -n "$SKILL_NAME" ]; then
    echo "=== Codex Skill Modified: $SKILL_NAME ===" >&2
    echo "Run the repo's skill validation workflow or compare against the Claude source skill when available." >&2
    echo "For Claude-native parity checks, inspect .claude/skills/skill-test/SKILL.md and the CCGS Skill Testing Framework." >&2
    echo "==========================================" >&2
  fi
fi

exit 0
