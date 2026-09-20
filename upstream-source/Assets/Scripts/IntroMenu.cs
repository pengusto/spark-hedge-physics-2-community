using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroMenu : MonoBehaviour
{
    public List<GameObject> IntroObj;
    public Image Fade;
    public string NextScene = "MENU - MAIN MENU";
    public float TimeInterval = 3;
    public float FadeSpeed = 2;
    public OptionsMenu Options;

    // CACHE
    int index = 0;
    bool fade = false;
    float c;
    Color fadeColor = Color.black;
    //Rewired.Player Inp;

    private void Start()
    {
        Time.timeScale = 1;
        //Inp = Rewired.ReInput.players.GetPlayer(0);
        for (int i = 0; i < IntroObj.Count; i++) { IntroObj[i].SetActive(false); }
        IntroObj[0].SetActive(true);

        // MISC
        Options.LoadSettings();
        Options.SetSettings();
    }

    private void Update()
    {
        if (fade)
        {
            c = 0;
            fadeColor.a += Time.deltaTime * FadeSpeed;
            if(fadeColor.a >=  0.99f) 
            {
                index++;
                for (int i = 0; i < IntroObj.Count; i++) { IntroObj[i].SetActive(false); }
                if(index < IntroObj.Count) { IntroObj[index].SetActive(true); }
                else { SceneController.LoadStageLoading(NextScene); }
                fade = false;
            }
        }
        else
        {
            // SKIP FOR THE FIRST 2
            if (index < 2) { if (c > TimeInterval) { fade = true; } }
            fadeColor.a -= Time.deltaTime * FadeSpeed;
           
            //if(Inp.GetButtonDown("A") || Inp.GetButtonDown("Start") 
            //    || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            //{
            //    c = 0;
            //    fade = true;
            //}

        }

        c += Time.deltaTime;
        Fade.color = fadeColor;
        fadeColor.a = Mathf.Clamp01(Fade.color.a);
    }
}
