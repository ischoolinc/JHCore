## ADDED Requirements

### Requirement: Primary panel presentation setup remains synchronous
The system SHALL call `SetupPresentation()` for the initially-visible panel synchronously during `Program.Main()`, so that the panel the user lands on is fully wired (ribbon, filters, detail builders) before the module reports itself ready.

#### Scenario: Module startup completes with primary panel usable
- **WHEN** `Program.Main()` executes
- **THEN** the primary panel's `SetupPresentation()` SHALL have completed before `Main()` returns

### Requirement: Non-primary panel presentation setup is deferred past window show
The system SHALL NOT block `Program.Main()`'s return on the three non-primary panels' `SetupPresentation()` calls. Instead, it SHALL schedule them to run on the UI thread after the message loop becomes idle (e.g. via `Application.Idle`, unsubscribed after first fire), so window appearance is not delayed by their UI-construction cost.

#### Scenario: Main() returns before non-primary panels are wired
- **WHEN** `Program.Main()` executes
- **THEN** `Main()` SHALL return without having called the non-primary panels' `SetupPresentation()` methods inline
- **THEN** those calls SHALL instead be scheduled to run once the application message loop is idle

#### Scenario: Deferred setup still runs exactly once
- **WHEN** the `Application.Idle` event fires for the first time after module startup
- **THEN** the system SHALL invoke the three deferred `SetupPresentation()` calls
- **THEN** the system SHALL unsubscribe from `Application.Idle` so the deferred setup does not run again on subsequent idle cycles

### Requirement: Relative ordering among deferred setup and its dependents is preserved
Code that depends on a deferred panel's `SetupPresentation()` having completed (e.g. `Class.Instance.AddDetailBulider(...)` gated on `IClassBaseInfoItemAPI` discovery) SHALL execute after that panel's `SetupPresentation()` call, in the same relative order as before this change, even though both now run inside the deferred callback rather than inline in `Main()`.

#### Scenario: Class detail-builder registration still follows Class setup
- **WHEN** the deferred setup callback runs
- **THEN** `Class.Instance.SetupPresentation()` SHALL complete before the `IClassBaseInfoItemAPI`-gated `Class.Instance.AddDetailBulider(...)` call executes

#### Scenario: Deferred panels are wired before a user can plausibly navigate to them
- **WHEN** a user opens the module and switches to a non-primary panel
- **THEN** that panel's ribbon buttons, filters, and detail builders SHALL already be present and functional, with no missing-menu-item regression versus the fully-synchronous behavior prior to this change
