using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarTest : MonoBehaviour
{
    public RectTransform[] Bars;
    [Range(0,1)] public float BarPos = 0;
    public float BarPositionOffset = 1;
    public float MOD1;

    void Update()
    {
        for (int i = 0; i < Bars.Length; i++)
        {
            // POS
            Bars[i].position = transform.position + new Vector3(0, i * BarPositionOffset, 0);

            // BARS
            //assuming i is 0 or 1 (only 2 bars)
            Vector3 p = new Vector3();
            p = Bars[i].localScale;
            p.x = i + 1;
            p.x = p.x - BarPos * (Bars.Length - MOD1);
            p.x = Mathf.Clamp01(p.x);
            Bars[i].localScale = p;

        }
    }
}
