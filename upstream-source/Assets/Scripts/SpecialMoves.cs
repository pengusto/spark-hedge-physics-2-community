using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpecialMoves : MonoBehaviour
{
    [Header("References")]
    public CharacterInput Inp;
    public CharacterActions Actions;
    public CharacterInteractions Interactions;

    // A(0), B(1), X(2), Y(3), R1(4), R2(5)
    // A(6), B(7), X(8), Y(9), R1(10), R2(11)

    [Header("Attacks")]
    public List<SpecialMove> QuickMoves;
    public List<SpecialMove> HardMoves;

    [Header("Progression")]
    public bool EnableProgression = false;
    public float[] HpLevel = new float[0];
    public float[] ApLevel = new float[0];
    public float[] PoiseLevel = new float[0];
    public float[] EnLevel = new float[0];

    [Header("Cache")]
    public Animator ShotAnimator;
    public SpecialMove CurrentMove;
    public float CurrentSpecialResetTime = 0.1f;
    public float SpecialTimer = 0f;
    bool firstframe = true;
    GameProgress g;
    ProgressIten p;
    int keyboardMode = 0;
    public float spcTimer;
    public bool ChargedJesterDashAvailable = false;
    public bool execute = true;

    private void FixedUpdate()
    {
        if (firstframe)
        {
            firstframe = false;
            if (EnableProgression)
            {
                SetPlayerStats();
                //UnlockMoves();
            }
        }

        // IF NOT THE PLAYER, DONE RUN REST OF SCRIPT
        if (Inp.Player == false) { this.enabled = false; return; }

        // CHECK FOR KEYBOARD MODE
        // todo - later :)

        // DO MOVES
        if (CheckIfAvailable())
        {
            if (Inp.SpecialHold || keyboardMode == 1)
            {
                // UI
                if (Interactions.Ref.SpecialsButtons.activeSelf == false)
                {
                    Interactions.Ref.SpecialsButtons.SetActive(true);
                    for (int i = 0; i < Interactions.Ref.SpecialBackgrounds.Count; i++)
                    {
                        Interactions.Ref.SpecialBackgrounds[i].color = Interactions.Ref.SpecialQuickMovesColor;
                    }
                }

                // UI SET
                for (int i = 0; i < Interactions.Ref.SpecialUiIcons.Count; i++)
                {
                    Interactions.Ref.SpecialUiIcons[i].Icon.sprite = QuickMoves[i].Icon;
                    Interactions.Ref.SpecialUiIcons[i].EnergyCircle.fillAmount = QuickMoves[i].EnergyCost / 100;
                }

                // START SPECIAL ATTACKS
                if (Inp.Jump || Input.GetKeyDown(KeyCode.Alpha3)) // DOWN DASH
                {
                    if (QuickMoves[0].Name == "Down Dash" && Interactions.En >= QuickMoves[0].EnergyCost)
                    {
                        if (Actions.Char.Grounded == false) // DOWN DASH ONLY IN AIR
                        {
                            SpecialTimer = 0;
                            Actions.SwitchAction(1);
                            Actions.Attacks.SubAction = 3;
                            Actions.Attacks.SpecialIndex = QuickMoves[0].Index;
                            CurrentMove = QuickMoves[0];
                            Actions.Interactions.En -= QuickMoves[0].EnergyCost;

                            Actions.Attacks.anim.SetBool("Idle", false);
                            Actions.Attacks.anim.SetTrigger("Special");
                            Actions.Attacks.anim.SetInteger("SpecialType", 0);
                        }
                    }
                    else if (QuickMoves[0].Name != "Down Dash")
                    {
                        SpecialTimer = 0;
                        Actions.SwitchAction(1);
                        Actions.Attacks.SubAction = 3;
                        Actions.Attacks.SpecialIndex = QuickMoves[0].Index;
                        CurrentMove = QuickMoves[0];
                        Actions.Interactions.En -= QuickMoves[0].EnergyCost;

                        Actions.Attacks.anim.SetBool("Idle", false);
                        Actions.Attacks.anim.SetTrigger("Special");
                        Actions.Attacks.anim.SetInteger("SpecialType", 0);
                    }
                }
                else if (Inp.JDash || Input.GetKeyDown(KeyCode.Alpha4)) // CHARGED JESTER DASH
                {
                    if (QuickMoves[1].Name == "Charged Jester Dash" && ChargedJesterDashAvailable == true)
                    {
                        if (QuickMoves[1].Unlocked && Interactions.En >= QuickMoves[1].EnergyCost)
                        {
                            SpecialTimer = 0;
                            Actions.SwitchAction(1);
                            Actions.Attacks.SubAction = 3;
                            Actions.Attacks.SpecialIndex = QuickMoves[1].Index;
                            CurrentMove = QuickMoves[1];
                            Actions.Interactions.En -= QuickMoves[1].EnergyCost;

                            Actions.Attacks.anim.SetBool("Idle", false);
                            Actions.Attacks.anim.SetTrigger("Special");
                            Actions.Attacks.anim.SetInteger("SpecialType", 1);
                        }
                    }
                    else if (QuickMoves[1].Name != "Charged Jester Dash")
                    {
                        if (QuickMoves[1].Unlocked && Interactions.En >= QuickMoves[1].EnergyCost)
                        {
                            SpecialTimer = 0;
                            Actions.SwitchAction(1);
                            Actions.Attacks.SubAction = 3;
                            Actions.Attacks.SpecialIndex = QuickMoves[1].Index;
                            CurrentMove = QuickMoves[1];
                            Actions.Interactions.En -= QuickMoves[1].EnergyCost;

                            Actions.Attacks.anim.SetBool("Idle", false);
                            Actions.Attacks.anim.SetTrigger("Special");
                            Actions.Attacks.anim.SetInteger("SpecialType", 1);
                        }
                    }
                }
                else if (Inp.LightAttack || Input.GetKeyDown(KeyCode.Alpha5))
                {
                    if (QuickMoves[2].Unlocked && Interactions.En >= QuickMoves[2].EnergyCost)
                    {
                        SpecialTimer = 0;
                        Actions.SwitchAction(1);
                        Actions.Attacks.SubAction = 3;
                        Actions.Attacks.SpecialIndex = QuickMoves[2].Index;
                        CurrentMove = QuickMoves[2];
                        Actions.Interactions.En -= QuickMoves[2].EnergyCost;

                        Actions.Attacks.anim.SetBool("Idle", false);
                        Actions.Attacks.anim.SetTrigger("Special");
                        Actions.Attacks.anim.SetInteger("SpecialType", 2);
                    }
                }
                else if (Inp.HeavyAttack || Input.GetKeyDown(KeyCode.Alpha6))
                {
                    if (QuickMoves[3].Unlocked && Interactions.En >= QuickMoves[3].EnergyCost)
                    {
                        SpecialTimer = 0;
                        Actions.SwitchAction(1);
                        Actions.Attacks.SubAction = 3;
                        Actions.Attacks.SpecialIndex = QuickMoves[3].Index;
                        CurrentMove = QuickMoves[3];
                        Actions.Interactions.En -= QuickMoves[3].EnergyCost;

                        Actions.Attacks.anim.SetBool("Idle", false);
                        Actions.Attacks.anim.SetTrigger("Special");
                        Actions.Attacks.anim.SetInteger("SpecialType", 3);
                    }
                }
                else if (Inp.Shot || Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (QuickMoves[4].Unlocked && Interactions.En >= QuickMoves[4].EnergyCost)
                    {
                        SpecialTimer = 0;
                        Actions.SwitchAction(1);
                        Actions.Attacks.SubAction = 3;
                        Actions.Attacks.SpecialIndex = QuickMoves[4].Index;
                        CurrentMove = QuickMoves[4];
                        Actions.Interactions.En -= QuickMoves[4].EnergyCost;

                        Actions.Attacks.anim.SetBool("Idle", false);
                        Actions.Attacks.anim.SetTrigger("Special");
                        Actions.Attacks.anim.SetInteger("SpecialType", 4);
                    }
                }
                else if (Inp.Dash || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (QuickMoves[5].Unlocked && Interactions.En >= QuickMoves[5].EnergyCost)
                    {
                        SpecialTimer = 0;
                        Actions.SwitchAction(1);
                        Actions.Attacks.SubAction = 3;
                        Actions.Attacks.SpecialIndex = QuickMoves[5].Index;
                        CurrentMove = QuickMoves[5];
                        Actions.Interactions.En -= QuickMoves[5].EnergyCost;

                        Actions.Attacks.anim.SetBool("Idle", false);
                        Actions.Attacks.anim.SetTrigger("Special");
                        Actions.Attacks.anim.SetInteger("SpecialType", 5);
                    }
                }

                // 0:A
                // 1:B
                // 2:X
                // 3:Y
                // 4:R1
                // 5:R2
            }
            else
            {
                // UI
                if (Interactions.Ref.SpecialsButtons.activeSelf)
                { Interactions.Ref.SpecialsButtons.SetActive(false); }
            }
        }

        // SPECIAL ACTIONS
        if (Actions.Action == 1 && Actions.Attacks.SubAction == 3)
        {
            if (CurrentMove != null)
            {
                // JUMP
                if (CurrentMove.SpcType == SpecialMove.SpecialType.Jump)
                {
                    if (SpecialTimer < CurrentMove.SpecialTypeTime)
                    {
                        if (Inp.Jump)
                        {
                            SpecialTimer = 0;
                            Actions.SwitchAction(0);
                            Actions.Basic.SubAction = 0;
                            Vector3 jump = Actions.Basic.Skin.up * CurrentMove.SpecialTypeValueY;
                            jump += Actions.Basic.Skin.forward * CurrentMove.SpecialTypeValueX;
                            Actions.Char.rigid.linearVelocity = jump;
                            Actions.Char.rigid.position += (transform.up * (Actions.Basic.JumpDetachDistance));
                            Actions.Char.CheckGroundTime = -Actions.Basic.GroundCheckInterval;
                            Actions.Char.Grounded = false;
                            Actions.Basic.AudioJump.Play();
                        }
                    }
                    execute = true;
                }
                // ADD SPEED AT START
                else if (CurrentMove.SpcType == SpecialMove.SpecialType.AddSpeedAtStart)
                {
                    if (SpecialTimer < CurrentMove.SpecialTypeTime)
                    {
                        Vector3 jump = Actions.Basic.Skin.up * CurrentMove.SpecialTypeValueY;
                        jump += Actions.Basic.Skin.forward * CurrentMove.SpecialTypeValueX;
                        Actions.Char.rigid.linearVelocity = jump;

                        if (CurrentMove.SpecialTypeValueY > 0.1f)
                        {
                            Actions.Char.rigid.position += (transform.up * (Actions.Basic.JumpDetachDistance));
                            Actions.Char.CheckGroundTime = -Actions.Basic.GroundCheckInterval;
                        }
                    }
                    execute = true;
                }
                // SHOT
                else if (CurrentMove.SpcType == SpecialMove.SpecialType.Shot)
                {
                    if (Actions.Inp.CurrentTarget == null)
                    {
                        Actions.Basic.anim.SetFloat("Dot", 0);
                        if (ShotAnimator) { ShotAnimator.SetFloat("Dot", 0); }
                    }
                    else
                    {
                        Vector3 dir = (transform.position - Actions.Inp.CurrentTarget.transform.position).normalized;
                        Actions.Basic.anim.SetFloat("Dot", -Vector3.Dot(dir, transform.up));
                        if (ShotAnimator) { ShotAnimator.SetFloat("Dot", -Vector3.Dot(dir, transform.up)); }
                    }

                    execute = true;
                }
                // NORMAL (charged jester dash)
                else if (CurrentMove.SpcType == SpecialMove.SpecialType.Normal)
                {
                    //Actions.Basic.CheckJump(false);
                    //Actions.Basic.CheckForDash();
                    //Those two are done in the combat action sub action 3
                    execute = true;
                }
                // MAY OR MAY NOT DO ANYTHING
                else if(CurrentMove.SpcType == SpecialMove.SpecialType.AirOnly)
                {
                    if(Actions.Char.Grounded == false)
                    { 
                        execute = true;
                    }
                    else
                    {

                    }
                }

                // EXECUTE
                if (spcTimer < 0.01f && execute)
                {
                    spcTimer = 1;
                    Actions.Basic.anim.SetTrigger(CurrentMove.AnimTrigger);
                }
            }
        }
        else
        {
            spcTimer = 0;
        }

        // CHARGED JESTER DASH CHECK
        if (Actions.Char.Grounded || Actions.Action == 2)
        {
            ChargedJesterDashAvailable = true;
        }

        // FINAL SETS
        SpecialTimer += Time.fixedDeltaTime;

        // OTHER SCRIPS
        if (Actions.Rail != null && Actions.Action != 2) { Actions.Rail.LookForRails(); }

    }

    bool CheckIfAvailable()
    {
        // TIME
        if(SpecialTimer < CurrentSpecialResetTime) { return false; }

        // INPUT
        if(Inp.InputEnabled == false) { return false; }

        // ACTIONS
        if (Actions.Action == 0)
        {
            if (Actions.Basic.SubAction == 0) { return true; }
            else if (Actions.Basic.SubAction == 1) { return true; }
            else if (Actions.Basic.SubAction == 2) { return true; }
            else if (Actions.Basic.SubAction == 4) { return true; }
        }
        else if(Actions.Action == 1)
        {
            if (Actions.Attacks.SubAction == 0) { return true; }
            else if (Actions.Attacks.SubAction == 1) { return true; }
            else if (Actions.Attacks.SubAction == 2) { return true; }
            else if (Actions.Attacks.SubAction == 3 && Actions.Attacks.SubActionTime > 0.2f) { return true; }
        }

        return false;
    }

    // LEVEL EFFECTS
    void SetPlayerStats()
    {
        g = SaveData.Data;

        // HP
        //p = g.FindIten(g.StoryItens, "Shell_HP");
        //if(p.ammount < HpLevel.Length)
        //{
        //    Interactions.HpMax = HpLevel[(int)p.ammount];
        //    Interactions.Hp = Interactions.HpMax;
        //}

        //// AP
        //p = g.FindIten(g.StoryItens, "Shell_AP");
        //if (p.ammount < ApLevel.Length)
        //{
        //    Interactions.ApMax = ApLevel[(int)p.ammount];
        //    Interactions.Ap = Interactions.ApMax;
        //}

        //// POISE
        //p = g.FindIten(g.StoryItens, "Shell_POISE");
        //if (p.ammount < PoiseLevel.Length)
        //{
        //    Interactions.BasePoise = PoiseLevel[(int)p.ammount];
        //}

        //// POISE
        //p = g.FindIten(g.StoryItens, "Shell_EN");
        //if (p.ammount < EnLevel.Length)
        //{
        //    Interactions.EnMax = EnLevel[(int)p.ammount];
        //    Interactions.En = Interactions.EnMax;
        //}

    }

    void UnlockMoves()
    {
        g = SaveData.Data;

        // COMBO BAR
        p = g.FindIten(g.StoryItens, "Move_Combo");
        if (p.unlocked) { Interactions.ComboEnabled = true; }

        // QUICK MOVES
        p = g.FindIten(g.StoryItens, "Special_FireJump");
        QuickMoves[0].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Special_DashBlast");
        QuickMoves[1].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Shoulder_Bash");
        QuickMoves[2].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Spin_Kick");
        QuickMoves[3].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Fireball");
        QuickMoves[4].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Dash_Slide");
        QuickMoves[5].Unlocked = p.unlocked;

        // HARD MOVES
        p = g.FindIten(g.StoryItens, "Moment_Blast");
        HardMoves[0].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Firespin_Kick");
        HardMoves[1].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Knockout_Punch");
        HardMoves[2].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Power_Kick");
        HardMoves[3].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Extreme_Ball");
        HardMoves[4].Unlocked = p.unlocked;
        p = g.FindIten(g.StoryItens, "Plasma_Blast");
        HardMoves[5].Unlocked = p.unlocked;

    }

    // CLASSES

    [System.Serializable]
    public class SpecialMove
    {
        public string Name;
        public string AnimTrigger;
        public int Index;
        public float EnergyCost = 5;
        public bool Unlocked = false;
        public SpecialType SpcType = 0;
        public float SpecialTypeTime = 0;
        public float SpecialTypeValueX = 0;
        public float SpecialTypeValueY = 0;
        public Sprite Icon;

        public enum SpecialType { Jump, AddSpeedAtStart, Shot, Normal, AirOnly };
    }

}
