## Goal

Update the student basic data import wizard so that the **validation field** and **identification field** dropdown options display:

* `父親` as `家長1`
* `母親` as `家長2`

The change must affect only the UI display text.

The internal field names, validation behavior, data lookup logic, and generated XML structure must remain unchanged.

After implementation and testing, record all changes in:

`匯入學生基本資料調整.md`

## Target File

* `StudentImportWizard.cs`

## Current Issue

The validation-field dropdown is populated directly from:

```csharp
foreach (BulkColumn each in Context.AcceptColumns.Values)
{
    if (each.Identifiable)
        cboIdField.Items.Add(each);

    if (each.ShiftCheckable)
        cboValidateField.Items.Add(each);
}
```

The ComboBoxes use:

```csharp
cboIdField.DisplayMember = "DisplayText";
cboValidateField.DisplayMember = "DisplayText";
```

Therefore, the dropdowns display the original internal `BulkColumn.DisplayText` values, such as:

```text
父親姓名
父親身分證號
父親國籍
母親姓名
母親身分證號
母親國籍
```

The existing `GetImportDisplayName()` method only affects the import-field `ListView`. It does not affect:

```text
cboIdField
cboValidateField
```

## Required Display Mapping

Apply the following UI-only mapping:

```text
Internal field name       Dropdown display name
父親姓名             →    家長1姓名
父親身分證號         →    家長1身分證號
父親國籍             →    家長1國籍
父親存歿             →    家長1存歿

母親姓名             →    家長2姓名
母親身分證號         →    家長2身分證號
母親國籍             →    家長2國籍
母親存歿             →    家長2存歿
```

If other parent-related fields are eligible for identification or validation, apply the same prefix mapping:

```text
父親* → 家長1*
母親* → 家長2*
```

Only the dropdown display text should change.

## Internal Data Requirements

After the user selects an option, the program must continue storing the original internal field name.

Examples:

```text
Dropdown displays: 家長1姓名
Context.ShiftCheckField stores: 父親姓名
```

```text
Dropdown displays: 家長2身分證號
Context.ShiftCheckField stores: 母親身分證號
```

The following existing lookup logic must continue to work without modification:

```csharp
BulkColumn key =
    Context.BulkDescription.Columns[Context.IdentifyField];

BulkColumn shift =
    Context.BulkDescription.Columns[Context.ShiftCheckField];
```

Do not store `家長1` or `家長2` in:

```text
Context.IdentifyField
Context.ShiftCheckField
Context.BulkDescription.Columns
Context.ValidateColumns
```

## Implementation Requirements

### 1. Add a Shared UI Alias Method

Add or reuse a centralized method that converts the internal parent field name into a UI display name.

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

Do not assign the converted value back to the original `BulkColumn.DisplayText`.

Incorrect:

```csharp
each.DisplayText =
    GetParentAliasDisplayText(each.DisplayText);
```

This may change the internal field name used by later logic.

### 2. Add a ComboBox Option Wrapper

Add a small wrapper class that keeps the original `BulkColumn` and a separate UI display name.

Example:

```csharp
private class BulkColumnOption
{
    private readonly BulkColumn _column;
    private readonly string _displayText;

    public BulkColumnOption(
        BulkColumn column,
        string displayText)
    {
        _column = column;
        _displayText = displayText;
    }

    public string DisplayText
    {
        get { return _displayText; }
    }

    public BulkColumn Column
    {
        get { return _column; }
    }
}
```

The wrapper must preserve:

```text
DisplayText = UI alias
Column      = original BulkColumn
```

Example:

```text
DisplayText          = 家長1姓名
Column.DisplayText   = 父親姓名
```

### 3. Update Dropdown Population

In `wpCollectKeyInfo_AfterPageDisplayed`, continue using:

```csharp
cboIdField.DisplayMember = "DisplayText";
cboValidateField.DisplayMember = "DisplayText";
```

However, add `BulkColumnOption` objects instead of adding the original `BulkColumn` objects directly.

Example:

```csharp
foreach (BulkColumn each in Context.AcceptColumns.Values)
{
    string displayText =
        GetParentAliasDisplayText(each.DisplayText);

    if (each.Identifiable)
    {
        cboIdField.Items.Add(
            new BulkColumnOption(
                each,
                displayText));
    }

    if (each.ShiftCheckable)
    {
        cboValidateField.Items.Add(
            new BulkColumnOption(
                each,
                displayText));
    }
}
```

### 4. Preserve the Empty Validation Option

The current empty validation option must continue to work.

Wrap `Context.EmptyShiftCheckField` using the same option class:

```csharp
BulkColumnOption emptyOption =
    new BulkColumnOption(
        Context.EmptyShiftCheckField,
        Context.EmptyShiftCheckField.DisplayText);

cboValidateField.Items.Add(emptyOption);
cboValidateField.SelectedIndex = 0;
```

Do not rename or change the behavior of the empty option.

### 5. Update Selected-Option Handling

The current code casts the selected item directly to `BulkColumn`:

```csharp
BulkColumn column =
    cboIdField.SelectedItem as BulkColumn;
```

Change it to retrieve the wrapper and then use its original `BulkColumn`.

Example for the identification field:

```csharp
BulkColumnOption idOption =
    cboIdField.SelectedItem as BulkColumnOption;

if (idOption != null &&
    idOption.Column != null)
{
    Context.IdentifyField =
        idOption.Column.DisplayText;
}
```

Example for the validation field:

