using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceMaterial { Default, Metal, HollowMetal, Sand, Snow, Dust, LowFriction, Grass }

public class SurfaceMaterialInfo : MonoBehaviour
{
    public SurfaceMaterial Mat = SurfaceMaterial.Default;
    public bool MovingPlatform = false;
    public bool ParentRegion = false;
    public Transform ParentRegionRoot;
}
