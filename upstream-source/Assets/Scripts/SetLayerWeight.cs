using UnityEngine;

public class SetLayerWeight : StateMachineBehaviour
{
    public int LayerIndex = 1;
    public float Weight = 0;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(LayerIndex, Weight);
    }

}
