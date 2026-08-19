## Goal

Modify the student import wizard so that all parent-related Excel columns follow a consistent external-name and internal-name mapping rule.

The Excel file and the UI must use:

- `家長1...`
- `家長2...`

The existing internal field names and XML structure must continue to use:

- `父親...`
- `母親...`

Examples:

| Excel Column / UI Display | Internal Field / XML |
|---|---|
| `家長1姓名` | `父親姓名` |
| `家長1身分證號` | `父親身分證號` |
| `家長1國籍` | `父親國籍` |
| `家長1存歿` | `父親存歿` |
| `家長1其它資訊` | `父親其它資訊` |
| `家長2姓名` | `母親姓名` |
| `家長2身分證號` | `母親身分證號` |
| `家長2國籍` | `母親國籍` |
| `家長2存歿` | `母親存歿` |
| `家長2其它資訊` | `母親其它資訊` |

## Target File

- `StudentImportWizard.cs`

Review the related Designer file only if required for the predefined UI text:

- `StudentImportWizard.designer.cs`

## Required Behavior

### 1. Excel must use the new parent column names

For all parent-related fields:

- Father-related Excel columns must start with `家長1`
- Mother-related Excel columns must start with `家長2`

Examples:

```text
家長1姓名
家長1身分證號
家長1國籍
家長1存歿
家長1其它資訊

家長2姓名
家長2身分證號
家長2國籍
家長2存歿
家長2其它資訊
```

Do not treat Excel columns beginning with `父親` or `母親` as valid new-format source columns.

### 2. Old Excel names must not be selectable

When the Excel file contains only an old field name, such as:

```text
父親姓名
```

the field-selection page may still display:

```text
家長1姓名
```

but the item must:

- Be disabled
- Be unchecked
- Appear using the existing disabled-field style
- Show a tooltip explaining the required Excel column name

Example tooltip:

```text
Excel 欄位「父親姓名」請改為「家長1姓名」
```

The same rule applies to all `父親...` and `母親...` fields.

Examples:

```text
父親身分證號 → 家長1身分證號
父親國籍 → 家長1國籍
母親身分證號 → 家長2身分證號
母親國籍 → 家長2國籍
```

### 3. New Excel names must remain selectable

When the Excel file contains:

```text
家長1姓名
家長2姓名
```

the field-selection page must display the same names and allow them to be selected when all existing import conditions are satisfied.

Do not generate duplicated names such as:

```text
家長11姓名
家長22姓名
```

### 4. Apply the rule to every parent field

Do not write logic only for `姓名`.

Use prefix-based mapping:

```text
父親... ↔ 家長1...
母親... ↔ 家長2...
```

The implementation should automatically cover all current and future parent-related fields that follow these prefixes.

## Recommended Implementation

### Parent field helpers

Add reusable helper methods.

```csharp
private static bool IsFatherField(string fieldName)
{
    return !string.IsNullOrEmpty(fieldName) &&
        fieldName.StartsWith("父親");
}

private static bool IsMotherField(string fieldName)
{
    return !string.IsNullOrEmpty(fieldName) &&
        fieldName.StartsWith("母親");
}
```

Add a method to convert the internal field name to the required Excel source name:

```csharp
private static string GetExpectedParentSourceName(
    string internalName)
{
    if (IsFatherField(internalName))
    {
        return "家長1" +
            internalName.Substring("父親".Length);
    }

    if (IsMotherField(internalName))
    {
        return "家長2" +
            internalName.Substring("母親".Length);
    }

    return internalName;
}
```

### Validate the actual Excel source name

Use `SheetColumn.SourceName` to determine the original Excel column header.

Do not use only:

- `SheetColumn.Name`
- `SheetColumn.GroupName`
- `SheetColumn.DisplayText`

because these values may already represent internal names or display aliases.

Expected validation:

```csharp
if (IsFatherField(column.Name))
{
    valid = !string.IsNullOrEmpty(column.SourceName) &&
        column.SourceName.StartsWith("家長1");
}
else if (IsMotherField(column.Name))
{
    valid = !string.IsNullOrEmpty(column.SourceName) &&
        column.SourceName.StartsWith("家長2");
}
```

Prefer exact mapped-name comparison when the source model reliably provides the full original Excel heading:

```csharp
column.SourceName ==
    GetExpectedParentSourceName(column.Name)
```

Use prefix matching only when grouped-column behavior requires it.

### Validate before adding the item to the ListView

In `DisplayColumns(...)`, after the existing checks for:

- Accepted fields
- Identification field
- Validation field
- Read-only fields
- Required fields
- Group completeness

validate the parent source names.

If invalid:

```csharp
each.Enabled = false;
each.ToolTipText = parentAliasMessage;
```

Then add the item to the list so the user can see why it is unavailable.

## UI Display Rules

