using UnityEngine;

public class MissleController : MonoBehaviour
{
    [Header("Parameters")]
    public Rigidbody rigid;
    public EntityInfo TargetFaction;
    public float TimeToStart;
    public float Speed = 0.1f;
    public float Drag = 1;
    public float TrackSpeed = 10f;
    public TargetData Target;

    [Header("Misc Parameter")]
    public bool UseStartSpeed = false;
    public float StartSpeed = 0;

    // CACHE
    float t;
    private void Start()
    {
    }

    private void FixedUpdate()
    {
        t += Time.fixedDeltaTime;

        if(t < TimeToStart) // BEFORE MOVEMENT
        {
            if (UseStartSpeed == false) { rigid.linearVelocity = (transform.forward * Speed); }
            else { rigid.linearVelocity = (transform.forward * StartSpeed); }
        }
        else if (Target != null)
        {
            rigid.linearVelocity = Vector3.Lerp(rigid.linearVelocity, Vector3.zero, Drag * Time.fixedDeltaTime);
            rigid.linearVelocity += (transform.forward * Speed);

            SetDirection();
            Rotation(chardir, World.WorldInfo.GravityDir, TrackSpeed);
        }

        if (Target == null)
        {
            for (int i = 0; i < TargetData.Targets.Count; i++)
            {
                if (TargetData.Targets[i].TargetFaction.FactionName == TargetFaction.FactionName) { Target = TargetData.Targets[i]; }
            }
        }
    }

    Quaternion charrot;
    public void Rotation(Vector3 direction, Vector3 up, float speed)
    {
        charrot = Quaternion.identity;
        charrot = Quaternion.LookRotation(direction, up) * charrot;
        transform.rotation = Quaternion.Slerp(transform.rotation, charrot, Time.fixedDeltaTime * speed);
    }

    Vector3 chardir;
    public void SetDirection()
    {
        if (Target) { chardir = (transform.position - Target.transform.position).normalized; }
    }

}
