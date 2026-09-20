using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationEvents : MonoBehaviour
{
    [Header("Audio Stuff")]
    public AudioSource Source1;
    public AudioSource Source2;

    [Header("Object Stuff")]
    public GameObject[] Objects;
    public GameObject[] ObjectsIHB;

    public void PlayAudioClip_1(AudioClip clip)
    {
        Debug.Log("Played Audio 1");
        Source1.clip = clip;
        Source1.Play();
    }

    public void PlayAudioClip_2(AudioClip clip)
    {
        Source2.clip = clip;
        Source2.Play();
    }

    public void ActivateGameobject(int index)
    {
        Objects[index].SetActive(true);
    }

    public void DeactivateGameobject(int index)
    {
        Objects[index].SetActive(false);
    }

    public void PlayIHB(int index)
    {
        IHB.StartIHB(ObjectsIHB[index]);
    }

    public void StopIHB(int index)
    {
        IHB.StopIHB(ObjectsIHB[index]);
    }

    // INSTANTIATE OR DESTROY

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void InstantiateObject(int index)
    {
        GameObject g = Objects[index];
        Instantiate(g, transform.position, Quaternion.identity);
    }
}
