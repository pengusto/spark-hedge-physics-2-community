using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPathfinding : MonoBehaviour
{

    public int MaxSteps = 12;
    public float MarchDistance = 0.5f;
    public float InitRadius = 1;
    public Transform Target;
    public CharacterInput Inp;
    public LayerMask mask;

    private void Update()
    {
        Inp.Pathfinding(MaxSteps, InitRadius, MarchDistance, transform.up, transform, Target, mask);
    }
}
