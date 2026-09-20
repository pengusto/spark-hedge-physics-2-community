using UnityEngine;

public class IHB_Trigger : MonoBehaviour
{
    [Header("Parameters")]
    public bool ExecuteOnStart = false;
    public bool ExecuteOnEnable = false;
    public GameObject[] Boxes;

    private void Start()
    {
        if (ExecuteOnStart) { ExecuteIHB(); }
    }

    private void OnEnable()
    {
        if (ExecuteOnEnable) { ExecuteIHB(); }
    }



    public void ExecuteIHB()
    {
        for (int i = 0; i < Boxes.Length; i++)
        {
            IHB.StartIHB(Boxes[i]);
        }
    }
}
