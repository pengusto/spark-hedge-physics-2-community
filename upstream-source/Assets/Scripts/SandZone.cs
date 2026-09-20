using UnityEngine;

public class SandZone : MonoBehaviour
{
    [Header("References")]
    public GameObject SandParticle;

    [Header("Parameters")]
    public string HitTag = "Player";
    public float SandMinSpeed = 1;
    public float SandDrag = 0.1f;
    public float SandParticleFrequency = 0.1f;

    [Header("CACHE")]
    public CharacterPhysics Phys;
    public float SandTime;

    private void OnTriggerEnter(Collider col)
    {
        if (col.attachedRigidbody != null)
        {
            if (col.attachedRigidbody.CompareTag(HitTag))
            {
                if (col.attachedRigidbody.TryGetComponent<CharacterPhysics>(out Phys))
                {
                    // SPAWN SAND PARTICLE ON ENTER
                    GameObject g = GameObject.Instantiate(SandParticle);
                    g.transform.position = Phys.transform.position;
                }
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.attachedRigidbody != null)
        {
            if (col.attachedRigidbody.CompareTag(HitTag))
            {
                if (col.attachedRigidbody.TryGetComponent<CharacterPhysics>(out Phys))
                {
                    // AFFECT PHYSICS
                    if (Phys.SpeedMagnitude > SandMinSpeed)
                    {
                        Phys.rigid.linearVelocity = Vector3.Lerp(Phys.rigid.linearVelocity,
                            Vector3.zero, Time.fixedDeltaTime * SandDrag);

                        SandTime += Time.deltaTime;
                        if(SandTime > SandParticleFrequency) 
                        {
                            GameObject g = GameObject.Instantiate(SandParticle);
                            g.transform.position = Phys.transform.position;
                            SandTime = 0;
                        }
                    }
                }
            }
        }
    }
}
