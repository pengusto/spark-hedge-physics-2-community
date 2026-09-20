using System.Collections;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("References")]
    public EntityInfo faction;
    public EntityInfo TargetFaction;
    public Rigidbody rigid;
    public Animator anim;
    public GameObject[] Hitboxes;

    [Header("Hurt Stuff")]
    public GameObject DieExplosion;
    public GameObject DieShell;
    public SkinnedMeshRenderer[] Meshes;
    public float FlyAwayForce = 3f;
    public float FlyUpForce = 3f;
    public float RotateAwaySpeed = 10f;

    //[Header("Line Of Sight")]
    //public Transform 

    [Header("Parameters")]
    public float HP = 1;
    public float FakeUpdateRate = 0.1f;
    public float Drag = 10;
    public int AttackAmm = 1;
    public float SkinDamageTime = 0.2f;
    public float EnergyOnDeath = 5;

    [Header("Actions")]
    public bool AttackIfClose = false;
    public float AttackTime = 1;
    public float AttackInterval = 0;
    public float AttackTargetDistance = 3;
    public float AttackLookAtTime = 0.4f;
    public float AttackRotationSpeed = 10;
    public bool Chase = false;
    public float ChaseSpeed = 1;
    public float HomeMaxDistance = 10;
    public float TooCloseToPlayerThreshold = 0.2f;
    public Collectable.Points points; 


    [Header("Cache")]
    public int Action = 0;
    public TargetData CurrentTarget;
    public Vector3 GravityDir = Vector3.down;
    public float ClosestTargetDistance = 999999;
    float tgtDist;
    float randf;
    int rand = 0;
    System.Random r;
    int Seed;
    int AttackType;
    float t;
    bool dead = false;
    Vector3 HomePosition;
    float HomeDist;

    private void Start()
    {
        // INITIALIZE
        InvokeRepeating("FakeUpdate", Random.Range(0.1f, 1f), FakeUpdateRate);
        chardir = transform.forward;
        HomePosition = transform.position;

        // INITIALIZE SEED
        Seed = Mathf.RoundToInt(((transform.position.x + transform.position.y) * 100) % 10000000);
        Debug.Log("Seed: " + Seed);
        r = new System.Random(Seed);
    }

    public void FixedUpdate()
    {
        if (Action == 0) // IDLE
        {
            if (AttackIfClose)
            {
                RotateToGravity(GravityDir, 10);
                if (AttackTargetDistance > ClosestTargetDistance)
                {
                    SetDirection();
                    Rotation(-chardir, -GravityDir, 10);
                    t += Time.fixedDeltaTime;
                }
                if (t > AttackTime)
                {
                    Action = 1;
                    t = 0;
                }
            }
            else if (Chase)
            {
                HomeDist = Vector3.Distance(transform.position, HomePosition);
                RotateToGravity(GravityDir, 10);
                if (AttackTargetDistance > ClosestTargetDistance) // IF PLAYER CLOSE ADD TO COUNTER
                {
                    SetDirection();
                    Rotation(-chardir, -GravityDir, AttackRotationSpeed);
                    t += Time.fixedDeltaTime;
                }
                else // IF NOT RETURN HOME
                {
                    if (HomeDist > 0.5f)
                    {
                        chardir = (transform.position - HomePosition).normalized;
                        Rotation(-chardir, -GravityDir, AttackRotationSpeed);
                        rigid.AddForce(transform.forward * ChaseSpeed, ForceMode.Acceleration);
                    }
                }

                if (t > AttackTime /*&& HomeDist < 0.5f*/)
                {
                    Action = 2;
                    t = 0;
                }
            }
        }
        else if (Action == 1) // ATTACK
        {
            // INITIAL, GENERATE RANDOM VALUE OF ATTACK AND START ATTACK
            if (t < 1)
            {
                AttackType = r.Next(0, 2048);
                randf = ((float)r.Next(0, 4096) / 4096f) * AttackAmm;
                AttackType = Mathf.FloorToInt(randf);
                AttackType = Mathf.Clamp(AttackType, 0, AttackAmm);
                anim.SetTrigger("at" + AttackType);
                t = 1;
            }
            else
            {
                t += Time.fixedDeltaTime;

                if (t - 1 < AttackLookAtTime)
                {
                    SetDirection();
                    Rotation(-chardir, -GravityDir, AttackRotationSpeed);
                }

                if (t > 2 && anim.GetBool("Idle"))
                {
                    Action = 0;
                    t = 0 - AttackInterval;
                }
            }


        }
        else if(Action == 2)
        {
            if (t < 1)
            {
                anim.SetTrigger("at0");
                t = 1;
            }
            else
            {
                t += Time.fixedDeltaTime;
                HomeDist = Vector3.Distance(transform.position, HomePosition);
                if (t > 1.8f)
                {
                    if(HomeDist < HomeMaxDistance)
                    {
                        SetDirection();
                        Rotation(-chardir, -GravityDir, AttackRotationSpeed);
                        rigid.AddForce(transform.forward * ChaseSpeed, ForceMode.Acceleration);
                    }
                    else
                    {
                        anim.SetTrigger("Reset");
                        Action = 0;
                        t = 0;
                    }

                    if(tgtDist < TooCloseToPlayerThreshold) 
                    {
                        anim.SetTrigger("Reset");
                        Action = 0;
                        t = 0;
                    }
                }
            }
        }

        // APPLY DRAG
        rigid.linearVelocity = Vector3.Lerp(rigid.linearVelocity, Vector3.zero, Drag * Time.fixedDeltaTime);

        // FUNCTION
        CheckDeath();
    }

    // FUNCTIONS
    public void FakeUpdate()
    {
        // LOOK FOR CLOSEST TARGET
        if (TargetData.Targets != null)
        {
            CurrentTarget = null;
            ClosestTargetDistance = 999999;
            tgtDist = 9999999;
            for (int i = 0; i < TargetData.Targets.Count; i++)
            {
                if(TargetData.Targets[i].TargetFaction.FactionName == TargetFaction.FactionName)
                {
                    tgtDist = Vector3.Distance(TargetData.Targets[i].transform.position, transform.position);
                    if (tgtDist < ClosestTargetDistance)
                    {
                        CurrentTarget = TargetData.Targets[i];
                        ClosestTargetDistance = tgtDist;
                    }
                }
            }
        }
    }

    Quaternion gravrot;
    public void RotateToGravity(Vector3 GravityDir, float RotationSpeed)
    {
        gravrot = transform.rotation;
        gravrot = Quaternion.FromToRotation(transform.up, -GravityDir) * gravrot;
        transform.rotation = Quaternion.Slerp(transform.rotation, gravrot, Time.fixedDeltaTime * RotationSpeed);
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
        if (CurrentTarget)
        {
            chardir = (transform.position - CurrentTarget.transform.position).normalized;
        }
    }

    public IEnumerator TriggerIHB(int index, float time)
    {
        yield return new WaitForSeconds(time);
        IHB.StartIHB(Hitboxes[index]);
        
    }

    GameObject g;
    void CheckDeath()
    {
        // SPAWN DEATH EXPLOSION AND "DEAD BODY"
        if (dead == false && HP <= 0.0f)
        {
            dead = true;
            if (DieExplosion != null)
            {
                g = GameObject.Instantiate(DieExplosion);
                g.transform.position = transform.position;
                g.transform.rotation = transform.rotation;
            }

            if (DieShell != null)
            {
                g = GameObject.Instantiate(DieShell);
                g.transform.position = transform.position;
                g.transform.rotation = transform.rotation;
                if (g.GetComponent<Rigidbody>() != null)
                {
                    g.GetComponent<Rigidbody>().linearVelocity = Vector3.ProjectOnPlane(chardir * -FlyAwayForce, -GravityDir)
                        + (-GravityDir * FlyUpForce);
                    g.GetComponent<Rigidbody>().AddTorque(chardir * RotateAwaySpeed, ForceMode.VelocityChange);
                }
            }

            // GIVE POINTS & ENERGY
            CharacterInteractions playerpoints;
            if (CharacterCamera.Main.Char.TryGetComponent<CharacterInteractions>(out playerpoints))
            {
                playerpoints.SecondaryPenaltyCounter = points.GracePeriod;
                playerpoints.AddScore(points.Score, points.MultiplierAdd, points.TimeAdded, points.ItenName);
                playerpoints.En += EnergyOnDeath;
            }

            Destroy(gameObject);
        }
    }

    public IEnumerator DamageSkinFlash(float time)
    {
        for (int i = 0; i < Meshes.Length; i++)
        { Meshes[i].material.SetFloat("_HitFX", 1); }

        yield return new WaitForSeconds(time);

        for (int i = 0; i < Meshes.Length; i++)
        { Meshes[i].material.SetFloat("_HitFX", 0); }

    }

    private void OnDestroy()
    {
        this.StopAllCoroutines();
    }

    // COLLISION
    HitboxData h;
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Hitbox"))
        {
            h = null;
            if (col.TryGetComponent<HitboxData>(out h))
            {
                HP -= (h.PhysicalDamage + h.ElementalDamage);
                DamageSkinFlash(SkinDamageTime);
                CheckDeath();
            }
        }
    }
}
