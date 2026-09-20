using UnityEngine;

public class ForceStoryMode : MonoBehaviour
{
    public bool DoOnAwake = false;

    private void Awake()
    {
        if (DoOnAwake)
        {
            StoryData.Instance.StoryMode = true;
            this.enabled = false;
        }
    }

    void Update()
    {
        StoryData.Instance.StoryMode = true;
        this.enabled = false;
    }
}
