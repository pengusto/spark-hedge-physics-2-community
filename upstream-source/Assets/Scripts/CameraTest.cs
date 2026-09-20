using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public Rigidbody Target;

    public bool smoothdamp;
    public bool fixedfollow;

    public bool ViaUpdate = false;
    public bool ViaLateUpdate = false;
    public bool ViaFixedUpdate = false;

    public float followSpeed = 10;
    public float fixedfollowspeed = 0.1f;

    public Vector3 Speed;

    // Update is called once per frame
    void Update()
    {
        Time.maximumDeltaTime = 99;

        if (ViaUpdate)
        {
            Follow(Time.deltaTime, fixedfollow);
        }
    }

    private void LateUpdate()
    {
        if (ViaLateUpdate)
        {
            Follow(Time.deltaTime, fixedfollow);
        }
    }

    private void FixedUpdate()
    {
        if (ViaFixedUpdate)
        {
            Follow(Time.fixedDeltaTime, fixedfollow);
        }
    }

    void Follow(float deltatime, bool fixedvalue)
    {
        if (fixedvalue) { deltatime = fixedfollowspeed; }
        Vector3 p = Target.transform.position + (Target.transform.right * 5);
        if (!smoothdamp) { transform.position = Vector3.Lerp(transform.position, p, deltatime * followSpeed); }
        else { transform.position = Vector3.SmoothDamp(transform.position, p, ref Speed, deltatime * followSpeed); }
    }
}
