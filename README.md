# Hephaestus VFX

Hephaestus VFX is a part of the Hephaestus framework. It provides a small,
Zenject-based VFX system for Unity that spawns effect prefabs by typed `enum`
keys generated from a config.

## Features

- Spawn VFX prefabs by strongly-typed `enum` keys (generated from a config asset).
- Stable key ids: reordering or removing keys never remaps existing library entries.
- Non-looping particle effects destroy themselves once their particles finish.
- Editor tooling: key list with validation, enum export, key → prefab library with Undo.

## Installation

The package is distributed via UPM from the WTFGames registry and depends on
Extenject (Zenject) from OpenUPM. Add both registries and the package to your
`Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "WTFGames",
      "url": "https://upm.wtfgames.com.ua/",
      "scopes": ["com.wtfgames.hephaestus"]
    },
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": ["com.svermeulen.extenject"]
    }
  ],
  "dependencies": {
    "com.wtfgames.hephaestus.vfx": "1.0.1"
  }
}
```

## Setup

### 1. Create the assets

Via the `Create > HephaestusMobile/Core/VFX` menu, create:

- **VFXLibraryConstants** — the list of VFX keys.
- **VFXLibrary** — maps each key to a prefab (assign the `VFXLibraryConstants`
  asset to it).
- **VFXManagerConfig** — references the `VFXLibrary`.
- **HephaestusVFXManagerSOInstaller** — references the `VFXManagerConfig`.

### 2. Define VFX keys and export the enum

Select the `VFXLibraryConstants` asset:

1. Add keys (e.g. `EXPLOSION`, `HIT_SPARKS`). Keys are upper case; spaces become
   underscores.
2. Pick an output folder inside the project and click **Export to enum**. It
   generates a `byte` enum in the `{CompanyName}.{ProductName}.VFX` namespace.
3. On the `VFXLibrary` asset, bind each key to its prefab.

Every key gets a permanent id that becomes its enum value. Ids are never reused,
so removing a key shows its library entries as `<Missing key N>` instead of
silently pointing them at another effect. Re-export the enum after changing keys.

### 3. Register the installers

Add `HephaestusVFXManagerSOInstaller` to your `SceneContext`/`ProjectContext`,
and install the manager bindings from your own installer:

```csharp
public override void InstallBindings()
{
    HephaestusVFXManagerInstaller.Install(Container);
}
```

This binds `IVFXManager` as a singleton. Zenject initializes it, so there is no
need to call `Initialize()` yourself.

## Usage

```csharp
public class HitEffects
{
    private readonly IVFXManager _vfx;

    public HitEffects(IVFXManager vfx) => _vfx = vfx;

    public void OnHit(Vector3 point) => _vfx.PlayVFX(VFXLibraryConstants.HIT_SPARKS, point);

    public void OnPickup(Transform owner) => _vfx.PlayVFX(VFXLibraryConstants.EXPLOSION, owner.position, owner);
}
```

`position` is in world space; the effect keeps the prefab's rotation and is
parented to `parent` when one is passed.

### Effect lifetime

- If none of the effect's particle systems loops, the instance is destroyed after
  `startDelay + duration + startLifetime` (scaled by `simulationSpeed`).
- Looping effects and prefabs without particle systems stay alive; destroy them
  yourself.
- A key without a mapped prefab logs a warning and spawns nothing.

## Requirements

- Unity 2019.2 or newer.
- Extenject `9.2.0-stcf3`.

## License

Copyright (C) 2021-2026 Serhii Chechui (WTFGames).

Hephaestus VFX is free software: you can redistribute it and/or modify it under
the terms of the GNU General Public License as published by the Free Software
Foundation, either version 3 of the License, or (at your option) any later
version (`GPL-3.0-or-later`). See [LICENSE.md](LICENSE.md) for the full text.
