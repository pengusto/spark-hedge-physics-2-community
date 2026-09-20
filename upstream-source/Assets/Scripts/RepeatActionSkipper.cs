using UnityEngine;

public class RepeatActionSkipper : StateMachineBehaviour
{
    [Header("Parameters")]
    public float MaxAmmount = 20;
    public string Parameter = "AirEscape";

    [Header("Debug")]
    public float IntroTimes;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IntroTimes++;
        if (animator.GetBool(Parameter)) { animator.ResetTrigger(Parameter); }
        if(IntroTimes > MaxAmmount)
        {
            animator.SetTrigger(Parameter);
            IntroTimes = 0;
        }
    }

}
