using UnityEngine;

public class CarTriggerNoticer : MonoBehaviour
{
    public string Tag = "";
    public bool Collided = false;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(Tag))
        {
            Collided = true;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag(Tag))
        {
            Collided = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag(Tag))
        {
            Collided = false;
        }
    }
}
