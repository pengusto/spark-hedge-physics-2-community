using UnityEngine;

public class WorldMapMusicPlayerSpecialSettings : MonoBehaviour
{
    public MusicPlayer MusicScript;
    public static bool StopPlayer = false;

    private void Awake()
    {
        if(StopPlayer == true)
        {
            MusicScript.enabled = false;
        }

        StopPlayer = false;
    }
}