The field-selection screen must always use the new terminology:

```text
父親... → 家長1...
母親... → 家長2...
```

Keep the existing display conversion method or consolidate it into one reusable method.

Example:

```csharp
private static string GetParentAliasDisplayText(
    string displayText)
{
    if (string.IsNullOrEmpty(displayText))
        return displayText;

    if (displayText.StartsWith("父親"))
    {
        return "家長1" +
            displayText.Substring("父親".Length);
    }

    if (displayText.StartsWith("母親"))
    {
        return "家長2" +
            displayText.Substring("母親".Length);
    }

    return displayText;
}
```

The display conversion must not change internal keys.

## Preserve Internal Field and XML Behavior

Do not rename or modify the original internal parent fields.

The following must remain based on the original `父親...` and `母親...` names:

- `SheetColumn.Name`
- `SheetColumn.GroupName`
- `SheetColumn.BindingBulkColumn`
- `BulkColumn.DisplayText`
- `BulkColumn.GroupName`
- `BulkColumn.FullDisplayText`
- `ImportItem.InternalGroupName`
- `ImportItem.InternalFieldName`
- `Context.AcceptColumns`
- `Context.SourceColumns`
- `Context.SelectedFields`
- `Context.BulkDescription`

When a selectable `家長1...` or `家長2...` item is checked, the selected field must still be added using:

```csharp
Context.SelectedFields.Add(
    column.Name,
    column);
```

Therefore:

```text
家長1姓名 → column.Name = 父親姓名
家長2姓名 → column.Name = 母親姓名
```

## Do Not Change XML or Import Contract

Do not change:

- XML element names
- XML node names
- XML attributes
- Insert request structure
- Update request structure
- Server contract

Do not modify the behavior of:

- `BulkDescription.GenerateInsertRequest`
- `BulkDescription.GenerateUpdateRequest`
- `StudentBulkProcess.InsertImportStudent`
- `StudentBulkProcess.UpdateImportStudent`

The final XML must continue to use the existing father/mother mapping.

## Keep Existing Import Logic Unchanged

Do not alter unrelated behavior, including:

- Insert mode
- Update mode
- Identification-field selection
- Validation-field selection
- Required-field handling
- Read-only-field handling
- Group-field handling
- Student lookup
- Validation rules
- Password hashing
- Class lookup
- Graduate-information processing
- Import logging
- ListView background-color adjustments

## Verification

### Case 1: Old father name only

Excel contains:

```text
父親姓名
```

Expected result:

- UI displays `家長1姓名`
- Item is disabled
- Item cannot be checked
- Tooltip says the Excel field should be changed to `家長1姓名`

### Case 2: New Parent 1 name

Excel contains:

```text
家長1姓名
```

Expected result:

- UI displays `家長1姓名`
- Item is enabled when other import rules pass
- Item can be checked
- Selected internal field remains `父親姓名`
- XML remains unchanged

### Case 3: Old mother name only

Excel contains:

```text
母親姓名
```

Expected result:

- UI displays `家長2姓名`
- Item is disabled
- Item cannot be checked
- Tooltip says the Excel field should be changed to `家長2姓名`

### Case 4: New Parent 2 name

Excel contains:

```text
家長2姓名
```

Expected result:

- UI displays `家長2姓名`
- Item can be selected
- Selected internal field remains `母親姓名`
- XML remains unchanged

### Case 5: Other Parent 1 fields

Test at least:

```text
父親身分證號
父親國籍
父親存歿
父親其它資訊
```

Verify they are disabled when the Excel file uses old names.

Then change them to:

```text
家長1身分證號
家長1國籍
家長1存歿
家長1其它資訊
```

Verify they become selectable and still map internally to the original father fields.

### Case 6: Other Parent 2 fields

Perform the same test for:

```text
母親身分證號
母親國籍
母親存歿
母親其它資訊
```

and the new Excel names:

```text
家長2身分證號
家長2國籍
家長2存歿
家長2其它資訊
```

### Case 7: Excel without parent fields

Verify that an Excel file without parent fields:

- Does not force the user to add them
- Does not create selectable parent items from Designer defaults
- Does not affect unrelated field imports

### Case 8: XML comparison

Compare the generated XML before and after this change.

Confirm:

- XML parent node names remain unchanged
- Father fields still use the existing father XML mapping
- Mother fields still use the existing mother XML mapping
- Insert and update requests remain compatible

## Completion Record

After the implementation and verification are complete, create or update:

`匯入家長1家長2調整.md`

Record:

- Modified files
- Root cause
- Parent field mapping rules
- Excel source-name validation logic
- UI enabled/disabled behavior
- Tooltip behavior
- Confirmation that internal father/mother names remain unchanged
- Confirmation that XML structure remains unchanged
- Test cases and results
- Any remaining risks or follow-up items