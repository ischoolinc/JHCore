## 1. Course Semester-Scoped Loading

- [x] 1.1 Add `_hasLoadedSemester` and `_loadedSemester` fields to `Course` to track which semester is currently in cache
- [x] 1.2 Update `Course.GetAllData()` to call `QueryCourse.GetAllCourses(schoolYear, semester)` using the tracked semester instead of `GetAllCourses()` (no-arg)
- [x] 1.3 Update `Course.FillFilter()` popup handler so that selecting an already-loaded semester calls `SetSource()` instead of `SyncAllBackground()`
- [ ] 1.4 Verify the Course filter menu correctly shows semesters from loaded data and allows switching to trigger a new load

## 2. Startup Background Sync Ordering

- [x] 2.1 Move the four `SyncAllBackground()` calls in `Program.Main()` to immediately after `Framework.Program.Initial()`, before any `SetupPresentation()` call
- [ ] 2.2 Confirm that the DSA connection is established by `Framework.Program.Initial()` so background syncs can proceed safely

## 3. Aspose Background Initialization

- [x] 3.1 Wrap all Aspose license `SetLicense()` calls in `Program.Main()` inside `Task.Run()`
- [ ] 3.2 Verify that reports (Words, BarCode, PDF) still function correctly after the change

## 4. FillFilter / SetSource Guards

- [x] 4.1 Add `if (!_Initilized || !Loaded) return;` guard at the top of `Student.FillFilter()`
- [x] 4.2 Add `if (!_Initilized || !Loaded) return;` guard at the top of `Class.FillFilter()`
- [x] 4.3 Add `if (!_Initilized || !Loaded) return;` guard at the top of `Teacher.FillFilter()`
- [x] 4.4 Add `if (!Loaded) return;` guard at the top of `LegacyPresentBase.SetSource()` to prevent premature iteration
- [ ] 4.5 Verify no empty-list flash occurs at startup by testing with a large dataset

## 5. Reverse Index for Student-Class Lookup

- [x] 5.1 Add `_ClassStudents` (`Dictionary<string, List<StudentRecord>>`) and `_StudentClassMap` (`Dictionary<string, string>`) fields to `Student`
- [x] 5.2 Implement `ItemLoaded` handler in `Student` constructor to fully rebuild both indexes from `this.Items`
- [x] 5.3 Implement `ItemUpdated` handler in `Student` constructor to remove old entries via `_StudentClassMap` and re-add updated students
- [x] 5.4 Update `Student.GetClassStudents(ClassRecord)` to return from `_ClassStudents` instead of scanning `Items`
- [x] 5.5 Add `lock (_ClassStudents)` guards around all index read/write operations

## 6. Reverse Index for Class-Teacher Lookup

- [x] 6.1 Add `_TeacherSupervised` (`Dictionary<string, List<ClassRecord>>`) and `_ClassTeacherMap` (`Dictionary<string, string>`) fields to `Class`
- [x] 6.2 Implement `ItemLoaded` handler in `Class` constructor to fully rebuild both indexes from `this.Items`
- [x] 6.3 Implement `ItemUpdated` handler in `Class` constructor to remove old entries via `_ClassTeacherMap` and re-add updated classes
- [x] 6.4 Update `Class.GetTecaherSupervisedClass(TeacherRecord)` to return from `_TeacherSupervised` instead of scanning `Items`
- [x] 6.5 Add `lock (_TeacherSupervised)` guards around all index read/write operations

## 7. Verification

- [ ] 7.1 Test startup on a school with multiple years of course data — confirm Course panel loads significantly faster
- [ ] 7.2 Test semester switching in the Course panel — confirm correct data appears and only one network request fires per new semester
- [ ] 7.3 Test student import/update — confirm `_ClassStudents` index reflects changes correctly
- [ ] 7.4 Test class update — confirm `_TeacherSupervised` index reflects changes correctly
- [ ] 7.5 Measure and record approximate startup time before and after for comparison
