using UnityEngine;

public class MapEffects : MonoBehaviour
{
    [Header("References")]
    public GameObject GhostDayEffects;

    // CACHE
    public static bool GhostDay = false;

    private void Start()
    {
        if (GhostDay)
        {
            if (GhostDayEffects) { GhostDayEffects.SetActive(true); }
        }
    }
}
