using UnityEngine;

public class RandomAnimatorSpeed : MonoBehaviour
{
    public Animator Anim;
    public Vector2 Range = new Vector2(0.5f, 1f);
    public bool UseNormalRandom = true;

    void Start()
    {
        if (UseNormalRandom)
        {
            Anim.speed = Random.Range(Range.x, Range.y);
        }
    }

    
}
