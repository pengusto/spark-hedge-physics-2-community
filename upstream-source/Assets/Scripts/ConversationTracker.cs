using System.Collections.Generic;
using UnityEngine;

public class ConversationTracker : MonoBehaviour
{
    [Header("Redirects")]
    public List<ConvoRedirecter> Redirects;

    // CACHE
    public static ConversationTracker instance;

    private void Start()
    {
        instance = this;
    }

    public static ConvoRedirecter LookForRedirect(string name)
    {
        for (int i = 0; i < instance.Redirects.Count; i++)
        {
            if(instance.Redirects[i].ConvoName == name)
            {
                return instance.Redirects[i];
            } 
        }
        return null;
    }

    [System.Serializable]
    public class ConvoRedirecter
    {
        public string ConvoName = "";
        public bool Done = false;
        public Conversation RedirectionConvo;
    }
}
