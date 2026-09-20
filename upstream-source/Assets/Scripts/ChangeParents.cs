using UnityEngine;

public class ChangeParents : MonoBehaviour
{
    public Transform NewParent;

    void Awake()
    {
        transform.SetParent(NewParent, false);
    }

    
}
