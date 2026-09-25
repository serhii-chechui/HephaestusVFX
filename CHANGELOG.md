# Changelog

All notable changes to this project will be documented in this file in accordance with the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) guidelines.

## [2.0.2] - 2026-09-25

### fix
- The folder picked with **Pick** in the `VFXLibraryConstants` inspector is saved and used for the enum export. The folder dialog used to break the inspector layout (`EndLayoutGroup: BeginLayoutGroup must be called first`) before the path was stored.
- The enum export path can be typed in; a missing export folder is created on export instead of blocking it.
- The asset setup replaces the export path `Assets/Hephaestus/VFX` left by 2.0.0 with `Assets/Hephaestus/Config/VFX` when that old folder no longer exists.

## [2.0.1] - 2026-09-25

### fix
- Missing VFX assets are created in `Assets/Hephaestus/Config/VFX`, next to the configs of the other Hephaestus packages, instead of `Assets/Hephaestus/VFX`. Assets already created in the old folder are still found and reused.

## [2.0.0] - 2026-09-24

### BREAKING CHANGES
- `IVFXManager.Initialize` is removed: Zenject initializes the manager through `IInitializable`. Drop explicit calls to it.
- `VFXLibrary.widgetsLibraryConstants` is renamed to `vfxLibraryConstants`. Existing `VFXLibrary` assets keep their reference.
- `VFXLibraryConstants.uiMapKeys` is no longer public; use `VFXLibraryConstants.keys`. Existing assets are migrated automatically.

### feat
- After import and on every script reload (not in batch mode) the package finds its assets anywhere in the project, creates the missing ones in `Assets/Hephaestus/VFX` and links them: `VFXLibrary` → `VFXLibraryConstants`, `VFXManagerConfig` → `VFXLibrary`, `HephaestusVFXManagerSOInstaller` → `VFXManagerConfig`. Only empty references are filled. Also available as **Hephaestus > VFX > Set Up Assets**.

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

### refactor
- Moved the editor scripts from `Editor/VFXManagerConfig/Editor/` to `Editor/`.
- Extracted key validation (`VFXKeysValidation`) and enum generation (`VFXEnumGenerator`) from the `VFXLibraryConstants` inspector.

### test
- Added PlayMode tests for the runtime (library lookup, key migration, spawning, effect lifetime, handler lifecycle) and EditMode tests for the editor (identifiers, key validation, enum generation, asset setup).

### docs
- Wrote the README and restored the changelog for 0.0.3–1.0.1.
- Added the GPL-3.0-or-later copyright notice to `LICENSE.md` and removed the duplicate `LICENSE`.

### build
- Declared `GPL-3.0-or-later` in `package.json` and switched the publish registry to https.

### ci
- Publish the package to the registry when a GitHub release is published.

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
