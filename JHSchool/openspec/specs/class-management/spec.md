## ADDED Requirements

### Requirement: Optimized class-to-teacher mapping
The `Class` class SHALL optimize the `ItemLoaded` handler to build the `_TeacherSupervised` mapping efficiently, ensuring it does not block the UI thread.

#### Scenario: Class-to-teacher mapping on background load
- **WHEN** class data is fetched from the server in the background
- **THEN** the mapping from teacher to class SHALL be built in the same background thread

#### Scenario: Incremental update of class-to-teacher mapping
- **WHEN** only specific classes are updated
- **THEN** only the affected teacher-to-class mappings SHALL be refreshed rather than a full rebuild
