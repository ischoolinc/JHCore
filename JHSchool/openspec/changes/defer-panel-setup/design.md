## Context

`Program.Main()` (the JHSchool module's `[ApplicationMain()]` entry point, invoked by the host FISCA/K12 shell after it has already created its own window) runs four `SetupPresentation()` calls back to back:

```csharp
Student.Instance.SetupPresentation();
Class.Instance.SetupPresentation();
Teacher.Instance.SetupPresentation();
Course.Instance.SetupPresentation(); //課程的類別已調整
```

Each is a large method (Teacher's alone spans ~320 lines) that registers ribbon buttons, list-pane fields/filters, `AddView(...)` calls, and detail builders via `AddDetailBulider(...)`, plus in Student's and Teacher's case one `FISCA.InteractionService.DiscoverAPI<T>()` reflection lookup. None of this reads data from `SyncAllBackground()` — it is pure static UI/menu wiring. A prior author's inline comment (`//1.1 秒`) confirms this block was already measured at roughly 1.1 seconds, on top of the `//1秒` cost of `Framework.Program.Initial()` just above it.

Only one panel is what the user lands on first. Deferring the other three panels' construction moves ~0.8s (three quarters of the measured 1.1s, if the cost is roughly even across panels) off the critical path between module load and the window becoming usable, without removing any functionality.

This is a small, single-file (`Program.cs`) sequencing change — no new dependencies, no data model changes. A design doc is included because getting the deferral *ordering* right is the one place this can silently break other startup code.

## Goals / Non-Goals

**Goals:**
- Make the initially-visible panel interactive as soon as possible after module load.
- Defer the other three panels' `SetupPresentation()` work to run shortly after the window is shown, without dropping or reordering any of the wiring those methods perform relative to each other.
- Keep the change mechanically simple and confined to `Program.cs` so it stays low-risk.

**Non-Goals:**
- Not changing what any panel's ribbon/menu/detail-builder wiring does — only when it runs.
- Not attempting to lazy-init on first panel *visit* (e.g. hooking a tab-activation event). The host's `NLDPanel`/`MotherForm` navigation lives in the external FISCA assembly and no such per-panel activation hook was found in this repo; inventing dependence on an unverified external hook is riskier than a simple post-show deferral. This can be revisited later if such a hook is confirmed to exist.
- Not touching `SyncAllBackground()` ordering, XML parsing, or index-building — those are already covered by prior changes (`startup-background-sync`, `xml-parsing-optimization`, `performance-optimization-framework`).
- Not profiling or changing `FISCA.InteractionService.DiscoverAPI<T>()` itself — its implementation is outside this repo.

## Decisions

**Decision: Determine the primary (kept-eager) panel by testing, default assumption is Student.**
Student is `SetupPresentation()`'s first call and is the conventional default landing panel for this module family. Before implementing, confirm which panel the user actually sees first when the module opens (manual check or asking a maintainer) — do not assume from call order alone. If uncertain, keep Student eager as the safe default since it's first today.

**Decision: Defer via `System.Windows.Forms.Application.Idle`, fired once.**
Alternatives considered:
- *`Task.Run(...)` on a background thread*: rejected — `SetupPresentation()` touches WinForms UI objects (ribbon items, controls) and must run on the UI thread. Running it on a thread-pool thread would cross-thread-violate WinForms and crash or corrupt UI state.
- *`Control.BeginInvoke` on some window handle*: works, but requires a valid, already-created handle at the point of scheduling, which may not exist yet this early in `Main()`. `Application.Idle` doesn't need a handle reference and fires naturally once the message loop starts processing (i.e., once the host has shown its window and is idle), which is exactly the point after which we want deferred work to run.
- *`Application.Idle` (chosen)*: runs on the UI thread, requires no handle, and fires as soon as the message loop is idle — reliably after the window is up and responsive to input. Must unsubscribe on first fire to avoid re-running on every idle cycle.

**Decision: Preserve relative ordering among the three deferred panels, and against downstream dependents.**
Program.cs:97 (`Class.Instance.AddDetailBulider(...)`, gated on `FISCA.InteractionService.DiscoverAPI<IClassBaseInfoItemAPI>()`) already runs *after* line 36 (`Class.Instance.SetupPresentation()`) in the current sequential code, and relies on `Class`'s panel/ribbon state existing. If `Class.Instance.SetupPresentation()` moves into the deferred `Application.Idle` callback, anything that depends on it (line 94-102, and by extension anything a maintainer adds later expecting `SetupPresentation()` semantics to already hold) must move into that same deferred callback, in the same relative order it has today. Concretely: the deferred callback should run `Class.Instance.SetupPresentation(); Teacher.Instance.SetupPresentation(); Course.Instance.SetupPresentation();` followed immediately by the `IClassBaseInfoItemAPI` detail-builder block (current lines 93-102), preserving today's ordering exactly, just shifted later as one unit.

**Decision: Everything between the current lines 39-92 (Aspose license Task.Run, log, StartMenu wiring, `SelectedListChanged()`) stays exactly where it is.**
None of it depends on the three deferred `SetupPresentation()` calls, so there's no reason to move it and every reason not to (minimizes diff, minimizes risk).

## Risks / Trade-offs

- **[Risk] A maintainer adds new code between the old lines 38 and the end of `Main()` that assumes `Class`/`Teacher`/`Course` presentation is already set up, without realizing it's now deferred.** → Mitigation: leave a clear comment at both the deferral call site and near the top of `Main()` stating that Class/Teacher/Course `SetupPresentation()` now run on `Application.Idle`, and that any code depending on their ribbon/detail-builder state must go inside that same deferred block.
- **[Risk] `Application.Idle` fires before the window is actually painted/visible in some host shell timing edge case, reducing the benefit.** → Mitigation: this is still strictly no worse than today (fully synchronous); even if `Idle` fires slightly earlier than ideal, `Main()` itself returns faster, which is the actual lever the host uses to unblock its own UI thread.
- **[Risk] Deferred panels (Class/Teacher/Course) are visited by the user before their `Application.Idle` callback has run, if the user switches panels extremely fast.** → Mitigation: `Application.Idle` fires as soon as the message loop has no pending messages, which in practice is near-immediate after `Main()` returns and well before a human can navigate to another panel; this should be verified during manual testing (see tasks.md) rather than assumed.
- **[Trade-off] This does not reduce total startup CPU work — it only reorders it relative to window-visible time.** Total time-to-fully-ready is roughly unchanged; the win is purely in perceived responsiveness (time to first interaction). This matches the actual user complaint pattern seen in prior related changes ("spinner or blank list pane for several seconds").

## Migration Plan

1. Implement the `Application.Idle`-based deferral in `Program.cs` only.
2. Manually test: module load → confirm primary panel is visible and usable quickly → switch to each deferred panel and confirm ribbon buttons, filters, and detail builders are present and functional (no missing menu items).
3. Rollback is trivial: this is a single-file, low-diff change — reverting `Program.cs` to call all four `SetupPresentation()` methods synchronously restores prior behavior exactly.

## Open Questions

- Which panel is actually shown first when the module opens — confirm Student is correct before finalizing which panel stays eager.
- Should the deferred block also wrap the `IClassBaseInfoItemAPI` DiscoverAPI/detail-builder code (current Program.cs:93-102), or does anything else in the host application read `Class.Instance`'s detail builders before `Application.Idle` fires? To be confirmed during implementation via a repo-wide search for early `Class.Instance` usage from other modules, if any exist outside this repo.
