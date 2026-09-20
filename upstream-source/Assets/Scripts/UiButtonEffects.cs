using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UiButtonEffects : MonoBehaviour
{
    Image img;

    [Header("GLOW")]
    public Color GlowColor = Color.white;
    public float GlowSpeed = 5;
    bool Glow;
    float GlowTimeCounter = 0;

    //CACHE
    Color InitialColor;

    private void Start()
    {
        img = GetComponent<Image>();
        InitialColor = img.color;
    }

    void Update()
    {
        if (Glow)
        {
            GlowTimeCounter -= Time.unscaledDeltaTime * GlowSpeed;
            img.color = Color.Lerp(InitialColor, GlowColor, GlowTimeCounter);
            if(GlowTimeCounter < 0) 
            {
                Glow = false;
                GlowTimeCounter = 0;
                img.color = InitialColor;
            }
        }
    }

    public void StartGlow(float time)
    {
        Glow = true;
        GlowTimeCounter = time;
    }
}
