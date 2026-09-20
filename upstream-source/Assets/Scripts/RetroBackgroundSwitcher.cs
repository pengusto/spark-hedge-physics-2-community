using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetroBackgroundSwitcher : MonoBehaviour
{
    public GameObject[] Backgrounds;

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "EffectBox")
        {
            if(col.TryGetComponent<ValuesHolder>(out ValuesHolder v))
            {
                Debug.Log("RetroCOL2: " + col.tag);
                for (int i = 0; i < Backgrounds.Length; i++) { Backgrounds[i].SetActive(false); }
                Backgrounds[(int)v.Floats[0]].SetActive(true);
            }
        }
    }
}
