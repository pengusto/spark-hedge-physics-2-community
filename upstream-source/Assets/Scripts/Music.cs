using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public static Music Manager;

    [Header("Sources")]
    public AudioSource intro_a;
    public AudioSource loop_a;

    // CACHE
    bool fade;
    float FadeSpeed = 0;
    AudioClip next_intro_a;
    AudioClip next_loop_b;
    Coroutine NewSongRoutine;

    
    private void Start()
    {
        if (Manager == null)
        {
            Manager = this;
            DontDestroyOnLoad(this);
        }
    }

    void Update()
    {
        if (fade) 
        {
            intro_a.volume  -= Time.unscaledDeltaTime * FadeSpeed;
            loop_a.volume   -= Time.unscaledDeltaTime * FadeSpeed;
        }
    }

    public void PlayNextSong(AudioClip intro, AudioClip loop, float fadeSpeed, float timeToPlay, float maxVolume, bool forcePlay)
    {
        Debug.Log("Playing: " + intro.name);

        if (loop_a.clip != null && !forcePlay) // STOP IF SAME SONG
        {
            if (loop_a.clip.name == loop.name) 
            { 
                Debug.Log("Same song"); 
                return; 
            } 
        }

        if (NewSongRoutine != null) { StopCoroutine(NewSongRoutine); }
        NewSongRoutine = StartCoroutine(NewSong(intro, loop, fadeSpeed, timeToPlay, maxVolume));
    }

    public void PlayNextSongSingle(AudioClip song, float fadeSpeed, float timeToPlay, float maxVolume)
    {
        Debug.Log("Playing: " + song.name);

        if (NewSongRoutine != null) { StopCoroutine(NewSongRoutine); }
        NewSongRoutine = StartCoroutine(NewSong(song, null, fadeSpeed, timeToPlay, maxVolume));
    }

    public void StopMusic(float fadeSpeed)
    {
        Debug.Log("Music Stopped!");
        if (NewSongRoutine != null) { StopCoroutine(NewSongRoutine); }
        NewSongRoutine = StartCoroutine(NewSong(null, null, fadeSpeed, 1, 0.5f));
    }

    IEnumerator NewSong(AudioClip intro, AudioClip loop, float fadeSpeed, float timeToPlay, float maxVolume) 
    {
        if (intro) { intro.LoadAudioData(); }
        if (loop) { intro.LoadAudioData(); }
        fade = true;
        FadeSpeed = fadeSpeed;

        yield return new WaitForSeconds(timeToPlay);
        if(loop != null) // WITH LOOP
        {
            intro_a.Stop();
            loop_a.Stop();
            intro_a.clip = intro;
            loop_a.clip = loop;
            intro_a.PlayDelayed(0.1f);
            loop_a.PlayDelayed(0.1f + intro.length);
            intro_a.volume = maxVolume;
            loop_a.volume = maxVolume;
        }
        else
        {
            intro_a.Stop();
            loop_a.Stop();
            loop_a.clip = null;
            //loop_a.PlayDelayed(0.1f);
            intro_a.volume = maxVolume;
            loop_a.volume = maxVolume;
            intro_a.clip = intro;
            intro_a.PlayDelayed(0.1f);
        }
        fade = false;
    }
}
