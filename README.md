## Scene Setup

The main playable scene is located under:

Assets/Game/Scenes

Open this scene to run and test the game.

---

## Level Tools (Editor)

Custom level tools are available via the Unity **Tools** menu.

From the Tools menu, you can:
- Preview the level grid live
- Create and edit level layouts
- Assign a level index

These tools are designed for fast iteration.

---

## Level Layout Creation

When a level layout is created using the editor tools:

- The layout is saved as a ScriptableObject under Game/Scriptables/Generated
- Generated assets are placed under:

Game/Scriptables/Levels

Each level layout contains:

Initial grid layout:
- Grid width and height
- Spawned hero positions and types

When a level layout asset is assigned to a level data:

- The level grid is generated using the layout’s width and height
- Heroes are spawned exactly as defined in the layout
  
---

## Level Configuration

All level configuration assets must be placed under:

Assets/Scriptables/LevelsConfig

