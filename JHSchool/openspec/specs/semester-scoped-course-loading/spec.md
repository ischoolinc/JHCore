### Requirement: Course data is scoped to a single semester on load
The system SHALL load only the courses belonging to the currently selected school year and semester when initializing the Course panel, rather than fetching all historical course data.

#### Scenario: Initial load uses default semester
- **WHEN** the module starts up
- **THEN** only courses matching `School.DefaultSchoolYear` and `School.DefaultSemester` are fetched from the server

#### Scenario: Loaded semester is tracked in memory
- **WHEN** courses are loaded for a given semester
- **THEN** the system SHALL record which semester was loaded so that redundant reloads are avoided

### Requirement: Switching semesters fetches the target semester from server
When the user selects a different semester in the Course filter menu, the system SHALL fetch that semester's courses if they have not already been loaded.

#### Scenario: User selects an unloaded semester
- **WHEN** the user selects a semester from the Course filter menu that differs from the currently loaded semester
- **THEN** the system SHALL call `SyncAllBackground()` to fetch the new semester's data from the server

#### Scenario: User re-selects the already-loaded semester
- **WHEN** the user selects the same semester that is currently loaded
- **THEN** the system SHALL NOT make a new network request and SHALL instead call `SetSource()` to refresh the display from cached data

### Requirement: Course filter menu shows only loaded semester options
The filter menu SHALL reflect the semesters present in the currently loaded course data.

#### Scenario: Only one semester loaded
- **WHEN** only the current semester has been loaded
- **THEN** the filter menu SHALL display only that semester as a selectable option

#### Scenario: Multiple semesters loaded over session
- **WHEN** the user has switched between semesters during the session
- **THEN** the filter menu SHALL display all semesters whose data is present in memory
