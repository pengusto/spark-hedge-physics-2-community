# Local asset review

The public project keeps the active demo and reusable physics code small. Local
review material can live under `Assets/LocalReview/`; that directory is ignored
by Git and must never be committed or pushed.

To inspect the original art locally:

1. Download the source archive from the official project page.
2. Copy reviewed art folders into `Assets/LocalReview/Original/`.
3. Let Unity import them, then open the asset or prefab you want to inspect.
4. Keep original scripts, editor code, plugins, and unknown third-party assets
   outside the active project unless their dependencies and rights have been
   checked.

The local review copy is useful for evaluating meshes, materials, characters,
stages, and presentation. It is not part of the public community mirror and
does not change the license or redistribution status of those files.

The active demo remains `Assets/CommunityDemo.unity`. Press **Play** and use
**W/A/S/D** to move the capsule through the physics test. The input adapter is
demo-only and can be replaced when the project gets a real character/input
layer.
