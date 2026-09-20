using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class RootMotionTransfer : MonoBehaviour
{
    [Header("Main")]
    public CharacterPhysics Char;
    public CharacterActions Actions;
    public Transform Parent;
    public Animator Anim;
    public Vector2 Multiplier = Vector2.one;
    public Vector3 InitialLocalSkinPos;
    public float RotationOffset = 90;

    [Header("Wall Clip Fix")]
    public bool UseClipFix = true;
    public LayerMask ClipMask;
    public float ClipRayMultiplier = 1;
    public float ClipPushMultiplier = 1;
    public float ClipBottonCastHeight = 0.1f;

    [Header("Cache")]
    public bool EnableRootMotion = true;
    public bool RootMotionForcedOn = false;
    public bool AllowVertical = false;
    public Vector3 DeltaPosition;
    public Vector3 RawDeltaPosition;
    public Quaternion DeltaRotation;
    public IEnumerator[] RepeaterCoroutines;
    public Vector3 RawAccumulatedMotion;
    public Vector3 AccumulatedMotion;
    Vector3 rootdir;
    RaycastHit hit;
    Vector3 move;
    public int AccumulatedFrames = 0;

    private void Start()
    {
        InitialLocalSkinPos = transform.localPosition;
    }

    private void FixedUpdate()
    {
        FixedRootMotion();
    }

    private void OnAnimatorMove()
    {
        // ACCUMULATE DELTA MOVEMENT OVER FRAMES
        AccumulatedFrames = 0;
        if (EnableRootMotion) // GET IT > ROTATE IT > ADD IT
        {
            rootdir = Anim.deltaPosition;
            rootdir = Quaternion.AngleAxis(RotationOffset, transform.up) * rootdir;
            AccumulatedMotion += rootdir;
            RawAccumulatedMotion += Anim.deltaPosition;
            AccumulatedFrames++;
        }
        else 
        { 
            AccumulatedMotion = Vector3.zero;
        }

        // ROTATE
        RawAccumulatedMotion = AccumulatedMotion;
    }

    void FixedRootMotion()
    {
        if (RootMotionForcedOn) { EnableRootMotion = true; }

        if (EnableRootMotion)
        {
            if (Anim.deltaPosition.sqrMagnitude > 0)
            {
                if (Char.Grounded)
                {
                    if (AllowVertical) 
                    {
                        move = AccumulatedMotion;
                    }
                    else
                    {
                        move = Vector3.ProjectOnPlane(AccumulatedMotion, Char.GroundNormal);
                    }
                }
                else
                {
                    move = AccumulatedMotion;
                }

                // RESET ACCUMULATED MOTION AND MOVE (PREVENTS WALL CLIPPING FROM ROOT MOTION)
                if (ClipFixRay(Char.transform.position, move, move.magnitude * ClipRayMultiplier * Multiplier.x) == false)
                { 
                    Char.transform.position += move * Multiplier.x;
                }
                AccumulatedMotion = Vector3.zero;
                RawAccumulatedMotion = Vector3.zero;

            }
        }

        RawDeltaPosition = Anim.deltaPosition;
        DeltaPosition = Anim.deltaPosition;
        DeltaPosition = Quaternion.AngleAxis(RotationOffset, transform.up) * DeltaPosition;
        DeltaRotation = Anim.deltaRotation;

        Debug.DrawRay(transform.position + (transform.up * 0.1f), move * 10, Color.cyan);
        Debug.DrawRay(transform.position + (transform.up * 0.1f), AccumulatedMotion * 10, Color.cyan);
        //DebugText.AddDebugString("Accumulated Root Frames: " + AccumulatedFrames);
    }

    private void LateUpdate()
    {

    }

    public bool ClipFixRay(Vector3 origin, Vector3 delta, float distance)
    {
        if (Char)
        {
            distance = Mathf.Clamp(distance, Char.SideRayDistance, 999);
            if (Physics.Raycast(origin, delta, out hit, distance, ClipMask)) { return true; }
            else if (Physics.Raycast(origin + (transform.up * ClipBottonCastHeight), delta, out hit, distance, ClipMask)) { return true; }
            else if (Physics.Raycast(origin + (transform.up * -ClipBottonCastHeight), delta, out hit, distance, ClipMask)) { return true; }

            Debug.DrawRay(origin, delta * 10, Color.yellow);
            Debug.DrawRay(origin + (transform.up * ClipBottonCastHeight), delta * 10, Color.yellow);
            Debug.DrawRay(origin + (transform.up * -ClipBottonCastHeight), delta * 10, Color.yellow);
        }

        return false;
    }

    public void TriggerIHB(int index)
    {
        if (Actions.Attacks)
        {
            IHB.StartIHB(Actions.Attacks.AttackHitboxes[index]);

        }
    }
}
