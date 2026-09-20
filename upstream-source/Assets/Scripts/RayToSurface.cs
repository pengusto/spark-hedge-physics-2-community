using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayToSurface : MonoBehaviour
{
    public Transform Obj;
    public float MaxDistance = 200;
    public LayerMask Mask;
    RaycastHit h = new RaycastHit();
    public float UpdateRate = 0.06666666666f;

    private void Start()
    {
        InvokeRepeating("FakeUpdate", 0.02f + Random.Range(0f,1f), UpdateRate);
    }

    void FakeUpdate()
    {
        if(Physics.Raycast(transform.position, -transform.up, out h, MaxDistance, Mask, QueryTriggerInteraction.Ignore))
        {
            Obj.position = h.point;
            Obj.rotation = Quaternion.AngleAxis(transform.eulerAngles.y + 180, h.normal);
        }
    }
}
