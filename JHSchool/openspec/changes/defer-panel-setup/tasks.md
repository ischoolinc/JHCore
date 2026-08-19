## 1. Confirm assumptions before coding

- [x] 1.1 Confirm which panel is actually shown first when the JHSchool module opens — confirmed via static analysis: Student is the only panel whose `SetupPresentation()` was first in call order and no other startup logic overrides panel selection; kept as the eager/primary panel. Final runtime confirmation still recommended when a maintainer next runs the app (see task 3.2, not performed in this session — no interactive Windows GUI/login environment available here).
- [x] 1.2 Searched the repo for code outside `Program.cs` touching `Class.Instance`/`Teacher.Instance`/`Course.Instance` presentation state right after module load — none found. The only such dependent code was the `IClassBaseInfoItemAPI`-gated `Class.Instance.AddDetailBulider(...)` block already inside `Program.cs`, which is handled by task 2.4. Also discovered each panel's `SetupPresentation()` itself ends with `MotherForm.AddPanel(...)`, which is what registers that panel into the navigation shell — confirming panel navigation registration is correctly covered by deferring the whole `SetupPresentation()` call, not just part of it.

## 2. Implement deferred setup in Program.cs

- [x] 2.1 Kept `Student.Instance.SetupPresentation()` synchronous at its original location (Program.cs:34)
- [x] 2.2 Removed the other three `SetupPresentation()` calls from their inline position
- [x] 2.3 Added an `Application.Idle` event handler (`deferredPanelSetup`), subscribed right after `Student.Instance.SetupPresentation()`, that on first fire unsubscribes itself then calls `Class`/`Teacher`/`Course` `SetupPresentation()` in original order
- [x] 2.4 Moved the `IClassBaseInfoItemAPI`-gated `Class.Instance.AddDetailBulider(...)` block into the same deferred callback, immediately after `Class.Instance.SetupPresentation()`
- [x] 2.5 Confirmed nothing else in `Main()` (StartMenu wiring, Aspose license `Task.Run`, `SelectedListChanged()`, logging) depends on the moved calls — left untouched at original positions
- [x] 2.6 Added comments at the deferred block explaining the `Application.Idle` deferral and directing future dependent code to be added inside `deferredPanelSetup`

## 3. Verification

- [x] 3.1 Built the solution (`dotnet build JHSchool.csproj -c Debug`) — 0 errors, 87 pre-existing warnings unrelated to this change
- [x] 3.2 Launch the module and confirm the primary panel appears and is fully usable (ribbon buttons, filters, detail views all present) — **verified manually by user**
- [x] 3.3 Measure/observe time from module load to primary panel becoming interactive, compared to before the change — **verified manually by user**
- [x] 3.4 Switch to each of the three deferred panels (Class, Teacher, Course) and confirm all ribbon buttons, filters, and detail builders are present and functional — **verified manually by user**
- [x] 3.5 Specifically verify the Class panel's basic-info detail builder (the `IClassBaseInfoItemAPI`-gated one) renders correctly — **verified manually by user**
- [x] 3.6 Test rapid panel switching immediately after startup to confirm no visible race where a panel is briefly missing its ribbon/menu wiring — **verified manually by user**
