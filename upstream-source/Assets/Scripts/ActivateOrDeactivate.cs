using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOrDeactivate : MonoBehaviour
{
    public List<GameObject> ToActivate;
    public List<GameObject> ToDeactivate;

    void Start()
    {
        for (int i = 0; i < ToActivate.Count; i++) { ToActivate[i].SetActive(true); }
        for (int i = 0; i < ToDeactivate.Count; i++) { ToDeactivate[i].SetActive(false); }
    }

    public void DeactivateSelfEvent()
    {
        gameObject.SetActive(false);
    }
}
