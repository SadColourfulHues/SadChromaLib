# SadChromaLib
The supermodule of SadChromaLib. It implements a variety of commonly-used features in game development. Its design is based partially from my 'Lumbra' module, but adapted to Godot 4.2.

## Core Modules

### SadChromaLib.Utils:
-> A module implementing core helper classes for common game-dev tasks.

### SadChromaLib.Types:
-> A module implementing core types used in SCHLib sub/modules.

### SadChromaLib.Persistence:
-> A module implementing helper classes to assist in de/serialising binary data.

----

## Optional Modules
These modules offer commonly-used functions in game-dev, but may not be needed in some projects.

### [SadChromaLib.UI](https://github.com/SadColourfulHues/UI)
-> A module implementing common user-interface elements.

*(This submodule can be used without needing the supermodule!)*

### [SadChromaLib.Audio](https://github.com/SadColourfulHues/Audio)
-> A module implementing common audio playback functionality.

### [SadChromaLib.Animation](https://github.com/SadColourfulHues/Animation)
-> A module implementing helper classes for in-game animation

### [SadChromaLib.AI](https://github.com/SadColourfulHues/AI)
-> A module implementing AI components for in-game entities. It currently offers support for   behaviour node and state machine controllers.

### [SadChromaLib.Input](https://github.com/SadColourfulHues/Input)
-> A module implementing helper classes for common input processing tasks.

----

## Specialisation Submodules
These submodules implements case-specific functionality that isn't required by every game.

### [SadChromaLib.Specialisations.Inventory](https://github.com/SadColourfulHues/Inventory):
-> A submodule implementing inventory and crafting functionality.

### [SadChromaLib.Specialisations.Entities](https://github.com/SadColourfulHues/Entities)
-> A submodule implementing health and status effect functionality

### [SadChromaLib.Specialisations.Dialogue)](https://github.com/SadColourfulHues/Dialogue)
-> A submodule implementing a custom dialogue script parser and graph-based dialogue playback.

*(To create binary dialogue graph files (.dgr), use [Dialogue Script editor](https://github.com/SadColourfulHues/DialogueEditor))*

### [SadChromaLib.Specialisations.Quests](https://github.com/SadColourfulHues/Quests)
-> A submodule implementing a basic questing system.

*(No longer being maintained, may be re-written in the future)*

## Testing
All SCHLib testing-related sources has been moved to the [SCHTesting](https://github.com/SadColourfulHues/SCHTesting) repository.

