using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    [Header("Actions and Indexes")]
    public float Action = 0;
    public CharacterPhysics Char;
    public CharacterBasicActions Basic;
    public CharacterCombat Attacks;
    public CharacterRailActions Rail;
    public CharacterInput Inp;
    public SpecialMoves Spc;
    public JesterSwipe Swipe;

    [Header("Extras")]
    public CharacterInteractions Interactions;

    private void Start()
    {
        SwitchAction(0);
    }

    public void SwitchAction(int action)
    {
        ResetActionsVariables();
        Action = action;

        if (action == 0)
        {
            Basic.enabled = true;
            Basic.SubActionTime = 0;
        }
        else if(action == 1)
        {
            Attacks.enabled = true;
            Attacks.SubActionTime = 0;
        }
        else if(action == 2)
        {
            Rail.enabled = true;
            Rail.SubAction = 0;
            Rail.SubActionTime = 0;
        }
    }

    public void ResetActionsVariables()
    {
        if (Basic)
        {
            Basic.finaltilt = -Char.GravityDir;
            Basic.enabled = false;
            Basic.animSpeedParam = 0;
            Basic.SubAction = 0;
            Basic.SubActionTime = 0;
            Basic.AttackBuffered = false;
            Basic.CoyoteActive = false;
            Basic.WallCoyoteTimeCounter = 0;
            Basic.WallCoyote = false;
            Basic.anim.SetBool("DoubleJump", false);
        }

        if (Attacks)
        {
            Attacks.Attacking = false;
            Attacks.AttackBufferOn = false;
            Attacks.TriggerLightAttack = false;
            Attacks.TriggerHeavyAttack = false;
            Attacks.SubActionTime = 0;
            Attacks.enabled = false;
            Attacks.IndependedAttackTime = 0;
            Attacks.HurtCounter = 0;
            Attacks.LightHold = 0;
            Attacks.HeavyHold = 0;
            Attacks.ParryCounter = 0;
            if (Attacks.ParryStart != null) { IHB.StopIHB(Attacks.ParryStart); }
        }

        if (Rail)
        {
            Rail.enabled = false;
            Rail.SubAction = 0;
            Rail.SubActionTime = 0;
            Rail.OutOfRailCounter = 0;
            Rail.finalSideG = 0;
            Rail.finalGravityG = 0;
            if (Rail.RailSound) { Rail.RailSound.Stop(); }
            Rail.Railtime = 0;
            Rail.BoostAudio.Stop();
            Rail.RegenBreakAudio.Stop();
            Rail.RailSound.Stop();
            Rail.RailCrouchDrag.Stop();
            Rail.IsCrouching = false;
            Char.rigid.isKinematic = false;
            Char.enabled = true;
        }
    }
}

public enum VectorDirection { Forward, Back, Right, Left, Up, Down }
