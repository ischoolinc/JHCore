## ADDED Requirements

### Requirement: Optimized record instantiation from XML
The `StudentRecord` class SHALL optimize the process of creating objects from server-provided XML by avoiding XPath queries and redundant parser allocations during bulk loading.

#### Scenario: Instantiating StudentRecord from XmlElement
- **WHEN** a `StudentRecord` is created from a `GetStudentListResponse` item
- **THEN** it SHALL extract fields like `ID`, `Name`, and `Status` using direct attribute or child node access
- **THEN** it SHALL NOT allocate a new `DSXmlHelper` instance per record
