## Why

The current system experiences slow loading speeds during startup and data refresh, especially in environments with large amounts of student and class data. This is primarily caused by redundant full-list iterations in the UI thread's event handlers and inefficient data indexing.

## What Changes

- **Optimize Data Indexing**: Refactor `Student.cs` and `Class.cs` to build internal maps (e.g., `_ClassStudents`, `_TeacherSupervised`) more efficiently, potentially utilizing lazy loading or optimized iteration patterns.
- **Reduce UI Thread Blocking**: Ensure that heavy data processing during `ItemLoaded` and `FillFilter` does not block the UI thread unnecessarily.
- **Optimize Filter Logic**: Streamline the `FillFilter` method in `Student.cs` to avoid multiple redundant iterations over the entire student list when multiple filters are applied.
- **Improve Startup Sequence**: Analyze and potentially defer non-critical initialization tasks to after the main UI is responsive.

## Capabilities

### New Capabilities
- `performance-optimization-framework`: A set of utilities or patterns to handle large data sets in the legacy presentation layer without blocking the UI.

### Modified Capabilities
- `student-management`: Improve loading and filtering performance for student data.
- `class-management`: Improve loading and indexing performance for class data.

## Impact

- **Affected Code**: `Student.cs`, `Class.cs`, `Program.cs`, `LegacyPresentBase.cs`.
- **APIs**: No change to public APIs, but internal data management logic will be updated.
- **Dependencies**: No new external dependencies.
- **Systems**: Improved responsiveness of the JHSchool application during startup and data navigation.
