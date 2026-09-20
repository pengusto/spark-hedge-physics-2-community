//using Rewired;
using System;
using UnityEngine;

public class CharacterSpeedEffects : MonoBehaviour
{
    [Header("References")]
    public CharacterPhysics Phys;
    public CharacterActions Actions;

    [Header("Back Speedlines")]
    public ParticleSystem BackSpeedLinesParticle;
    public AnimationCurve BackSpeedlineEmissionCurve;

    [Header("Wind Sound")]
    public AudioSource WindSound;
    public AnimationCurve WindSoundVolume;
    public AnimationCurve WindSoundPitch;
    public float WindSoundSmoothness = 10;

    [Header("Camera Up In Medium Speed")]
    public bool MediumSpeedCamHeightChange = false;
    public float MediumSpeedThreshold = 5;
    public float MediumSpeedHeight = 20;
    public float MediumSpeedCamSpeed = 1;

    [Header("Camera Rotation In Medium Speed")]
    public bool CamRotEnabled = false;
    public float CamRotSpeedThreshold = 5;
    public float CamRotPower = 1;

    [Header("Camera FX With Speed")]
    public bool EnableCamFX = true;
    public AnimationCurve CamFX_FovAddWithSpeed;
    public AnimationCurve CamFX_DistanceAddWithSpeed;
    public float CamFXSmoothnessSpeed = 3;

    [Header("Cache")]
    public int spdLineEmit = 0;
    public float windvol;
    public float windptch;
    public CharacterCamera cam;
    public float camInitialFov;
    public float camInitialDistance;
    public float camfov;
    public float camdist;
    public float camlerp;
    public float camrotcounter;
    float finalspeed;

    void Update()
    {
        if (Actions.Inp.Player)
        {
            if(Actions.Action == 2) 
            {
                if (Actions.Rail) { finalspeed = Mathf.Abs(Actions.Rail.RailSpeed); }
                else { finalspeed = Phys.SpeedMagnitude; }
            }
            else
            {
                finalspeed = Phys.SpeedMagnitude;
            }

            // HIGH SPEED EFFECTS
            if (finalspeed > 5)
            {
                spdLineEmit = Mathf.FloorToInt(BackSpeedlineEmissionCurve.Evaluate(finalspeed) * (Time.deltaTime * 60));
                BackSpeedLinesParticle.Emit(spdLineEmit);

                windvol = Mathf.Lerp(windvol, WindSoundVolume.Evaluate(finalspeed), Time.deltaTime * WindSoundSmoothness);
                windptch = Mathf.Lerp(windptch, WindSoundPitch.Evaluate(finalspeed), Time.deltaTime * WindSoundSmoothness);
                if (!WindSound.enabled) { WindSound.enabled = true; }

            }
            else
            {
                windvol = Mathf.Lerp(windvol, 0, Time.deltaTime * WindSoundSmoothness);
                windptch = Mathf.Lerp(windptch, 0, Time.deltaTime * WindSoundSmoothness);
                if (windvol <= 0.01f && WindSound.enabled) { WindSound.enabled = false; }
            }

            // WIND SOUND
            if (WindSound.enabled)
            {
                WindSound.volume = windvol;
                WindSound.pitch = windptch;
            }

            // CAM HEIGHT MED SPEED
            if (MediumSpeedCamHeightChange)
            {
                if (Phys.Grounded)
                {
                    if (finalspeed > MediumSpeedThreshold && MediumSpeedThreshold < Actions.Basic.HighSpeedThreshold)
                    {

                        Actions.Inp.CharCam.inp.y =
                            Mathf.Lerp(Actions.Inp.CharCam.inp.y, MediumSpeedHeight, Time.deltaTime * MediumSpeedCamSpeed);
                    }
                }
            }

            // ROTATE CAMERA
            if (CamRotEnabled)
            {
                if (camrotcounter < 0.0f)
                {
                    if (Phys.Grounded && finalspeed > CamRotSpeedThreshold && finalspeed < Actions.Basic.HighSpeedThreshold)
                    {
                        if (Actions.Inp.CharCam)
                        {
                            Actions.Inp.CharCam.LookAtAngle
                                (Vector3.zero, 0.05f, CamRotPower, Quaternion.identity, null,
                                -999, -Phys.GravityDir, Phys.SpeedDirection);
                        }
                    }
                }
                else
                {
                    camrotcounter -= Time.deltaTime;
                }
            }

            // CAMERA HIGH SPEED
            if (cam == null)
            {
                if (Actions.Inp.CharCam != null)
                {
                    cam = Actions.Inp.CharCam;
                    camInitialFov = cam.Fov;
                    camInitialDistance = cam.Distance;
                    camfov = camInitialFov;
                    camdist = camInitialDistance;
                }
            }
            else if (EnableCamFX)
            {
                if (finalspeed > Actions.Basic.HighSpeedThreshold)
                {
                    camlerp += Time.deltaTime;
                    camlerp = Mathf.Clamp01(camlerp);

                    camfov = Mathf.Lerp(camfov, camInitialFov + CamFX_FovAddWithSpeed.Evaluate(finalspeed),
                        CamFXSmoothnessSpeed * Time.deltaTime);
                    camdist = Mathf.Lerp(camdist, camInitialDistance + CamFX_DistanceAddWithSpeed.Evaluate(finalspeed),
                        CamFXSmoothnessSpeed * Time.deltaTime);
                    CamFX();
                }
                else
                {
                    if (camlerp >= 0.0f)
                    {
                        camlerp -= Time.deltaTime;
                        CamFX();
                    }
                }
            }
        }

        void CamFX()
        {
            cam.Fov = Mathf.SmoothStep(camInitialFov, camfov, camlerp);
            cam.Distance = Mathf.SmoothStep(camInitialDistance, camdist, camlerp);
        }


    }

}
