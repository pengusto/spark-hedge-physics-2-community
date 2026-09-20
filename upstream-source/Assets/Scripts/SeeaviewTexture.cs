using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEditor;

//[ExecuteAlways]
public class SeeaviewTexture : MonoBehaviour
{
    public Camera ThisCam;
    public Camera MainCam;
    public PostProcessVolume PostVolume;
    public RenderTexture Tex;
    public static RenderTexture SeeaviewReference;
    public Material[] Materials;
    public RawImage UiImage;

    [Header("Misc")]
    public bool IsRenderingSkybox = true;
    public bool UseCustomScale = false;
    public Vector2 CustomScale = new Vector2(640, 480);
    public bool CustomTextureName = false;
    public string TextureName = "_AdvertPannel";

    //CACHE
    SkyboxBlendingFogPPSSettings PostFog;


    private void Update()
    {
        if (!MainCam)
        {
            MainCam = Camera.main;
        }

        transform.rotation = MainCam.transform.rotation;
        ThisCam.fieldOfView = MainCam.fieldOfView;

        if (IsRenderingSkybox)
        {
           PostVolume.profile.TryGetSettings<SkyboxBlendingFogPPSSettings>(out PostFog);
        }

    }

    void LateUpdate()
    {
        if (ThisCam.targetTexture != null) { ThisCam.targetTexture.Release(); }

        if (!UseCustomScale)
        {
            RenderTextureDescriptor d = Tex.descriptor;
            d.width = Screen.width;
            d.height = Screen.height;
            ThisCam.targetTexture = new RenderTexture(d);
            SeeaviewReference = ThisCam.targetTexture;
            //Tex = ThisCam.targetTexture;
        }
        else
        {
            ThisCam.targetTexture = new RenderTexture((int)CustomScale.x, (int)CustomScale.y, 6);
            SeeaviewReference = ThisCam.targetTexture;
            //Tex = ThisCam.targetTexture;
 
        }

        // SET TEXTURE
        if (IsRenderingSkybox) 
        {
           PostFog._SkyboxTexture.value = ThisCam.targetTexture;
        }
        for (int i = 0; i < Materials.Length; i++)
        {
            if (!CustomTextureName) { Materials[i].SetTexture("_Emission", Tex); }
            else { Materials[i].SetTexture(TextureName, Tex); }
        }
        if (UiImage) { UiImage.texture = Tex; }

    }
}
