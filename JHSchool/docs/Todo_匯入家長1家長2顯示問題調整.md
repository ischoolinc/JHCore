## Goal

Fix the student import wizard so that parent-related field names are displayed consistently as:

* `父親姓名` → `家長1姓名`
* `母親姓名` → `家長2姓名`

Also ensure that any other display field beginning with `父親` or `母親` is shown as `家長1` or `家長2` respectively.

## Target Files

* `StudentImportWizard.cs`
* `StudentImportWizard.designer.cs`

## Requirements

### 1. Fix the predefined ListView display text

In `StudentImportWizard.designer.cs`, update the predefined `ListViewItem` display text:

* Change `父親姓名` to `家長1姓名`
* Change `母親姓名` to `家長2姓名`

This change is for UI display only.

### 2. Fix dynamically generated import-field display names

In `StudentImportWizard.cs`, review the field-display flow used by:

* `wpCollectKeyInfo_AfterPageDisplayed`
* `GroupSheetColumn`
* `GetImportDisplayName`
* `GetParentAliasDisplayText`
* `DisplayColumns`

Ensure all UI display names follow these rules:

* Any display text starting with `父親` must be shown with the prefix `家長1`
* Any display text starting with `母親` must be shown with the prefix `家長2`

Examples:

* `父親姓名` → `家長1姓名`
* `父親電話` → `家長1電話`
* `母親姓名` → `家長2姓名`
* `母親電話` → `家長2電話`

Apply the conversion to both:

* Group fields
* Non-group fields

Do not limit the conversion only to Excel source columns that already begin with `家長1` or `家長2`.

### 3. Preserve all internal field names and data processing

Do not rename or modify internal field keys used by the import process.

The following values and logic must remain based on the original `父親` / `母親` field names where currently required:

* `SheetColumn.Name`
* `SheetColumn.GroupName`
* `BulkColumn.DisplayText`
* `BulkColumn.FullDisplayText`
* `ImportItem.InternalGroupName`
* `ImportItem.InternalFieldName`
* `Context.AcceptColumns`
* `Context.SourceColumns`
* `Context.SelectedFields`
* `Context.IdentifyField`
* `Context.ShiftCheckField`
* `Context.BulkDescription`

The `家長1` / `家長2` alias must only be used for visible UI text.

Do not use the display alias as the key when accessing collections or generating import data.

### 4. Do not change the XML format

Do not change any XML element names, node names, attributes, or request structures.

Do not modify the behavior of:

* `BulkDescription.GenerateInsertRequest`
* `BulkDescription.GenerateUpdateRequest`
* `StudentBulkProcess.InsertImportStudent`
* `StudentBulkProcess.UpdateImportStudent`

The generated XML and server-side import contract must remain exactly compatible with the existing implementation.

### 5. Keep existing import logic unchanged

Do not alter unrelated behavior, including:

* Insert and update modes
* Identification-field selection
* Validation-field selection
* Required-field handling
* Read-only-field handling
* Field grouping
* Validation rules
* Student lookup
* Class lookup
* Graduate information processing
* Password hashing
* Import logging

## Recommended Implementation

Use one display-only alias method for all relevant UI locations.

```csharp
private static string GetParentAliasDisplayText(string displayText)
{
    if (string.IsNullOrEmpty(displayText))
        return displayText;

    if (displayText.StartsWith("父親"))
        return "家長1" + displayText.Substring("父親".Length);

    if (displayText.StartsWith("母親"))
        return "家長2" + displayText.Substring("母親".Length);

    return displayText;
}
```

Update `GetImportDisplayName()` so that both group and non-group display text pass through this method.

Suggested structure:

```csharp
private static string GetImportDisplayName(SheetColumn column)
{
    if (column == null)
        return string.Empty;

    string displayText = column.IsGroupField
        ? column.GroupName
        : column.DisplayText;

    return GetParentAliasDisplayText(displayText);
}
```

Keep the original `SheetColumn`, `BulkColumn`, internal group name, and internal field name unchanged.

## Verification

Test at least the following scenarios:

### Test 1: Predefined field list

Open the student import wizard and verify that the predefined list displays:

* `家長1姓名`
* `家長2姓名`

It must not display:

* `父親姓名`
* `母親姓名`

### Test 2: Excel using old field names

Import an Excel file containing:

* `父親姓名`
* `母親姓名`

Verify that the field-selection page displays:

* `家長1姓名`
* `家長2姓名`

### Test 3: Excel using new field names

Import an Excel file containing:

* `家長1姓名`
* `家長2姓名`

Verify that the field-selection page still displays:

* `家長1姓名`
* `家長2姓名`

Make sure invalid duplicated prefixes are not generated, such as:

* `家長11姓名`
* `家長22姓名`

### Test 4: Other parent fields

When other fields beginning with `父親` or `母親` exist, verify that they are displayed with the corresponding aliases.

Examples:

* `父親電話` → `家長1電話`
* `母親電話` → `家長2電話`

### Test 5: Identification and validation fields

Verify that the identification-field and validation-field dropdowns use the new display aliases.

Confirm that the selected values stored in:

* `Context.IdentifyField`
* `Context.ShiftCheckField`

still use the original internal field names.

### Test 6: Import field mapping

Verify that selecting `家長1姓名` and `家長2姓名` still maps to the original internal fields:

* `父親姓名`
* `母親姓名`

Confirm that collection lookups still work correctly.

### Test 7: XML compatibility

Compare the generated insert and update XML before and after the modification.

Confirm that:

* XML element names are unchanged
* XML attributes are unchanged
* Internal field mapping is unchanged
* Insert request structure is unchanged
* Update request structure is unchanged

### Test 8: Regression test

Verify that the following functions still work normally:

* New student import
* Existing student update
* Data validation
* Identification-field matching
* Validation-field comparison
* Required-field checks
* Read-only-field checks

## Completion Record

After the modification and verification are complete, create or update:

`家長1家長2調整.md`

Record the following:

* Modified files
* Root cause
* Modified methods and locations
* Display-name conversion logic
* Confirmation that internal field names were not changed
* Confirmation that XML structure was not changed
* Confirmation that insert/update logic was not changed
* Test scenarios and results
* Any remaining risks or follow-up items
