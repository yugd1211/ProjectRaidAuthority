# Codex/OMX Template & Catalog Compatibility

This directory records how Claude-native catalogs and templates are reused by Codex/OMX compatibility surfaces.

Source-of-truth remains:

- `.claude/docs/workflow-catalog.yaml`
- `.claude/docs/skills-reference.md`
- `.claude/docs/agent-roster.md`
- `.claude/docs/templates/*`
- related `.claude/docs/*` catalog/reference documents

Current Codex/OMX strategy is **prefer direct reuse over duplication** unless a native Codex surface requires a narrower adapter or wrapper.
