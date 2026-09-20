using UnityEngine;

public class ActivateDelayed : MonoBehaviour
{
    public float ActivationTime = 1f;
    public GameObject ObjectToActivate;
    float t;

    private void Update()
    {
        t += Time.deltaTime;
        if(t > ActivationTime)
        {
            ObjectToActivate.SetActive(true);
            this.enabled = false;
        }
    }
}
