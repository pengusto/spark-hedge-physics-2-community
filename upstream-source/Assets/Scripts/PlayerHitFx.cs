using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitFx : MonoBehaviour
{
    [Header("Parameters")]
    public AudioSource[] AudioSources;
    public AnimationCurve ComboCurve;

    [Header("Volume Control")]
    public AnimationCurve VolumeOnMultiple;
    public static int InstancesAmmount = 0;

    [Header("Secondary")]
    public bool UseSecondarySource = false;
    public AudioSource SecondarySource;
    public float SecondarySourceThreshold = 20f;

    // CACHE
    static float ComboCounter = 0;
    static float ComboAdd = 0.5f;
    public static float ComboDecay = 10f;
    public static float Combo;
    int i;

    private void Start()
    {
        // INSTANCES
        InstancesAmmount += 1;

        // SET SOURCES
        Combo += ComboAdd;
        ComboCounter = 0.75f;
        for (i = 0; i < AudioSources.Length; i++)
        {
            AudioSources[i].pitch = ComboCurve.Evaluate(Combo);
            AudioSources[i].volume *= VolumeOnMultiple.Evaluate(InstancesAmmount);
        }

        // SECONDARY SOUND
        if (UseSecondarySource)
        {
            if(Combo > SecondarySourceThreshold)
            {
                // PLAY OTHER SOUND AND STOP MAIN
                SecondarySource.Play();
                AudioSources[0].enabled = false;
            }
            else
            {

            }
        }
    }

    public static void ComboAudioManager()
    {
        ComboCounter -= Time.deltaTime;
        if (Combo > 0 && ComboCounter < 0) { Combo -= Time.deltaTime * ComboDecay; }
    }

    private void OnDestroy()
    {
        InstancesAmmount -= 1;
    }
}
