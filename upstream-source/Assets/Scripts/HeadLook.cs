using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLook : MonoBehaviour
{
    [Header("References")]
    public CharacterPhysics Char;
    public CharacterInput Inp;
    public CharacterActions Actions;
    public Transform Head;

    [Header("Params")]
    public bool Look = true;
    public bool UseCurrentTarget = true;
    public float LookSpeed = 10;
    public float SideLookLimits = 0.1f;
    public float UpLookLimits = 0.5f;
    public bool AlternateHorizontalMode = false;

    [Header("Cache")]
    public Transform LookAt;
    Vector3 dir;
    Vector3 euler;
    Quaternion q;
    float ds;
    float du;
    float t;

    private void LateUpdate()
    {     
        if(Char && Inp && Actions && Inp.CurrentTarget != null)
        {
            if (UseCurrentTarget) { LookAt = Inp.CurrentTarget.transform; }

            if (Inp.CurrentTarget && Look && LookAt && Actions.Action <= 0)
            {
                euler = Head.eulerAngles;
                dir = LookAt.position - Head.transform.position;
                dir = dir / dir.magnitude;

                if (AlternateHorizontalMode) { ds = Vector3.Dot(Head.right, dir); }
                else { ds = Vector3.Dot(Head.forward, dir); }               
                du = Vector3.Dot(Head.up, dir);

                if(ds > SideLookLimits && (du > -UpLookLimits && du < UpLookLimits))
                {
                    t = 0;
                    q = Quaternion.Lerp(q, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * LookSpeed);
                }
                else
                {
                    if (t < 2) 
                    { 
                        q = Quaternion.Lerp(q, Head.rotation, Time.deltaTime * LookSpeed);
                        t += Time.deltaTime; 
                    }
                    else 
                    { 
                        q = Head.rotation; 
                    }
                }

                Head.rotation = q;
            }
            else
            {     
                if (t < 2) 
                { 
                    q = Quaternion.Lerp(q, Head.rotation, Time.deltaTime * LookSpeed);
                    t += Time.deltaTime; 
                }
                else
                { 
                    q = Head.rotation; 
                }

                Head.rotation = q;
            }
        }
    }
}
