//using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class CharacterBasicActions : MonoBehaviour
{
    [Header("References")]
    public CharacterPhysics Char;
    public CharacterInput Inp;
    public CharacterActions Actions;
    public Rigidbody rigid;
    public Transform CameraTarget;
    public Animator anim;
    public Transform Skin;
    public int SubAction = 0;

    [Header("Movement Values - 00")]
    public float TopSpeed = 1;
    public float MoveAccell = 0.5f;
    public float AirMoveAccell = 0.5f;
    public float MaxSpeed = 300;
    public float TangentialDrag = 10;
    public float AirTopSpeed = 1;
    public float AirTangentialDrag = 1;
    public float AirControlMultiplier = 1.5f;
    public float GroundDrag = 10;
    public float SkinRotationMinSpeed = 1;
    public AnimationCurve GroundedDragOverSpeed;
    public float AirDrag = 0.01f;
    [HideInInspector] public float landingtime = 0;

    [Header("High Speed - 00")]
    public float HighSpeedThreshold = 5;
    public float HS_TurnignRadius = 0.2f;
    public float HS_CameraForce = 2;
    public float HS_CameraHeight = 0.5f;
    public float HS_FacingThreshold = 0.5f;
    public float HS_TiltMultiplier = 0.1f;
    public float HS_TiltDeadZone = 0.05f;
    public Vector3 HS_Input;
    bool HS_SkinTilt = false;

    [Header("Slope Values - 00")]
    public float SlopeDownAcceleration = 0.1f;
    public float SlopeUpDeceleration = 0.1f;
    public float SlopeLandingAcceleration = 1;

    [Header("Overspeed - 00")]
    public float OverSpeedMin = 4;
    public float OverspeedMax = 9;
    public float OverspeedPower = 1;

    [Header("Skidding - 00")]
    public float SkiddingPower = 10;
    public float SkiddingAirPower = 1;
    public float SkiddingThreshold = -1;
    public AnimationCurve SkiddingOverSpeed;
    public bool IsSkidding = false;

    [Header("Jump Value - 01")]
    public float CoyoteTime = 1;
    public float JumpDetachDistance = 0.5f;
    public float JumpDuration = 1;
    public float JumpInitialForce = 1;
    public float JumpForce = 1;
    public float GroundCheckInterval = 0.3f;
    public float DoubleJumpInitialForce = 0.5f;
    public float DoubleJumpDuration = 1f;
    public float DoubleJumpFroce = 1;
    public GameObject DoubleJumpIHB;

    [Header("Wall Jump Value - 02 / 03")]
    public float WallCoyoteTime = 0.5f;
    public float WallRayDistance = 1;
    public float WallPushForce = 1;
    public float WallJumpPower = 5;
    public float WallJumpUpPower = 2;
    Vector3 wallnormal;

    [Header("Dash - 04")]
    public bool DashAvailable = true;
    public bool AirDash = true;
    public bool AirDashAvailable = false;
    public float DashSpeed = 4;
    public float DashSpeedChangeDuration = 0.1f;
    public float DashDuration = 0.6f;
    public float DashSkinRotSpeed = 4;
    public float DashMaxSpeed = 7.5f;
    public GameObject DashIHB;

    [Header("Wall Walking - 05")]
    public float WallWalkMinSpeed = 5f;
    public float WallWalkDuration = 2.5f;
    public float WallWalkInterval = 0.5f;
    public float WallWalkAttachForce = 2;
    public float WallWalkJumpSidewaysForce = 3.4f;
    public float WallWalkJumpUpForce = 3.8f;
    public float WallWalkCameraSpeed = 10;
    public AnimationCurve WallWalkJumpMultiplierOverSpeed;

    [Header("Jester Dash - 06")]
    public float JesterDashSpeed = 10;
    public float JesterDashTimeout = 2;
    public float JesterDashHitHeight = 4f;
    public float JesterDashHitSpeed = 2f;
    public TargetData JesterDashTarget;
    public GameObject JesterDashIHB;
    public GameObject JesterDashHitIHB;
    public GameObject JesterDashNone;
    public Vector3 JesterDashDir;
    public float MagnetDashPower = 10f;
    public float MagnetDashAccumulationForce = 1.3f;
    public float MagnetDashUpSpeed = 1;

    [Header("Ground Anim")]
    public float AnimSpeedSmoothing = 5;
    public float AnimRotationSpeed = 10;
    public float TiltSpeed = 3;
    public float TiltMax = 1;
    public AnimationCurve TiltOverSpeed;
    public int LandingLayer = 1;
    public float LandingAnimSpeed = 2;
    public float LandingAnimDuration = 1.2f;

    [Header("Air Anim")]
    public float AirAnimSpeedThreshold = 4;

    [Header("Audio")]
    public AudioSource AudioLanding;
    public AudioSource AudioDash;
    public AudioSource AudioJump;
    public AudioSource AudioWallStart;
    public AudioSource AudioWallJump;

    //[Header("Misc")]
    //public float SkinRotationCorrection = 90;
    //public float TiltRotationCorrection;

    [Header("Cache")]
    public bool AllowAttacking = true;
    Vector3 upTilt;
    float groundedTime;
    public float animSpeedParam;
    float fallspeedParam;
    float landedParam;
    Quaternion skinrot;
    Quaternion charrot;
    Vector3 vProxy;
    public Vector3 prevInput = Vector3.forward;
    Vector3 localspeed;
    public Vector3 finaltilt;
    public float SubActionTime = 0;
    public float WallTime = 0;
    public bool CoyoteActive = false;
    public bool WallCoyote = false;
    public float WallCoyoteTimeCounter = 0;
    public float airTime = 0;
    bool WallHit = false;
    RaycastHit RayHit;
    Vector3 wallrayDirection;
    Vector3 finalTangent;
    public bool InCombat = false;
    public float CombatCounter = 0;
    Vector3 MovingPlatformDeltaEdit;
    public float AttackCounter;
    public bool AttackBuffered = false;
    public int AttackBufferType = 0;
    public int SkinRotMode = 0;
    public bool DoubleJumpAvailable = false;
    Vector3 overspeed;
    Vector3 planespd;
    Vector3 planeinp;
    public float FinalJumpTime;
    public float FinalJumpForce;
    public float HS_Dot;
    public float WallWalkCurrentSpeed;
    public Vector3 WallWalkDir;
    public float WallwalkTime = 0;
    public float WallOutTime = 0;
    public Vector3 PreWallSpeed;
    public float WallWalkDot = 0;
    float wallWalkJumpMultiplier;
    bool wallhit;
    public float JdashObjectDistance;
    public Vector3 JdashInitialSpeed;
    public float MagnetDashInitialSpeed;
    Vector3 rotatedInput;

    private void Start()
    {
        //flippingSign = 1; // this is for the walljump raycast (hehe)
    }

    private void FixedUpdate()
    {
        // SETUP
        localspeed = transform.InverseTransformDirection(rigid.linearVelocity);
        anim.SetBool("Wall", false);
        anim.SetBool("Sliding", false);
        WallHit = false;

        Quaternion rot = Quaternion.FromToRotation(transform.up, Char.GroundNormal) * rigid.rotation;
        Vector3 f = rot * rigid.linearVelocity.normalized;

        if (Char.Grounded) 
        {
            CoyoteActive = true;
            airTime = 0; 
            groundedTime += Time.fixedDeltaTime; 
        }
        else 
        {
            anim.SetBool("Skid", false);
            if (airTime > CoyoteTime) { CoyoteActive = false; }
            landingtime = 0;  
            airTime += Time.fixedDeltaTime; 
            groundedTime = 0; 
        }

        // SUB ACTION
        if (SubAction == 0) // BASIC MOVEMENT
        {
            Movement();
            WallTime = 0;
            if (SubActionTime < 1.0f)
            {
                SubActionTime = 1;
            }

            SubActionTime += Time.fixedDeltaTime;
            animSpeedParam = Mathf.Lerp(animSpeedParam, Char.SpeedMagnitude, Time.fixedDeltaTime * AnimSpeedSmoothing);

            if (Char.Grounded)
            {
                DashAvailable = true;
                if (landingtime <= 0.1f)
                {
                    AudioLanding.Play();
                    anim.SetTrigger("Landed");
                    groundedTime = 1;
                    landedParam = LandingAnimDuration;
                    landingtime = 1;

                    // SLOPE LANDING
                    Char.SpeedAngle = Vector3.Dot(Char.SpeedDirection, -Char.GravityDir);
                    if (Char.SpeedAngle < 0.0f)
                    {
                        Char.rigid.AddForce((Char.SpeedDirection * Mathf.Abs(Char.SpeedAngle)) * SlopeLandingAcceleration,
                            ForceMode.Acceleration);
                    }
                }
                fallspeedParam = 0;
                landedParam -= Time.fixedDeltaTime * LandingAnimSpeed;

                // SKID
                if (Char.b_normalSpeed < SkiddingThreshold)
                {
                    anim.SetBool("Skid", true);
                    rigid.linearDamping = SkiddingPower * SkiddingOverSpeed.Evaluate(Char.SpeedMagnitude);
                    IsSkidding = true;
                }
                else
                {
                    anim.SetBool("Skid", false);
                    IsSkidding = false;
                }
            }
            else
            {
                groundedTime = 0;
                landedParam = 0;
                fallspeedParam = Mathf.Lerp(fallspeedParam, localspeed.y, Time.fixedDeltaTime * 40);

                // SKID AIR
                if (Char.b_normalSpeed < SkiddingThreshold)
                {
                    rigid.linearVelocity -= Inp.CamRelativeInput * (Time.fixedDeltaTime * SkiddingAirPower);
                    Debug.Log("AIR SKIDDING");
                    //rigid.linearDamping = SkiddingAirPower;
                    IsSkidding = true;
                }
                else
                {
                    IsSkidding = false;
                }
            }

            // SKIN ROT
            if (Char.SpeedMagnitude > 0.05f && !Char.Sliding && !IsSkidding)
            {
                if (Char.Grounded)
                {
                    SkinRotMode = 0;
                    if (Inp.InputMag > 0.01f)
                    {
                        // TILT AT HIGH SPEED
                        if (Char.SpeedMagnitude > HighSpeedThreshold)
                        {
                            rotatedInput = Inp.CamRelativeInput;
                            if(Inp.InputMag < 0.01f) { rotatedInput = Char.SpeedDirection; }
                            if (!HS_SkinTilt)
                            {
                                upTilt = Vector3.Lerp(rotatedInput, transform.up, 1);
                            }
                            else
                            {
                                upTilt = Vector3.Lerp(rotatedInput, transform.up,
                                    TiltOverSpeed.Evaluate(Char.SpeedMagnitude) * HS_TiltMultiplier);
                            }

                            finaltilt = Vector3.Lerp(finaltilt, upTilt, Time.fixedDeltaTime * (TiltSpeed * 3));
                            prevInput = Vector3.ProjectOnPlane(rigid.linearVelocity.normalized, transform.up);
                            SkinRotation(prevInput, finaltilt, AnimRotationSpeed, SkinRotMode);
                        }
                        else
                        {
                            // NORMAL SPEED (I think)
                            if (Inp.InputMag > 0.01f)
                            {
                                finalTangent = Vector3.ClampMagnitude(-Char.b_tangentVelocity, TiltMax);
                                upTilt = Vector3.Lerp(transform.TransformDirection(finalTangent), transform.up, TiltOverSpeed.Evaluate(Char.SpeedMagnitude));
                                finaltilt = Vector3.Lerp(finaltilt, upTilt, Time.fixedDeltaTime * TiltSpeed);
                            }

                            if (Inp.InputMag > 0.01f && Char.SpeedMagnitude > SkinRotationMinSpeed)
                            { prevInput = Vector3.ProjectOnPlane(Char.SpeedDirection, transform.up); }
                            else if (Inp.InputMag > 0.01f) { prevInput = Vector3.ProjectOnPlane(Inp.CamRelativeInput.normalized, transform.up); }
                            else { prevInput = Vector3.ProjectOnPlane(Char.SpeedDirection, transform.up); }
                            SkinRotation(prevInput, finaltilt, AnimRotationSpeed, SkinRotMode);
                        }
                    }
                    else
                    {
                        // NO INPUT
                        if (Char.SpeedMagnitude > SkinRotationMinSpeed) 
                        { 
                            prevInput = Char.SpeedDirection;
                            finaltilt = transform.up;
                        }
                        else
                        { 
                            prevInput = Vector3.ProjectOnPlane(prevInput, -Char.GravityDir);
                            finaltilt = Vector3.Lerp(finaltilt, -Char.GravityDir, Time.fixedDeltaTime * 2);
                        }

                        SkinRotation(prevInput, finaltilt, AnimRotationSpeed, SkinRotMode);

                        Debug.Log("5");
                    }
                }
                else
                {
                    InAirSkinRot();

                }
            }
            else if (IsSkidding && !Char.Sliding)
            {
                prevInput = Vector3.ProjectOnPlane(rigid.linearVelocity.normalized, -Char.GravityDir);
                finaltilt = finaltilt = -Char.GravityDir;
                SkinRotation(prevInput, finaltilt, AnimRotationSpeed, 0);
            }
            else if (Char.Sliding)
            {
                DashAvailable = false;
                anim.SetBool("Sliding", true);
                finaltilt = finaltilt = -Char.GravityDir;
                SkinRotation(prevInput, finaltilt, AnimRotationSpeed, 0);
            }
            else
            {
                prevInput = Vector3.ProjectOnPlane(prevInput, transform.up);
                finaltilt = transform.up;
                SkinRotation(prevInput, finaltilt, AnimRotationSpeed, 0);
            }
            

            // MISC
            if (InCombat)
            {
                CombatCounter -= Time.fixedDeltaTime;
                if (CombatCounter < 0) { InCombat = false; }
            }

            // ACTION TRANSITION
            if (!Char.Grounded) { CheckForWall(); }
            CheckForDash();
            CheckJump(false);
            CheckForAttackHere();
            Actions.Attacks.CheckForShot();
            anim.SetBool("Wall", false);
        }
        else if (SubAction == 1) // JUMP
        {
            Movement();
            // JUMP START
            if(SubActionTime < -0.5f) // DOUBLE JUMP
            {
                landedParam = 0;
                anim.ResetTrigger("WallJump");
                anim.SetBool("DoubleJump", true);

                rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, -Char.GravityDir);
                rigid.linearVelocity += (transform.up * DoubleJumpInitialForce);

                FinalJumpTime = DoubleJumpDuration;
                FinalJumpForce = DoubleJumpFroce;
                SubActionTime = 1;

                if (DoubleJumpIHB) { IHB.StartIHB(DoubleJumpIHB); }
            }
            else if (SubActionTime < 1.0f) // JUMP
            {
                landedParam = 0;
                anim.ResetTrigger("WallJump");
                anim.SetTrigger("Jump");

                rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, Char.GroundNormal);
                MovingPlatformDeltaEdit = Vector3.Scale(Char.MovingPlatformDeltaStored, -Char.GravityDir);
                rigid.position += (transform.up * JumpDetachDistance);
                rigid.linearVelocity += (transform.up * JumpInitialForce) + (MovingPlatformDeltaEdit / Time.fixedDeltaTime);

                FinalJumpTime = JumpDuration;
                FinalJumpForce = JumpForce;
                SubActionTime = 1;
            }

            // JUMP HOLD
            if (SubActionTime - 1 < FinalJumpTime)
            {
                if (Inp.JumpHold)
                {
                    Char.HandleGroundControl(Time.fixedDeltaTime, transform.up, FinalJumpForce, 0, false, 10, 999999);
                }
                else if (SubActionTime > 1.3f)
                {
                    SubActionTime = FinalJumpTime + 1f;
                }
            }

            // END JUMP
            if (SubActionTime > FinalJumpTime)
            {
                anim.ResetTrigger("Jump");
                SubAction = 0;
                SubActionTime = 0;
            }

            // SKIN ROT
            InAirSkinRot();

            SubActionTime += Time.fixedDeltaTime;
            // CHANGE ACTION
            if (!Char.Grounded) { CheckForWall(); }
            CheckForDash();
            CheckForAttackHere();
            CheckJump(true);
            Actions.Attacks.CheckForShot();
            fallspeedParam = localspeed.y;
        }
        else if (SubAction == 2) // WALL JUMP
        {
            SubActionTime += Time.fixedDeltaTime;
            if (SubActionTime > 0.25f)
            {
                anim.ResetTrigger("WallJump");
                SubActionTime = 0;
                SubAction = 0;
            }
            else if (Char.Grounded)
            {
                anim.ResetTrigger("WallJump");
                SubActionTime = 0;
                SubAction = 0;
            }

            //CHANGE ACTION
            CheckForDash();
            CheckForAttackHere();
            Actions.Attacks.CheckForShot();
        }
        else if (SubAction == 3) // IN WALL
        {
            // ENTER WALL STUFF
            WallTime += Time.fixedDeltaTime;
            if (WallTime < 1) { AudioWallStart.Play(); WallTime = 2; }
            anim.SetBool("Wall", true);

            // COYOTE
            WallCoyote = true;
            WallCoyoteTimeCounter = WallCoyoteTime;

            Movement();
            CheckForWall();
            CheckForDash();
            DashAvailable = true;
            SubActionTime += Time.fixedDeltaTime;
            if (SubActionTime > 0.2f) { SubAction = 0; SubActionTime = 0; }

            if (Char.Grounded) // END ACTION
            {
                anim.SetBool("Wall", false);
                anim.ResetTrigger("WallJump");
                SubActionTime = 0;
                SubAction = 0;
                WallTime = 0;
            }

            // WALL WALKING
            if (Inp.DashHold && WallOutTime > 0.0f)
            {
                SubAction = 5;
                SubActionTime = 0;
            }

            // WALL SKIN ROT
            prevInput = Vector3.ProjectOnPlane(wallnormal, -Char.GravityDir);
            finaltilt = -Char.GravityDir;
            SkinRotation(prevInput, finaltilt, AnimRotationSpeed * 10, 0);

            CheckForAttackHere();
        }
        else if (SubAction == 4) // DASH
        {
            if (SubActionTime < 0.05f)
            {
                landedParam = 0;
                anim.SetTrigger("Dash");
                SubActionTime = 0.06f;
                SkinRotMode = 1;
                if (DashIHB) { IHB.StartIHB(DashIHB); }

                // CANCEL VERTICAL VELICITY
                if (Char.Grounded) { rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, Char.GroundNormal); }
                else { rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, -Char.GravityDir); }
            }

            Movement();
            CheckJump(false);

            if (Char.SpeedMagnitude < HighSpeedThreshold)
            {
                if (SubActionTime < DashSpeedChangeDuration)
                {
                    rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, Char.GroundNormal);
                    if (rigid.linearVelocity.magnitude < DashMaxSpeed) // CLAMP DASH SO IT ONLY CHANGES DIRECTION AT LOW SPEEDS
                    {
                        if (Inp.InputMag > 0.05f) { prevInput = Inp.CamRelativeInput.normalized; }
                        else { }
                        Char.HandleGroundControl(Time.fixedDeltaTime, prevInput, 99, 99, false, DashSpeed, 9999);
                    }

                    DashSkinDir(0);
                }
                else
                {
                    DashSkinDir(1);
                }
            }

            void DashSkinDir(int type)
            {
                if (Inp.InputMag > 0.1f && type == 0) { prevInput = Inp.CamRelativeInput; }
                else { prevInput = Skin.transform.forward; }
                SkinRotation(prevInput, Char.GroundNormal, DashSkinRotSpeed, 0);
            }

            if (SubActionTime > DashDuration)
            {
                anim.ResetTrigger("Dash");
                SubActionTime = 0;
                SubAction = 0;
            }

            SubActionTime += Time.fixedDeltaTime;
            fallspeedParam = Mathf.Lerp(fallspeedParam, localspeed.y, Time.fixedDeltaTime * 40);

            // CHANGE ACTION
            CheckForWall();
            CheckForAttackHere();
            Actions.Attacks.CheckForShot();
        }
        else if (SubAction == 5) // WALL WALKING
        {
            // INTRO
            if (SubActionTime < 0.05f)
            {
                landedParam = 0;
                SubActionTime = 0.06f;
                SkinRotMode = 1;

                // SET DIRECTION
                if (Char.SpeedMagnitude < 3)
                { WallWalkDir = Vector3.ProjectOnPlane(prevInput, Char.GravityDir).normalized; }
                else { WallWalkDir = Vector3.ProjectOnPlane(Char.SpeedDirection, Char.GravityDir).normalized; }
                WallWalkDir = Vector3.ProjectOnPlane(WallWalkDir, wallnormal).normalized;

                // SET SPEED
                WallWalkCurrentSpeed = Vector3.ProjectOnPlane(PreWallSpeed, Char.GravityDir).magnitude;
                WallWalkCurrentSpeed = Mathf.Clamp(WallWalkCurrentSpeed, WallWalkMinSpeed, 9999f);
            }
            else
            {
                SubActionTime += Time.fixedDeltaTime;
                WallwalkTime += Time.fixedDeltaTime;
            }

            // WALL RAY
            if (Physics.Raycast(transform.position + (transform.up * Char.Height), wallrayDirection,
                out RayHit, WallRayDistance, Char.GroundRayMask))
            {
                wallrayDirection = -RayHit.normal;
                wallnormal = RayHit.normal;
                WallHit = true;
            }
            else
            {
                EndWallWalk();
            }

            // WALL WALKING
            if (WallwalkTime < WallWalkDuration)
            {       
                WallWalkDir = Vector3.ProjectOnPlane(Char.SpeedDirection, wallnormal).normalized;
                WallWalkDir = Vector3.ProjectOnPlane(Char.SpeedDirection, Char.GravityDir).normalized;
                rigid.linearVelocity = WallWalkDir * WallWalkCurrentSpeed;

                // FORCE TOWARDS WALL
                rigid.AddForce(-wallnormal * WallWalkAttachForce, ForceMode.Acceleration);
            }

            // WALL WALK SKIN ROT
            prevInput = Vector3.ProjectOnPlane(wallnormal, -Char.GravityDir);
            finaltilt = -Char.GravityDir;
            SkinRotation(prevInput, finaltilt, AnimRotationSpeed * 10, 0);

            // DIRECTION CORRECTION
            WallWalkDot = Vector3.Dot(Skin.transform.right, Char.SpeedDirection);
            anim.SetFloat("WallSide", WallWalkDot);

            // JUMP OUT
            if (Inp.Jump)
            {
                AudioWallJump.Play();      
                anim.SetBool("Wall", false);
                anim.SetInteger("SubAction", SubAction);
                anim.SetTrigger("WallJump");
                SubAction = 2;
                SubActionTime = 0;

                wallWalkJumpMultiplier = WallWalkJumpMultiplierOverSpeed.Evaluate(Char.SpeedMagnitude);

                rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, -Char.GravityDir);
                rigid.linearVelocity += wallnormal.normalized * (WallWalkJumpSidewaysForce * wallWalkJumpMultiplier);
                rigid.linearVelocity += transform.up * WallWalkJumpUpForce;
                prevInput = wallnormal.normalized;
                wallrayDirection = wallnormal.normalized;
                // SKINROT
                prevInput = rigid.linearVelocity.normalized;
                finaltilt = -Char.GravityDir;
                SkinRotation(prevInput, finaltilt, AnimRotationSpeed * 9999, 0);
                WallOutTime = -WallWalkInterval;
                WallwalkTime = 0;
            }

            if (Char.Grounded || !Inp.DashHold) // END ACTION
            {
                EndWallWalk();
            }

            // CAMERA
            Actions.Inp.CharCam.LookAtAngle
                            (Vector3.zero, 0.1f, WallWalkCameraSpeed, Quaternion.identity, 
                            null, -999, transform.up, Char.SpeedDirection);

            void EndWallWalk()
            {
                WallOutTime = -WallWalkInterval;
                anim.SetBool("Wall", false);
                anim.ResetTrigger("WallJump");
                anim.SetBool("Substate", false);
                SubActionTime = 0;
                SubAction = 0;
                WallTime = 0;
                anim.SetInteger("SubAction", 0);
            }

        }
        else if (SubAction == 6) // JESTER DASH
        {
            // INTRO
            if (SubActionTime < 0.05f)
            {
                if (JesterDashIHB) { IHB.StartIHB(JesterDashIHB); }
                landedParam = 0;
                anim.SetTrigger("Dash");
                SubActionTime = 0.06f;
                SkinRotMode = 1;
                MagnetDashInitialSpeed = Char.SpeedMagnitude;
                JdashInitialSpeed = Char.rigid.linearVelocity;

                // GRAB DIRECTION
                if(JesterDashTarget != null)
                {
                    JesterDashDir = (transform.position - JesterDashTarget.transform.position).normalized;
                }
            }

            // MOVE TO TARGET
            if(JesterDashTarget != null)
            {
                // SET DISTANCE
                JdashObjectDistance = Vector3.Distance(transform.position, JesterDashTarget.transform.position);

                // DASH
                rigid.linearVelocity = Vector3.zero;
                Char.rigid.Move(Char.rigid.position + (JesterDashDir * JesterDashSpeed * -Time.fixedDeltaTime), transform.rotation);

                // SKIN ROT
                prevInput = -JesterDashDir;
                SkinRotation(prevInput, Char.GroundNormal, DashSkinRotSpeed, 0);

                // MAGNET DASH
                if (Inp.Dash)
                {
                    if (MagnetDashInitialSpeed < MagnetDashPower)
                    {
                        rigid.linearVelocity = Inp.CamRelativeInput * MagnetDashPower;
                    }
                    else
                    {
                        rigid.linearVelocity = Inp.CamRelativeInput * (MagnetDashAccumulationForce * MagnetDashInitialSpeed);
                    }
                    rigid.linearVelocity += -Char.GravityDir * MagnetDashUpSpeed;

                    anim.SetTrigger("Dash");
                    if (DashIHB) { IHB.StartIHB(DashIHB); }
                    SubAction = 0;
                    SubActionTime = 0;
                    DoubleJumpAvailable = true;
                    AirDashAvailable = true;
                    WallwalkTime = 0;
                }

                // HIT TARGET
                if(JdashObjectDistance < JesterDashTarget.SecondaryRadius)
                {
                    if (JesterDashHitIHB) { IHB.StartIHB(JesterDashHitIHB); }
                    anim.SetTrigger("Dash");
                    Vector3 endspeed = Vector3.ProjectOnPlane(-JesterDashDir, Char.GravityDir).normalized * JesterDashHitSpeed;
                    Char.rigid.linearVelocity = endspeed + (-Char.GravityDir * JesterDashHitHeight);
                    SubAction = 0;
                    SubActionTime = 0;
                    DoubleJumpAvailable = true;
                    AirDashAvailable = true;
                    WallwalkTime = 0;
                }
            }

            // FINAL ACTION SETS
            Char.Grounded = false;
            SubActionTime += Time.deltaTime;
            Actions.Attacks.CheckForShot();

            // ABORT CONDITIONS
            if ((JesterDashTarget == null && !Inp.WeakTarget) || SubActionTime > 2.0f)
            {
                SubAction = 0;
                SubActionTime = 0;
            }

        }

        void CheckForWall()
        {
            // CHECK IF IN WALL
            if (!Char.Grounded && airTime > 0.032f)
            {
                bool Wallray()
                {
                    wallhit = false;

                    wallrayDirection = Inp.CamRelativeInput.normalized;
                    //Debug.DrawRay(transform.position + (transform.up * Char.Height), wallrayDirection * WallRayDistance, Color.purple);
                    if (Physics.Raycast(transform.position + (transform.up * Char.Height), wallrayDirection, out RayHit, WallRayDistance, Char.GroundRayMask))
                    { wallhit = true; return wallhit; }

                    if (Char.SideHit)
                    { wallrayDirection = (transform.position + (transform.up * Char.Height) - Char.sideHitInfo.point).normalized; }
                    //Debug.DrawRay(transform.position + (transform.up * Char.Height), wallrayDirection * WallRayDistance, Color.mediumPurple);
                    if (Physics.Raycast(transform.position + (transform.up * Char.Height), wallrayDirection, out RayHit, WallRayDistance, Char.GroundRayMask))
                    { wallhit = true; return wallhit; }

                    wallrayDirection = Vector3.ProjectOnPlane(Char.SpeedDirection, transform.up).normalized;
                    wallrayDirection = Quaternion.AngleAxis(90, transform.up) * wallrayDirection;
                    //Debug.DrawRay(transform.position + (transform.up * Char.Height), wallrayDirection * WallRayDistance, Color.rebeccaPurple);
                    if (Physics.Raycast(transform.position + (transform.up * Char.Height), wallrayDirection, out RayHit, WallRayDistance, Char.GroundRayMask))
                    { wallhit = true; return wallhit; }

                    wallrayDirection = Vector3.ProjectOnPlane(Char.SpeedDirection, transform.up).normalized;
                    wallrayDirection = Quaternion.AngleAxis(-90, transform.up) * wallrayDirection;
                    //Debug.DrawRay(transform.position + (transform.up * Char.Height), wallrayDirection * WallRayDistance, Color.rebeccaPurple);
                    if (Physics.Raycast(transform.position + (transform.up * Char.Height), wallrayDirection, out RayHit, WallRayDistance, Char.GroundRayMask))
                    { wallhit = true; return wallhit; }

                    return false;
                }

                if (!WallHit)
                {
                    PreWallSpeed = rigid.linearVelocity;
                    if (Wallray())
                    {
                        WallCoyote = true;
                        WallCoyoteTimeCounter = WallCoyoteTime;
                        wallnormal = RayHit.normal;
                        WallHit = true;
                        rigid.AddForce(-wallnormal * WallPushForce, ForceMode.Acceleration);
                        if (SubAction != 1)
                        {
                            SubAction = 3;
                            SubActionTime = 0;
                        }
                    }
                    else
                    {
                        WallHit = false;
                        anim.SetBool("Wall", false);
                    }
                }
                else
                {
                    if (!Char.Grounded && airTime > 0.15f)
                    {
                        if (Physics.Raycast(transform.position + (transform.up * Char.Height), -wallnormal, out RayHit, WallRayDistance, Char.GroundRayMask))
                        {
                            WallCoyote = true;
                            WallCoyoteTimeCounter = WallCoyoteTime;
                            wallnormal = RayHit.normal;
                            WallHit = true;
                            rigid.AddForce(-wallnormal * WallPushForce, ForceMode.Acceleration);

                            // WALL SKIN ROT
                            prevInput = Vector3.ProjectOnPlane(wallnormal, -Char.GravityDir);
                            finaltilt = -Char.GravityDir;
                            SkinRotation(prevInput, finaltilt, AnimRotationSpeed * 10, 0);
                        }
                        else
                        {
                            WallTime = 0;
                            WallHit = false;
                            anim.SetBool("Wall", false);
                        }
                    }
                }

                // IN WALL AND OTHER ACTIONS
                if ((WallHit || WallCoyote) && !Char.Grounded && airTime > (0.032f * 2f))
                {
                    if (Inp.Jump)
                    {
                        WallCoyote = false;
                        WallCoyoteTimeCounter = 0;
                        AudioWallJump.Play();
                        anim.SetTrigger("WallJump");
                        SubAction = 2;
                        SubActionTime = 0;
                        rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, -Char.GravityDir);
                        rigid.linearVelocity += wallnormal.normalized * WallJumpPower;
                        rigid.linearVelocity += transform.up * WallJumpUpPower;
                        prevInput = wallnormal.normalized;
                        wallrayDirection = wallnormal.normalized;
                        // SKINROT
                        prevInput = rigid.linearVelocity.normalized;
                        finaltilt = -Char.GravityDir;
                        SkinRotation(prevInput, finaltilt, AnimRotationSpeed * 9999, 0);
                    }
                }

                // COUNTERS
                WallOutTime += Time.fixedDeltaTime;

            }
            else
            {
                wallrayDirection = prevInput;
            }

            if(WallCoyoteTimeCounter > 0) { WallCoyoteTimeCounter -= Time.fixedDeltaTime; }
            else { WallCoyote = false; }
        }
        void InAirSkinRot()
        {
            finaltilt = -Char.GravityDir;
            if (Char.SpeedMagnitude < AirAnimSpeedThreshold)
            {
                if (Char.SpeedMagnitude > SkinRotationMinSpeed) { prevInput = Vector3.ProjectOnPlane(rigid.linearVelocity.normalized, transform.up); }
                else { prevInput = Vector3.ProjectOnPlane(prevInput, -Char.GravityDir); }
                SkinRotation(prevInput, finaltilt, AnimRotationSpeed, SkinRotMode);
            }
            else
            {
                prevInput = Vector3.ProjectOnPlane(Char.SpeedDirection, -Char.GravityDir);
                SkinRotation(prevInput, finaltilt, AnimRotationSpeed, SkinRotMode);
            }
        }

        anim.SetLayerWeight(LandingLayer, landedParam);
        anim.SetInteger("Action", 0);
        anim.SetInteger("SubAction", SubAction);
        anim.SetFloat("Speed", animSpeedParam);
        anim.SetFloat("PlaneSpeed", Vector3.ProjectOnPlane(Char.rigid.linearVelocity, -Char.GravityDir).magnitude);
        anim.SetBool("Grounded", Char.Grounded);
        anim.SetFloat("FallSpeed", fallspeedParam);
        anim.SetBool("Combat", InCombat);

        // OTHER ACTIONS
        if (SubAction != 6) { CheckForJesterDash(); }
        if (Actions.Attacks != null) { Actions.Attacks.CheckForBlockInput(); }
        if (Actions.Rail != null) { Actions.Rail.LookForRails(); }
        if (Inp.Player)
        {
            if(AttackCounter <= 0.001f)
            {
                AttackCounter += Time.fixedDeltaTime;
            }
        }

        // FINAL SETS
        Char.MovingPlatformDeltaStored = Vector3.zero;
        if (Char.Grounded) 
        {
            anim.SetBool("DoubleJump", false);
            anim.SetFloat("GroundFloat", 1.0f);
            DoubleJumpAvailable = true;
            WallwalkTime = 0;
            WallOutTime = 0;
        }
        else 
        { 
            anim.SetFloat("GroundFloat", 0f);
        }
     }

    void CheckForAttackHere()
    {
        if (Actions.Attacks != null)
        {
            // CHECK FOR ATTACK
            if (AllowAttacking && AttackCounter > 0.0f)
            {
                Actions.Attacks.CheckForAttack();
                if (AttackBuffered) // FORCE ATTACK WITH BUFFER
                {
                    AttackBuffered = false;
                    if (Char.Grounded) { Actions.Attacks.StartAttack(AttackBufferType); }
                    else { Actions.Attacks.StartAttack(AttackBufferType + 2); }
                }
            }
            else if (AllowAttacking)
            {
                if ((Inp.LightAttack || Inp.HeavyAttack) && !Char.Sliding)
                {
                    AttackBuffered = true;
                    if (Inp.LightAttack) { AttackBufferType = 0; }
                    if (Inp.HeavyAttack) { AttackBufferType = 1; }
                }
            }


        }
    }

    public void Movement()
    {
        //MOVEMENT & DRAG
        if (Char.Grounded && !Char.Sliding)
        {
            anim.ResetTrigger("WallJump");
            OverSpeed();

            // LOW SPEED
            if (Char.SpeedMagnitude < HighSpeedThreshold)
            {
                HS_SkinTilt = false;
                Char.HandleGroundControl
                    (Time.fixedDeltaTime, Inp.CamRelativeInput, MoveAccell, TangentialDrag, false, TopSpeed, MaxSpeed);
            }
            else if (Char.SpeedMagnitude >= HighSpeedThreshold)
            {
                // HIGH SPEED
                // SET RELATIVE INPUT
                HS_Dot = Mathf.LerpUnclamped(1, 0, Vector3.Dot(Inp.CamRelativeInput, Char.SpeedDirection));
                if (HS_Dot < HS_FacingThreshold)
                {
                    planespd = Vector3.ProjectOnPlane(Char.SpeedDirection, transform.up);
                    planeinp = Vector3.ProjectOnPlane(Inp.CamRelativeInput, transform.up);

                    if (Mathf.Abs(Inp.LeftAnalogInput.x) > HS_TiltDeadZone)
                    {
                        HS_SkinTilt = true;
                        HS_Input = Inp.CamRelativeInput;
                        HS_Input = Vector3.Slerp(planespd, planeinp, HS_TurnignRadius);
                    }
                    else
                    {
                        HS_SkinTilt = false;
                        HS_Input = planespd; // test it with new camera
                    }
                }
                else
                {
                    HS_SkinTilt = false;
                    HS_Input = Inp.CamRelativeInput;
                }

                // MOVE CHARACTER
                Char.HandleGroundControl
                    (Time.fixedDeltaTime, HS_Input, MoveAccell, TangentialDrag, false, TopSpeed, MaxSpeed);

                Actions.Inp.CharCam.LookAtAngle
                        (Vector3.zero, 0.1f, HS_CameraForce, Quaternion.identity, null, HS_CameraHeight, transform.up, Char.SpeedDirection);
            }

            // SLOPE PHYSICS
            if (Char.SlopeMode == 1 && Char.Grounded)
            {
                if (Char.SpeedAngle < 0.0f)
                {
                    rigid.AddForce(Char.SpeedDirection * (Char.SpeedAngle * -SlopeDownAcceleration)
                        , ForceMode.Acceleration);
                }
                else
                {
                    rigid.AddForce(Char.SpeedDirection * (Char.SpeedAngle * -SlopeUpDeceleration)
                        , ForceMode.Acceleration);
                }
            }

            // DRAG
            if (Inp.InputMag > 0.1f)
            {
                rigid.linearDamping = 0;
            }
            else
            {
                if (Inp.InputEnabled == true)
                {
                    rigid.linearDamping = GroundDrag;
                    rigid.linearDamping *= GroundedDragOverSpeed.Evaluate(Char.SpeedMagnitude);
                }
            }
        }
        else if (Char.Grounded && Char.Sliding)
        {
            rigid.linearDamping = 0;
        }
        else // AIR MOVEMENT
        {
            localspeed = transform.InverseTransformDirection(rigid.linearVelocity);
            localspeed.x = Mathf.Lerp(localspeed.x, 0, Time.fixedDeltaTime * AirDrag);
            localspeed.z = Mathf.Lerp(localspeed.z, 0, Time.fixedDeltaTime * AirDrag);
            rigid.linearVelocity = transform.TransformDirection(localspeed);

            // NOTE: AirControlMultiplier is used here as this exact thing is set to 1.5 in SparkTEJ 3
            Char.HandleGroundControl(Time.fixedDeltaTime * AirControlMultiplier,
                Inp.CamRelativeInput, AirMoveAccell, AirTangentialDrag, false, AirTopSpeed, MaxSpeed);
            rigid.linearDamping = 0;
        }
    }

    void OverSpeed()
    {
        if (Char.SpeedMagnitude >= OverSpeedMin && Char.SpeedMagnitude < OverspeedMax)
        {
            overspeed = Char.SpeedDirection * OverspeedPower;
            Char.rigid.AddForce(overspeed, ForceMode.Acceleration);
        }
    }

    public void SkinRotation(Vector3 direction, Vector3 up, float speed, int mode)
    {
        if (mode == 0) // STANDARD
        {
            skinrot = Quaternion.identity;
            skinrot = Quaternion.LookRotation(direction, up) * skinrot;
            Skin.rotation = Quaternion.Slerp(Skin.rotation, skinrot, Time.fixedDeltaTime * speed);
        }
        else if(mode == 1) // FORCE TO SPEED DIRECTION
        {
            skinrot = Quaternion.identity;
            direction = Vector3.ProjectOnPlane(rigid.linearVelocity.normalized, -Char.GravityDir);
            skinrot = Quaternion.LookRotation(direction, up) * skinrot;
            Skin.rotation = Quaternion.Slerp(Skin.rotation, skinrot, Time.fixedDeltaTime * speed);
        }
    }

    public void CheckJump(bool DoubleOnly)
    {
        if (DoubleOnly == false)
        {
            if (Inp.Jump && (CoyoteActive || Char.Grounded))
            {
                if (Char.Sliding) { AirDashAvailable = false; }
                AudioJump.Play();
                Char.Grounded = false;
                CoyoteActive = false;
                SubAction = 1;
                SubActionTime = 0;
                Char.CheckGroundTime = -GroundCheckInterval;
            }
            else if (Inp.Jump && DoubleJumpAvailable && (!Char.Grounded))
            {
                DoubleJump();
            }
        }
        else if (Inp.Jump && DoubleOnly && DoubleJumpAvailable)
        {
            DoubleJump();
        }

        void DoubleJump()
        {
            DoubleJumpAvailable = false;
            Char.Grounded = false;
            CoyoteActive = false;
            SubAction = 1;
            SubActionTime = -1;
        }
    }

    public void CheckForDash()
    {
        if (Inp.Dash && !Char.Sliding && DashAvailable)
        {
            if (Char.Grounded)
            {
                SubAction = 4;
                SubActionTime = 0;
                anim.ResetTrigger("WallJump");
                anim.SetBool("Skid", false);
                AudioDash.Play();
            }
            else if (AirDashAvailable)
            {
                SubAction = 4;
                SubActionTime = 0;
                AirDashAvailable = false;
                anim.ResetTrigger("WallJump");
                anim.SetBool("Skid", false);
                AudioDash.Play();
            }
        }

        if (Char.Grounded && !Char.Sliding) { AirDashAvailable = true; }
    }

    public void CheckForJesterDash()
    {
        if (Inp.WeakTarget)
        {
            if (Inp.CurrentTarget) 
            { 
                JesterDashTarget = Inp.CurrentTarget;
                if (Inp.JDash) // START JESTER DASH
                {
                    SubAction = 6;
                    SubActionTime = 0;
                }
            }
            else 
            {
                if (Inp.JDash) { IHB.StartIHB(JesterDashNone); }
                JesterDashTarget = null; 
            }
        }
        else
        {
            if (Inp.JDash) { IHB.StartIHB(JesterDashNone); }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        //(Physics.Raycast(t.position + localoffset, dir, out hit, rayDistance, Mask))
        //if (Physics.Raycast(transform.position, transform.position - col.contacts[0].point, out RayHit, 2, Char.GroundRayMask))
        //{
        //    ColliderTouchingSomthing = true;
        //}

        //Debug.DrawRay(transform.position, transform.position - col.contacts[0].point, Color.gray);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.deepPink;

    }

    private void OnDisable()
    {
        anim.SetBool("DoubleJump", false);
    }

}
