## ADDED Requirements

### Requirement: Lazy construction of reverse indexes
The system SHALL NOT rebuild the entire class-to-student or teacher-to-supervised-class index during `ItemLoaded` on the UI thread. Instead, it SHALL construct these indexes lazily on the first request, or perform the construction on a background thread.

#### Scenario: Index is empty until accessed
- **WHEN** `Student.ItemLoaded` fires
- **THEN** `_ClassStudents` SHALL NOT be iterated or populated immediately
- **THEN** the first call to `GetClassStudents()` SHALL trigger the index population if it is empty

#### Scenario: Background index population
- **WHEN** data loading completes in the background
- **THEN** the system SHALL attempt to build the reverse indexes in the same background thread before firing `ItemLoaded`

### Requirement: Optimized status filtering
The system SHALL use pre-filtered collections for common status filters (e.g., "一般", "輟學") to avoid full-list iterations during `FillFilter`.

#### Scenario: Filtering for "一般" status
- **WHEN** the user selects only the "一般" status filter
- **THEN** `FillFilter` SHALL use the pre-calculated `_StatusMaps["一般"]` instead of iterating over `Items`
