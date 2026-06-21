# SystemExtensionLib

SystemExtensionLib is a mod for **Monster Black Market** —— designed to provide system APIs and shared utilities for other mods.

### Features

- **Mod Configuration Management**<br>
  Manages external configuration files for Mods. It handles directory initialization and resource exporting, creating a separate configuration subdirectory for each Mod.
- **Embedded Resource Handling**<br>
  Provides functions to load and export embedded resources. It supports exporting resources from specific namespaces to external files.
- **File Filtering**<br>
  Includes a tool for building file filtering logic. Developers can combine conditions to create rules for matching files by extension, folder path, or wildcard patterns.
- **File and Path Utilities**<br>
  Offers basic helper functions for file and path operations, such as directory creation and path conversion.

### Requirements

| Dependency           | Version   |
| -------------------- | --------- |
| MBM.ModLoader        | ≥ 0.6.0   |
| Monster Black Market | ≥ 2.1.2.0 |

### For Other Modders

This library ships with a complete XML documentation file. After referencing the DLL in your project, you will automatically get full IntelliSense tooltips and parameter descriptions in your IDE.