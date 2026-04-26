# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Wormtide** is a 3D game built with Godot 4.6 and C# (.NET 8.0). Physics is handled by Jolt Physics. The main entry point is `res://scenes/main.tscn`.

## Running and Building

This project is developed through the Godot editor. There are no standalone build scripts. Typical commands:

```bash
# Run the game from the command line
godot --path . --headless

# Build C# assemblies only (without editor)
dotnet build Wormtide.csproj
```

Normal development happens inside the Godot 4.6 editor (JetBrains Rider is configured as the external script editor via `.idea/`).

## Architecture

### Scene Hierarchy

- `scenes/main.tscn` — Root scene. Contains `DirectionalLight3D`, the `Player` instance, and a `Map` (StaticBody3D ground plane).
- `scenes/player.tscn` — Player scene: `CharacterBody3D` → `CollisionShape3D` (capsule) + `MeshInstance3D` + `Camera3D`.

### Scripts

All gameplay logic lives in `scripts/` as C# files under the `Wormtide` namespace.

- `scripts/PlayerBody3d.cs` — Attached to `player.tscn`. Inherits `CharacterBody3D`. Implements `_PhysicsProcess(delta)` for WASD movement, jumping (Space), and gravity via `move_and_slide`.

### Key Config

- `project.godot` — Engine settings: Jolt Physics for 3D, Forward Plus renderer, D3D12 on Windows.
- `Wormtide.csproj` — Targets `net8.0`, nullable enabled, implicit usings enabled.

## C# / Godot Conventions

- Scripts use `_PhysicsProcess` (not `_Process`) for movement and physics interactions.
- Godot input is read via `Input.GetVector` / `Input.IsActionPressed` using the built-in action map defined in `project.godot`.
- Node references inside scripts should use `GetNode<T>` or exported `[Export]` properties — avoid hardcoded paths where possible.
- New scenes should follow the pattern: scene file in `scenes/`, attached script in `scripts/`.