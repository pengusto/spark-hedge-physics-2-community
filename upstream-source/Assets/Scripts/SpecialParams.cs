using UnityEngine;
using static SpecialMoves;

public class SpecialParams : StateMachineBehaviour
{
    [Header("Parameters")]
    public RootMotionTransfer Transfer;
    public SpecialMoves Specials;
    public bool UseSoftLock = true;
    public bool UseRootMotion = true;
    public bool AllowVerticalRootMotion = true;
    public bool ApplyDrag = true;
    public bool ResetDash = false;
    public bool ForceRotationToTarget = false;
    public bool ForceSpecialAction = false;
    public bool UseSpecialSkinRotation = true;
    public SpecialMove SpecialDetails;
    public float SkinRotationTime = 0.1f;
    public float SpecialCooldownTime = 0.1f;
    public float SpecialEarlyExitTime = 99f;
    public float SpecialForceExitTime = 20;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Transfer == null)
        {
            animator.transform.TryGetComponent<RootMotionTransfer>(out Transfer);
            if(Transfer != null) { Specials = Transfer.Actions.GetComponent<SpecialMoves>(); }
        }

        if(Transfer != null)
        {
            if (Specials) 
            {
                Specials.CurrentSpecialResetTime = SpecialCooldownTime; 
            }

            if (ResetDash)
            {
                Transfer.Actions.Basic.DashAvailable = true;
                Transfer.Actions.Basic.AirDashAvailable = true;
                Transfer.Actions.Char.Sliding = false;
            }

            if (ForceSpecialAction)
            {
                Transfer.Actions.Attacks.SubAction = 3;
                Transfer.Actions.Spc.SpecialTimer = 0;
                Transfer.Actions.SwitchAction(1);
                Transfer.Actions.Attacks.SubAction = 3;
                Transfer.Actions.Attacks.SpecialIndex = -1;       
                Transfer.Actions.Spc.CurrentMove = SpecialDetails;
            }

            Transfer.Actions.Attacks.UseSpecialSkinRotation = UseSpecialSkinRotation;
            Transfer.Actions.Attacks.UseSoftLock = UseSoftLock;
            Transfer.Actions.Attacks.SkinRotationThreshold = SkinRotationTime;
            Transfer.Actions.Attacks.SpecialEarlyExitTime = SpecialEarlyExitTime;
            Transfer.Actions.Attacks.SpecialForceExitTime = SpecialForceExitTime;
            Transfer.EnableRootMotion = UseRootMotion;
            Transfer.AllowVertical = AllowVerticalRootMotion;
            Transfer.Actions.Attacks.ApplyDragOnSpecial = ApplyDrag;
            Transfer.Actions.Attacks.SpecialForceSkinRotationToTarget = ForceRotationToTarget;
        }
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (Transfer)
        //{
        //    Transfer.EnableRootMotion = false;
        //    Transfer.AllowVertical = false;
        //}
    }
}
