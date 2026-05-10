#!/usr/bin/env bash
# Codex/OMX compatibility surface for the Claude CCGS validate-assets hook.
# Source of truth: .claude/hooks/validate-assets.sh
#
# Input schema (compatible with Claude PostToolUse Write/Edit payload):
# { "tool_name": "Write", "tool_input": { "file_path": "assets/data/foo.json", "content": "..." } }
#
# Exit 0 = allow / advisory warnings only
# Exit 1 = block for build-breaking asset/data problems

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

if ! echo "$FILE_PATH" | grep -qE '(^|/)assets/'; then
  exit 0
fi

FILENAME=$(basename "$FILE_PATH")
WARNINGS=""
ERRORS=""

if echo "$FILENAME" | grep -qE '[A-Z[:space:]-]'; then
  WARNINGS="$WARNINGS\n  NAMING: $FILE_PATH must be lowercase with underscores (got: $FILENAME)"
fi

if echo "$FILE_PATH" | grep -qE '(^|/)assets/data/.*\.json$'; then
  if [ -f "$FILE_PATH" ]; then
    PYTHON_CMD=""
    for cmd in python python3 py; do
      if command -v "$cmd" >/dev/null 2>&1; then
        PYTHON_CMD="$cmd"
        break
      fi
    done

    if [ -n "$PYTHON_CMD" ]; then
      if ! "$PYTHON_CMD" -m json.tool "$FILE_PATH" >/dev/null 2>&1; then
        ERRORS="$ERRORS\n  FORMAT: $FILE_PATH is not valid JSON — fix syntax errors before continuing"
      fi
    fi
  fi
fi

if [ -n "$WARNINGS" ]; then
  echo -e "=== Codex/OMX Asset Validation: Warnings ===$WARNINGS\n============================================\n(Warnings are advisory. Fix before final commit.)" >&2
fi

if [ -n "$ERRORS" ]; then
  echo -e "=== Codex/OMX Asset Validation: ERRORS (Blocking) ===$ERRORS\n=====================================================\nFix these errors before proceeding." >&2
  exit 1
fi

exit 0
