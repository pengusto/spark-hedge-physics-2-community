using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;

public class GraphicsMenu : MonoBehaviour
{
    [Header("References & Parameter")]
    public GraphicsEditor editor;
    public RectTransform MenuObject;
    public float MenuMoveSpeed = 1000;
    public Vector3 MenuObjPosMinMax = new Vector3(-100, 100);
    public PauseMenu PauseScript;
    public GameObject PauseObj;
    public MenuExtensions.MenuIten[] MenuItens;
    public GraphicsEditor.GraphicsSettings[] Profile; //5 PROFILES, -1 is custom
    public Text Description;
    public int Iten;
    public Transform Arrow;
    public float ArrowSpeed = 1;

    [Header("FX")]
    public AudioSource SoundMove;
    public AudioSource SoundBack;
    public AudioSource SoundClick;

    [Header("Settings")]
    public float[] LOD_Levels = { 1, 1.5f, 2.5f, 5, 7 };

    // CACHE
    public MenuExtensions MenuEx = new MenuExtensions();
    public MenuExtensions MenuEy = new MenuExtensions();
    //public Rewired.Player Inp;
    int inpx;
    int inpy;
    public Vector2 RawInput;
    GraphicsEditor.GraphicsSettings g = new GraphicsEditor.GraphicsSettings();
    bool applySignal = false;

    // MENU
    private void Start()
    {
        g = GraphicsEditor.CurrentSettings;
        //Inp = Rewired.ReInput.players.GetPlayer(0);
        SetUI_GraphicsText();

        for (int i = 0; i < MenuItens.Length; i++)
        {
            MenuItens[i].Obj.TryGetComponent<UiButtonEffects>(out MenuItens[i].Effects);
        }
    }

    private void OnEnable()
    {
        Iten = 0;
    }

