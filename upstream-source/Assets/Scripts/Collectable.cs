using UnityEngine;

public class Collectable : MonoBehaviour
{
    public enum CollectableType { Points, HP, PointsAndHP, None }
    [Header("Main")]
    public CollectableType ObjectType = CollectableType.Points;
    public bool Swippable = false;
    public Transform ThisTransform;

    [Header("Points")]
    public Points points;

    [Header("HP")]
    public float Hp = 40;


    [System.Serializable]
    public class Points
    {
        public int Score = 1;
        public float MultiplierAdd = 0;
        public float TimeAdded = 0.5f;
        public float GracePeriod = 2;
        public string ItenName;
        public GameObject GetEffect;
        public ActivateOnDistance AoD_Ref;
        public GameObject MainObject;
        public SimpleCameraFxParams FX;
    }
}
