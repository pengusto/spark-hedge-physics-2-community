using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OptionsMenu : MonoBehaviour
{

    public static OptionsSettings CurrentSettings;

    [Header("Menu")]
    public int ItenY = 0;
    public float VolumeIteration = 1;
    public List<MenuExtensions.MenuIten> Itens;
    public RectTransform Sfx_Bar;
    public RectTransform Cars_Bar;
    public RectTransform Music_Bar;
    public Transform MenuArrow;
    public GameObject PauseMenu;
    public GameObject InputAssMenu;

    [Header("Audio")]
    public UnityEngine.Audio.AudioMixer Mixer;
    public AudioSource SoundMove;
    public AudioSource SoundBack;
    public AudioSource SoundClick;

    // CACHE
    public MenuExtensions MenuEx = new MenuExtensions();
    public MenuExtensions MenuEy = new MenuExtensions();
    //public Rewired.Player Inp;
    public const float MusicVolumeOffset = 10;
    int inpx;
    int inpy;
    public Vector2 RawInput;

    void Start()
    {
        //Inp = Rewired.ReInput.players.GetPlayer(0);
        LoadSettings();
        SetSettings();
    }

    void Update()
    {
        //RawInput = new Vector2(Inp.GetAxis("LeftAnalogX"), -Inp.GetAxis("LeftAnalogY"));
        //if (Inp.GetButton("D_Right")) { RawInput.x++; }
        //if (Inp.GetButton("D_Left")) { RawInput.x--; }
        //if (Inp.GetButton("D_Up")) { RawInput.y--; }
        //if (Inp.GetButton("D_Down")) { RawInput.y++; }
        RawInput.x = Mathf.Clamp(RawInput.x, -1, 1);
        RawInput.y = Mathf.Clamp(RawInput.y, -1, 1);
        inpx = MenuEx.MenuMovement(RawInput.x);
        inpy = MenuEy.MenuMovement(RawInput.y);

        // UP AND DOWN MOVE
        if (inpy == 1 || inpy == -1)
        {
            ItenY += inpy;
            SoundMove.Play();
            inpy = 0;
        }

        if (ItenY >= Itens.Count) { ItenY = 0; }
        else if (ItenY < 0) { ItenY = Itens.Count - 1; }

        Mixer.GetFloat("SfxVolume", out CurrentSettings.Volume_Sfx);
        Mixer.GetFloat("MusicVolume", out CurrentSettings.Volume_Music);
        Mixer.GetFloat("CarsVolume", out CurrentSettings.Volume_Cars);
        CurrentSettings.Volume_Music -= MusicVolumeOffset;

        Sfx_Bar.localScale =    new Vector3(Mathf.Lerp(1, 0, -CurrentSettings.Volume_Sfx / 80), 1, 1);
        Music_Bar.localScale =  new Vector3(Mathf.Lerp(1, 0, -CurrentSettings.Volume_Music / 80), 1, 1);
        Cars_Bar.localScale =   new Vector3(Mathf.Lerp(1, 0, -CurrentSettings.Volume_Cars / 80), 1, 1);

        if(ItenY == 0) // SFX
        {
            if (inpx == 1 || inpx == -1)
            {
                CurrentSettings.Volume_Sfx += (VolumeIteration * inpx);
                CurrentSettings.Volume_Sfx = Mathf.Clamp(CurrentSettings.Volume_Sfx, -80, 0);
                SetSettings();
            }
        }
        else if (ItenY == 1) // MUSIC
        {
            if (inpx == 1 || inpx == -1)
            {
                CurrentSettings.Volume_Music += (VolumeIteration * inpx);
                CurrentSettings.Volume_Music = Mathf.Clamp(CurrentSettings.Volume_Music, -80, 0);
                SetSettings();
            }
        }
        else if (ItenY == 2) // CARS
        {
            if (inpx == 1 || inpx == -1)
            {
                CurrentSettings.Volume_Cars += (VolumeIteration * inpx);
                CurrentSettings.Volume_Cars = Mathf.Clamp(CurrentSettings.Volume_Cars, -80, 0);
                SetSettings();
            }
        }
        else if (ItenY == 3) // INVERT X
        {
            if (inpx == 1)          { CurrentSettings.InvertX = true; }
            else if (inpx == -1)    { CurrentSettings.InvertX = false; }
            SetSettings();
        }
        else if (ItenY == 4) // INVERT Y
        {
            if (inpx == 1) { CurrentSettings.InvertY = true; }
            else if (inpx == -1) { CurrentSettings.InvertY = false; }
            SetSettings();
        }
        else if (ItenY == 5) // SENS X
        {
            if (inpx == 1) { CurrentSettings.CamX_Sens += 0.05f; }
            else if (inpx == -1) { CurrentSettings.CamX_Sens -= 0.05f; }

            CurrentSettings.CamX_Sens = Mathf.Clamp(CurrentSettings.CamX_Sens, 0, 5);
            SetSettings();
        }
        else if (ItenY == 6) // SENS Y
        {
            if (inpx == 1) { CurrentSettings.CamY_Sens += 0.05f; }
            else if (inpx == -1) { CurrentSettings.CamY_Sens -= 0.05f; }

            CurrentSettings.CamY_Sens = Mathf.Clamp(CurrentSettings.CamY_Sens, 0, 5);
            SetSettings();
        }
        else if (ItenY == 7) // INPUT ASS MENU
        {
            //if (Inp.GetButtonDown("A"))
            //{
            //    SetSettings();
            //    SaveSettings();
            //    if (PauseMenu) { PauseMenu.gameObject.SetActive(false); }
            //    if (InputAssMenu) { InputAssMenu.gameObject.SetActive(true); }
            //    gameObject.SetActive(false);
            //    SoundClick.Play();
            //}
        }
        else if (ItenY == 8) // END
        {
            //if (Inp.GetButtonDown("A"))
            //{
            //    SetSettings();
            //    SaveSettings();
            //    if (PauseMenu) { PauseMenu.gameObject.SetActive(true); }
            //    gameObject.SetActive(false);
            //    SoundBack.Play();
            //}
        }

        //if (Inp.GetButtonDown("B")) // END
        //{
        //    SetSettings();
        //    SaveSettings();
        //    if (PauseMenu) { PauseMenu.gameObject.SetActive(true); }
        //    gameObject.SetActive(false);
        //}

        // ARROW
        MenuArrow.position = Vector3.Lerp(MenuArrow.position, Itens[ItenY].Position.position, Time.unscaledTime * 35);
        SetSettings();
    }

    public void SetSettings()
    {
        Mixer.SetFloat("CarsVolume", CurrentSettings.Volume_Cars);
        Mixer.SetFloat("MusicVolume", CurrentSettings.Volume_Music + MusicVolumeOffset);
        Mixer.SetFloat("SfxVolume", CurrentSettings.Volume_Sfx);
        if (CurrentSettings.InvertX) { Itens[3].ParameterText.text = "[YES]"; } else { Itens[3].ParameterText.text = "[NO]"; }
        if (CurrentSettings.InvertY) { Itens[4].ParameterText.text = "[YES]"; } else { Itens[4].ParameterText.text = "[NO]"; }
        Itens[5].ParameterText.text = "[" + string.Format("{0:000}", CurrentSettings.CamX_Sens * 100) + "]";
        Itens[6].ParameterText.text = "[" + string.Format("{0:000}", CurrentSettings.CamY_Sens * 100) + "]";

        Debug.Log("Set Mixer");
    }

    public void SaveSettings()
    {
        System.IO.File.WriteAllText(Application.dataPath + "/Settings.gs", JsonUtility.ToJson(CurrentSettings, true));
        Debug.Log("Saved Settings!");
    }

    public OptionsSettings LoadSettings()
    {
        string s;
        if (File.Exists(Application.dataPath + "/Settings.gs"))
        {
            s = File.ReadAllText(Application.dataPath + "/Settings.gs");
            OptionsSettings g = JsonUtility.FromJson<OptionsSettings>(s);
            CurrentSettings = g;
            Debug.Log("Loaded Settings!");
            return g;
        }
        else
        {
            OptionsSettings g = new OptionsSettings();
            g.Volume_Sfx = 0;
            g.Volume_Cars = 0;
            g.Volume_Music = 0;
            g.InvertX = false;
            g.InvertY = false;
            g.CamX_Sens = 1;
            g.CamY_Sens = 1;
            CurrentSettings = g;
            Debug.Log("New settings created");
            return g;
        }
    }

    [System.Serializable]
    public class OptionsSettings
    {
        public float Volume_Sfx = 0;
        public float Volume_Cars = 0;
        public float Volume_Music = 0;

        public bool InvertX = false;
        public bool InvertY = false;

        public float CamX_Sens = 1;
        public float CamY_Sens = 1;
    }
}
