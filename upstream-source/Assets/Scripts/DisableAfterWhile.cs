using UnityEngine;

public class DisableAfterWhile : MonoBehaviour
{
    [Header("Parameters")]
    public float TimeToDisable = 0.1f;

    [Header("Cache")]
    public float t;

    void Update()
    {
        t += Time.deltaTime;
        if(t > TimeToDisable)
        {
            t = 0;
            gameObject.SetActive(false);
        }
    }
}
