## Context

The JHSchool application uses a legacy presentation layer (`LegacyPresentBase<T>`) that heavily relies on background synchronization and in-memory caching. While the synchronization itself is handled in the background, the UI thread often performs expensive operations when data is loaded or when filters are applied. These operations include full-list iterations and full-dictionary rebuilds, which significantly impact loading performance in schools with large student populations.

## Goals / Non-Goals

**Goals:**
- Eliminate UI thread blocking during data loading and filtering.
- Optimize reverse-index construction for student-to-class and class-to-teacher mappings.
- Streamline status-based filtering to use pre-calculated sets.
- Improve initial startup responsiveness.

**Non-Goals:**
- Completely rewrite the synchronization mechanism.
- Migrate to a different database or data access framework.
- Change the existing UI layout or user experience.

## Decisions

### 1. Lazy or Background Index Construction
Instead of iterating through thousands of records on the UI thread during `ItemLoaded`, we will build these indexes in the background thread that fetches the data, or lazily on first access.
- **Why**: The UI thread should be dedicated to rendering. Building a dictionary of thousands of items is pure data processing and belongs off the UI thread.
- **Alternatives**: Keeping the current logic but optimizing the loops. However, even optimized loops can block for 100-200ms with large datasets, which is noticeable.

### 2. Status-Based Pre-Filtered Sets
Maintain a `Dictionary<string, HashSet<string>>` where the key is the student status and the value is a set of student IDs.
- **Why**: `FillFilter` currently iterates over all students to check their status. With pre-filtered sets, combining multiple status filters becomes a series of `UnionWith` operations on small sets, which is much faster.
- **Alternatives**: Using LINQ. While LINQ is concise, it still performs full-list iteration behind the scenes unless carefully optimized.

### 3. Incremental Updates for Mappings
Refactor `ItemUpdated` handlers to only update the affected entries in the mappings instead of potentially rebuilding anything from scratch.
- **Why**: Currently, `ItemUpdated` is relatively efficient in `Student.cs` (using a reverse map), but we should ensure all entities follow this pattern and that the initial load is also optimized.

## Risks / Trade-offs

- **Memory Usage**: Storing pre-filtered status sets increases memory consumption slightly.
  - [Risk]  Memory overhead for large datasets.
  - Mitigation: Student IDs are strings; storing them in multiple `HashSet`s will consume some MBs, but this is acceptable in modern environments compared to UI lag.
- **Complexity**: Background thread management and lazy loading add complexity to the data management code.
  - [Risk]  Thread safety issues (race conditions).
  - Mitigation: Use appropriate locks or thread-safe collections when building and accessing indexes.
