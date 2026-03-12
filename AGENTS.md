# Repository Guidelines

## Overview
TEngine is a Unity-based framework/project with a hybrid hot-update workflow (HybridCLR) and asset pipeline (YooAsset). Most day-to-day work happens inside the Unity project under `UnityProject/`.

## Project Structure
- `UnityProject/` — Unity project root.
- `UnityProject/Assets/TEngine/Runtime/` — runtime framework code.
- `UnityProject/Assets/TEngine/Editor/` — editor tooling (build, HybridCLR helpers, Luban tools).
- `UnityProject/Assets/GameScripts/Main/` — main game/startup assemblies.
- `UnityProject/Assets/GameScripts/HotFix/` — hot-update assemblies (keep “hotfix boundary” code here).
- `UnityProject/Assets/AssetRaw/` and `UnityProject/Assets/AssetArt/` — raw/hot-update assets and art assets.
- `Books/` — project documentation (start with `Books/1-快速开始.md`).
- `BuildCLI/` — headless build scripts for CI/local automation.
- `Tools/` — auxiliary scripts (e.g., building Luban tooling).

## Build, Test, and Development Commands
- **Open in Unity**: open `UnityProject/` with Unity **2021.3.20f1c1** (recommended) or newer.
- **Editor run**: use the `EditorMode` simulation mode and click `Launcher` (see `README.md`).
- **HybridCLR + packaging (menu)**:
  - `HybridCLR/Install...`, then `HybridCLR/Generate/All`
  - `HybridCLR/Build/BuildAssets And CopyTo AssemblyTextAssetPath`
  - `YooAsset/AssetBundle Builder`, then **Build And Run**
- **CLI Android build (Windows)**:
  - Edit `BuildCLI/path_define.bat` (`WORKSPACE`, `UNITYEDITOR_PATH`)
  - Run `BuildCLI/build_android.bat` (calls `-executeMethod TEngine.ReleaseTools.AutomationBuildAndroid`)
- **Luban tool build**: `Tools/build-luban.bat` or `Tools/build-luban.sh` (requires `dotnet`).

## Coding Style & Naming Conventions
- C# conventions: 4-space indentation; `PascalCase` for types/methods, `camelCase` for locals/fields.
- Keep editor-only code under an `Editor/` folder (Unity special folder semantics).
- Avoid moving hot-update code across the boundary: `Main/` vs `HotFix/` affects packaging and runtime behavior.

## Testing Guidelines
Unity uses the Unity Test Framework (NUnit-based). Prefer adding tests as assemblies with a `Tests` folder + `.asmdef`, then run via **Test Runner**. Command-line example:
`Unity.exe -runTests -batchmode -projectPath UnityProject -testPlatform EditMode -testResults .\\TestResults.xml`.

## Commit & Pull Request Guidelines
- Prefer Conventional Commits-style subjects (examples in docs): `feat: ...`, `fix: ...`, `docs: ...`.
- PRs should include: intent/summary, affected platforms (e.g., Windows/Android), and verification steps (Editor play, HybridCLR generate/build, AB build when applicable).

## Configuration & Safety
- Treat `BuildCLI/path_define.*` as machine-local configuration (do not commit personal paths).
- Do not hardcode credentials or keys; use environment variables or secure storage where needed.
