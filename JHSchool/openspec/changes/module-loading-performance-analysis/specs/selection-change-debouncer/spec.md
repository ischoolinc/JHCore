## ADDED Requirements

### Requirement: Selection change event is debounced
The system MUST introduce a short debounce delay (100–150ms) between a selection change on the main list and the triggering of background data fetching for the active detail panels.

#### Scenario: Rapid selection changes skip loading intermediate items
- **WHEN** the user rapidly changes selection on the main list (e.g., holding down the keyboard's down-arrow key) such that the interval between changes is less than the debounce threshold
- **THEN** the system SHALL cancel any pending load timers and SHALL NOT spawn background workers or network requests for intermediate records

#### Scenario: Stopping on a record triggers load after threshold
- **WHEN** the user pauses or stops on a record, and the selection remains stable for longer than the debounce threshold
- **THEN** the system SHALL execute the background workers for all active/expanded detail panels for that record
