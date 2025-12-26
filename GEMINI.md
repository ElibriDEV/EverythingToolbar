# GEMINI.md: EverythingToolbar Project

## Project Overview

This repository contains the source code for **EverythingToolbar**, a file search integration for the Windows taskbar powered by the "Everything" search tool. It is a Windows desktop application built with C# using WPF and .NET 8.

The project is structured into several parts:
- **EverythingToolbar**: The main WPF application that provides the UI, including the search window, results, and settings.
- **EverythingToolbar.Deskband**: A C# project that implements the deskband functionality, allowing the toolbar to be embedded into the Windows taskbar.
- **EverythingToolbar.Launcher**: A separate launcher application, primarily for Windows 11 where traditional deskband support is limited.
- **EverythingSDK**: A C++ project that provides the native interface to communicate with the "Everything" search service via its IPC mechanism.
- **Installer**: Contains Inno Setup scripts for creating the application installer.

The UI is built with WPF and features themes for both Windows 10 and 11, supporting light and dark modes. It is highly customizable, allowing users to define filters, custom actions, and keyboard shortcuts.

## Building and Running

The project uses MSBuild for building and Inno Setup for creating the installer. The continuous integration setup in GitHub Actions provides a clear reference for the build process.

### Prerequisites

- Visual Studio with .NET 8.0 SDK and C++ development tools.
- NuGet package manager.
- [Inno Setup](https://jrsoftware.org/isinfo.php) is required to build the installer.

### Key Commands

1.  **Restore Dependencies:**
    Before building, restore the NuGet packages for the solution.
    ```shell
    nuget restore EverythingToolbar.sln
    ```

2.  **Build the Solution:**
    Use MSBuild to compile the entire solution. Specify the `Release` configuration and the desired platform (`x64` or `arm64`).
    ```shell
    # For x64
    MSBuild EverythingToolbar.sln /p:Configuration=Release /p:Platform=x64

    # For ARM64
    MSBuild EverythingToolbar.sln /p:Configuration=Release /p:Platform=arm64
    ```

3.  **Run in Development:**
    -   **Deskband (Windows 10):**
        1.  Build the `EverythingToolbar.Deskband` project.
        2.  Run the `tools/install_deskband.cmd` script as an administrator to register the deskband.
    -   **Launcher (Windows 11 / Standalone):**
        1.  Set `EverythingToolbar.Launcher` as the startup project in Visual Studio.
        2.  Start debugging (F5).

4.  **Build the Installer:**
    After building the solution, use Inno Setup to create the installer package.
    ```shell
    # For x64
    iscc "Installer/Installer-x64.iss"

    # For ARM64
    iscc "Installer/Installer-arm64.iss"
    ```

## Development Conventions

### Coding Style

The project enforces a consistent coding style through an `.editorconfig` file. Key conventions include:
-   **Indentation:** 4 spaces.
-   **Line Endings:** CRLF.
-   **Braces:** C# formatting rules are specified, such as `csharp_new_line_before_open_brace = all`.
-   **Naming:** Interfaces should be prefixed with `I` (`IInterface`). Types and non-field members use PascalCase.

The project also uses `csharpier`, an opinionated C# code formatter, as seen in the `.github/workflows/csharpier.yml` workflow, to ensure consistent formatting across the codebase.

### Testing

The repository does not contain a dedicated unit testing project. Testing appears to be done manually.

### Contribution

Contributions are welcome. The `README.md` file encourages reporting bugs, requesting features, and submitting pull requests. The project uses Crowdin for localization.
