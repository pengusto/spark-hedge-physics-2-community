using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public bool Collided;
    public string[] tags;

    private void OnCollisionEnter(Collision col)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (col.gameObject.CompareTag(tags[i])) 
            { 
                Collided = true; 
                return; 
            }
        }

        Debug.Log("COL TEST: " + col.gameObject.tag);
    }

    private void OnCollisionExit(Collision col)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (col.gameObject.CompareTag(tags[i]))
            {
                Collided = false;
                return;
            }
        }
    }

    // ======

    private void OnTriggerEnter(Collider col)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (col.gameObject.CompareTag(tags[i]))
            {
                Collided = true;
                return;
            }
        }

        Debug.Log("COL TEST: " + col.gameObject.tag);
    }

    private void OnTriggerExit(Collider col)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (col.gameObject.CompareTag(tags[i]))
            {
                Collided = false;
                return;
            }
        }
    }
}
