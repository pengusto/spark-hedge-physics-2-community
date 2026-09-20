using UnityEngine;

public class ToonEyeTracker : MonoBehaviour
{
    public Material Pupil;
    public Material EyeMask;
    public Material Mouth;
    public Vector3 Multiplier = Vector3.one;
    public Transform PupilBone;
    public Transform EyeMaskBone;
    public Transform MouthBone;

    Vector3 pupil_move;
    Vector3 eyemask_move;
    Vector3 mouth_move;

    void Update()
    {
        if (PupilBone) { pupil_move = Vector3.Scale(PupilBone.localPosition, Multiplier); }
        if (EyeMaskBone) { eyemask_move = Vector3.Scale(EyeMaskBone.localPosition, Multiplier); }
        if (MouthBone) { mouth_move = Vector3.Scale(MouthBone.localPosition, Multiplier); }

        if (Pupil != null) { Pupil.SetVector("_PupilMap", pupil_move); }
        if (EyeMask != null) { EyeMask.SetVector("_MaskMap", eyemask_move); }
        if (Mouth != null) { Mouth.SetVector("_MaskMap", mouth_move); }
    }
}
