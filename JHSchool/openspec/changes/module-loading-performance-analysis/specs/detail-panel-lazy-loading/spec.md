## ADDED Requirements

### Requirement: Detail content loads only when expanded
The system MUST defer background data fetching for all detail content panels (inheriting from `FISCA.Presentation.DetailContent` or `PalmerwormItem`) if they are currently collapsed or hidden. Background loading and UI binding SHALL only be executed when the panel becomes expanded or visible.

#### Scenario: Collapsed panel does not trigger load on selection change
- **WHEN** the user selects a new record on the main list while a detail panel is collapsed
- **THEN** the system SHALL NOT invoke the background worker or issue server requests for that collapsed panel

#### Scenario: Expanding a panel triggers data load
- **WHEN** the user expands a collapsed detail panel
- **THEN** the system SHALL immediately execute the background worker to load data for the currently selected record

### Requirement: Expanded panel loads current active ID
When a previously collapsed detail panel is expanded, it MUST load the data for the current active selection, even if the selection was changed multiple times while the panel was collapsed.

#### Scenario: Lazy loaded panel updates with current selection
- **WHEN** the user changes the selection multiple times while a detail panel is collapsed, and then expands the panel
- **THEN** the system SHALL load the data matching the current active selection ID rather than any stale or intermediate selection IDs
