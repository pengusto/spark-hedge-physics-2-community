using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneTriggerIHB : StateMachineBehaviour
{
    //[Header("Main")]
    public int Hitbox = 0;
    public float Time = 0.1f;
    public DroneController Drone;

    //[Header("Extras")]
    public bool Repeat = false;
    public float RepeatInterval = 0.1f;
    public float RepeatTimes = 13;

    // CACHE
    public IEnumerator AttackCoroutine;
    float t;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Drone == null)
        {
            animator.TryGetComponent<DroneController>(out Drone);
        }

        if(Drone != null)
        {
            if (Repeat == false)
            {
                AttackCoroutine = Drone.TriggerIHB(Hitbox, Time);
                Drone.StartCoroutine(AttackCoroutine);
            }
            else if (Repeat)
            {
                t = Time;
                for (int i = 0; i < RepeatTimes; i++)
                {
                    Drone.StartCoroutine(Drone.TriggerIHB(Hitbox, t));
                    t += RepeatInterval;
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Drone.StopAllCoroutines();
    }


}
