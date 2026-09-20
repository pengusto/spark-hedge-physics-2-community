# Codex project rules

This repository is a community mirror and a small Unity playground around Spark / Hedge Physics 2. Keep the upstream snapshot and our additions visibly separate.

## Editing

- Work on reusable code in `Assets/CommunityCore/`.
- Treat `upstream-source/` as a read-only reference snapshot. Do not silently rewrite it.
- Keep the runtime demo free of Spark the Electric Jester character, level, audio, and texture assets.
- Before adding any third-party asset or package, record its source and license in `THIRD_PARTY_NOTICES.md`.
- Preserve the upstream credit phrase in product-facing examples: `Powered by Spark Physics 2` or `Powered by Hedge Physics 2`.

## Unity

- The active `main` project uses Unity `6000.5.6f1`. The source archive names Unity `6000.3.16f1`; that setting remains available on `legacy/unity-6000.3.16f1`.
- Open the repository root as a Unity project and run `Assets/CommunityDemo.unity`.
- A local Unity license is required for editor import and batch verification.

## Legal boundary

Do not call this repository official or imply endorsement. Do not copy excluded Spark 3 or unreviewed third-party assets back into `Assets/`. Update `ASSET_REVIEW.md` and `THIRD_PARTY_NOTICES.md` before publishing such a change.
