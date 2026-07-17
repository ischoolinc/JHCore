## Goal

Update the student basic data Excel export so that all father-related column headers use `家長1`, and all mother-related column headers use `家長2`.

The field selection screen and the exported Excel file must use the same renamed headers.

## Target File

* `StudentExportWizard.cs`

## Files for Reference Only

* `StudentBulkProcess.cs`
* `JH_S_ExportDescription.xml`

Do not modify the XML resource.

## Current Issue

The field selection screen already converts:

* `父親` to `家長1`
* `母親` to `家長2`

However, only the local UI variable is changed.

The original `Field` object stored in `ListViewItem.Tag` still contains the original `DisplayText`, such as:

* `父親姓名`
* `母親姓名`

During export, `GetSelectedFields()` retrieves the original `Field` object from `ListViewItem.Tag`.

As a result, the generated Excel file still uses the original father and mother column headers.

## Required Changes

### 1. Update the runtime `Field.DisplayText`

In `StudentExportWizard.ExportWizard_Load`, convert the parent-related display name and assign the converted value back to the runtime `Field.DisplayText`.

Example:

```csharp
string originalDisplayText = field.DisplayText;
string displayText = ConvertParentDisplayText(originalDisplayText);

// Update the runtime export header.
// This does not modify the XML resource or the underlying field name.
field.DisplayText = displayText;
```

### 2. Use the converted name in the field selection screen

Continue using the converted `displayText` when adding the item to the `ListView`.

```csharp
ListViewItem item = listView.Items.Add(displayText);
item.Tag = field;
item.Checked = true;
```

Because `item.Tag` now contains the runtime `Field` object with the updated `DisplayText`, the Excel export header will also use `家長1` and `家長2`.

### 3. Add a helper method

Add a helper method to convert the parent-related display names.

```csharp
private string ConvertParentDisplayText(string displayText)
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

### 4. Update `ExportWizard_Load`

Use the following logic inside the field loop:

```csharp
foreach (Field field in collection)
{
    string originalDisplayText = field.DisplayText;

    // Use the original field name for filtering.
    if (avoids.Contains(originalDisplayText))
        continue;

    string displayText =
        ConvertParentDisplayText(originalDisplayText);

    // Update only the runtime field header.
    // The XML resource and field identifiers remain unchanged.
    field.DisplayText = displayText;

    ListViewItem item =
        listView.Items.Add(displayText);

    // Preserve the existing required-field highlighting logic.
    if (list.Contains(originalDisplayText))
    {
        item.ForeColor = Color.Red;
    }

    item.Tag = field;
    item.Checked = true;
}
```

## Expected Results

The field selection screen and exported Excel file should display:

* `父親姓名` → `家長1姓名`
* `父親身分證號` → `家長1身分證號`
* `父親國籍` → `家長1國籍`
* `父親存歿` → `家長1存歿`
* `父親其它資訊` → `家長1其它資訊`
* `母親姓名` → `家長2姓名`
* `母親身分證號` → `家長2身分證號`
* `母親國籍` → `家長2國籍`
* `母親存歿` → `家長2存歿`
* `母親其它資訊` → `家長2其它資訊`

## Important Constraints

Do not modify:

* `JH_S_ExportDescription.xml`
* `Properties.Resources.JH_S_ExportDescription`
* `StudentBulkProcess.GetExportDescription()`
* Field identifiers
* Database fields
* XML storage format
* Exported data values
* Existing export behavior unrelated to the column headers

Keep the following field identifiers unchanged:

```text
FatherName
FatherIDNumber
FatherNationality
FatherLiving
FatherOtherInfo
FatherEducationDegree
FatherJob

MotherName
MotherIDNumber
MotherNationality
MotherLiving
MotherOtherInfo
MotherEducationDegree
MotherJob
```

Only the runtime `DisplayText` used for the UI and Excel column headers should change.

## Validation Checklist

* The field selection screen displays `家長1` instead of `父親`.
* The field selection screen displays `家長2` instead of `母親`.
* The exported Excel file displays `家長1` instead of `父親`.
* The exported Excel file displays `家長2` instead of `母親`.
* Parent data values remain correct.
* Non-parent fields remain unchanged.
* The original field identifiers remain unchanged.
* `JH_S_ExportDescription.xml` remains unchanged.
* The project builds successfully.
* No unrelated program logic is modified.

## Completion Record

After implementation and testing, create or update:

`匯出學生基本資料調整.md`

Record the following:

* Modified files
* Modified methods
* Root cause of the Excel header issue
* Parent field conversion rules
* Confirmation that only runtime `DisplayText` was changed
* Confirmation that the XML resource was not modified
* Confirmation that the field identifiers and exported data format remain unchanged
* Test results for both `.xlsx` and `.xls`
* Any issues found during implementation
