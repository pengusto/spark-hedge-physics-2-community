using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform TeleportPosition;
    public float OutForce = 50;

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            col.attachedRigidbody.transform.position = TeleportPosition.position;
            col.attachedRigidbody.transform.rotation = TeleportPosition.rotation;
            col.attachedRigidbody.linearVelocity = TeleportPosition.forward * OutForce;
        }
    }
}
