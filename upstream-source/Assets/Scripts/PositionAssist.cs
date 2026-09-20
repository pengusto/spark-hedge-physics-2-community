using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAssist : MonoBehaviour
{

    public float DistanceCheck = 0.05f;

    private void OnDrawGizmosSelected()
    {
        RaycastHit h;
        if(Physics.Raycast(new Ray(transform.position, -transform.up), out h))
        {
            Gizmos.color = Color.Lerp(Color.red, Color.white, 0.2f);
            Gizmos.DrawRay(transform.position, -transform.up * h.distance);
            Gizmos.DrawWireSphere(h.point, 0.025f);
        }

        Gizmos.color = Color.Lerp(Color.red, Color.white, 0.1f);
        Gizmos.DrawWireSphere(transform.position, 0.05f);
        Gizmos.color = Color.Lerp(Color.blue, Color.white, 0.1f);
        Gizmos.DrawRay(transform.position, transform.forward);

        Gizmos.color = Color.Lerp(Color.green, Color.white, 0.1f);
        Gizmos.DrawWireSphere(transform.position, DistanceCheck);
    }
}
