using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWritter : MonoBehaviour
{

    public Text Txt;
    public float WriteTickSpeed = 0.033f;
    public int CharactersPerTick = 3;
    public string OriginalText;

    void Awake()
    {
        OriginalText = Txt.text;
    }

    public void OnEnable()
    {
        Txt.text = "";
        InvokeRepeating("Write", 0.01f, WriteTickSpeed);
    }

    void Write()
    {
        for (int i = 0; i < CharactersPerTick; i++)
        {
            if (Txt.text.Length < OriginalText.Length)
            {
                Txt.text += OriginalText[Txt.text.Length];
            }
            else
            {
                Debug.Log("InvokeGone: " + gameObject.name);
                CancelInvoke("Write");
            }
        }  
    }

    public void WriteNewText(string txt)
    {
        this.CancelInvoke("Write");
        this.OriginalText = txt;
        this.Txt.text = ""; this.OnEnable();
    }

    
}
