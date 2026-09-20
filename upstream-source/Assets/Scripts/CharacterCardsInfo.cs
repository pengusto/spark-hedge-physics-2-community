using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CharacterCardsInfo : MonoBehaviour
{
    [Header("Intro Card")]
    public PauseMenu Pause;
    public GameObject IntroCard;
    public Text TopName;
    public Text BottonName;
    public float IntroDuration = 2.5f;
    bool Intro = false;
    float IntroCounter = 0;

    [Header("Fail Card")]
    public GameObject FailObject;
    public List<RectTransform> FailMenuItens = new List<RectTransform>();
    public List<RectTransform> FailMenuArrowPos = new List<RectTransform>();
    public Text RestartsText;
    public int RestartIndex = 0;
    public Animator Arrow;
    public bool ConsumeResetOnRestart = true;
    bool AllowRestarts = true;
    bool Fail = false;
    float FailCounter = 0;

    [Header("Complete Card")]
    public GameObject CompleteCard;
    public GameObject Complete_Obj;
    public Text Score_Txt;
    public Text Bonus_Txt;
    public GameObject Goalmet_Obj;
    public GameObject CompleteFinalFade;
    bool Complete = false;
    bool addBonus = false;
    float CompleteCounter = 0;
    [HideInInspector] public float FinalScore;
    [HideInInspector] public float TotalScore;

    [Header("FX")]
    public AudioSource AudioMove;
    public AudioSource AudioClick;
    public AudioSource AudioBack;

    [Header("Transition Card")]
    public GameObject SceneTransitionObject;
    public bool SceneTransition = false;
    public float TransitionCounter = 0;
    public Image Fade;
    public string NextStage;

    [Header("Cache")]
    CharacterInput CharInp;
    CharacterInteractions CharInt;
    public MenuExtensions MenuEx = new MenuExtensions();
    public MenuExtensions MenuEy = new MenuExtensions();
    public Vector2 RawInput;
    int Iten;
    int inpx;
    int inpy;
    string scoreFormat = "###,###,000,000,000";
    ProgressIten resets;

    private void Start()
    {
        resets = SaveData.Data.FindIten(SaveData.Data.StoryItens, "Iten_Reset");
    }

    void Update()
    {
        // MENU STUFF, MOSTLY INPUT SETUP
        if (CharInp && (Fail))
        {
            //RawInput = new Vector2(CharInp.Inp.GetAxis("LeftAnalogX"), -CharInp.Inp.GetAxis("LeftAnalogY"));
            //if (CharInp.Inp.GetButton("D_Right")) { RawInput.x++; }
            //if (CharInp.Inp.GetButton("D_Left")) { RawInput.x--; }
            //if (CharInp.Inp.GetButton("D_Up")) { RawInput.y--; }
            //if (CharInp.Inp.GetButton("D_Down")) { RawInput.y++; }
            RawInput.x = Mathf.Clamp(RawInput.x, -1, 1);
            RawInput.y = Mathf.Clamp(RawInput.y, -1, 1);
            inpx = MenuEx.MenuMovement(RawInput.x);
            inpy = MenuEy.MenuMovement(RawInput.y);
        }

        // CARDS
        if (Intro)
        {
            IntroCounter += Time.deltaTime;
            if (IntroCounter > IntroDuration) 
            { 
                CharInp.InputEnabled = true; 
                Intro = false;
                Pause.enabled = true;
            }
        }
        else if (Fail)
        {
            FailCounter += Time.deltaTime;
            if(FailCounter > 0)
            {
                Pause.enabled = false;

                if (inpy == 1)
                {
                    Iten++;
                    if (Iten >= FailMenuItens.Count) { Iten = 0; }
                    else if (Iten < 0) { Iten = FailMenuItens.Count - 1; }
                    AudioMove.Play();
                }
                else if (inpy == -1)
                {
                    Iten--;
                    if (Iten >= FailMenuItens.Count) { Iten = 0; }
                    else if (Iten < 0) { Iten = FailMenuItens.Count - 1; }
                    AudioMove.Play();
                }

                // IF NO RESETS
                if(AllowRestarts == false)
                { 
                    FailMenuItens[RestartIndex].gameObject.SetActive(false);
                }
                else
                {
                    if (ConsumeResetOnRestart) { RestartsText.text = "Restart (" + resets.ammount + "x)"; }
                    else { RestartsText.text = "Restart!"; }
                }

                // FAIL ITENS
                if(Iten == 0) // RESTART
                {
                    if(AllowRestarts == false) { Iten = 1; }

                    // RESTART
                    //if (CharInp.Inp.GetButtonDown("A"))
                    //{
                    //    if (resets.ammount > 0.0f)
                    //    {
                    //        if (ConsumeResetOnRestart) { resets.ammount -= 1.0f; }
                    //        FailObject.GetComponent<Animator>().SetTrigger("End");
                    //        Arrow.SetTrigger("CLick");
                    //        AudioClick.Play();
                    //        StartCoroutine(GoToScene(2, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));
                    //        Fail = false;
                    //    }
                    //    else
                    //    {
                    //        if (AudioBack.isPlaying == false) { AudioBack.Play(); }
                    //    }
                    //}

                }
                else if(Iten == 1) // EXIT
                {
                    //if (CharInp.Inp.GetButtonDown("A"))
                    //{
                    //    FailObject.GetComponent<Animator>().SetTrigger("End");
                    //    Arrow.SetTrigger("CLick");
                    //    AudioClick.Play();
                    //    StartCoroutine(GoToScene(2, CharacterStageDetails.Current.SceneToGoAfterFail));
                    //    Fail = false;
                    //}
                }

                // MANAGE ARROW
                Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, FailMenuArrowPos[Iten].position, Time.deltaTime * 20f);
            }
        }
        else if (Complete)
        {
            if (CharacterStageDetails.Current)
            {
                Pause.enabled = false;

                Bonus_Txt.text = "BONUS GOAL: " + CharacterStageDetails.Current.ScoreGoal.ToString(scoreFormat);
                if(addBonus == false) { Score_Txt.text = "SCORE: " + FinalScore.ToString(scoreFormat); }
                else
                {
                    FinalScore = Mathf.Lerp(FinalScore, TotalScore, Time.deltaTime * 20);
                    Score_Txt.text = "SCORE: " + FinalScore.ToString(scoreFormat);
                }
            }
        }
        else if (SceneTransition)
        {
            Pause.enabled = false;
            if (TransitionCounter < 0.1f) 
            {
                TransitionCounter = 0.13f;
                Fade.color = new Color(0, 0, 0, 0);
            }

            TransitionCounter += Time.fixedDeltaTime;
            Fade.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), TransitionCounter - 0.11f);

            if (TransitionCounter > 1.5f)
            {
                SceneController.LoadStageLoading(NextStage);
                this.enabled = false;
            }
        }
    }

    public void StartIntroCard(CharacterInput Inp, CharacterInteractions Int, string topname, string botname)
    {
        // THIS GETS CALLED VIA "CharacterUiReferences.cs" IN THE PLAYER IN "CharacterInteractions.cs"
        Pause.enabled = false;
        this.gameObject.SetActive(true);
        Intro = true;
        IntroCounter = 0;
        CharInp = Inp;
        CharInt = Int;
        IntroCard.SetActive(true);
        TopName.text = topname;
        BottonName.text = botname;
        CharInp.InputEnabled = false;
    }

    // DELAYED EVENTS
    public IEnumerator StartFailCard(CharacterInput Inp, CharacterInteractions Int, bool allowRestarts, float time)
    {
        Music.Manager.StopMusic(1);
        if (CharacterStageDetails.Current)
        {
            if(CharacterStageDetails.Current.ConsumeResetsOnRetry == false) 
            { ConsumeResetOnRestart = false; }
        }
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(true);
        AllowRestarts = allowRestarts;
        Intro = false;
        Fail = true;
        CharInp = Inp;
        CharInt = Int;
        FailObject.SetActive(true);
        CharInp.InputEnabled = false;
        TotalScore = Int.Score;
    }

    public IEnumerator GoToScene(float time, string scene)
    {
        // ADD VXP
        ProgressIten p = SaveData.Data.FindIten(SaveData.Data.StoryItens, "VXP");
        p.ammount += TotalScore;
        if (AllowRestarts == false) { LoadingScreen.Messages.Add("Gained: " + TotalScore + " VXP"); }

        // LOAD NEXT SCENE
        yield return new WaitForSeconds(time);
        SceneController.LoadStageLoading(scene);
    }

    public IEnumerator StartCompleteCard(CharacterInput Inp, CharacterInteractions Int, float time)
    {
        Music.Manager.StopMusic(1);
        CharInp = Inp;
        CharInt = Int;
        FinalScore = Int.Score;
        Intro = false;
        Fail = false;
        Complete = true;
        yield return new WaitForSeconds(time);
        CompleteCard.SetActive(true);
        yield return new WaitForSeconds(2);
        Complete_Obj.SetActive(true);
        Score_Txt.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        Bonus_Txt.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        TotalScore = 0;

        //("CurrentStage.ammount is related to completion, 1 is complete and 2 is complete + bonus)
        // Cannot get bonus again once you already got it
        ProgressIten nextStage = SaveData.Data.FindIten(SaveData.Data.StoryItens, CharacterStageDetails.Current.UnlockWhenCompleted);
        ProgressIten p2 = SaveData.Data.FindIten(SaveData.Data.StoryItens, "Iten_Reset");
        ProgressIten CurrentStage = SaveData.Data.FindIten(SaveData.Data.StoryItens, CharacterStageDetails.Current.CurrentStageID);

        // ADD FINAL SCORE BONUS OR NOT
        // UNLOCK NEXT STAGE + GAIN RESET
        if (FinalScore > CharacterStageDetails.Current.ScoreGoal) 
        {
            if(CurrentStage != null)
            {
                if(CurrentStage.ammount <= 1)
                {
                    UnlockNextStage();
                    GetResetIfFirsTime();
                    CurrentStage.ammount = 2;
                }
            }
           
            Goalmet_Obj.SetActive(true);
            yield return new WaitForSeconds(2);
            addBonus = true;
            TotalScore += FinalScore + CharacterStageDetails.Current.BonusScore;
            yield return new WaitForSeconds(2f);
            CompleteFinalFade.SetActive(true);
            CutsceneRepo.ConvoToPlay = CharacterStageDetails.Current.CutsceneIndex;
            CutsceneRepo.TimeToStart = 0.5f;
            //if (CharacterStageDetails.Current.ForceDayToMoveForward) { CallendarScreen.DontChangeDay = false; }
            StartCoroutine(GoToScene(1.1f, CharacterStageDetails.Current.SceneToGoAfterComplete));
        }
        else
        {
            if (CurrentStage != null)
            {
                UnlockNextStage();
                if (CurrentStage.ammount <= 1) { CurrentStage.ammount = 1; }
            }

            GetResetIfFirsTime();
            TotalScore += FinalScore;
            CompleteFinalFade.SetActive(true);
            CutsceneRepo.ConvoToPlay = CharacterStageDetails.Current.CutsceneIndex;
            CutsceneRepo.TimeToStart = 0.5f;
            //if (CharacterStageDetails.Current.ForceDayToMoveForward) { CallendarScreen.DontChangeDay = false; }
            StartCoroutine(GoToScene(1.1f, CharacterStageDetails.Current.SceneToGoAfterComplete));
        }

        void GetResetIfFirsTime()
        {
            if (CurrentStage != null)
            {
                if (p2 != null && CurrentStage.ammount <= 0)
                {
                    p2.ammount += CharacterStageDetails.Current.ResetsToGainOnComplete;
                    LoadingScreen.Messages.Add("Got Reset!: " + CharacterStageDetails.Current.ResetsToGainOnComplete + "x");
                }
            }
        }

        void UnlockNextStage()
        {
            if (nextStage != null)
            {
                if (nextStage.unlocked == false)
                {
                    nextStage.unlocked = true; nextStage.unChecked = true;
                    LoadingScreen.Messages.Add("Next Stage Unlocked!");
                    TotalScore += CharacterStageDetails.Current.FirstTimeBonus;
                }
            }
        }
    }

    public void StartSceneTransition(string scene)
    {
        Intro = false;
        NextStage = scene;
        SceneTransitionObject.SetActive(true);
        SceneTransition = true;
        TransitionCounter = 0;
    }
}
