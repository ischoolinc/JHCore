
## Goal

Investigate why the student basic data export still produces the following Excel column headers:

* `父親姓名`
* `母親姓名`

even though the runtime `Field.DisplayText` values are already changed to:

* `家長1姓名`
* `家長2姓名`

Identify the actual cause, verify which export stage replaces or ignores the modified `DisplayText`, and fix the export so that the final Excel file contains the correct headers.

After completion, record all changes and test results in:

`匯出學生基本資料調整.md`

## Current Confirmed Behavior

The following debug code has already been tested:

```csharp
FieldCollection selectedFields = GetSelectedFields();

foreach (Field field in selectedFields)
{
    Debug.WriteLine(
        "Before Export: FieldName=" +
        field.FieldName +
        ", DisplayText=" +
        field.DisplayText);
}
```

The debug result is correct before export:

```text
FieldName=FatherName, DisplayText=家長1姓名
FieldName=MotherName, DisplayText=家長2姓名
```

However, after:

```csharp
ec.SetSelectedFields(GetSelectedFields());
ExportTable table = ec.Export();
```

the exported Excel column headers become:

```text
父親姓名
母親姓名
```

This confirms that the issue occurs inside or after:

* `ExportStudentConnector.SetSelectedFields()`
* `ExportStudentConnector.Export()`
* the service called by `Export()`
* the creation of `ExportTable`

The UI conversion logic is not the main issue.

## Target File

Primary file:

* `StudentExportWizard.cs`

Also inspect all relevant implementation files or referenced assemblies containing:

* `ExportStudentConnector`
* `ExportTable`
* `ExportOutput`
* export request builders
* export response handlers

## Reference Files

* `StudentBulkProcess.cs`
* `JH_S_ExportDescription.xml`

Do not modify the XML resource unless investigation proves that no safe runtime solution exists. The preferred solution must keep the XML resource unchanged.

## Required Investigation

### 1. Use the Same `FieldCollection` Instance

Replace repeated calls to `GetSelectedFields()` with one local variable during testing:

```csharp
FieldCollection selectedFields = GetSelectedFields();

ec.SetSelectedFields(selectedFields);
ExportTable table = ec.Export();
```

Do not test one collection and pass a different collection instance.

### 2. Check Whether `SetSelectedFields()` Modifies the Fields

Add debug output before and after `SetSelectedFields()`:

```csharp
FieldCollection selectedFields = GetSelectedFields();

foreach (Field field in selectedFields)
{
    Debug.WriteLine(
        "Before SetSelectedFields: FieldName=" +
        field.FieldName +
        ", DisplayText=" +
        field.DisplayText);
}

ec.SetSelectedFields(selectedFields);

foreach (Field field in selectedFields)
{
    Debug.WriteLine(
        "After SetSelectedFields: FieldName=" +
        field.FieldName +
        ", DisplayText=" +
        field.DisplayText);
}

ExportTable table = ec.Export();
```

Determine whether `SetSelectedFields()`:

* modifies `DisplayText`
* clones the fields
* stores only `FieldName`
* discards the runtime `DisplayText`

### 3. Inspect `ExportStudentConnector.SetSelectedFields()`

Check whether it stores the complete `Field` object or only the internal field identifier.

Look for logic similar to:

```csharp
selectedFields.Add(field);
```

or:

```csharp
selectedFieldNames.Add(field.FieldName);
```

or XML request generation such as:

```xml
<Field Name="FatherName"/>
```

If only `FieldName` is retained, document this as one possible root cause.

### 4. Inspect `ExportStudentConnector.Export()`

Check whether `Export()`:

* recreates new `Field` objects
* reloads the original export description
* maps `FatherName` back to `父親姓名`
* calls a remote DSA service
* uses server-returned column captions
* builds `ExportTable` columns from the original XML definition
* ignores the caller-provided `DisplayText`

Search for:

```text
FieldName
DisplayText
FatherName
MotherName
父親姓名
母親姓名
Columns.Add
ColumnName
DataColumn
ExportTable
CallService
DSAServices
DSRequest
DSResponse
```

### 5. Inspect the Export Request

If `Export()` calls a service, inspect the generated request.

Determine whether the request contains only:

```xml
<Field Name="FatherName"/>
```

or also contains:

```xml
<Field Name="FatherName" DisplayText="家長1姓名"/>
```

If only the internal field name is sent, verify whether the server decides the returned column caption.

### 6. Inspect the Export Response and `ExportTable`

Immediately after:

```csharp
ExportTable table = ec.Export();
```

inspect the table structure in the debugger.

Find the exact property or collection containing the final column headers.

Possible structures may include:

```text
table.Columns
table.Fields
table.Headers
table.ColumnNames
DataTable.Columns
DataColumn.ColumnName
```

