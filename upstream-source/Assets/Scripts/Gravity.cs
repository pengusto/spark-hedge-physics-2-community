using UnityEngine;

public class Gravity : MonoBehaviour
{
    public Rigidbody rigid;
    public Vector3 GravityDir = -Vector3.up;

    private void FixedUpdate()
    {
        if (rigid)
        {
            rigid.AddForce(GravityDir * World.WorldInfo.WorldGravity, ForceMode.VelocityChange);
        }
    }
}
