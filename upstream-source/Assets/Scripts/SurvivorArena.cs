using System.Collections;
using UnityEngine;

public class SurvivorArena : MonoBehaviour
{
    [Header("References")]
    public GameObject[] Preps;
    public GameObject[] Lightings;
    public float Frequency = 2;
    public float ReleaseTime = 2;
    public float LightingRandomness = 0.5f;
    public float CooldownTime = -1;
    public float LightingAmmount = 0.2f;

    [Header("Cache")]
    public float t;
    public float randomtime;
    public float randomExecution;
    public System.Random r;

    private void Start()
    {
        r = new System.Random(Mathf.FloorToInt(transform.position.x * 10) + gameObject.name.Length);
    }

    private void Update()
    {
        if(t < Frequency)
        {
            t += Time.deltaTime;
        }
        else
        {
            // START PREPS & THUNDERS
            for (int i = 0; i < Preps.Length; i++)
            {
                randomtime = (r.Next(0, 1000) / 1000f);
                randomExecution = (r.Next(0, 1000) / 1000f);
                if (randomExecution > LightingAmmount)
                {
                    StartCoroutine(StartPrep(randomtime * LightingRandomness, i, Preps[i]));
                }
            }
            t = -100f;
        }
    }

    IEnumerator StartPrep(float time, int index, GameObject g)
    {
        yield return new WaitForSeconds(time);
        IHB.StartIHB(g);
        StartCoroutine(StartLighting(ReleaseTime, CooldownTime, Lightings[index]));
    }
    IEnumerator StartLighting(float time, float returnTime, GameObject g)
    {
        yield return new WaitForSeconds(time);
        IHB.StartIHB(g);
        t = returnTime;
    }
}
