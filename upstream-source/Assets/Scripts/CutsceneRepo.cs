using UnityEngine;
using UnityEngine.UI;

public class CutsceneRepo : MonoBehaviour
{
    [Header("Parameters")]
    public Conversation[] Convos;
    public SingleSceneCutsceneInfo[] CurrentSceneInfo;
    public GameObject[] ConvoObjects;
    public Image Fade;

    [Header("Cache")]
    public Conversation CurrentConvo;
    public SingleSceneCutsceneInfo Next;
    float endTime;
    float fadetime;
    float t;
    bool started = false;
    public int ForceCutscene = -1;

    // STATICS
    public static int ConvoToPlay = 0;
    public static float TimeToStart = 1;
    public static float FadeStartSpeed = 1;
    public static float FadeEndSpeed = 1;
    public static float EndTime = 2;
    public static string SceneToGoAfter = "";
    public static bool ConvoScene = false;

    private void Start()
    {
        ConvoScene = true;
        if(ForceCutscene >= 0) { ConvoToPlay = ForceCutscene; }
        StartCutscene(Convos[ConvoToPlay], CurrentSceneInfo[ConvoToPlay], ConvoObjects[ConvoToPlay]);
    }

    void Update()
    {

        // MANAGE FADE AND IF CONVO IS OVER
        if(CurrentConvo != null)
        {
            // PREP CONVO TO START AND FADE OUT
            if (started == false)
            {
                t += Time.deltaTime;
                if (t > TimeToStart)
                {
                    if (CurrentConvo)
                    {
                        CurrentConvo.enabled = true;
                        CurrentConvo.Active = true;
                        started = true;
                    }
                    else
                    {
                        Debug.LogError("Invalid Cutscene to load, SOFTLOCK");
                    }
                }

                FadeFunction(Fade, true);
            }
            else // DURING CONVO AND POST CONVO ACTIONS
            {
                if (CurrentConvo.Active)
                {
                    FadeFunction(Fade, true);
                }
                else
                {
                    endTime += Time.deltaTime;
                    FadeFunction(Fade, false);
                    if (endTime > EndTime)
                    {
                        ConvoToPlay = Next.NextConvoID;
                        TimeToStart = Next.TimeToStart;
                        FadeStartSpeed = Next.FadeStartSpeed;
                        FadeEndSpeed = Next.FadeEndSpeed;
                        EndTime = Next.EndTime;
                        SceneController.LoadStageLoading(Next.SceneToGoAfter);
                    }
                }
            }
        }
        else
        {
            FadeFunction(Fade, true);
        }

        // FX
        void FadeFunction(Image image, bool fade)
        {
            if (fade)
            {
                image.enabled = true;
                fadetime += FadeStartSpeed * Time.deltaTime; 
            }
            else
            {
                image.enabled = true;
                fadetime -= FadeEndSpeed * Time.deltaTime;
            }

            image.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), fadetime);
            fadetime = Mathf.Clamp01(fadetime);
        }
    }

    void StartCutscene(Conversation d, SingleSceneCutsceneInfo info, GameObject g)
    {
        Next = info;
        CurrentConvo = d;

        ConvoToPlay = info.NextConvoID;
        TimeToStart = info.TimeToStart;
        FadeStartSpeed = info.FadeStartSpeed;
        FadeEndSpeed = info.FadeEndSpeed;
        EndTime = info.EndTime;

        g.SetActive(true);
    }

    private void OnDestroy()
    {
        // RESET EVERYTHING
        ConvoScene = false;
    }
}
