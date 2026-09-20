using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerIHB : StateMachineBehaviour
{
    public RootMotionTransfer Transfer;
    public int HitboxID = -1;
    public float HitboxTime = 0.1f;
    public IEnumerator AttackCoroutine;
    public Coroutine Routine;
    public bool ResetOnExit = true;
    public bool StopOnExit = false;
    public bool ForceStop_OnExit = false;
    public bool ForceStop_OnEnter = false;
    public bool StopAllCoroutines = false;
    public float[] MultiCallTime;
    int i = 0;
    float t;

    [Header ("Repeater")]
    public bool Repeat = false;
    public float RepeatInterval = 0.1f;
    public float RepeatTimes = 13;
    float time;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // EXECUTE IHB WITH DELAY IN ATTACK SCRIPT
        if (HitboxID >= 0)
        {
            if (Transfer == null)
            {
                if (animator.transform.TryGetComponent<RootMotionTransfer>(out Transfer))
                {
                    StartActions(stateInfo, animator);
                }
            }
            else
            {
                StartActions(stateInfo, animator);
            }
        }
    }

    void StartActions(AnimatorStateInfo info, Animator anim)
    {
        if (ForceStop_OnEnter && HitboxID >= 0) { IHB.StopIHB(Transfer.Actions.Attacks.AttackHitboxes[HitboxID]); }
        time = HitboxTime / (info.speed + 0.0001f);
        time = time / (anim.speed + 0.0001f);
        AttackCoroutine = Transfer.Actions.Attacks.HitboxCoroutine(time, HitboxID);
        Routine = Transfer.Actions.Attacks.StartCoroutine(AttackCoroutine);

        for (i = 0; i < MultiCallTime.Length; i++)
        {
            Transfer.Actions.Attacks.StartCoroutine(Transfer.Actions.Attacks.HitboxCoroutine(MultiCallTime[i], HitboxID));
        }

        if (Repeat)
        {
            t = HitboxTime / ((info.speed /** anim.speed*/) + 0.0001f);
            for (i = 0; i < RepeatTimes; i++)
            {
                Transfer.Actions.Attacks.StartCoroutine(Transfer.Actions.Attacks.HitboxCoroutine(t, HitboxID));
                t += RepeatInterval / ((info.speed /** anim.speed*/) + 0.0001f);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ResetOnExit)
        {
            if (AttackCoroutine != null && Transfer != null) { Transfer.Actions.Attacks.StopCoroutine(AttackCoroutine); }
        }

        if (StopOnExit && Transfer != null)
        {
            AttackCoroutine = Transfer.Actions.Attacks.StopHitboxCoroutine(HitboxTime, HitboxID);
            Transfer.Actions.Attacks.StartCoroutine(AttackCoroutine);
        }

        if (ForceStop_OnExit && HitboxID >= 0)
        {
            if (Routine != null) { Transfer.Actions.Attacks.StopCoroutine(Routine); }
            IHB.StopIHB(Transfer.Actions.Attacks.AttackHitboxes[HitboxID]);
        }

        if (StopAllCoroutines)
        {
            Transfer.Actions.Attacks.StopAllCoroutines();
        }
    }
}
