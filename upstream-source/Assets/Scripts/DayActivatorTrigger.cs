using UnityEngine;

public class DayActivatorTrigger : MonoBehaviour
{
    public DateActivator[] Scripts;

    void Update()
    {
        for (int i = 0; i < Scripts.Length; i++)
        {
            Scripts[i].Update();
        }

        this.enabled = false;
    }
}
