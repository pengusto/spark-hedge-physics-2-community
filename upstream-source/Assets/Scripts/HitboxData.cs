using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxData : MonoBehaviour
{
    [Header("References")]
    public bool ExecuteOnStart = false;
    public Collider HitboxCol;
    public Collider[] HitboxColMiltiple;
    public Transform FakeCenter;
    public Transform Parent;
    public GameObject HitObject;
    public CharacterActions Owner;

    [Header("Time Data")]
    public float StartTime = 0.0f;
    public float Duration = 0.1f;
    public bool UseRepeatSystem = false;
    public float RepeatRate = 0.1f;
    float RepeatCounter = 0;

    [Header("Faction Data")]
    public EntityInfo BoxFaction;
    public bool FriendlyFire = false;

    [Header("Damage Data")]
    public DamageValueType DamageType = DamageValueType.Normal;
    public float PhysicalDamage = 10;
    public float ElementalDamage = 0;

    [Header("Special Data")]
    public bool NegateInvencibility = false;
    public bool NegateSuperArmour = false;

    [Header("Stagger Data")]
    public DamageStaggerType StaggerType;
    public bool ForceStagger = false;
    public int ForcedStaggerType = 0;
    public float ApDamage = 10;
    public bool DontBuildRevenge = false;

    [Header("Damage Forces")]
    public DamageForceType ForceType = DamageForceType.Set;
    public float AwayForce = 10;
    public bool OnlyUpwardsOnStagger = false;
    public bool OnlyUpwardsIfInAir = false;
    public float UpwardForce = 0;

    [Header ("Invencibility Frames")]
    public bool ForceInvencibility = false;
    public float InvencibilityTime = 0;

    [Header("Hitbox Use FX")]
    public bool UseFX = false;
    public float ShakeDuration = 0;
    public float ShakeAmplitude = 0;
    public float SlowDownDuration = 0;
    public float SlowDownTime = 0;
    public float SlowDownRestore = 0;

    [Header("Hit FX")]
    public bool UseFxWhenHit = false;
    public float ShakeHitDuration = 2;
    public float ShakeHitAmplitude = 0.01f;
    public float SlowDownHitDuration = 0;
    public float SlowDownHitTime = 0;
    public float SlowDownHitRestore = 100;
    public SimpleCameraFxParams EffectsOnPlayerHit;

    //CACHE
    [HideInInspector] public string ParentName = string.Empty;
    [HideInInspector] public float PhysDamageMultiplier = 1;
    [HideInInspector] public float ElemDamageMultiplier = 1;
    [HideInInspector] public float ApDamageMultiplier = 1;
    public enum DamageValueType { Normal, NormalUnblockable, TrueDamage }
    public enum DamageStaggerType { Normal, Strong, Forced }
    public enum DamageForceType { Set, Additive, OnlyAddIfGrounded }

    private void Start()
    {
        if (ExecuteOnStart) { StartHitbox(StartTime, Duration); }
    }

    public void StartHitbox(float time,float duration)
    {
        // PREVENT AN ERROR MESSAGE
        if(this.gameObject.activeSelf == false) { return; }

        // SET PARENT NAME
        if (Parent != null && ParentName != string.Empty) { ParentName = Parent.name; }

        // SET SINGLE COLLIDER
        if (HitboxCol != null)
        {
            if (UseRepeatSystem)
            {
                RepeatCounter = time;
                while(RepeatCounter < Duration)
                {
                    StartCoroutine(ChangeBox(RepeatCounter, true));
                    StartCoroutine(ChangeBox(RepeatCounter + (RepeatRate * 0.5f), false));
                    RepeatCounter += RepeatRate;
                }
            }
            else
            {
                StartCoroutine(ChangeBox(time, true));
                StartCoroutine(ChangeBox(time + duration, false));
            }
        }

        // SET MULTI COLLIDER
        if (HitboxColMiltiple.Length > 0)
        {
            if (UseRepeatSystem)
            {
                RepeatCounter = time;
                while (RepeatCounter < Duration)
                {
                    StartCoroutine(ChangeBoxMulti(RepeatCounter, true));
                    StartCoroutine(ChangeBoxMulti(RepeatCounter + (RepeatRate * 0.5f), false));
                    RepeatCounter += RepeatRate;
                }
            }
            else
            {
                StartCoroutine(ChangeBoxMulti(time, true));
                StartCoroutine(ChangeBoxMulti(time + duration, false));
            }
        }
    }

    public IEnumerator ChangeBox(float time, bool on)
    {
        yield return new WaitForSeconds(time);
        HitboxCol.enabled = on;

        // FX APPLICATION
        if(UseFX && on == true)
        {
            CharacterCamera.SlowDown(SlowDownDuration, SlowDownTime, SlowDownRestore, transform.position);
            CharacterCamera.ShakeCameraAddtive(ShakeDuration, ShakeAmplitude, transform.position);
        }
    }

    public IEnumerator ChangeBoxMulti(float time, bool on)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < HitboxColMiltiple.Length; i++)
        {
            HitboxColMiltiple[i].enabled = on;
        }
    }

    // REPEATER

}
