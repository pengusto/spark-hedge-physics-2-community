using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageIntro : MonoBehaviour
{
    public static StageIntro Player1Intro;
    public static StageIntro Player2Intro;
    public int Player = 1;
    public Text TopText;
    public Text BottonText;

    private void Start()
    {
        if (Player == 1)        { Player1Intro = this; }
        else if (Player == 2)   { Player2Intro = this; }
        else                    { Player1Intro = this; }
        gameObject.SetActive(false);
        Debug.Log("Setup Intro");
    }

    private void OnDestroy()
    {
        Player1Intro = null;
        Player2Intro = null;
    }
}
