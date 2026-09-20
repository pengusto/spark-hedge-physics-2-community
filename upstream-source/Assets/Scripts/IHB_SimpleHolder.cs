using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IHB_SimpleHolder : MonoBehaviour
{
    public GameObject[] Boxes;

    public IEnumerator TriggerIHB(float time, int id)
    {
        yield return new WaitForSeconds(time);
        IHB.StartIHB(Boxes[id]);
    }
}
