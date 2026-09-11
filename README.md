# Falling Axe

A 2D arcade ASMR game about a pickaxe falling through endless rows of blocks and breaking ore. Controls are minimal — everything is driven by physics, TNT explosions and randomness. Built with Unity 6 (URP 2D).

## About the game

- Press **Enter** — a player (pickaxe or cube) shoots out of the spawn point with a random angle and impulse.
- The player falls down through rows of blocks. Every contact with a block damages it.
- **Ore** (coal, copper, iron, gold, lapis, redstone, diamond, emerald) adds to the total score and to its own counter.
- **Brainrot blocks** (Tralalero, Crocodilo, Balerina) are counted separately and **only** when broken by the player himself — explosions don't affect them.
- **TNT** ignites on hit, swells up, explodes after a delay and triggers a chain reaction across neighbouring blocks.
- If the player stays still for 2 seconds, it ignites itself, explodes and respawns at the same spot with a new random look.
- The side walls are **bounce pads** (`BouncePad`): they knock the player back into the field.
- A **chasing ceiling** crawls down behind the player: it destroys blocks left above and knocks the player down if it rises too high.

## Script structure

Everything lives in `Assets/Scripts/`:

| Script | What it does |
|---|---|
| `ActivateOnEnter.cs` (`SpawnPlayerOnEnter`) | Starts the game on Enter, blinks the hint text, spawns a random player from the pool (never the same one twice in a row), spawn particle and sound |
| `PlayerCube.cs` | Player physics, self-ignition when idle, radius explosion, chain reaction through TNT, respawn via a temporary `RespawnTimer` |
| `DestructibleCube.cs` | Block with ore type (`OreType`), hit count, damage colour, TNT mode (delay, radius, force) and score awarding |
| `RowSpawner.cs` | Generates rows of blocks below the camera across the full screen width using weighted chances (`CubeSpawnData`), cleans up blocks that went above |
| `ScoreManager.cs` | Singleton with the total score and per-ore counters, renders to TextMeshPro |
| `BouncePad.cs` | Bounce pad: pushes the player along the normal, away from the contact point, or in a custom direction |
| `FollowPlayerDown.cs` | Ceiling: follows the player from above, destroys blocks and knocks the player down |
| `FollowPlayerVertical.cs` | Object rigidly locked to the player's height (walls) |
| `CameraFollow.cs` | Camera follows the player on Y with an offset |

## Prefabs

- `Assets/Prefabs/player/` — `player` (cube) and `player pickaxe` (pickaxe), both tagged `Player`.
- `Assets/Prefabs/minecraft blocks/` — ore, stone, dirt, TNT (100 hits, explosive).
- `Assets/Prefabs/new blocks/` — brainrot blocks (tag `Brainrot`), `Cube Strong` with 10 hits, decorative blocks.
- Root of `Assets/Prefabs/` — particles for brainrot, TNT and walls.

## Tech

- Unity **6000.3.8f1**
- Universal Render Pipeline 17.3 (2D Renderer)
- Input System 1.18 (scripts still use the legacy `Input.GetKeyDown`)
- TextMeshPro for UI
- Build scene: `Assets/Scenes/game full hd.unity`, 1920×1080 fullscreen

## How to run

1. Open the project folder in Unity Hub (version 6000.3.8f1).
2. Open the scene `Assets/Scenes/game full hd.unity`.
3. Play → press **Enter**.

## Asset licenses

The project uses third-party demo packs: **Pixel Skies DEMO** (backgrounds), **Simple FX Kit** (particles) and the standard TextMeshPro examples. All rights belong to their respective authors.

---

**MORENTY**
