using UnityEngine;

public class LaserRay : MonoBehaviour
{
    [Header("References")]
    public Transform Laser;
    public Transform EndSpot;

    [Header("Parameters")]
    public float MaxDistance;
    public LayerMask Mask;

    [Header("Cache")]
    public Vector3 scale;
    RaycastHit hit;

    void FixedUpdate()
    {

        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance, Mask, QueryTriggerInteraction.Ignore))
        {
            scale = Vector3.one;
            scale.z = hit.distance;
            EndSpot.transform.position = hit.point;
        }
        else
        {
            scale = Vector3.one;
            scale.z = MaxDistance;
            EndSpot.transform.localPosition = new Vector3(0,0, MaxDistance);
        }

        Laser.localScale = scale;
    }
}
