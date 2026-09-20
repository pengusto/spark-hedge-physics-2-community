using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameTrigger : MonoBehaviour
{
    int Frame = 0;
    int FrameToTrigger = 60;

    public bool Animation;
    public string AnimationTrigger = "Next";

    void Update()
    {
        Frame++;
        if(Frame > FrameToTrigger)
        {
            if (Animation)
            {
                if(TryGetComponent<Animator>(out Animator a))
                {
                    a.SetTrigger(AnimationTrigger);
                }
            }

            this.enabled = false;
        }
    }
}
