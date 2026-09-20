using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JesterSwipe : MonoBehaviour
{
    [Header("References")]
    public CharacterPhysics Phys;
    public CharacterActions Actions;
    public CharacterInput Inp;

    [Header("Parameters")]
    public float ScanRate = 0.2f;
    public float ScanRadius = 10;
    public LayerMask Mask;

    [Header("Path Parameters")]
    public float MarchDistance = 0.5f;
    public float MaximunDistance = 1.5f;
    public float DotThreshold = -0.5f;

    [Header("Swipeing")]
    public float SwipeSpeed = 0.032f;
    public float MinExitSpeed = 6f;
    public float ExitSpeedMultiplier = 1.2f;

    [Header("Cache")]
    public Collider[] c = new Collider[50];
    public List<Collectable> Points = new List<Collectable>();
    public List<Collectable> Route = new List<Collectable>();
    Collectable collec = new Collectable();
    Vector3 pos;
    Vector3 InpDir;
    Vector3 Dir;
    public float closest = 999;
    public float dist;
    public int CurrentID = 0;
    public float dot;
    public bool Swiping = false;
    public bool SwipeAvailable = false;
    int removeAt;
    float swipetime = 0;

    private void Start()
    {
        InvokeRepeating("FakeUpdate", 0.1f, ScanRate);
    }

    private void FixedUpdate()
    {
        if (Inp.JDash)
        {
            FakeUpdate();
            if (SwipeAvailable && Swiping == false && Inp.WeakTarget == false && Actions.Action == 0 && Route.Count > 0)
            {
                swipetime = 0;
                IHB.StartIHB(Actions.Basic.JesterDashIHB);
                for (int i = 0; i < Route.Count; i++)
                {
                    if (i <= Route.Count - 2) { StartCoroutine(SwipeToTarget(swipetime, Route[i].ThisTransform.position, false)); }
                    else { StartCoroutine(SwipeToTarget(swipetime, Route[i].ThisTransform.position, true)); }
                    swipetime += SwipeSpeed;
                }
            }
        }

    }

    IEnumerator SwipeToTarget(float time, Vector3 point, bool end)
    {
        Swiping = true;
        yield return new WaitForSeconds(time);
        Phys.rigid.MovePosition(point);
        if (end) 
        { 
            if(Phys.SpeedMagnitude < MinExitSpeed) { Phys.rigid.linearVelocity = InpDir * MinExitSpeed; }
            else { Phys.rigid.linearVelocity *= ExitSpeedMultiplier; }
            Swiping = false;
            // MISC
            if (Actions.Basic != null)
            {
                Actions.Basic.anim.SetTrigger("Dash");
                Actions.Basic.SubAction = 0;
                Actions.Basic.SubActionTime = 0;
                Actions.Basic.DoubleJumpAvailable = true;
                Actions.Basic.AirDashAvailable = true;
                Actions.Basic.WallwalkTime = 0;
            }
        }
    } 

    void FakeUpdate()
    {
        if (Swiping == false)
        {
            // SCAN AND ADD TO LIST
            Physics.OverlapSphereNonAlloc(transform.position, ScanRadius, c, Mask, QueryTriggerInteraction.Collide);
            Points.Clear();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] != null)
                {
                    if (c[i].TryGetComponent<Collectable>(out collec))
                    {
                        if (collec.Swippable)
                        {
                            Points.Add(collec);
                        }
                    }
                }
            }

            // DIRECTIONS
            if (Inp.InputMag > 0.1f) { InpDir = Inp.CamRelativeInput.normalized; }
            else { InpDir = Inp.Actions.Basic.Skin.transform.forward; }

            // MAKE PATH
            Route.Clear();
            SwipeAvailable = false;
            pos = transform.position;
            CurrentID = 0;
            //Debug.DrawRay(pos, InpDir * MarchDistance, new Color(Random.Range(0, 1), 1, 0));

            for (int i = 0; i < Points.Count; i++)
            {
                collec = GetClosest(pos);
                if (collec != null)
                {
                    Route.Add(collec);
                    pos = collec.transform.position;
                    //Debug.DrawRay(pos, InpDir * MarchDistance, new Color(Random.Range(0, 1), 1, 0));
                    pos += InpDir * MarchDistance;
                    SwipeAvailable = true;
                }
            }

            // FUNCTIONS
            Collectable GetClosest(Vector3 pos)
            {
                collec = null;
                closest = 9999999;
                for (int i = 0; i < Points.Count; i++)
                {
                    dist = Vector3.Distance(Points[i].transform.position, pos);
                    Dir = (pos - Points[i].ThisTransform.position).normalized;
                    dot = Vector3.Dot(InpDir, Dir);
                    if (closest > dist && dot < DotThreshold && dist < MaximunDistance)
                    {
                        closest = dist;
                        collec = Points[i];
                        removeAt = i;
                    }
                }
                if (removeAt < Points.Count) { Points.RemoveAt(removeAt); }
                return collec;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < Route.Count; i++)
        {
            Gizmos.color = Color.Lerp(Color.blue, Color.red, i / Route.Count);
            Gizmos.DrawWireSphere(Route[i].transform.position, 0.1f + (i * 0.01f));
        }
    }
}
