using UnityEngine;

public class InterludeMusicSettings : MonoBehaviour
{
    public MusicPlayer MusicScript;
    public bool ResetOnEnd = false;
    public static bool StopPlayer = false;

    private void Awake()
    {
        if (StopPlayer == true)
        {
            MusicScript.enabled = false;
        }

        // RESET VARIABLES ON END
        if (ResetOnEnd)
        {
            StopPlayer = false;
        }
    }
}
