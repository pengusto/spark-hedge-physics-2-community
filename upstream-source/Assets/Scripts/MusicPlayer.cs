using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public enum Type { OnStart, OnEnable }

    [Header("Audio Data")]
    public Type PlayType = Type.OnStart;
    public AudioClip Intro;
    public AudioClip Loop;

    [Header("Data")]
    public float FadeSpeed = 2;
    public float PlayInterval = 1;
    public float MaxVolume = 0.8f;
    public bool Force = true;
    public bool Debug;

    public void Start()
    {
        if(PlayType == Type.OnStart)
        {
            Music.Manager.PlayNextSong(Intro, Loop, FadeSpeed, PlayInterval, MaxVolume, Force);
        }
    }

    private void OnEnable()
    {
        if(PlayType == Type.OnEnable)
        {
            Music.Manager.PlayNextSong(Intro, Loop, FadeSpeed, PlayInterval, MaxVolume, Force);
        }
    }

    private void Update()
    {
        if (Debug)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Music.Manager.PlayNextSong(Intro, Loop, FadeSpeed, PlayInterval, MaxVolume, Force);
            }
        }
    }

}
