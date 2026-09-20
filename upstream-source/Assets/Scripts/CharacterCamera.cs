using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CharacterCamera : MonoBehaviour
{
    [Header("References")]
    public Transform Target;
    public CharacterPhysics Char;
    public CharacterUiReferences CharUiRefs;
    public Camera Cam;
    public Transform Proxy;
    public static CharacterCamera Main;
    public OptionsMenu Options;
    public InputActionReference RightAnalog;

    [Header("Parameters")]
    public int Mode = 0;
    public LayerMask Mask;
    public float Distance = 6;
    public AnimationCurve DistanceOffset;
    public float CamX_Sensitivity = 10;
    public float CamY_Sensitivity = 5;
    public float Fov = 65;
    public float TopClamp = 90;
    public float BottonClamp = -90;
    public float Rayhitoffset = 0.1f;
    public float NormalCameraSmoothing = 20;
    public AnimationCurve CameraSmoothingCurve;
    public float KeyboardMultiplier = 0.1f;
    public float MinAngle = 0.7f;
    public float AngleSpeed = 5;

    [Header("Combat Cam Params")]
    public TargetData EnemyTarget;
    public AnimationCurve PlayerVsTargetFocus;
    public float CombatCameraSmoothing = 5;
    Vector3 CombatCenter;
    public float TargetDistance;
    float lerpedTgtDist;

    [Header("Shake Parameters")]
    public float ShakeMaxAmplitude = 0.1f;
    public float ShakeDecay = 10;
    public float ShakeFreqency = 10;
    public float ShakeAmplitude = 0f;
    public AnimationCurve ShakeDistanceDecay;
    float Shake = 0;
    Vector2 perlin;

    [Header("Time Controller")]
    public float SlowDownMinDistance = 20;
    [HideInInspector] public float SlowDownRestoreTime = 10;
    [HideInInspector] public float SlowDownDuration = 1;
    public float SlowDownCounter = 0;
    public float timeScale;

    [Header("Cache")]
    //Rewired.Player Inp;
    public Vector3 inp = Vector3.zero;
    public Vector3 dir = Vector3.one;
    RaycastHit hit = new RaycastHit();
    Quaternion lookRotation;
    Vector3 lookDirection;
    float finalDistance;
    float finalsmooth;
    int PreviousMode = -1;
    float PreviousSlopeMode = -1;
    public float SlopeLerping = 0;
    public float PreviousLerping = 0;
    public Vector3 TargetProxy;
    Vector3 shakepos;
    public float modeCounter = 0;
    float LookCounter = 0;
    float LookSpeed = 0;
    Vector3 LookAngle;
    Quaternion LookQuat;
    Transform LookTransform;
    Vector3 Look_T_Up;
    Vector3 Look_T_Forward;
    public float LookHeightOffset;
    float d1;
    float d2;
    bool ChangeLookHeight = false;
    Quaternion s1;
    Quaternion s2;
    Vector2 caminp;

    private void Start() 
    {
        Main = this;
        //Inp = Rewired.ReInput.players.GetPlayer(0);
        PreviousLerping = NormalCameraSmoothing;
        Options.LoadSettings();
        Options.SetSettings();
    }

    void LateUpdate()
    {
        // SETUP
        Cam.fieldOfView = Fov;
        if (InputDevice.CurrentControllerType == InputDevice.Device.Keyboard)
        {
            caminp = RightAnalog.action.ReadValue<Vector2>();
            Vector3 v = new Vector3(caminp.x, 0, caminp.y);

            if (Mathf.Abs(v.x) > 0.1f)
            {
                inp.x += v.x * CamX_Sensitivity * KeyboardMultiplier * OptionsMenu.CurrentSettings.CamX_Sens * Time.deltaTime;
            }
            if (Mathf.Abs(v.z) > 0.1f)
            {
                inp.y += v.z * -CamY_Sensitivity * KeyboardMultiplier * OptionsMenu.CurrentSettings.CamY_Sens * Time.deltaTime;
            }
        }
        else
        {
            caminp = RightAnalog.action.ReadValue<Vector2>();
            Vector3 v = new Vector3(caminp.x, 0, caminp.y);

            if (Mathf.Abs(v.x) > 0.1f)
            {
                inp.x += v.x * CamX_Sensitivity * OptionsMenu.CurrentSettings.CamX_Sens * Time.deltaTime;
            }
            if (Mathf.Abs(v.z) > 0.1f)
            {
                inp.y += v.z * -CamY_Sensitivity * OptionsMenu.CurrentSettings.CamY_Sens * Time.deltaTime;
            }
        }

        // CLAMP
        inp.y = Mathf.Clamp(inp.y, BottonClamp, TopClamp);

        // MODE SWITCH WARNER
        if(PreviousMode != Mode)
        {
            if(PreviousMode == 0) { PreviousLerping = NormalCameraSmoothing; }
            else if(PreviousMode == 1) { PreviousLerping = CombatCameraSmoothing; }
            PreviousMode = Mode;
            modeCounter = 0;
        }

        if (Mode == 0) // NORMAL
        {
            // SET DISTANCE
            finalDistance = Distance * DistanceOffset.Evaluate(inp.y);
            if (Physics.SphereCast(Target.position, 0.1f, -Target.forward, out hit, finalDistance + Rayhitoffset, Mask))
            {
                if (hit.distance < 0.1f) { finalDistance = hit.distance + Rayhitoffset; }
                else { finalDistance = hit.distance - Rayhitoffset; }
            }

            finalsmooth = Mathf.Lerp(PreviousLerping, (NormalCameraSmoothing * CameraSmoothingCurve.Evaluate(Char.SpeedMagnitude)), modeCounter);
            modeCounter += Time.deltaTime;

            // CAMERA MOVEMENT
            Target.localEulerAngles = new Vector3(inp.y, inp.x, 0);
            TargetProxy = Vector3.Lerp(TargetProxy, Target.position, Time.deltaTime * finalsmooth);
            transform.position = TargetProxy - (Target.forward * finalDistance);

            // ROTATION
            s1 = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(TargetProxy - transform.position, Target.up), Time.deltaTime * finalsmooth);

            transform.rotation = s1;

            // ENEMY STUFF
            if (EnemyTarget != null)
            {
                TargetDistance = Vector3.Distance(transform.position, EnemyTarget.transform.position) + EnemyTarget.TargetRadius;
                lerpedTgtDist = Mathf.Lerp(lerpedTgtDist, TargetDistance, Time.deltaTime * 20);
                CombatCenter = Vector3.Lerp(Target.position, EnemyTarget.transform.position, PlayerVsTargetFocus.Evaluate(lerpedTgtDist));
            }
        } 
        else if (Mode == 1) // COMBAT CAM
        {
            finalsmooth = Mathf.Lerp(PreviousLerping, (CombatCameraSmoothing), modeCounter);
            modeCounter += Time.deltaTime;

            // SET MIDDLE
            if (EnemyTarget != null)
            {
                TargetDistance = Vector3.Distance(transform.position, EnemyTarget.transform.position) + EnemyTarget.TargetRadius;
                lerpedTgtDist = Mathf.Lerp(lerpedTgtDist, TargetDistance, Time.deltaTime * finalsmooth);
                CombatCenter = Vector3.Lerp(Target.position, EnemyTarget.transform.position, PlayerVsTargetFocus.Evaluate(lerpedTgtDist));
            }
            else
            {
                CombatCenter = transform.position;
            }

            // SET DISTANCE FROM CENTER
            finalDistance = Distance * DistanceOffset.Evaluate(inp.y);
            if (Physics.SphereCast(CombatCenter, 0.01f, -Target.forward, out hit, finalDistance + Rayhitoffset, Mask))
            {
                if (hit.distance < 0.1f) { finalDistance = hit.distance + Rayhitoffset; }
                else { finalDistance = hit.distance - Rayhitoffset; }
            }

            // CAMERA MOVEMENT
            Target.localEulerAngles = new Vector3(inp.y, inp.x, 0);
            TargetProxy = Vector3.Lerp(TargetProxy, CombatCenter, Time.deltaTime * finalsmooth);
            transform.position = TargetProxy - (Target.forward * finalDistance);
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(TargetProxy - transform.position, -Char.GravityDir), Time.deltaTime * finalsmooth * 9999);

            Debug.DrawRay(TargetProxy, Vector3.up);
            Debug.DrawRay(CombatCenter, Vector3.up);
        }

        // SET PROXY
        Proxy.parent = null;
        Proxy.rotation = transform.rotation;
        Proxy.rotation = Quaternion.FromToRotation(transform.up, Char.transform.up) * Proxy.rotation;


        // LOOK AT DIRECTION FUNCTION
        if (LookCounter > 0.0f)
        {
            LookCounter -= Time.deltaTime;
            d1 = Vector3.Dot(transform.right, Look_T_Forward);
            if (ChangeLookHeight) { d2 = Vector3.Dot(transform.forward, Look_T_Up) + LookHeightOffset; }

            inp.x += d1 * LookSpeed * Time.deltaTime;
            inp.y += d2 * LookSpeed * Time.deltaTime;

            //if(new Vector2(Inp.GetAxis("RightAnalogX"), Inp.GetAxis("RightAnalogY")).magnitude > 0.2f) { LookCounter = 0; }
        }
        else
        {
            d1 = 0;
            d2 = 0;
            LookTransform = null;
            Look_T_Up = Vector3.zero;
            Look_T_Forward = Vector3.zero;
            LookHeightOffset = 0;
            LookSpeed = 0;
            ChangeLookHeight = false;
        }

        // MORE MODIFIERS
        CameraShaker();
        TimeController();
        PlayerHitFx.ComboAudioManager();

        // FX
        FXchecks();

        void FXchecks()
        {
            if (CharUiRefs.HurtRed.color.a > 0.05f)
            {
                CharUiRefs.HurtRed.color = Color.Lerp(CharUiRefs.HurtRed.color, CharUiRefs.HurtUiColorEnd, Time.deltaTime * 5);
            }
            else
            {
                CharUiRefs.HurtRed.color = CharUiRefs.HurtUiColorEnd;
            }

            if (CharUiRefs.StrikeCounter > 0.0f)
            {
                CharUiRefs.StrikeCounter -= Time.fixedDeltaTime;
                CharUiRefs.StrikeImage.enabled = true;
            }
            else
            {
                if (CharUiRefs.StrikeImage.enabled) { CharUiRefs.StrikeImage.enabled = false; }
            }
        }
    }

    void CameraShaker()
    {
        // DECAY
        if (Shake > 0.0f)
        {
            // SHAKE
            Shake -= Time.unscaledDeltaTime * ShakeDecay;
            perlin.x = Mathf.PerlinNoise((Time.unscaledTime + 01) * ShakeFreqency, (Time.unscaledTime + 10) * ShakeFreqency);
            perlin.y = Mathf.PerlinNoise((Time.unscaledTime + 100) * ShakeFreqency, (Time.unscaledTime + 100) * ShakeFreqency);

            // REMAP
            perlin.x = (perlin.x - 0.5f) * 2;
            perlin.y = (perlin.y - 0.5f) * 2;

            // ACTAULLY SHAKE
            shakepos = transform.position;
            shakepos += (transform.up    * (perlin.x * ShakeAmplitude)) * Mathf.Clamp(Shake, 0, 1);
            shakepos += (transform.right * (perlin.y * ShakeAmplitude)) * Mathf.Clamp(Shake, 0, 1);
            transform.position = shakepos;
        }
        else
        {
            Shake = 0;
            ShakeAmplitude = 0;
        }
    }

    void TimeController()
    {
        if(Time.timeScale > 0.0f && Time.timeScale < 1.0f)
        {
            if(SlowDownCounter > 0.0f)
            {
                // KEEP SLOWED DOWN
                SlowDownCounter -= Time.unscaledDeltaTime;
                Time.timeScale = timeScale;
            }
            else
            {
                // START RESTORING TIME
                if(timeScale < 1.0f)
                {
                    timeScale += Time.unscaledDeltaTime * SlowDownRestoreTime;
                    timeScale = Mathf.Clamp01(timeScale); // CLAMP SO IT NEVER GETS STUCK ABOVE 1
                    Time.timeScale = timeScale;
                }
                else
                {
                    timeScale = 1;
                    Time.timeScale = 1;
                }
            }
        }
    }

    public void CameraFX(int type, float intensity)
    {
        // HURT RED
        if (type == 0)
        {
            CharUiRefs.HurtRed.color = CharUiRefs.HurtUiColorInitial;
        }
        else if(type == 1) // STRIKE
        {
            CharUiRefs.StrikeCounter = intensity;
        }
    }

    public static void ShakeCameraAddtive(float duration, float amplitude, Vector3 origin)
    {
        if(Main != null)
        {
            float d = Main.ShakeDistanceDecay.Evaluate(Vector3.Distance(Main.transform.position, origin));
            Main.Shake += duration * d;
            Main.ShakeAmplitude = amplitude /* d*/;
            Main.ShakeAmplitude = Mathf.Clamp(Main.ShakeAmplitude, 0, Main.ShakeMaxAmplitude);
        }
    }

    public static void ShakeCamera(float duration, float amplitude, Vector3 origin)
    {
        if (Main != null)
        {
            float d = Main.ShakeDistanceDecay.Evaluate(Vector3.Distance(Main.transform.position, origin));
            Main.Shake = duration * d;
            Main.ShakeAmplitude = amplitude /* d*/;
            Main.ShakeAmplitude = Mathf.Clamp(Main.ShakeAmplitude, 0, Main.ShakeMaxAmplitude);
        }
    }

    public static void SlowDown(float duration, float slowdownAmmount, float restoreTime, Vector3 origin)
    {
        if (Main != null && slowdownAmmount > 0.0f && Time.timeScale > 0.0f)
        {
            float d = Vector3.Distance(Main.transform.position, origin);
            if (d > Main.SlowDownMinDistance) { return; }

            Main.SlowDownCounter = duration;
            Main.SlowDownDuration = duration;
            Main.SlowDownRestoreTime = restoreTime;
            Main.timeScale = Mathf.Clamp(slowdownAmmount, 0.001f, 1);
            Time.timeScale = Main.timeScale;
        }
    }

    // EXTRA FUNCTIONS

    public void LookAtAngle(Vector3 euler, float time, float speed, Quaternion quat, Transform transf, float heightoffset, Vector3 up, Vector3 forward)
    {
        LookSpeed = speed;
        LookCounter = time;
        if (euler != null)  { LookAngle = euler; }
        if (quat != null)   { LookQuat = quat; }
        if(transf == null)
        {
            Look_T_Up = up;
            Look_T_Forward = forward;
        }
        else
        {
            LookTransform = transf;
            Look_T_Up = transf.up;
            Look_T_Forward = transf.forward;
        }

        //CHANGE HEIGHT OR BYPASS
        if (heightoffset >= -900) 
        { 
            LookHeightOffset = heightoffset;
            ChangeLookHeight = true;
        }
        else 
        {
            LookHeightOffset = 0;
            ChangeLookHeight = false;
        }
    }

}

[System.Serializable]
public class SimpleCameraFxParams
{
    public float ShakeDuration = 1;
    public float ShakeAmplitude = 0.11f;
    public float SlowDownDuration = 0;
    public float SlowDownTime = 0.05f;
    public float SlowDownRestoreSpeed = 999;
}
