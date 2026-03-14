## Why

Module startup time is visibly slow — there is a noticeable delay between the module being loaded and the UI becoming ready with data. Users experience a spinner or blank list pane for several seconds after login, which hurts perceived responsiveness and first-impression quality.

## What Changes

- Scope course data loading to the current school year/semester only, instead of fetching all historical courses on startup
- Move expensive background syncs to fire as early as possible, before UI setup begins
- Build in-memory reverse indexes (student→class, class→teacher) at load time to eliminate O(n) scans during incremental updates
- Move Aspose license initialization to a background thread so it does not block the main thread
- Guard `FillFilter()` and `SetSource()` calls to no-op when data is not yet loaded, preventing redundant iterations

## Capabilities

### New Capabilities

- `semester-scoped-course-loading`: Course data is loaded per-semester rather than all-time, dramatically reducing payload for schools with years of historical data
- `startup-background-sync`: Data sync for Class, Student, Teacher, and Course begins before UI setup, running in parallel with presentation initialization

### Modified Capabilities

<!-- No existing spec-level behavior changes — all modifications are implementation-level optimizations -->

## Impact

- [Program.cs](Program.cs): Ordering of `SyncAllBackground()` calls relative to `Framework.Program.Initial()` and `SetupPresentation()`
- [Course.cs](Course.cs) / [Feature/QueryCourse.cs](Feature/QueryCourse.cs): `GetAllData()` now uses semester-scoped query
- [Student.cs](Student.cs): `_ClassStudents` / `_StudentClassMap` reverse index built on `ItemLoaded`
- [Class.cs](Class.cs): `_TeacherSupervised` / `_ClassTeacherMap` reverse index built on `ItemLoaded`
- [LegacyPresentBase.cs](LegacyPresentBase.cs): `SetSource()` guard for unloaded state
- No API changes, no breaking changes for other modules
