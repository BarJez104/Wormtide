# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Wormtide** is a 3D game built with Godot 4.6 and C# (.NET 8.0). Physics is handled by Jolt Physics. The main entry point is `res://scenes/main.tscn`.

The game is inspired by **Magicka** — see [`GAME-INSPIRATION.md`](GAME-INSPIRATION.md) for an overview of its core mechanics and the design pillars that inform Wormtide's direction.

## Running and Building

```bash
# Build C# assemblies only (without editor)
dotnet build Wormtide.csproj
```

Normal development happens inside the Godot 4.6 editor (JetBrains Rider is configured as the external script editor via `.idea/`).

## Architecture

### Directory Layout

```
scenes/       — gameplay scenes (player, coin, main)
scripts/      — gameplay scripts attached to scenes/
components/   — reusable scene components (Valuable, ValuableCollector, PollingRemoval)
  scripts/    — scripts for components
    interfaces/ — abstract base classes
addons/       — editor plugins (godot_mcp)
```

New scenes go in `scenes/` with their script in `scripts/`. Reusable components go in `components/` with their script in `components/scripts/`.

### Scene Hierarchy

**`scenes/main.tscn`** — Root scene (`Node3D`):
- `DirectionalLight3D`
- `Player` (instance of `player.tscn`)
- `Map` (`StaticBody3D`) — 100×2×100 box ground plane
- Two `Area3D` coin instances (instances of `coin.tscn`)

**`scenes/player.tscn`** — Player (`CharacterBody3D`, script: `PlayerBody3D.cs`):
- `MeshInstance3D` — capsule mesh
- `Camera3D` — slightly elevated, looking downward
- `ValuableCollector` (instance of `components/ValuableCollector.tscn`) — sphere collision shape (radius ~2.7)
- `CollisionShape3D` — capsule body shape
- `Label` — coin counter HUD, yellow text with shadow

**`scenes/coin.tscn`** — Coin (`Node3D`, script: `Coin.cs`):
- `MeshInstance3D` — flat cylinder mesh (radius 0.25, height 0.1)
- `Valuable` (instance of `components/Valuable.tscn`) — value 5.0, uses `PollingRemoval`
- `PollingRemoval` (instance of `components/PollingRemoval.tscn`) — targets the root `Coin` node

**`components/Valuable.tscn`** — `Area3D`, script: `Valuable.cs`

**`components/ValuableCollector.tscn`** — `Area3D`, script: `ValuableCollector.cs`

**`components/PollingRemoval.tscn`** — `Node3D`, script: `PollingRemoval.cs`

### Scripts

**`scripts/PlayerBody3D.cs`** (`Wormtide.scripts` namespace)
- Inherits `CharacterBody3D`
- `[Export]` fields: `Speed` (5.0), `JumpVelocity` (4.5), `ValuableCollector`, `CoinsLabel`
- `_PhysicsProcess`: WASD movement, Space to jump, gravity; uses `UiActions` constants for input
- Subscribes to `ValuableCollector.ValuableCollected` to update the coin HUD label

**`scripts/Coin.cs`** (`Wormtide.scripts` namespace)
- Inherits `Node3D`
- Spins the coin around Y each `_Process` frame (`RotationSpeed` export, default 0.5)

**`scripts/consts/UiActions.cs`** (`Wormtide` namespace)
- Static class of string constants for all built-in Godot UI actions (`ui_accept`, `ui_left`, etc.)
- Always use these constants instead of raw strings when reading input

**`components/scripts/interfaces/Removal.cs`** (`Wormtide.components.scripts.interfaces` namespace)
- Abstract `Node3D` base with a single method: `void Begin(Node3D collector)`

**`components/scripts/Valuable.cs`** (global namespace)
- Inherits `Area3D`
- `[Export]` fields: `Value` (float), `RemovalType` (nullable `Removal`)
- Guards against double-collection with `IsCollected` flag
- Calls `RemovalType.Begin(collector)` if set, otherwise `QueueFree()`
- Emits `Collected` signal

**`components/scripts/ValuableCollector.cs`** (global namespace)
- Inherits `Area3D`
- `[Export]` field: `CollectedCoins` (float)
- On `AreaEntered`: if the area is a `Valuable`, adds its value and calls `OnCollected`
- Emits `ValuableCollected` signal

**`components/scripts/PollingRemoval.cs`** (`Wormtide.components.scripts` namespace)
- Inherits `Removal`
- Each `_Process` frame: moves `TargetOfDestruction` toward the collector, shrinks it to 50% scale proportional to distance travelled, then `QueueFree()`s it when within 0.05 units
- `[Export]` fields: `PullSpeed` (10.0), `TargetOfDestruction` (defaults to parent node)

### Key Config

- `project.godot` — Engine settings: Jolt Physics for 3D, Forward Plus renderer, D3D12 on Windows.
- `Wormtide.csproj` — Targets `net8.0`, nullable enabled, implicit usings enabled.

## C# / Godot Conventions

- All scripts use the `Wormtide` namespace (or a sub-namespace like `Wormtide.scripts`, `Wormtide.components.scripts`).
- Use `_PhysicsProcess` for movement and physics; `_Process` for purely visual updates (e.g. spinning).
- Read input via `Input.GetVector` / `Input.IsActionJustPressed` using constants from `UiActions`.
- Node references use `[Export]` properties wired in the scene, not hardcoded `GetNode` paths.
- `[Export]` fields on `CharacterBody3D`/`Area3D` nodes will show CS8618 nullable warnings — this is expected for Godot export fields and can be ignored.
