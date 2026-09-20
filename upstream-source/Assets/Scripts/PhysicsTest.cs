using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTest : MonoBehaviour
{
    public Rigidbody rigid;
    public float Speed = 10;
    bool init = false;

    public bool TransformInUpdate;

    private void Update()
    {
        if (TransformInUpdate)
        {
            transform.Translate(transform.forward * Speed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!init && !TransformInUpdate)
        {
            rigid.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
            init = true;
        }
    }
}
