using UnityEngine;

public class NpcHeadLook : MonoBehaviour
{
    [Header("References")]
    public Transform NpcHead;
    public Transform PlayerHead;
    public float DistThreshold = 5;
    public Vector2 DistCheckTime = new Vector2(0.5f, 0.5f);
    public float DotThreshold = 0.7f;
    public float HeadRotationSpeed =  6;

    [Header("Cache")]
    public float Distance;
    public bool Close;
    public Vector3 dir;
    public float dot;
    Quaternion q;
    Quaternion initialRotation;
    Quaternion finalrot;

    private void Start()
    {
        InvokeRepeating("FakeUpdate", 0.1f, DistCheckTime.x + Random.Range(0, DistCheckTime.y));
        initialRotation = NpcHead.rotation;
        finalrot = initialRotation;
    }

    //private void Update()
    //{
    //    initialRotation = NpcHead.rotation;
    //}

    private void LateUpdate()
    {
        if (Close)
        {
            dot = -Vector3.Dot(NpcHead.forward, dir);
            dir = (NpcHead.position - PlayerHead.position).normalized;
            q = Quaternion.LookRotation(-dir, transform.right);
            if (dot > DotThreshold)
            {
                finalrot = Quaternion.Lerp(finalrot, q, HeadRotationSpeed * Time.deltaTime);
            }
            else
            {
                finalrot = Quaternion.Lerp(finalrot, initialRotation, HeadRotationSpeed * Time.deltaTime);
            }

            NpcHead.rotation = finalrot;
        }
        else 
        {
            finalrot = Quaternion.Lerp(finalrot, initialRotation, HeadRotationSpeed * Time.deltaTime);
            NpcHead.rotation = finalrot;
        }
    }

    void FakeUpdate()
    {
        Distance = Vector3.Distance(NpcHead.position, PlayerHead.position);
        if(Distance < DistThreshold) { Close = true; } else { Close = false; }
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

}
