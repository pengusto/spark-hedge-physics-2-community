using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    [Header("References")]
    public BoxCollider MainTrigger;
    public GameObject Barrier;
    public GameObject EndIHB;
    public List<CombatZoneEnemyWave> Waves = new List<CombatZoneEnemyWave>();

    [Header("Parameters")]
    public float StartDelay = 0.5f;
    public float InterWaveDelay = 2;
    public float EndDelay = 1.5f;
    public bool CompleteStageAfter = false;

    [Header("Cache")]
    CharacterActions PlayerActions;
    CharacterInteractions CharInteraction;
    public bool Active;
    public int Index = 0;
    float t;

    void Update()
    {
        if (Active)
        {
            // CHECK IF ENEMIES ARE DEAD AND SPAWN NEW WAVE OR END
            t += Time.deltaTime;
            if(t > StartDelay + 0.1f)
            {
                if (Waves[Index].Done)
                {
                    t = InterWaveDelay;
                    Index++;

                    // END
                    if(Index >= Waves.Count)
                    {
                        t = 0;
                        Active = false;
                        StartCoroutine(End(EndDelay));
                    }
                    // CONTINUE
                    else
                    {
                        Active = false;
                        StartCoroutine(StartWave(InterWaveDelay));
                    }
                }
            }
        }
    }

    public IEnumerator StartWave(float time)
    {
        Active = true;
        if (MainTrigger) { MainTrigger.enabled = false; }
        if (Barrier) { Barrier.SetActive(true); }
        yield return new WaitForSeconds(time);
        Waves[Index].gameObject.SetActive(true);
    }

    public IEnumerator End(float time)
    {
        yield return new WaitForSeconds(time);
        if (EndIHB) 
        { 
            EndIHB.gameObject.SetActive(true);
            IHB.StartIHB(EndIHB);

            // TRIGGER TO END STAGE
            if (CompleteStageAfter)
            {
                if(PlayerActions.TryGetComponent<CharacterInteractions>(out CharInteraction))
                {
                    CharInteraction.CompleteTrigger = true;
                }
            }
        }

        if (Barrier)
        {
            Barrier.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (col.attachedRigidbody && col.attachedRigidbody.TryGetComponent<CharacterActions>(out PlayerActions))
            {
                if (PlayerActions.Inp.Player == true)
                {
                    StartCoroutine(StartWave(StartDelay));
                }
            }
        }
    }

    //private void OnCollisionEnter(Collision col)
    //{
    //    if (col.gameObject.tag == "Player")
    //    {
    //        if (col.rigidbody && col.rigidbody.TryGetComponent<CharacterActions>(out PlayerActions))
    //        {
    //            StartCoroutine(StartWave(StartDelay));
    //        }
    //    }
    //}

    private void OnDestroy()
    {
        
    }
}
