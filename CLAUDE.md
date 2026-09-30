# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

A discrete-event combat simulator for a warrior in World of Warcraft Forever. The code was copied from a WoW TBC hunter simulator and still contains hunter-specific content (Auto Shot, aspects, hunter talents and procs, ranged stat calculators, hunter gear data). That content is being replaced with warrior equivalents; treat it as scaffolding to convert, not as the target design, and do not add new hunter-specific code. C# on .NET 10, MSTest. The solution and its projects live under `src/`: the library `WarriorForeverSim` (its `Program.cs` is still a placeholder), `WarriorForeverSim.Tests`, and `WarriorForeverSim.Api`, an ASP.NET Core minimal API that backs the React + TypeScript (Vite) web UI in `web/`.

## Commands

Run from `src/` (the solution file is there, and CI uses `working-directory: src`).

```
dotnet restore WarriorForeverSim.sln
dotnet build WarriorForeverSim.sln --configuration Release --no-restore
dotnet test WarriorForeverSim.sln --configuration Release --no-build
dotnet test WarriorForeverSim.Tests --filter "FullyQualifiedName~SerpentsSwiftnessTests"   # one class
dotnet test WarriorForeverSim.Tests --filter "Name=SerpentsSwiftness"                      # one method
dotnet format WarriorForeverSim.sln --verify-no-changes --severity warn                    # what CI's lint job runs
dotnet format WarriorForeverSim.sln                                                        # fix formatting
```

Web UI: run `dotnet run --project WarriorForeverSim.Api` (from `src/`, serves on http://localhost:5058), then `npm install` and `npm run dev` from `web/` (Vite proxies `/api` to the API). `npm run lint` and `npm run build` are what CI's web job runs. The build writes to `src/WarriorForeverSim.Api/wwwroot` (gitignored), so the API alone serves the built UI.

The API (`SimulationRunner.cs`) builds a `DefaultConfig` per iteration, overrides the request's settings, and aggregates `SimulationReport.FromState` results. The response also carries the first iteration's event-by-event combat log from `SimulationLogEntry.FromState`, which uses each event's `Description` (a virtual on `EventInfo`; override it on new event types). Simulations are serialized behind a lock because `RandomGenerator` and the stat calculator cache are process-wide singletons.

CI (`.github/workflows/ci.yml`) fails on any `.editorconfig` rule at `warning` severity or above, so run `dotnet format` before pushing. Notable enforced rules: `var` everywhere, braces always, block-scoped namespaces, `using` outside namespace, `_camelCase` private fields, `readonly` where possible, no unused usings/members/assignments (IDE0005/0051/0052/0059).

## Architecture

**Event loop.** `Simulation.Run()` validates state, then loops: `ExecuteRotation()` queues abilities, `GetNextEvent()` pops the earliest `EventInfo` from `SimulationState.Events`, advances `CurrentTime`, and processes every event at that timestamp until `FightLength` is exceeded. Each event's `ProcessEvent(state)` mutates state and typically schedules follow-up events (e.g. `AutoShotCastEvent` schedules `AutoShotCompletedEvent` and `AutoShotCooldownCompletedEvent`).

**Procs are subscribers, not events.** After an event is processed, `EventPublisher.PublishEvent` appends it to `ProcessedEvents` and dispatches by type to the static classes in `Procs/` (e.g. `ExposeWeakness.ProcessEvent`), which roll for a proc and enqueue `*ProcEvent`/`*ExpiredEvent`. Adding a proc means: new event classes in `Events/`, a handler in `Procs/`, a `case` in `EventPublisher`, and usually an `Aura` enum value.

**State vs. config.** `SimulationState` holds mutable run-time data (event queue, `CurrentTime`, active `Auras`, warnings/errors). `SimulationConfig` holds inputs: `Gear`, `Buffs`, `Talents` (dictionary of `Talent` to rank), `PlayerSettings`, `BossSettings`, `SimulationSettings`. Validation issues are strings from `SimulationWarnings` / `SimulationErrors`; errors abort the run, warnings do not.

**Stat calculators are singletons with mock injection.** Every derived stat is a `BaseStatCalculator` subclass in `StatCalculators/` exposing a static `Calculate(state)` that routes through a per-type cached instance. Calculators layer base stats, gear totals (`Gear.GetStatTotal`), buffs, and talents in a specific order with `.Floor()` between multiplicative steps to mirror in-game rounding; preserve that ordering when editing. Tests replace a calculator with `BaseStatCalculator.InjectMock(typeof(X), new FakeStatCalculator(value))` and must call `ClearMocks()` in `[TestCleanup]`.

**Randomness is also a mockable singleton.** All rolls go through `RandomGenerator.Roll(RollType)`. Tests inject `FakeRandomGenerator` (scripted values, optionally per `RollType`) via `RandomGenerator.InjectMock` and clean up with `ClearMock()`.

**Gear is data-driven YAML.** Items, enchants, and gems are one `.yml` file each under `Gear/<Slot>/`, `Enchants/<Slot>/`, and `Gems/`, copied to the output directory and loaded lazily by `GearItemFactory` from the assembly location. Each file is validated against `Config/GearItem-Schema.json` before being mapped onto `GearItem` via `[YamlProperty("...")]` attributes, so a new stat needs a schema entry and an attributed property. Folder name must match a `GearType` enum value. Look up items by name with `GearItemFactory.Load("Item Name")` or the per-slot `LoadRanged(...)` etc.

**Code-driven gear.** Meta gems (`MetaGems/`, subclasses of `MetaGem`) and set bonuses (`GearSets/`, implementations of `IGearSet`) are discovered by reflection and their `Apply(state)` runs during `SimulationState.Validate()`. Adding one is just adding the class.

## Tests

Tests mirror the domain, not the source tree: `TalentTests/`, `BuffTests/`, `AuraTests/`, `ProcTests/`, `MetaGemTests/`, `GearSetTests/`, plus `StatCalculatorTests.cs`, `SimulationTests.cs`, `ConfigValidationTests.cs`. Most tests build a bare `SimulationState`, set a race/talent/buff, and assert a calculator result. Expected base stats live in `Constants.cs` (Draenei is the reference race). Tests that inject mocks must clear them or they leak into other tests in the same run.
