using UnityEngine;

public class ChangeSceneOnEnable : MonoBehaviour
{
    public string Scene;
    public void Start()
    {
        SceneController.LoadStageLoading(Scene);
    }
}
