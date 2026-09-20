using UnityEngine;

public class StopMusicAtWorldMap : MonoBehaviour
{
    public MusicPlayer SongOBJ;
    public static bool Stop = false;
    public static bool AltSong = false;

    void Awake()
    {
        if (SongOBJ && Stop)
        {
            SongOBJ.enabled = false;
            SongOBJ.StopAllCoroutines();
            Stop = false;
        }

        if (AltSong)
        {
            SongOBJ.enabled = false;
            SongOBJ.StopAllCoroutines();
            SongOBJ.gameObject.SetActive(false);
        }
    }

}
