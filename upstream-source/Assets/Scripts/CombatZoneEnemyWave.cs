using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatZoneEnemyWave : MonoBehaviour
{
    [Header("References")]
    public List<Transform> Positions;
    public List<GameObject> EnemiesToSpawn;
    public List<GameObject> SpawnParticle;
    public float SpawnInterval = 0.1f; 

    [Header("Cache")]
    public List<CharacterInteractions> EnemiesSpawned;
    public List<DroneController> DronesSpawned;
    public MeshRenderer[] Meshes;
    public bool Done = false;
    bool Initiated = false;

    CharacterInteractions inter;
    DroneController drone;
    GameObject g;

    // WHEN OBJECT TURNS ON, SPAWN EVERYTHING AND ADD TO LISTS, THEN IT CHECKS IF THEY ALL DIED
    // AND SET DONE TO TRUE
    private void Start()
    {
        for (int i = 0; i < Meshes.Length; i++) { Meshes[i].enabled = false; }
        StartCoroutine(SpawnEnemy(SpawnInterval, 0));
    }

    public IEnumerator SpawnEnemy(float time, float offset)
    {
        for (int i = 0; i < EnemiesToSpawn.Count; i++)
        {
            yield return new WaitForSeconds(time + (offset * i));
            // ENEMY AND DRONE SPAWNING PLUS ADDING TO LIST SO WE KNOW THEY ARE THERE
            if (EnemiesToSpawn[i] != null && Positions[i] != null)
            {
                g = Instantiate(EnemiesToSpawn[i]);
                g.transform.position = Positions[i].position;
                g.transform.rotation = Positions[i].rotation;
                if (g.TryGetComponent<CharacterInteractions>(out inter)) { EnemiesSpawned.Add(inter); }
                if (g.TryGetComponent<DroneController>(out drone)) { DronesSpawned.Add(drone); }

                if (SpawnParticle != null)
                {
                    // EFFECT SPAWNING
                    g = Instantiate(SpawnParticle[i]);
                    g.transform.position = Positions[i].position;
                    g.transform.rotation = Positions[i].rotation;
                }
            }

            if(i >= EnemiesToSpawn.Count - 1) { Initiated = true; }
        }
    }

    private void FixedUpdate()
    {
        // DONE IS SET TO TRUE THEN AS LONG AS AN ENEMY IS STILL IN THE FIELD IT RESETS TO FALSE
        // UNTIL THERE ARE NO MORE ENEMIES TO RESET IT
        if (Done == false && Initiated)
        {
            Done = true;
            for (int i = 0; i < EnemiesSpawned.Count; i++) 
            { 
                if(EnemiesSpawned[i] != null) { Done = false; }
            }

            for (int i = 0; i < DronesSpawned.Count; i++)
            {
                if (DronesSpawned[i] != null) { Done = false; }
            }
        }

        if(Done == true) 
        {
            EnemiesSpawned.Clear();
            DronesSpawned.Clear();
            this.gameObject.SetActive(false);
            this.enabled = false;
        }
    }

}