Verify whether the table already contains:

```text
父親姓名
母親姓名
```

If so, the problem occurs before `ExportOutput.Save()`.

### 7. Inspect `ExportOutput`

Confirm whether:

```csharp
output.SetSource(table);
output.Save(fileName);
```

uses the existing `ExportTable` headers directly or rebuilds them again.

Do not modify `ExportOutput` unless debugging proves that it replaces the column names.

## Possible Root Causes to Verify

### Case 1: `SetSelectedFields()` Saves Only `FieldName`

Example behavior:

```text
FatherName
MotherName
```

are saved, but:

```text
家長1姓名
家長2姓名
```

are discarded.

Then `Export()` maps the internal names back to the original display text.

### Case 2: `Export()` Reloads the Original Export Description

`Export()` may reload `JH_S_ExportDescription` and recreate the fields:

```text
FatherName → 父親姓名
MotherName → 母親姓名
```

### Case 3: A Remote Service Determines the Column Headers

The client may send only internal field identifiers, and the server may return:

```text
父親姓名
母親姓名
```

### Case 4: `ExportTable` Rebuilds the Headers

The connector may return correct field information, but `ExportTable` may create columns from another description source.

### Case 5: `ExportOutput` Rebuilds the Headers

This is less likely, but verify whether the output component replaces the existing table headers during file creation.

## Required Fix

Implement the fix at the earliest reliable point where the final Excel column headers are determined.

Use this priority order:

### Preferred Fix 1: Preserve Runtime `DisplayText` in the Connector

If `ExportStudentConnector` is editable, modify it so that the selected field's runtime `DisplayText` is preserved and used when creating `ExportTable`.

Expected mapping:

```text
FieldName=FatherName
DisplayText=家長1姓名

FieldName=MotherName
DisplayText=家長2姓名
```

The internal field identifiers must remain unchanged.

### Preferred Fix 2: Rename `ExportTable` Headers After `Export()`

If the connector or service cannot be modified, rename only the returned table headers immediately after:

```csharp
ExportTable table = ec.Export();
```

and before:

```csharp
output.SetSource(table);
```

The logic must precisely rename:

* `父親姓名` → `家長1姓名`
* `母親姓名` → `家長2姓名`

Do not rename unrelated fields.

Do not use broad prefix replacement unless the requirement explicitly includes every father-related and mother-related field.

### Fallback Fix 3: Rename Excel Headers Before Saving

Only use this approach if `ExportTable` headers cannot be modified.

Modify the generated workbook or worksheet headers before the final file is saved.

This is the least preferred approach because it couples the fix to the Excel output implementation.

## Final Expected Result

The export field selection screen must display:

```text
家長1姓名
家長2姓名
```

The generated `.xlsx` and `.xls` files must also contain:

```text
家長1姓名
家長2姓名
```

The exported data values must remain unchanged.

The internal field identifiers must remain:

```text
FatherName
MotherName
```

## Important Constraints

Do not change:

* `FatherName`
* `MotherName`
* database field names
* XML storage structure
* exported student data values
* unrelated parent fields
* unrelated export behavior
* existing logging behavior
* existing file save behavior
* existing student selection behavior

Do not modify unrelated program logic.

Do not solve the issue only by changing the `ListViewItem.Text`, because that changes only the UI and does not guarantee the Excel header is changed.

## Validation Checklist

* Confirm the value before `SetSelectedFields()`.
* Confirm the value after `SetSelectedFields()`.
* Inspect the request created by `Export()`.
* Inspect the response returned by the service.
* Inspect the final headers inside `ExportTable`.
* Confirm whether `ExportOutput` changes the headers.
* Identify and document the exact root cause.
* The UI displays `家長1姓名`.
* The UI displays `家長2姓名`.
* The `.xlsx` file displays `家長1姓名`.
* The `.xlsx` file displays `家長2姓名`.
* The `.xls` file displays `家長1姓名`.
* The `.xls` file displays `家長2姓名`.
* Father and mother data values remain correct.
* `FatherName` and `MotherName` remain unchanged.
* Other export columns remain unchanged.
* The XML resource remains unchanged.
* The project builds successfully.
* No unrelated logic is modified.

## Completion Record

After implementation and testing, create or update:

`匯出學生基本資料調整.md`

Record:

* Modified files
* Modified classes and methods
* Debugging steps performed
* Values before and after `SetSelectedFields()`
* Export request contents
* Export response findings
* `ExportTable` header findings
* Confirmed root cause
* Selected fix and why it was chosen
* Confirmation that internal field identifiers remain unchanged
* Confirmation that exported data values remain unchanged
* Confirmation that the XML resource remains unchanged
* `.xlsx` test results
* `.xls` test results
* Any limitations or unresolved issues
