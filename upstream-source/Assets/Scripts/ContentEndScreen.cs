using UnityEngine;

public class ContentEndScreen : MonoBehaviour
{
    public bool done = false;
    public bool setup = false;
    public float fadetime;
    //public Rewired.Player Inp;
    public UnityEngine.UI.Image Fade;
    public bool SetDayToZero = false;

    void Start()
    {
        //Inp = Rewired.ReInput.players.GetPlayer(0);
        //Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 0).enabled = false;
        //Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 1).enabled = true;

        Debug.Log("STOP saving on loading screen");
        LoadingScreen.SaveOnLoadingScreen = false;
    }

    private void Update()
    {
        if(done == false)
        {

            //if (Inp.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Return))
            //{
            //    done = true;
            //}

            // FADE
            fadetime += Time.deltaTime;
            Fade.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), fadetime);
        }
        else
        {
            // FADE
            fadetime += Time.deltaTime;
            Fade.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, fadetime);

            if (fadetime > 1.5f)
            {
                // RESET DATE TO ZERO
                if (SetDayToZero == true)
                {
                    ProgressIten p;
                    p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Day");
                    p.ammount = 1;
                    p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Month");
                    p.ammount = 12;
                    p.FlavorText = "December";
                    p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Year");
                    p.ammount = 2999;
                    p.FlavorText = "2999";
                    p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Year");
                    p.ammount = 2999;
                    p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Story_Started");
                    p.ammount = 0;
                    
                    Debug.Log("DATE RESET: " + "(" + p.ammount + ") "
                        + " (" + LoadingScreen.SaveOnLoadingScreen + ")");
                }


                Music.Manager.StopMusic(0.5f);
                SceneController.LoadStageLoading("MENU - MAIN MENU");
            }

        }
    }
}
