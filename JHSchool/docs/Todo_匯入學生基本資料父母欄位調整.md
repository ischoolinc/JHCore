## Goal

Update the student basic data import process so that Excel files can use `家長1` and `家長2` column headers while the internal import field names and generated XML structure continue to use the existing `父親` and `母親` definitions.

The change must not modify unrelated program behavior or the existing XML storage structure.

After implementation and testing, record all changes in:

`匯入學生基本資料調整.md`

## Main Requirement

Add the following Excel-to-internal field-name mappings:

```text
Excel column header       Internal import field
家長1姓名            →    父親姓名
家長1身分證號        →    父親身分證號
家長1國籍            →    父親國籍
家長1存歿            →    父親存歿

家長2姓名            →    母親姓名
家長2身分證號        →    母親身分證號
家長2國籍            →    母親國籍
家長2存歿            →    母親存歿
```

If grouped parent-information columns are supported, also map:

```text
家長1其它資訊:*      →    父親其它資訊:*
家長2其它資訊:*      →    母親其它資訊:*
```

Examples:

```text
家長1其它資訊:學歷   →    父親其它資訊:學歷
家長1其它資訊:職業   →    父親其它資訊:職業
家長1其它資訊:電話   →    父親其它資訊:電話

家長2其它資訊:學歷   →    母親其它資訊:學歷
家長2其它資訊:職業   →    母親其它資訊:職業
家長2其它資訊:電話   →    母親其它資訊:電話
```

## Expected Data Flow

The required data flow is:

```text
Excel header
    家長1姓名
        ↓
Import alias normalization
    父親姓名
        ↓
Existing BulkColumn mapping
        ↓
Existing validation rules
        ↓
GenerateInsertRequest / GenerateUpdateRequest
        ↓
Existing father XML structure
```

And:

```text
Excel header
    家長2姓名
        ↓
Import alias normalization
    母親姓名
        ↓
Existing BulkColumn mapping
        ↓
Existing validation rules
        ↓
GenerateInsertRequest / GenerateUpdateRequest
        ↓
Existing mother XML structure
```

The generated XML must remain exactly compatible with the existing import service.

## Files to Inspect

Primary file:

* `StudentImportWizard.cs`

Also inspect the implementation of:

* `WizardContext`
* `WizardContext.RefreshImportSource()`
* `SheetColumn`
* `SheetColumnCollection`
* `SheetReader`
* `BulkDescription`
* `BulkColumn`
* `BulkColumnCollection`
* `GenerateInsertRequest()`
* `GenerateUpdateRequest()`

Reference resource:

* `JH_S_BulkDescription`

Do not modify the bulk-description XML unless there is no safe runtime solution. The expected solution is to keep the XML resource unchanged.

## Current Import Flow

The current import process:

1. Loads the original bulk-description XML:

```csharp
XmlElement bulk = StudentBulkProcess.GetBulkDescription();
Context.BulkDescription = new BulkDescription(bulk);
```

2. Reads Excel headers:

```csharp
Context.RefreshImportSource();
```

3. Creates `Context.SourceColumns`.

4. Compares Excel column names against:

```csharp
Context.AcceptColumns
Context.BulkDescription.Columns
```

5. Creates:

```csharp
Context.SelectedFields
Context.ValidateColumns
```

6. Retrieves the matching `BulkColumn` objects:

```csharp
BulkColumnCollection columns =
    Context.BulkDescription.Columns.GetColumnList(
        Context.ValidateColumns.GetNames());
```

7. Generates the existing import XML:

```csharp
bulkdesc.GenerateInsertRequest(
    Context.SourceReader,
    columns,
    record);
```

or:

```csharp
bulkdesc.GenerateUpdateRequest(
    Context.SourceReader,
    columns,
    record,
    Context.IdentifyField,
    Context.ShiftCheckField,
    ref_student_id);
```

The alias conversion must occur before field matching and XML generation.

## Implementation Requirements

### 1. Add a Centralized Alias Conversion Method

Create one reusable method for converting Excel parent aliases into the existing internal field names.

Example:

```csharp
private string NormalizeParentImportFieldName(string fieldName)
{
    if (string.IsNullOrEmpty(fieldName))
        return fieldName;

    if (fieldName.StartsWith("家長1"))
    {
        return "父親" +
            fieldName.Substring("家長1".Length);
    }

    if (fieldName.StartsWith("家長2"))
    {
        return "母親" +
            fieldName.Substring("家長2".Length);
    }

    return fieldName;
}
```

Use exact mapping instead of broad prefix replacement if the actual grouped-field format requires more precise control.

### 2. Apply the Mapping When Excel Columns Are Loaded

The alias conversion should be applied when Excel headers are read and `SheetColumn` objects are created.

The preferred location is:

```text
WizardContext.RefreshImportSource()
```

This method is preferred because `StudentImportWizard` calls it multiple times:

* after selecting the Excel file
* before validation
* before the final import

The conversion must be applied every time the Excel source is refreshed.

Do not apply the conversion only to the `ListView` text because the validation and import processes refresh the Excel source again.

### 3. Preserve the Original Excel Header

If the current `SheetColumn` and `SheetReader` design requires the original Excel header to read cell values, preserve both names:

```text
SourceName  = 家長1姓名
InternalName = 父親姓名
DisplayText = 家長1姓名
```

Possible property design:

```csharp
public string SourceName { get; set; }
public string Name { get; set; }
public string DisplayText { get; set; }
```

Use the existing class design where possible. Do not add new properties unless they are necessary.

The responsibilities should be:

```text
SourceName
    Used to read the physical Excel column.

Name / InternalName
    Used for BulkDescription, validation, field selection,
    GenerateInsertRequest, and GenerateUpdateRequest.

DisplayText
    Used for the import UI.
```

