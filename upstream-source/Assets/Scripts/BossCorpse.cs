using UnityEngine;
using UnityEngine.UI;

public class BossCorpse : MonoBehaviour
{
    [Header("References")]
    public CharacterActions Player;
    public SimpleBossController BossController;
    public CharacterCamera Cam;
    public CharacterActions Boss;
    public PauseMenu Pause;
    public Transform Target;
    public Image Fade;

    [Header("Parameters")]
    public float Distance = 6;
    public float CameraHeight = 0.2f;
    public float RotateSpeed = 1;
    public float Timescale = 0.05f;
    public float EndTime = 3;

    [Header("Cache")]
    public Vector3 CameraAngle;
    public float t = 0;
    public float fade = 0;
    public Color a = new Color(0, 0, 0, 0);
    public Color b = new Color(0, 0, 0, 1);

    private void Start()
    {
        // GET REFS
        Player = CharacterCamera.Main.Char.GetComponent<CharacterActions>();
        Cam = CharacterCamera.Main;
        BossController = SimpleBossController.Instance;
        Boss = SimpleBossController.Instance.BossCharacter;
        Fade = Pause.Fade;

        // SET STUFF
        Cam.enabled = false;
        CameraAngle = Player.transform.forward;
        Boss.gameObject.SetActive(false);
    }

    private void Update()
    {
        Time.timeScale = Timescale;
        CameraAngle = Quaternion.AngleAxis(RotateSpeed * Time.unscaledDeltaTime, Player.transform.up) * CameraAngle;
        Cam.transform.position = Target.position + (CameraAngle * -Distance);
        Cam.transform.position += (Player.transform.up * CameraHeight);
        Cam.transform.LookAt(Target, Player.transform.up);

        if (t > EndTime)
        {
            Fade.enabled = true;
            Fade.color = Color.Lerp(a, b, fade);
            fade += Time.unscaledDeltaTime * 0.5f;
            if(fade > 1.1f)
            {
                for (int i = 0; i < BossController.ToActivateOnEnd.Length; i++)
                {
                    BossController.ToActivateOnEnd[i].SetActive(true);
                }
            }
        }
        else
        {
            t += Time.unscaledDeltaTime;
        }
    }
}
