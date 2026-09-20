using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTest : MonoBehaviour
{

    public Transform obj;
    public Vector3 initialpos;
    public float Speed = 10;

    void Start()
    {
        initialpos = transform.position;
    }


    void Update()
    {


        //Vector3 pos1 = initialpos + (transform.forward * 10);
        //Vector3 pos2 = initialpos - (transform.forward * 10);

        transform.position = Vector3.Lerp(transform.position, obj.position, Time.deltaTime * Speed);

    }
}
