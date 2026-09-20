using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParameter : StateMachineBehaviour
{
    [Header("Boolean")]
    public string BoolParameter = "param";
    public bool SetTo = true;
    public bool SetToFalseOnExit = true;

    [Header("Float")]
    public string FloatParameter = string.Empty;
    public float FloatValue = 0;

    [Header("Integer")]
    public string Integer = string.Empty;
    public int IntegerValue = 0;

    [Header("Advanced")]
    public bool SetAfterFullTransition = false;
    public int CurrentLayer = 0;
    bool DoneTransitioning = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!SetAfterFullTransition) 
        { 
            animator.SetBool(BoolParameter, SetTo);
        }

        if (FloatParameter != string.Empty) { animator.SetFloat(FloatParameter, FloatValue); }
        if (Integer != string.Empty) { animator.SetInteger(Integer, IntegerValue); }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SetAfterFullTransition)
        {
            if (!DoneTransitioning)
            {
                if (animator.IsInTransition(CurrentLayer) == false)
                {
                    animator.SetBool(BoolParameter, SetTo);
                    DoneTransitioning = true; 
                }
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SetToFalseOnExit)
        {
            animator.SetBool(BoolParameter, false);
            DoneTransitioning = false;
        }
    }
}
