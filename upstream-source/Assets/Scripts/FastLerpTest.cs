using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastLerpTest : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Obj;
    public Text txt;
    public float followSpeed = 10;
    public float objectSpeed = 100;

    Vector3 target;

    private void Start()
    {

        Obj.GetComponent<Rigidbody>().AddForce(Obj.transform.forward * objectSpeed, ForceMode.VelocityChange);

    }


    private void FixedUpdate()
    {
        target = Obj.transform.position + (Obj.transform.right * 5);
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, target, Time.fixedDeltaTime * followSpeed);
    }

}
