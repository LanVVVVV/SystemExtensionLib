# SystemExtensionLib

SystemExtensionLib is a mod for **Monster Black Market** —— designed to provide system APIs and shared utilities for other mods.

### Features

- **Mod Config Management**
  - Automatically initializes directories
  - Exports Config files
  - Manages external Config files
  - Each Mod has its own independent Config subdirectory

- **Extended Info Slot Management**
  - Adds an extended info slot area to the character information panel
  - Provides registration methods to unify the creation and management of extended info slots in Extended Area
  - Supports registering visibility conditions through a management component

- **Gallery Extended Info Slot Extension**
  - Provides registration methods to unify the creation and management of Gallery extended info slots
  - Allows other Mods to add or modify slots in the gallery interface
  - Supports automatic adjustment of layout background size

- **Embedded Resource Handling**
  - Provides functions to load and export embedded resources
  - Supports export from specific namespaces to external files

- **File Filtering Tools**
  - Offers utilities to build file filtering logic
  - Developers can combine conditions to match files by extension, folder path, or wildcard patterns

- **File and Path Utilities**
  - Provides helper functions for file and path operations
  - Includes directory creation and path conversion

- **UI Localization Helpers**
  - Supplies localization support for `ReferenceString` and `ReferenceFormattingText` components
  - Simplifies multilingual UI development

### Requirements

| Dependency           | Version   | Link |
| -------------------- | --------- | ---- |
| Monster Black Market | ≥ 2.1.2.0 |      |
| MBM.ModLoader        | ≥ 0.7.1    | [F95Zone](https://f95zone.to/threads/monster-black-market-mbm-modloader-0-7-1.290109/) |

### For Other Modders

This library ships with a complete XML documentation file. After referencing the DLL in your project, you will automatically get full IntelliSense tooltips and parameter descriptions in your IDE.