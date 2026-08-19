
## Goal

Update the student basic data export field selection screen so that parent-related fields are displayed as:

* `父親` → `家長1`
* `母親` → `家長2`

This change must affect only the UI display in `StudentExportWizard.cs`.

## Files to Review

* `StudentExportWizard.cs`
* `StudentBulkProcess.cs`
* `JH_S_ExportDescription.xml`

## Implementation Requirements

1. Modify only the field names displayed in the export field selection `ListView`.

2. In `StudentExportWizard.ExportWizard_Load`, convert UI display text as follows:

   * Any field name starting with `父親` should be displayed with `家長1`.
   * Any field name starting with `母親` should be displayed with `家長2`.

3. Expected UI examples:

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

4. Use a separate local variable for the UI text, for example:

```csharp
string displayText = field.DisplayText;

if (!string.IsNullOrEmpty(displayText))
{
    if (displayText.StartsWith("父親"))
    {
        displayText = "家長1" + displayText.Substring("父親".Length);
    }
    else if (displayText.StartsWith("母親"))
    {
        displayText = "家長2" + displayText.Substring("母親".Length);
    }
}

ListViewItem item = listView.Items.Add(displayText);
```

5. Keep the original `Field` object unchanged:

```csharp
item.Tag = field;
```

6. Do not assign the new UI name back to `field.DisplayText`.

Incorrect:

```csharp
field.DisplayText = "家長1姓名";
```

Correct:

```csharp
string displayText = field.DisplayText;
```

7. Do not modify:

   * `JH_S_ExportDescription.xml`
   * `Properties.Resources.JH_S_ExportDescription`
   * Field `Name` values such as `FatherName` or `MotherName`
   * `StudentBulkProcess.GetExportDescription()`
   * Export connector logic
   * Exported data structure
   * Database or XML storage format

8. Preserve the existing required-field highlighting logic by continuing to check the original value:

```csharp
if (list.Contains(field.DisplayText))
{
    item.ForeColor = Color.Red;
}
```

9. Preserve all existing export behavior and unrelated program logic.

## Validation Checklist

* The export field selection screen displays `家長1` instead of `父親`.
* The export field selection screen displays `家長2` instead of `母親`.
* Non-parent fields remain unchanged.
* All parent-related fields can still be selected normally.
* The original `Field` object remains stored in `ListViewItem.Tag`.
* Exported student data remains correct.
* Existing field identifiers such as `FatherName` and `MotherName` remain unchanged.
* The XML resource remains unchanged.
* The project builds successfully.
* No unrelated behavior is modified.

## Completion Record

After completing and testing the change, create or update:

`匯出學生基本資料調整.md`

Record the following:

* Modified files
* Modified methods
* UI field-name conversion rules
* Confirmation that the XML resource was not modified
* Confirmation that the underlying export field names and data format remain unchanged
* Test results
* Any issues found during implementation

請 Cursor 特別遵守「只改畫面顯示文字，不修改 `Field.DisplayText` 與 XML」這項限制。
