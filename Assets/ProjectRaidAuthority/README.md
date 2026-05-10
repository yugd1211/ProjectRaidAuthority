# ProjectRaidAuthority Asset Structure

`Assets/ProjectRaidAuthority` is the project-owned content root. Keep gameplay code, scenes, prefabs, art, audio, UI, settings, and tests here unless Unity requires a special folder name/location.

## Project-owned folders

- `Animations/` - animation clips, controllers, override controllers.
- `Art/` - source visual assets grouped by type (`Sprites`, `Textures`, `Materials`, `Models`).
- `Audio/` - `Music` and `SFX`.
- `Prefabs/` - reusable GameObjects; network prefabs go in `Prefabs/Network`.
- `Scenes/` - playable/editor scenes grouped by feature.
- `ScriptableObjects/` - data assets and configs.
- `Scripts/` - runtime C# by domain; editor-only scripts under an `Editor` folder, currently `Scripts/Editor`.
- `Settings/` - project-owned render/input/game settings assets (`Input/`, `Rendering/`).
- `UI/` - UI-specific fonts, icons, sprites, prefabs as needed.
- `VFX/` - particles, VFX graph assets, and effect-specific shaders.
- `Tests/` - EditMode and PlayMode test assets.

## Migrated project template assets

The Unity template/sample assets that are project-owned have also been moved under this root:

- Default scene: `Assets/ProjectRaidAuthority/Scenes/SampleScene.unity`
- Input actions: `Assets/ProjectRaidAuthority/Settings/Input/InputSystem_Actions.inputactions`
- URP/rendering settings: `Assets/ProjectRaidAuthority/Settings/Rendering`

## External package and plugin policy

- Prefer Unity Package Manager dependencies in root `Packages/manifest.json` and keep `Packages/packages-lock.json` committed for deterministic resolution.
- Keep imported vendor/plugin roots outside `Assets/ProjectRaidAuthority`. Current example: `Assets/Mirror` is third-party Mirror content and should remain vendor-owned.
- Use `Assets/ThirdParty/<VendorOrAssetName>` only for manually managed external assets that do not import/update safely into their own top-level vendor folder.
- Do not edit vendor/plugin source unless there is no practical alternative. Put adapters, wrappers, sample scenes, and game-specific integration code under `Assets/ProjectRaidAuthority`.
- If a plugin requires Unity-reserved folder names such as `Editor`, `Resources`, `StreamingAssets`, `Plugins`, `Gizmos`, or `Editor Default Resources`, follow Unity's required placement rules instead of forcing it into this folder tree.
- When moving assets in the filesystem, move the matching `.meta` file with the asset/folder so Unity GUID references stay intact.

## Current Mirror sample

The blog-style Mirror sample generated for this project lives in:

- Scenes: `Assets/ProjectRaidAuthority/Scenes/MirrorSample`
- Prefabs: `Assets/ProjectRaidAuthority/Prefabs/Network/MirrorSample`
- Runtime scripts: `Assets/ProjectRaidAuthority/Scripts/Network/MirrorSample`
- Editor builder/tests: `Assets/ProjectRaidAuthority/Scripts/Editor`

Mirror itself remains in `Assets/Mirror` as third-party/vendor code.
