# Asset review boundary

This repository separates reusable physics code from the larger source archive.

| Location | Treatment | Reason |
| --- | --- | --- |
| `Assets/CommunityCore/` | Active public Unity project | Small demo plus the upstream components used by it. No Spark character art, game audio, or bundled third-party art. |
| `upstream-source/Assets/Scripts/` | Reference snapshot | Original C# snapshot from the linked archive. It contains game-specific scripts and external assumptions, so Unity does not compile it automatically. |
| Spark character, stage, audio, `Spark3CarryOver`, base-asset, and package folders | Excluded | These areas are not needed for the core demo and can contain game-specific or third-party material. The source archive's warning asks re-uploaders to check this boundary. |

The source archive checksum used for this review was:

`1fad5c0feaf91081bf4b64c074fbd5b54ca7f56644419b5d5b543688cb6f957a`

If a rights holder identifies a problem, remove the affected path from the public mirror first and record the change here.
