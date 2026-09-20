using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetData : MonoBehaviour
{
    [Header("Parameters")]
    public CharacterActions AttachedCharacter;
    public EntityInfo TargetFaction;
    public int Priority = 1;
    public bool IsLockedOnTo = false;
    public bool IsHostile = false;
    public bool IsWeak = false;
    public bool NoIcon = false;
    public float TargetRadius = 1;
    public float SecondaryRadius = 1.1f;
    public float CameraRadius = 6;

    // STATIC CACHE
    public static List<TargetData> Targets = new List<TargetData>();

    private void Start()
    {
        CleanTargetsOfNulls();
        Targets.Add(this);
    }

    public static void CleanTargetsOfNulls()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i] == null)
            {
                Targets.RemoveAt(i);
                Debug.LogWarning("Null Target Removed at index: " + i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, TargetRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, SecondaryRadius);
        Gizmos.color = Color.cyan / 2;
        Gizmos.DrawWireSphere(transform.position, CameraRadius);
    }
}

[System.Serializable]
public class EntityInfo
{
    public string EntityName;
    public string FactionName;
}


