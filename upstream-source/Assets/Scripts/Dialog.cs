using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{

    public List<Script> Scripts = new List<Script>(); 
    public bool Done = false;

    private void Update()
    {
        if (/*Input.GetKey(KeyCode.T)*/ false) 
        {
            Scripts[0].LineIndex = 0;
            TextBox.Instance.StartWritter(this, Scripts[0], false); 
        }
    }

    [System.Serializable]
    public class Line
    {
        public string Text = "";
        public string Speaker = "...";
        public StoryCharacter Character = StoryCharacter.Null;
        public Color SpeakerColor = Color.magenta;

        [Header("Participants Action")]
        public int[] Anim;
        [Tooltip("Value is Participant index on DialogItens")] 
        public int[] Target;
        public bool[] RotateTowardsTarget;
        public AudioClip LineSFX;
        public LineAction Actions;
        public FlagFunction[] FlagChanges;
    }

    [System.Serializable]
    public class LineAction
    {
        public GameObject[] ToEnable = new GameObject[0];
        public GameObject[] ToDisable = new GameObject[0];
        public AudioSource[] ExtraLineSFX = new AudioSource[0];
        public Redirects[] Redirect = new Redirects[0];

        public static void DoLineAction(LineAction action)
        {
            for (int i = 0; i < action.ToEnable.Length; i++) 
            { 
                if(action.ToEnable != null) { action.ToEnable[i].SetActive(true); }
            }

            for (int i = 0; i < action.ToDisable.Length; i++)
            {
                if (action.ToDisable != null) { action.ToDisable[i].SetActive(false); }
            }

            for (int i = 0; i < action.ExtraLineSFX.Length; i++)
            {
                if (action.ExtraLineSFX != null) { action.ExtraLineSFX[i].Play(); }
            }

            for (int i = 0; i < action.Redirect.Length; i++)
            {
                ConversationTracker.ConvoRedirecter r;
                r = ConversationTracker.LookForRedirect(action.Redirect[i].RedirectName);
                if(action.Redirect[i].Trigger != null && r != null)
                {
                    action.Redirect[i].Trigger.DialogObject = r.RedirectionConvo;
                    if (action.Redirect[i].IgnoreFlagsAfterRedirect)
                    {
                        action.Redirect[i].Trigger.IgnoreRedirects = true;
                        Debug.Log("REDIRECT DIALOG ACTION: " + action.Redirect[i].Trigger.IgnoreRedirects);
                    }
                }
                else
                {
                    Debug.Log("REDIRECT not FOUND! " + action.Redirect[i].RedirectName 
                        + " > " + (action.Redirect[i].Trigger) + " : " + (r));
                    action.Redirect[i].Trigger.IgnoreRedirects = true;
                    Debug.Log("REDIRECT DIALOG ACTION: " + action.Redirect[i].Trigger.IgnoreRedirects);
                }
            }
        }

        [System.Serializable]
        public class Redirects
        {
            public string RedirectName;
            public TriggerEffect Trigger;
            public bool IgnoreFlagsAfterRedirect = true;
        }
    }

    [System.Serializable]
    public class Choice
    {
        public string Text = "";
        public int NextScriptIndex = 0;
        public bool End = false;

        [Header("Participants Action")]
        public int[] Anim;
        public int[] Target;
        public bool[] RotateTowardsTarget;
        public GameObject[] ActivateOnChoice;
        public FlagFunction[] FlagChanges;
        public LineAction Actions;

    }

    [System.Serializable]
    public class Script
    {
        [Header("BASE")]
        public string name = "Script Name";
        public int LineIndex = 0;
        public int NextOnNoChoice = -1;
        public List<Line> Lines = new List<Line>();

        [Header("END STUFF")]
        public List<Choice> Choices = new List<Choice>();

    }

    // ALL CHARACTERS
    public enum StoryCharacter { Null, Shell, Shoe, DigiGirl, 
        DigiBot, Cingul, Narrator, Information, Individual, Vespa, Wolfman, Chrome, Hana, Clark, Survivor }

    // SPECIAL TYPES
    public enum FlagAction { RedirectToOtherScript, Add, Set }

    // SPECIALS
    [System.Serializable]
    public class FlagFunction
    {
        public string Flag;
        public FlagAction Action;
        public float Value;
        public float Redirect;

        public static void DoFlagAction(FlagFunction func, Choice choice)
        {
            // MAKE FLAG IF IT DOESNT EXIST
            ProgressIten p;
            if(SaveData.Data.FindIten(SaveData.Data.StoryFlags, func.Flag) == null) 
            {
                p = new ProgressIten();
                p.name = func.Flag; 
                p.ammount = 0; 
                SaveData.Data.StoryFlags.Add(p);
                p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, func.Flag);
                Debug.Log("New story flag created: " + p.name);
            }
            else 
            {
                p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, func.Flag);
                Debug.Log("Loaded right function flag: " + p.name);
            }

            // CHECK FUNCTION AND DO THING
            if (func.Action == FlagAction.Add)
            {
                p.ammount += func.Value;
                Debug.Log("Flag Addition (" + p.name + ") : " + p.ammount);
            }
            else if (func.Action == FlagAction.Set)
            {
                p.ammount = func.Value;
                Debug.Log("Flag Setted Value (" + p.name + ") : " + p.ammount);
            }
            else if (func.Action == FlagAction.RedirectToOtherScript)
            {
                if(choice != null)
                {
                    Debug.Log("REDIRECT CHECK: " + func.Flag + "(" + func.Value + ")");
                    if(p.ammount >= func.Value)
                    {
                        Debug.Log("REDIRECT CHECK 2: " + "(" + func.Flag + ") - (" + func.Value + "/" + p.ammount + ")");
                        choice.NextScriptIndex = Mathf.RoundToInt(func.Redirect);
                    }
                }
            }

            // DEBUG
            if (choice != null)
            {
                Debug.Log("Flag was set on choise text: (" + choice.Text + ")");
            }
        }
    }
}
