using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rigid;
    public EntityInfo ProjectileFaction;
    public List<EntityInfo> TargetFactions;
    public bool AnyFaction = true;
    public bool CollideWithUntagged = true;
    public GameObject ExplosionObject;

    [Header("Parameters")]
    public float TimeToStart = 0.2f;
    public float TimeToEnd = 5;
    public bool ExplodeOnEnd = true;
    public bool ExplosionOnTargetPosition = false;
    float t = 0;

    [Header("Speed")]
    public bool UseSpeed = true;
    public Vector3 MovementSpeed = Vector3.one;
    public Vector3 Gravity = Vector3.zero;
    public float Drag = 0;

    // CACHE
    GameObject g;
    CharacterPhysics phys;

    void FixedUpdate()
    {
        if (UseSpeed)
        {
            MovementSpeed += Gravity;
            MovementSpeed = Vector3.Lerp(MovementSpeed, Vector3.zero, Time.fixedDeltaTime * Drag);
            rigid.linearVelocity = MovementSpeed;
        }

        t += Time.fixedDeltaTime;

        if (t > TimeToEnd) { EndProjectile(null); }
    }

    private void OnTriggerStay(Collider col)
    {
        if(t > TimeToStart && col.isTrigger == false)
        {
            if (CheckIfTargetToCollider(col, null))
            {
                EndProjectile(col.gameObject);
            }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (t > TimeToStart)
        {
            if (CheckIfTargetToCollider(null, col))
            {
                EndProjectile(col.gameObject);
            }
        }
    }


    // FUNCTIONS =====

    public CharacterInput inp;

    public bool CheckIfTargetToCollider(Collider a, Collision b)
    {
        inp = null;
        if(a != null) // FOR TRIGGERS
        {
            if (AnyFaction) 
            { 
                return true;
            }
            else
            {
                if (CollideWithUntagged) 
                {
                    if (a.tag == "Untagged") { return true; } 
                }

                // CHARACTER COLLISION
                if (a.attachedRigidbody != null)
                {       
                    if (a.attachedRigidbody.TryGetComponent<CharacterInput>(out inp))
                    {
                        for (int i = 0; i < TargetFactions.Count; i++)
                        {
                            if (inp.CharacterFaction.FactionName == TargetFactions[i].FactionName) 
                            {
                                Debug.Log("Trigger, Faction: " + TargetFactions[i].FactionName);
                                return true;
                            }
                        }
                    }
                }
            }

        }
        else if(b != null) // FOR COLLIDERS
        {
            if (AnyFaction) 
            { 
                return true;
            }
            else
            {
                if (CollideWithUntagged) { if (b.gameObject.tag == "Untagged") { return true; } }

                // CHARACTER COLLISION
                if (b.rigidbody != null) 
                {
                    if (b.rigidbody.TryGetComponent<CharacterInput>(out inp))
                    {
                        for (int i = 0; i < TargetFactions.Count; i++)
                        {
                            if (inp.CharacterFaction.FactionName == TargetFactions[i].FactionName)
                            {
                                Debug.Log("Collision, Faction: " + TargetFactions[i].FactionName);
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public void EndProjectile(GameObject collider)
    {
        if (ExplodeOnEnd && ExplosionObject)
        {
            g = Instantiate(ExplosionObject);

            if (ExplosionOnTargetPosition)
            {
                if (collider) 
                { 
                    if(collider.transform.root.TryGetComponent<CharacterPhysics>(out phys))
                    {
                        g.transform.position = collider.transform.position;
                    }
                    else
                    {
                        g.transform.position = transform.position;
                    }
                }
                else 
                { g.transform.position = transform.position; }
                g.transform.rotation = Quaternion.identity;
            }
            else
            {
                g.transform.position = transform.position;
                g.transform.rotation = Quaternion.identity;
            }

            Destroy(gameObject);
        }
    }

}
