using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public float TimeToDisable = 1;
    float t;

    private void OnEnable()
    {
        t = 0;
    }

    private void Update()
    {
        t += Time.deltaTime;
        if(t > TimeToDisable)
        {
            gameObject.SetActive(false);
            t = 0;
        }
    }
}
