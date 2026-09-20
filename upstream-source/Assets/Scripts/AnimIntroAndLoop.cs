using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimIntroAndLoop : StateMachineBehaviour
{
    public string LoopParameter = "Loop";
    public float LoopTime = 0.1f;
    public float TransitionSpeed = 20;
    float t = 0;
    float lerp = 0;
    public bool ResetOnEnd = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lerp = 0;
        t = 0;
        animator.SetFloat(LoopParameter, t);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(t > LoopTime) 
        {
            lerp += Time.deltaTime * TransitionSpeed;
            animator.SetFloat(LoopParameter, lerp);
        }
        t += Time.deltaTime;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ResetOnEnd) { animator.SetFloat(LoopParameter, 0); }
    }
}
