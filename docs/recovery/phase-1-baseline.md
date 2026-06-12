# Phase 1 stabilization backlog

## Goal
Turn the current recovered OpenEdge build into a repeatable, low-risk baseline that can be changed without rediscovering recovery assumptions every time.

## Scope
Phase 1 is stabilization only.

Included:
- canonical build and launch workflow
- documented runtime layout
- repeatable smoke checks
- validated core entry-flow coverage
- pathing and packaging decisions needed to protect the current baseline

Excluded:
- broad architectural refactors
- warning cleanup beyond what blocks stabilization
- exhaustive full-app parity testing
- packaging redesign beyond one canonical recovery path

## Canonical baseline
- Source project: `src/OpenEdge/OpenEdge.csproj`
- Build output: `runtime/local/app/`
- Canonical loose assets: `Data/resources/` and `Data/audio/`
- Repo-owned DLL dependencies: `vendor/dotnet/`
- Current validated entry flows:
  - `Page1` → `Begin Session`
  - `Page1` → `Tag Media`
  - `Page1` → `Connect Toys`

## Ordered backlog

### 1. Freeze the stabilization baseline
**Purpose**
Define exactly what Phase 1 promises so later cleanup cannot silently move the goalposts.

**Deliverables**
- this backlog document
- updated recovery entrypoint docs
- explicit in-scope / out-of-scope boundaries

**Verification**
- a contributor can identify the canonical build path, launch path, and validated flows from `docs/recovery/` alone

### 2. Formalize the build + launch smoke check
**Purpose**
Turn the current manual recovery confidence into a repeatable check.

**Deliverables**
- `docs/recovery/smoke-check.ps1`
- documented usage and pass/fail expectations

**Verification**
- the smoke check builds the project, verifies output artifacts, launches the rebuilt app, and confirms startup writes `flags/temp/open.txt`
- known recovery-era warnings may still appear; the stabilization pass/fail rule is zero build errors plus successful launch/startup checks

### 3. Lock the validated core entry-flow matrix
**Purpose**
Preserve the minimal user-visible flows that must not regress during future cleanup.

**Deliverables**
- explicit matrix of validated entry flows and expected evidence

**Verification**
- docs show which flows are protected now and which are still unverified

### 4. Close the RuntimePaths migration audit
**Purpose**
Remove ambiguity about which runtime path callers are already normalized and which remain risky.

**Deliverables**
- inventory of remaining non-normalized callers
- migrated vs deferred list

**Verification**
- no known runtime path hotspot is left undocumented

### 5. Standardize the Phase 1 packaging rule
**Purpose**
Make the current recovery packaging model explicit before any broader cleanup starts.

**Deliverables**
- one documented canonical packaging path for recovery builds
- explicit note on deferred mixed-resource cleanup

**Verification**
- build/output layout can be reproduced from docs without tribal knowledge

### 6. Define the Phase 1 exit checklist
**Purpose**
Create an objective stop condition for stabilization.

**Deliverables**
- checklist covering build, launch, runtime layout, smoke checks, validated flows, path audit state, and packaging rule

**Verification**
- each checklist item can be answered yes/no from repo evidence

## Core entry-flow matrix

| Flow | Expected evidence | Status |
|---|---|---|
| `Page1` → `Begin Session` | consent gate shows `I decline` / `I accept` | Validated |
| `Page1` → `Tag Media` | tagging controls show `Prev`, `Next`, `Untagged`, `Groups`, `Back`, `Image`, `Folder` | Validated |
| `Page1` → `Connect Toys` | Bluetooth controls show `Start Scanning`, `Find IC`, `Launch IC`, `Back` | Validated |
| deeper sub-screens and external dependencies | explicit manual follow-up required | Deferred |

## Phase 1 exit checklist
- [ ] Canonical build command is documented and reproducible
- [ ] Canonical runtime layout is documented and reproducible
- [ ] Smoke check is executable from the repo
- [ ] Startup artifact check is part of the smoke path
- [ ] Validated core entry flows are recorded in docs
- [ ] Remaining RuntimePaths migration targets are inventoried
- [ ] Canonical packaging rule is documented
- [ ] Deferred work is clearly separated from stabilization

