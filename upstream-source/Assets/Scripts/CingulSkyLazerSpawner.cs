using UnityEngine;

public class CingulSkyLazerSpawner : MonoBehaviour
{
    public int LazerAmmount = 10;
    public float LazerInterval = 1f;
    public float PositionRange = 1;
    public float t = 0;
    public CharacterActions Player;
    public GameObject Lazer;
    public LayerMask LazerMask;
    public System.Random r;
    public Vector3 PositionOffset = new Vector3(0, 0.05f, 0);
    RaycastHit hit;

    private void Start()
    {
        // INITIATE SEED
        r = new System.Random(99229922 + Mathf.RoundToInt(transform.position.x));
        GameObject[] c = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i].TryGetComponent<CharacterActions>(out Player))
            {
                break;
            }
        }
        
        if(Player == null) { this.enabled = false; }
    }

    private void FixedUpdate()
    {
        t += Time.fixedDeltaTime;
        if(t > LazerInterval)
        {
            // RANDOM VECTOR
            Vector3 dir = Vector3.zero;
            dir.x = (float)(r.Next(-1000, 1000) / 1000f);
            dir.y = (float)(r.Next(-1000, 1000) / 1000f);
            dir.z = (float)(r.Next(-1000, 1000) / 1000f);
            //dir = dir.normalized;

            // CREATE LAZER
            Vector3 pos = Player.transform.position + (dir * PositionRange);
            if (Physics.Raycast(pos, Vector3.down, out hit, 10, LazerMask, QueryTriggerInteraction.Ignore))
            {
                GameObject g = GameObject.Instantiate(Lazer);
                Lazer.transform.position = hit.point + PositionOffset;
            }

            // TIMER VARIABLES
            t = 0;
            LazerAmmount -= 1;

            // KILL
            if(LazerAmmount < 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
