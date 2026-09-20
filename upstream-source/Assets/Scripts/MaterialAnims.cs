using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAnims : MonoBehaviour
{
    public List<MeshRenderer> Mats;
    public enum Type { None, Beat };
    public Type AnimType;
    public int Index = 0;
    public float Speed = 1;
    public float Intensity = 1.5f;
    public float Min = 0.05f;

    public float f1 = 0;

    private void Update()
    {

        switch (AnimType)
        {
            case Type.None:
                break;
            case Type.Beat:
                f1 = Intensity * Mathf.Lerp(1, Min, ((Time.timeSinceLevelLoad * Speed) % 1));
                for (int i = 0; i < Mats.Count; i++)
                {
                    Mats[i].material.SetColor("_MainColor", f1 * Color.white);
                }
                break;
            default:
                break;
        }
    }
}
