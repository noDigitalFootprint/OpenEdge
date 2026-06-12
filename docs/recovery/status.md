# Recovery status

## Current baseline
- Recovered source lives in `src/OpenEdge/`.
- Minimal committed runtime fixtures live in `runtime/sample/`.
- Local launch/runtime output is `runtime/local/app/`.
- Canonical loose rebuild assets live in `Data/resources/` and `Data/audio/`.
- Repo-owned binary DLL dependencies live in `vendor/dotnet/`.
- The project builds with `dotnet build src/OpenEdge/OpenEdge.csproj`.
- The smoke baseline runs with `powershell -ExecutionPolicy Bypass -File docs/recovery/smoke-check.ps1` and launches `runtime/local/app/OpenEdge.exe`.
- VS Code launch configs live in `.vscode/launch.json`; `C#: OpenEdge` is the current config and `C#: EverEdge` is retained as a compatibility alias.
- The active rebuilt project now targets `net8.0-windows`.
- The codebase and user-facing app identity have been renamed from EverEdge to OpenEdge.
- The legacy decompiled project tree has been removed from the active repo after dependency relocation and re-verification.

## Confirmed working
- The recovered OpenEdge project compiles successfully.
- The rebuilt OpenEdge executable launches and stays alive through startup smoke testing.
- A session can be started from the recovered repo build.
- Rebuilds repopulate `runtime/local/app/resources/` and `runtime/local/app/audio/` from `Data/`.
- Startup writes expected runtime markers such as `runtime/local/app/flags/temp/open.txt`.
- A repeatable baseline smoke script now exists at `docs/recovery/smoke-check.ps1`.
- The first warning-triage batch completed without breaking the baseline: dead private fields were removed from `TaskPage.cs`, `BluetoothDeviceTester.cs`, and `Homework.cs`.
- After that batch, the documented Phase 1 smoke check still passes and the build warning count dropped from 181 to 173.
- The second warning-triage batch completed without breaking the baseline: dead helper/UI state was removed from `Voc.cs`, `WriteTask.cs`, `ImageTagger.cs`, and `MainWindow.cs`.
- After the current warning-triage work, the documented Phase 1 smoke check still passes and the build warning count is down to 53.
- Runtime media/resource normalization now reaches the active flow between `MainWindow`, `ImageTagger`, `SecondWindow`, and the fixed-resource callers that were moved onto `RuntimePaths.Resource(...)`.
- The latest baseline pass keeps the repo functional with 49 build warnings remaining, 0 build errors, and a passing launch/startup smoke check.
- A bounded async cleanup batch completed without breaking the baseline: `Bluetooth.connectToIC`, `BluetoothDeviceTester.btLinearAsync`, and `SecondWindow.changeBg` now await their local asynchronous work instead of dropping it on the floor.
- The current baseline now builds with 39 warnings, 0 errors, and still passes the documented launch/startup smoke check.
- A second async cleanup batch completed in `MainWindow`: the live device motion helpers now await `LinearAsync(...)` and use direct `await Task.Delay(...)` sequencing instead of `ContinueWith(...)`.
- The current baseline now builds with 25 warnings, 0 errors, and still passes the documented launch/startup smoke check.
- A nullable-context cleanup batch completed without affecting runtime behavior: local reference-type `?` annotations were removed in files that compile under disabled nullable context.
- The current baseline now builds with 5 warnings, 0 errors, and still passes the documented launch/startup smoke check.
- The project file was modernized to match the current SDK contract without changing runtime behavior: the SDK root is now `Microsoft.NET.Sdk`, and `SelfContained` is explicit.
- The current baseline now builds with 0 warnings, 0 errors, and still passes the documented launch/startup smoke check.
- The active rebuilt project was retargeted from `net7.0-windows` to `net8.0-windows`, and the smoke baseline still passes under the new framework target.
- A first source-based media pipeline slice is now implemented: a shared media catalog drives media discovery, playback consumers, and tag lookups, while a `Media Sources` page allows folder registration and image/video source toggling.
- A first resilient tag-identity slice is now implemented: the media catalog persists identity records in `media-tag-index.json`, mirrors tags to `tags.txt`, and uses identity matching to preserve tags across at least same-source file moves.
- UI-driven smoke checks confirm these rebuilt core flows are reachable:
  - `Page1` → `Begin Session` reaches the consent gate (`I decline` / `I accept`).
  - `Page1` → `Tag Media` reaches the media tools (`Prev`, `Next`, `Untagged`, `Groups`, `Back`, `Image`, `Folder`).
  - `Page1` → `Connect Toys` reaches the Bluetooth tools (`Start Scanning`, `Find IC`, `Launch IC`, `Back`).
- The canonical settings/tags migration guardrails are in place:
  - `verify-baseline.ps1` runs build, SettingsHarness, legacy-state audit, and smoke check.
  - `LegacyStateAdapter`, `SessionFlagStore`, and `MediaTagCompatibilityAdapter` isolate compatibility seams.
  - Script setting predicates/commands are centralized through shared evaluators/executor classes.
  - Built-in base scripts have been migrated away from safe setting-like legacy aliases; remaining legacy script usage is audited and classified.
  - Media tag compatibility now has diagnostics and identity tests for rename, move, duplicate fingerprint ambiguity, collision rename, and delete/re-add.
  - Settings UI includes a Compatibility / Advanced alias view and mod setting metadata (`group`, `description`, `warning`).

## Known issues
- The window icon has been restored through `FrameWindow.xaml` using the packaged `app.ico` resource.
- Path handling is now centralized enough for the validated rebuilt runtime flow, though future feature work should continue using `RuntimePaths` instead of ad hoc path construction.
- Interactive feature-parity checks beyond the validated core entry flows are still manual work; this pass did not exhaustively verify every sub-screen or external dependency path.

## Next cleanup targets
- The remaining work is concentrated in three areas:
  - any remaining design-level cleanup that should be validated against real feature intent rather than warning count alone
  - optional policy decisions about framework/support posture rather than code correctness
- The next media-pipeline step is to strengthen identity matching beyond the current conservative move/rebind rules and reduce the remaining path-shaped assumptions in legacy compatibility flows.
- Decide whether lower-case embedded WPF resources and camel-case loose runtime assets should be normalized into one naming scheme.
- Keep resource naming stable unless a future packaging issue requires a focused resource migration.
