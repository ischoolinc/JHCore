## Context

The JHSchool module loads four data sets on startup: Class, Student, Teacher, and Course. These are fetched from a remote DSA service over the network. Historically, all four were loaded synchronously or in ways that blocked the UI from becoming interactive. The `fast_loading` branch has already applied several optimizations; this document captures the design decisions behind those changes and identifies remaining opportunities.

**Current startup sequence (after optimizations):**

```
Framework.Program.Initial()          ~1 sec (auth + schema, not modifiable here)
    │
    ├── SyncAllBackground() × 4      parallel background threads (network queries)
    │
    ├── Student.SetupPresentation()  ┐
    ├── Class.SetupPresentation()    │  sequential on main thread (pure UI work)
    ├── Teacher.SetupPresentation()  │
    └── Course.SetupPresentation()   ┘
                │
        MotherForm.AddPanel() × 4    window becomes visible
                │
    (background threads finish)
    ItemLoaded → FillFilter → SetFilteredSource    data appears in list pane
```

**Key constraint**: `Framework` and `FISCA` are external libraries — their internals (e.g., `CacheManager`, `NLDPanel`, `MotherForm`) cannot be modified.

## Goals / Non-Goals

**Goals:**
- Reduce the time from module load to data visible in the list pane
- Eliminate unnecessary work on the main thread during startup
- Reduce the volume of data fetched on initial load (Course is the largest)
- Ensure incremental updates (ItemUpdated) remain fast after initial load

**Non-Goals:**
- Modifying `Framework.Program.Initial()` — it is outside this module's control
- Changing the user-visible behaviour of any panel
- Adding server-side caching or protocol changes
- Optimizing non-startup paths (search, detail pane load)

## Decisions

### Decision 1: Scope course loading to current semester only

**Problem**: `GetAllCourses()` fetches every course ever created. Schools that have used the system for multiple years accumulate thousands of historical courses, making startup proportionally slower each year.

**Decision**: `Course.GetAllData()` calls `GetAllCourses(schoolYear, semester)` using the configured default semester (`School.DefaultSchoolYear` / `School.DefaultSemester`). Switching semesters in the UI triggers a fresh `SyncAllBackground()` for that semester.

**Alternatives considered**:
- *Fetch all, filter in memory*: Still transfers the full payload; no improvement.
- *Paginate*: DSA service doesn't support cursor-based pagination; not viable.
- *Cache to disk*: Adds complexity and stale-data risk; deferred to future work.

**Trade-off**: When users switch semesters via the filter menu, a new network round-trip occurs. Acceptable — semester switching is infrequent.

---

### Decision 2: Build reverse indexes at ItemLoaded, not on-demand

**Problem**: `Class.GetTecaherSupervisedClass(teacher)` and `Student.GetClassStudents(class)` were implemented as O(n) linear scans over all items each time they were called. During `ItemUpdated` events (e.g., after an import), these were called in a loop.

**Decision**: Maintain `_ClassStudents` / `_StudentClassMap` in `Student` and `_TeacherSupervised` / `_ClassTeacherMap` in `Class`. Built fully on `ItemLoaded`, updated incrementally on `ItemUpdated`.

**Trade-off**: Slight memory overhead for the two extra dictionaries. Negligible for typical school sizes (<5,000 students, <200 classes).

---

### Decision 3: Move Aspose license initialization to a background thread

**Problem**: Aspose license setup (`Aspose.Words`, `Aspose.BarCode`, `Aspose.Pdf`) reads an embedded resource and initialises DRM — a CPU-bound operation that previously ran on the main thread.

**Decision**: Wrap in `Task.Run()`. Aspose license objects are thread-safe for initialization.

**Risk**: If a report is generated before the background task completes, the license won't be set. In practice, users cannot navigate to report features before the window is fully loaded, so the race window is effectively zero.

---

### Decision 4: Guard FillFilter / SetSource against premature invocation

**Problem**: `FillFilter()` was called at the end of each `SetupPresentation()`. At that point, background data hasn't arrived yet, so the call iterated an empty collection and called `SetFilteredSource([])`, causing a flash of an empty list before data arrived.

**Decision**: All `FillFilter()` overrides and `SetSource()` begin with `if (!_Initilized || !Loaded) return;`. The guard prevents unnecessary work and the empty-list flash.

---

### Decision 5: SyncAllBackground fires before SetupPresentation

**Decision**: The four `SyncAllBackground()` calls are placed immediately after `Framework.Program.Initial()`, before any `SetupPresentation()` calls. This maximises the overlap between network I/O (background) and UI setup (main thread).

**Constraint**: `SyncAllBackground()` requires the DSA connection to be established, which happens inside `Framework.Program.Initial()`. Calling earlier is not possible.

## Risks / Trade-offs

| Risk | Mitigation |
|------|-----------|
| Semester-switching requires a network round-trip | UX is acceptable; semester switch is rare. Could prefetch in future. |
| Aspose license race on very fast machines | Window open → report click requires several user interactions; race window is negligible. |
| `ItemLoaded` index build adds CPU work after fetch | O(n) over in-memory objects; <5ms for typical school sizes. |
| Course filter popup only shows loaded semester | By design — users select different semesters to trigger load of that semester. |

## Migration Plan

All changes are contained within this module. No database migrations, no API changes, no changes to other modules.

1. Merge `fast_loading` branch to `master`
2. Verify startup timing manually on a school with multiple years of data
3. Verify semester switching in Course panel still works correctly
4. Verify reports still function (Aspose license)

Rollback: revert the merge commit.

## Open Questions

- Should we prefetch the adjacent semester (e.g., current ± 1) in background after the initial load to make semester switching instant?
- Is `Framework.Program.Initial()` timing measurable / improvable from the outside, or is it truly a black box?
- Are there other module-level DSA service calls at startup (outside this module) that could be similarly scoped?
