using UnityEngine;

public class LagMachine : MonoBehaviour
{

    public int LagIntensity = 1000000;
    float a;

    void FixedUpdate()
    {
        for (int i = 0; i < LagIntensity; i++)
        {
            a = Mathf.Sin(10);
        }
    }
}
