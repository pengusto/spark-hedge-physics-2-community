using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [Header("Unlocker")]
    public GameObject UnlockObj;
    public GameObject UnlockObjStar;
    public Transform Container;
    public float UnlockPromptTime = 2.5f;
    float timer = 0;
    float interval = 0.2f;
    ProgressIten p;

    [Header("Story Stuff")]
    public float FuelHullPityAmm = 0.05f;

    // MESSAGES
    public static List<string> Messages = new List<string>();
    public static float MessageAddedTime = 0;
    public static float MessageSpeed = 1;
    //public Rewired.Player Inp;

    // SAVING + LOADING
    public static bool SaveOnLoadingScreen = true;

    private void Start()
    {
        // INITIAL STUFF
        //Inp = Rewired.ReInput.players.GetPlayer(0);

        // DO STORY STUFF
        if (StoryData.Instance)
        {
            if (StoryData.Instance.StoryMode) { FuelHullPity(FuelHullPityAmm); }
        }

        // LOAD SAVE DATA MESSAGES
        Time.timeScale = 1;
        if (SaveData.Data == null) 
        { 
            //SaveData.Data = SaveData.LoadProgress();
        }

        if (SaveData.Data != null)
        {
            for (int i = 0; i < SaveData.Data.Arcade.Count; i++)
            {
                if (SaveData.Data.Arcade[i].notify == true)
                {
                    StartCoroutine(UnlockIcon(SaveData.Data.Arcade[i].name, false, true, false, false));
                    interval += 0.5f; 
                    timer -= (UnlockPromptTime + MessageAddedTime);
                }
            }
            for (int i = 0; i < SaveData.Data.StoryItens.Count; i++)
            {
                if (SaveData.Data.StoryItens[i].notify == true)
                {
                    StartCoroutine(UnlockIcon(SaveData.Data.StoryItens[i].name, false, false, true, false));
                    interval += 0.5f; 
                    timer -= (UnlockPromptTime + MessageAddedTime);
                }
            }
            for (int i = 0; i < SaveData.Data.StoryFlags.Count; i++)
            {
                if (SaveData.Data.StoryFlags[i].notify == true)
                {
                    StartCoroutine(UnlockIcon(SaveData.Data.StoryFlags[i].name, true, false, false, false));
                    interval += 0.5f; 
                    timer -= (UnlockPromptTime + MessageAddedTime);
                }
            }
        }

        // LOAD REGULAR MESSAGES
        for (int i = 0; i < Messages.Count; i++)
        {
            StartCoroutine(UnlockIcon(Messages[i], false, false, false, true));
            interval += 0.5f; 
            timer -= (UnlockPromptTime + MessageAddedTime);
        }

        Messages.Clear();
    }

    IEnumerator UnlockIcon(string name, bool story, bool arcade, bool storyIten, bool message)
    {
        yield return new WaitForSeconds(interval);
        GameObject g;

        if (story)
        {
            g = Instantiate(UnlockObj, Container);
            g.transform.position = Vector3.zero;
            g.transform.rotation = Quaternion.identity;
            p = new ProgressIten();
            p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, name);
            g.GetComponent<ValuesHolder>().Ui_text[0].text = p.UnlockMessage;
            g.GetComponent<Animator>().speed = MessageSpeed;
            p.notify = false;
        }
        else if (arcade)
        {
            g = Instantiate(UnlockObj, Container);
            g.transform.position = Vector3.zero;
            g.transform.rotation = Quaternion.identity;
            p = new ProgressIten();
            p = SaveData.Data.FindIten(SaveData.Data.Arcade, name);
            g.GetComponent<ValuesHolder>().Ui_text[0].text = p.UnlockMessage;
            g.GetComponent<Animator>().speed = MessageSpeed;
            p.notify = false;
        }
        else if(storyIten)
        {
            g = Instantiate(UnlockObj, Container);
            g.transform.position = Vector3.zero;
            g.transform.rotation = Quaternion.identity;
            p = new ProgressIten();
            p = SaveData.Data.FindIten(SaveData.Data.StoryItens, name);
            g.GetComponent<ValuesHolder>().Ui_text[0].text = p.UnlockMessage;
            g.GetComponent<Animator>().speed = MessageSpeed;
            p.notify = false;
        }
        else if(message)
        {
            g = Instantiate(UnlockObjStar, Container);
            g.transform.position = Vector3.zero;
            g.transform.rotation = Quaternion.identity;
            g.GetComponent<ValuesHolder>().Ui_text[0].text = name;
            g.GetComponent<Animator>().speed = MessageSpeed;
        }

       
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0)
        {
            this.enabled = false;
            if (SaveOnLoadingScreen) 
            {
                Debug.Log("Saving on loading screen: " + SaveOnLoadingScreen);
                SaveData.SaveProgress(SaveData.Data);
            }
            MessageAddedTime = 0;
            MessageSpeed = 1;
            Debug.Log("Trying to load scene: " + SceneController.LevelToLoad);
            SceneManager.LoadSceneAsync(SceneController.LevelToLoad, LoadSceneMode.Single);
        }

        //if (Inp.GetButtonDown("A") || Inp.GetButtonDown("Start"))
        //{
        //    if( timer < -1) { timer = -1; } else { }
        //}
    }

    // STORY PITY
    public void FuelHullPity(float threshold)
    {
        ProgressIten h = SaveData.Data.FindIten(StoryData.Data.StoryFlags, "Kes_Hull");
        ProgressIten f = SaveData.Data.FindIten(StoryData.Data.StoryFlags, "Kes_Fuel");

        if(h.ammount < threshold)
        {
            h.ammount = threshold;
            Messages.Add("Hull Pity, Made some repairs.");
        }

        if (f.ammount < threshold)
        {
            f.ammount = threshold;
            Messages.Add("Fuel Pity, Added some fuel.");
        }


    }
}
