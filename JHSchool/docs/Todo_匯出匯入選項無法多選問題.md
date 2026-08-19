## Objective
Fix the student basic-data import/export field lists so users can select multiple options with the mouse.

The following operations must work normally:

- Drag the mouse over multiple items to select them.
- Hold `Ctrl` and click to select multiple non-contiguous items.
- Hold `Shift` and click to select a continuous range.

After completing the changes, document the implementation and test results in:

`匯出匯入選項調整0731.md`

---

## Target Files

1. `StudentExportWizard.cs`
2. `StudentImportWizard.cs`

Review the related Designer files only when necessary. Do not change the UI layout unless required.

---

## Current Problem

Both forms register an `ItemSelectionChanged` event in `ConfigureFieldListView()`.

The current event handler immediately clears the selection:

```csharp
private void FieldListView_ItemSelectionChanged(
    object sender,
    ListViewItemSelectionChangedEventArgs e)
{
    if (e.IsSelected)
        e.Item.Selected = false;
}
```

Because every selected item is immediately changed back to `Selected = false`, the following ListView functions cannot work:

- Mouse drag selection
- `Ctrl` multi-selection
- `Shift` range selection

This issue exists in both:

- Export field ListView: `listView`
- Import field ListView: `lvSourceFieldList`

---

## Required Changes

### 1. Fix `StudentExportWizard.cs`

Update `ConfigureFieldListView()`:

- Keep the existing background and border settings.
- Explicitly set `listView.MultiSelect = true`.
- Set `listView.HideSelection = false` so the selected items remain visually visible when focus changes.
- Remove or stop registering `FieldListView_ItemSelectionChanged`.
- Remove the unused event handler if no other code uses it.

Expected direction:

```csharp
private void ConfigureFieldListView()
{
    listView.MultiSelect = true;
    listView.HideSelection = false;
    listView.BackColor = SystemColors.Window;
    listView.BorderStyle = BorderStyle.FixedSingle;
}
```

### 2. Fix `StudentImportWizard.cs`

Update `ConfigureFieldListView()`:

- Keep the existing background and border settings.
- Explicitly set `lvSourceFieldList.MultiSelect = true`.
- Set `lvSourceFieldList.HideSelection = false`.
- Remove or stop registering `FieldListView_ItemSelectionChanged`.
- Remove the unused event handler if no other code uses it.

Expected direction:

```csharp
private void ConfigureFieldListView()
{
    lvSourceFieldList.MultiSelect = true;
    lvSourceFieldList.HideSelection = false;
    lvSourceFieldList.BackColor = SystemColors.Window;
    lvSourceFieldList.BorderStyle = BorderStyle.FixedSingle;
}
```

---

## Important Behavior Rules

1. Do not confuse ListView selection with checkbox state.

   - `Selected` means the item is highlighted/selected.
   - `Checked` means the item's checkbox is checked.

2. Multi-select must not automatically check or uncheck items.

3. Keep the existing checkbox behavior unchanged.

4. Keep the existing import restrictions unchanged, including:

   - Disabled fields cannot be checked.
   - Read-only fields cannot be imported.
   - Required fields remain checked and locked.
   - `lvSourceFieldList_ItemCheck` must continue to enforce the current rules.

5. Keep the existing "全選／全部選取" checkbox behavior unchanged.

6. Do not change:

   - Import/export data logic
   - Parent field mappings (`家長1／家長2` and internal `父親／母親`)
   - XML field names or XML structure
   - Required-field highlighting
   - Wizard navigation
   - Validation logic
   - File output format

7. Avoid unrelated refactoring.

---

## Verification Checklist

### Export Form

- [ ] Open `匯出學生基本資料`.
- [ ] Drag the mouse across multiple fields; all covered fields remain selected.
- [ ] Hold `Ctrl` and click multiple non-contiguous fields.
- [ ] Hold `Shift` and select a continuous range.
- [ ] Confirm selection highlighting remains visible.
- [ ] Confirm selecting items does not change checkbox states.
- [ ] Confirm checking/unchecking individual fields still works.
- [ ] Confirm `全選` still checks or unchecks all fields.
- [ ] Confirm export uses only checked fields, not merely selected fields.

### Import Form

- [ ] Open `匯入學生基本資料` and reach the field-selection page.
- [ ] Drag the mouse across multiple fields; all covered fields remain selected.
- [ ] Hold `Ctrl` and click multiple non-contiguous fields.
- [ ] Hold `Shift` and select a continuous range.
- [ ] Confirm selection highlighting remains visible.
- [ ] Confirm selecting items does not change checkbox states.
- [ ] Confirm enabled fields can still be checked normally.
- [ ] Confirm disabled/read-only fields still cannot be checked.
- [ ] Confirm required and locked fields retain their original behavior.
- [ ] Confirm `全部選取` still processes only enabled fields.
- [ ] Confirm import validation and import execution remain unchanged.

---

## Completion Documentation

Create or update:

`匯出匯入選項調整0731.md`

The document must include:

1. Problem description.
2. Root cause.
3. Modified files.
4. Modified methods.
5. Before/after behavior.
6. Explanation that `Selected` and `Checked` are separate states.
7. Confirmation that import/export data logic was not changed.
8. Manual test steps and results for mouse drag, `Ctrl`, `Shift`, checkbox operations, and select-all operations.
9. Build result and any remaining issues.
