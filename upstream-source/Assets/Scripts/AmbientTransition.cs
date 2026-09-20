using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class AmbientTransition : MonoBehaviour
{
    [Header ("References")]
    public PostProcessVolume Volume;
    public Light Sun;
    public float Speed = 1;
    AmbientSetting InitialSettings = new AmbientSetting();
    float t;
    public ReflectionProbe[] Probes;
    public Material Skybox;

    [Header("Mode")]
    public bool SingleTransition = false;
    public bool ChangeSkybox;
    bool SingleTransitionStarted = false;
    public float StartValue = -1;
    public AmbientSetting[] Ambients = new AmbientSetting[1];

    [Header("Extras")]
    public bool ChangeMats = false;
    public bool EnableEmissionAtPoint = false;
    public float EmissionThreshold = 0.8f;
    [ColorUsageAttribute(false, true)] public Color EmissionColorToSet = Color.white;
    bool done = false;
    public Material[] SharedMats;

    private void Start()
    {
        Skybox.SetFloat("_SkyboxSimpleBlend", 0);
        for (int i = 0; i < SharedMats.Length; i++)
        { SharedMats[i].SetColor("_EmissiveColor", Color.black); }
    }

    private void Update()
    {
        if (SingleTransition)
        {
            if (!SingleTransitionStarted)
            {
                SetCurrentSettings(StartValue); 
                SingleTransitionStarted = true;
                InvokeRepeating("UpdateReflectionProbe", 0.01f, 0.9f);
            }
            else 
            { 
                TransitionToAtmosphere(Ambients[0]); 
            }
        }
    }

    private void OnDestroy()
    {
        Speed = 99999999999999;
        TransitionToAtmosphere(InitialSettings);
    }


    void SetCurrentSettings(float startValue)
    {
        t = startValue;
        SkyboxBlendingFogPPSSettings fog;
        Volume.profile.TryGetSettings<SkyboxBlendingFogPPSSettings>(out fog);
        InitialSettings.AtmosphereColor =  fog._FogTint.value;
        InitialSettings.AtmosphereSmoothness = fog._FogSoftness.value;
        InitialSettings.AmbientTrilightTop = RenderSettings.ambientSkyColor;
        InitialSettings.AmbientTrilightMiddle = RenderSettings.ambientEquatorColor;
        InitialSettings.AmbientTrilightBottom = RenderSettings.ambientGroundColor;
        InitialSettings.SunColor = Sun.color;
        InitialSettings.SunIntensity = Sun.intensity;
    }

    void TransitionToAtmosphere(AmbientSetting Ao)
    {
        // EFFECTS
        SkyboxBlendingFogPPSSettings fog;
        Volume.profile.TryGetSettings<SkyboxBlendingFogPPSSettings>(out fog);
        fog._FogTint.value =                    Color.Lerp(InitialSettings.AtmosphereColor, Ao.AtmosphereColor, t);
        RenderSettings.ambientSkyColor =        Color.Lerp(InitialSettings.AmbientTrilightTop,      Ao.AmbientTrilightTop, t);
        RenderSettings.ambientEquatorColor =    Color.Lerp(InitialSettings.AmbientTrilightMiddle,   Ao.AmbientTrilightMiddle, t);
        RenderSettings.ambientGroundColor =     Color.Lerp(InitialSettings.AmbientTrilightBottom,   Ao.AmbientTrilightBottom, t);
        Sun.color =          Color.Lerp(InitialSettings.SunColor, Ao.SunColor, t);
        Sun.intensity =      Mathf.Lerp(InitialSettings.SunIntensity, Ao.SunIntensity, t);

        // MIDDLE
        if (ChangeSkybox) { Skybox.SetFloat("_SkyboxSimpleBlend", t); }
        if (ChangeMats)
        {
            if (EnableEmissionAtPoint)
            {
                if (!done)
                {
                    if (t > EmissionThreshold) 
                    {
                        for (int i = 0; i < SharedMats.Length; i++)
                        {
                            SharedMats[i].SetColor("_EmissiveColor", EmissionColorToSet);
                            done = true;
                        }
                    }
                }
            }
        }
        
        // END
        if (t > 1.0f) 
        {
            t = 0;
            SingleTransition = false;
            CancelInvoke("UpdateReflectionProbe");
        }

        // COUNTER
        t += Time.deltaTime * Speed;
    }

    void UpdateReflectionProbe()
    {
        for (int i = 0; i < Probes.Length; i++)
        {
            Probes[i].RenderProbe();
        }
    }
    
    [System.Serializable]
    public class AmbientSetting
    {
        public Color AmbientTrilightTop = Color.gray;
        public Color AmbientTrilightMiddle = Color.gray;
        public Color AmbientTrilightBottom = Color.gray;
        public Color SunColor = Color.white;
        public float SunIntensity = 1;
        public Color AtmosphereColor = Color.gray;
        public float AtmosphereSmoothness = 0.1f;
    }

}
