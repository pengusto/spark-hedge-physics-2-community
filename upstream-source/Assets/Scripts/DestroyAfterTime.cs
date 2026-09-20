using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float DestroyTime = 1;

    private void Start()
    {
        Invoke("DestroyAfter", DestroyTime);
    }

    void DestroyAfter()
    {
        CancelInvoke();
        Destroy(gameObject);
    }
}
