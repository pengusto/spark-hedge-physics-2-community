using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DialogItens : MonoBehaviour
{
    public enum ConvoAction { None, SnapToPoint, Pathfind }
    public enum CameraType { None, Cut, Pan, Smooth }

    [Header("Initial")]
    public float TimeToDialog = 1;
    public float TimeToCamera = 0.1f;
    public Dialog dialog;
    public List<CharacterInput> Participants = new List<CharacterInput>();
    public List<TargetData> ParticipantsTarget = new List<TargetData>();
    public List<ConvoAction> ParticipantActions = new List<ConvoAction>();
    public List<CameraActions> Cam = new List<CameraActions>();
    public List<bool> RotateOnStart = new List<bool>();

    [Header("Movement Action")]
    public List<Transform> Positions = new List<Transform>();
    public List<bool> SetPositionsToParticipantPos = new List<bool>();
    public List<float> MoveSpeed = new List<float>();
    public List<float> MaxSpeed = new List<float>();
    public List<float> PathfindDistance = new List<float>();
    public List<float> MoveTime = new List<float>();

    [Header("Animate Action")]
    public int Animtype = 0;

    [Header("Audio")]
    public AudioSource CutsceneSFX;

    // CLASSES
    [System.Serializable]
    public class CameraActions
    {
        public DialogItens.CameraType Type;
        public Transform CamPos;
        public Transform FinalCamPos;
        public float Speed;
        public float t;
    }

}