### 4. Keep the Import UI User-Friendly

When the Excel file contains:

```text
家長1姓名
家長2姓名
```

the import field-selection screen should display those Excel names or another clearly understandable equivalent.

Internally, these fields must bind to:

```text
父親姓名
母親姓名
```

Do not display an error stating that the system does not support the `家長1` or `家長2` fields.

### 5. Preserve Legacy Excel Compatibility

Existing Excel files that use the original headers should continue to work:

```text
父親姓名
母親姓名
父親身分證號
母親身分證號
```

Both naming formats should be accepted:

```text
家長1姓名 or 父親姓名
    → internal field: 父親姓名

家長2姓名 or 母親姓名
    → internal field: 母親姓名
```

Do not require users to update older Excel files.

### 6. Use Internal Names for Validation

Before calling:

```csharp
Context.BulkDescription.Columns.GetColumnList(
    Context.ValidateColumns.GetNames());
```

the field-name collection should contain the existing internal names:

```text
父親姓名
母親姓名
```

It must not contain only:

```text
家長1姓名
家長2姓名
```

Otherwise the existing `BulkColumn` definitions may not be found.

### 7. Keep XML Generation Unchanged

Do not modify the existing XML-generation rules in:

```csharp
GenerateInsertRequest()
GenerateUpdateRequest()
```

The alias mapping must resolve the Excel names before these methods are called.

The generated XML must remain exactly the same as before the change.

For example, importing:

```text
家長1姓名 = 王大明
家長2姓名 = 李小華
```

must produce the same XML result that was previously produced by:

```text
父親姓名 = 王大明
母親姓名 = 李小華
```

### 8. Add Debug Verification During Development

Before XML generation, temporarily verify the selected internal fields:

```csharp
foreach (string fieldName in Context.ValidateColumns.GetNames())
{
    System.Diagnostics.Debug.WriteLine(
        "Internal import field: " + fieldName);
}
```

Expected output:

```text
Internal import field: 父親姓名
Internal import field: 母親姓名
```

After:

```csharp
bulkdesc.GenerateInsertRequest(...)
```

or:

```csharp
bulkdesc.GenerateUpdateRequest(...)
```

temporarily inspect:

```csharp
System.Diagnostics.Debug.WriteLine(record.OuterXml);
```

Compare the generated XML before and after the change.

Remove unnecessary debug output after verification unless it is consistent with the project's existing diagnostics policy.

## Important Constraints

Do not modify:

* the existing father XML structure
* the existing mother XML structure
* `JH_S_BulkDescription`
* database field names
* service request element names
* `GenerateInsertRequest()` behavior
* `GenerateUpdateRequest()` behavior
* existing student identification logic
* existing validation rules
* student status handling
* class lookup processing
* diploma-number processing
* password hashing
* import logging
* unrelated import fields
* unrelated program behavior

Do not globally replace `父親` with `家長1` or `母親` with `家長2`.

The conversion must apply only to Excel import aliases.

## Required Test Cases

### Test 1: New Parent Alias Headers

Import an Excel file containing:

```text
姓名
家長1姓名
家長2姓名
```

Verify:

* the fields are recognized
* the fields can be selected
* validation succeeds
* the correct values are imported
* the generated XML uses the original father and mother structure

### Test 2: All Parent Alias Fields

Test:

```text
家長1姓名
家長1身分證號
家長1國籍
家長1存歿
家長2姓名
家長2身分證號
家長2國籍
家長2存歿
```

Verify that every field maps to the correct original internal field.

### Test 3: Grouped Parent Information

If supported, test:

```text
家長1其它資訊:學歷
家長1其它資訊:職業
家長1其它資訊:電話
家長2其它資訊:學歷
家長2其它資訊:職業
家長2其它資訊:電話
```

Verify that grouped-field validation and XML generation remain correct.

### Test 4: Legacy Headers

Import an older Excel file containing:

```text
父親姓名
母親姓名
```

Verify that it continues to work without changes.

### Test 5: Insert Mode

Verify the alias mapping in `ImportMode.Insert`.

### Test 6: Update Mode

Verify the alias mapping in `ImportMode.Update`.

### Test 7: XML Comparison

Use equivalent source data in two files:

```text
File A uses 父親 / 母親 headers.
File B uses 家長1 / 家長2 headers.
```

Compare the generated student XML.

Except for irrelevant ordering differences, the XML results must be identical.

### Test 8: Unrelated Fields

Verify that unrelated fields such as the following are not affected:

```text
姓名
學號
身分證號
班級
座號
監護人姓名
聯絡電話
狀態
```

## Validation Checklist

* Excel `家長1姓名` is recognized.
* Excel `家長2姓名` is recognized.
* Excel `家長1*` fields map to existing `父親*` fields.
* Excel `家長2*` fields map to existing `母親*` fields.
* Original `父親*` Excel headers still work.
* Original `母親*` Excel headers still work.
* The import UI does not mark the alias fields as unsupported.
* Validation uses the original internal field definitions.
* Insert mode works correctly.
* Update mode works correctly.
* Generated XML remains unchanged.
* Existing `JH_S_BulkDescription` remains unchanged.
* No database or service field name is changed.
* No unrelated import behavior is modified.
* The project builds successfully.

## Completion Record

After completing and testing the change, create or update:

`匯入學生基本資料調整.md`

Record:

* modified files
* modified classes and methods
* where the alias conversion was implemented
* the final mapping table
* how the original Excel header is preserved
* how the internal field name is used
* confirmation that legacy headers remain supported
* confirmation that `JH_S_BulkDescription` was not modified
* confirmation that the generated XML structure remains unchanged
* insert-mode test results
* update-mode test results
* legacy-header test results
* grouped-field test results
* XML comparison results
* any limitations or issues found