```csharp
BulkColumnOption validateOption =
    cboValidateField.SelectedItem
        as BulkColumnOption;

if (validateOption != null &&
    validateOption.Column != null &&
    validateOption.Column !=
        Context.EmptyShiftCheckField)
{
    Context.ShiftCheckField =
        validateOption.Column.DisplayText;
}
else
{
    Context.ShiftCheckField =
        string.Empty;
}
```

The saved values must remain the original internal names:

```text
父親姓名
母親姓名
父親身分證號
母親身分證號
```

### 6. Preserve Existing Validation Logic

Do not change the following behavior:

```csharp
if (Context.IdentifyField ==
    Context.ShiftCheckField)
{
    MsgBox.Show(
        "「識別欄」與「驗證欄」必須是不同的欄位。");

    return;
}
```

The comparison must continue using original internal field names.

### 7. Preserve Shift-Check Processing

Do not modify:

```csharp
BulkColumn key =
    Context.BulkDescription.Columns[
        Context.IdentifyField];

BulkColumn shift =
    Context.BulkDescription.Columns[
        Context.ShiftCheckField];

checkList =
    new ShiftCheckList(key, shift);
```

The alias conversion must not affect `ShiftCheckList`, database lookups, or field validation.

### 8. Preserve Import and XML Generation

Do not modify:

```csharp
GenerateInsertRequest()
GenerateUpdateRequest()
StudentBulkProcess.InsertImportStudent()
StudentBulkProcess.UpdateImportStudent()
```

The generated XML must remain exactly the same as before this UI change.

Example:

```text
UI selection: 家長1姓名
Internal field: 父親姓名
Generated XML: original father structure
```

```text
UI selection: 家長2姓名
Internal field: 母親姓名
Generated XML: original mother structure
```

## Recommended Final Data Flow

```text
Context.AcceptColumns
    BulkColumn.DisplayText = 父親姓名
                ↓
GetParentAliasDisplayText()
                ↓
BulkColumnOption.DisplayText = 家長1姓名
                ↓
ComboBox displays 家長1姓名
                ↓
User selects the option
                ↓
BulkColumnOption.Column.DisplayText = 父親姓名
                ↓
Context.ShiftCheckField = 父親姓名
                ↓
Existing validation and XML logic
```

The same pattern must apply to mother fields:

```text
母親* → UI 家長2* → internal 母親*
```

## Important Constraints

Do not modify:

* `JH_S_BulkDescription`
* XML element names
* XML storage structure
* database field names
* service request fields
* `BulkColumn.DisplayText`
* `Context.AcceptColumns` keys
* `Context.BulkDescription.Columns` keys
* `Context.IdentifyField` internal format
* `Context.ShiftCheckField` internal format
* validation rules
* shift-check service behavior
* import-field selection behavior unrelated to parent aliases
* student identification logic
* import-mode behavior
* any unrelated program logic

Do not globally replace:

```text
父親 → 家長1
母親 → 家長2
```

The mapping must be used only for UI display.

## Required Test Cases

### Test 1: Validation Dropdown Display

Use an Excel file containing parent fields and open update-import mode.

Verify that the validation-field dropdown displays:

```text
家長1姓名
家長1身分證號
家長1國籍
家長2姓名
家長2身分證號
家長2國籍
```

### Test 2: Internal Selected Value

Select `家長1姓名`.

Verify:

```text
Context.ShiftCheckField = 父親姓名
```

Select `家長2姓名`.

Verify:

```text
Context.ShiftCheckField = 母親姓名
```

### Test 3: Identification Dropdown

If a parent field is identifiable, verify the identification dropdown uses the same UI alias while storing the original internal field name.

### Test 4: Empty Validation Option

Select the empty validation option.

Verify:

```text
Context.ShiftCheckField = ""
```

### Test 5: Shift Check

Select a parent validation field and execute validation.

Verify:

* the correct database field is queried
* comparison works correctly
* no missing-key exception occurs
* error messages are attached to the correct field

### Test 6: XML Comparison

Run the same import before and after the UI modification.

Verify that the generated XML is identical.

### Test 7: Unrelated Fields

Verify that fields such as the following remain unchanged:

```text
姓名
學號
身分證號
班級
座號
監護人姓名
狀態
```

### Test 8: Legacy Parent Headers

Verify that Excel files using the original `父親` and `母親` headers continue to work.

## Validation Checklist

* Validation dropdown displays `家長1` instead of `父親`.
* Validation dropdown displays `家長2` instead of `母親`.
* Identification dropdown uses the same alias rule where applicable.
* `Context.IdentifyField` stores the original internal name.
* `Context.ShiftCheckField` stores the original internal name.
* `BulkColumn.DisplayText` is not modified.
* `Context.AcceptColumns` is not modified.
* Existing validation behavior remains correct.
* Existing shift-check behavior remains correct.
* Generated XML remains unchanged.
* Existing XML resources remain unchanged.
* Legacy Excel files still work.
* No unrelated behavior is modified.
* The project builds successfully.

## Completion Record

After implementation and testing, create or update:

`匯入學生基本資料調整.md`

Record:

* modified files
* modified classes and methods
* original source of the validation dropdown values
* the UI alias mapping rules
* the wrapper class design
* how the original `BulkColumn` is preserved
* how `Context.IdentifyField` is assigned
* how `Context.ShiftCheckField` is assigned
* confirmation that validation logic remains unchanged
* confirmation that the XML resource was not modified
* confirmation that the generated XML structure remains unchanged
* validation-dropdown test results
* identification-dropdown test results
* shift-check test results
* legacy-header test results
* XML comparison results
* any issues or limitations found
