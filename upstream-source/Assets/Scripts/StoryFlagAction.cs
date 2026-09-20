using UnityEngine;

public class StoryFlagAction : MonoBehaviour
{
    [Header("Parameters")]
    public string Flag = "";
    ProgressIten p;

    [Header("Disablaler")]
    public bool DisableIfTag = false;
    public int Threshold = 0;
    public GameObject[] ToDisable;

    private void Start()
    {
        CheckFlag();
    }

    private void OnEnable()
    {
        CheckFlag();
    }

    void CheckFlag()
    {
        p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, Flag);

        if (p != null)
        {
            // DISABLER
            if (DisableIfTag)
            {
                if (p.ammount >= Threshold)
                {
                    for (int i = 0; i < ToDisable.Length; i++)
                    {
                        ToDisable[i].SetActive(false);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Flag of (" + Flag + ") was null.");
        }
    }
}
