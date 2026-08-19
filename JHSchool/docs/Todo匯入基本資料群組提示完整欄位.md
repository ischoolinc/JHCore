# Todo.md

## Goal

Adjust the Student Basic Data Import group-field message.

When an Excel group is incomplete, the import field-selection screen currently displays only the missing fields.

Change the behavior so that the message displays the COMPLETE list of fields required for that group.

After completing the modification, record the changes and verification results in:

匯入學生基本資料調整0810.md


## Scope

Primary file:

StudentImportWizard.cs

Primary method:

ImportItem.CheckAccept(BulkColumnCollection basis)

Keep this change small and focused.

Do not refactor unrelated Student Import logic.


## Current Behavior

The current CheckAccept() logic collects only missing fields:

List<string> missingFields = new List<string>();

foreach (BulkColumn each in basis.Values)
{
    if (!SheetColumns.ContainsKey(each.FullDisplayText))
        missingFields.Add(
            GetParentAliasDisplayText(each.FullDisplayText));
}

If missingFields.Count > 0, the tooltip currently lists only those missing fields.

Example:

BulkDescription group:

監護人:學歷
監護人:職業
監護人:電話

Excel contains:

監護人:電話

Current message:

Excel「監護人」群組欄位不完整。
若要匯入此資料，Excel 尚需包含：

監護人:學歷
監護人:職業


## Required New Behavior

Continue detecting whether any group field is missing.

However, once the group is determined to be incomplete, display ALL fields defined by the BulkDescription group.

Example:

BulkDescription group:

監護人:學歷
監護人:職業
監護人:電話

Excel contains only:

監護人:電話

New message must be:

Excel「監護人」群組欄位不完整。
若要匯入此資料，Excel 必須包含完整欄位：

監護人:學歷
監護人:職業
監護人:電話


## Important

The condition for disabling the group must still be:

At least one BulkDescription group field is missing from the Excel source.

Do NOT change the rule so that every group always shows a warning.

Complete groups must remain enabled.


## Recommended Implementation

Modify CheckAccept() so it performs two separate operations:

1. Determine whether the Excel source is missing any field from the BulkDescription group.
2. If the group is incomplete, build the message by iterating through ALL fields in basis.Values.


## Suggested Code Direction

Use logic similar to:

public void CheckAccept(BulkColumnCollection basis)
{
    bool hasMissingField = false;

    // Check whether Excel is missing any field required by this group.
    foreach (BulkColumn each in basis.Values)
    {
        if (!SheetColumns.ContainsKey(each.FullDisplayText))
        {
            hasMissingField = true;
            break;
        }
    }

    if (hasMissingField)
    {
        string groupDisplay =
            GetParentAliasDisplayText(InternalGroupName);

        StringBuilder msg = new StringBuilder();

        msg.AppendLine(
            "Excel「" + groupDisplay + "」群組欄位不完整。");

        msg.AppendLine(
            "若要匯入此資料，Excel 必須包含完整欄位：");

        msg.AppendLine();

        // Display the complete field list of this BulkDescription group.
        foreach (BulkColumn each in basis.Values)
        {
            msg.AppendLine(
                GetParentAliasDisplayText(each.FullDisplayText));
        }

        Enabled = false;
        Checked = false;
        ToolTipText = msg.ToString();
    }
    else
    {
        Enabled = true;
    }
}

Review the actual existing code before applying the change.

Do not blindly replace unrelated logic.


## Parent Alias Requirement

The existing parent alias behavior must remain unchanged.

Internal names:

父親
母親

Excel/UI names:

家長1
家長2

Therefore, when displaying the complete group field list, continue using:

GetParentAliasDisplayText(each.FullDisplayText)

Example internal BulkDescription:

父親:學歷
父親:職業
父親:電話

The user-facing message must be:

Excel「家長1」群組欄位不完整。
若要匯入此資料，Excel 必須包含完整欄位：

家長1:學歷
家長1:職業
家長1:電話

Do NOT display:

父親:學歷
父親:職業
父親:電話

to the user.


## 家長2 Example

Internal BulkDescription:

母親:學歷
母親:職業
母親:電話

User-facing message:

Excel「家長2」群組欄位不完整。
若要匯入此資料，Excel 必須包含完整欄位：

家長2:學歷
家長2:職業
家長2:電話


## 前級 Example

If BulkDescription defines:

前級:學校名稱
前級:學校所在地
前級:班級
前級:座號
前級:備註

and Excel contains only:

前級:學校名稱

the field-selection screen should show:

Excel「前級」群組欄位不完整。
若要匯入此資料，Excel 必須包含完整欄位：

