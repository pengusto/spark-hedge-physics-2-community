using UnityEngine;

public class DebugSceneChange : MonoBehaviour
{
    public string Scene;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneController.LoadStageLoading(Scene);
        }
    }
}
