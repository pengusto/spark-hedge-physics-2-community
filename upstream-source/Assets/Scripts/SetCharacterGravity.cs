using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCharacterGravity : MonoBehaviour
{
    CharacterPhysics Char;
    public bool UseCentralPoint = false;
    public float Multiplier = 1;
    public Transform CentralPoint;

    // CACHE
    Vector3 dir;

    private void OnTriggerStay(Collider col)
    {
        if (col.attachedRigidbody.CompareTag("Player"))
        {
            if(col.attachedRigidbody.TryGetComponent<CharacterPhysics>(out Char))
            {
                if (UseCentralPoint) 
                {
                    dir = (CentralPoint.position - Char.transform.position).normalized;
                    Char.GravityDir = dir * Multiplier;
                }
                else
                {
                    Char.GravityDir = transform.up * Multiplier;
                }      
            }
        }
    }
}
