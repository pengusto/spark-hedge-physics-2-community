using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryMisc : MonoBehaviour
{
    [Header("Text Box Stuff")]
    public bool ForceTextboxSingonton = true;
    public TextBox BoxObject;

    void Start()
    {
        if(ForceTextboxSingonton)
        {
            Debug.Log("Set Box Instance");
            TextBox.Instance = BoxObject;
        }
    }

    
}
