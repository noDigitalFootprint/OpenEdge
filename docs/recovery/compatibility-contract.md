# OpenEdge compatibility contract

This document defines how OpenEdge keeps old EverEdge/OpenEdge state working while new code moves to canonical settings and identity-backed media tags.

## Canonical state is primary

New code should prefer:

- `SettingsRegistry` for user preferences and answer state.
- Structured setting helpers for substate/progression-like choices, e.g. anal, pet play, outside-session rules, LOB window state.
- `MediaCatalogService` for media identity, sources, and media tags.
- Derived contexts for combinations of settings and media availability.

## Legacy flags and vars remain supported

Old script commands and old installs may still contain state such as:

- `FLAG:<key>`
- `DELFLAG:<key>`
- `SETVAR:<key>,<value>`
- `ADDVAR:<key>,<value>`
- `ISFLAG:<key>`
- `ISNOFLAG:<key>`

These commands must continue to work. Their behavior should be routed through compatibility adapters as the migration progresses.

## Permanent aliases

Some old keys are permanent aliases for canonical settings. Examples:

- `cuck` / `cuckNo` -> canonical `cuck`
- `gay` / `gayNo` -> canonical `gay`
- `domTitle` -> canonical raw value `domTitle`
- `sessionLengthMod` -> canonical numeric value `sessionLengthMod`

Permanent aliases must:

- read old state into canonical state,
- mirror canonical writes back to old files when required,
- clear canonical mirrors when the legacy key is explicitly deleted,
- remain tested in `SettingsHarness`.

## Structured setting substates

Some old keys describe substate rather than a simple toggle. Examples:

- anal stage/preference/training keys,
- pet persona and pet identity keys,
- chastity cage/wearing/lost-key/date/type keys,
- outside-session rule counters,
- LOB timing/window keys.

New scripts should use structured commands and predicates for these states. Old keys remain compatibility inputs/outputs until the adapter layer owns them fully.

## Runtime/session temp state

Short-lived script flow and session recovery flags should not become canonical settings. They may remain file-backed flags, but must be clearly separated from persistent preference/progression state.

Examples:

- startup/open flag,
- leave-early/interruption markers,
- current module/session flags,
- temporary equipment state.

## Reset behavior

Reset progression should clear:

- canonical settings and answers,
- mapped legacy preference/progression aliases,
- queued setting asks,
- structured preference/progression substates covered by the reset operation.

Reset progression must preserve:

- media sources,
- identity-backed media tags,
- legacy `tags.txt` mirror data,
- mod installation/config files,
- compatibility transfer/export history unless explicitly requested.

Clear Session State is narrower. It should clear interruption/recovery markers only and must not wipe normal long-term progression, preferences, media tags, or media sources.

## Media tags

`media-tag-index.json` is the primary media tag store. It uses media identity/fingerprints so tags can survive moves or renames where safe.

`tags.txt` is a legacy mirror. It remains readable/writable for compatibility, but new code should not treat it as the source of truth.

Canonical media identity records currently keep tags as the same compact concatenated tag string used by the legacy UI/tagger. This is intentional for now: tag parsing rules are display-order-sensitive and the legacy mirror must remain byte-compatible. Future work may introduce an internal array/set representation, but only behind conversion helpers and after the existing compatibility tests are preserved.

Compatibility behavior should:

- import path-based legacy tag claims where they can be resolved safely,
- mirror canonical identity tags back to `tags.txt`,
- report ambiguous duplicate-fingerprint matches instead of silently stealing tags,
- preserve old tags during reset progression.

## Migration rule for new work

New implementation should not add new setting-like `FLAG:`/`SETVAR:` usage unless it is explicitly classified in `legacy-state-map.json` and allowlisted. Use canonical commands/predicates instead.
