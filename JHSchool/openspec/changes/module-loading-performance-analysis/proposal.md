## Why

Currently, when launching the JHSchool module or navigating between records (Students, Classes, Teachers, Courses), the application experiences severe responsiveness bottlenecks and produces a high volume of redundant network/database requests. This is caused by eager background loading across multiple concurrently registered detail panels (even when collapsed) and the absence of scrolling debouncing, which saturates thread pools and connection limits.

## What Changes

- **Implement Lazy Loading in Detail Panels**: Introduce a mechanism to defer data loading (`LoadContent` / `_BGWorker.RunWorkerAsync()`) for all detail content panels that are currently collapsed (not expanded/visible), executing queries only when a user expands a panel.
- **Add Selected List Debouncing**: Introduce a short debounce delay (100–150ms) on selection change before initiating background data-fetching workers, effectively eliminating intermediate load requests when a user scrolls rapidly through lists.
- **Cache Reuse and Optimization**: Refactor detail panels to read common static configurations (e.g., Classes, Teachers, Departments) from global memory caches instead of querying the backend service individually.

## Capabilities

### New Capabilities
- `detail-panel-lazy-loading`: Ensures detail content panels only load their data via background workers when they are expanded/visible to the user.
- `selection-change-debouncer`: Delays selection-triggered background workers by 100-150ms during list scrolling to throttle network queries.

### Modified Capabilities
<!-- No requirement changes to existing functional capabilities -->

## Impact

- **Affected Code**: `LegacyPresentBase.cs`, `PalmerwormItem.cs`, and individual detail content views (e.g., `BaseInfoPalmerwormItem.cs`, `ClassBaseInfoItem.cs`, `TeachCourseItem.cs`).
- **APIs**: No breaking changes to existing public APIs.
- **Dependencies**: Uses standard WinForms/Windows Timer components for debouncing.
- **Systems**: Greatly reduces database and network load on the DSA server, significantly speeding up startup times and making navigation between records smooth and snappy.
