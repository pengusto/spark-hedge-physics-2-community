using UnityEngine;
using UnityEngine.UI;

public class CutsceneCard : MonoBehaviour
{
    [Header("Main")]
    public Image Card;
    public AudioSource NewCardAudio;
    public float DistortIntensity = 0.17f;
    public float DistorySpeed = 10;

    [Header("Followup")]
    public GameObject FollowUpCard;
    public float FollowUpTime = 2;

    //CACHE
    Material CardMaterial;
    float lerp;
    float distort;

    void Start()
    {
        Initializing();
    }

    void OnEnable()
    {
        Initializing();
    }

    void Initializing()
    {
        CardMaterial = Card.material;
        lerp = 0;
        distort = 0;
        if(NewCardAudio) NewCardAudio.Play();
    }

    void Update()
    {
        lerp += Time.deltaTime * DistorySpeed;
        CardMaterial.SetFloat("_Dist", Mathf.Lerp(DistortIntensity, 0, lerp));

        if (FollowUpCard)
        {
            if(lerp / DistorySpeed >= FollowUpTime)
            {
                this.gameObject.SetActive(false);
                FollowUpCard.gameObject.SetActive(true);
            }
        }
    }
}
