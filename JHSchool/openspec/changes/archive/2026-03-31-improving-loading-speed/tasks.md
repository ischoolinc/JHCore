## 1. Optimize Student Data Management

- [x] 1.1 Implement lazy/background indexing for `_ClassStudents` and `_StudentClassMap` in `Student.cs`
- [x] 1.2 Implement pre-filtered status sets (`_StatusMaps`) in `Student.cs`
- [x] 1.3 Refactor `Student.FillFilter` to use `_StatusMaps` instead of iterating over all `Items`
- [x] 1.4 Ensure incremental updates to `_StatusMaps` in `Student.ItemUpdated`

## 2. Optimize Class Data Management

- [x] 2.1 Refactor `Class.cs` to build `_TeacherSupervised` and `_ClassTeacherMap` in the background or lazily
- [x] 2.2 Ensure incremental updates to `_TeacherSupervised` and `_ClassTeacherMap` in `Class.ItemUpdated`

## 3. General Framework Optimizations

- [x] 3.1 Review `LegacyPresentBase.cs` for potential UI-thread blocking operations in `SetSource` or `SetFilteredSource`
- [x] 3.2 Ensure `SyncAllBackground` calls in `Program.cs` don't cause unnecessary contention on first load

## 4. Verification

- [x] 4.1 Verify loading speed improvement with a large student dataset (simulated if necessary)
- [x] 4.2 Verify that filtering functionality remains correct (no regressions)
- [x] 4.3 Verify that incremental updates (edit student/class) correctly update indexes
