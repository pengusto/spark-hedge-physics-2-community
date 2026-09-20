using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPhysics : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rigid;

    [Header("Parameter")]
    public float GravityMultiplier = 5;
    public float RotationSpeed = 10;

    [Header("Collision")]
    public float Height = 0.6f;
    public float GroundRayDistance = 0.195f;
    public float StandingDistance = 0.05f;
    public float GroundRaySideOffset = 0.05f;
    public float GroundAuxiliaryRaysDistance = 0.05f;
    public float SideRayDistance = 0.1f;
    public float SideRayPenetrationOffset = 1.0001f;
    public float LegHeight = 0.5f;
    public float StandSpeed = 10f;
    public float SideRayHeight = 0.2f;
    public float CliffThreshold = 0.6f;
    public LayerMask GroundRayMask;
    public float Distance = 1;

    [Header("Slope Physics")]
    public float SlopeRotationSpeed = 10;
    public float SlopeMinSpeed = 3f;
    public float SlopeExitSpeed = 1;
    public float SlopeMinAngle = 0.95f;
    public float SlopeDetachDistance = 0.1f;
    public float SlopeMinDetachAngle = 0.8f;

    [Header("Moving Platforms (origin)")]
    public float ExitMultiplier = 1;
    public Transform PlatformReference;
    public bool OnMovingPlatform = false;
    public Vector3 MovingPlatformDelta;
    public Vector3 OnParentDelta;
    public Vector3 onParentDelta;
    int MovingPlatformFrames = 0;
    public Vector3 MovingPlatformDeltaStored;

    [Header("CACHE")]
    public float SpeedMagnitude;
    public Vector3 SpeedDirection;
    public bool Grounded = false;
    public bool WallCollision = false;
    public bool Sliding = false;
    public Vector3 offsetter;
    public Vector3 sideoffesetter;
    public Vector3 GravityDir = Vector3.down;
    public Vector3 FinalGravity;
    public Vector3 GroundNormal;
    public Vector3 GroundPosition;
    public float GroundDistance;
    public RaycastHit hit = new RaycastHit();
    public RaycastHit sideHitInfo = new RaycastHit();
    public float GroundAngle;
    public float GravityAngle;
    public float NormalAngle;
    public float SpeedAngle;
    float climb;
    Vector3 rdist;
    public bool EnableMovement = true;
    public float b_normalSpeed;
    public Vector3 b_normalVelocity;
    public Vector3 b_tangentVelocity;
    public Vector3 b_fallVelocity;
    public Vector3 G_Forces;
    public Vector3 LocalSpeed;
    Quaternion charrot;
    public List<RaycastHit> RayHits = new List<RaycastHit>();
    public float LargestRayDistance = 0;
    public bool MainHit;
    public bool SideHit;
    public float CheckGroundTime = 0;
    public float AirTime = 0;
    public float GroundTime = 0;
    public SurfaceMaterialInfo Surface;
    TrackObjectDelta ObjectDelta;
    float sideRayDot;
    public float RayMode = 0;
    public float FinalRayDistance = 0;
    Vector3 siderayoffset;
    Vector3 heightoffset;
    public float SlopeMode = 1;
    Vector3 sideray_diagonal;
    public bool ForceSlopeDetection = false;
    public float ForceSlopeDetectionTime = 0;
    public bool ForceSlopeDetach = false;

    private void Start()
    {
        PlatformReference.parent = null;
    }

    private void FixedUpdate()
    {
        // INITIAL SETS
        MainHit = false;
        SideHit = false;
        Grounded = false;
        Sliding = false;
        LocalSpeed = rigid.transform.InverseTransformDirection(rigid.linearVelocity);
        if (OnMovingPlatform)
        {
            MovingPlatformDelta = (PlatformReference.position - MovingPlatformDelta);
            MovingPlatformDeltaStored = MovingPlatformDelta;
        }

        Debug.DrawRay(transform.position, MovingPlatformDelta / Time.fixedDeltaTime, Color.yellow);
        Debug.DrawRay(transform.position, MovingPlatformDeltaStored / Time.fixedDeltaTime, Color.yellow * 0.5f);

        // COLLISION DOWN
        RayHits.Clear();
        LargestRayDistance = 99999;
        if (CheckGroundTime > 0.0f)
        {
            FinalRayDistance = GroundRayDistance;
            // MAIN CAST
            heightoffset = transform.up * Height;
            AdvancedRaycast(transform, -transform.up, FinalRayDistance, GroundRayMask, heightoffset, 0.95f, LegHeight, true);
            // SIDE CASTS
            if (RayMode == 0)
            {
                offsetter = transform.TransformDirection(transform.right * (GroundRaySideOffset)) + heightoffset;
                AdvancedRaycast(transform, -transform.up, FinalRayDistance + GroundAuxiliaryRaysDistance, GroundRayMask, offsetter, 0.95f, LegHeight, false);
                offsetter = transform.TransformDirection(-transform.right * (GroundRaySideOffset)) + heightoffset;
                AdvancedRaycast(transform, -transform.up, FinalRayDistance + GroundAuxiliaryRaysDistance, GroundRayMask, offsetter, 0.95f, LegHeight, false);
                offsetter = transform.TransformDirection(transform.forward * (GroundRaySideOffset)) + heightoffset;
                AdvancedRaycast(transform, -transform.up, FinalRayDistance + GroundAuxiliaryRaysDistance, GroundRayMask, offsetter, 0.95f, LegHeight, false);
                offsetter = transform.TransformDirection(-transform.forward * (GroundRaySideOffset)) + heightoffset;
                AdvancedRaycast(transform, -transform.up, FinalRayDistance + GroundAuxiliaryRaysDistance, GroundRayMask, offsetter, 0.95f, LegHeight, false);
            }
        }
        else
        {
            FinalRayDistance = GroundRayDistance;
            CheckGroundTime += Time.fixedDeltaTime;
        }

        // COMPUTE DOWN COLLISION
        if (Grounded && RayHits.Count > 0)
        {
            // SET NORMAL
            GroundNormal = Vector3.zero;
            for (int i = 0; i < RayHits.Count; i++)
            {
                GroundNormal += RayHits[i].normal;
                Debug.DrawRay(transform.position, RayHits[i].normal * 10, Color.white);
            }
            GroundNormal = GroundNormal / RayHits.Count;

            GroundAngle = Vector3.Dot(transform.up, GroundNormal);
            GravityAngle = Vector3.Dot(transform.up, -GravityDir);
            NormalAngle = Vector3.Dot(-GravityDir, GroundNormal);
            SpeedAngle = Vector3.Dot(SpeedDirection, -GravityDir);
            if (GroundAngle < CliffThreshold) { Sliding = true; }
            climb = LegHeight;

            // SET POS
            if (MainHit)
            {
                GroundPosition = RayHits[0].point;
                GroundDistance = RayHits[0].distance;
                if (GroundDistance < climb /*&& GroundDistance < 0.05f*/) // RISE UP
                {
                    rigid.position += (-transform.up * -StandSpeed) * Time.fixedDeltaTime;
                    rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, GroundNormal);
                }
                else // SET
                {
                    rigid.position = GroundPosition + (transform.up * (StandingDistance - Height));
                    //rigid.position = (GroundPosition) - ((-transform.up * (StandingDistance - Height)) * GroundRayDistance);
                    rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, GroundNormal);
                   
                }

                // MOVING PLATFORM HANDLER
                ManageMovingPlatCollision(RayHits[0]);
            }
        }
        else
        {

        }

        // GRAVITY (if not grounded, or if is)
        if (!Grounded)
        {
            ApplyGravity();
            GroundTime = 0;
            AirTime += Time.fixedDeltaTime;
        }
        if (Grounded && Sliding)
        {
            AirTime = 0;
            GroundTime += Time.fixedDeltaTime;
            ApplyGravity();
        }
        else
        {
            GroundTime += Time.fixedDeltaTime;
            AirTime = 0;
        }

        void ApplyGravity()
        {
            FinalGravity = (GravityDir * World.WorldInfo.WorldGravity) * GravityMultiplier;
            rigid.linearVelocity += FinalGravity * Time.fixedDeltaTime;
        }

        // COLLISION SIDES
        if (GroundTime > 0.05f || Grounded == false)
        {
            // CARDINALS
            sideoffesetter = transform.up * Height;
            SideRays(transform, -transform.right,   SideRayDistance, GroundRayMask, sideoffesetter, SideRayPenetrationOffset, 1f);
            SideRays(transform, transform.right,    SideRayDistance, GroundRayMask, sideoffesetter, SideRayPenetrationOffset, 1f);
            SideRays(transform, -transform.forward, SideRayDistance, GroundRayMask, sideoffesetter, SideRayPenetrationOffset, 1f);
            SideRays(transform, transform.forward,  SideRayDistance, GroundRayMask, sideoffesetter, SideRayPenetrationOffset, 1f);

            // DIAGONALS
            sideray_diagonal = Vector3.Slerp(transform.forward, transform.right, 0.5f);
            SideRays(transform, sideray_diagonal, SideRayDistance, GroundRayMask, sideoffesetter, SideRayPenetrationOffset, 1f);
            sideray_diagonal = Vector3.Slerp(transform.forward, -transform.right, 0.5f);
            SideRays(transform, sideray_diagonal, SideRayDistance, GroundRayMask, sideoffesetter, SideRayPenetrationOffset, 1f);
            sideray_diagonal = Vector3.Slerp(-transform.forward, transform.right, 0.5f);
            SideRays(transform, sideray_diagonal, SideRayDistance, GroundRayMask, sideoffesetter, SideRayPenetrationOffset, 1f);
            sideray_diagonal = Vector3.Slerp(-transform.forward, -transform.right, 0.5f);
            SideRays(transform, sideray_diagonal, SideRayDistance, GroundRayMask, sideoffesetter, SideRayPenetrationOffset, 1f);
        }

        // SLOPE PHYSICS
        SlopeCheck();
        if (SlopeMode == 0)
        {
            RotateToGravity();
        }
        else if(SlopeMode == 1)
        {
            // ROTATE TO SLOPE
            if (Grounded)
            {
                charrot = transform.rotation;
                charrot = Quaternion.FromToRotation(transform.up, GroundNormal) * charrot;
                //transform.rotation = Quaternion.Slerp(transform.rotation, charrot, Time.fixedDeltaTime * SlopeRotationSpeed);
                rigid.Move(rigid.position, Quaternion.Slerp(transform.rotation, charrot, Time.fixedDeltaTime * SlopeRotationSpeed));
            }
            else
            {
                RotateToGravity();
            }
            
            // EXIT SLOPE MODE
            if (SpeedMagnitude < SlopeExitSpeed && !ForceSlopeDetection) 
            { SlopeMode = 0;}
            if(Grounded == false) { SlopeMode = 0; }
        }

        void SlopeCheck()
        {
            if (ForceSlopeDetection) 
            {
                SlopeMode = 1;
                ForceSlopeDetectionTime -= Time.fixedDeltaTime;
                if(ForceSlopeDetectionTime < 0.0f) 
                {
                    ForceSlopeDetection = false;
                    ForceSlopeDetach = false; 
                }

                if (ForceSlopeDetach && SlopeDetach())
                {
                    ForceSlopeDetection = false;
                    ForceSlopeDetach = false;
                    ForceSlopeDetectionTime = 0;
                }
            }
            else
            {
                if (SlopeMode == 0)
                {
                    if (SpeedMagnitude > SlopeMinSpeed && SlopeMinAngle > NormalAngle)
                    {
                        SlopeMode = 1;
                    }
                }
                else
                {
                    SlopeDetach();
                }
            }

            bool SlopeDetach()
            {
                if (ForceSlopeDetection == false)
                {
                    // DETACH BASED ON ANGLE AND SPEED
                    if (SpeedMagnitude < SlopeExitSpeed)
                    {
                        SlopeMode = 0;
                        if (NormalAngle < SlopeMinDetachAngle && Grounded)
                        {
                            PopOut();
                        }
                        return true;
                    }
                }
                else
                {
                    // ONLY DETACH BASED ON ANGLE
                    if (NormalAngle < SlopeMinDetachAngle && Grounded)
                    {
                        PopOut();
                    }
                }
                return false;
            }

            void PopOut()
            {
                Grounded = false;
                CheckGroundTime = -0.1f;
                rigid.position += transform.up * SlopeDetachDistance;
            }
        }

        void RotateToGravity()
        {
            // ROTATE TO GRAIVTY
            charrot = transform.rotation;
            charrot = Quaternion.FromToRotation(transform.up, -GravityDir) * charrot;
            //transform.rotation = Quaternion.Slerp(transform.rotation, charrot, Time.fixedDeltaTime * RotationSpeed);
            rigid.Move(rigid.position, Quaternion.Slerp(transform.rotation, charrot, Time.fixedDeltaTime * RotationSpeed));
        }

        // MOVING PLATFORM MANAGEMENT
        if (OnMovingPlatform)
        {
            MovingPlatformFrames += 1;
            if (MovingPlatformFrames > 2)
            {
                rigid.position += MovingPlatformDelta;
            }
            if (!Grounded)
            {
                ExitMovingPlatform();
            }
        }
        else
        {
            MovingPlatformFrames = 0;
        }

        // PARENT MOVING PLATFORM STUFF
        if (transform.parent != null)
        {
            if (Grounded) 
            { 
                onParentDelta = (transform.position - OnParentDelta) / Time.deltaTime;      
            }
            Debug.DrawRay(transform.position, onParentDelta, Color.green);
            OnParentDelta = transform.position;
        }

        // EXPORT
        SpeedMagnitude = rigid.linearVelocity.magnitude;
        SpeedDirection = rigid.linearVelocity.normalized;
        if (OnMovingPlatform) { MovingPlatformDelta = PlatformReference.position; }
    }

    void AdvancedRaycast(Transform t, Vector3 dir, float rayDistance, LayerMask Mask, Vector3 localoffset, float penetrationoffset, float climbableHeight, bool main)
    {
        if (Physics.Raycast(t.position + localoffset, dir, out hit, rayDistance, Mask))
        {
            if (main) // MAIN RAY HIT
            {
                MainHit = true;
                Grounded = true;
            }
            LargestRayDistance = hit.distance;
            RayHits.Add(hit);
        }
        else
        {
            if (main) { MainHit = false; }
            Sliding = false;
            GroundNormal = transform.up;
            GroundAngle = 0;
            PlatformReference.parent = null;
        }

        // DEBUG
        if (main) { Debug.DrawRay(t.position + localoffset, dir * rayDistance, Color.blue); }
        else {      Debug.DrawRay(t.position + localoffset, dir * rayDistance, Color.red); }
    }

    void ManageMovingPlatCollision(RaycastHit h)
    {
        // MOVING PLATFORM ANCHOR
        if (h.transform.TryGetComponent<SurfaceMaterialInfo>(out Surface) && Grounded)
        {
            if (Surface.MovingPlatform && Grounded)
            {
                if (PlatformReference.parent == null) { PlatformReference.position = h.point; }
                PlatformReference.parent = h.transform;
                PlatformReference.position = h.point;
                OnMovingPlatform = true;
            }
            else
            {
                PlatformReference.parent = null;
            }
        }
        else
        {
            PlatformReference.parent = null;
        }
    }

    public void ExitMovingPlatform()
    {
        if (OnMovingPlatform)
        {
            Debug.Log("PLAT_1: " + MovingPlatformDelta);
            rigid.linearVelocity += (MovingPlatformDelta) / Time.fixedDeltaTime;
            OnMovingPlatform = false;
        }
    }

    public void SideRays(Transform t, Vector3 dir, float rayDistance, LayerMask Mask, Vector3 localoffset, float penetrationoffset, float climbableHeight)
    {
        if (Physics.Raycast(t.position + localoffset, dir, out hit, rayDistance, Mask))
        {
            SideHit = true;
            WallCollision = true;
            if (hit.distance > 0.01f)
            {
                //Debug.DrawRay(hit.point, rigid.linearVelocity * 10, Color.cyan);
                //Debug.DrawRay(hit.point, Vector3.ProjectOnPlane(rigid.linearVelocity, hit.normal).normalized * 3, Color.magenta);

                // SET POSITION
                rigid.position = (hit.point - localoffset) - (((dir * penetrationoffset) * rayDistance));

                // VELOCITY
                sideRayDot = Vector3.Dot(rigid.linearVelocity, hit.normal);
                if (sideRayDot < 0.0f)
                {
                    rigid.linearVelocity = Vector3.ProjectOnPlane(rigid.linearVelocity, hit.normal);
                }

                // SETS
                sideHitInfo = hit;
            }
        }
        else
        {
            WallCollision = false;
        }


        Debug.DrawRay(t.position + localoffset, dir * rayDistance, Color.red);
    }

    public void HandleGroundControl(float deltaTime, Vector3 input, float Speed, float tangDrag, bool BreakMaxSpeed, float TopSpeed, float MaxSpeed)
    {
        // By Damizean, edited by LakeFeperd
        // We assume input is already in the Player's local frame...
        // If there is some input...

        if (input.sqrMagnitude != 0.0f)
        {
            // Normalize to get input direction
            var inputDirection = transform.InverseTransformDirection(input.normalized);
            var inputMagnitude = input.magnitude;

            // Fetch velocity in the Player's local frame, decompose into lateral and vertical
            // motion, and decompose lateral motion further into normal and tangential components.

            var velocity = rigid.linearVelocity;
            var localVelocity = rigid.transform.InverseTransformDirection(velocity);

            Vector3 lateralVelocity;
            Vector3 verticalVelocity;

            lateralVelocity = new Vector3(localVelocity.x, 0.0f, localVelocity.z);
            verticalVelocity = new Vector3(0.0f, localVelocity.y, 0.0f);

            var normalSpeed = Vector3.Dot(lateralVelocity, inputDirection);
            var normalVelocity = inputDirection * normalSpeed;
            var tangentVelocity = lateralVelocity - normalVelocity;

            // Note: normalSpeed is the magnitude of normalVelocity, with the added
            // bonus that it's signed. If positive, the speed goes towards the same
            // direction than the input :)

            if (normalSpeed < TopSpeed && !BreakMaxSpeed)
            {
                // Accelerate towards the input direction.
                normalSpeed += (Speed * deltaTime) * inputMagnitude;
                normalSpeed = Mathf.Min(normalSpeed, TopSpeed);

                // Rebuild back the normal velocity with the correct modulus.
                if (normalSpeed >= 0f)
                {
                    normalVelocity = inputDirection * normalSpeed;
                }
                else
                {
                    normalVelocity = inputDirection * normalSpeed;
                }

            }
            if (BreakMaxSpeed)
            {
                // Accelerate towards the input direction.
                normalSpeed = deltaTime * inputMagnitude;

                // Rebuild back the normal velocity with the correct modulus.
                if (normalSpeed >= 0f)
                {
                    normalVelocity = inputDirection * normalSpeed;
                }
                else
                {
                    // (Reverse the inpit of inputdirection (on x and z, here)
                    normalVelocity = inputDirection * normalSpeed;
                }
            }

            // Additionally, we can apply some drag on the tangent directions for
            // tighter control.

            float curvePosTang = (rigid.linearVelocity.sqrMagnitude / MaxSpeed) / MaxSpeed;
            tangentVelocity = Vector3.MoveTowards(tangentVelocity, Vector3.zero, (tangDrag) * deltaTime * inputMagnitude);

            // Compose local velocity back and compute velocity back into the Global frame.
            // You probably want to delay doing this to the end of the physics processing,
            // as transformations can incur into numerical damping of the velocities.
            // The last step is included only for the sake of completeness.

            if (EnableMovement)
            {
                localVelocity = normalVelocity + tangentVelocity + verticalVelocity;
                velocity = rigid.transform.TransformDirection(localVelocity);
                rigid.linearVelocity = velocity;
            }

            //Export nescessary variables
            b_normalSpeed = normalSpeed;
            b_normalVelocity = normalVelocity;
            b_tangentVelocity = tangentVelocity;
            b_fallVelocity = verticalVelocity;

        }
        else
        {
            b_normalSpeed = 0;
            //b_normalVelocity = Vector3.zero;
            b_tangentVelocity = Vector3.zero;
            //b_fallVelocity = Vector3.zero;

        }

    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(rigid.centerOfMass, 0.05f);
    }

    private void OnDrawGizmosSelected()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        // ENTER PARENT REGION
        if (col.CompareTag("Platform"))
        {
            if (col.TryGetComponent<SurfaceMaterialInfo>(out Surface))
            {
                if (Surface.ParentRegion)
                {
                    transform.parent = Surface.ParentRegionRoot;
                    OnParentDelta = transform.position;
                }

                if(col.TryGetComponent<TrackObjectDelta>(out ObjectDelta))
                {
                    //rigid.velocity -= ObjectDelta.Delta;
                    Debug.Log("ADDED SPEED: " + ObjectDelta.Delta);
                }
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        // IF ON PARENT REGION, EXIT
        if (col.CompareTag("Platform"))
        {
            if (col.TryGetComponent<SurfaceMaterialInfo>(out Surface))
            {
                transform.parent = null;
                transform.localScale = Vector3.one;
                rigid.linearVelocity += onParentDelta * ExitMultiplier;
            }
        }


    }
}
