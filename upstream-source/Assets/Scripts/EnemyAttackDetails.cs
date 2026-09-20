using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDetails : StateMachineBehaviour
{
    [Header("References")]
    public RootMotionTransfer Transfer;
    public List<IEnumerator> EnemyActions = new List<IEnumerator>();

    [Header("Actions")]
    public bool TrackPlayer = false;
    public float TrackPlayerDuration = 0.25f;
    public bool InvencibleDuringAttack = false;
    public float InvencibleDA_Multiplier = 0.5f;
    public bool SuperArmourDuringAttack = false;
    public float SuperArmourDA_Multiplier = 0.5f;

    [Header("Special")]
    public bool ForceRootMotion = false;
    public bool SetDotProducts = false;

    [Header("Resetters")]
    public bool ResetSuperArmour = false;

    [Header("Specials")]
    public bool JumpToStart = false;
    public float JumpSpeed = 1;
    public bool SetTrackingSpeed = false;
    public float TrackingSpeed = 1;

    // CACHE
    int i;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Transfer == null)
        {
            if (animator.transform.TryGetComponent<RootMotionTransfer>(out Transfer))
            {
                Do();
            }
        }
        else
        {
            Do();
        }

        void Do()
        {
            // DO STUFF
            if (TrackPlayer) { Transfer.Actions.Inp.ai.trackingCounter = TrackPlayerDuration; }
            if (InvencibleDuringAttack) { Transfer.Actions.Interactions.InvencibilityCounter = stateInfo.length * InvencibleDA_Multiplier; }
            if (SuperArmourDuringAttack) { Transfer.Actions.Interactions.SuperArmourCounter = stateInfo.length * SuperArmourDA_Multiplier; }
            if (SetTrackingSpeed) { Transfer.Actions.Inp.ai.trackingSpeed = TrackingSpeed; }

            // RESETTERS
            if (ResetSuperArmour) { Transfer.Actions.Interactions.SuperArmourCounter = -1; }

            // SPECIAL
            if (JumpToStart)
            {
                Transfer.Actions.Inp.ai.JumpToStart = true;
                Transfer.Actions.Inp.ai.JumpToStartSpeed = JumpSpeed;
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Transfer != null)
        {
            if (SetDotProducts)
            {
                if(Transfer.Actions.Inp.CurrentTarget != null)
                {
                    Vector3 tgtdir = (Transfer.Actions.Inp.CurrentTarget.transform.position - Transfer.Actions.transform.position).normalized;
                    animator.SetFloat("Dot", Vector3.Dot(tgtdir, Transfer.transform.right));
                    animator.SetFloat("Dot_Up", Vector3.Dot(tgtdir, Transfer.transform.up));

                }
                else
                {
                    animator.SetFloat("Dot", 0);
                    animator.SetFloat("Dot_Up", 0);
                }
            }

            if (ForceRootMotion) { Transfer.RootMotionForcedOn = true; }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // RESET COROUTINES
        if (EnemyActions != null && Transfer != null)
        {
            for (i = 0; i < EnemyActions.Count; i++)
            {
                Transfer.Actions.Attacks.StopCoroutine(EnemyActions[i]);
            }
        }

        // RESET NON COROUTINES
        if(Transfer != null)
        {
            if (TrackPlayer) { Transfer.Actions.Inp.ai.trackingCounter = 0; }
            if (InvencibleDuringAttack) { Transfer.Actions.Interactions.InvencibilityCounter = 0; }
            if (SuperArmourDuringAttack) { Transfer.Actions.Interactions.SuperArmourCounter = 0; }
            if (ForceRootMotion) { Transfer.RootMotionForcedOn = false; }
        }
    }

}
