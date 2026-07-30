Goal

Fix the field option lists in both the student import and student export wizards so that list items no longer show a blue selection background.

The field list should use checkboxes for selection only. Clicking an item must not leave a blue selected-row background.

Target Files

StudentExportWizard.cs

StudentExportWizard.designer.cs

StudentImportWizard.cs

StudentImportWizard.designer.cs

Problem Summary

Both screens use SmartSchool.Common.ListViewEX.

The current controls allow ListViewItem.Selected to remain active, and the Designer currently uses:

HideSelection = false;

The import list also uses:

FullRowSelect = true;

Therefore, when the user clicks a field item, the Office/DotNetBar style displays a blue selected-item background.

This blue color is a selection-state effect, not the normal item background color.

Requirements

1. Remove the blue selected-item background

Apply the fix to both controls:

Export: listView

Import: lvSourceFieldList

The field list must continue to use checkbox selection, but items must not remain visually selected.

2. Set a normal list background

Set the ListView background to the normal window background:

BackColor = SystemColors.Window;

In the Designer, use:

System.Drawing.SystemColors.Window

3. Hide selection when the control loses focus

Change:

HideSelection = false;

to:

HideSelection = true;

for both import and export ListView controls.

4. Clear the selected state

Add a display-only configuration method to both wizard classes.

Recommended pattern:

private void ConfigureFieldListView()
{
    listView.HideSelection = true;
    listView.BackColor = SystemColors.Window;
    listView.ItemSelectionChanged += FieldListView_ItemSelectionChanged;
}

For the import wizard, use lvSourceFieldList.

Add an event handler:

private void FieldListView_ItemSelectionChanged(
    object sender,
    ListViewItemSelectionChangedEventArgs e)
{
    if (e.IsSelected)
        e.Item.Selected = false;
}

Call the configuration method immediately after InitializeComponent().

5. Set new items to the normal background

For export items created in ExportWizard_Load, set:

item.BackColor = SystemColors.Window;
item.Selected = false;

For import items created by ImportItem, set:

BackColor = SystemColors.Window;
Selected = false;

6. Preserve existing behavior

Do not change any unrelated logic.

The following behavior must remain unchanged:

Checkbox selection

Select-all behavior

Required-field red text

Disabled-field gray text

Read-only-field handling

Required-field locking

Tooltips

Parent field aliases:

父親... → 家長1...

母親... → 家長2...

Import field mapping

Export field mapping

XML structure

Insert/update logic

Validation logic

Export output format

7. Do not use item background color to represent state

Do not replace the blue selection background with another custom selection color.

The expected behavior is:

Normal items: window background

Required export fields: red text only

Disabled import fields: gray text only

Locked import fields: blue text only

Checked state: checkbox only

No selected-row background remains visible

Verification

Export Wizard

Open the student export wizard.

Click different field names.

Verify no field keeps a blue background.

Verify checkboxes can still be checked and unchecked.

Verify the select-all checkbox still works.

Verify required fields remain red.

Export a file and verify output is unchanged.

Import Wizard

Open the student import wizard.

Continue to the field-selection page.

Click different field names.

Verify no field keeps a blue background.

Verify checkboxes can still be checked and unchecked.

Verify select-all still works.

Verify disabled fields remain gray.

Verify locked/required fields still work normally.

Verify 家長1 and 家長2 display aliases remain correct.

Complete validation and import to confirm behavior is unchanged.

Regression Checks

Confirm that:

Clicking the checkbox still changes Checked.

Clearing Selected does not clear Checked.

Keyboard and mouse interaction do not leave a selected-row background.

No recursive selection event or UI freeze occurs.

XML and exported Excel content are unchanged.

Completion Record

After implementation and verification, create or update:

匯出匯入項目底色調整.md

Record:

Modified files

Root cause

Modified controls

Designer changes

Runtime event-handling changes

Confirmation that checkbox behavior is unchanged

Confirmation that required/disabled text colors are unchanged

Confirmation that XML and import/export logic are unchanged

Test scenarios and results

Any remaining risks or follow-up items