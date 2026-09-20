using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyanmicWater : MonoBehaviour
{
    public MeshFilter Filter;
    public MeshCollider Coll;
    public float verticaloffset = 1;
    public float wavefrequency = 200;
    public float wavestrenght = 20;
    public float wavespeed = 0.01f;
    public Texture2D Noise;
    public int Mip = 1;
    public float finalMultiplier = 1;
    public float Distance = 2000;
    public float UpdateRate = 0.0166666f;
    public TriggerSignaler Signal;

    // CACHE
    Vector3[] pos;
    Vector3[] normal;
    Vector4[] tangent;
    Color[] vertcol;
    Camera cam;
    Mesh m;
    float dist;
    float z;
    float timespeed;

    private void Start()
    {
        m = Filter.mesh;
        pos = m.vertices;
        normal = m.normals;
        tangent = m.tangents;
        vertcol = m.colors;
        cam = Camera.main;
        InvokeRepeating("FakeUpdate", 0.1f, UpdateRate);
    }

    void FakeUpdate()
    {
        dist = Vector3.Distance(transform.position, cam.transform.position);   
        if(dist <= Distance || Signal.Collision == true) { finalMultiplier += 0.0166666f; }
        else { finalMultiplier -= 0.0166666f; }
        finalMultiplier = Mathf.Clamp01(finalMultiplier);

        CPU_code();
    }

    void CPU_code()
    {
        if (finalMultiplier > 0.01f)
        {
            //Debug.Log("RUNNING WATER: " + gameObject.name);
            Vector3[] vertices = m.vertices;
            Vector3[] normals = m.normals;
            timespeed = Time.timeSinceLevelLoad * wavespeed;

            for (var i = 0; i < vertices.Length; i++)
            {
                float z =
                    Noise.GetPixelBilinear((pos[i].x * wavefrequency) + timespeed,
                    (pos[i].y * wavefrequency) + timespeed, Mip).r * wavestrenght;
                z -= verticaloffset;
                z *= vertcol[i].g;
                z *= finalMultiplier;
                vertices[i] = pos[i] + (normal[i] * z);
            }

            //m.RecalculateNormals();
            m.vertices = vertices;
            Coll.sharedMesh = m;
        }
    }

    //float z = (Mathf.Sin((pos[i].x * wavefrequency) + Time.timeSinceLevelLoad) * wavestrenght);
    //z += (Mathf.Sin((pos[i].y * wavefrequency) + (Time.timeSinceLevelLoad) + 200) * wavestrenght);
}