## Warning triage log
- Batch 1 completed: removed low-risk dead private fields from `TaskPage.cs`, `BluetoothDeviceTester.cs`, and `Homework.cs`.
- Baseline verification after Batch 1: `docs/recovery/smoke-check.ps1` passed from the repo root.
- Warning count after Batch 1: 173 warnings, down from 181.
- Batch 2 completed: removed dead helper/UI state from `Voc.cs`, `WriteTask.cs`, `ImageTagger.cs`, and declaration-only state from `MainWindow.cs`.
- Baseline verification after Batch 2: `docs/recovery/smoke-check.ps1` passed from the repo root.
- Warning count after Batch 2: 53 warnings, down from 173.
- Remaining warning groups are no longer purely mechanical: mostly `CS4014`, `CS8632`, SDK warnings, and two `CS0649` cases that need intent review.
- Batch 3 completed: normalized runtime media/resource flow so `MainWindow` now emits runtime-relative media paths, `ImageTagger` tagged lookups return the same shape, and `SecondWindow` resolves them at use time through `RuntimePaths`.
- Batch 4 completed: removed the remaining low-risk dead-state `CS0649` cases by collapsing permanently-false `MainWindow.breakLoop` state and deleting unreachable `IllegalBreath.progress` storage plus its unused punishment path.
- Baseline verification after Batches 3-4: `docs/recovery/smoke-check.ps1` passed from the repo root.
- Current warning count: 49 warnings, down from 53.
- Batch 5 completed: bounded async cleanup in `Bluetooth.cs`, `BluetoothDeviceTester.cs`, and `SecondWindow.cs` now awaits local asynchronous work where sequencing was already implicit.
- Baseline verification after Batch 5: `docs/recovery/smoke-check.ps1` passed from the repo root.
- Current warning count: 39 warnings, down from 49.
- Batch 6 completed: `MainWindow` device motion helpers now await `LinearAsync(...)` and direct delays instead of chaining unawaited continuations.
- Baseline verification after Batch 6: `docs/recovery/smoke-check.ps1` passed from the repo root.
- Current warning count: 25 warnings, down from 39.
- Batch 7 completed: local reference-type nullable annotations were removed where the project still compiles with nullable context disabled.
- Baseline verification after Batch 7: `docs/recovery/smoke-check.ps1` passed from the repo root.
- Current warning count: 5 warnings, down from 25.
- Batch 8 completed: the project file was aligned with the modern SDK contract by switching to `Microsoft.NET.Sdk` and making `SelfContained` explicit.
- Baseline verification after Batch 8: `docs/recovery/smoke-check.ps1` passed from the repo root.
- Current warning count: 3 warnings, down from 5.
- Batch 9 completed: the active rebuilt project was retargeted from `net7.0-windows` to `net8.0-windows`.
- Baseline verification after Batch 9: `docs/recovery/smoke-check.ps1` passed from the repo root.
- Current warning count: 0 warnings, down from 3.
- The rebuilt repo now has a clean .NET 8 build baseline with zero code or SDK warnings.
- Batch 10 completed: the first source-based media pipeline slice was added with `MediaCatalogService`, `media-sources.json`, and a `Media Sources` page.
- Baseline verification after Batch 10: project builds cleanly, smoke check passes, and the UI automation path confirms the `Media Sources` page opens and exposes `Add Folder`, `Save`, and `Back` actions.
- Batch 11 completed: the first identity-backed tag layer was added through `media-tag-index.json`, with legacy `tags.txt` retained as a compatibility mirror.
- Baseline verification after Batch 11: project builds cleanly, smoke check passes, and a manual harness attempt was made specifically to validate move resilience; the runtime feature remains verified at the app level, while the harness itself was limited by isolated assembly-loading quirks outside the main app path.
