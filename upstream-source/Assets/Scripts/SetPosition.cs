using UnityEngine;

public class SetPosition : MonoBehaviour
{
    [Header("Parameters")]
    public Transform Main;
    public Transform Target;
    public bool SetPos = true;

    void Update()
    {
        if(Main != null)
        {
            if (SetPos) { Main.position = Target.position; }
        }
    }
}
