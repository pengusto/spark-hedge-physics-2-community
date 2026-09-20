using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateMeshIfClose : MonoBehaviour
{
    public float MinDistance = 1;
    public MeshRenderer[] Meshes;
    public SkinnedMeshRenderer[] SkinnedMeshes;
    float Distance;

    void Update()
    {
        if(CharacterCamera.Main != null)
        {
            Distance = Vector3.Distance(transform.position, CharacterCamera.Main.transform.position);
            if(Distance < MinDistance)
            {
                ChangeMeshes(false);
                ChangeSkinnedMeshes(false);
            }
            else
            {
                ChangeMeshes(true);
                ChangeSkinnedMeshes(true);
            }
        }
    }

    void ChangeMeshes(bool on)
    {
        if(Meshes.Length > 0)
        {
            if(Meshes[0].enabled != on)
            {
                for (int i = 0; i < Meshes.Length; i++)
                {
                    Meshes[i].enabled = on;
                }
            }
        }
    }

    void ChangeSkinnedMeshes(bool on)
    {
        if (SkinnedMeshes.Length > 0)
        {
            if (SkinnedMeshes[0].enabled != on)
            {
                for (int i = 0; i < SkinnedMeshes.Length; i++)
                {
                    SkinnedMeshes[i].enabled = on;
                }
            }
        }
    }
}
