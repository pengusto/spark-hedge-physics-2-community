using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour
{
    public Transform Skin;
    public Transform RayStartPos;
    public LayerMask LayersToHit;
    public float RayLenght = 0.1f;
    public float LerpingSpeed = 2;
    RaycastHit hit;
    public Vector2 FootStepAudioPitchRange = new Vector2(0.95f, 1.05f);
    public AudioSource Footstep;

    Vector3 dir;
    Vector3 rot;

    public int Type;
    public Vector3 offset;
    bool audioTrigger = false;

    void LateUpdate()
    {
        if (Physics.Raycast(RayStartPos.position, -Skin.transform.up, out hit, RayLenght, LayersToHit, QueryTriggerInteraction.Ignore))
        {
            rot = hit.normal;
            if (Type == 0)      { dir = Skin.transform.forward; }
            else if (Type == 1) { dir = -Skin.transform.forward; }
            else if (Type == 2) { dir = Skin.transform.up; }
            else if (Type == 3) { dir = -Skin.transform.up; }
            else if (Type == 4) { dir = Skin.transform.right; }
            else if (Type == 5) { dir = -Skin.transform.right; }
            
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(dir, rot) * transform.rotation, Time.deltaTime * LerpingSpeed);
            if (!audioTrigger)
            {
                if (Footstep) 
                {
                    audioTrigger = true;
                    Footstep.pitch = Random.Range(FootStepAudioPitchRange.x, FootStepAudioPitchRange.y);
                    Footstep.Play(); 
                }
            }
        }
        else
        {
            audioTrigger = false;
        }

        Debug.DrawRay(RayStartPos.position, -Skin.transform.up * RayLenght);
    }
}
