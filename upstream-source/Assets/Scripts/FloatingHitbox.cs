using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHitbox : MonoBehaviour
{

    [Header("Main")]
    public LayerMask Mask;
    public Transform BottonCollider;
    public float RayRadius;
    public float FloatingMaxDistance = 3;
    public float PushupForce = 1;

    [Header("Misc")]
    public float BottonColliderOffset = -0.01f;
    public float FloatingPositionOffset = 0.05f;

    // CACHE
    //public Rewired.Player Inp;
    RaycastHit hit = new RaycastHit();
    Ray r = new Ray();

    private void Start()
    {
        //Inp = Rewired.ReInput.players.GetPlayer(0);
    }

    void FixedUpdate()
    {
        r.direction = -transform.up;
        r.origin = transform.position;

        if(Physics.SphereCast(r, RayRadius, out hit, FloatingMaxDistance, Mask, QueryTriggerInteraction.Ignore))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point + (transform.up * (FloatingMaxDistance * FloatingPositionOffset)), Time.fixedDeltaTime * PushupForce);
            BottonCollider.position = hit.point + (transform.up * BottonColliderOffset);
        }
        else
        {
            BottonCollider.position = transform.position + (-transform.up * (FloatingMaxDistance - RayRadius));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * FloatingMaxDistance);
        Gizmos.DrawWireSphere(transform.position, RayRadius);
        Gizmos.DrawWireSphere(hit.point + (transform.up * RayRadius), RayRadius);

        Gizmos.DrawCube(hit.point + ( transform.up * (FloatingMaxDistance - FloatingPositionOffset)), Vector3.one * 0.05f);
    }
}
