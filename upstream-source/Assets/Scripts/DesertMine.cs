using UnityEngine;

public class DesertMine : MonoBehaviour
{
    [Header("References")]
    public GameObject Mine;
    public GameObject MineExplosion;

    [Header("Parameters")]
    public float MineExplodeTime = 3;
    public string HitTag = "Player";
    public float t;

    private void Start()
    {
        Mine.SetActive(false);
    }

    private void Update()
    {
        if (Mine.activeSelf)
        {
            t += Time.deltaTime;
            if(t > MineExplodeTime)
            {
                GameObject g;
                g = GameObject.Instantiate(MineExplosion);
                g.transform.position = Mine.transform.position;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(Mine.activeSelf == false && col.tag == HitTag)
        {
            Mine.SetActive(true);
        }

        if(col.tag == "Hitbox")
        {
            Mine.SetActive(true);
        }
    }
}
