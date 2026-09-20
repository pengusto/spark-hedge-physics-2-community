using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Parameter")]
    public float CheckRate = 0.5f;
    public float RateRandomness = 0.5f;
    public float SpawnDistance = 50f;
    public MeshRenderer mesh;
    public GameObject SpawnerParticle;
    public GameObject Enemy;

    //CACHE
    public static List<Transform> Characters = new List<Transform>();
    public static List<GameObject> Enemies = new List<GameObject>();
    static int SpawnerSeed = 555222777;
    static System.Random r = new System.Random(SpawnerSeed);
    float random;
    int i = 0;
    float dist;

    // INITIATE A FUNCTION THAT REPEATS OCASIONALLY WITH A RANDOM INTERVAL
    // THIS MAKES IT SO THERE ARE ONLY A FEW CALLS PER FRAME, THOUSANDS OF THIS OBJECT CAN EXIST
    // WITH VERY LITTLE PERFORMANCE INPACT
    void Start()
    {
        random = (float)(r.Next(0, 9999) / (float)99999f);
        InvokeRepeating("FakeUpdate", CheckRate + (random * RateRandomness), CheckRate + (random * RateRandomness));
        Debug.Log("NEW FAKE UPDATE: " + CheckRate + (random * RateRandomness));
        mesh.enabled = false;
    }

    void FakeUpdate()
    {
        for (i = 0; i < Characters.Count; i++)
        {
            if(Characters[i] != null)
            {
                dist = Vector3.Distance(transform.position, Characters[i].position);
                if (dist < SpawnDistance) { Spawn(); }
            }
        }
    }

    GameObject g;
    public void Spawn()
    {
        if (SpawnerParticle)
        {
            g = Instantiate(SpawnerParticle);
            g.transform.position = transform.position;
            g.transform.rotation = transform.rotation;
        }

        if (Enemy)
        {
            g = Instantiate(Enemy);
            g.transform.position = transform.position;
            g.transform.rotation = transform.rotation;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

}
