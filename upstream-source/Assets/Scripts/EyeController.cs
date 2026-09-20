using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    public Vector2 Multiplier = new Vector2(1, 1);
    public Vector2 Additive = new Vector2(0, 0);
    public Transform Bone1;
    public Transform Bone2;
    public SkinnedMeshRenderer EyeMat;

    Vector4 og;

    void Update()
    {
        og = EyeMat.material.GetVector("_TilingAndOffset");
        og.z = (Bone2.localPosition.y + Additive.y) * Multiplier.x;
        og.w = (Bone2.localPosition.x + Additive.x) * Multiplier.y;
        EyeMat.material.SetVector("_TilingAndOffset", og);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(Bone1.transform.position, 0.001f);
    //    Gizmos.DrawLine(Bone1.transform.position, Bone2.transform.position);
    //}
}
