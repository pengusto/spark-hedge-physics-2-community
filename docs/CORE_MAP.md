# Community core map

The active demo uses five upstream components plus one small community bootstrap:

- `CharacterPhysics.cs`: floating capsule collision, ground and side rays, slope handling, gravity, and moving-platform hooks.
- `World.cs`: shared world gravity and environment state.
- `SurfaceMaterialInfo.cs`: surface and platform metadata.
- `TrackObjectDelta.cs`: moving-platform delta tracking.
- `Gravity.cs`: optional rigidbody gravity helper.
- `CommunityDemoBootstrap.cs`: creates a plane, capsule, world, and camera without game-specific assets.

The remaining upstream scripts stay under `upstream-source/` for reference. They include combat, menus, networking, game progression, and Spark-specific components. Bring them into an active project one group at a time and check their package and asset dependencies first.
