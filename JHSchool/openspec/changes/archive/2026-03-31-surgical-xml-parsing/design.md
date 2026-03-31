## Context

The current `StudentRecord` constructor uses `DSXmlHelper` to wrap an `XmlElement` and perform multiple `GetText` (XPath) calls. For 3,000 records, this results in ~39,000 XPath evaluations. This is a CPU bottleneck during initial data sync.

## Goals / Non-Goals

**Goals:**
- Eliminate `DSXmlHelper` and XPath lookups in `StudentRecord` instantiation.
- Reduce CPU cycles spent on XML parsing by ~80% during bulk loading.
- Maintain existing record property names and logic.

**Non-Goals:**
- Rewrite the `DSXmlHelper` class itself.
- Change the server-side XML schema.
- Implement data lazy-loading at this stage (focused on parsing efficiency).

## Decisions

### 1. Direct Attribute and Child Node Access
Instead of `helper.GetText("@ID")`, use `element.GetAttribute("ID")`. Instead of `helper.GetText("Status")`, use manual child node iteration or direct access.
- **Why**: `GetAttribute` and direct child iteration are native to the DOM and avoid the massive overhead of compiling and evaluating XPath strings for every field.
- **Alternatives**: Using `XmlReader`. While faster, `XmlReader` requires a fundamental change to the data-fetching pipeline which currently returns `XmlElement` lists.

### 2. Manual Property Mapping
Explicitly map fields in the constructor.
```csharp
foreach (XmlNode node in element.ChildNodes) {
    switch (node.Name) {
        case "Status": Status = node.InnerText; break;
        case "Name": Name = node.InnerText; break;
        // ...
    }
}
```
- **Why**: This ensures we only traverse the child nodes of an element once, rather than re-searching from the start for every field.

## Risks / Trade-offs

- **Maintainability**: The parsing logic becomes slightly more verbose than `DSXmlHelper.GetText`.
  - [Risk]  Harder to read/maintain.
  - Mitigation: Use a clean `switch` statement or a helper method that operates on local `XmlNode` lists rather than global XPath.
- **XML Schema Sensitivity**: Direct child iteration is sensitive to XML structure (e.g., nesting).
  - [Risk]  If the server adds nested tags with the same names, simple name matching might fail.
  - Mitigation: The current schema for `Brief` records is flat; keep the mapping logic simple but robust.
