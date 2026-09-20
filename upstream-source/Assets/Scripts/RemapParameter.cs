using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemapParameter : StateMachineBehaviour
{
    public string InputParameter = "Speed";
    public string OutputParameter = "RemappedSpeed";
    public Vector2 NewRange = new Vector2(1, 4);
    float p;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        p = Mathf.Lerp(NewRange.x, NewRange.y, animator.GetFloat(InputParameter));
        animator.SetFloat(OutputParameter, p);
    }


}
