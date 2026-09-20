using UnityEngine;

public class LookAt : MonoBehaviour
{
    [Header("References")]
    public CharacterActions Actions;
    public Transform LookSource;
    public Transform UpSource;
    public float Multiplier = 1;
    public float Y_Multiplier = 1;
    Quaternion rot;
    Vector3 dir;


    void Start()
    {
        if (Actions)
        {
            LookSource = Actions.Inp.CharCam.transform;
            UpSource = Actions.Inp.CharCam.transform;
        }

    }

    void Update()
    {
        dir = (transform.position - LookSource.position).normalized;
        dir.y *= Y_Multiplier;
        rot = transform.rotation;
        rot = Quaternion.LookRotation(dir * Multiplier, LookSource.up);
        transform.rotation = rot; 
    }
}
