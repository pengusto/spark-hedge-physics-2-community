using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeSource : MonoBehaviour
{
    [Header ("DEBUG SHAKE")]
    public float RepeatRate = 1;
    public float ShakeDuration = 1;
    public float ShakeAmplitude = 1;

    [Header("DEBUG TIME SCALE")]
    public bool ChangeTimeScale = true;
    public float SlowDownDuration = 1;
    public float SlowDownTime = 0.01f;
    public float SlowDownRestore = 10;
    public bool On = false;

    private void Start()
    {
        EnableShake();
    }

    private void OnEnable()
    {
        EnableShake();
    }

    void EnableShake()
    {
        if (On == false)
        {
            InvokeRepeating("FakeUpdate", 0.001f, RepeatRate);
            On = true;
        }
    }

    void FakeUpdate()
    {
        if (ChangeTimeScale) { CharacterCamera.SlowDown(SlowDownDuration, SlowDownTime, SlowDownRestore, transform.position); }
        CharacterCamera.ShakeCameraAddtive(ShakeDuration, ShakeAmplitude, transform.position);
    }

    private void OnDisable()
    {
        On = false;
        CancelInvoke();
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
