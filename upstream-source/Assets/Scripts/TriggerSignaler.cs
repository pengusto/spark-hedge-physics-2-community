using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSignaler : MonoBehaviour
{
    public bool Collision;
    public bool CheckIfPlayer = true;

    private void Start()
    {
        Collision = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        coll(col, true);
    }

    private void OnTriggerExit(Collider col)
    {
        coll(col, false);
    }

    public void coll(Collider col, bool enter)
    {
        if (col.tag == "Player")
        {
            if (CheckIfPlayer) 
            {
                if (col.attachedRigidbody != null)
                {
                }
            }
            else 
            {
                Collision = enter;
            }
        }
    }
}
