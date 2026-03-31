# xml-parsing-optimization

## ADDED Requirements

### Requirement: Performance-oriented XML Field Extraction
The system SHALL provide or follow patterns for direct XML field extraction that minimize CPU cycles and memory allocations compared to generic XPath-based helpers.

#### Scenario: Surgical field extraction
- **WHEN** multiple fields must be extracted from a repeatable XML element
- **THEN** the system SHALL prefer single-pass iteration of child nodes or direct attribute lookups
- **THEN** the use of `SelectSingleNode` or `DSXmlHelper.GetText` SHALL be avoided in performance-critical loops
