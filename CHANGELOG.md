# Changelog

All notable changes to this project will be documented in this file in accordance with the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) guidelines.

## [Unreleased]

### fix
- Renamed `HephaestusUIManagerSOInstaller.cs` to `HephaestusVFXManagerSOInstaller.cs` so the ScriptableObject script matches its class name. The GUID is kept, existing installer assets stay valid.
- `VFXLibrary.GetPrefabByType` returns `null` for an unmapped key; `PlayVFX` logs a warning instead of throwing a `NullReferenceException`.
- Non-looping particle effects are destroyed once their particles finish instead of staying in the scene forever.
- The VFX handler survives scene loads and is destroyed on `Dispose`; `PlayVFX` before initialization logs an error.
- Added the missing `position` default to `IVFXManager.PlayVFX`.
- VFX keys have stable ids: reordering or removing keys no longer remaps library entries. Legacy assets are migrated automatically, keeping their index-based ids.
- Removing a key in the `VFXLibraryConstants` inspector no longer skips the next key.
- Enum export validates keys, the enum class name and the export folder, sanitizes the namespace and keeps the folder when the dialog is cancelled.
- The `VFXLibraryConstants` and `VFXLibrary` inspectors support Undo.

### deprecated
- `IVFXManager.Initialize` is obsolete: Zenject initializes the manager through `IInitializable`. It will be removed from the interface in the next major version.

### changed
- `VFXLibraryConstants.uiMapKeys` is no longer public; use `VFXLibraryConstants.keys`.

## [1.0.1] - 2024-10-04

### build
- Added `publishConfig` with the WTFGames UPM registry.

## [1.0.0] - 2024-10-04

### refactor
- Cleaned up namespaces (`WTFGames.Hephaestus.VFX`) and renamed the assemblies to `com.wtfgames.hephaestus.vfx` and `com.wtfgames.hephaestus.vfx.editor`.
- Added `VFXLibraryConstants` with enum export and a key picker in the `VFXLibrary` inspector.
- Added `VFXManagerHandler` and the Zenject installers `HephaestusVFXManagerInstaller` and `HephaestusVFXManagerSOInstaller`.

## [0.0.3] - 2024-02-29

### build
- Replaced the dependency on Hephaestus Core with a direct dependency on Extenject `9.2.0-stcf3`.

## [0.0.2] - 2021-03-23

### fix
- Namespace errors have been resolved to enhance compatibility and reduce conflicts within the project scope.

## [0.0.1] - 2021-03-23

### feat
- Initial creation of HephaestusVFX as a UnityPackage, introducing foundational visual effects capabilities to the project.
