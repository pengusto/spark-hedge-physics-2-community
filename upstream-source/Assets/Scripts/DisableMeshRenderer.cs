using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMeshRenderer : MonoBehaviour
{
    public MeshRenderer Renderer;
    public MeshRenderer[] Renderers;

    void Start()
    {
        if (Renderer) { Renderer.enabled = false; }
        for (int i = 0; i < Renderers.Length; i++)
        {
            if(Renderers[i] != null) { Renderers[i].enabled = false; }
        }

        this.enabled = false;
    }

    
}
