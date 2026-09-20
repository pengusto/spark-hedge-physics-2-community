# Spark / Hedge Physics 2 Community

A community mirror and Codex-ready Unity playground for the physics framework LakeFeperd published as **Spark / Hedge Physics 2**.

LakeFeperd built a remarkably practical foundation for momentum-heavy platformers: a floating capsule controller, surface climbing, slope handling, loops, all-around gravity, rails, springs, and speed pads. The project grew out of the earlier **Hedge Physics** engine that helped power *Spark the Electric Jester 2* and *Spark the Electric Jester 3*. This repository keeps that lineage visible while giving the reusable physics a small, clean place to experiment.

This is an independent community mirror. It is not an official Feperd Games repository, and LakeFeperd did not review this packaging.

## What you can do here

- Open `Assets/CommunityDemo.unity` and press Play. The demo creates a plane, a capsule, a world gravity object, and the upstream `CharacterPhysics` component at runtime.
- Study the original script snapshot in `upstream-source/Assets/Scripts/`.
- Build a new character, level, or movement prototype without importing Spark character art, stages, audio, or the original game's presentation assets.
- Use Codex to extend the small demo first, then bring over individual upstream scripts after checking their dependencies.

The active demo includes a small WASD adapter using Unity's Input System. It is intentionally separate from the upstream physics code so the input layer can be replaced later. The upstream page says the original project used Unity's Input System after removing its former Rewired setup.

## Setup

1. Install Unity `6000.5.6f1`, the editor version used by the current `main` branch.
2. Add this repository's root folder in Unity Hub.
3. Open `Assets/CommunityDemo.unity`.
4. Press Play. Unity creates the demo objects at runtime.
5. Use **W/A/S/D** to move the capsule, then add your own input, mesh, camera, animation, and level assets under a license you control.

The original project may take a long time to import. This community project intentionally starts with a small source set so you can iterate before adding larger assets. The `legacy/unity-6000.3.16f1` branch preserves the original project-version setting from the source archive.

## Upstream and credit

- [Spark / Hedge Physics 2 by LakeFeperd](https://sites.google.com/view/spark-hedgephysics2)
- [Original Hedge Physics thread](https://forums.sonicretro.org/index.php?threads/1-0-final-hedge-physics-3d-sonic-engine-on-unity.36171/)
- [Spark the Electric Jester 3 on Steam](https://store.steampowered.com/app/1629530/Spark_the_Electric_Jester_3/)
- [Original source archive](https://drive.google.com/file/d/11xMFh20vq_XpMGhxrrCAjo_3djterNfp/view?usp=sharing)
- [Feperd Games Discord](https://discord.gg/xRYjexBagC)

If you ship a project based on the upstream material, keep the author's requested credit visible: **Powered by Spark Physics 2** or **Powered by Hedge Physics 2**. The original Hedge Physics post also credits LakeFeperd and Damizean.

## Repository boundary

`Assets/CommunityCore/` contains the small active playground and the physics components it uses. `upstream-source/` contains a reference snapshot from the linked archive. The original archive also contained generated files, Spark 3 carry-over material, and assets whose third-party rights were not independently verifiable. Those parts are documented and excluded from the active public Unity project.

Read [UPSTREAM-LICENSE.md](UPSTREAM-LICENSE.md), [ASSET_REVIEW.md](ASSET_REVIEW.md), and [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) before redistributing a build or adding assets.
