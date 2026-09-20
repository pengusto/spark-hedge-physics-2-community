using UnityEngine;
using UnityEngine.UI;

public class PauseMenuHighjack : MonoBehaviour
{
    [Header("Pause Hijack")]
    public PauseMenu Pause;
    public Text ResetRaceUI;
    public Text ResetRaceUI_Amm;
    public Text ResetTourneyUI;
    public Text ReturnToMenuUI;

    void Start()
    {
        Pause.DisableRestartRace = true;
        Pause.DisableRestartTourney = true;
        Pause.EnableReturnToMenu = false;
        ReturnToMenuUI.text = "---";
        ResetRaceUI.text = "---";
        ResetRaceUI_Amm.text = "";
        ResetTourneyUI.text = "---";
    }


}
