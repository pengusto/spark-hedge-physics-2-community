using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObjectDelta : MonoBehaviour
{
    public bool EnableDebug = false;
    public Vector3 Delta;
    Vector3 delta;

    private void Start()
    {
        Delta = transform.position;
        delta = transform.position;
    }

    private void FixedUpdate()
    {
        Delta = (transform.position - delta) / Time.fixedDeltaTime;
        delta = transform.position;
        if (EnableDebug) { Debug.DrawRay(transform.position, Delta, Color.green); }
    }
}
