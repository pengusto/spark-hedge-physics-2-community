using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("References")]
    public CharacterPhysics Char;
    public CharacterBasicActions Basic;
    public CharacterActions Actions;
    public CharacterInput Inp;
    public Rigidbody rigid;
    public Animator anim;
    public Transform Skin;
    public int SubAction = 0;
    public float SubActionTime = 0;
    [Tooltip("Value is reset by the animator to keep track of animaion change")]
    public float IndependedAttackTime = 0;

    [Header("Physics")]
    public float GroundDrag = 5;

    [Header("Combat Parameters")]
    public float AttackSpeed = 0;
    public float DefaultAttackBufferStartTime = 0.1f;
    public float DefaultAttackNextTime = 1;
    public float DefaultAttackEndTime = 1.5f;
    public float DefaultAttackExitTime = 2.0f;
    public float SkinRotationDefaultThreshold = 0.5f;
    public float SkinRotationSpeed = 20;
    public float AttackDelayOnCancel = 0.3f;
    public float AttackHoldThreshold = 0.2f;

    [Header("Blocking Parameters")]
    public float BlockingDrag = 3;
    public float BlockingSkinRotationThreshold = 0.2f;
    public float BlockReleaseTime = 0.75f;
    public float BlockingCooldown = 0.2f;
    public float BlockingAttackTime = 0.4f;
    public bool BlockingAvailable = true;
    public bool BlockingBuffer = false;
    public bool BlockingAttackBuffer = true;

    [Header("Parry Parameters")]
    public GameObject ParryStart;
    public GameObject ParryBlock;
    public GameObject ParryParry;
    public GameObject ParryMiss;
    public float ParryCounter = 0;
    public float ParryPerfectThreshold = 0.16f;
    public float ParryMissThreshold = 0.16f;

    [Header("Special Parameters")]
    public List<GameObject> SpecialAttacksToResetOnExit;

    [Header("04 - Shot Parameters")]
    public bool HasShot = false;
    public float ShotDuration = 0.75f;
    public float ShotChargeDuration = 2f;
    public Transform ShotCenter;
    public AudioSource ShotChargeAudioSource;
    public ParticleSystem ShotChargingParticle;
    public float ShotCharging_Emission = 10;
    public ParticleSystem ShotChargedParticle;
    public float ShotCharged_Emission = 20;
    public GameObject ShotSmallIHB;
    public GameObject ShotLargeIHB;

    [Header("Hurt Parameters")]
    public int HurtType = 0;
    public float HurtCounter = 0;
    public float FlinchRecoveryTime = 0.3f;
    public float StaggerRecoveryTime = 1;
    public float HurtGroundDragMultiplier = 5;
    public float HurtAirDragMultiplier = 1;
    public float HurtAirDetachForce = 0.15f;
    public float HurtAirGroundCheckInterval = 0.16f;
    public bool HurtWallBounce = true;
    public float WallBounceMultiplier = 0.9f;
    public float WallBounceMinSpeed = 1;
    public float WallUpBounce = 1;
    public float WallRayDistance = 2;
    public GameObject WallHitIHB;
    public LayerMask WallBounceMask;
    float WallBounceCount;
    Vector3 wallBounceRay;
    Vector3 wallBounceReflected;

    [Header("Softlock Params")]
    public RootMotionTransfer RootMotion;
    public float MinSoftLockDistance = 2;
    public float MaxSoftLockDistance = 6;
    public float SoftLockDotMin = 0.6f;
    public float SoftLockGapCloserRootMotionPower = 1;
    public float SoftLockGapCloserPower = 2;
    public float SoftLockPushAwayPower = 1;
    public float SoftLockGapCloserDuration = 0.3f;
    float tgtDist;
    Vector3 tgtDir;
    float tgtDot;

    [Header("Softlock Air Params")]
    public float AirSfotLockMinDistance = 3;
    public float AirSoftLockTime = 2;
    public float AirSoftLockSpeed = 5;
    public float AirSoftLockDrag = 10;
    Vector3 AirSoftLockPos;

    [Header("Attack FX")]
    public List<GameObject> AttackHitboxes;

    [Header("Misc / Fixes")]
    public VectorDirection PreviousInputSkinDirection = VectorDirection.Forward;

    [Header("Cache")]
    public int AttackType = 0;
    public bool Attacking = false;
    public bool AttackBufferOn = false;
    public bool TriggerLightAttack = false;
    public bool TriggerHeavyAttack = false;
    public bool TriggerLightHoldAttack = false;
    public bool TriggerHeavyHoldAttack = false;
    public float BufferStartTime;
    public float AttackNextTime;
    public float AttackEndTime;
    public float AttackExitTime;
    public float SkinRotationThreshold;
    public bool SequenceEnd = false;
    public bool NewAttack = false;
    public bool GroundedAttack = false;
    public int LastAttackType;
    public bool HurtFlying = false;
    public bool UseSoftLock = true;
    public bool AirAttackExitAfterEnd = false;
    public float LightHold = 0;
    public float HeavyHold = 0;
    Vector3 prevInput;
    Vector3 localspeed;
    Vector3 hitForce;
    Vector3 worldHitForce;
    Vector3 localhitvel;
    Vector3 hitDirProxy;
    Vector3 AirPosProxy;
    float HurtUpForce;
    HitboxData HitHitBox;
    CharacterInteractions Stats;
    Vector3 deltaPosition;
    RaycastHit hit;
    public bool BypassGapCloser = false;
    public bool ApplyDragOnSpecial = true;
    public bool SpecialForceSkinRotationToTarget = false;
    public float SpecialEarlyExitTime = 99;
    public float SpecialForceExitTime = 20;
    public int SpecialIndex;
    public bool UseSpecialSkinRotation = true;
    Vector3 TargetDir;
    public float ChargeShotCharge;

    private void Start()
    {
        BufferStartTime = DefaultAttackBufferStartTime;
        AttackNextTime = DefaultAttackNextTime;
        AttackEndTime = DefaultAttackEndTime;
        SkinRotationThreshold = SkinRotationDefaultThreshold;
        AttackExitTime = DefaultAttackExitTime;
    }

    public void StartAttack(int type)
    {
        Basic.InCombat = true;
        Basic.CombatCounter = 3;
        Actions.SwitchAction(1);
        SubAction = 0;
        SubActionTime = 0;
        SequenceEnd = false;
        anim.ResetTrigger("BlockStart");
        if (type == 0) // LIGHT
        {
            TriggerLightAttack = true;
            GroundedAttack = true;
        }
        else if(type == 1)
        {
            TriggerHeavyAttack = true;
            GroundedAttack = true;
        }
        if (type == 2) 
        {
            TriggerLightAttack = true;
            GroundedAttack = false;
        }
        else if (type == 3)
        {
            TriggerHeavyAttack = true;
            GroundedAttack = false;
        }
    }

    public void CheckForBlockInput()
    {
        // STOP IF NOT AVAILABLE
        if (!BlockingAvailable)
        {
            if (Inp.Block) { BlockingBuffer = true; }
            return;
        }

        if (BlockingBuffer) 
        { 
            BlockingBuffer = false; 
            Inp.Block = true;
        }

        // START BLOCK
        if(Actions.Action == 0 && Actions.Basic != null && Char.Sliding == false)
        {
            if (Inp.Block)
            {
                if (Actions.Basic.SubAction != 3) // IF NOT IN WALL
                {
                    Actions.SwitchAction(1);
                    SubAction = 1;
                    SubActionTime = 0;
                }
            }
        }
        else if(Actions.Action == 1 && Actions.Attacks != null)
        {
            if (Inp.Block)
            {
                if (SubAction == 0)
                {
                    Actions.SwitchAction(1);
                    SubAction = 1;
                    SubActionTime = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (SubAction == 0) // ATTACKING
        {
            if (TriggerLightAttack || TriggerHeavyAttack || TriggerLightHoldAttack || TriggerHeavyHoldAttack)
            {
                // START THE ATTACK
                anim.ResetTrigger("LightAttack");
                anim.ResetTrigger("HeavyAttack");
                anim.ResetTrigger("NextLight");
                anim.ResetTrigger("NextHeavy");
                anim.ResetTrigger("NextLightHold");
                anim.ResetTrigger("NextHeavyHold");
                if (!Attacking)
                {
                    if (TriggerLightAttack && GroundedAttack) { anim.SetTrigger("LightAttack"); }
                    if (TriggerHeavyAttack && GroundedAttack) { anim.SetTrigger("HeavyAttack"); }
                    if (TriggerLightAttack && !GroundedAttack) { anim.SetTrigger("Air_LightAttack"); }
                    if (TriggerHeavyAttack && !GroundedAttack) { anim.SetTrigger("Air_HeavyAttack"); }
                    SubActionTime = 0;
                    TriggerLightAttack = false;
                    TriggerHeavyAttack = false;
                    TriggerLightHoldAttack = false;
                    TriggerHeavyHoldAttack = false;
                    AttackBufferOn = false;
                    Attacking = true;
                    LightHold = 0;
                    HeavyHold = 0;
                }
                // CONTINUE ATTACKING
                else
                {
                    Debug.Log("NEXT ATTACK");
                    if (TriggerLightAttack) { anim.SetTrigger("NextLight"); }
                    if (TriggerHeavyAttack) { anim.SetTrigger("NextHeavy"); }
                    if (TriggerLightHoldAttack) { anim.SetTrigger("NextLightHold"); }
                    if (TriggerHeavyHoldAttack) { anim.SetTrigger("NextHeavyHold"); }
                    SubActionTime = 0;
                    TriggerLightAttack = false;
                    TriggerHeavyAttack = false;
                    TriggerLightHoldAttack = false;
                    TriggerHeavyHoldAttack = false;
                    AttackBufferOn = false;
                    Attacking = true;
                    LightHold = 0;
                    HeavyHold = 0;
                }
            }

            // WHILE ATTACKING
            if (Attacking)
            {
                SubActionTime += Time.fixedDeltaTime;
                IndependedAttackTime += Time.fixedDeltaTime;

                // HOLD TIMERS
                if (Inp.LightAttackHold) { LightHold += Time.fixedDeltaTime; } else { LightHold = 0; }
                if (Inp.HeavyAttackHold) { HeavyHold += Time.fixedDeltaTime; } else { HeavyHold = 0; }

                // PHYSICS
                if (Char.Grounded)
                {
                    rigid.linearDamping = Mathf.Lerp(rigid.linearDamping, GroundDrag, Time.fixedDeltaTime * 5);
                }
                else
                {
                    rigid.linearDamping = 0;
                }

                // END ATTACK, RETURN TO BASIC
                if ((SubActionTime > AttackEndTime || IndependedAttackTime > (AttackEndTime + 0.05f)) && Inp.InputMag > 0.01f)
                {
                    EndAttack();
                }
                else if (SubActionTime > AttackExitTime)
                {
                    EndAttack();
                }

                // SET BUFFER 
                if (Inp.LightAttack && SubActionTime > BufferStartTime && !AttackBufferOn)
                {
                    if (SequenceEnd == false && NewAttack)
                    {
                        NewAttack = false;
                        AttackBufferOn = true;
                        AttackType = 0;
                    }
                }
                else if (Inp.HeavyAttack && SubActionTime > BufferStartTime && !AttackBufferOn)
                {
                    if (SequenceEnd == false && NewAttack)
                    {
                        NewAttack = false;
                        AttackBufferOn = true;
                        AttackType = 1;
                    }
                } 
                // HOLDING ATTACK
                else if (LightHold > AttackHoldThreshold) 
                {
                    if (SequenceEnd == false && NewAttack)
                    {
                        NewAttack = false;
                        AttackBufferOn = true;
                        AttackType = 0;
                    }
                }
                else if (HeavyHold > AttackHoldThreshold)
                {
                    if (SequenceEnd == false && NewAttack)
                    {
                        NewAttack = false;
                        AttackBufferOn = true;
                        AttackType = 1;
                    }
                }

                // PREPARE FOR NEXT ATTACK
                if (AttackBufferOn)
                {
                    if (SubActionTime > AttackNextTime)
                    {
                        if (AttackType == 0)
                        {
                            if (LightHold > AttackHoldThreshold) { TriggerLightHoldAttack = true; }
                            else { TriggerLightAttack = true; }
                        }
                        if (AttackType == 1)
                        {
                            if (HeavyHold > AttackHoldThreshold) { TriggerHeavyHoldAttack = true; }
                            else { TriggerHeavyAttack = true; }
                        }
                    }
                }

                // END ATTACK WITH DASH OR JUMP
                if (CheckForJumpOrDash()) { return; }

                // END IF CHANGED GROUNDED STATE
                if (GroundedAttack && !Char.Grounded) { EndAttack(); }

                if (AirAttackExitAfterEnd) 
                {
                    if (!GroundedAttack && Char.Grounded && SubActionTime > AttackEndTime + 0.05f) { EndAttack(); }
                    if (Char.Sliding) { EndAttack(); }
                }
                else
                {
                    if (!GroundedAttack && Char.Grounded && SubActionTime > AttackNextTime + 0.05f) { EndAttack(); }
                    if (Char.Sliding) { EndAttack(); }
                }

                // CHANGE STUFF WITH TARGET
                if(Inp.CurrentTarget != null && UseSoftLock) { SoftLock(); }

                // ROTATE SKIN
                if (Actions.Basic != null && SubActionTime < SkinRotationThreshold)
                {   
                    if(Inp.InputMag > 0.01f) { prevInput = Vector3.ProjectOnPlane(Inp.CamRelativeInput.normalized, -Char.GravityDir); }
                    else 
                    {
                        switch (PreviousInputSkinDirection)
                        {
                            case VectorDirection.Forward:
                                prevInput = Skin.transform.forward;
                                break;
                            case VectorDirection.Back:
                                prevInput = -Skin.transform.forward;
                                break;
                            case VectorDirection.Right:
                                prevInput = Skin.transform.right;
                                break;
                            case VectorDirection.Left:
                                prevInput = -Skin.transform.right;
                                break;
                            case VectorDirection.Up:
                                prevInput = Skin.transform.up;
                                break;
                            case VectorDirection.Down:
                                prevInput = -Skin.transform.up;
                                break;
                        }

                        //prevInput = Skin.transform.forward; // PROBLEMATIC LINE
                        //prevInput = Inp.CamRelativePreviousInput;
                    }

                    Actions.Basic.prevInput = prevInput;
                    Actions.Basic.SkinRotation(prevInput, -Char.GravityDir, SkinRotationSpeed, 0);
                }

            }

            CheckForShot();
            CheckForBlockInput();
            ParryCounter = 0;
        }
        else if(SubAction == 1) // BLOCKING & PARRY
        {
            ParryCounter += Time.fixedDeltaTime;
            if(SubActionTime < 1.0f)
            {
                IHB.StartIHB(Actions.Attacks.ParryStart);
                BlockingAttackBuffer = false;
                anim.SetTrigger("BlockStart");
                SubActionTime = 1;
            }

            // END BLOCK
            if(Inp.BlockHold == false && SubActionTime > BlockReleaseTime + 1)
            {
                StartCoroutine(BlockCooldownCoroutine(BlockingCooldown));
                Actions.SwitchAction(0);
            }

            // DRAG
            localspeed = transform.InverseTransformDirection(rigid.linearVelocity);
            localspeed.x = Mathf.Lerp(localspeed.x, 0, Time.fixedDeltaTime * BlockingDrag);
            localspeed.z = Mathf.Lerp(localspeed.z, 0, Time.fixedDeltaTime * BlockingDrag);
            rigid.linearVelocity = transform.TransformDirection(localspeed);
            SubActionTime += Time.fixedDeltaTime;

            // ROTATE SKIN
            if (Actions.Basic && SubActionTime < BlockingSkinRotationThreshold + 1)
            {
                if (Inp.InputMag > 0.01f) { prevInput = Vector3.ProjectOnPlane(Inp.CamRelativeInput.normalized, -Char.GravityDir); }
                else { prevInput = Skin.transform.forward; }
                Actions.Basic.prevInput = prevInput;
                Actions.Basic.SkinRotation(prevInput, -Char.GravityDir, SkinRotationSpeed, 0);
            }

            // FINAL CHECKS
            if (Inp.Jump && Char.Grounded)
            {
                if (Actions.Basic)
                {
                    EndAttack();
                    Actions.SwitchAction(0);
                    Actions.Basic.SubAction = 1;
                    Actions.Basic.SubActionTime = 0;
                    Actions.Basic.CoyoteActive = true;
                    Actions.Basic.CheckJump(false);
                    StartCoroutine(BlockCooldownCoroutine(BlockingCooldown));
                    return;
                }
            }
            if (Inp.Dash && Basic.AirDashAvailable)
            {
                if (Actions.Basic)
                {
                    EndAttack();
                    Actions.SwitchAction(0);
                    rigid.linearDamping = 0;
                    Actions.Basic.SubAction = 4;
                    Actions.Basic.SubActionTime = 0;
                    Actions.Basic.CheckForDash();
                    StartCoroutine(BlockCooldownCoroutine(BlockingCooldown));
                    return;
                }
            }

            // SET ATTACK BUFFER
            if (BlockingAttackBuffer == false && (Inp.LightAttack || Inp.HeavyAttack))
            {
                if (Inp.LightAttack) { LastAttackType = 0; }
                if (Inp.HeavyAttack) { LastAttackType = 1; }
                BlockingAttackBuffer = true;
            }

            // DO ATTACK BUFFER
            if (BlockingAttackBuffer && SubActionTime > BlockingAttackTime + 1) 
            { 
                // CORRECT FOR GROUND
                if(Char.Grounded == false)
                {
                    if (LastAttackType == 0) { LastAttackType = 2; }
                    if (LastAttackType == 1) { LastAttackType = 3; }
                }

                StartCoroutine(BlockCooldownCoroutine(BlockingCooldown));
                StartAttack(LastAttackType);
                BlockingAttackBuffer = false; 
            }
        }
        else if(SubAction == 2) // HURT AND DAMAGE
        {
            // INITIAL SETS
            ParryCounter = 0;
            localspeed = transform.InverseTransformDirection(rigid.linearVelocity);

            // INITIAL HURT ANIM
            if (SubActionTime < 1f)
            {
                if (HitHitBox != null)
                {
                    if (HitHitBox.StaggerType == HitboxData.DamageStaggerType.Normal)
                    {
                        // CHECK IF ALREADY FLYING
                        SetHurtAnim();
                    }
                    else if (HitHitBox.StaggerType == HitboxData.DamageStaggerType.Strong)
                    {
                        if (HurtFlying == false) // MAKE FLY
                        {
                            anim.SetTrigger("Damage_Fly");
                            HurtFlying = true;
                            SetHurtAnim();
                        }
                        else
                        {
                            SetHurtAnim();
                        }
                    }
                }
                else
                {
                    if (HurtFlying) { anim.SetTrigger("Damage_Fly"); } 
                    else { anim.SetTrigger("Damage_Flinch"); }

                }

                //wallBounceRay = worldHitForce;
                WallBounceCount = 0;
                SubActionTime = 1;
            }
            else
            {
                // APPLY DRAG
                if (!HurtFlying)
                {
                    if (Char.Grounded)
                    {
                        if (Char.SpeedMagnitude < 1) { Char.rigid.linearDamping = 1 * HurtGroundDragMultiplier; }
                        else { Char.rigid.linearDamping = 1 * HurtGroundDragMultiplier; }
                    }
                    else
                    {
                        Char.rigid.linearDamping = 0.1f * HurtAirDragMultiplier;
                    }
                }
                else
                {
                    Basic.landingtime = 2;
                    Char.rigid.linearDamping = 0;
                    if (Char.Grounded) 
                    {
                        if (Char.SpeedMagnitude < 1) { Char.rigid.linearDamping = 1 * HurtGroundDragMultiplier; }
                        else { Char.rigid.linearDamping = 0.5f * HurtGroundDragMultiplier; }
                    }
                    else 
                    {
                        SidewaysDrag(0.1f * HurtAirDragMultiplier); 
                    }

                    // WALL BOUNCE
                    WallBounceCount += Time.fixedDeltaTime;
                    wallBounceRay = Vector3.ProjectOnPlane(wallBounceRay, Char.GravityDir);
                    wallBounceRay = Vector3.ProjectOnPlane(wallBounceRay, -Char.GravityDir).normalized;
                    if (WallBounceCount > 0.15f && Char.SpeedMagnitude > WallBounceMinSpeed)
                    {
                        if (Physics.Raycast(transform.position, wallBounceRay, out hit, WallRayDistance, WallBounceMask))
                        {
                            // ACTUALLY BOUNCE
                            WallBounceCount = 0;
                            wallBounceReflected = Vector3.Reflect(wallBounceRay, hit.normal);
                            //if (rigid.velocity.magnitude > 1) { rigid.velocity = Vector3.Reflect(rigid.velocity, hit.normal) * WallBounceMultiplier; }
                            //else { rigid.velocity = wallBounceRay * WallBounceMultiplier; }
                            if (Vector3.Dot(wallBounceRay, hit.normal) < 0)
                            {
                                rigid.linearVelocity = wallBounceReflected * Char.SpeedMagnitude * WallBounceMultiplier;
                            }

                            anim.SetTrigger("Damage_Fly");

                            // WALL UP BOUNCE
                            if(localspeed.y < 0) { rigid.linearVelocity += transform.up * WallUpBounce; }

                            // ROTATE SKIN
                            wallBounceRay = Vector3.ProjectOnPlane(-rigid.linearVelocity, transform.up);
                            wallBounceRay = Vector3.ProjectOnPlane(-rigid.linearVelocity, -transform.up).normalized;
                            Actions.Basic.SkinRotation(wallBounceRay, transform.up, 9999, 0);

                            // WALL IHB
                            if (WallHitIHB) 
                            { 
                                IHB.StartIHB(WallHitIHB);
                                WallHitIHB.transform.rotation = Quaternion.LookRotation(transform.up, hit.normal);
                            }
                        }
                    }
                }

                // END HURT
                if (SubActionTime > 10 || anim.GetBool("Idle"))
                {
                    HurtFlying = false; 
                    Actions.SwitchAction(0);
                    Actions.Basic.SubAction = 0;
                    Actions.Basic.SubActionTime = 0;
                    Actions.Basic.CoyoteActive = true;
                    return;
                }

                // FAILED PARRY
                if (Actions.Interactions && !HurtFlying)
                {
                    if(Inp.Block && (SubActionTime - 1) < ParryMissThreshold)
                    {
                        if (ParryMiss) { IHB.StartIHB(ParryMiss); }
                        Actions.Interactions.Hp += Actions.Interactions.LastDamageAmmTaken;
                        Actions.Interactions.LastDamageAmmTaken = 0;
                    }
                }

                // CANCEL FLINCH
                if (!HurtFlying && HurtCounter > FlinchRecoveryTime)
                {
                    HurtFlying = false;
                    if (Inp.InputMag > 0.5f) 
                    { Actions.SwitchAction(0); }
                    CheckForJumpOrDash();
                }
                else if(HurtFlying && HurtCounter > StaggerRecoveryTime)
                {
                    HurtFlying = false;
                    if (Inp.InputMag > 0.5f)
                    { Actions.SwitchAction(0); }
                    CheckForJumpOrDash();
                }
            }

            // COUNTERS
            SubActionTime += Time.fixedDeltaTime;
            // HURT COUNTER COUNTS SLOWER IF ON THE AIR
            if (Char.Grounded) { HurtCounter += Time.fixedDeltaTime; }
            else { HurtCounter += Time.fixedDeltaTime / 4; }

            void SetHurtAnim()
            {
                if (HurtFlying == false)
                {
                    if (Stats.Ap > 0 && !Stats.ApRecoverMode) { anim.SetTrigger("Damage_Flinch"); }
                    else { anim.SetTrigger("Damage_Stagger"); }
                }
                else
                {
                    if (Char.Grounded) { anim.SetTrigger("Damage_OnGroundFell"); }
                    else { anim.SetTrigger("Damage_Fly"); }
                }
            }

        }
        else if(SubAction == 3) // SPECIAL MOVES
        {
            SubActionTime += Time.deltaTime;

            // MISC
            if (Inp.CurrentTarget != null && UseSoftLock) 
            { 
                SoftLock();
                if (ApplyDragOnSpecial)
                { 
                    rigid.linearDamping = Mathf.Lerp(rigid.linearDamping, GroundDrag, Time.fixedDeltaTime * 5); 
                }
                else
                {
                    rigid.linearDamping = 0;
                }
            }
            else
            {
                if (ApplyDragOnSpecial)
                {
                    rigid.linearVelocity = Vector3.zero;
                    rigid.linearDamping = Mathf.Lerp(rigid.linearDamping, GroundDrag, Time.fixedDeltaTime * 5);
                }
                else
                {
                    rigid.linearDamping = 0;
                }
                
                if (Actions.Basic != null && SubActionTime < SkinRotationThreshold && UseSpecialSkinRotation)
                {
                    if (Inp.InputMag > 0.01f) { prevInput = Vector3.ProjectOnPlane(Inp.CamRelativeInput.normalized, -Char.GravityDir); }
                    else { prevInput = Skin.transform.forward; }
                    Actions.Basic.prevInput = prevInput;

                    if (SpecialForceSkinRotationToTarget)
                    {
                        if (Inp.CurrentTarget)
                        {
                            tgtDir = (Inp.CurrentTarget.transform.position - transform.position).normalized;
                            Actions.Basic.SkinRotation(Vector3.ProjectOnPlane(tgtDir, -Char.GravityDir), -Char.GravityDir, SkinRotationSpeed, 0);
                        }
                        else
                        {
                            Actions.Basic.SkinRotation(prevInput, -Char.GravityDir, SkinRotationSpeed, 0);
                        }
                    }
                    else
                    {
                        Actions.Basic.SkinRotation(prevInput, -Char.GravityDir, SkinRotationSpeed, 0);
                    }
                }
            }

            if (CheckForJumpOrDash()) 
            {
                for (int i = 0; i < SpecialAttacksToResetOnExit.Count; i++)
                {
                    StopAllCoroutines();
                    IHB.StopIHB(SpecialAttacksToResetOnExit[i]);
                }
                return;
            }

            // END SPECIAL
            if (CheckEndSpecial(false))
            {
                HurtFlying = false;
                Actions.SwitchAction(0);
                Actions.Basic.SubAction = 0;
                Actions.Basic.SubActionTime = 0;
                return;
            }

            bool CheckEndSpecial(bool forceEnd)
            {
                if(SubActionTime > SpecialEarlyExitTime && Inp.InputMag > 0.5f) { return true; }
                if(SubActionTime > SpecialForceExitTime || anim.GetBool("Idle")) { return true; }
                if (forceEnd) { return true; }
                return false;
            }

            // SPECIALS
            if (Actions.Spc.CurrentMove.Name == "Charged Jester Dash")
            {
                anim.SetBool("b_hold", Inp.JDashHold);
            }
            else if(Actions.Spc.CurrentMove.Name == "Down Dash")
            {
                if (Char.Grounded) { CheckEndSpecial(true); }
            }
        }
        else if(SubAction == 4) // SHOT
        {
            if(SubActionTime < 0.01f)
            {
                SubActionTime = 0.02f;
            }

            // SETS & SKIN ROT
            if (Inp.CurrentTarget)
            {
                TargetDir = (ShotCenter.position - Inp.CurrentTarget.transform.position).normalized;
                anim.SetFloat("UpDown", Vector3.Dot(transform.up, TargetDir));

                prevInput = Vector3.ProjectOnPlane(-TargetDir, -Char.GravityDir);
                Actions.Basic.SkinRotation(prevInput, -Char.GravityDir, 999, 0);
            }
            else
            {
                if (Inp.InputMag > 0.01f) { prevInput = Vector3.ProjectOnPlane(Inp.CamRelativeInput, -Char.GravityDir); }
                else { prevInput = Vector3.ProjectOnPlane(Skin.transform.forward, -Char.GravityDir); }
                Actions.Basic.SkinRotation(prevInput, -Char.GravityDir, 999, 0);
            }

            // JUMP EXIT
            if (Inp.JumpHold)
            {
                Actions.SwitchAction(0);
                Actions.Basic.CheckJump(false);
            }

            // EXIT
            if (SubActionTime > ShotDuration) { Actions.SwitchAction(0); }
            Actions.Basic.Movement();
            Actions.Basic.CheckForDash();
            SubActionTime += Time.deltaTime;
        }

        // OTHER ACTIONS
        if (Actions.Rail != null) { Actions.Rail.LookForRails(); }

        // FINAL SETS
        anim.SetLayerWeight(Actions.Basic.LandingLayer, 0);
        anim.SetInteger("Action", 1);
        anim.SetInteger("SubAction", SubAction);
        anim.SetFloat("Speed", localspeed.x + localspeed.z);
        anim.SetBool("Grounded", Char.Grounded);
        anim.SetFloat("FallSpeed", localspeed.y);
        anim.SetBool("Combat", Basic.InCombat);
        anim.SetFloat("AT_LightHold", LightHold);
        anim.SetFloat("AT_HeavyHold", HeavyHold);

        if (Char.Grounded) { anim.SetFloat("GroundFloat", 1.0f); } 
        else { anim.SetFloat("GroundFloat", 0f); }
    }

    public bool CheckForAttack()
    {
        if (!Char.Sliding)
        {
            if (Char.Grounded)
            {
                if (Inp.LightAttack)
                {
                    LastAttackType = 0;
                    Actions.Attacks.StartAttack(0);
                    return true;
                }
                else if (Inp.HeavyAttack)
                {
                    LastAttackType = 1;
                    Actions.Attacks.StartAttack(1);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Inp.LightAttack)
                {
                    LastAttackType = 2;
                    Actions.Attacks.StartAttack(2);
                    return true;
                }
                else if (Inp.HeavyAttack)
                {
                    LastAttackType = 3;
                    Actions.Attacks.StartAttack(3);
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        else
        {
            return false;
        }
    }

    public bool CheckForJumpOrDash()
    {
        if (Inp.Jump && Char.Grounded)
        {
            if (Actions.Basic)
            {
                EndAttack();
                Actions.SwitchAction(0);
                Actions.Basic.SubAction = 1;
                Actions.Basic.SubActionTime = 0;
                Actions.Basic.CoyoteActive = false;
                Actions.Basic.AttackCounter = -AttackDelayOnCancel;
                Actions.Basic.CheckJump(false);
                return true;
            }
        }

        if (Inp.Dash)
        {
            if (Actions.Basic)
            {
                if (Actions.Basic.AirDashAvailable)
                {
                    EndAttack();
                    Actions.SwitchAction(0);
                    rigid.linearDamping = 0;
                    Actions.Basic.SubAction = 4;
                    Actions.Basic.DashAvailable = false;
                    Actions.Basic.AirDashAvailable = false;
                    Actions.Basic.SubActionTime = 0;
                    Actions.Basic.AttackCounter = -AttackDelayOnCancel;
                    Actions.Basic.AudioDash.Play();
                    Actions.Basic.CheckForDash();
                    return true;
                }
            }
        }

        return false;
    }

    public void EndAttack()
    {
        anim.ResetTrigger("LightAttack");
        anim.ResetTrigger("HeavyAttack");
        anim.ResetTrigger("NextLight");
        anim.ResetTrigger("NextHeavy");
        TriggerLightAttack = false;
        TriggerHeavyAttack = false;
        Attacking = false;
        AttackBufferOn = false;
        Actions.SwitchAction(0);
        Actions.Basic.SubAction = 0;
        Actions.Basic.SubActionTime = 0;
        IndependedAttackTime = 0;
    }

    public void DoHurtPlayerAnim(int DamageType, bool ForceAnimation, int ForcedAnimation, Vector3 dir, Vector3 upDir, HitboxData hit, CharacterInteractions stats)
    {
        //0: FLINCH
        //1: STAGGER + FLY

        if (!ForceAnimation)
        {
            BasicHurt();
        }
        else
        {
            BasicHurt();
            Actions.Attacks.HurtType = ForcedAnimation;
            
        }

        void BasicHurt()
        {
            if (hit.ForceType == HitboxData.DamageForceType.Set) { Char.rigid.linearDamping = 0; }
            SubActionTime = 0;
            Actions.Attacks.SubActionTime = 0;
            Actions.Attacks.SubAction = 2;
            Actions.Attacks.HurtType = DamageType;
            Actions.SwitchAction(1);
            Stats = stats;
            if (hit != null) { HitHitBox = hit; }

            // SET VELOCITY
            HurtUpForce = hit.UpwardForce;
            if (hit.OnlyUpwardsIfInAir && Char.Grounded) { HurtUpForce = 0; }
            hitForce = dir;
            worldHitForce = dir;
            wallBounceRay = dir - upDir;
            hitForce = transform.TransformDirection(hitForce);

            // TYPE
            if (hit.ForceType == HitboxData.DamageForceType.Set) { Char.rigid.linearVelocity = dir; }
            else if (hit.ForceType == HitboxData.DamageForceType.Additive) { Char.rigid.linearVelocity += dir; }
            else if (hit.ForceType == HitboxData.DamageForceType.OnlyAddIfGrounded)
            {
                if (Char.Grounded) { Char.rigid.linearVelocity += dir; }
                else { Char.rigid.linearVelocity = dir; }
            }

            // POP OUT OF THE GROUND IF GOIN UP
            if (Char.Grounded && upDir != Vector3.zero && hit.UpwardForce > 0.0001f)
            {
                Char.CheckGroundTime = -HurtAirGroundCheckInterval;
                transform.Translate(transform.up * HurtAirDetachForce);
            }

            // ROTATION (NOTE: Funcion is not triggered if character poised trough attack)
            if (Stats.RotateOnHit && hit.StaggerType == HitboxData.DamageStaggerType.Normal) { RotateSkin(); }
            else if (Stats.RotateOnStrongHit && hit.StaggerType == HitboxData.DamageStaggerType.Normal) { RotateSkin(); }
        }

        void RotateSkin()
        {
            if (hit.FakeCenter) { hitDirProxy = hit.FakeCenter.position; }
            else if (hit.Parent) { hitDirProxy = hit.Parent.position; }
            else { hitDirProxy = hit.transform.position; }

            hitDirProxy = Vector3.ProjectOnPlane((transform.position - hitDirProxy), -Char.GravityDir).normalized;
            Actions.Basic.prevInput = -hitDirProxy;
            Actions.Basic.SkinRotation(Vector3.ProjectOnPlane(-hitDirProxy, -Char.GravityDir), -Char.GravityDir, 9999, 0);
        }
    }

    void SidewaysDrag(float amm)
    {
        localspeed = transform.InverseTransformDirection(rigid.linearVelocity);
        localspeed.x = Mathf.Lerp(localspeed.x, 0, Time.fixedDeltaTime * amm);
        localspeed.z = Mathf.Lerp(localspeed.z, 0, Time.fixedDeltaTime * amm);
        rigid.linearVelocity = transform.TransformDirection(localspeed);
    }

    public void CheckForShot()
    {
        if(HasShot == false) { return; }

        // SHOOT NORMAL SHOT
        if (Inp.Shot)
        {
            Actions.SwitchAction(1);
            SubAction = 4;
            SubActionTime = 0;
            anim.SetTrigger("SmallShot");

            // PROJECTILE
            if (Inp.CurrentTarget)
            {
                TargetDir = (ShotCenter.position - Inp.CurrentTarget.transform.position).normalized;
                ShotSmallIHB.transform.rotation = Quaternion.LookRotation(-TargetDir, -Char.GravityDir);
            }
            else
            {
                ShotSmallIHB.transform.rotation = Quaternion.LookRotation
                    (Vector3.ProjectOnPlane(Skin.forward, -Char.GravityDir), -Char.GravityDir);
            }

            IHB.StartIHB(ShotSmallIHB);
        }

        // SHOT STRONG
        if (Inp.ShotHold == false && ChargeShotCharge > ShotChargeDuration)
        {
            Actions.SwitchAction(1);
            SubAction = 4;
            SubActionTime = 0;
            anim.SetTrigger("ChagedShot");

            // PROJECTILE
            if (Inp.CurrentTarget)
            {
                TargetDir = (ShotCenter.position - Inp.CurrentTarget.transform.position).normalized;
                ShotLargeIHB.transform.rotation = Quaternion.LookRotation(-TargetDir, -Char.GravityDir);
            }
            else
            {
                ShotLargeIHB.transform.rotation = Quaternion.LookRotation
                    (Vector3.ProjectOnPlane(Skin.forward, -Char.GravityDir), -Char.GravityDir);
            }

            // SHOT BIG
            IHB.StartIHB(ShotLargeIHB);
        }

        if (Inp.ShotHold) 
        { 
            ChargeShotCharge += Time.fixedDeltaTime;
            if (ChargeShotCharge > 0.1f)
            {
                if(ShotChargeAudioSource.isPlaying == false && ChargeShotCharge < 0.2f) 
                { ShotChargeAudioSource.Play(); }

                // PARTICLE
                if (ChargeShotCharge < ShotChargeDuration)
                {
                    var emi = ShotChargingParticle.emission;
                    emi.rateOverTime = ShotCharging_Emission;
                    if (ShotChargingParticle.isPlaying == false) { ShotChargingParticle.Play(); }
                }
                else
                {
                    var emi = ShotChargedParticle.emission;
                    emi.rateOverTime = ShotCharged_Emission;
                    if (ShotChargedParticle.isPlaying == false) { ShotChargedParticle.Play(); ShotChargingParticle.Stop(); }
                }
            }
        }
        else 
        { 
            ChargeShotCharge = 0;
            if (ShotChargingParticle.isPlaying) { ShotChargingParticle.Stop(); }
            if (ShotChargedParticle.isPlaying) { ShotChargedParticle.Stop(); }
            if (ShotChargeAudioSource.isPlaying) { ShotChargeAudioSource.Stop(); }
        }
    }

    void SoftLock()
    {
        // SET DIST AND DOT
        tgtDist = Vector3.Distance(transform.position, Inp.CurrentTarget.transform.position);
        tgtDir = (Inp.CurrentTarget.transform.position - transform.position).normalized;
        SetDeltaPositionBasedOnGroundNormal();

        if (Inp.InputMag > 0.1f) { tgtDot = Vector3.Dot(tgtDir, Inp.CamRelativeInput); }
        else 
        {
            tgtDot = 1;
            //tgtDot = Vector3.Dot(tgtDir, Skin.transform.forward);
        }

        // MOVE PLAYER TOWARDS THE TARGET OR PUSH AWAY
        if (Char.Grounded && RootMotion && tgtDot > SoftLockDotMin)
        {
            if (BypassGapCloser == false)
            {
                // GAP CLOSER
                if (tgtDist < MaxSoftLockDistance + Inp.CurrentTarget.TargetRadius && tgtDist > MinSoftLockDistance + Inp.CurrentTarget.TargetRadius)
                {
                    if (SubActionTime < SoftLockGapCloserDuration)
                    {
                        RootMotion.EnableRootMotion = false;
                        //transform.position += Vector3.ProjectOnPlane(tgtDir, Char.GroundNormal) * (SoftLockGapCloserPower * Time.fixedDeltaTime);

                        // ONLY APPLY EXTRA ROOT MOTION IF FACING, OR ELSE, NORMAL MOTION
                        if (Vector3.Dot(tgtDir, -RootMotion.DeltaPosition.normalized) < 0)
                        {
                            if (!RootMotion.ClipFixRay(Char.rigid.position, deltaPosition, deltaPosition.magnitude * SoftLockGapCloserRootMotionPower))
                            { Char.rigid.position += deltaPosition * (SoftLockGapCloserRootMotionPower); }
                        }
                        else
                        {
                            if (!RootMotion.ClipFixRay(Char.rigid.position, deltaPosition, deltaPosition.magnitude))
                            { Char.rigid.position += deltaPosition; }
                        }

                    }
                    else
                    {
                        if (!RootMotion.ClipFixRay(Char.rigid.position, deltaPosition, deltaPosition.magnitude * (SoftLockGapCloserPower * Time.fixedDeltaTime)))
                        {
                            Char.rigid.position += deltaPosition * (SoftLockGapCloserPower * Time.fixedDeltaTime);
                            RootMotion.EnableRootMotion = true;
                        }

                    }
                }
                // CLOSE TO TARGET
                else if (tgtDist < MinSoftLockDistance + Inp.CurrentTarget.TargetRadius)
                {
                    RootMotion.EnableRootMotion = true;
                    if (tgtDist < Inp.CurrentTarget.TargetRadius) // PUSH PLAYER AWAY
                    {
                        RootMotion.EnableRootMotion = false;
                        // ONLY APPLY EXTRA ROOT MOTION IF FACING AWAY
                        if (Vector3.Dot(tgtDir, -RootMotion.DeltaPosition.normalized) > 0)
                        {
                            if (!RootMotion.ClipFixRay(Char.rigid.position, deltaPosition * (SoftLockGapCloserRootMotionPower), deltaPosition.magnitude))
                            {
                                transform.position += deltaPosition * (SoftLockGapCloserRootMotionPower);
                            }
                        }

                        transform.position -= Vector3.ProjectOnPlane(tgtDir, Char.GroundNormal) * (SoftLockPushAwayPower * Time.fixedDeltaTime);
                    }
                }
                else
                {
                    RootMotion.EnableRootMotion = true;
                }
            }
            else
            {
                RootMotion.enabled = true;
            }

            // ROTATE PLAYER
            if(SubActionTime < SoftLockGapCloserDuration)
            {
                Actions.Basic.SkinRotation(Vector3.ProjectOnPlane(tgtDir, -Char.GravityDir), -Char.GravityDir, SkinRotationSpeed, 0);
            }
        }
        else if (!Char.Grounded) // WHILE IN AIR
        {
            AirSoftLockPos = transform.position;
            if (BypassGapCloser == false)
            {
                if (tgtDot > 0 && (tgtDist + Inp.CurrentTarget.TargetRadius) < AirSfotLockMinDistance)
                {
                    // MOVE TO TARGET
                    if (SubActionTime < AirSoftLockTime)
                    {
                        AirPosProxy = Inp.CurrentTarget.transform.position
                            + (Vector3.ProjectOnPlane(-tgtDir, -Char.GravityDir) * Inp.CurrentTarget.SecondaryRadius);

                        //if(RootMotion.ClipFixRay(AirSoftLockPos, (AirSoftLockPos - AirPosProxy).normalized, (AirPosProxy - AirPosProxy).magnitude) ))
                        if (Char.SideHit == false)
                        {
                            AirSoftLockPos = Vector3.Lerp(AirSoftLockPos, AirPosProxy, Time.fixedDeltaTime * AirSoftLockSpeed);
                            Char.rigid.MovePosition(AirSoftLockPos);
                        }

                        rigid.linearVelocity = Vector3.zero;
                        ApplyAirSoftLockDrag(AirSoftLockDrag);
                    }
                }
            }

            // ROTATE PLAYER (NOTE, was inside "bypassGapCloser" but was moved here)
            if (SubActionTime < SoftLockGapCloserDuration)
            {
                Actions.Basic.SkinRotation(Vector3.ProjectOnPlane(tgtDir, -Char.GravityDir), -Char.GravityDir, SkinRotationSpeed, 0);
            }

            void ApplyAirSoftLockDrag(float drag)
            {
                if(Char.SpeedMagnitude > 1) { rigid.linearDamping = AirSoftLockDrag; }
                else { rigid.linearDamping = 99; }
            }
        }
        else
        {
            RootMotion.EnableRootMotion = true;
        }

        void SetDeltaPositionBasedOnGroundNormal()
        {
            deltaPosition = Vector3.ProjectOnPlane(RootMotion.DeltaPosition, Char.GroundNormal);
            deltaPosition = Vector3.ProjectOnPlane(RootMotion.DeltaPosition, -Char.GroundNormal);
        }

        //Debug.DrawRay(transform.position, tgtDir * 5);
        //Debug.DrawRay(transform.position, Inp.CamRelativeInput * 5);
        //Debug.DrawRay(Inp.CurrentTarget.transform.position, Vector3.forward * Inp.CurrentTarget.TargetRadius);
    }

    public IEnumerator BlockCooldownCoroutine(float time)
    {
        BlockingAvailable = false;
        yield return new WaitForSeconds(time);
        BlockingAvailable = true;
    }

    public void StartHiboxCoroutine(float time, int box)
    {
        if (AttackHitboxes.Count > box)
        {
            if(AttackHitboxes[box] != null)
            {
                StartCoroutine(HitboxCoroutine(time, box));
            }
            else
            {
                Debug.LogError("Invalid Hitbox ID");
            }
        }
    }

    public IEnumerator HitboxCoroutine(float time, int box)
    {
        yield return new WaitForSeconds(time);
        IHB.StartIHB(AttackHitboxes[box]);
    }

    public IEnumerator StopHitboxCoroutine(float time, int box)
    {
        yield return new WaitForSeconds(time);
        IHB.StopIHB(AttackHitboxes[box]);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(AirPosProxy, Vector3.one * 0.1f);
    }

}
