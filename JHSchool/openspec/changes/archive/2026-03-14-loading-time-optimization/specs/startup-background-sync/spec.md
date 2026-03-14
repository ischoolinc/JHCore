## ADDED Requirements

### Requirement: Data sync begins before UI setup
The system SHALL initiate background data synchronization for all four entity types (Class, Student, Teacher, Course) before beginning presentation setup, so that network I/O overlaps with main-thread UI initialization.

#### Scenario: SyncAllBackground fires before SetupPresentation
- **WHEN** the module's `Main()` entry point executes
- **THEN** `SyncAllBackground()` SHALL be called for Class, Student, Teacher, and Course before any `SetupPresentation()` call is made

### Requirement: Aspose license initialization runs off the main thread
The system SHALL initialize Aspose component licenses on a background thread so that the main thread is not blocked during startup.

#### Scenario: License setup does not delay window appearance
- **WHEN** the module starts
- **THEN** Aspose license initialization SHALL be dispatched to a background thread via `Task.Run()`
- **THEN** the main window SHALL become visible without waiting for license initialization to complete

### Requirement: FillFilter and SetSource are no-ops before data is ready
The system SHALL skip filter and source update operations when data has not yet been loaded, to prevent empty-list flashes and redundant iterations.

#### Scenario: FillFilter called before ItemLoaded
- **WHEN** `FillFilter()` is called and `Loaded` is false
- **THEN** the method SHALL return immediately without modifying the panel's filtered source

#### Scenario: SetSource called before presentation is initialized
- **WHEN** `SetSource()` is called and `_Initilized` is false
- **THEN** the method SHALL return immediately without invoking `FillFilter()` or `SetFilteredSource()`

### Requirement: In-memory reverse indexes are maintained for fast lookup
The system SHALL maintain reverse-lookup indexes that enable O(1) resolution of class→students and teacher→supervised-classes relationships, avoiding O(n) full-collection scans on incremental updates.

#### Scenario: Student-to-class index is built on load
- **WHEN** `Student.ItemLoaded` fires
- **THEN** `_ClassStudents` (classID → List<StudentRecord>) and `_StudentClassMap` (studentID → classID) SHALL be fully rebuilt from the loaded data

#### Scenario: Student index is updated incrementally
- **WHEN** `Student.ItemUpdated` fires with a set of changed student IDs
- **THEN** only those students SHALL be removed from and re-added to the indexes, without rebuilding the entire collection

#### Scenario: Class-to-teacher index is built on load
- **WHEN** `Class.ItemLoaded` fires
- **THEN** `_TeacherSupervised` (teacherID → List<ClassRecord>) and `_ClassTeacherMap` (classID → teacherID) SHALL be fully rebuilt from the loaded data

#### Scenario: Class index is updated incrementally
- **WHEN** `Class.ItemUpdated` fires with a set of changed class IDs
- **THEN** only those classes SHALL be removed from and re-added to the indexes, without rebuilding the entire collection
