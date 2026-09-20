using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGrapSetPos : MonoBehaviour
{
    public Transform Pos;
    public Vector3 offset = new Vector3(0,0,0);

    void Update()
    {
        transform.position = Pos.position + offset;
    }
}