前級:學校名稱
前級:學校所在地
前級:班級
前級:座號
前級:備註

The message must include the existing field:

前級:學校名稱

because the purpose is now to show the complete required group format.


## 監護人 Example

If BulkDescription defines:

監護人:學歷
監護人:職業
監護人:電話

and Excel contains only:

監護人:電話

the message must display:

Excel「監護人」群組欄位不完整。
若要匯入此資料，Excel 必須包含完整欄位：

監護人:學歷
監護人:職業
監護人:電話


## Complete Group Behavior

If Excel already contains every field defined in basis.Values:

Expected:

Enabled = true

No incomplete-group tooltip should be displayed.

Do not disable the group.


## Preserve Current Group Detection

Do not change the current BulkDescription-based group detection in DisplayColumns().

Keep the logic conceptually equivalent to:

BulkColumnCollection groupColumns = null;

bool isBulkGroup =
    bfields.TryGetValue(
        each.InternalGroupName,
        out groupColumns)
    && groupColumns != null
    && groupColumns.Count > 1;

if (isBulkGroup)
{
    each.CheckAccept(groupColumns);
}

The system BulkDescription must remain the source of truth for group definitions.


## Do Not Return to Excel Count-Based Detection

Do NOT change the logic back to relying on:

each.IsGroupColumn

or:

SheetColumns.Count > 1

to determine whether a field is a system group.

A BulkDescription group must still be recognized even if the Excel file contains only one field from that group.


## Do Not Hard-Code Group Fields

Do not hard-code:

監護人
前級
戶籍
聯絡
家長1
家長2
其它電話

and do not hard-code their child-field lists.

The complete field list must always come dynamically from:

BulkColumnCollection basis

which originates from:

Context.BulkDescription.Columns


## Preserve Existing Behavior

Do not change:

- GroupBulkColumn()
- GroupSheetColumn()
- BulkColumnCollection.GetColumnList()
- BulkDescription.GenerateInsertRequest()
- BulkDescription.GenerateUpdateRequest()
- StudentBulkProcess
- XML structure
- database structure
- identification-field handling
- validation-field handling
- Insert/Update behavior
- ReadOnly logic
- Required-field logic
- parent field normalization
- parent alias priority rules


## Required Tests

### Test 1 - Incomplete 監護人 Group

Excel:

監護人:電話

Missing other BulkDescription group fields.

Expected:

- 監護人 remains visible
- cannot be checked
- Enabled = false
- Checked = false
- tooltip displays ALL fields required by the 監護人 group


### Test 2 - Complete 監護人 Group

Excel contains the entire group.

Expected:

- Enabled = true
- group can be checked
- no incomplete-group warning


### Test 3 - Incomplete 前級 Group

Excel contains only part of the 前級 group.

Expected:

Tooltip displays the COMPLETE BulkDescription-defined 前級 field list.

Not only the missing fields.


### Test 4 - 家長1 Group

Use an incomplete 家長1 group.

Expected:

The complete field list is shown using:

家長1

not:

父親


### Test 5 - 家長2 Group

Use an incomplete 家長2 group.

Expected:

The complete field list is shown using:

家長2

not:

母親


### Test 6 - Other Groups

Verify at least:

戶籍
聯絡
其它電話

If incomplete:

display complete group field list.

If complete:

remain selectable.


## Regression Verification

Verify that this UI-message change does not affect:

- which groups are considered complete/incomplete
- actual selected fields
- validation processing
- generated XML
- database update
- 身分證號 identification
- 學號 identification
- 學生系統編號 identification
- 家長1 / 家長2 alias handling


## Build Verification

After modification:

1. Review Git diff.
2. Confirm the primary functional change is inside CheckAccept().
3. Clean Solution.
4. Rebuild Solution.
5. Confirm no new compilation errors.
6. Test the field-selection screen with complete and incomplete groups.
7. Do not mark tests as PASS unless actually executed.


## Completion Documentation

After completing the work, update:

匯入學生基本資料調整0810.md

Record:

- adjustment purpose
- original behavior
- new behavior
- modified file
- modified method
- why the group completeness rule itself was not changed
- how the complete group field list is generated dynamically
- parent alias display handling
- test cases executed
- actual results
- build result
- regression results
- any untested items


## Expected Final Result

Before:

Incomplete group
-> disabled
-> tooltip shows only missing fields

After:

Incomplete group
-> disabled
-> tooltip shows the COMPLETE BulkDescription-defined field list

Complete group
-> enabled normally

All group definitions continue to come dynamically from BulkDescription.

No XML/database/import architecture changes are required.