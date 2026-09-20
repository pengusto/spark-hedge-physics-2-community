using UnityEngine;

public class SpecialDaySettings : MonoBehaviour
{
    [Header("Alternate Music")]
    public bool AlternateMusic = false;
    public MusicPlayer MusicScript;
    public AudioClip NewIntro;
    public AudioClip NewLoop;

    [Header("Ghost Day")]
    public bool IsGhostDay = false;

    [Header("Misc")]
    public bool ForceMusicToPlay = false;

    public void ExecuteEvents()
    {
        MapEffects.GhostDay = false;
        StopMusicAtWorldMap.AltSong = false;

        if (AlternateMusic && MusicScript)
        {
            MusicScript.Intro = NewIntro;
            MusicScript.Loop = NewLoop;
            StopMusicAtWorldMap.AltSong = true;
        }

        if (IsGhostDay)
        {
            MapEffects.GhostDay = true;
        }

        if (ForceMusicToPlay)
        {
            WorldMapMusicPlayerSpecialSettings.StopPlayer = false;
            MusicScript.gameObject.SetActive(true);
            MusicScript.enabled = true;
            MusicScript.Start();
        }
    }


}
