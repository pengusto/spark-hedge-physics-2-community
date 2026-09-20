using UnityEngine;

public class CharacterStageDetails : MonoBehaviour
{
    public static CharacterStageDetails Current;
    [Header("Main")]
    public bool HasIntroCard = true;
    public string AreaName = "...";
    public string StageName = "...";
    public string CurrentStageID = "";
    public string UnlockWhenCompleted = "";
    public int ResetsToGainOnComplete = 3;

    [Header("Score")]
    public bool ScoreEnabled = true;
    public int ScoreGoal = 500;
    public int BonusScore = 1000;
    public int FirstTimeBonus = 500;
    public string SceneToGoAfterComplete = "...";
    public int CutsceneIndex = -1;
    public float ScoreGainMultiplier = 1;
    public float DifficultyMultiplier = 1;
    public bool ForceDayToMoveForward = false;

    [Header("Death")]
    public bool AllowResetsAfterDeath = false;
    public string SceneToGoAfterFail = "...";
    public bool ConsumeResetsOnRetry = true;

    private void Start()
    {
        Current = this;
    }
}
