using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class CharacterRailActions : MonoBehaviour
{
    [Header("References - Action 2")]
    public CharacterPhysics Char;
    public CharacterInput Inp;
    public CharacterActions Actions;
    public Animator anim;
    public int SubAction = 0;
    public float SubActionTime = 0;

    [Header("Parameters - Basic")]
    public float RailScanRadius = 20;
    public LayerMask RailLayerMask;
    public float RailCameraSpeedThreshold = 10;
    public float RailCameraForce = 300;
    public float RailCameraHeight = 0.25f;

    [Header("Parameters - Physics")]
    public float DownhillForce = 1;
    public float UphillForce = 1;
    public float DownhillForceCrouch = 1;
    public float UphillForceCrouch = 1;
    public float RailMaxSpeed = 100;

    [Header("Parameters - Crouch")]
    public bool IsCrouching = false;
    public ParticleSystem CrouchSpeedupParticle;
    public GameObject CrouchStartParticle;
    public float CrouchWindParticleFreqency = 1;

    [Header("Parameters - Regen Break")]
    public bool HasRegenBreak = true;
    public float RegenBreakPower = 10;
    public float RegenParticleAmm = 10;
    public AnimationCurve RegenBreakCurve;
    public ParticleSystem RegenBreakParticle;
    public AudioSource RegenBreakAudio;
    public float RegenBreakAudioMaxVolume;
    public AnimationCurve RegenBreakPitchCurve;

    [Header("Parameters - RailBoost")]
    public bool HasBoost = true;
    public float BoostStartEnergyUsage = 3f;
    public float BoostEnergyUsage = 10f;
    public ParticleSystem BoostParticle;
    public AudioSource BoostAudio;
    public float BoostHoldThreshold = 0.5f;
    public float BoostSpeedAdd = 10;
    public SimpleCameraFxParams BoostFx;

    [Header("Parameters - Attach")]
    public float RailAttachMinDistance = 1;
    public float RailAttachDotLimit = 0;
    public float RailLandingInterval = 0.5f;

    [Header("Parameters - Dash 02")]
    public float DashSpeed = 5;
    public float DashEnd = 0.5f;

    [Header("Parameters - RailSwitch - 03")]
    public float SwitchActionDuration = 0.25f;
    public float LookForNextRailDistance = 3;
    public float RailSwitchMaxDistance = 4;
    public GameObject JesterDashEffect;
    public float SwitchSpeed = 60;
    public float SwitchInterval = 0.2f;

    [Header("Parameters - RailDamage - 04")]
    public float DamageSpeedMultiplier = 0.9f;

    [Header("Parameters - Animation")]
    public float SideForcesMultiplier = -200;
    public float SideForcesSmoothness = 20;
    public float GravityForcesMultiplier = -200;
    public float GravityForcesSmoothness = 5;

    [Header("Parameters - Audio & FX")]
    public AudioSource RailSound;
    public ParticleSystem RailParticle;
    public float RailParticleAmmount = 10;
    public AnimationCurve RailPitch;
    public AnimationCurve RailVolume;
    public float RailSoundSmoothness = 20;
    public AudioSource RailCrouchDrag;
    public float RailCrouchDragVolume = 0.8f;

    [Header("Cache")]
    public SplineContainer CurrentSpline;
    public SplinePath Path;
    public List<SplineContainer> NearbySplinesContainers = new List<SplineContainer>();
    public float RailSpeed = 1;
    public float SplineProgress;
    public Collider[] NearbyRails = new Collider[30];
    public float OutOfRailCounter;
    SplineContainer spl = new SplineContainer();
    public float RailDot;
    public Vector3 RailPosition;
    Vector3 nearpoint;
    Vector3 railup;
    public Vector3 railforward;
    public Vector3 railforwardThatIsntZero = Vector3.one;
    Vector3 previousRight;
    Vector3 actualRight;
    Vector3 actualForward;
    public float SideG;
    public float finalSideG;
    public float GravityG;
    public float finalGravityG;
    public float UpDot;
    public float Hillspeed = 0;
    public float RailDir;
    float3 right;
    float CrouchTime;
    float crouchanim;
    float OverallSpeed;
    float crouchParticleCounter;
    float railParticleCounter;
    public float Railtime;
    Vector3 JumpDirection;
    Vector3 NextRailPoint;
    public Vector3 PreviousRailPosition;
    public float SwitchTime;
    public float SwitchDelayTime;
    List<SplineContainer> AllSplinesExceptThisOne;
    public float SwitchDot;
    public float RegenValue;
    float RegenParticleCounter;
    float BoostCounter;
    float RailVolumeEvaluate;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if(SubAction == 0) // MAIN
        {
            // INITIAL RAIL EVENTS
            if (SubActionTime < 0.1f)
            {
                if(RailSound != null) { RailSound.Play(); }
                SubActionTime = 0.11f;
            }

            // CROUCH
            if (Inp.LightAttackHold)
            {
                // INITIAL CROUCH STUFF
                if(CrouchTime < 0.1) 
                {
                    if (CrouchStartParticle) { IHB.StartIHB(CrouchStartParticle); }
                    CrouchTime = 0.2f;
                }

                // CROUCH FX
                if (CrouchSpeedupParticle && (UpDot * RailDir) < 0)
                {
                    // WIND FX
                    crouchParticleCounter += Mathf.Abs(RailSpeed) * CrouchWindParticleFreqency * Time.fixedDeltaTime;
                    if (crouchParticleCounter > 1) { CrouchSpeedupParticle.Emit(1); crouchParticleCounter = 0; }
                }

                // ANIM
                crouchanim = Mathf.Lerp(crouchanim, 1, Time.fixedDeltaTime * 20);
                anim.SetFloat("RailCrouch", crouchanim);
                CrouchTime += Time.fixedDeltaTime;
                IsCrouching = true;
            }
            else
            {
                // ANIM (non crouch)
                crouchanim = Mathf.Lerp(crouchanim, 0, Time.fixedDeltaTime * 20);
                anim.SetFloat("RailCrouch", crouchanim);
                CrouchTime = 0;
                IsCrouching = false; 
            }

            // MOVEMENT
            RailMovement();
            RailCamera();
            if (HasRegenBreak) { RegenBreakManagement(); }
            if (HasBoost) { BoostManagement(); }

            // MOVEMENT FX
            railParticleCounter += Mathf.Abs(RailSpeed) * RailParticleAmmount * Time.deltaTime; ;
            if(railParticleCounter > 1) { RailParticle.Emit(Mathf.RoundToInt(railParticleCounter)); railParticleCounter = 0; }

            // AUDIO
            if(RailSound.isPlaying == false || RailSound.enabled == false)
            {
                RailSound.Play();
                RailCrouchDrag.volume = 0;
                RailSound.volume = 0;
                RailSound.enabled = true;
            }
            else
            {
                RailVolumeEvaluate = RailVolume.Evaluate(OverallSpeed);
                RailSound.volume = Mathf.Lerp(RailSound.volume, RailVolumeEvaluate, Time.fixedDeltaTime * RailSoundSmoothness);
                RailSound.pitch = Mathf.Lerp(RailSound.pitch, RailPitch.Evaluate(OverallSpeed), Time.fixedDeltaTime * RailSoundSmoothness);
            }
            if (RailCrouchDrag.isPlaying == false) { RailCrouchDrag.Play(); }
            if (IsCrouching)
            {
                if (Mathf.Abs(UpDot) > 0.1f)
                {
                    RailCrouchDrag.volume = Mathf.Lerp(RailCrouchDrag.volume, RailCrouchDragVolume * (Mathf.Sign(UpDot) * RailDir), Time.fixedDeltaTime * 5);
                }
                else
                {
                    RailCrouchDrag.volume = Mathf.Lerp(RailCrouchDrag.volume, 0, Time.fixedDeltaTime * 5);
                }
            }
            else
            {
                RailCrouchDrag.volume = Mathf.Lerp(RailCrouchDrag.volume, 0, Time.fixedDeltaTime * 5);
            }


            // EXIT
            if (Inp.Jump) { SubAction = 1; Actions.Basic.CheckJump(false); }

            // DASH
            if (Inp.Dash)
            {
                SubAction = 2;
                SubActionTime = 0;
            }

            // FINAL SETS
            finalSideG = Mathf.Lerp(finalSideG, SideG, Time.fixedDeltaTime * SideForcesSmoothness);
            finalGravityG = Mathf.Lerp(finalGravityG, GravityG, Time.fixedDeltaTime * GravityForcesSmoothness);
            anim.SetFloat("SideGForce", (finalSideG * SideForcesMultiplier) + (finalGravityG * GravityForcesMultiplier));
            anim.SetFloat("Speed", Mathf.Abs(RailSpeed));
            SubActionTime += Time.deltaTime;
        }
        else if(SubAction == 1) // JUMP
        {
            Char.rigid.isKinematic = false;
            Char.enabled = true;
            Actions.SwitchAction(0);
            Char.rigid.linearVelocity = railup * Actions.Basic.JumpInitialForce;
            Char.rigid.linearVelocity += railforward.normalized * Mathf.Abs(RailSpeed);
            Actions.Basic.AudioJump.Play();
            Actions.Basic.SubAction = 1;
            Actions.Basic.SubActionTime = 0;
            Actions.Basic.DoubleJumpAvailable = true;
            Actions.Basic.AirDashAvailable = true;
            Actions.Basic.DashAvailable = true;
        }
        else if(SubAction == 2) // DASH
        {
            // INITIAL DASH EVENTS
            if (SubActionTime < 0.1f)
            {
                IHB.StartIHB(Actions.Basic.DashIHB);
                anim.SetTrigger("RailDash");
                SubActionTime = 0.11f;

                if (Mathf.Abs(RailSpeed) < DashSpeed)
                {
                    RailSpeed = (DashSpeed * RailDir);
                }
            }

            // MOVEMENT
            RailMovement();
            RailCamera();

            // END
            if(SubAction >= DashEnd)
            {
                SubAction = 0;
                SubActionTime = 0;
            }
        }
        else if(SubAction == 3) // RAIL SWITCH
        {
            if (SubActionTime < 0.1f)
            {
                // ANIM
                if(SwitchDot < 0.5f) { anim.SetInteger("RailSwitch", -1); }
                else { anim.SetInteger("RailSwitch", 1); }
                SubActionTime = 0.11f;
            }

            RailMovement();

            Char.rigid.Move(Vector3.Lerp(PreviousRailPosition, RailPosition, SwitchTime), Char.rigid.rotation);
            SwitchTime += Time.fixedDeltaTime * SwitchSpeed;
            SubActionTime += Time.fixedDeltaTime;

            if (SubActionTime > SwitchActionDuration)
            {
                SubAction = 0;
                SubActionTime = 0;
            }
        }
        else if(SubAction == 4) // RAIL DAMAGE
        {
            // RAIL DAMAGE INTRO
            if (SubActionTime < 0.1f)
            {
                anim.SetTrigger("RailDamage");
                SubActionTime = 0.11f;
            }

            SubActionTime += Time.fixedDeltaTime;
            RailMovement();

            // RETURN TO ACTION 0
            if (SubActionTime > 0.35f) 
            {
                anim.ResetTrigger("RailDamage");
                SubAction = 0;
                SubActionTime = 0;
            }
        }

        // DETACH IF DEAD
        if(Actions.Interactions.Hp <= 0)
        {
            Char.rigid.isKinematic = false;
            Char.enabled = true;
            Actions.SwitchAction(0);
        }

        // FINAL SETS
        if (IsCrouching == false) { RailCrouchDrag.volume = Mathf.Lerp(RailCrouchDrag.volume, 0, Time.fixedDeltaTime * 5); }
        else { if (Inp.LightAttackHold == false) { IsCrouching = false; } }
    }

    void RailMovement()
    {
        // DISABLE MAIN PHYSICS
        Char.rigid.isKinematic = true;
        Char.rigid.linearVelocity = Vector3.zero;
        Char.enabled = false;

        // MOVE ACROSS RAIL
        if (Path != null)
        {
            // SNAP TO RAIL
            previousRight = FloatToVec(right);
            SplineProgress += RailSpeed * (Time.fixedDeltaTime / Path.GetLength());
            Path.Evaluate(SplineProgress, out var pos, out right, out var up);
            railup = up;
            railforward = right * Mathf.Sign(RailSpeed);

            // ANGLE & MISC (animation)
            OverallSpeed = Mathf.Abs(RailSpeed / RailMaxSpeed);
            RailDir = Mathf.Sign(RailSpeed);
            actualForward = FloatToVec(right).normalized;
            actualRight = Quaternion.AngleAxis(90, anim.transform.up) * actualForward;         
            SideG = Vector3.Dot(previousRight.normalized, actualRight);
            GravityG = Vector3.Dot(-Char.GravityDir, actualRight);
            UpDot = Vector3.Dot(-Char.GravityDir, actualForward);

            // PHYSICS
            Hillspeed = (Time.fixedDeltaTime * UpDot) * RailDir;
            if (IsCrouching == false)
            {
                if(UpDot < 0) // DOWNHILL
                {
                    RailSpeed += -Hillspeed * (DownhillForce * RailDir);
                }
                else // UPHILL
                {
                    RailSpeed += -Hillspeed * (UphillForce * RailDir);
                }
            }
            else // CROUCHING
            {
                if (UpDot < 0) // DOWNHILL
                {
                    RailSpeed += -Hillspeed * (DownhillForceCrouch * RailDir);
                }
                else // UPHILL
                {
                    RailSpeed += -Hillspeed * (UphillForceCrouch * RailDir);
                }
            }

            // LIMITS
            RailSpeed = Mathf.Clamp(RailSpeed, -RailMaxSpeed, RailMaxSpeed);

            // RAIL END
            if(railforward != Vector3.zero) { railforwardThatIsntZero = railforward; }
            if (Railtime > 0.1f && CurrentSpline.Spline.Closed == false)
            {
                if (SplineProgress <= 0.0f || SplineProgress >= 1.0f)
                {
                    Char.rigid.isKinematic = false;
                    Char.enabled = true;
                    Actions.SwitchAction(0);
                    Actions.Basic.SubAction = 0;
                    Actions.Basic.SubActionTime = 0;
                    Char.rigid.linearVelocity += railforwardThatIsntZero.normalized;
                    Actions.Basic.DoubleJumpAvailable = true;
                    Actions.Basic.AirDashAvailable = true;
                    Actions.Basic.DashAvailable = true;
                    anim.SetTrigger("Jump");
                    Actions.Basic.prevInput = railforwardThatIsntZero.normalized;
                    return;
                }
            }
            else if (CurrentSpline.Spline.Closed == true)
            {
                if (SplineProgress <= 0.0f)
                {
                    SplineProgress = 1;
                }
                else if(SplineProgress >= 1.0f)
                {
                    SplineProgress = 0;
                }
            }

            // POS AND ROTATION
            if (SubAction != 3)
            {
                RailPosition = pos;
                Char.rigid.Move(RailPosition, quaternion.identity);
                anim.transform.rotation = Quaternion.LookRotation(right * Mathf.Sign(RailSpeed), up);
                Railtime += Time.fixedDeltaTime;
            }
            else
            {
                RailPosition = pos;
                anim.transform.rotation = Quaternion.LookRotation(right * Mathf.Sign(RailSpeed), up);
                Railtime += Time.fixedDeltaTime;
            }

            // RAIL SWITCH
            LookForRailToSwitch();
        }
    }

    void RailCamera()
    {
        if (Mathf.Abs(RailSpeed) > RailCameraSpeedThreshold)
        {
            Actions.Inp.CharCam.LookAtAngle
                (Vector3.zero, 0.1f, RailCameraForce, Quaternion.identity, null, RailCameraHeight, -Char.GravityDir, actualForward * RailDir);
        }
    }

    public void LookForRails()
    {
        // COUNTER
        if(OutOfRailCounter < RailLandingInterval)
        {
            OutOfRailCounter += Time.fixedDeltaTime;
            return;
        }

        // FIND ALL NEARBY RAILS AND ADD TO LIST
        FindNearbySplines();

        // CHECK IF CLOSE TO RAIL (using unity's example code here from "ShowNearestPoint.cs")
        var nearest = new float4(0, 0, 0, float.PositiveInfinity);
        foreach (var container in NearbySplinesContainers)
        {
            using var native = new NativeSpline(container.Spline, container.transform.localToWorldMatrix);
            float d = SplineUtility.GetNearestPoint(native, transform.position, out float3 p, out float t);
            if (d < nearest.w)
            {
                nearest = new float4(p, d);
            }

            native.Evaluate(t, out var pos, out var right, out var up);
            nearpoint = FloatFourToVec(nearest);

            Debug.DrawRay(nearpoint, transform.up, Color.deepPink);
            Debug.DrawRay(pos, transform.up, Color.cadetBlue);

            // RAIL ATTACH
            if(Char.Grounded == false)
            {
                RailDot = Vector3.Dot(Char.SpeedDirection, (transform.position - nearpoint).normalized);
                Debug.DrawRay(nearpoint, transform.up * RailDot, Color.blue);
                if (Vector3.Distance(transform.position, nearpoint) < RailAttachMinDistance && RailDot < RailAttachDotLimit)
                {
                    // SET SPLINE
                    CurrentSpline = container;
                    SplineProgress = t;

                    // SET PARAMETERS
                    Vector3 railvec = Vector3.ProjectOnPlane(Char.rigid.linearVelocity, up);
                    RailSpeed = railvec.magnitude * Vector3.Dot(railvec.normalized, FloatToVec(right).normalized);

                    // GO TO RAIL ACTION
                    SubActionTime = 0;
                    Actions.SwitchAction(2);
                    anim.SetTrigger("RailStart");
                    Path = new SplinePath(new[]
                    {
                        new SplineSlice<Spline>(CurrentSpline.Splines[0], new SplineRange(0, 9999),
                            CurrentSpline.transform.localToWorldMatrix),
                    });
                    return;
                }
            }
        }

    }

    void FindNearbySplines()
    {
        NearbySplinesContainers.Clear();
        NearbyRails = Physics.OverlapSphere(transform.position, RailScanRadius, RailLayerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < NearbyRails.Length; i++)
        {
            if (NearbyRails[i] != null)
            {
                if (NearbyRails[i].TryGetComponent<SplineContainer>(out spl))
                {
                    NearbySplinesContainers.Add(spl);
                }
            }
            else
            {
                continue;
            }
        }
    }

    public bool LookForRailToSwitch()
    {
        // CANCELL IF...
        if(Inp.InputMag < 0.7f) { return false; }
        if(SwitchDelayTime < SwitchInterval) { SwitchDelayTime += Time.fixedDeltaTime; return false; }

        // SET DIRECTION AND POINT
        JumpDirection = Quaternion.AngleAxis(90, Vector3.ProjectOnPlane(actualRight, -Char.GravityDir)) *
            Vector3.ProjectOnPlane(Inp.CamRelativeInput, -Char.GravityDir);
        NextRailPoint = transform.position + (JumpDirection * LookForNextRailDistance);

        // CHECK CLOSEST SPLINE (using unity's example code here from "ShowNearestPoint.cs")
        AllSplinesExceptThisOne = NearbySplinesContainers;
        AllSplinesExceptThisOne.Remove(CurrentSpline);
        SplineContainer nextspline = new SplineContainer();
        float3 right = new float3();
        float3 pos = new float3();
        float splineTime = 0;
        float finalSplineTime = 0;
        var nearest = new float4(0, 0, 0, float.PositiveInfinity);
        foreach (var container in AllSplinesExceptThisOne)
        {
            using var native = new NativeSpline(container.Spline, container.transform.localToWorldMatrix);
            float d = SplineUtility.GetNearestPoint(native, NextRailPoint, out float3 p, out splineTime);
            if (d < nearest.w)
            {
                nearest = new float4(p, d);
                nearpoint = FloatFourToVec(nearest);
                nextspline = container;

                // GET POINT IN NEXT RAIL
                native.Evaluate(splineTime, out pos, out right, out var up);
                finalSplineTime = splineTime;
                Debug.DrawRay(nearpoint, transform.up, Color.yellowGreen);
            }

            // DEBUG
            Debug.DrawRay(NextRailPoint, transform.up * splineTime, Color.blue);

            // CANCEL IF TOO FAR
            if (nearest.w > RailSwitchMaxDistance) 
            {
                Debug.DrawRay(transform.position, transform.up, Color.yellowGreen);
                return false; 
            }
        }
        
        // RAIL SWITCH
        if (Inp.JDash && AllSplinesExceptThisOne.Count > 0)
        {
            // SET SPLINE
            PreviousRailPosition = transform.position;
            RailPosition = pos;
            CurrentSpline = nextspline;
            SplineProgress = finalSplineTime;
            actualRight = Quaternion.AngleAxis(90, anim.transform.up) * FloatToVec(right);
            SwitchDot = Vector3.Dot((PreviousRailPosition - RailPosition).normalized, actualRight.normalized) * RailDir;

            // RESET LIST
            FindNearbySplines();

            // FX
            if (JesterDashEffect) { IHB.StartIHB(JesterDashEffect); }

            // GO TO RAIL ACTION
            SubAction = 3;
            SubActionTime = 0;
            SwitchTime = 0;

            Path = new SplinePath(new[]
            {
                    new SplineSlice<Spline>(CurrentSpline.Splines[0], new SplineRange(0, 9999),
                          CurrentSpline.transform.localToWorldMatrix),
                    });
            return true;
        }

        //DEBUG
        Debug.DrawRay(NextRailPoint, transform.up, Color.red);
        Debug.DrawRay(transform.position, JumpDirection * LookForNextRailDistance);
        return false;
    }

    Vector3 FloatToVec(float3 f)
    {
        return new Vector3(f.x, f.y, f.z);
    }

    Vector3 FloatFourToVec(float4 f)
    {
        return new Vector3(f.x, f.y, f.z);
    }

    void RegenBreakManagement()
    {
        // BREAK
        if (Inp.HeavyAttackHold && Mathf.Abs(RailSpeed) > 0.1f)
        {
            // BREAKING
            RailSpeed -= (RegenBreakPower * Time.fixedDeltaTime) * RailDir;
            RegenValue = RegenBreakCurve.Evaluate(Mathf.Abs(RailSpeed));
            if (RegenValue > 0.01f)
            {
                RegenParticleCounter += Time.fixedDeltaTime * Mathf.Abs(RailSpeed) * RegenParticleAmm;
                if (RegenParticleCounter > 1) { RegenParticleCounter = 0; RegenBreakParticle.Emit(1); }
            }

            if (RegenValue > 0.1f)
            {
                // ADD ENERGY
                if (Actions.Interactions) { Actions.Interactions.En += RegenValue * Time.fixedDeltaTime; }

                // REGEN AUDIO
                RegenBreakAudio.enabled = true;
                if (RegenBreakAudio.volume < RegenBreakAudioMaxVolume) { RegenBreakAudio.volume += Time.fixedDeltaTime * 4; }
                else { RegenBreakAudio.volume -= Time.fixedDeltaTime * 4; }
                RegenBreakAudio.pitch = RegenBreakPitchCurve.Evaluate(OverallSpeed);
            }
            else
            {
                RegenBreakAudio.volume -= Time.fixedDeltaTime * 4;
            }

            // ANIM
            anim.SetBool("RegenBreak", true);
        }
        else
        {
            if (RegenBreakAudio.enabled)
            {
                RegenBreakAudio.volume -= Time.fixedDeltaTime * 4;
                if (RegenBreakAudio.volume <= 0.01f) { RegenBreakAudio.enabled = false; }
            }

            // ANIM
            anim.SetBool("RegenBreak", false);
        }
    }

    void BoostManagement()
    {
        if (Inp.DashHold && Inp.HeavyAttackHold == false && Actions.Interactions.En > 0.1f)
        {
            if(BoostCounter > BoostHoldThreshold)
            {
                if(BoostCounter < 20)
                {
                    // INITIAL BOOST EFFECTS
                    Actions.Interactions.En -= BoostStartEnergyUsage;
                    BoostAudio.Play();
                    BoostCounter += 21f;
                    BoostParticle.Emit(3);

                    // SHAKE AND SLOW
                    CharacterCamera.ShakeCamera(BoostFx.ShakeDuration, BoostFx.ShakeAmplitude, transform.position);
                    CharacterCamera.SlowDown(BoostFx.SlowDownDuration, BoostFx.SlowDownTime, BoostFx.SlowDownRestoreSpeed, transform.position);
                }
                else
                {
                    Actions.Interactions.En -= BoostEnergyUsage * Time.fixedDeltaTime;
                    BoostParticle.Emit(1);
                    RailSpeed += BoostSpeedAdd * RailDir * Time.fixedDeltaTime;
                }
            }

            BoostCounter += Time.fixedDeltaTime;
        }
        else
        {
            BoostCounter = 0;
        }
    }

    public void RailDamage()
    {
        if (Actions.Interactions.Invencible == false)
        {
            SubAction = 4;
            SubActionTime = 0;
            RailSpeed *= DamageSpeedMultiplier;
        }

        if(Actions.Interactions.Hp < 0)
        {
            OutOfRailCounter = -1;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(nearpoint, 0.1f);
    //}
}
