#!/usr/bin/env bash
# Resolve and run the oh-my-codex native hook entrypoint without hardcoding a single absolute install path.

set -euo pipefail

candidates=()

if [ -n "${OMX_NATIVE_HOOK_PATH:-}" ]; then
  candidates+=("$OMX_NATIVE_HOOK_PATH")
fi

if command -v npm >/dev/null 2>&1; then
  npm_root=$(npm root -g 2>/dev/null || true)
  if [ -n "$npm_root" ]; then
    candidates+=("$npm_root/oh-my-codex/dist/scripts/codex-native-hook.js")
  fi
fi

candidates+=(
  "/opt/homebrew/lib/node_modules/oh-my-codex/dist/scripts/codex-native-hook.js"
  "/usr/local/lib/node_modules/oh-my-codex/dist/scripts/codex-native-hook.js"
)

resolved=""
for candidate in "${candidates[@]}"; do
  if [ -f "$candidate" ]; then
    resolved="$candidate"
    break
  fi
done

if [ -z "$resolved" ]; then
  echo "OMX native hook script not found. Set OMX_NATIVE_HOOK_PATH or install oh-my-codex globally." >&2
  exit 1
fi

if [ "${OMX_HOOK_WRAPPER_PRINT_TARGET:-0}" = "1" ]; then
  echo "$resolved"
  exit 0
fi

exec node "$resolved"
