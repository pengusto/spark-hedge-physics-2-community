using System.Collections;
using UnityEngine;

public class SimpleTriggerIHB : StateMachineBehaviour
{
    [Header("Parameters")]
    public int Id = 0;
    public float Time = 0.1f;

    // CACHE
    IHB_SimpleHolder main;
    float t;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(main == null)
        {
            if(animator.TryGetComponent<IHB_SimpleHolder>(out main))
            {
                FireIHB();
            }
        }
        else
        {
            FireIHB();
        }

        void FireIHB()
        {
            //t = Time / ((stateInfo.speed * animator.speed) + 0.00001f); might be better function
            t = Time * stateInfo.speed;
            main.StartCoroutine(main.TriggerIHB(t, Id));
        }
    }




}
