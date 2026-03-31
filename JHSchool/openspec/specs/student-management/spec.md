## ADDED Requirements

### Requirement: Optimized student list iteration
The `Student` class SHALL optimize the `FillFilter` method to avoid multiple redundant iterations over the entire student list when filtering by status.

#### Scenario: User applies multiple status filters
- **WHEN** the user selects multiple status filters (e.g., "一般" and "輟學")
- **THEN** `FillFilter` SHALL combine pre-filtered status sets using bitwise or set-union operations rather than re-filtering `Items`

#### Scenario: Background building of status maps
- **WHEN** `Student.ItemLoaded` fires
- **THEN** the system SHALL build a `Dictionary<string, HashSet<string>>` mapping each status to its student IDs on a background thread

### Requirement: Optimized record instantiation from XML
The `StudentRecord` class SHALL optimize the process of creating objects from server-provided XML by avoiding XPath queries and redundant parser allocations during bulk loading.

#### Scenario: Instantiating StudentRecord from XmlElement
- **WHEN** a `StudentRecord` is created from a `GetStudentListResponse` item
- **THEN** it SHALL extract fields like `ID`, `Name`, and `Status` using direct attribute or child node access
- **THEN** it SHALL NOT allocate a new `DSXmlHelper` instance per record
