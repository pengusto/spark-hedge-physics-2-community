using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMath : MonoBehaviour
{

    public LayerMask Mask;

    private void FixedUpdate()
    {
        Ray r = new Ray();
        RaycastHit h;
        r.origin = transform.position;
        r.direction = -transform.up;

        if(Physics.Raycast(r, out h, 5f, Mask))
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, h.normal) * transform.rotation;
        }
    }
}
