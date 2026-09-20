using UnityEngine;

public class BossCamera : MonoBehaviour
{
    [Header("References")]
    public CharacterActions Player;
    public CharacterInput Target;
    public CharacterCamera Cam;
    public TriggerEffect Trigger;
    public Transform Center;

    [Header("Parameters")]
    public float NewCameraDistance = 1;
    public float NewCameraDistanceSpeed = 5;
    public AnimationCurve NewCameraDistanceCurve;
    public float FacingAmmount = 0.4f;
    public float DotMultiplier = 1;
    public float RotationOffset = 10;
    public float UnlockDuration = 1.5f;

    [Header("Cache")]
    public Vector3 Pos = Vector3.zero;
    public Vector3 TargetPos = Vector3.zero;
    public Vector3 EnemyDir = Vector3.zero;
    public Vector3 FinalAngle = Vector3.zero;
    public float EnemyDistance;
    public float TargetValue = 0;
    public float OriginalCamDist;
    public float FinalCameraDist;
    public float t = -1;

    private void Start()
    {
        OriginalCamDist = Cam.Distance;
    }

    void LateUpdate()
    {
        // CHECK TO ACTIVATE
        if(Player.Inp.CurrentTarget == null) 
        { 
            return;
        }
        else
        {
            if (Player.Inp.LockedOn)
            {
                if(Player.Inp.CurrentTarget.AttachedCharacter != null) 
                { Target = Player.Inp.CurrentTarget.AttachedCharacter.Inp; }
                else { return; }
            }
            else
            {
                return;
            }
        }

        // LOCKED ON
        if (t < 0)
        {
            // CAMERA DISTANCE
            EnemyDistance = Vector3.Distance(Center.position, Target.transform.position);
            FinalCameraDist = Mathf.Lerp(OriginalCamDist, NewCameraDistance, NewCameraDistanceCurve.Evaluate(EnemyDistance));
            Cam.Distance = Mathf.Lerp(Cam.Distance, FinalCameraDist, Time.deltaTime * NewCameraDistanceSpeed);

            // DOT
            Pos = Center.position;
            Pos = Vector3.ProjectOnPlane(Pos, Player.transform.up);
            Pos = Vector3.ProjectOnPlane(Pos, -Player.transform.up);
            TargetPos = Target.transform.position;
            TargetPos = Vector3.ProjectOnPlane(TargetPos, Target.transform.up);
            TargetPos = Vector3.ProjectOnPlane(TargetPos, -Target.transform.up);

            // ROTATION
            transform.position = Center.position;
            Vector3 dir = -(Center.position - Target.transform.position).normalized;
            dir = Vector3.ProjectOnPlane(dir, Player.transform.up);
            transform.rotation = Quaternion.LookRotation(dir, Player.transform.up);
            //transform.LookAt(newpos, Player.transform.up);

            EnemyDir = (Pos - TargetPos).normalized;
            TargetValue = Vector3.Dot(Cam.transform.forward, EnemyDir) * DotMultiplier;

            FinalAngle = Quaternion.AngleAxis(RotationOffset, Player.transform.up) * Trigger.transform.localEulerAngles;
            transform.rotation *= Quaternion.Euler(0, RotationOffset, 0);

            // ROTATE
            if (TargetValue < FacingAmmount)
            {
                Player.Inp.CharCam.LookAtAngle
                                   (FinalAngle, Trigger.Look_Time,
                                   Trigger.Look_Speed, transform.rotation,
                                   Trigger.transform, Trigger.Look_HeightOffset, Vector3.zero, Vector3.zero);
            }
        }
        else
        {
            t -= Time.deltaTime;
            Cam.Distance = Mathf.Lerp(Cam.Distance, OriginalCamDist, Time.deltaTime * NewCameraDistanceSpeed);
        }

        // UNLOCK CAMERA
        //if (Player.Inp.Inp.GetAxis("RightAnalogX") > 0.1f || Player.Inp.Inp.GetAxis("RightAnalogY") > 0.1f
        //    || (Player.Char.Grounded == false)
        //    || (Target.Char.Grounded == false && Target.Actions.Action == 1 && Target.Actions.Attacks.SubAction == 2))
        //{
        //    t = UnlockDuration;
        //}
    }
}
