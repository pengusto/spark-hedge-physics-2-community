using UnityEngine;

public class NR_CheckFlagForRace : MonoBehaviour
{
    [Header("Parameters")]
    public string Flag;
    ProgressIten p;

    void Start()
    {
        p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, Flag);

        if (p != null)
        {
            if (p.ammount == 1) { gameObject.SetActive(true); }
            else { gameObject.SetActive(false); }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    
}
