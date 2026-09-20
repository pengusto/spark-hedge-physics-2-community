using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class GraphicsEditor : MonoBehaviour
{
    public GraphicsMenu MenuObj;
    public Volume MainVolume;
    public Camera MainCam;
    public UniversalRendererData RenderData;
    public UniversalRenderPipelineAsset RenderAsset;
    public static GraphicsSettings CurrentSettings;
    public static Resolution[] Resolutions =
    {
            // 16:9
            new Resolution(new Vector2(1, 1), "??:?", "CUSTOM"),
            new Resolution(new Vector2(1280, 720), "16:9", "720p - HD"),
            //new Resolution(new Vector2(1366, 768), "16:9", "WXGA"),
            //new Resolution(new Vector2(1600, 900), "16:9", "HD+"),
            new Resolution(new Vector2(1920, 1080), "16:9", "1080p - Full HD"),
            new Resolution(new Vector2(2560, 1440), "16:9", "1440p"),
            //new Resolution(new Vector2(3200, 1800), "16:9", "QHD+"),
            new Resolution(new Vector2(3840, 2160), "16:9", "2160p - 4k"),
            //new Resolution(new Vector2(5120, 2880), "16:9", "5k"),
            new Resolution(new Vector2(7680, 4320), "16:9", "8k"),
            //new Resolution(new Vector2(15360, 8640), "16:9", "16k"),
            //// 16:10
            //new Resolution(new Vector2(1280, 800), "16:10", "800p"),
            //new Resolution(new Vector2(1440, 900), "16:10", "900p"),
            //new Resolution(new Vector2(1680, 1050), "16:10", "1050p"),
            //new Resolution(new Vector2(1920, 1200), "16:10", "1200p"),
            //new Resolution(new Vector2(2560, 1600), "16:10", "1600p"),
            //new Resolution(new Vector2(3840, 2400), "16:10", "2400p"),
            // 21:9
            //new Resolution(new Vector2(1680, 720), "21:9", "720p"),
            new Resolution(new Vector2(2560, 1080), "21:9", "1080p"),
            new Resolution(new Vector2(3440, 1440), "21:9", "1440p"),
            new Resolution(new Vector2(7680, 2160), "21:9", "4k"),
            new Resolution(new Vector2(10240, 4320), "21:9", "8k"),
            // 4:3
            new Resolution(new Vector2(960, 720), "4:3", "720p"),
            //new Resolution(new Vector2(1024, 768), "4:3", "768p"),
            //new Resolution(new Vector2(1152, 864), "4:3", "864p"),
            new Resolution(new Vector2(1280, 960), "4:3", "960p"),
            //new Resolution(new Vector2(1400, 1050), "4:3", "1050p"),
            new Resolution(new Vector2(1440, 1080), "4:3", "1080p"),
            //new Resolution(new Vector2(1600, 1200), "4:3", "1200p"),
            //new Resolution(new Vector2(1856, 1392), "4:3", "1392p"),
            //new Resolution(new Vector2(1920, 1440), "4:3", "1440p"),
            //new Resolution(new Vector2(2048, 1536), "4:3", "1536p"),
            //new Resolution(new Vector2(2560, 1920), "4:3", "1536p")
    };
    bool VolumeSet = false;
    float timer = 0;

    private void Start()
    {
        // INITIATE VOLUME
        CurrentSettings = new GraphicsSettings();
        CurrentSettings = LoadGraphicsSettings();
        ApplyGraphicsLocal();
        //InvokeRepeating("FakeUpdate", UnityEngine.Random.Range(1f,2f), 1);
    }

    private void OnDestroy()
    {
        SaveGraphicsSettings(CurrentSettings);
    }

    void FakeUpdate()
    {

    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    // FUNCTIONS
    public void ApplyGraphicsLocal()
    {
        ApplyGraphics(CurrentSettings, MainVolume);
        SaveGraphicsSettings(CurrentSettings);
    }

    public void ApplyGraphics(GraphicsSettings graphics, Volume volume)
    {
        Debug.Log("Applying GRAPHICS!");

        // RESOLUTION
        if (graphics.CurrentResolution.resolution.x >= 20)
        {
            if (graphics.FullScreen == FullScreenMode.Windowed)
            {
                Screen.SetResolution((int)(graphics.CurrentResolution.resolution.x),
                (int)(graphics.CurrentResolution.resolution.y), graphics.FullScreen);
            }
            else
            {
                Screen.SetResolution((int)(graphics.CurrentResolution.resolution.x),
                (int)(graphics.CurrentResolution.resolution.y), graphics.FullScreen);
            }
        }
        else
        {
            Screen.SetResolution((int)(Display.main.systemWidth),
            (int)(Display.main.systemHeight), graphics.FullScreen);
        }

        // WINDOWED
        Screen.fullScreenMode = graphics.FullScreen;

        // RENDER SCALE
        RenderAsset.renderScale = graphics.ResolutionScale;

        // UPSCALING
        if (graphics.UpScalingType == 0) { RenderAsset.upscalingFilter = UpscalingFilterSelection.Linear; }
        else if (graphics.UpScalingType == 1) { RenderAsset.upscalingFilter = UpscalingFilterSelection.FSR; }

        // FRAMERATE
        Application.targetFrameRate = graphics.FPS_Target;

        // VSYNC
        if(graphics.Vsync == 0) { QualitySettings.vSyncCount = 0; }
        else { QualitySettings.vSyncCount = 1; }

        // TEXTURES
        QualitySettings.globalTextureMipmapLimit = graphics.TextureQuality;
        QualitySettings.anisotropicFiltering = graphics.AnisoQuality;

        // TEXTURE STREAMING
        if (graphics.TextureStreaming == 0)
        {
            QualitySettings.streamingMipmapsActive = true;
            QualitySettings.streamingMipmapsMemoryBudget = SystemInfo.graphicsMemorySize * 0.25f;
        }
        else if(graphics.TextureStreaming == 1)
        {
            QualitySettings.streamingMipmapsActive = true;
            QualitySettings.streamingMipmapsMemoryBudget = SystemInfo.graphicsMemorySize * 0.4f;
        }
        else if (graphics.TextureStreaming == 2)
        {
            QualitySettings.streamingMipmapsActive = true;
            QualitySettings.streamingMipmapsMemoryBudget = SystemInfo.graphicsMemorySize * 0.7f;
        }
        else if (graphics.TextureStreaming == 3)
        {
            QualitySettings.streamingMipmapsActive = false;
        }
        else
        {
            QualitySettings.streamingMipmapsActive = false;
        }

        // SHADOWS
        if(graphics.ShadowRes == 0) { RenderAsset.mainLightShadowmapResolution = 512; }
        else if (graphics.ShadowRes == 1) { RenderAsset.mainLightShadowmapResolution = 1024; }
        else if (graphics.ShadowRes == 2) { RenderAsset.mainLightShadowmapResolution = 2048; }
        else if (graphics.ShadowRes == 3) { RenderAsset.mainLightShadowmapResolution = 4096; }
        else if (graphics.ShadowRes == 4) { RenderAsset.mainLightShadowmapResolution = 8192; }
        CurrentSettings.ShadowRes = Mathf.Clamp(CurrentSettings.ShadowRes, 0, 5);
        QualitySettings.lodBias = graphics.LOD_Bias;

        // AO
        ScreenSpaceAmbientOcclusion AO;
        RenderData.TryGetRendererFeature<ScreenSpaceAmbientOcclusion>(out AO);
        if (AO != null)
        {
            if (graphics.AO_Quality == 0)
            {
                AO.SetActive(false);
            }
            else if (graphics.AO_Quality == 1)
            {
                AO.SetActive(true);
            }
        }

        // SSR
        //ShinySSRR.ShinyScreenSpaceRaytracedReflections SSR; 
        //ShinySSRR.ShinySSRR SSR2;
        //RenderData.TryGetRendererFeature<ShinySSRR.ShinySSRR>(out SSR2);
        //if (MainVolume.profile.TryGet<ShinySSRR.ShinyScreenSpaceRaytracedReflections>(out SSR))
        //{
        //    if (graphics.SSR_Quality == 0) // OFF
        //    {
        //        SSR2.SetActive(false);
        //    }
        //    else if (graphics.SSR_Quality == 1) // LOW
        //    {
        //        SSR2.SetActive(true);
        //        SSR.sampleCount.value = 1;
        //        SSR.binarySearchIterations.value = 4;

        //    }
        //    else if (graphics.SSR_Quality == 2) // NORMAL
        //    {
        //        SSR2.SetActive(true);
        //        SSR.sampleCount.value = 16;
        //        SSR.binarySearchIterations.value = 6;
        //    }
        //    else if (graphics.SSR_Quality == 3) // ULTRA
        //    {
        //        SSR2.SetActive(true);
        //        SSR.sampleCount.value = 128;
        //        SSR.binarySearchIterations.value = 12;
        //    }
        //}

        // MOTION BLUR
        UnityEngine.Rendering.Universal.MotionBlur Blur;
        if (MainVolume.profile.TryGet<UnityEngine.Rendering.Universal.MotionBlur >(out Blur))
        {
            if (graphics.BlurQuality == 0) // OFF
            {
                Blur.intensity.value = 0;
            }
            else if (graphics.BlurQuality == 1) // FX ONLY NORMAL
            {
                Blur.intensity.value = 1;
                Blur.quality.value = MotionBlurQuality.Low;
            }
            else if (graphics.BlurQuality == 2) // FX ONLY HIGH
            {
                Blur.intensity.value = 1;
                Blur.quality.value = MotionBlurQuality.High;
            }
        }

        // BLOOM
        if (MainVolume.profile.TryGet(out UnityEngine.Rendering.Universal.Bloom bloom))
        {
            if (graphics.BloomQuality == 0) // OFF
            {
                bloom.intensity.value = 0;
            }
            else if (graphics.BloomQuality == 1) // LOW
            {
                bloom.intensity.value = 1;
                bloom.highQualityFiltering.value = false;
            }
            else if (graphics.BloomQuality == 2) // NORMAL
            {
                bloom.intensity.value = 1;
                bloom.highQualityFiltering.value = true;

            }
        }

        // GI
        //RadiantGI.Universal.RadiantGlobalIllumination GI;
        //RadiantGI.Universal.RadiantRenderFeature GI2;
        //RenderData.TryGetRendererFeature<RadiantGI.Universal.RadiantRenderFeature>(out GI2);
        //if (MainVolume.profile.TryGet<RadiantGI.Universal.RadiantGlobalIllumination>(out GI))
        //{
        //    if (graphics.GI_Quality == 0) // OFF
        //    {
        //        GI2.SetActive(false);
        //    }
        //    else if (graphics.GI_Quality == 1) // LOW
        //    {
        //        GI2.SetActive(true);
        //        GI.raytracerAccuracy.value = 1;
        //        GI.downsampling.value = 4;
        //        GI.rayCount.value = 1;
        //        GI.rayBounce.value = false;
        //        GI.blurQuality.value = RadiantGI.Universal.RadiantGlobalIllumination.BlurQuality.Faster;

        //    }
        //    else if (graphics.GI_Quality == 2) // NORMAL
        //    {
        //        GI2.SetActive(true);
        //        GI.raytracerAccuracy.value = 6;
        //        GI.downsampling.value = 2;
        //        GI.rayCount.value = 1;
        //        GI.rayBounce.value = false;
        //        GI.blurQuality.value = RadiantGI.Universal.RadiantGlobalIllumination.BlurQuality.Faster;

        //    }
        //    else if (graphics.GI_Quality == 3) // HIGH
        //    {
        //        GI2.SetActive(true);
        //        GI.raytracerAccuracy.value = 8;
        //        GI.downsampling.value = 1;
        //        GI.rayCount.value = 4;
        //        GI.rayBounce.value = true;
        //        GI.blurQuality.value = RadiantGI.Universal.RadiantGlobalIllumination.BlurQuality.Faster;
        //    }
        //}

        // AA
        UniversalAdditionalCameraData cam = MainCam.GetUniversalAdditionalCameraData();
        if (cam != null)
        {
            if(graphics.AntiAliasing == 0) 
            { 
                cam.antialiasing = AntialiasingMode.None;
                cam.antialiasingQuality = AntialiasingQuality.Medium;
            }
            else if (graphics.AntiAliasing == 1) 
            { 
                cam.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                cam.antialiasingQuality = AntialiasingQuality.High;
            }
            else if (graphics.AntiAliasing == 2) 
            { 
                cam.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                cam.antialiasingQuality = AntialiasingQuality.High;
            }
            else if (graphics.AntiAliasing == 3)
            {
                cam.antialiasing = AntialiasingMode.TemporalAntiAliasing;
                cam.antialiasingQuality = AntialiasingQuality.High;
            }
        }
    }

    public void SaveGraphicsSettings(GraphicsSettings graphics)
    {
        System.IO.File.WriteAllText(Application.dataPath + "/GraphicSettings.gs", JsonUtility.ToJson(graphics, true));
        Debug.Log("Saved Graphics Settings!");
    }

    public GraphicsSettings LoadGraphicsSettings()
    {
        string s;
        if (File.Exists(Application.dataPath + "/GraphicSettings.gs"))
        {
            s = File.ReadAllText(Application.dataPath + "/GraphicSettings.gs");
            Debug.Log("Loaded Graphics Settings!");
            return JsonUtility.FromJson<GraphicsSettings>(s);
        }
        else
        {
            GraphicsSettings g = new GraphicsSettings(1);
            g = MenuObj.Profile[2];
            Debug.Log("New graphic settings created, Profile: " + g.TextureQuality + ", " + g.AO_Quality);
            return g;
        }
    }

    // CLASSES
    [System.Serializable]
    public struct GraphicsSettings
    {       
        // SET CURRENT TO index 6, ON START (STEAM DECK IS 15)
        public Resolution CurrentResolution;
        public int ResolutionIndex;
        public FullScreenMode FullScreen;
        public float ResolutionScale;
        public int UpScalingType;
        public int FPS_Target;
        public int TextureQuality;
        public AnisotropicFiltering AnisoQuality;
        public int TextureStreaming;
        public int Shadowquality;
        public int ShadowRes;
        public float LOD_Bias;

        public int AO_Quality;
        public int SSR_Quality;
        public int BlurQuality;
        public int BloomQuality;
        public int GI_Quality;
        public int AntiAliasing;
        public int Vsync;

        public int Preset;

        public GraphicsSettings(int i)
        {
            this.CurrentResolution = GraphicsEditor.Resolutions[4];
            this.ResolutionIndex = 6;
            this.FullScreen = FullScreenMode.FullScreenWindow;
            this.ResolutionScale = 1;
            this.UpScalingType = 0;
            this.FPS_Target = 60;
            this.Vsync = 1;
            this.TextureQuality = 1;
            this.AnisoQuality = AnisotropicFiltering.ForceEnable;
            this.TextureStreaming = 2;
            this.Shadowquality = 3;
            this.ShadowRes = 3;
            this.LOD_Bias = 1;

            this.AO_Quality = 1;
            this.SSR_Quality = 2;
            this.BlurQuality = 1;
            this.BloomQuality = 1;
            this.GI_Quality = 2;
            this.AntiAliasing = 1;

            this.Preset = 2;
        }
    }

    /*
	> PRESETS
		0 > Low, Med, Normal, Ultra, Overkill.

	> SCREEN
		1 > Resolution
		2 > Window                  [ BORDERLESS, WINDOW, FULL ]
		3 > Resolution Scale(%)     [ ]
        4 > Upscaling Type          [ NORMAL, FSR ]
        5 > Dynamic Res             [ OFF, ON ]
        6 > Framerate               [ ]
        7 > Vsync                   [ OFF, ON ]
		8 > Texture Max Resolution  [ LOWEST, LOW, NORMAL, FULL ]
		9 > Texture Aniso           [ OFF, ON ]
		10 > Texture Streaming       [ OFF, HALF, NORMAL, ULTRA ]
		11 > Shadow Resolution      [ 512, 1024, 2048, 4096, 8192 ]
		12 > Draw Distance          [ NORMAL, EXTENDED+, EXTENDED++, EXTENDED++++ ]

	> POST PROCESSING
		13 > Ambient Occlusion      [ OFF, NORMAL ]
		14 > SSR                    [ OFF, LOW, NORMAL, ULTRA ]
		15 > Blur                   [ OFF, FX ONLY - NORMAL, FX ONLY - HIGH)
		16 > Bloom                  [ OFF, NORMAL, HIGH ]
		17 > Global Illumination    [ OFF, LOW, NORMAL, OVERKILL ]
        18 > FXAA                   [ OFF, FXAA, SMAA, TAA ]
    */

    [System.Serializable]
    public struct Resolution
    {
        public Vector2 resolution;
        public string AspectRatio;
        public string Description;

        public Resolution(Vector2 res, string aspect, string descrp)
        {
            resolution = res;
            AspectRatio = aspect;
            Description = descrp;
        }
    }
}
