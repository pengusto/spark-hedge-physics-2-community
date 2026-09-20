using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InputAssMenu : MonoBehaviour
{

    //[Header("References")]
    //public List<Transform> MenuItens;
    //public List<ValuesHolder> Controllers;
    //public Transform P1_Pos;
    //public Transform P2_Pos;
    //public Transform NONE_Pos;
    //public Transform MenuArrow;
    //public GameObject PauseUI;
    //public GameObject SettingsUI;

    //[Header("Values")]
    //public Color NormalIconColor = Color.white;
    //public Color TriggeredIconColor = Color.yellow;

    //[Header("Audio")]
    //public AudioSource SoundMove;
    //public AudioSource SoundBack;
    //public AudioSource SoundClick;

    //// CACHE
    //public MenuExtensions MenuEx = new MenuExtensions();
    //public MenuExtensions MenuEy = new MenuExtensions();
    //public Rewired.Player Rewinp1;
    //public Rewired.Player Rewinp2;
    //public Rewired.Player Rewinp3;
    //public Rewired.Player Sys;
    //public Vector2 RawInput;
    //public int ItenY = 7;
    //int inpx;
    //int inpy;
    //int ControllerCount = -11;

    //public void Start()
    //{
    //    Rewinp1 = Rewired.ReInput.players.GetPlayer(0);
    //    Rewinp2 = Rewired.ReInput.players.GetPlayer(1);
    //    Rewinp3 = Rewired.ReInput.players.GetPlayer(2);
    //    Sys = Rewired.ReInput.players.SystemPlayer;
    //    MenuArrow.gameObject.SetActive(true);

    //    SetControllersUI();
    //    AssingControllers();
    //}

    //private void Update()
    //{
    //    RawInput = new Vector2(Sys.GetAxis("LeftAnalogX"), -Sys.GetAxis("LeftAnalogY"));
    //    RawInput += new Vector2(Rewinp1.GetAxis("LeftAnalogX"), -Rewinp1.GetAxis("LeftAnalogY"));
    //    RawInput += new Vector2(Rewinp2.GetAxis("LeftAnalogX"), -Rewinp2.GetAxis("LeftAnalogY"));

    //    if (Sys.GetButton("D_Right")) { RawInput.x++; }
    //    if (Sys.GetButton("D_Left")) { RawInput.x--; }
    //    if (Sys.GetButton("D_Up")) { RawInput.y--; }
    //    if (Sys.GetButton("D_Down")) { RawInput.y++; }

    //    if (Rewinp1.GetButton("D_Right")) { RawInput.x++; }
    //    if (Rewinp1.GetButton("D_Left")) { RawInput.x--; }
    //    if (Rewinp1.GetButton("D_Up")) { RawInput.y--; }
    //    if (Rewinp1.GetButton("D_Down")) { RawInput.y++; }

    //    if (Rewinp2.GetButton("D_Right")) { RawInput.x++; }
    //    if (Rewinp2.GetButton("D_Left")) { RawInput.x--; }
    //    if (Rewinp2.GetButton("D_Up")) { RawInput.y--; }
    //    if (Rewinp2.GetButton("D_Down")) { RawInput.y++; }

    //    RawInput.x = Mathf.Clamp(RawInput.x, -1, 1);
    //    RawInput.y = Mathf.Clamp(RawInput.y, -1, 1);
    //    inpx = MenuEx.MenuMovement(RawInput.x);
    //    inpy = MenuEy.MenuMovement(RawInput.y);

    //    // UP AND DOWN MOVE
    //    if (inpy == 1 || inpy == -1)
    //    {
    //        ItenY += inpy;
    //        // SKIP DISABLED ITEN
    //        int skip = 0;
    //        while (MenuItens[Mathf.Clamp(ItenY, 0, MenuItens.Count - 1)].gameObject.activeSelf == false && skip < 100)
    //        {
    //            skip++;
    //            ItenY += inpy;
    //        }

    //        SoundMove.Play();
    //        inpy = 0;
    //    }

    //    if (ItenY >= MenuItens.Count) { ItenY = 0; }
    //    else if (ItenY < 0) { ItenY = MenuItens.Count - 1; }

    //    // MOVE SIDEWAYS
    //    if (inpx == 1 || inpx == -1)
    //    {
    //        if (MenuItens[ItenY].GetComponent<ValuesHolder>())
    //        {
    //            if(inpx == -1) 
    //            { 
    //                Rewinp1.controllers.AddController(MenuItens[ItenY].GetComponent<ValuesHolder>().controller, true);
    //                SetControllersUI();
    //                AssingControllers();
    //            }
    //            else if(inpx == 1) 
    //            { 
    //                Rewinp2.controllers.AddController(MenuItens[ItenY].GetComponent<ValuesHolder>().controller, true);
    //                SetControllersUI();
    //                AssingControllers();
    //            }

    //            SoundMove.Play();
    //            inpx = 0;
    //        }
    //    }

    //    if(ItenY == 7)
    //    {
    //        if (Sys.GetButtonDown("A") || Rewinp1.GetButtonDown("A"))
    //        {
    //            this.gameObject.SetActive(false);
    //            if (PauseUI) { PauseUI.SetActive(false); }
    //            SettingsUI.SetActive(true);
    //            SoundClick.Play();
    //        }
    //    }

    //    // END
    //    if (Sys.GetButtonDown("B") || Rewinp1.GetButtonDown("B"))
    //    {
    //        this.gameObject.SetActive(false);
    //        PauseUI.SetActive(false);
    //        SettingsUI.SetActive(true);
    //        gameObject.SetActive(false);
    //        SoundBack.Play();
    //    }

    //    // ARROW
    //    MenuArrow.position = Vector3.Lerp(MenuArrow.position, MenuItens[ItenY].transform.position, Time.unscaledTime * 35);

    //    // INPUT TESTING, WILL CHANGE THE COLOR OF THE CONTROLLER ON SCREEN
    //    Rewired.Controller c;
    //    for (int i = 0; i < Controllers.Count; i++)
    //    {
    //        IEnumerator e = Rewinp1.controllers.Controllers.GetEnumerator();
    //        while (e.MoveNext())
    //        {
    //            c = (Rewired.Controller)e.Current;
    //            if (c.name == Controllers[i].Strings[0])
    //            {
    //                if (c.GetAnyButton())
    //                {
    //                    Controllers[i].Ui_images[0].color = TriggeredIconColor;
    //                }
    //                else
    //                {
    //                    Controllers[i].Ui_images[0].color = NormalIconColor;
    //                }
    //            }
    //        }

    //        e = Rewinp2.controllers.Controllers.GetEnumerator();
    //        while (e.MoveNext())
    //        {
    //            c = (Rewired.Controller)e.Current;
    //            if (c.name == Controllers[i].Strings[0])
    //            {
    //                if (c.GetAnyButton())
    //                {
    //                    Controllers[i].Ui_images[0].color = TriggeredIconColor;
    //                }
    //                else
    //                {
    //                    Controllers[i].Ui_images[0].color = NormalIconColor;
    //                }
    //            }
    //        }
    //    }

    //    // CHECK IF NEW CONTROLLER
    //    if(Rewired.ReInput.controllers.controllerCount != ControllerCount)
    //    {
    //        FakeUpdate();
    //        ControllerCount = Rewired.ReInput.controllers.controllerCount;
    //    }
    //}

    //void SetControllersUI()
    //{
    //    // DISABLE EVERYTHING TO ENABLE LATER, ALSO SET THEM ALL TO MIDDLE
    //    for (int i = 0; i < Controllers.Count; i++)
    //    {
    //        Controllers[i].gameObject.SetActive(false);
    //        Vector3 p = Controllers[i].GetComponent<RectTransform>().position;
    //        p.x = NONE_Pos.GetComponent<RectTransform>().position.x;
    //        Controllers[i].GetComponent<RectTransform>().position = p;
    //    }

    //    // SKIP IF MOUSE, THEN SET OTHER GAMEPADS (ALSO SET ICONS AND TEXT)
    //    for (int i = 0; i < Rewired.ReInput.controllers.Controllers.Count; i++)
    //    {   
    //        if(Rewired.ReInput.controllers.Controllers[i].type == Rewired.ControllerType.Mouse) 
    //        { 

    //        }
    //        else if (Rewired.ReInput.controllers.Controllers[i].type == Rewired.ControllerType.Keyboard)
    //        {
    //            //Controllers[i].gameObject.SetActive(true);
    //            //Controllers[i].Ui_images[0].enabled = true;
    //            //Controllers[i].Ui_images[1].enabled = false;
    //            //Controllers[i].Ui_text[0].text = Rewired.ReInput.controllers.Controllers[i].name;
    //            //Controllers[i].Floats[0] = Rewired.ReInput.controllers.Controllers[i].id + 1;
    //            //Controllers[i].Strings[0] = Rewired.ReInput.controllers.Controllers[i].name;
    //            //Controllers[i].controller = Rewired.ReInput.controllers.Controllers[i];
    //        }
    //        else
    //        {
    //            Controllers[i].gameObject.SetActive(true);
    //            Controllers[i].Ui_images[0].enabled = true;
    //            Controllers[i].Ui_images[1].enabled = false;
    //            Controllers[i].Ui_text[0].text = Rewired.ReInput.controllers.Controllers[i].name;
    //            Controllers[i].Floats[0] = Rewired.ReInput.controllers.Controllers[i].id + 1;
    //            Controllers[i].Strings[0] = Rewired.ReInput.controllers.Controllers[i].name;
    //            Controllers[i].controller = Rewired.ReInput.controllers.Controllers[i];
    //        }
    //    }

    //    // SET SIDES
    //    Rewired.Controller c;
    //    for (int i = 0; i < Controllers.Count; i++)
    //    {
    //        // FOR PLAYER 1, CHECK ID
    //        IEnumerator e = Rewinp1.controllers.Controllers.GetEnumerator();
    //        while (e.MoveNext())
    //        {
    //            c = (Rewired.Controller)e.Current;
    //            if (c.name == Controllers[i].Strings[0])
    //            {
    //                Vector3 p = Controllers[i].GetComponent<RectTransform>().position;
    //                p.x = P1_Pos.GetComponent<RectTransform>().position.x;
    //                Controllers[i].GetComponent<RectTransform>().position = p;
    //            }
    //        }

    //        e = Rewinp2.controllers.Controllers.GetEnumerator();
    //        while (e.MoveNext())
    //        {
    //            c = (Rewired.Controller)e.Current;
    //            if (c.name == Controllers[i].Strings[0])
    //            {
    //                Vector3 p = Controllers[i].GetComponent<RectTransform>().position;
    //                p.x = P2_Pos.GetComponent<RectTransform>().position.x;
    //                Controllers[i].GetComponent<RectTransform>().position = p;
    //            }       
    //        }
    //    }


    //}

    //void AssingControllers()
    //{
    //    Rewinp1.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 0).enabled = false;
    //    Rewinp1.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 1).enabled = true;

    //    // SET ALL CONTROLLERS TO SYSTEM AS WELL
    //    //for (int i = 0; i < Rewired.ReInput.controllers.Controllers.Count; i++)
    //    //{
    //    //   Sys.controllers.AddController(Rewired.ReInput.controllers.Controllers[i], false);
    //    //}

    //    //Sys.controllers.AddController(Rewired.ReInput.controllers.Keyboard, false);
    //    //Sys.controllers.AddController(Rewired.ReInput.controllers.Mouse, false);
    //}

    //void FakeUpdate()
    //{
    //    Debug.Log("INPUT ASSIGN: FAKE UPDATE TRIGGER");
    //    SetControllersUI();
    //    AssingControllers();
    //}
}
