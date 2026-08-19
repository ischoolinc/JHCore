## Why

`Program.Main()` still calls `Student.Instance.SetupPresentation()`, `Class.Instance.SetupPresentation()`, `Teacher.Instance.SetupPresentation()`, and `Course.Instance.SetupPresentation()` synchronously, one after another, before the module's window becomes interactive. A code comment left by a prior author already measured this block at `//1.1 秒` (1.1 seconds). Each `SetupPresentation()` wires up ribbon buttons, list-pane fields, detail builders, and (for Teacher/Student) a `FISCA.InteractionService.DiscoverAPI<T>()` reflection lookup — all pure UI/menu construction that does not depend on the data being fetched by `SyncAllBackground()`. Only one of the four panels (Student, based on call order) is what the user actually sees first; the other three panels' full ribbon/menu wiring is paid for up front even though the user may not visit them in that session.

Four prior changes (`loading-time-optimization`, `improving-loading-speed`, `surgical-xml-parsing`, and the semester-scoped Course loading) already eliminated the largest known costs: O(n) index rebuilds, XML parsing overhead, unscoped course history, and startup sync ordering. A fresh audit of the remaining codebase found this sequential `SetupPresentation()` block to be the highest-confidence structural cost still unaddressed — the rest of the data layer, filtering, and XML parsing paths are already optimized.

## What Changes

- Keep the initially-visible panel's `SetupPresentation()` call synchronous (the module's window must be fully usable for that panel immediately).
- Defer the remaining three panels' `SetupPresentation()` calls so they run after `Main()` returns and the message loop starts pumping (e.g. via `Application.Idle` fired once, or an equivalent post-show continuation), instead of blocking before the window appears.
- Preserve strict ordering guarantees that other startup code relies on (e.g. `Class.Instance.AddDetailBulider(...)` at Program.cs:97, which depends on `Class.Instance.SetupPresentation()` having already run) — deferred calls must still complete, in order, before anything that touches their results.
- No change to *what* each panel's UI ends up wired to — only *when* the wiring for non-primary panels happens.

## Capabilities

### New Capabilities
- `deferred-panel-presentation-setup`: Non-primary panel `SetupPresentation()` calls are deferred until after the module window is shown/interactive, so startup no longer pays the full four-panel UI-construction cost up front.

### Modified Capabilities
<!-- No existing spec-level behavior changes — startup-background-sync governs data sync ordering, not presentation setup ordering, so it is unaffected. -->

## Impact

- [Program.cs](../../../Program.cs): lines 35-38 (the four `SetupPresentation()` calls) and line 94-102 (code that depends on `Class.Instance.SetupPresentation()` having completed).
- [Student.cs](../../../Student.cs), [Class.cs](../../../Class.cs), [Teacher.cs](../../../Teacher.cs), [Course.cs](../../../Course.cs): no internal changes expected — only the call-site timing in `Program.cs` changes.
- No API, dependency, or data-model changes. Purely a startup-sequencing change confined to `Program.cs`.