    private void Update()
    {
        // INPUT
        //RawInput = new Vector2(Inp.GetAxis("LeftAnalogX"), -Inp.GetAxis("LeftAnalogY"));
        //if (Inp.GetButton("D_Right")) { RawInput.x++; }
        //if (Inp.GetButton("D_Left")) { RawInput.x--; }
        //if (Inp.GetButton("D_Up")) { RawInput.y--; }
        //if (Inp.GetButton("D_Down")) { RawInput.y++; }
        RawInput.x = Mathf.Clamp(RawInput.x, -1, 1);
        RawInput.y = Mathf.Clamp(RawInput.y, -1, 1);
        inpx = MenuEx.MenuMovement(RawInput.x);
        inpy = MenuEy.MenuMovement(RawInput.y);

        // SOUND
        if (inpx == 1) 
        {
            SetUI_GraphicsText();
            applySignal = true;
            SoundMove.Play();  
        }
        else if (inpx == -1) 
        {
            SetUI_GraphicsText();
            applySignal = true;
            SoundMove.Play(); 
        }

        // UP AND DOWN MOVE
        if (inpy == 1) 
        {
            Iten++;
            if(Iten >= MenuItens.Length) { Iten = 0; }
            else if (Iten < 0) { Iten = MenuItens.Length - 1; }
            SoundMove.Play();
        }
        else if (inpy == -1) 
        {
            Iten--;
            if (Iten >= MenuItens.Length) { Iten = 0; }
            else if (Iten < 0) { Iten = MenuItens.Length - 1; }
            SoundMove.Play();
        }

        // ITENS
        if (Iten == 0) // PRESET
        {
            if (inpx == 1)
            {
                g.Preset += 1;
                if (g.Preset < 0) { g.Preset = 4; }
                if (g.Preset > 4) { g.Preset = 0; }
                SetUI_GraphicsText();
                if (editor) { editor.ApplyGraphicsLocal(); }
            }
            else if (inpx == -1)
            {
                g.Preset -= 1;
                if (g.Preset < 0) { g.Preset = 4; }
                if (g.Preset > 4) { g.Preset = 0; }
                SetUI_GraphicsText();
                if (editor) { editor.ApplyGraphicsLocal(); }
            }
        }
        else if (Iten == 1) // RES
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.ResolutionIndex += 1;
                if (g.ResolutionIndex > GraphicsEditor.Resolutions.Length - 1) { g.ResolutionIndex = 0; }
                else if (g.ResolutionIndex < 0) { g.ResolutionIndex = GraphicsEditor.Resolutions.Length - 1; }
                g.CurrentResolution = GraphicsEditor.Resolutions[g.ResolutionIndex];
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.ResolutionIndex -= 1;
                if (g.ResolutionIndex > GraphicsEditor.Resolutions.Length - 1) { g.ResolutionIndex = 0; }
                else if (g.ResolutionIndex < 0) { g.ResolutionIndex = GraphicsEditor.Resolutions.Length - 1; }
                g.CurrentResolution = GraphicsEditor.Resolutions[g.ResolutionIndex];
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 2)
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.FullScreen = FullScreenMode.FullScreenWindow;
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.FullScreen = FullScreenMode.Windowed;
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 3) // RES SCALE
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.ResolutionScale += 0.1f;
                g.ResolutionScale = Mathf.Clamp(g.ResolutionScale, 0.2f, 2f);
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.ResolutionScale -= 0.1f;
                g.ResolutionScale = Mathf.Clamp(g.ResolutionScale, 0.2f, 2f);
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 4) // UPSCALING TYPE
        {
            // SET FPS
            if (inpx == 1)
            {
                g.Preset = -1;
                g.UpScalingType -= 1;
                if (g.UpScalingType < 0) { g.UpScalingType = 1; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.UpScalingType += 1;
                if (g.UpScalingType > 1) { g.UpScalingType = 0; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 5) // DYNAMIC RESOLUTION
        {
            // You're gonna have to implement this yourself.
        }
        else if (Iten == 6) // TARGET FRAMERATE
        {
            // SET FPS
            if (inpx == 1)
            {
                g.Preset = -1;
                g.FPS_Target += 10;
                Mathf.Clamp(g.FPS_Target, 30, 9000);
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.FPS_Target -= 10;
                Mathf.Clamp(g.FPS_Target, 30, 9000);
                SetUI_GraphicsText();
            }
        }
        else if(Iten == 7) // VSYNC
        {
            // SET FPS
            if (inpx == 1)
            {
                g.Preset = -1;
                g.Vsync = 0;
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.Vsync = 1;
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 8) // TEX RES
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.TextureQuality -= 1;
                if (g.TextureQuality > 4) { g.TextureQuality = 0; }
                else if (g.TextureQuality < 0) { g.TextureQuality = 4; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.TextureQuality += 1;
                if (g.TextureQuality > 4) { g.TextureQuality = 0; }
                else if (g.TextureQuality < 0) { g.TextureQuality = 4; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 9) // ANISO
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.AnisoQuality = AnisotropicFiltering.ForceEnable;
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.AnisoQuality = AnisotropicFiltering.Disable;
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 10) // TEX STREAMING
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.TextureStreaming += 1;
                if (g.TextureStreaming > 3) { g.TextureStreaming = 0; }
                else if (g.TextureStreaming < 0) { g.TextureStreaming = 3; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.TextureStreaming -= 1;
                if (g.TextureStreaming > 3) { g.TextureStreaming = 0; }
                else if (g.TextureStreaming < 0) { g.TextureStreaming = 3; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 11) // SHADOW RES
        {
            if (inpx == -1)
            {
                g.Preset = -1;
                g.ShadowRes -= 1;
                if (g.ShadowRes < 0) { g.ShadowRes = 4; }
                SetUI_GraphicsText();
            }
            else if (inpx == 1)
            {
                g.Preset = -1;
                g.ShadowRes += 1;
                if (g.ShadowRes > 4) { g.ShadowRes = 0; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 12) // LOD (DRAW DISTANCE)
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                if (g.LOD_Bias == 1f) { g.LOD_Bias = 1.5f; }
                else if (g.LOD_Bias == 1.5f) { g.LOD_Bias = 2.5f; }
                else if (g.LOD_Bias == 2.5f) { g.LOD_Bias = 5f; }
                else if (g.LOD_Bias == 5f) { g.LOD_Bias = 7f; }
                else if (g.LOD_Bias == 7f) { g.LOD_Bias = 1f; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                if (g.LOD_Bias == 1f) { g.LOD_Bias = 7f; }
                else if (g.LOD_Bias == 7f) { g.LOD_Bias = 5f; }
                else if (g.LOD_Bias == 5f) { g.LOD_Bias = 2.5f; }
                else if (g.LOD_Bias == 2.5f) { g.LOD_Bias = 1.5f; }
                else if (g.LOD_Bias == 1.5f) { g.LOD_Bias = 1f; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 13) // POST START - AO
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.AO_Quality = 0;
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.AO_Quality = 1;
                SetUI_GraphicsText();
            }

            if (g.AO_Quality > 2) { g.AO_Quality = 0; }
            else if (g.AO_Quality < 0) { g.AO_Quality = 2; }
        }
        else if (Iten == 14) // SSR
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.SSR_Quality += 1;
                if (g.SSR_Quality > 3) { g.SSR_Quality = 0; }
                else if (g.SSR_Quality < 0) { g.SSR_Quality = 3; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.SSR_Quality -= 1;
                if (g.SSR_Quality > 3) { g.SSR_Quality = 0; }
                else if (g.SSR_Quality < 0) { g.SSR_Quality = 3; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 15) // BLUR
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.BlurQuality += 1;
                if (g.BlurQuality > 2) { g.BlurQuality = 0; }
                else if (g.BlurQuality < 0) { g.BlurQuality = 2; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.BlurQuality -= 1;
                if (g.BlurQuality > 2) { g.BlurQuality = 0; }
                else if (g.BlurQuality < 0) { g.BlurQuality = 2; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 16) // BLOOM
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.BloomQuality += 1;
                if (g.BloomQuality > 2) { g.BloomQuality = 0; }
                else if (g.BloomQuality < 0) { g.BloomQuality = 2; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.BloomQuality -= 1;
                if (g.BloomQuality > 2) { g.BloomQuality = 0; }
                else if (g.BloomQuality < 0) { g.BloomQuality = 2; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 17) // GI
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.GI_Quality += 1;
                if (g.GI_Quality > 3) { g.GI_Quality = 0; }
                else if (g.GI_Quality < 0) { g.GI_Quality = 3; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.GI_Quality -= 1;
                if (g.GI_Quality > 3) { g.GI_Quality = 0; }
                else if (g.GI_Quality < 0) { g.GI_Quality = 3; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 18) // AA
        {
            if (inpx == 1)
            {
                g.Preset = -1;
                g.AntiAliasing += 1;
                if (g.AntiAliasing > 3) { g.AntiAliasing = 0; }
                else if (g.AntiAliasing < 0) { g.AntiAliasing = 3; }
                SetUI_GraphicsText();
            }
            else if (inpx == -1)
            {
                g.Preset = -1;
                g.AntiAliasing -= 1;
                if (g.AntiAliasing > 3) { g.AntiAliasing = 0; }
                else if (g.AntiAliasing < 0) { g.AntiAliasing = 3; }
                SetUI_GraphicsText();
            }
        }
        else if (Iten == 19) // APPLY
        {
            SetUI_GraphicsText();
            //editor.ApplyGraphics(g, editor.Volume, editor.GI);
        }
        else if (Iten == 20) // BACK
        {
            //if (Inp.GetButton("A"))
            //{
            //    SoundClick.Play();
            //    SetUI_GraphicsText();
            //    if (PauseObj) { PauseObj.SetActive(true); }
            //    gameObject.SetActive(false);
            //    return;
            //}
        }

        // MENU MOVEMENT
        if (Arrow.localPosition.y > MenuObjPosMinMax.y) 
        {
            Vector3 p = MenuObject.position;
            p.y -= Time.unscaledDeltaTime * MenuMoveSpeed;
            MenuObject.position = p;
        }
        else if (Arrow.localPosition.y < MenuObjPosMinMax.x)
        {
            Vector3 p = MenuObject.position;
            p.y += Time.unscaledDeltaTime * MenuMoveSpeed;
            MenuObject.position = p;
        }

        // MOVE ARROW
        Arrow.position = Vector3.Lerp(Arrow.position, MenuItens[Iten].Position.position, Time.unscaledDeltaTime * 30);
        Description.text = MenuItens[Iten].Description;
        inpy = 0;

        // APPLY
        //if (Inp.GetButtonDown("A"))
        //{
        //    SoundClick.Play();
        //    SetUI_GraphicsText();
        //    if (MenuItens[Iten].Effects) { MenuItens[Iten].Effects.StartGlow(1); }
        //}

        // BACK
        //if (Inp.GetButtonDown("B"))
        //{
        //    SoundBack.Play();
        //    if (PauseScript) { PauseScript.OnSubMenu = false; }
        //    if (PauseObj) { PauseObj.SetActive(true); }
        //    gameObject.SetActive(false);
        //}

        // APPLY WITH SIGNAL
        if (applySignal)
        {
            SetUI_GraphicsText();
            applySignal = false;
        }

    }

    // FUNCTIONS
    public void SetUI_GraphicsText()
    {
        // PRESET
        if (g.Preset == 0)      { g = Profile[0]; g.Preset = 0; MenuItens[0].ParameterText.text = "<LOW>";}
        else if (g.Preset == 1) { g = Profile[1]; g.Preset = 1; MenuItens[0].ParameterText.text = "<MED>"; }
        else if (g.Preset == 2) { g = Profile[2]; g.Preset = 2; MenuItens[0].ParameterText.text = "<NORMAL>"; }
        else if (g.Preset == 3) { g = Profile[3]; g.Preset = 3; MenuItens[0].ParameterText.text = "<ULTRA>"; }
        else if (g.Preset == 4) { g = Profile[4]; g.Preset = 4; MenuItens[0].ParameterText.text = "<OVERKILL>"; }
        else { MenuItens[0].ParameterText.text = "<CUSTOM>"; }

        // RESOLUTION
        if(g.CurrentResolution.resolution.x <= 20)
        {
            MenuItens[1].ParameterText.text = "Current";
        }
        else
        {
            MenuItens[1].ParameterText.text =
                g.CurrentResolution.resolution.x + "x" +
                g.CurrentResolution.resolution.y + ", " +
                g.CurrentResolution.AspectRatio + ", " +
                g.CurrentResolution.Description;
        }

        // WINDOW
        if (g.FullScreen == FullScreenMode.ExclusiveFullScreen)
            { MenuItens[2].ParameterText.text = "<EX FULL SCREEN>"; }
        else if (g.FullScreen == FullScreenMode.FullScreenWindow)
            { MenuItens[2].ParameterText.text = "<FULL SCREEN>"; }
        else if (g.FullScreen == FullScreenMode.Windowed)
            { MenuItens[2].ParameterText.text = "<WINDOWED>"; }
        else { MenuItens[2].ParameterText.text = "<!ERROR!>"; }

        // RESOLUTION SCALE
        MenuItens[3].ParameterText.text = "<" + System.Math.Round(g.ResolutionScale, 2) + ">";

        // UPSCALING
        if (g.UpScalingType == 0) { MenuItens[4].ParameterText.text = "<NORMAL>"; }
        else if (g.UpScalingType == 1) { MenuItens[4].ParameterText.text = "<FSR>"; }

        // DYNAMIC RES
        MenuItens[5].ParameterText.text = "<--->";

        // FPS
        MenuItens[6].ParameterText.text = "<" + g.FPS_Target + ">";

        // VSYNC
        if (g.Vsync == 0) { MenuItens[7].ParameterText.text = "<OFF>"; }
        else if (g.Vsync == 1) { MenuItens[7].ParameterText.text = "<ON>"; }

        // TEXTURE LIMIT
        if (g.TextureQuality == 0) { MenuItens[8].ParameterText.text = "<OVERKILL>"; }
        else if (g.TextureQuality == 1) { MenuItens[8].ParameterText.text = "<NORMAL>"; }
        else if (g.TextureQuality == 2) { MenuItens[8].ParameterText.text = "<MED>"; }
        else if (g.TextureQuality == 3) { MenuItens[8].ParameterText.text = "<LOW>"; }
        else if (g.TextureQuality == 4) { MenuItens[8].ParameterText.text = "<LOWEST>"; }
        else { MenuItens[8].ParameterText.text = "<!ERROR!>"; }

        // ANISO
        if (g.AnisoQuality == AnisotropicFiltering.Disable) 
            { MenuItens[9].ParameterText.text = "<OFF>"; }
        else if (g.AnisoQuality == AnisotropicFiltering.Enable) 
            { MenuItens[9].ParameterText.text = "<PER TEX>"; }
        else if (g.AnisoQuality == AnisotropicFiltering.ForceEnable) 
            { MenuItens[9].ParameterText.text = "<ON>"; }
        else { MenuItens[9].ParameterText.text = "<!ERROR!>"; }

        // STREAMING
        if (g.TextureStreaming == 0) { MenuItens[10].ParameterText.text = "<HALF>"; }
        else if (g.TextureStreaming == 1) { MenuItens[10].ParameterText.text = "<NORMAL>"; }
        else if (g.TextureStreaming == 2) { MenuItens[10].ParameterText.text = "<OVERLOADED>"; }
        else if (g.TextureStreaming == 3) { MenuItens[10].ParameterText.text = "<OFF>"; }
        else { MenuItens[10].ParameterText.text = "<!ERROR!>"; }

        // SHADOW QUALITY (no more)

        // SHADOW RES
        if (g.ShadowRes == 0) { MenuItens[11].ParameterText.text = "<BAD>"; }
        else if (g.ShadowRes == 1) { MenuItens[11].ParameterText.text = "<VERY LOW>"; }
        else if (g.ShadowRes == 2) { MenuItens[11].ParameterText.text = "<NOT SO LOW>"; }
        else if (g.ShadowRes == 3) { MenuItens[11].ParameterText.text = "<NORMAL>"; }
        else if (g.ShadowRes == 4) { MenuItens[11].ParameterText.text = "<HIGH>"; }
        else { MenuItens[11].ParameterText.text = "<!ERROR!>"; }

        // LOD
        if (g.LOD_Bias == 1) { MenuItens[12].ParameterText.text = "<NORMAL>"; }
        else if (g.LOD_Bias == 1.5f) { MenuItens[12].ParameterText.text = "<EXTENDED>"; }
        else if (g.LOD_Bias == 2.5f) { MenuItens[12].ParameterText.text = "<EXTENDED+>"; }
        else if (g.LOD_Bias == 5f) { MenuItens[12].ParameterText.text = "<EXTENDED++>"; }
        else if (g.LOD_Bias == 7f) { MenuItens[12].ParameterText.text = "<EXTENDED+++>"; }
        else { MenuItens[12].ParameterText.text = "<!ERROR!>"; }

        // POST PROCESSING ====
        // AO
        if (g.AO_Quality == 0) { MenuItens[13].ParameterText.text = "<OFF>"; }
        else if (g.AO_Quality == 1) { MenuItens[13].ParameterText.text = "<ON>"; }
        else { MenuItens[13].ParameterText.text = "<!ERROR!>"; }

        // SSR
        if (g.SSR_Quality == 0) { MenuItens[14].ParameterText.text = "<OFF>"; }
        else if (g.SSR_Quality == 1) { MenuItens[14].ParameterText.text = "<LOW>"; }
        else if (g.SSR_Quality == 2) { MenuItens[14].ParameterText.text = "<NORMAL>"; }
        else if (g.SSR_Quality == 3) { MenuItens[14].ParameterText.text = "<ULTRA>"; }
        else { MenuItens[14].ParameterText.text = "<!ERROR!>"; }

        // BLUR
        if(g.BlurQuality == 0) { MenuItens[15].ParameterText.text = "<OFF>"; }
        else if (g.BlurQuality == 1) { MenuItens[15].ParameterText.text = "<FX ONLY - NORMAL>"; }
        else if (g.BlurQuality == 2) { MenuItens[15].ParameterText.text = "<FX ONLY - HIGH>"; }
        else { MenuItens[15].ParameterText.text = "<!ERROR!>"; }

        // BLOOM
        if (g.BloomQuality == 0) { MenuItens[16].ParameterText.text = "<OFF>"; }
        else if (g.BloomQuality == 1) { MenuItens[16].ParameterText.text = "<LOW>"; }
        else if (g.BloomQuality == 2) { MenuItens[16].ParameterText.text = "<ON>"; }
        else { MenuItens[16].ParameterText.text = "<!ERROR!>"; }

        // GI
        if (g.GI_Quality == 0) { MenuItens[17].ParameterText.text = "<OFF>"; }
        else if (g.GI_Quality == 1) { MenuItens[17].ParameterText.text = "<LOW>"; }
        else if (g.GI_Quality == 2) { MenuItens[17].ParameterText.text = "<NORMAL>"; }
        else if (g.GI_Quality == 3) { MenuItens[17].ParameterText.text = "<OVERKILL>"; }
        else { MenuItens[17].ParameterText.text = "<!ERROR!>"; }

        // ANTI ALIAS
        if(g.AntiAliasing == 0) { MenuItens[18].ParameterText.text = "<OFF>"; }
        else if (g.AntiAliasing == 1) { MenuItens[18].ParameterText.text = "<FXAA (NORMAL)>"; }
        else if (g.AntiAliasing == 2) { MenuItens[18].ParameterText.text = "<SMAA>"; }
        else if (g.AntiAliasing == 3) { MenuItens[18].ParameterText.text = "<TAA>"; }
        else { MenuItens[18].ParameterText.text = "<!ERROR!>"; }

        // APPLY
        GraphicsEditor.CurrentSettings = g;
        g = GraphicsEditor.CurrentSettings;
        editor.ApplyGraphics(g, editor.MainVolume);
    }

}

// CLASSES
[System.Serializable]
public class MenuExtensions
{
    public float c = 0;
    public float HoldingCounter = 0;
    public float HoldingThreshold = 1;
    public float HoldingIntervalCounter;
    public float HoldingIntervalThreshold = 0.1f;

    [System.Serializable]
    public class MenuIten
    {
        public GameObject Obj;
        public Transform Position;
        public Text ParameterText;
        public string Description;
        public UiButtonEffects Effects;
        public Image HoldCircle;
    }

    public int MenuMovement(float input)
    {
        HoldingCounter += Time.unscaledDeltaTime;
        if(input > 0.7f) 
        {
            return InputToReturn(Mathf.Ceil(input));
        }
        else if (input < -0.7f)
        {
            return InputToReturn(Mathf.Floor(input));
        }
        else
        {
            c = 0;
            HoldingCounter = 0;
            return 0;
        }
    }

    int InputToReturn(float input)
    {
        if (c < 1)
        {
            c = 2;
            return 1 * (int)input;
        }
        else if (HoldingCounter > HoldingThreshold)
        {
            HoldingIntervalCounter += Time.unscaledDeltaTime;
            if (HoldingIntervalCounter > HoldingIntervalThreshold)
            {
                HoldingIntervalCounter = 0;
                return 1 * (int)input;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }
}
