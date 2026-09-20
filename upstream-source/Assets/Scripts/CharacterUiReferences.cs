using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUiReferences : MonoBehaviour
{
    [Header("Main")]
    public GameObject StatsBars;
    public GameObject StatsHpBar;
    public GameObject StatsApBar;
    public GameObject StatsEnBar;
    public GameObject StatsComboBar;
    public CharacterCardsInfo Cards;

    [Header("Child Objects")]
    public Image Hp_Bar;
    public Image Hp_BarBelow;
    public Text Hp_Text;
    public GameObject MultipleHP_Bars_Object;
    public Text MutipleHPBars_Amm;
    public RectTransform Hp_BarScale;
    public Image Ap_Bar;
    public Image Ap_BarBelow;
    public RectTransform Ap_BarScale;
    public RectTransform Ap_PoiseIcon;
    public RectTransform Ap_PoiseEnd;
    public Image En_Bar;
    public Image En_BarBelow;
    public Text En_Text;
    public RectTransform En_BarScale;
    public Image Combo_Bar;
    public Text Combo_Text;
    public GameObject EnemyBar_Object;
    public Image Enemy_Bar;
    public Image Enemy_BarBelow;
    public Text Enemy_Bar_Name;

    [Header("Score Objects")]
    public Color CurrentScoreColor = Color.white;
    public GameObject ScoreObject;
    public Gradient ColorOverTime;
    public Text ScoreText;
    public Text ScoreNumber;
    public Image ScoreBar;
    public Text ScoreMultiplierText;
    public Text ScoreMultiplierNumber;
    public Transform ScoreNotifPosition;
    public GameObject ScoreNotifPrefab;
    public float ScoreNotifOffset = -33.3f;

    [Header("FX")]
    public Image HurtRed;
    public float HurtRedAlpha = 0.7f;
    public Image StrikeImage;
    public Animator FallFade;
    public Animator UseMultipleHP_Animator;
    public AudioSource UseMultipleHpBarSFX;

    [Header("Specials")]
    public GameObject SpecialsButtons;
    public Color SpcUsableColor = Color.white;
    public Color SpcUnusableColor = Color.gray;
    public Color SpecialQuickMovesColor = Color.black;
    public Color SpecialHardMovesColor = Color.black;
    public List<Image> SpecialBackgrounds;
    public List<SpecialUiIcon> SpecialUiIcons;

    [Header("Pause Hijack")]
    public bool EnableHijack = false;
    public PauseMenu Pause;
    public Text RestartRaceText;
    public Text RestartRaceTextNum;
    public Text RestartTourneyText;
    public Text QuitToMenuText;

    // CACHE
    [HideInInspector] public Color HurtUiColorInitial;
    [HideInInspector] public Color HurtUiColorEnd;
    [HideInInspector] public float StrikeCounter;
    bool firsttime = true;

    private void Start()
    {
        if (HurtRed) 
        {
            HurtUiColorInitial = HurtRed.color;
            HurtUiColorInitial.a = HurtRedAlpha;
            HurtUiColorEnd = HurtRed.color;
            HurtUiColorEnd.a = 0;
        }

        if (EnableHijack)
        {
            Pause.DisableRestartRace = true;
            Pause.DisableRestartTourney = true;
            Pause.EnableReturnToMenu = false;
            RestartRaceText.text = "---";
            RestartRaceTextNum.text = "";
            RestartTourneyText.text = "---";
            QuitToMenuText.text = "---";
        }
    }

    private void Update()
    {
        // INITIAL EVENTS THAT CAN'T BE PUT ON START
        if (firsttime)
        {
            if (CharacterStageDetails.Current) 
            {
                if (CharacterStageDetails.Current.ScoreEnabled == false) { ScoreObject.SetActive(false); }
            }

            firsttime = false;
        }
    }

    float notifpos;
    GameObject g;
    public void AddNotifObject(string name, float score, float multi)
    {
        g = Instantiate(ScoreNotifPrefab, ScoreNotifPosition);
        notifpos = ScoreNotifPosition.position.y + (ScoreNotifOffset * ScoreNotifPosition.childCount);
        g.transform.position = new Vector3(ScoreNotifPosition.position.x, notifpos, 0);
        g.GetComponent<ValuesHolder>().Ui_text[0].text = name + ": " + score + "p";
    }

    [System.Serializable]
    public class SpecialUiIcon
    {
        public string Type = "";
        public Image Icon;
        public Image EnergyCircle;
    }
}
