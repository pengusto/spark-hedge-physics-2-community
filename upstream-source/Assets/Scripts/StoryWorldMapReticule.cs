using UnityEngine;

public class StoryWorldMapReticule : MonoBehaviour
{
    public Animator anim;
    public ValuesHolder Hit;
    ValuesHolder v;

    private void OnTriggerStay(Collider col)
    {
        if(col.tag == "Hitbox")
        {
            if(col.TryGetComponent<ValuesHolder>(out v))
            {
                Hit = v;
            }
            else
            {
                Hit = null;
            }
        }
        else
        {
            Hit = null;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Hit = null;
    }
}
