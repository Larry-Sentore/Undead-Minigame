# Undead Minigame

A 2D top-down survival shooter built in Unity. Move around the arena, survive increasingly large enemy waves, collect experience gems, and try to finish with a high score.

## Run the project

The project editor version is **Unity 6.6.0f1**. Open the project in that version, allow Unity to import packages and compile scripts, open `Assets/Scenes/SampleScene.unity`, and press **Play**.

### Controls

| Action | Input |
| --- | --- |
| Move | WASD, arrow keys, or gamepad left stick |
| Aim | Mouse cursor |
| Shoot | Automatic; toward the cursor and independently at the nearest living enemy |

## Gameplay

- The player starts with 100 health. Enemy contact deals damage over time; reaching zero health ends the run.
- The spawner starts with a wave size of three and adds one enemy to each later wave. Enemies spawn around the player, with a four-second pause between waves.
- Zombies steadily chase the player. Skeletons resist some damage and periodically burst forward.
- Defeating an enemy adds its score value and drops an experience gem. Collecting a gem grants experience and additional score.
- The HUD displays score, kills, survival time, and health. At game over, the leaderboard displays the five highest saved scores.
- Scores are stored locally with Unity `PlayerPrefs`; they are not shared across devices.

Gameplay values such as wave sizes, spawn radius, health, movement, damage, and firing rates can be tuned in the Unity Inspector.

## OOP and design patterns

The game demonstrates **encapsulation, abstraction, inheritance, and polymorphism**. `Enemy` is an abstract base class; `ZombieEnemy` and `SkeletonEnemy` inherit shared health and death behavior and specialize movement and damage. `IDamageable` and `ICollectible` let systems interact through reusable contracts.

Three design patterns used in the game are:

1. **Singleton-style access:** `GameManager.Instance` provides access to run state, while `PlayerController.Instance` provides the active player to enemies and the camera.
2. **Factory:** `EnemyFactory` selects and instantiates an enemy prefab based on `EnemyType`.
3. **Observer:** enemy-killed, health, stat, and game-over events notify the spawner, game manager, and UI without requiring direct references between every system.

`List<T>` and `IReadOnlyList<T>` are used for typed collections, alongside the damage and collectible interfaces, to keep systems reusable and modular.

## Algorithms

- **Nearest-enemy search:** `AutoWeapon` scans active enemies and compares squared distances to select the closest living target. The scan is O(n) in the number of active enemies.
- **Leaderboard Merge Sort:** `LeaderboardManager` recursively sorts scores from highest to lowest in O(n log n) time and O(n) auxiliary storage.
- **Skeleton burst state:** `SkeletonEnemy` uses two timers to alternate between burst movement and cooldown at O(1) work per physics update.

The full OOP discussion, pattern explanations, UML class diagram, algorithm steps, complexity, and reflection are in the [project report](docs/PROJECT_REPORT.md). The diagram is also available as an [SVG image](docs/uml-class-diagram.svg), and the [video walkthrough script](VIDEO_SCRIPT.md) includes timed presentation cues.

## Project layout

```text
Assets/
  Scenes/SampleScene.unity
  Scripts/
  prefabs/
Packages/manifest.json
ProjectSettings/
docs/
```

Gameplay code is in `Assets/Scripts/`. The project uses Unity's Input System package. The project includes an imported art collection under `Assets/Undead Survivor`; check the original package or asset listing for creator and license details when preparing attribution.
