using UnityEngine;

public class TriggerEffect : MonoBehaviour
{
    public enum Effect { None, CameraDirection, SceneChange, Animation, Dialog }
    public Effect TrigEffect;

    [Header("Camera Direction")]
    public float Look_Speed = 10;
    public float Look_Time = 2;
    public float Look_HeightOffset = 0.2f;

    [Header("Scene Change")]
    public string SceneToGoTo;
    public int CutsceneIndex = -1;

    [Header("Animation")]
    public Animator Anim;
    public string[] Anims_Bools;
    public bool[] Anim_Bools_Set;
    public string[] Anims_Triggers;

    [Header("Dialog")]
    public Conversation DialogObject;
    public bool DialogTriggerOnContact = false;
    public bool Dialog_DisableOnTrigger;
    public GameObject Dialog_ToEnableOnTrigger;
    [HideInInspector]public bool DialogTriggered = false;

    [Header("Alternate Dialogs")]
    public AlternateDialogs[] AlternateDialogObjects;
    AlternateDialogs alternate = null;
    public bool IgnoreRedirects = false;

    public void TriggerAnimations(Animator anim)
    {
        for (int i = 0; i < Anims_Bools.Length; i++)
        {
            anim.SetBool(Anims_Bools[i], Anim_Bools_Set[i]);
        }

        for (int i = 0; i < Anims_Triggers.Length; i++)
        {
            anim.SetTrigger(Anims_Triggers[i]);
        }
    }

    public void StartDialog(Conversation d)
    {
        // CHECK FOR ALTERNATES
        if (IgnoreRedirects == false)
        {
            for (int i = 0; i < AlternateDialogObjects.Length; i++)
            {
                alternate = AlternateDialogs.CheckForAlternateDialogs(AlternateDialogObjects[i]);
                if (alternate != null) { d = alternate.Dialog; }
            }
        }

        // START DIALOG
        if (d)
        {
            d.enabled = true;
            d.Active = true;

            if (Dialog_DisableOnTrigger) { gameObject.SetActive(false); }
            if (Dialog_ToEnableOnTrigger != null) { Dialog_ToEnableOnTrigger.SetActive(true); }
        }
    }

    [System.Serializable]
    public class AlternateDialogs
    {
        public string Flag = "";
        public MultiFlag[] Flags = new MultiFlag[0];
        public int FlagThreshold = 0;
        public Conversation Dialog;

        public static AlternateDialogs CheckForAlternateDialogs(AlternateDialogs alternate)
        {
            ProgressIten p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, alternate.Flag);

            // CHECK SINGLE FLAG
            if (alternate.Flags.Length <= 0)
            {
                if (p == null) 
                { 
                    Debug.Log("CHECKING FOR ALT DIALOG, FLAG NOT FOUND: " + alternate.Flag); 
                    return null;
                }
                else
                {
                    if (p.ammount >= alternate.FlagThreshold)
                    {
                        Debug.Log("CHECK FOR FLAG: " + p.name + "(" + p.ammount + ")" 
                            + " DONE, REDIRECTING DIALOG TO: " + alternate.Dialog.gameObject.name);
                        return alternate;
                    }
                    else
                    {
                        Debug.Log("RETURNING NULL FLAG!, Threshold not met. " + alternate.Flag);
                        return null;
                    }
                }
            }
            else
            {
                int FlagsTrue = 0;
                // CHECK MULTI FLAG
                for (int i = 0; i < alternate.Flags.Length; i++)
                {
                    p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, alternate.Flags[i].FlagName);
                    if(p == null) 
                    { 
                        Debug.Log("MULTI FLAG, no flag found: " + alternate.Flags[i].FlagName); 
                        return null; 
                    }
                    else
                    {
                        if (p.ammount >= alternate.Flags[i].Threshold)
                        {
                            FlagsTrue += 1;
                            Debug.Log("CHECK FOR MULTI FLAG: " + p.name + "(" + p.ammount + ")" 
                                + " DONE, ADDING TO COUNT: " + "(" + FlagsTrue + "/" + alternate.Flags.Length + ")");
                        }
                        else
                        {
                            Debug.Log("RETURNING NULL MULTI FLAG!, Threshold not met. " + alternate.Flags[i].FlagName);
                        }
                    }
                }

                if(FlagsTrue >= alternate.Flags.Length) 
                {
                    Debug.Log("MULTI FLAG WORKED, Going to: " + alternate.Dialog.gameObject.name); 
                    return alternate; 
                }
                else 
                {
                    return null;
                }
            }

            // RETURN IF NOTHING
            Debug.Log("Nothing to redirect, returning, this may be an ERROR");
            return null;
        }
    }

    [System.Serializable]
    public class MultiFlag
    {
        public string FlagName;
        public int Threshold;
    }
}
