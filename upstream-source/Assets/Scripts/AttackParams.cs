using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class AttackParams : StateMachineBehaviour
{
    [Header("Parameters")]
    public RootMotionTransfer Transfer;
    public bool UseDefaults = false;
    public bool UseSoftLock = true;
    public bool EndOfAttackSequence = false;
    public bool UseRootMotion = true;
    public bool BypassStopWhenClose = false;
    public bool ResetAttackParameters = false;
    [Tooltip("If player presses the attack button after this time, queue next attack.")]
    public float AttackBufferStart = 0.1f;
    [Tooltip("If next attack input is queued, go to next attack")]
    public float NextAttackTime = 0.4f;
    [Tooltip("For When the player is trying to move or start another action")] 
    public float AttackEndTime = 1.0f;
    [Tooltip("When the attack ends for real.")]
    public float AttackExitTime = 1.4f;
    [Tooltip("Duration of tracking enemy target.")]
    public float SkinRotationTime = 0.1f;

    [Header("Misc Params")]
    public bool ForceNoLandingAnim = false;
    public bool MakeLandingAnimHappenLater = false;
    public bool ForceGroundedState = false;
    public bool ForceAttackCounterToZero = false;

    [Header("Speed Params")]
    public bool AddSpeed = false;
    public Vector3 AddRelativeSpeed = Vector3.zero;
    public bool ResetSpeed = false;
    public float ResetSpeedThreshold = 2;

    [Header("Hitbox")]
    public int HitboxID = -1;
    public float HitboxTime = 0.1f;
    float time;

    [Header("Hitbox Repeater")]
    public bool Repeat = false;
    public float RepeatFreq = 0.064f;
    public float RepeatDuration = 0;
    float f;
    int s;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Transfer == null)
        {
            if (animator.transform.TryGetComponent<RootMotionTransfer>(out Transfer))
            {
                SetAttackStatuses(animator, stateInfo);
            }
        }
        else
        {
            SetAttackStatuses(animator, stateInfo);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Transfer != null)
        {
            if(Transfer.Actions.Basic != null)
            {
                if (ForceNoLandingAnim) { Transfer.Actions.Basic.landingtime = 1; }
            }

            if (Transfer.Actions.Attacks != null)
            {
                if (ForceAttackCounterToZero) { Transfer.Actions.Attacks.SubActionTime = 0; }
            }
        }
    }

    public void SetAttackStatuses(Animator anim, AnimatorStateInfo stateInfo)
    {
        if (Transfer.Actions.Attacks != null)
        {
            Transfer.Actions.Attacks.NewAttack = true;
            Transfer.Actions.Attacks.IndependedAttackTime = 0;
            if (UseDefaults)
            {
                Transfer.Actions.Attacks.UseSoftLock = true;
                Transfer.Actions.Attacks.SequenceEnd = EndOfAttackSequence;
                Transfer.Actions.Attacks.BufferStartTime = Transfer.Actions.Attacks.DefaultAttackBufferStartTime;
                Transfer.Actions.Attacks.AttackNextTime = Transfer.Actions.Attacks.DefaultAttackNextTime;
                Transfer.Actions.Attacks.AttackEndTime = Transfer.Actions.Attacks.DefaultAttackEndTime;
                Transfer.Actions.Attacks.SkinRotationThreshold = Transfer.Actions.Attacks.SkinRotationDefaultThreshold;
                Transfer.Actions.Attacks.AttackExitTime = Transfer.Actions.Attacks.DefaultAttackExitTime;
            }
            else
            {
                Transfer.EnableRootMotion = UseRootMotion;
                Transfer.Actions.Attacks.BypassGapCloser = BypassStopWhenClose;
                Transfer.Actions.Attacks.UseSoftLock = UseSoftLock;
                Transfer.Actions.Attacks.SequenceEnd = EndOfAttackSequence;
                Transfer.Actions.Attacks.BufferStartTime = AttackBufferStart;
                Transfer.Actions.Attacks.AttackNextTime = NextAttackTime;
                Transfer.Actions.Attacks.AttackEndTime = AttackEndTime;
                Transfer.Actions.Attacks.SkinRotationThreshold = SkinRotationTime;
                Transfer.Actions.Attacks.AttackExitTime = AttackExitTime;
            }

            if (ResetSpeed && Transfer.Actions.Char.SpeedMagnitude < ResetSpeedThreshold)
            {
                Transfer.Actions.Char.rigid.linearVelocity = Vector3.zero;
            }

            if (AddSpeed)
            {
                Transfer.Actions.Char.rigid.linearVelocity += Transfer.transform.up * AddRelativeSpeed.y;
                Transfer.Actions.Char.rigid.linearVelocity += Transfer.transform.right * AddRelativeSpeed.x;
                Transfer.Actions.Char.rigid.linearVelocity += Transfer.transform.forward * AddRelativeSpeed.z;
            }

            if (ResetAttackParameters)
            {
                Transfer.Actions.Attacks.SubActionTime = 0;
                Transfer.Actions.Attacks.AttackBufferOn = false;
            }

            if (MakeLandingAnimHappenLater) { Transfer.Actions.Attacks.AirAttackExitAfterEnd = true; }

            if (ForceGroundedState) { Transfer.Actions.Attacks.GroundedAttack = true; }

            // TRIGGER HITBOX
            if (Repeat == false)
            {
                time = HitboxTime / (stateInfo.speed + 0.0001f);
                time = time / (anim.speed + 0.0001f);
                if (HitboxID >= 0) { Transfer.Actions.Attacks.StartHiboxCoroutine(time, HitboxID); }
            }
            else // REPEAT (QUEUE MULTIPLE HITBOXES)
            {
                if (HitboxID >= 0)
                {
                    f = 0; s = 0;
                    while (f < RepeatDuration && s < 999)
                    { s++; f += RepeatFreq; }

                    Transfer.RepeaterCoroutines = new IEnumerator[s];
                    for (int i = 0; i < Transfer.RepeaterCoroutines.Length; i++)
                    {
                        Debug.Log("STARTED Routine: " + f + ", " + s);
                        Transfer.RepeaterCoroutines[i] = Transfer.Actions.Attacks.HitboxCoroutine((RepeatFreq * (i + 1)) + HitboxTime, HitboxID);
                        Transfer.StartCoroutine(Transfer.RepeaterCoroutines[i]);
                    }

                }
            }
        }
        else
        {
            Debug.LogError("No Attack Script in " + Transfer.name);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Transfer != null)
        {
            // STOP COROUTINES
            if (Repeat)
            {
                for (int i = 0; i < Transfer.RepeaterCoroutines.Length; i++)
                {
                    if (Transfer.RepeaterCoroutines != null)
                    {
                        Transfer.StopCoroutine(Transfer.RepeaterCoroutines[i]);
                    }
                }
            }

            // RESET STUFF
            if (Transfer.Actions.Attacks != null)
            {
                Transfer.Actions.Attacks.AirAttackExitAfterEnd = false;
            }
        }
    }


}
