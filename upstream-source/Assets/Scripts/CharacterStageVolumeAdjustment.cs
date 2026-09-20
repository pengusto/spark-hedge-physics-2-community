using UnityEngine;

public class CharacterStageVolumeAdjustment : MonoBehaviour
{
    public OptionsMenu Options;
    public UnityEngine.Audio.AudioMixer Mixer;
    public float VolumeAdd = 3;
    //public float OriginalVolume;
    public bool DontReset = false;

    private void Start()
    {
        if (Options != null)
        {
            Options.LoadSettings();
            Options.SetSettings();
        }

        //Mixer.GetFloat("MainVolume", out OriginalVolume);
        Mixer.SetFloat("MainVolume", VolumeAdd);
    }

    private void OnDestroy()
    {
        if (DontReset == false)
        {
            Mixer.SetFloat("MainVolume", 0);
        }
    }
}
