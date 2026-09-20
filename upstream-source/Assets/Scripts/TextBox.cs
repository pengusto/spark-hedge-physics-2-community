using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    [Header("References")]
    public CharacterCamera Cam;
    public Text Box;
    public Image CharBoxImage;
    public Text CharBox;
    public Animator SkipIcon;
    public static TextBox Instance;

    [Header("Parameters")]
    public float Tick = 0.016f;
    public float CharactersPerTick = 3;
    public float PunctuationSpacingTime = 0.37f;
    public float SpecialSpacingMinTime = 0.2f;

    [Header("Choices")]
    public List<ValuesHolder> ChoiceObjects = new List<ValuesHolder>();
    public List<GameObject> ChoiceMiscObjects = new List<GameObject>();
    public GameObject ChoiceArrow;
    public float ChoiceOpenInterval = 0.1f;
    public int ChoiceIndex;
    public float ChoiceItenGlowPower = 1.2f;
    public float ChoiceItenGlowSpeed = 100;
    public float ChoiceNextScriptTime = 0.5f;
    Color ChoiceItenColor;
    MenuExtensions MenuEx = new MenuExtensions();
    Vector2 RawInput;
    int inpy;

    [Header("Fx")]
    public AudioSource SoundMove;
    public AudioSource SoundBack;
    public AudioSource SoundClick;
    public AudioSource SoundNext;
    public AudioSource SoundNextLine;
    public AudioSource SoundWrite;
    public Vector2 WriteSoundRange = new Vector2(0.8f, 1.2f);

    [Header("Cache")]
    //public Rewired.Player Inp;
    public bool Active = false;
    public bool Done = false;
    public bool Skipped = false;
    public int MenuMode = 0;
    Dialog CurrentDialog;
    public Dialog.Script CurrentScript;
    StoryData.CharacterInfo character;

    private void Start()
    {
        Instance = this;
        ChoiceItenColor = ChoiceObjects[0].Ui_images[0].color;
    }

    private void Update()
    {
        // GET CAMERA
        if(Cam == null && CharacterCamera.Main != null) { Cam = CharacterCamera.Main; }

        if (Active && CurrentScript != null && CurrentDialog != null)
        {
            // REGULAR TEXT BOX
            if (MenuMode == 0)
            {
                // CHECK FOR SKIPPING
                if (Done == false && Skipped == false && MainBoxRoutine != null)
                {
                    //if (Inp.GetButtonDown("A") || Inp.GetButtonDown("B") || Inp.GetButtonDown("X") || Inp.GetButton("R2"))
                    //{
                    //    Skipped = true;
                    //    if (SkipIcon) { SkipIcon.SetTrigger("Click"); SoundNext.Play(); }
                    //    StartWritter(CurrentDialog, CurrentScript, true);
                    //}
                }
                else if (Done)  // TRIGGER NEXT LINE
                {
                    //if (Inp.GetButtonDown("A") || Inp.GetButtonDown("B") || Inp.GetButtonDown("X") || Inp.GetButton("R2"))
                    //{
                    //    CurrentScript.LineIndex++;
                    //    if (CurrentScript.LineIndex < CurrentScript.Lines.Count)
                    //    {
                    //        // NEXT
                    //        if (SkipIcon) { SkipIcon.SetTrigger("Click"); SoundNextLine.Play(); }
                    //        StartWritter(CurrentDialog, CurrentScript, false);
                    //    }
                    //    else
                    //    {
                    //        if (CurrentScript.Choices.Count > 0)
                    //        {
                    //            // OPEN CHOICES MENU
                    //            StartCoroutine(OpenChoices(ChoiceOpenInterval));
                    //            MenuMode = -1;
                    //        }
                    //        else
                    //        {
                    //            if(CurrentScript.NextOnNoChoice >= 0)
                    //            {
                    //                t = 0;
                    //                MenuMode = 0;
                    //                CurrentScript.LineIndex = 0;
                    //                CurrentScript = CurrentDialog.Scripts[CurrentScript.NextOnNoChoice];
                    //                CurrentScript.LineIndex = 0;
                    //                ChoiceArrow.SetActive(false);
                    //                StartWritter(CurrentDialog, CurrentScript, false);
                    //            }
                    //            else
                    //            {
                    //                // END
                    //                Debug.Log("TEXT BOX SET TO DONE NORMALLY");
                    //                Box.text = "";
                    //                CharBox.text = "";
                    //                CurrentDialog.Done = true;
                    //                CurrentScript = null;
                    //                gameObject.SetActive(false);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            // CHOOSE CHOICE MODE
            else if(MenuMode == 1)
            {
                // INPUT SETUP
                //RawInput = new Vector2(Inp.GetAxis("LeftAnalogX"), -Inp.GetAxis("LeftAnalogY"));
                //if (Inp.GetButton("D_Right")) { RawInput.x++; }
                //if (Inp.GetButton("D_Left")) { RawInput.x--; }
                //if (Inp.GetButton("D_Up")) { RawInput.y--; }
                //if (Inp.GetButton("D_Down")) { RawInput.y++; }
                RawInput.x = Mathf.Clamp(RawInput.y, -1, 1);
                inpy = MenuEx.MenuMovement(RawInput.y);

                // UP AND DOWN MOVE
                if (inpy == 1 || inpy == -1)
                {
                    ChoiceIndex += inpy;
                    if (ChoiceIndex >= CurrentScript.Choices.Count) { ChoiceIndex = 0; }
                    else if (ChoiceIndex < 0) { ChoiceIndex = CurrentScript.Choices.Count - 1; }
                    SoundMove.Play();
                    inpy = 0;
                }

                // ARROW
                ChoiceArrow.transform.position = ChoiceObjects[ChoiceIndex].Objects[0].transform.position;

                // SELECT
                //if (Inp.GetButtonDown("A") || Inp.GetButtonDown("X"))
                //{
                //    t = 0;
                //    MenuMode = 2;
                //    SoundClick.Play();
                //    ChoiceArrow.GetComponent<Animator>().SetTrigger("Click");
                //    ChoiceFlags();
                //    Dialog.LineAction.DoLineAction(CurrentScript.Choices[ChoiceIndex].Actions);
                //}

                void ChoiceFlags()
                {
                    Dialog.Choice choice = CurrentScript.Choices[ChoiceIndex];
                    for (int i = 0; i < choice.FlagChanges.Length; i++)
                    {
                        Dialog.FlagFunction.DoFlagAction(choice.FlagChanges[i], choice);
                    }
                }

                // MENU FX
                for (int i = 0; i < ChoiceObjects.Count; i++)
                { ChoiceObjects[i].Ui_images[0].color = ChoiceItenColor; }
                ChoiceObjects[ChoiceIndex].Ui_images[0].color = 
                    Color.Lerp(ChoiceItenColor, ChoiceItenColor * ChoiceItenGlowPower, 
                    Mathf.Abs(Mathf.Sin(Time.unscaledTime * ChoiceItenGlowSpeed)));
            }
            // SWITCH TO NEXT SCRIPT
            else if (MenuMode == 2) 
            {
                t += Time.unscaledDeltaTime;

                // NEXT
                if(t > ChoiceNextScriptTime)
                {
                    // DEACTIVATE CHOICES
                    //for (i = 0; i < ChoiceObjects.Count; i++) { ChoiceObjects[i].gameObject.GetComponent<Animator>().SetTrigger("End"); }
                    for (i = 0; i < ChoiceObjects.Count; i++) { ChoiceObjects[i].gameObject.SetActive(false); }
                    for (i = 0; i < ChoiceMiscObjects.Count; i++) { ChoiceMiscObjects[i].gameObject.SetActive(false); }


                    if (CurrentScript.Choices[ChoiceIndex].End)
                    {
                        // END SCRIPT
                        t = 0;
                        MenuMode = 0;
                        Box.text = "";
                        CharBox.text = "";
                        CurrentScript = null;
                        CurrentDialog.Done = true;
                        gameObject.SetActive(false);
                        ChoiceArrow.SetActive(false);
                    }
                    else
                    {
                        // GO TO NEXT SCRIPT
                        t = 0;
                        MenuMode = 0;
                        CurrentScript.LineIndex = 0;
                        CurrentScript = CurrentDialog.Scripts[CurrentScript.Choices[ChoiceIndex].NextScriptIndex];
                        CurrentScript.LineIndex = 0;
                        ChoiceArrow.SetActive(false);
                        StartWritter(CurrentDialog, CurrentScript, false);
                    }
                }

                // MENU FX
                for (int i = 0; i < ChoiceObjects.Count; i++)
                { ChoiceObjects[i].Ui_images[0].color = ChoiceItenColor; }
                ChoiceObjects[ChoiceIndex].Ui_images[0].color =
                    Color.Lerp(ChoiceItenColor, ChoiceItenColor * (ChoiceItenGlowPower * 1.2f),
                    Mathf.Abs(Mathf.Sin(Time.unscaledTime * (ChoiceItenGlowSpeed * 3))));
            }
        }
    }

    public void StartWritter(Dialog dialog, Dialog.Script script, bool skip)
    {
        // SETUP
        gameObject.SetActive(true);
        CurrentScript = script;
        Skipped = skip;
        Active = true;
        Done = false;
        index = 0;
        mode = 0;
        parsed = 0;
        CurrentDialog = dialog;
        Box.text = string.Empty;
        finalWrite = string.Empty;

        if(script.Lines[script.LineIndex].Character == Dialog.StoryCharacter.Null)
        {
            CharBoxImage.color = script.Lines[script.LineIndex].SpeakerColor;
            CharBox.text = script.Lines[script.LineIndex].Speaker;
        }
        else
        {
            character = StoryData.GetCharacterDetails(script.Lines[script.LineIndex].Character);
            CharBoxImage.color = character.CharacterColor;
            CharBox.text = character.Name;
        }

        // WRITE
        if (MainBoxRoutine != null) { StopCoroutine(MainBoxRoutine); }
        MainBoxRoutine = StartCoroutine(MainBoxNumerator = WriteToBox(Tick, script.Lines[script.LineIndex].Text, CharactersPerTick, skip));
    }

    public void StopWritter()
    {
        Box.text = "";
        CharBox.text = "";
        CurrentScript = null;
        gameObject.SetActive(false);
    }

    // WRITTING COROUTINE
    public Coroutine MainBoxRoutine;
    public IEnumerator MainBoxNumerator;
    string s;
    string finalWrite;
    float t;
    int parsed;
    int index;
    int mode = 0;
    int i;

    IEnumerator WriteToBox(float interval, string txt, float charamm, bool skip)
    {
        // INITIAL SETUP
        //Inp = Rewired.ReInput.players.GetPlayer(0);
        s = txt;
        t = interval;

        // ADD TEXT TO BOX THEN DO IT AGAIN
        while (index < s.Length && skip == false)
        {
            while(i < CharactersPerTick && index < s.Length)
            {
                // CHECK WHAT TO WRITE
                //if(txt[index] == '<') { mode = 1; }

                // CHECK
                if(index > s.Length) { break; }

                // DOT SPACING CHECKS
                if(/*txt[index] == '.' ||*/ txt[index] == ',') { t = PunctuationSpacingTime; }
                else { t = interval; }

                // SPECIAL SPACING CHECKS
                if(txt[index] == '&')
                {
                    index++;
                    if (int.TryParse(txt[index].ToString(), out parsed))
                    {
                        t = (float)parsed * SpecialSpacingMinTime;
                        index++;
                    }
                }

                // CHECK 2
                if (index > s.Length) { break; }

                // WRITE 
                if (mode == 0)
                {
                    Box.text += txt[index];
                    index++;
                    i++;
                }
                else
                {
                    if (txt[index] == '>') { mode = 0; }
                    index++;
                    i++;
                }
            }

            // WAIT A BIT AND RETURN TO START OF WHILE LOOP
            i = 0;
            yield return new WaitForSeconds(t);
            if (SoundWrite.isPlaying == false)
            {
                SoundWrite.pitch = Random.Range(WriteSoundRange.x, WriteSoundRange.y);
                SoundWrite.Play();
            }
        }

        // GET FINAL TEXT TO DISPLAY
        for (i = 0; i < txt.Length; i++)
        {
            if (txt[i] == '&') { i += 2; }
            finalWrite += txt[i];
        }

        Box.text = finalWrite;
        Done = true;
    }

    // MISC ROUTINES
    IEnumerator OpenChoices(float interval)
    {
        for (int i = 0; i < ChoiceMiscObjects.Count; i++)
        {
            ChoiceMiscObjects[i].SetActive(true);
        }

        for (i = 0; i < ChoiceObjects.Count; i++)
        {
            ChoiceObjects[i].gameObject.SetActive(false);
            if (i < CurrentScript.Choices.Count)
            {
                ChoiceObjects[i].gameObject.SetActive(true);
                ChoiceObjects[i].Ui_text[0].text = CurrentScript.Choices[i].Text;
                yield return new WaitForSeconds(interval);
            }
        }

        ChoiceArrow.SetActive(true);
        ChoiceIndex = 0;
        MenuMode = 1;
    }
    
}
