# SadChromaLib
The super-module of SadChromaLib. It implements a variety of commonly-used features in game development. Its design is based partially from my 'Lumbra' library, but adapted to Godot 4.1.

## Modules:

### SadChromaLib.AI:
  -> A module implementing AI components for in-game entities. It also features base classes for project-specific behaviour nodes and/or state machines.

### SadChromaLib.Animation:
  -> A module implementing procedural methods for animating in-game objects.

### SadChromaLib.Utils:
  -> A module implementing core types and helper classes for many SadChromaLib submodules.

### SadChromaLib.Persistence:
  -> A module implementing data persistence support (saving/loading).

### SadChromaLib.Tests:
  -> A module implementing instances for testing SadChromaLib functionality.

----

## Specialisations:
These submodules implements case-specific functionality that isn't required by every game.

### Inventory (SadChromaLib.Specialisations.Inventory):
#### (https://github.com/SadColourfulHues/Inventory)
  -> A submodule implementing inventory and crafting functionality.

### Entities (SadChromaLib.Specialisations.Entities):
#### (https://github.com/SadColourfulHues/Entities)
  -> A submodule implementing health and status effect functionality

### Dialogue (SadChromaLib.Specialisations.Dialogue):
#### (https://github.com/SadColourfulHues/Dialogue)
  -> A submodule implementing a custom dialogue script parser and graph-based dialogue playback.

### Quests (SadChromaLib.Specialisations.Quests):
#### (https://github.com/SadColourfulHues/Quests)
  -> A submodule implementing a basic questing system.


