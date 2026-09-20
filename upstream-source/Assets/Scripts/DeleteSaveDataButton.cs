using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveDataButton : MonoBehaviour
{
    public AudioSource DeleteAudio;
    public Image Circle;
    //public Rewired.Player Inp;
    float progress;

    void Start()
    {
        //Inp = Rewired.ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        //if(Inp.GetButton("Y") || Input.GetKey(KeyCode.Backspace))
        //{
        //    progress += Time.deltaTime * 0.8f;

        //    if (progress >= 1.0f)
        //    {
        //        RestartStoryMode();
        //        progress = 0;
        //    }
        //}
        //else
        //{
        //    progress -= Time.deltaTime * 2;
        //    progress = Mathf.Clamp01(progress);
        //}

        Circle.fillAmount = progress;
    }

    public void RestartStoryMode()
    {
        SaveData.Data.StoryFlags.Clear();
        SaveData.Data.CarVariations.Clear();
        StoryData.InitiateStoryData();
        DeleteAudio.Play();
        Debug.Log("DELETED STORY DATA");
    }
}
