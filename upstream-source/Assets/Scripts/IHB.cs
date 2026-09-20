using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I.H.B = Integrated Hit Box.
/// Searches parent and children of a gameobject and triggers all its hitboxes(triggers), particle systems, audio and the IHB_Effects Script.
/// IBH_Effects has a ton of small things that the specific hitbox is able to do, anything else can be added to it with very little performance impact
/// Hint: How are IHBs triggered in the first place? They are usually triggered in scripts contained inside an animator component and its states.
/// They can also be found anywhere else, just refecence a parent GameObject and the IHB will trigger the parent and all children's components.
/// Note: Did I write this? When? It's been so many years...
/// </summary>
public class IHB : MonoBehaviour
{

    public static void StartIHB(GameObject g)
    {
        ExecuteObject(g.transform);
        for (int i = 0; i < g.transform.childCount; i++)
        {
            ExecuteObject(g.transform.GetChild(i));
        }
    }

    public static void StartIHBContinous(GameObject g)
    {
        ExecuteContinuous(g.transform);
        for (int i = 0; i < g.transform.childCount; i++)
        {
            ExecuteContinuous(g.transform.GetChild(i));
        }
    }

    public static void StopIHB(GameObject g)
    {
        StopObject(g.transform);
        for (int i = 0; i < g.transform.childCount; i++)
        {
            StopObject(g.transform.GetChild(i));
        }
    }


    // EXECUTE FUNCTIONS
    static HitboxData h;
    static ParticleSystem ps;
    static AudioSource a;
    static IHB_Effects fx;

    static void ExecuteObject(Transform g)
    {
        if (g.TryGetComponent<ParticleSystem>(out ps)) { ps.Stop(); ps.Play(); }
        if (g.TryGetComponent<AudioSource>(out a)) { a.Play(); }
        if (g.TryGetComponent<HitboxData>(out h)) { h.StartHitbox(h.StartTime, h.Duration); }
        if (g.TryGetComponent<IHB_Effects>(out fx)) { fx.PlayIHBEffects(); }
    }

    static void ExecuteContinuous(Transform g)
    {
        if (g.TryGetComponent<ParticleSystem>(out ps)) { ps.Play(); }
        if (g.TryGetComponent<AudioSource>(out a)) { a.Play(); }
        if (g.TryGetComponent<HitboxData>(out h)) { h.StartHitbox(h.StartTime, h.Duration); }
        if (g.TryGetComponent<IHB_Effects>(out fx)) { fx.PlayIHBEffects(); }
    }

    static void StopObject(Transform g)
    {
        if (g.TryGetComponent<ParticleSystem>(out ps)) { ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); } // USE CUSTOM SCRIPT TO FORCE STOP
        if (g.TryGetComponent<AudioSource>(out a)) { a.Stop(); }
        if (g.TryGetComponent<HitboxData>(out h)) 
        { 
            h.HitboxCol.enabled = false;
            h.StopAllCoroutines();
        }
    }
}
