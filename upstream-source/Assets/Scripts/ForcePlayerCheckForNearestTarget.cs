using UnityEngine;

public class ForcePlayerCheckForNearestTarget : MonoBehaviour
{
    public CharacterInput PlayerInp;
    public float UpdateRate = 0.3f;

    private void Start()
    {
        InvokeRepeating("FakeUpdate", 0.1f, UpdateRate);
    }

    void FakeUpdate()
    {
        PlayerInp.GetNearTargets();
    }
}
