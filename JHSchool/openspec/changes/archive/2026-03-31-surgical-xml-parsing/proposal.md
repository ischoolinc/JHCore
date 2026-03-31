## Why

The current XML parsing in `StudentRecord.cs` (and potentially other record types) is highly inefficient, creating a new `DSXmlHelper` instance per student and performing multiple XPath queries (GetText) per record. In schools with thousands of students, this results in tens of thousands of CPU-intensive XPath evaluations, significantly slowing down the initial data load.

## What Changes

- **Surgical XML Extraction**: Replace `DSXmlHelper` and XPath-based extraction in `StudentRecord` constructors with direct `XmlElement` attribute/node access.
- **Iteration Optimization**: Shift from random-access XPath queries to a single-pass iteration of child nodes where appropriate.
- **Resource Efficiency**: Eliminate the redundant allocation of `DSXmlHelper` objects during batch record creation.

## Capabilities

### New Capabilities
- `xml-parsing-optimization`: High-performance XML to Object mapping patterns for record types.

### Modified Capabilities
- `student-management`: Improve the data instantiation speed for student records.

## Impact

- **Affected Code**: `StudentRecord.cs`, `QueryStudent.cs`, and potentially other `*Record.cs` files if the pattern is applied broadly.
- **APIs**: No external API changes; purely an internal implementation optimization.
- **Dependencies**: Reduces reliance on `DSXmlHelper` for batch processing.
- **Systems**: Significant reduction in CPU usage during the "Loading..." phase of the application.
