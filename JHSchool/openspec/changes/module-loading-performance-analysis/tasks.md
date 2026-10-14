## 1. Detail Panel Lazy Loading

- [ ] 1.1 Add expanded/visibility state detection helpers in `PalmerwormItem` and `DetailContentBase`
- [ ] 1.2 Modify `ContentItemBulider` and `ContentItemBulider<T>` in `PalmerwormItem.cs` to gate `LoadContent()` behind expanded/visibility checks
- [ ] 1.3 Subscribe to expand/collapse events on detail panels to trigger `LoadContent()` immediately upon user expansion

## 2. Selection Change Debouncer

- [ ] 2.1 Integrate a WinForms `Timer` (with a 100ms threshold) into selection change event handlers in `LegacyPresentBase.cs`
- [ ] 2.2 Configure the Timer to trigger data synchronization for active/expanded detail panels on `Tick`
- [ ] 2.3 Cancel and restart the Timer on each subsequent selection change during continuous navigation to debounce requests

## 3. Cache Reuse and Local Optimization

- [ ] 3.1 Audit `BaseInfoPalmerwormItem.cs`, `ClassBaseInfoItem.cs`, and `TeachCourseItem.cs` to replace direct server queries with global in-memory cache lookups for Classes, Teachers, and Departments

## 4. Verification and Testing

- [ ] 4.1 Verify that collapsed detail panels do not invoke background workers or issue network requests during record navigation
- [ ] 4.2 Verify that expanding a previously collapsed panel triggers an immediate background data fetch for the active record
- [ ] 4.3 Verify that rapid selection changes (scrolling) ignore intermediate items and only load data for the final selected item
