Goal

Fix the blue background shown behind every field option in both the student import and student export wizards.

This is the second correction. The previous adjustment only changed selection-related properties such as HideSelection, Selected, and BackColor, but the blue background still remained.

The root cause is that both field lists use:

SmartSchool.Common.ListViewEX

ListViewEX applies its own custom Office/DotNetBar item rendering, so each item can still be drawn with a blue background even when the item is not selected.

Replace the custom ListViewEX controls with the standard WinForms ListView.

Target Files

StudentExportWizard.cs

StudentExportWizard.designer.cs

StudentImportWizard.cs

StudentImportWizard.designer.cs

Root Cause

The current field list controls are declared and instantiated as:

SmartSchool.Common.ListViewEX

The blue background inside the field list is not only caused by:

ListViewItem.Selected
HideSelection
ListViewItem.BackColor

It is also caused by the custom rendering behavior of ListViewEX.

Therefore, the previous changes do not fully remove the blue item background.

Requirements

1. Replace the export ListViewEX control

In StudentExportWizard.designer.cs, change:

this.listView = new SmartSchool.Common.ListViewEX();

to:

this.listView = new System.Windows.Forms.ListView();

Change the field declaration from:

private ListViewEX listView;

to:

private System.Windows.Forms.ListView listView;

2. Replace the import ListViewEX control

In StudentImportWizard.designer.cs, change:

this.lvSourceFieldList =
    new SmartSchool.Common.ListViewEX();

to:

this.lvSourceFieldList =
    new System.Windows.Forms.ListView();

Change the field declaration from:

private ListViewEX lvSourceFieldList;

to:

private System.Windows.Forms.ListView lvSourceFieldList;

3. Remove ListViewEX-specific border properties

The standard WinForms ListView does not support the DotNetBar border properties used by ListViewEX.

Remove the following settings from both Designer files:

Border.Class
Border.CornerType

Examples to remove:

this.listView.Border.Class = "RibbonClientPanel";
this.listView.Border.CornerType =
    DevComponents.DotNetBar.eCornerType.Square;

this.lvSourceFieldList.Border.Class = "ListViewBorder";
this.lvSourceFieldList.Border.CornerType =
    DevComponents.DotNetBar.eCornerType.Square;

Replace them with the standard WinForms border setting:

BorderStyle = BorderStyle.FixedSingle;

The setting may be placed in the Designer or in the runtime configuration method.

4. Keep the normal background color

Set both controls to:

BackColor = SystemColors.Window;
HideSelection = true;

In Designer code, use:

System.Drawing.SystemColors.Window

5. Prevent normal ListView selection highlighting

Keep or add the ItemSelectionChanged event so that clicking item text does not leave a normal Windows selection highlight.

Recommended pattern for the export wizard:

private void ConfigureFieldListView()
{
    listView.HideSelection = true;
    listView.BackColor = SystemColors.Window;
    listView.BorderStyle = BorderStyle.FixedSingle;
    listView.ItemSelectionChanged +=
        FieldListView_ItemSelectionChanged;
}

Recommended pattern for the import wizard:

private void ConfigureFieldListView()
{
    lvSourceFieldList.HideSelection = true;
    lvSourceFieldList.BackColor = SystemColors.Window;
    lvSourceFieldList.BorderStyle = BorderStyle.FixedSingle;
    lvSourceFieldList.ItemSelectionChanged +=
        FieldListView_ItemSelectionChanged;
}

Event handler:

private void FieldListView_ItemSelectionChanged(
    object sender,
    ListViewItemSelectionChangedEventArgs e)
{
    if (e.IsSelected)
        e.Item.Selected = false;
}

Call ConfigureFieldListView() immediately after InitializeComponent() in both constructors.

6. Preserve all existing ListView settings

Keep the existing behavior and properties where applicable:

CheckBoxes = true;
View = View.List;
UseCompatibleStateImageBehavior = false;
HideSelection = true;

For the import list, preserve:

FullRowSelect = true;
ShowItemToolTips = true;

For the export list, preserve:

HeaderStyle = ColumnHeaderStyle.None;
Dock = DockStyle.Bottom;

7. Preserve all checkbox behavior

Do not change:

Checked

CheckedItems

ItemCheck

Select-all logic

Required-field locking

Disabled-field handling

The field checkbox remains the only selection mechanism.

8. Preserve text colors

Do not change the current foreground-color behavior:

Export required fields remain red.

Import disabled fields remain gray.

Import locked/required fields remain blue.

Normal enabled fields remain black.

Only the item background must become the standard window background.

9. Preserve parent display aliases

Do not change the existing display-only conversion:

父親... → 家長1...
母親... → 家長2...

Do not modify internal field names, XML field names, collection keys, or data mappings.

10. Preserve import/export logic

Do not change:

Export field collection

GetSelectedFields

Export output

Import field grouping

Validation logic

Required-field checks

Read-only-field checks

Insert/update behavior

XML generation

XML node names

XML attributes

Server request structure

Important Constraint

Do not solve this by changing every item background color while still using ListViewEX.

The required fix is to replace:

SmartSchool.Common.ListViewEX

with:

System.Windows.Forms.ListView

for these two field-list controls only.

Do not replace unrelated ListViewEX controls elsewhere in the project.

Verification

Export Wizard

Open the student export wizard.

Verify every field item has a normal white/window background.

Click field text and verify no blue item block remains.

Check and uncheck individual checkboxes.

Use the select-all checkbox.

Verify required fields remain red.

Export an Excel file.

Verify exported content and column selection are unchanged.

Import Wizard

Open the student import wizard.

Continue to the field-selection page.

Verify every field item has a normal white/window background.

Click field text and verify no blue item block remains.

Check and uncheck available fields.

Use the select-all checkbox.

Verify disabled fields remain gray.

Verify required/locked fields remain functional.

Verify tooltips still display.

Verify 家長1 and 家長2 display names remain correct.

Complete validation and import.

Verify XML and imported data are unchanged.

Regression Checks

Confirm that:

ItemCheck still fires correctly.

Clearing Selected does not clear Checked.

CheckedItems returns the correct items.

No recursive selection event occurs.

No UI freeze occurs.

The field list layout remains readable.

Required and disabled text colors remain correct.

Import and export results are unchanged.

Completion Record

After implementation and verification, create or update:

匯出匯入項目底色調整.md

Record:

Modified files

Previous incorrect diagnosis

Confirmed root cause

Controls changed from ListViewEX to System.Windows.Forms.ListView

Removed ListViewEX-specific properties

Added standard BorderStyle

Confirmation that checkbox behavior is unchanged

Confirmation that foreground colors are unchanged

Confirmation that parent aliases are unchanged

Confirmation that XML and import/export logic are unchanged

Test scenarios and results

Remaining risks or follow-up items