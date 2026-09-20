using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorSpeed : MonoBehaviour
{
    public Animator Anim;

    [Header("Set Type")]
    public bool SetSpeed = false;
    public bool RandomSpeed = false;

    [Header("Parameters")]
    public Vector2 SpeedRange;

    private void Start()
    {
        if (Anim) 
        {
            if (RandomSpeed)
            {
                Anim.speed = Anim.speed * Random.Range(SpeedRange.x, SpeedRange.y);
            }
            if (SetSpeed)
            {
                Anim.speed = Anim.speed * SpeedRange.x;
            }
        }
    }
}
