using UnityEngine;

public class Fan : MonoBehaviour
{
    [Header("Parameters")]
    public float Power = 1;
    public AnimationCurve DotMultiplier;

    [Header("Cache")]
    CharacterPhysics p;
    public float dot;
    
    private void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player")
        {
            if(col.attachedRigidbody.TryGetComponent<CharacterPhysics>(out p))
            {
                dot = Mathf.Lerp(dot, Vector3.Dot(p.SpeedDirection, transform.forward), Time.fixedDeltaTime * 5);
                p.rigid.linearVelocity += (transform.forward * DotMultiplier.Evaluate(dot) * Power) * Time.fixedDeltaTime;
            }
        }
    }
}
