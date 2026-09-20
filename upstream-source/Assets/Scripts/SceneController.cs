using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//using Rewired;

public class SceneController : MonoBehaviour {

    public Scene Gameplay;
    public static string LevelToLoad;
    public static int LastLoadedLevel;
    //Player Rewinp;

    void Start()
    {

    }

    public static void ResetValues()
    {
        Time.timeScale = 1;
    }

    public static void LoadStageLoading(string stageName)
    {
        ResetValues();
        SceneManager.LoadScene("LOADING SCREEN", LoadSceneMode.Single);
        LevelToLoad = stageName;
    }

}
