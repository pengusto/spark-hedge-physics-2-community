using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEvents : MonoBehaviour
{
    public AudioSource Source1;
    public AudioSource Source2;

    public void PlaySample(AudioClip clip)
    {
        if(Source1.isPlaying == false) { Source1.clip = clip; Source1.Play(); }
        else { Source2.clip = clip; Source2.Play(); }
    }
}
