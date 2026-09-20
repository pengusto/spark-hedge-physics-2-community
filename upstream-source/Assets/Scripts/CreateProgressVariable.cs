using UnityEngine;

public class CreateProgressVariable : MonoBehaviour
{
    [Header("Story Parameter")]
    public bool StoryVariable = true;
    public string StoryParam;
    public float StoryValue;

    //CACHE
    public ProgressIten p;

    void Start()
    {
        if (StoryVariable)
        {
            p = SaveData.Data.FindIten(SaveData.Data.StoryFlags, StoryParam);
            if (p == null)
            {
                p = new ProgressIten();
                p.name = StoryParam;
                p.ammount = StoryValue;
                p.Description = "";
                p.UnlockMessage = "";
                p.UnlockCriteriaText = "";
                SaveData.Data.StoryFlags.Add(p);
                Debug.Log("Created Story Flag: " + p.ammount);
            }
            else
            {
                p.ammount = StoryValue;
                Debug.Log("Set Story Flag: " + p.ammount);
            }
        }
    }

    
}
