using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class NogoMenu : MonoBehaviour
{
    [Header("References")]
    public Transform Arrow;
    public TextWritter Lore;
    public TextWritter LoreMoves;
    public Image Fade;
    public GameObject GlowEffect;
    public RawImage VidPlayerImage;
    public GameObject VideoPlayerObject;
    public UnityEngine.Video.VideoPlayer VidPlayer;
    public Image StagePreviewImage;
    public Sprite StageLockedImage;
    public string LockedStageMessage = "";
    public Text VXP_Amm;
    public Color BoughtItenTextColor = Color.green;

    [Header("Audio")]
    public AudioSource SoundMove;
    public AudioSource SoundBack;
    public AudioSource SoundClick;

    [Header("Menus")]
    public GameObject StageMenu;
    public GameObject VxpShop;
    public GameObject MovesMenu;
    public GameObject ComboMenu;
    public GameObject SpecialsMenu;
    public GameObject UpgradesMenu;
    public GameObject ItensMenu;

    [Header("Menu Itens")]
    public ValuesHolder[] StageMenu_Itens;
    public ValuesHolder[] StagesMenu_Itens_Only;
    public ValuesHolder[] Vxp_Itens;
    public ValuesHolder[] Moves_Itens;
    public ValuesHolder[] Combos_Itens;
    public ValuesHolder[] Specials_Itens;
    public ValuesHolder[] Upgrades_Itens;
    public ValuesHolder[] Itens_Itens;

    [Header("Cache")]
    public string PreviousMenu = "<Insert world map here>";
    public MenuExtensions MenuEx = new MenuExtensions();
    public MenuExtensions MenuEy = new MenuExtensions();
    //public Rewired.Player Inp;
    int inpx;
    int inpy;
    public Vector2 RawInput;
    public int difiIndex = 0;
    bool end = false;
    bool moved = true;
    bool selected = false;
    bool menuChanged = false;
    bool back = false;
    bool firsttime = true;
    bool vxp_changed = true;
    public int ItenY;
    public Vector3 ArrowPos;
    public GameProgress Data = new GameProgress();
    ProgressIten progIten;
    ProgressIten progIten2;
    ProgressIten progIten3;
    ProgressIten vxp_amm;
    int vxp_display;

    private void Start()
    {
        //Inp = Rewired.ReInput.players.GetPlayer(0);
        //Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 0).enabled = false;
        //Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 1).enabled = true;
        Fade.enabled = true;
        Fade.color = Color.black;
    }

    private void Update()
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

        //if (Inp.GetButtonDown("A")) { selected = true; }

        // GET SAVE DATA
        Data = SaveData.Data;

        // VXP SHOW
        if (vxp_changed)
        {
            vxp_amm = Data.FindIten(Data.StoryItens, "VXP");
            vxp_display = (int)Mathf.Lerp(vxp_display, vxp_amm.ammount, Time.deltaTime * 20);
            vxp_changed = false;
        }
        VXP_Amm.text = "VXP: " + vxp_display;

        // DEBUG, REMOVE LATER
        if (true)
        {
            if (Input.GetKey(KeyCode.U)) { vxp_amm.ammount += 10; }
            vxp_changed = true;
        }

        // MENU
        if (end == false)
        {
            // INPUT
            if (inpy == 1 || inpy == -1)
            {
                moved = true;
                ItenY += inpy;
                SoundMove.Play();
                inpy = 0;
            }

            // FADE IN
            Fade.color = Color.Lerp(Fade.color, new Color(0, 0, 0, 0), Time.deltaTime * 10);

            // MENUS
            if (StageMenu.activeSelf)
            {
                if (firsttime) // FIRST TIME
                {
                    // SET ICONS FOR STAGES
                    for (int i = 0; i < StagesMenu_Itens_Only.Length; i++)
                    {
                        progIten = Data.FindIten(Data.StoryItens, StagesMenu_Itens_Only[i].Strings[2]);
                        if (progIten != null)
                        {
                            if (progIten.unlocked == false)
                            {
                                StagesMenu_Itens_Only[i].transform.Find("LOCKED").gameObject.SetActive(true);
                            }
                            if (progIten.unChecked)
                            {
                                progIten.unChecked = false;
                                StagesMenu_Itens_Only[i].transform.Find("unchecked").gameObject.SetActive(true);
                            }

                            if (progIten.ammount == 1)
                            {
                                StagesMenu_Itens_Only[i].transform.Find("finished").gameObject.SetActive(true);
                            }
                            else if (progIten.ammount == 2)
                            {
                                StagesMenu_Itens_Only[i].transform.Find("bonus").gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            Debug.LogError("No stage save data found - LakeError " +
                                "(Iten that erroerd): " + StagesMenu_Itens_Only[i].Strings[2]);
                        }
                    }
                    firsttime = false;
                    VideoPlayerObject.SetActive(false);

                    Lore.CancelInvoke("Write");
                    Lore.OriginalText = StageMenu_Itens[ItenY].Strings[0];
                    Lore.Txt.text = ""; Lore.OnEnable();
                }

                // INDEX
                if (ItenY >= StageMenu_Itens.Length) { ItenY = 0; }
                else if (ItenY < 0) { ItenY = StageMenu_Itens.Length - 1; }

                // ARROW
                ArrowPos = StageMenu_Itens[ItenY].transform.Find("p").transform.position;
                Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, ArrowPos, Time.deltaTime * 20);

                if (ItenY == 0) // SHOP
                {
                    if (moved)
                    {
                        Lore.CancelInvoke("Write");
                        Lore.OriginalText = StageMenu_Itens[ItenY].Strings[0];
                        Lore.Txt.text = ""; Lore.OnEnable();
                        StagePreviewImage.sprite = StageMenu_Itens[ItenY].Images[0];
                    }

                    if (selected)
                    {
                        menuChanged = true;
                        SoundClick.Play();
                        StageMenu.SetActive(false);
                        VxpShop.SetActive(true);
                        VideoPlayerObject.SetActive(true);
                        GlowEffect.SetActive(true);
                    }
                }
                else if (ItenY >= 1 && ItenY <= StageMenu_Itens.Length - 1) // STAGES
                {
                    if (moved)
                    {
                        if (StageMenu_Itens[ItenY].transform.Find("LOCKED").gameObject.activeSelf)
                        {
                            Lore.CancelInvoke("Write");
                            Lore.OriginalText = LockedStageMessage;
                            Lore.Txt.text = ""; Lore.OnEnable();
                            StagePreviewImage.sprite = StageLockedImage;
                        }
                        else
                        {
                            Lore.CancelInvoke("Write");
                            Lore.OriginalText = StageMenu_Itens[ItenY].Strings[0];
                            Lore.Txt.text = ""; Lore.OnEnable();
                            StagePreviewImage.sprite = StageMenu_Itens[ItenY].Images[0];
                        }
                    }
                    if (selected)
                    {
                        if (StageMenu_Itens[ItenY].transform.Find("LOCKED").gameObject.activeSelf)
                        {
                            SoundBack.Play();
                        }
                        else
                        {
                            SoundClick.Play();
                            Music.Manager.StopMusic(1);
                            StartCoroutine(GoToSceneOrStage(StageMenu_Itens[ItenY].Strings[1], 1.5f));
                        }

                        GlowEffect.SetActive(true);
                    }
                }

                // GO BACK WITH B
                //if (Inp.GetButtonDown("B"))
                //{ StartCoroutine(GoToSceneOrStage("MENU - STORY MAIN MAP", 1.5f)); }

            }
            else if (VxpShop.activeSelf)
            {
                // START VXP
                if (menuChanged)
                {
                    ItenY = 0;
                    VidPlayerImage.gameObject.SetActive(false);
                    VideoPlayerObject.SetActive(true);
                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Vxp_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                    menuChanged = false;
                }

                // INDEX
                if (ItenY >= Vxp_Itens.Length) { ItenY = 0; }
                else if (ItenY < 0) { ItenY = Vxp_Itens.Length - 1; }

                // ARROW
                ArrowPos = Vxp_Itens[ItenY].transform.Find("p").transform.position;
                Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, ArrowPos, Time.deltaTime * 20);

                if (moved)
                {
                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Vxp_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                }

                if (ItenY == 0 && selected)
                {
                    menuChanged = true;
                    VxpShop.SetActive(false);
                    MovesMenu.SetActive(true);
                    SoundClick.Play();
                    GlowEffect.SetActive(true);
                }
                else if (ItenY == 1 && selected)
                {
                    menuChanged = true;
                    VxpShop.SetActive(false);
                    ComboMenu.SetActive(true);
                    SoundClick.Play();
                    GlowEffect.SetActive(true);
                }
                else if (ItenY == 2 && selected)
                {
                    menuChanged = true;
                    VxpShop.SetActive(false);
                    SpecialsMenu.SetActive(true);
                    SoundClick.Play();
                    GlowEffect.SetActive(true);
                }
                else if (ItenY == 3 && selected)
                {
                    menuChanged = true;
                    VxpShop.SetActive(false);
                    UpgradesMenu.SetActive(true);
                    SoundClick.Play();
                    GlowEffect.SetActive(true);
                }
                else if (ItenY == 4 && selected)
                {
                    menuChanged = true;
                    VxpShop.SetActive(false);
                    ItensMenu.SetActive(true);
                    SoundClick.Play();
                    GlowEffect.SetActive(true);
                }
                else if (ItenY == 5 && selected)
                {
                    ItenY = 0;
                    firsttime = true;
                    SoundBack.Play();
                    StageMenu.SetActive(true);
                    VxpShop.SetActive(false);
                    VideoPlayerObject.SetActive(false);
                    GlowEffect.SetActive(true);
                }

                // GO BACK WITH B
                //if (Inp.GetButtonDown("B"))
                //{
                //    ItenY = 0;
                //    firsttime = true;
                //    SoundBack.Play();
                //    StageMenu.SetActive(true);
                //    VxpShop.SetActive(false);
                //    VideoPlayerObject.SetActive(false);
                //    GlowEffect.SetActive(true);
                //}
            }
            else if (MovesMenu.activeSelf)
            {
                // START MOVES MENU
                if (menuChanged)
                {
                    ItenY = 0;
                    VidPlayerImage.gameObject.SetActive(true);
                    VideoPlayerObject.SetActive(true);
                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Moves_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                    menuChanged = false;
                    moved = true;

                    // SET PRICES
                    SetPrice();
                }

                void SetPrice()
                {
                    // COMBO BAR PRICE
                    progIten2 = Data.FindIten(Data.StoryItens, "Move_Combo");
                    if (progIten2.unlocked)
                    {
                        if (Moves_Itens[4].transform.Find("cost"))
                        {
                            Moves_Itens[4].transform.Find("cost").GetComponent<Text>().text = "BOUGHT!";
                            Moves_Itens[4].transform.Find("cost").GetComponent<Text>().color = BoughtItenTextColor;
                        }
                    }
                    else
                    {
                        if (Moves_Itens[4].transform.Find("cost"))
                        {
                            Moves_Itens[4].transform.Find("cost").GetComponent<Text>()
                                .text = Moves_Itens[4].Integers[0] + " VXP";
                        }
                    }

                }

                // INDEX
                if (ItenY >= Moves_Itens.Length) { ItenY = 0; }
                else if (ItenY < 0) { ItenY = Moves_Itens.Length - 1; }

                // BUY COMBO BAR
                if (ItenY == 4)
                {
                    if (moved)
                    {
                        progIten2 = Data.FindIten(Data.StoryItens, "Move_Combo");
                    }

                    if (progIten2.name == "Move_Combo")
                    {
                        if (selected && progIten2.unlocked == false)
                        {
                            if (vxp_amm.ammount > Moves_Itens[ItenY].Integers[0])
                            {
                                // BUY
                                SoundClick.Play();
                                vxp_amm.ammount -= Moves_Itens[ItenY].Integers[0];
                                progIten2.unlocked = true;
                                LoreMoves.WriteNewText("Item Acquired.");
                                SetPrice();
                            }
                            else
                            {
                                SoundBack.Play();
                                LoreMoves.WriteNewText("Not enough VXP.");
                                SetPrice();
                            }
                        }
                    }
                }

                // ARROW
                ArrowPos = Moves_Itens[ItenY].transform.Find("p").transform.position;
                Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, ArrowPos, Time.deltaTime * 20);

                // CHANGE STUFF ON MOVE
                if (moved)
                {
                    if (Moves_Itens[ItenY].Video.Count > 0 && Moves_Itens[ItenY].Video[0] != null)
                    {
                        VidPlayerImage.gameObject.SetActive(true);
                        VidPlayer.clip = Moves_Itens[ItenY].Video[0];
                    }
                    else
                    {
                        VidPlayerImage.gameObject.SetActive(false);
                    }

                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Moves_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();

                    // SET BOUGHT ITENS
                    progIten = Data.FindIten(Data.StoryItens, "Move_Combo");
                    if (progIten.unlocked)
                    {
                        if (Moves_Itens[5].transform.Find("cost"))
                        {
                            Moves_Itens[5].transform.Find("cost").GetComponent<Text>()
                                .text = "UNLOCKED";
                        }
                    }
                }

                // GO BACK WITH ITEN
                if (ItenY == Moves_Itens.Length - 1 && selected)
                {
                    ItenY = 0;
                    menuChanged = true;
                    firsttime = true;
                    SoundBack.Play();
                    VxpShop.SetActive(true);
                    MovesMenu.SetActive(false);
                    GlowEffect.SetActive(true);
                }

                // GO BACK WITH B
                //if (Inp.GetButtonDown("B"))
                //{
                //    ItenY = 0;
                //    menuChanged = true;
                //    firsttime = true;
                //    SoundBack.Play();
                //    VxpShop.SetActive(true);
                //    MovesMenu.SetActive(false);
                //    GlowEffect.SetActive(true);
                //}
            }
            else if (ComboMenu.activeSelf)
            {
                if (menuChanged)
                {
                    ItenY = 0;
                    VidPlayerImage.gameObject.SetActive(true);
                    VideoPlayerObject.SetActive(true);
                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Combos_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                    menuChanged = false;
                    moved = true;
                }

                // INDEX
                if (ItenY >= Combos_Itens.Length) { ItenY = 0; }
                else if (ItenY < 0) { ItenY = Combos_Itens.Length - 1; }

                // CHANGE STUFF ON MOVE
                if (moved)
                {
                    if (Combos_Itens[ItenY].Video.Count > 0 && Combos_Itens[ItenY].Video[0] != null)
                    {
                        VidPlayerImage.gameObject.SetActive(true);
                        VidPlayer.clip = Combos_Itens[ItenY].Video[0];
                    }
                    else
                    {
                        VidPlayerImage.gameObject.SetActive(false);
                    }

                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Combos_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                }

                // ARROW
                ArrowPos = Combos_Itens[ItenY].transform.Find("p").transform.position;
                Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, ArrowPos, Time.deltaTime * 20);

                // GO BAD WITH ITEN
                if (ItenY == Combos_Itens.Length - 1 && selected)
                {
                    ItenY = 0;
                    menuChanged = true;
                    firsttime = true;
                    SoundClick.Play();
                    VxpShop.SetActive(true);
                    ComboMenu.SetActive(false);
                    GlowEffect.SetActive(true);
                }

                // GO BACK WITH B
                //if (Inp.GetButtonDown("B"))
                //{
                //    ItenY = 0;
                //    menuChanged = true;
                //    firsttime = true;
                //    SoundBack.Play();
                //    VxpShop.SetActive(true);
                //    ComboMenu.SetActive(false);
                //    GlowEffect.SetActive(true);
                //}
            }
            else if (SpecialsMenu.activeSelf)
            {
                if (menuChanged)
                {
                    ItenY = 0;
                    VidPlayerImage.gameObject.SetActive(true);
                    VideoPlayerObject.SetActive(true);
                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Specials_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                    menuChanged = false;
                    moved = true;
                    SetPrices(Specials_Itens);
                }

                // INDEX
                if (ItenY >= Specials_Itens.Length) { ItenY = 0; }
                else if (ItenY < 0) { ItenY = Specials_Itens.Length - 1; }

                // CHANGE STUFF ON MOVE
                if (moved)
                {
                    if (Specials_Itens[ItenY].Video.Count > 0 && Specials_Itens[ItenY].Video[0] != null)
                    {
                        VidPlayerImage.gameObject.SetActive(true);
                        VidPlayer.clip = Specials_Itens[ItenY].Video[0];
                    }
                    else
                    {
                        VidPlayerImage.gameObject.SetActive(false);
                    }

                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Specials_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                    vxp_changed = true;
                }

                // BUY ITENS
                if (selected)
                {
                    if (ItenY < Specials_Itens.Length - 1 && Specials_Itens[ItenY].Strings.Count > 1)
                    {
                        progIten2 = progIten2 = Data.FindIten(Data.StoryItens, Specials_Itens[ItenY].Strings[1]);
                        if (progIten2.unlocked)
                        {
                            vxp_changed = true;
                            SoundBack.Play();
                            LoreMoves.WriteNewText("Item already acquired.");
                        }
                        else
                        {
                            if (vxp_amm.ammount >= Specials_Itens[ItenY].Integers[0])
                            {
                                SoundClick.Play();
                                vxp_amm.ammount -= Specials_Itens[ItenY].Integers[0];
                                progIten2.unlocked = true;
                                LoreMoves.WriteNewText("Item Acquired.");
                                SetPrices(Specials_Itens);
                                vxp_changed = true;
                            }
                            else
                            {
                                SoundBack.Play();
                                LoreMoves.WriteNewText("Not enough VXP.");
                            }
                        }
                    }
                }

                // ARROW
                ArrowPos = Specials_Itens[ItenY].transform.Find("p").transform.position;
                Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, ArrowPos, Time.deltaTime * 20);

                // GO BACK WITH ITEN
                if (ItenY == Specials_Itens.Length - 1 && selected)
                {
                    ItenY = 0;
                    menuChanged = true;
                    firsttime = true;
                    SoundClick.Play();
                    VxpShop.SetActive(true);
                    SpecialsMenu.SetActive(false);
                    GlowEffect.SetActive(true);
                }

                // GO BACK WITH B
                //if (Inp.GetButtonDown("B"))
                //{
                //    ItenY = 0;
                //    menuChanged = true;
                //    firsttime = true;
                //    SoundBack.Play();
                //    VxpShop.SetActive(true);
                //    SpecialsMenu.SetActive(false);
                //    GlowEffect.SetActive(true);
                //}
            }
            else if (UpgradesMenu.activeSelf)
            {
                if (menuChanged)
                {
                    ItenY = 0;
                    VidPlayerImage.gameObject.SetActive(false);
                    VideoPlayerObject.SetActive(true);
                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Upgrades_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                    menuChanged = false;
                    moved = true;

                    SetUpgradeValue();
                }

                // INDEX
                if (ItenY >= Upgrades_Itens.Length) { ItenY = 0; }
                else if (ItenY < 0) { ItenY = Upgrades_Itens.Length - 1; }

                // CHANGE STUFF ON MOVE
                if (moved)
                {
                    if (Upgrades_Itens[ItenY].Video.Count > 0 && Upgrades_Itens[ItenY].Video[0] != null)
                    {
                        VidPlayerImage.gameObject.SetActive(true);
                        VidPlayer.clip = Upgrades_Itens[ItenY].Video[0];
                    }
                    else
                    {
                        VidPlayerImage.gameObject.SetActive(false);
                    }

                    // LORE AND LEVEL
                    if (ItenY < Upgrades_Itens.Length - 1 && Upgrades_Itens[ItenY].Strings.Count > 1)
                    {
                        progIten2 = Data.FindIten(Data.StoryItens, Upgrades_Itens[ItenY].Strings[1]);
                        LoreMoves.WriteNewText("[ Lv:" + progIten2.ammount + "] " + Upgrades_Itens[ItenY].Strings[0]);
                    }
                    else
                    {
                        LoreMoves.WriteNewText(Upgrades_Itens[ItenY].Strings[0]);
                    }

                    vxp_changed = true;
                    SetUpgradeValue();
                }

                if (selected && Upgrades_Itens[ItenY].Strings.Count > 1)
                {
                    // LEVEL UP
                    progIten2 = Data.FindIten(Data.StoryItens, Upgrades_Itens[ItenY].Strings[1]);
                    if ((int)progIten2.ammount < Upgrades_Itens[ItenY].Integers.Count)
                    {
                        if (vxp_amm.ammount >= Upgrades_Itens[ItenY].Integers[(int)progIten2.ammount])
                        {
                            vxp_amm.ammount -= Upgrades_Itens[ItenY].Integers[(int)progIten2.ammount];
                            progIten2.ammount += 1;
                            SoundClick.Play();
                            SetUpgradeValue();
                            GlowEffect.SetActive(true);
                            LoreMoves.WriteNewText("Power increased." + " [ Lv: " + progIten2.ammount + " ]");
                            vxp_changed = true;
                            moved = true;
                        }
                        else
                        {
                            LoreMoves.WriteNewText("Not enough VXP.");
                            SoundBack.Play();
                        }
                    }
                    else
                    {
                        LoreMoves.WriteNewText("We cannot go any further at the moment.");
                        SoundBack.Play();
                    }
                }

                void SetUpgradeValue()
                {
                    Text cost;
                    Transform pos;

                    // HP
                    progIten3 = Data.FindIten(Data.StoryItens, "Shell_HP");
                    pos = Upgrades_Itens[0].transform.Find("cost");
                    cost = pos.GetComponent<Text>();
                    if ((int)progIten3.ammount < Upgrades_Itens[0].Integers.Count - 1)
                    { cost.text = Upgrades_Itens[0].Integers[(int)progIten3.ammount] + " VXP"; }
                    else { cost.text = "MAX"; }

                    // AP
                    progIten3 = Data.FindIten(Data.StoryItens, "Shell_AP");
                    pos = Upgrades_Itens[1].transform.Find("cost");
                    cost = pos.GetComponent<Text>();
                    if ((int)progIten3.ammount < Upgrades_Itens[1].Integers.Count - 1)
                    { cost.text = Upgrades_Itens[1].Integers[(int)progIten3.ammount] + " VXP"; }
                    else { cost.text = "MAX"; }

                    // POISE
                    progIten3 = Data.FindIten(Data.StoryItens, "Shell_POISE");
                    pos = Upgrades_Itens[2].transform.Find("cost");
                    cost = pos.GetComponent<Text>();
                    if ((int)progIten3.ammount < Upgrades_Itens[2].Integers.Count)
                    { cost.text = Upgrades_Itens[2].Integers[(int)progIten3.ammount] + " VXP"; }
                    else { cost.text = "MAX"; }

                    // POISE
                    progIten3 = Data.FindIten(Data.StoryItens, "Shell_EN");
                    pos = Upgrades_Itens[3].transform.Find("cost");
                    cost = pos.GetComponent<Text>();
                    if ((int)progIten3.ammount < Upgrades_Itens[3].Integers.Count)
                    { cost.text = Upgrades_Itens[3].Integers[(int)progIten3.ammount] + " VXP"; }
                    else { cost.text = "MAX"; }

                }

                // ARROW
                ArrowPos = Upgrades_Itens[ItenY].transform.Find("p").transform.position;
                Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, ArrowPos, Time.deltaTime * 20);

                // GO BACK WITH ITEN
                if (ItenY == Upgrades_Itens.Length - 1 && selected)
                {
                    ItenY = 0;
                    menuChanged = true;
                    firsttime = true;
                    SoundClick.Play();
                    VxpShop.SetActive(true);
                    UpgradesMenu.SetActive(false);
                    GlowEffect.SetActive(true);
                }

                // GO BACK WITH B
                //if (Inp.GetButtonDown("B"))
                //{
                //    ItenY = 0;
                //    menuChanged = true;
                //    firsttime = true;
                //    SoundBack.Play();
                //    VxpShop.SetActive(true);
                //    UpgradesMenu.SetActive(false);
                //    GlowEffect.SetActive(true);
                //}

            }
            else if (ItensMenu.activeSelf)
            {
                if (menuChanged)
                {
                    ItenY = 0;
                    VidPlayerImage.gameObject.SetActive(true);
                    VideoPlayerObject.SetActive(true);
                    LoreMoves.CancelInvoke("Write");
                    LoreMoves.OriginalText = Itens_Itens[ItenY].Strings[0];
                    LoreMoves.Txt.text = ""; LoreMoves.OnEnable();
                    menuChanged = false;
                    moved = true;
                    SetPrices(Itens_Itens);
                }

                // INDEX
                if (ItenY >= Itens_Itens.Length) { ItenY = 0; }
                else if (ItenY < 0) { ItenY = Itens_Itens.Length - 1; }

                // ARROW
                ArrowPos = Itens_Itens[ItenY].transform.Find("p").transform.position;
                Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, ArrowPos, Time.deltaTime * 20);

                if (moved)
                {
                    if (Upgrades_Itens[ItenY].Video.Count > 0 && Upgrades_Itens[ItenY].Video[0] != null)
                    {
                        VidPlayerImage.gameObject.SetActive(true);
                        VidPlayer.clip = Upgrades_Itens[ItenY].Video[0];
                    }
                    else
                    {
                        VidPlayerImage.gameObject.SetActive(false);
                    }

                    // LORE AND LEVEL
                    if (Itens_Itens[ItenY].Strings.Count > 1)
                    {
                        progIten2 = Data.FindIten(Data.StoryItens, Itens_Itens[ItenY].Strings[1]);
                        LoreMoves.WriteNewText("[Currently Owned: " + progIten2.ammount + " ] " + Itens_Itens[ItenY].Strings[0]);
                    }
                    else
                    {
                        LoreMoves.WriteNewText(Itens_Itens[ItenY].Strings[0]);
                    }

                    vxp_changed = true;
                }

                if (selected)
                {
                    if (Itens_Itens[ItenY].Integers.Count >= 1)
                    {
                        if (vxp_amm.ammount >= Itens_Itens[ItenY].Integers[0])
                        {
                            SoundClick.Play();
                            GlowEffect.SetActive(true);
                            progIten2 = Data.FindIten(Data.StoryItens, Itens_Itens[ItenY].Strings[1]);
                            progIten2.ammount += 1;
                            vxp_amm.ammount -= Itens_Itens[ItenY].Integers[0];
                            vxp_changed = true;
                            LoreMoves.WriteNewText("Item acquired. Total: [ " + (int)progIten2.ammount + " ]");
                        }
                        else
                        {
                            SoundBack.Play();
                            LoreMoves.WriteNewText("Not enough VXP.");
                        }
                    }
                }

                // GO BACK WITH ITEN
                if (ItenY == Itens_Itens.Length - 1 && selected)
                {
                    ItenY = 0;
                    menuChanged = true;
                    firsttime = true;
                    SoundClick.Play();
                    VxpShop.SetActive(true);
                    ItensMenu.SetActive(false);
                    GlowEffect.SetActive(true);
                }

                // GO BACK WITH B
                //if (Inp.GetButtonDown("B"))
                //{
                //    ItenY = 0;
                //    menuChanged = true;
                //    firsttime = true;
                //    SoundBack.Play();
                //    VxpShop.SetActive(true);
                //    ItensMenu.SetActive(false);
                //    GlowEffect.SetActive(true);
                //}

            }

            // FINAL SETS
            moved = false;
            selected = false;
        }
        else // END MENU
        {
            Fade.gameObject.SetActive(true);
            Fade.enabled = true;
            Fade.color = Color.Lerp(Fade.color, new Color(0, 0, 0, 1), Time.deltaTime * 4);
        }
    }

    void SetPrices(ValuesHolder[] values)
    {
        // COMBO BAR PRICE
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i].Strings.Count > 1)
            {
                progIten2 = Data.FindIten(Data.StoryItens, values[i].Strings[1]);
                if (progIten2.unlocked)
                {
                    if (values[i].transform.Find("cost"))
                    { 
                        values[i].transform.Find("cost").GetComponent<Text>().text = "BOUGHT!";
                        values[i].transform.Find("cost").GetComponent<Text>().color = BoughtItenTextColor;
                    }
                }
                else
                {
                    if (values[i].transform.Find("cost"))
                    {
                        values[i].transform.Find("cost").GetComponent<Text>().text = values[i].Integers[0] + " VXP";
                    }
                }
            }
        }
    }

    IEnumerator GoToSceneOrStage(string stage, float time)
    {
        SoundBack.Play();
        end = true;
        yield return new WaitForSeconds(time);
        SceneController.LoadStageLoading(stage);
    }

}
