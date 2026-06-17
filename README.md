# Rift Protocol

**Rift Protocol** is a 2D pixel-art puzzle platformer developed with Unity.

Players must switch between two versions of the same laboratory, overcome environmental hazards, interact with control panels, and reach the exit portal.

## Project Overview

The game takes place in a damaged futuristic laboratory affected by an unstable rift.

The player controls a small robot travelling through two overlapping versions of the laboratory:

- **Normal World**
- **Rift World**

The two worlds contain different platforms, hazards, barriers, and routes. Players must observe the differences between them and switch worlds at the correct time to progress through each level.

The main objective is to avoid traps, solve environmental puzzles, and reach the portal at the end of the level.

## World-Switching Mechanic

World switching is the core mechanic of the game.

Pressing `Space` switches the level between the Normal World and the Rift World. Some platforms, hazards, and paths only exist in one world.

Players must choose the correct world for each situation and may also need to switch worlds while moving or jumping.

For example:

- A blocked route in the Normal World may be open in the Rift World.
- A safe platform in one world may become a hazard in the other.
- Some obstacles can only be passed by switching worlds at the correct time.

## Core Mechanics

### World Switching

The level is divided into two object sets: WorldA and WorldB. Pressing `Space` swaps which set is active, changing the available routes, hazards, and platforms.

### Hazards

The game includes several hazard types:

- Continuous laser traps.
- Saw traps.
- Timed barriers.
- World-specific trap tilemaps.
- Animated lethal barriers.

Hazards trigger player death and respawn through the shared player and game manager systems.

### Control Panels

Control panels let the player interact with the environment. A panel can activate or deactivate linked objects, play audio feedback, trigger fade effects, or load another scene after a scripted sequence.

### Moving Platforms

Moving platforms and moving tilemaps are used for timing-based traversal. Some moving objects can also block laser beams, creating safe windows for the player to pass.

## Controls

| Input | Action |
|---|---|
| `A` / `D` | Move left or right |
| `W` | Jump |
| `Space` | Switch between the Normal World and Rift World |
| `E` | Interact with control panels |
| `Esc` | Open or close the pause menu |

## Main Features

- 2D platforming movement with jump and wall jump support.
- Dual-world switching between Normal World and Rift World.
- Player death, respawn, and state reset behavior.
- Interactive control panels for environmental puzzle logic.
- Sprite-based continuous laser traps with blocking detection.
- Moving platforms and moving tilemaps.
- Saw traps, barriers, and world-specific hazard tilemaps.
- Pause menu, level completion UI, intro sequence, and ending scene.
- Audio feedback for player actions, UI buttons, hazards, and scene moments.

## Project Structure

| Path | Description |
|---|---|
| `Assets/Scenes` | Main game scenes, including menu, intro, levels, and ending |
| `Assets/Scripts` | Gameplay, hazard, interaction, audio, and UI scripts |
| `Assets/Scripts/UI` | Menu, pause, completion, hint, and UI controller scripts |
| `Assets/Prefabs` | Reusable gameplay prefabs such as player, panels, barriers, traps, and portal |
| `Assets/Art` | Character, tileset, background, trap, panel, and animation art assets |
| `Assets/UI` | UI-related assets |
| `Assets/Tiles` | Tile assets used by tilemaps |
| `TestDoc.md` | Manual testing and bug-fix verification document |

## Key Scripts

| Script | Purpose |
|---|---|
| `PlayerController.cs` | Handles player movement, wall jumping, audio feedback, trap detection, and moving platform support |
| `GameManager.cs` | Handles player death, death animation timing, and respawn |
| `WorldSwitcher.cs` | Toggles WorldA and WorldB object sets |
| `ContinuousLaser2D.cs` | Handles continuous laser collision, blocking, and sprite-based laser visuals |
| `RedPanelInteraction.cs` | Handles panel interaction, object activation/deactivation, audio, fade, and optional scene transition |
| `MovingPlatform2D.cs` | Handles moving platform behavior |
| `VerticalMover.cs` | Moves objects vertically using physics updates |
| `TimedBarrier.cs` | Controls timed barrier behavior |
| `SawHorizontalMover.cs` | Moves saw hazards horizontally |
| `LevelCompletionUI.cs` | Handles level completion UI and scene navigation |
| `PauseMenuUI.cs` | Handles pause menu behavior |
| `IntroSequenceController.cs` | Controls intro text sequence and transition into gameplay |
| `WorldPositionHintUI.cs` | Shows hints based on player world position |

## Setup and Running Instructions

### Requirements

- Unity Hub
- Unity Editor `2022.3.62f3c1`
- Git

### Clone the Repository

```bash
git clone https://github.com/Comp1exxx045/Game-Programing-Project.git
```

### Open the Project

1. Open Unity Hub.
2. Select **Add project from disk**.
3. Select the cloned `Game-Programing-Project` folder.
4. Open the project with Unity `2022.3.62f3c1`.
5. Open `Assets/Scenes/MainMenu.unity`.
6. Press the **Play** button in the Unity Editor.

## Testing

Manual testing was performed through Unity Editor playtesting.

The project includes a testing and bug-fix verification document:

```text
TestDoc.md
```

The test document covers:

- Core gameplay tests.
- Scene flow tests.
- Hazard and respawn tests.
- UI tests.
- Audio tests.
- Regression checks for known bugs.

## Notes

- The project relies heavily on Unity scene configuration, physics colliders, triggers, animation clips, and prefab references.
- Manual regression testing is important after editing scenes, hazards, panels, or build settings.
- If a scene transition fails, check that the target scene is included in Unity Build Settings.
- If a trap does not damage the player, check that it has the `Trap` tag and a valid collider or trigger collider.
- If world switching does not work as expected, check the `WorldSwitcher` WorldA and WorldB object arrays.
