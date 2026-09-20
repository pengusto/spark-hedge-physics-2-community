using UnityEngine;

public class StopMusicAtDayChange : MonoBehaviour
{
    void Start()
    {
        WorldMapMusicPlayerSpecialSettings.StopPlayer = true;
    }
}
