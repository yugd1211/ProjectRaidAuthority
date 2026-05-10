# ThirdParty

Optional holding area for manually managed third-party assets that do not install cleanly through Unity Package Manager and do not maintain their own top-level vendor folder.

Current policy:

- UPM packages: manage in root `Packages/manifest.json` / `Packages/packages-lock.json`.
- Asset Store or vendor imports with their own folder, such as `Assets/Mirror`: leave at their vendor path for safer updates.
- Project-specific wrappers/integration code: place under `Assets/ProjectRaidAuthority`, not here.
- Avoid editing vendor source directly; document any unavoidable vendor patch.
