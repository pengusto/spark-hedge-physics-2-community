using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject CarUI;
    public GameObject CharacterUI;
    public CharacterCamera CharCamera;
    int PreviousMenu;
    public GameObject PauseUI;
    public GameObject GraphicsUI;
    public GameObject SettingsUI;
    public OptionsMenu SettingsScript;
    public Transform Arrow;
    public MenuExtensions.MenuIten[] MenuItens;
    public Image Fade;
    public AudioSource SoundMove;
    public AudioSource SoundBack;
    public AudioSource SoundClick;
    public Text RestartTries;
    public UnityEngine.Audio.AudioMixer CarSounds;


    [Header("Misc")]
    public bool RaceMode = false;
    public Text RestartText;
    public Text RestartTriesText;
    public Text RestartCupText;
    public Text QuitToMenuText;

    [Header("Cache")]
    MenuExtensions MenuEx = new MenuExtensions();
    public bool OnSubMenu = false;
    [HideInInspector] public int Iten = 0;
    float inpy;
    bool eventOn = false;
    float fadeCounter = 0;
    float PauseCounter = 0;
    bool restarting = false;
    bool resetTourney = false;
    float carsvol = 0;
    Vector3 RawInput;
    public bool DisableRestartRace = false;
    public bool DisableRestartTourney = false;
    public bool EnableReturnToMenu = true;
    PitRespawner CharacterRespawner;
    bool fadeout = false;

    // CACHE
    //public Rewired.Player Inp;

    private void Start()
    {
        //Inp = Rewired.ReInput.players.GetPlayer(0);
        PauseUI.SetActive(false);
    }

    //private void Update()
    //{
    //    // COUNTER
    //    PauseCounter += Time.unscaledDeltaTime;

    //    // EVENTS
    //    if (eventOn)
    //    {
    //        if (restarting) { Restarting(); }
    //        return;
    //    }

    //    // ENABLE DISABLE PAUSE
    //    if (Inp.GetButtonDown("Start") && PauseCounter > 0.5f)
    //    {
    //        // CLICK
    //        if (!OnSubMenu)
    //        {
    //            if (!PauseUI.activeSelf) // OPEN PAUSE
    //            {
    //                Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 0).enabled = false;
    //                Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 1).enabled = true;
    //                Arrow.gameObject.SetActive(true);
    //                SoundClick.Play();
    //                PauseUI.SetActive(true);

    //                if (CarUI.activeSelf) { PreviousMenu = 0; }
    //                if (CharacterUI.activeSelf) { PreviousMenu = 1; }

    //                CarUI.SetActive(false);
    //                CharacterUI.SetActive(false);
    //                Time.timeScale = 0;
    //            }
    //            else if (PauseUI.activeSelf)
    //            {
    //                OnSubMenu = false;
    //                DisablePauseMenu();
    //            }
    //        }
    //        else
    //        {
    //            if (GraphicsUI.activeSelf)
    //            {
    //                OnSubMenu = false;
    //                DisablePauseMenu();
    //            }
    //        }
    //    }

    //    // PAUSE MENU CODE
    //    if (PauseUI.activeSelf)
    //    {
    //        // GO BACK WITH THE B BUTTON
    //        if (Inp.GetButtonDown("B")) { OnSubMenu = false; DisablePauseMenu(); }

    //        RawInput = new Vector2(Inp.GetAxis("LeftAnalogX"), -Inp.GetAxis("LeftAnalogY"));
    //        if (Inp.GetButton("D_Right")) { RawInput.x++; }
    //        if (Inp.GetButton("D_Left")) { RawInput.x--; }
    //        if (Inp.GetButton("D_Up")) { RawInput.y--; }
    //        if (Inp.GetButton("D_Down")) { RawInput.y++; }
    //        RawInput.x = Mathf.Clamp(RawInput.y, -1, 1);
    //        inpy = MenuEx.MenuMovement(RawInput.y);

    //        // UP AND DOWN MOVE
    //        if (inpy == 1)
    //        {
    //            Iten++;
    //            if (Iten >= MenuItens.Length) { Iten = 0; }
    //            else if (Iten < 0) { Iten = MenuItens.Length - 1; }
    //            SoundMove.Play();
    //            inpy = 0;
    //        }
    //        else if (inpy == -1)
    //        {
    //            Iten--;
    //            if (Iten >= MenuItens.Length) { Iten = 0; }
    //            else if (Iten < 0) { Iten = MenuItens.Length - 1; }
    //            SoundMove.Play();
    //            inpy = 0;
    //        }

    //        if (Iten == 0) // RESUME
    //        {
    //            if (Inp.GetButtonDown("A"))
    //            {
    //                SoundClick.Play();
    //                DisablePauseMenu();
    //            }
    //        }
    //        else if(Iten == 1) // RESTART
    //        {
    //            if (Inp.GetButtonDown("A") && !DisableRestartRace)
    //            {
    //                if (RaceMode)
    //                {

    //                }
    //                else
    //                {
    //                    eventOn = true;
    //                    restarting = true;
    //                    SoundClick.Play();
    //                }
    //            }
    //        }
    //        else if (Iten == 2) // RESTART CUP
    //        {
    //            if (Inp.GetButtonDown("A") && !DisableRestartTourney)
    //            {
    //                if (RaceMode)
    //                {
    //                    SoundClick.Play();
    //                    eventOn = true;
    //                    restarting = true;
    //                    resetTourney = true;
    //                }
    //                else
    //                {
    //                    SoundBack.Play();
    //                    // TODO LATER hehe
    //                }
    //            }
    //        }
    //        else if (Iten == 3) // SETTINGS
    //        {
    //            if (Inp.GetButtonDown("A"))
    //            {
    //                OnSubMenu = true;
    //                PauseUI.SetActive(false);
    //                SettingsUI.SetActive(true);
    //                SoundClick.Play();
    //            }
    //        }
    //        else if (Iten == 4) // GRAPHICS SETTINGS
    //        {
    //            if (Inp.GetButtonDown("A"))
    //            {
    //                OnSubMenu = true;
    //                PauseUI.SetActive(false);
    //                GraphicsUI.SetActive(true);
    //                SoundClick.Play();
    //            }
    //        }
    //        else if (Iten == 5) // QUIT MENU
    //        {
    //            if (Inp.GetButtonDown("A"))
    //            {
    //                if (EnableReturnToMenu)
    //                {
    //                    if (StoryData.Instance.StoryMode == false)
    //                    {
    //                        SoundClick.Play();
    //                        Music.Manager.StopMusic(1);
    //                        SceneController.LoadStageLoading("MENU - MAIN MENU");
    //                    }
    //                    else
    //                    {

    //                    }
    //                }
    //                else
    //                {
    //                    SoundBack.Play();
    //                }
    //            }
    //        }
    //        else if (Iten == 6) // QUIT GAME
    //        {
    //            if (Inp.GetButton("A"))
    //            {
    //                MenuItens[Iten].HoldCircle.fillAmount += Time.unscaledDeltaTime;
    //                if (MenuItens[Iten].HoldCircle.fillAmount > 0.98f) { Application.Quit(); }
    //            }
    //        }

    //        for (int i = 0; i < MenuItens.Length; i++)
    //        {
    //            if (MenuItens[i].HoldCircle) 
    //            {
    //                if (MenuItens[i].HoldCircle.fillAmount > 0)
    //                {
    //                    MenuItens[i].HoldCircle.fillAmount -= Time.unscaledDeltaTime * 0.2f;
    //                }
    //            }
    //        }

    //        // EXTRA PAUSED STUFF
    //        if (CarSounds)
    //        {
    //            carsvol = Mathf.Lerp(carsvol, -80, Time.unscaledDeltaTime * 2);
    //            CarSounds.SetFloat("CarsParentVolume", carsvol);
    //        }

    //        // MOVE ARROW
    //        Arrow.position = Vector3.Lerp(Arrow.position, MenuItens[Iten].Position.position, Time.unscaledDeltaTime * 30);
    //    }

    //    // FADE
    //    if (fadeout && fadeCounter > 0.0f)
    //    {
    //        if (fadeCounter <= 0.0f) 
    //        { 
    //            fadeout = false; 
    //            fadeCounter = 0; 
    //            Fade.enabled = false; 
    //        }

    //        fadeCounter -= Time.unscaledDeltaTime * 2;
    //        Fade.color = new Color(0, 0, 0, fadeCounter);
    //        fadeCounter = Mathf.Clamp01(fadeCounter);
    //    }
    //}

    //public void DisablePauseMenu()
    //{
    //    if (PreviousMenu == 0)
    //    {
    //        Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 0).enabled = true;
    //        Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 1).enabled = false;
    //        Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 2).enabled = false;
    //    }
    //    else
    //    {
    //        Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 0).enabled = false;
    //        Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 1).enabled = false;
    //        Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 2).enabled = true;
    //    }

    //    Arrow.gameObject.SetActive(false);
    //    SoundBack.Play();
    //    GraphicsUI.SetActive(false);
    //    PauseUI.SetActive(false);
    //    OnSubMenu = false;


    //    if (PreviousMenu == 0) { CarUI.SetActive(true); }
    //    else if(PreviousMenu == 1) { CharacterUI.SetActive(true); }
    //    Time.timeScale = 1;
    //    carsvol = 0;
    //    CarSounds.SetFloat("CarsParentVolume", carsvol);
    //}

    private void OnEnable()
    {
        OnSubMenu = false;
    }

    public void Restarting()
    {
        //if(fadeCounter > 1.1f)
        //{      
        //    if (RaceMode)
        //    {
        //        Time.timeScale = 1;
        //        carsvol = 0;
        //        CarSounds.SetFloat("CarsParentVolume", carsvol);
        //        if (resetTourney)
        //        {
        //            SceneController.LoadStageLoading(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        //        }
        //        else
        //        {
        //            SceneController.LoadStageLoading(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        //        }
        //    }
        //    else
        //    {
        //        Time.timeScale = 1;
        //        PitRespawner pit;
        //        CharacterActions act;
        //        CharCamera.Char.TryGetComponent<CharacterActions>(out act);
        //        if (CharCamera.Char.TryGetComponent<PitRespawner>(out pit))
        //        {
        //            CharCamera.Char.rigid.position = pit.checkpoint.position;
        //            // ADD A THING THAT MAKES YOU LOOK AT THE CHECKPOINT DIRECTION

        //            CharCamera.Char.rigid.linearVelocity = Vector3.zero;

        //            // CHAR ACTIONS
        //            if (act)
        //            {
        //                act.SwitchAction(0);
        //                act.Basic.SubAction = 0;
        //                act.Basic.SubActionTime = 0;
        //            }

        //            // END RESET
        //            eventOn = false;
        //            restarting = false;
        //            fadeout = true;
        //            DisablePauseMenu();
        //        }
        //    }
        //}
        //else
        //{
        //    Fade.enabled = true;
        //    fadeCounter += Time.unscaledDeltaTime * 2;
        //    Fade.color = new Color(0, 0, 0, fadeCounter);
        //}
    }

    public void NightRaceMode()
    {
        DisableRestartRace = true;
        DisableRestartTourney = true;
        //EnableReturnToMenu = false;

        RestartText.text = "---";
        RestartTriesText.text = " ";
        RestartCupText.text = "---";
        QuitToMenuText.text = "QUIT RACE";
    }
}
