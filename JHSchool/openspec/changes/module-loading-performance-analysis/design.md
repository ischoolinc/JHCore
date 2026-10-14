## Context

Over several previous iterations, the JHSchool module's startup performance was significantly optimized by:
1. Moving data synchronization (`SyncAllBackground()`) for Class, Student, Teacher, and Course to concurrent background threads.
2. Restricting initial Course loading to the configured default semester.
3. Optimizing high-volume XML parsing (e.g., in `StudentRecord`) to use surgical, lightweight node/attribute parsing.
4. Deferring the initialization of non-primary panels' UI setups (Class, Teacher, Course) until the application is in an `Idle` state.

Despite these startup gains, a major performance bottleneck remains in the **record navigation path** (the "Palmerworm" detail panel area). Currently, when a user selects a student, class, teacher, or course, *every single registered detail panel* eagerly spawns a `BackgroundWorker` to run backend SQL/DSA service queries concurrently, even if those panels are currently collapsed (and thus completely invisible to the user). Furthermore, holding down arrow keys to quickly scroll through the list triggers a cascade of concurrent thread pool tasks and network requests for every intermediate item, saturating connection limits and locking up the UI thread during serialization.

## Goals / Non-Goals

**Goals:**
- Eliminate redundant server and database load caused by collapsed detail panels.
- Throttle selection-change triggers during fast scrolling using debouncing.
- Maximize reuse of in-memory caches for static lookup data (e.g., classes, teachers, departments) within individual detail panels.
- Maintain full compatibility with the existing FISCA presentation framework.

**Non-Goals:**
- Redesigning the layout, visual styles, or user-facing behavior of the "Palmerworm" panel structure.
- Bypassing or rewriting the core `CacheManager` or `LegacyPresentBase` synchronization mechanisms.
- Removing or altering any existing security access control lists (ACLs).

## Decisions

### 1. Panel Expanded/Visible State-Gated Loading
- **Decision**: Delay invoking `LoadContent` or launching `_BGWorker.RunWorkerAsync()` for any detail content panel (inheriting from `FISCA.Presentation.DetailContent` or `PalmerwormItem`) unless that panel is currently expanded or visible. 
- **Rationale**: Collapsed panels are hidden from view. Retrieving their data eagerly serves no user purpose but incurs massive server-side query overhead and client-side XML parsing. Hooking into the panel's expansion/visibility state allows us to load data "just-in-time" when the user expands the panel.
- **Alternatives Considered**: 
  - *Keep eager loading but optimize database queries*: Rejected, because even optimal queries still require network roundtrips and connection handshakes, which aggregate to heavy latency over dozens of panels.

### 2. Standard WinForms Timer for Selection Change Debouncing
- **Decision**: Introduce a 100–150ms debounce window on selection change events before invoking detail-panel updates.
- **Rationale**: When users rapidly navigate lists (such as holding down the keyboard arrow keys), selection events fire in quick succession. A WinForms `Timer` running on the UI thread is the safest, most reliable way to throttle these. Each new selection event resets the timer; only when selection remains stable for >100ms does the timer tick, triggering a single background load for the final record.
- **Alternatives Considered**:
  - *`Task.Delay` with async/await*: Rejected. The legacy codebase is written in older synchronous .NET patterns. Introducing async/await context switching inside WinForms event handlers can cause subtle thread deadlock issues and UI state synchronization bugs on older runtime environments.

### 3. Read Static/Lookup Data from Global Memory Caches
- **Decision**: Refactor individual detail panels to query `Class.Instance.Items`, `Teacher.Instance.Items`, or `Department.Instance` in memory instead of sending separate `DSAServices.CallService` requests for basic metadata.
- **Rationale**: Global cache managers already sync class, teacher, and department information on startup. Individual panels can retrieve class names, tutor names, and department names instantly from memory without a network roundtrip.

## Risks / Trade-offs

- **[Risk] User experiences a slight loading spinner delay when expanding a panel**
  - *Mitigation*: The `picWaiting` loading spinner is already fully supported. Because other collapsed panels are not competing for threads and connections, a single expanded panel's background query will execute much faster, minimizing visible delay.
- **[Risk] Selection feel might seem slightly "delayed" for extremely fast users**
  - *Mitigation*: The 100ms threshold is selected to be just below human visual/cognitive reaction time (~200ms). This ensures that while the user perceives the selection as instant, the system successfully filters out intermediate rapid scrolling.
- **[Risk] Thread safety or state desynchronization during rapid tab-switching**
  - *Mitigation*: The debounce timer will cancel any active/pending load timers, and `PalmerwormItem`'s `_isBGBusy` pattern naturally coalesces subsequent requests.
