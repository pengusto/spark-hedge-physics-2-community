using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [Header("References")]
    public CharacterPhysics Char;
    public CharacterActions Actions;
    public PitRespawner PitScript;
    public GameObject FallObject;
    public Transform FallTransform;
    public Animator Anim;

    [Header("Parameters")]
    public float MinFallDamageSpeed = 15;
    public float FatalFallSpeed = 30;
    public float FatalFallCounterSpeed = 0.15f;
    public float MinFallDuration = 1;
    public float FallDirectionMin = 0.2f;

    //[Header("Animation")]

    [Header("Audio")]
    public AudioSource WarningSound;

    [Header("Cache")]
    public CharacterCamera Cam;
    public float FallTime = 0;
    public float FatalFallTime = 0;
    public float FallDot;

    private void FixedUpdate()
    {
        if(Char.Grounded == false && PitScript.Resetting == false)
        {
            // CHECK IF YOU'RE ACTUALLY FALLING
            FallDot = Vector3.Dot(Char.SpeedDirection, Char.GravityDir);
            if(FallDot > FallDirectionMin && Char.SpeedMagnitude > MinFallDamageSpeed)
            {
                // ACTIVATE FALL DAMAGE OBJECT
                if (FallObject.activeSelf == false) 
                { 
                    FallObject.SetActive(true);
                    Cam = Actions.Inp.CharCam;
                }

                // UI ICON LOOK AT CAMERA
                FallTransform.rotation = Quaternion.LookRotation
                    ((Cam.transform.position - transform.position).normalized, -Char.GravityDir);

                // ADD TIMER
                FallTime += Time.fixedDeltaTime;

                // FALL LOGIC
                if(FallTime > MinFallDuration)
                {
                    // YELLOW WARNING
                    if(Char.SpeedMagnitude > MinFallDamageSpeed && Char.SpeedMagnitude < FatalFallSpeed)
                    {
                        if(WarningSound.enabled == false) { WarningSound.enabled = true; WarningSound.Play(); }
                        Anim.ResetTrigger("WarningHeavy");
                        Anim.SetTrigger("WarningYellow");
                        Anim.SetFloat("Circle", 0);
                        FatalFallTime = 0;
                    }
                    else if (Char.SpeedMagnitude > FatalFallSpeed)
                    {
                        if (WarningSound.enabled == false) { WarningSound.enabled = true; WarningSound.Play(); }
                        if (FatalFallTime < 1.0f)
                        {
                            FatalFallTime += FatalFallCounterSpeed * Time.fixedDeltaTime;
                            Anim.SetFloat("Circle", FatalFallTime);
                            Anim.ResetTrigger("WarningYellow");
                            Anim.SetTrigger("WarningHeavy");
                        }
                        else
                        {
                            PitScript.FallOnPit(null);
                        }
                    }
                }
            }
            else
            {
                ResetFall();
            }
        }
        else
        {
            ResetFall();
        }

        void ResetFall()
        {
            if (FallObject.activeSelf)
            {
                FallObject.SetActive(false);
                FallTime = 0;
                FatalFallTime = 0;
                FallDot = 0;
                WarningSound.enabled = false;
                Anim.SetTrigger("WarningYellow");
                Anim.ResetTrigger("WarningHeavy");
                Anim.SetFloat("Circle", 0);
            }
        }
    }

}
