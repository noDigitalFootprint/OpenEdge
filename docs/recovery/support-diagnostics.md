# OpenEdge support diagnostics

Use these tools when checking compatibility state, script migration, or media tag recovery.

## Baseline verification

```powershell
powershell -ExecutionPolicy Bypass -File docs/recovery/verify-baseline.ps1
```

This runs build, SettingsHarness, and the smoke check.

## Legacy script/state audit

```powershell
powershell -ExecutionPolicy Bypass -File docs/recovery/audit-legacy-state.ps1 -OutputPath docs/recovery/legacy-state-audit.md
```

This reports remaining `FLAG:`, `DELFLAG:`, `SETVAR:`, `ADDVAR:`, `ISFLAG:`, `ISNOFLAG:`, and direct app-code legacy state calls.

## Full diagnostics export

```powershell
powershell -ExecutionPolicy Bypass -File docs/recovery/export-diagnostics.ps1
```

This writes `docs/recovery/openedge-diagnostics.md` with compatibility state counts, media tag counts, and the script migration audit.

## In-app diagnostics

Open `Settings -> Migration Tools -> Export diagnostics report` to export a runtime diagnostics markdown file under `runtime/local/app/debug/`.

## Media tag recovery notes

- `media-tag-index.json` is primary.
- `tags.txt` is a legacy mirror.
- If media was moved or renamed, reload Media Sources first so identity matching can rebind tags.
- Duplicate files with the same fingerprint are intentionally not guessed; ambiguous claims are logged instead of stealing tags.
