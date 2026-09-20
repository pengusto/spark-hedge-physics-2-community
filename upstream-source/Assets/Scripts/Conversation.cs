using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation : MonoBehaviour
{
    [Header("References")]
    TextBox box;
    public int Index = 0;
    public int DiaIndex = 0;
    public List<DialogItens> DiaItens = new List<DialogItens>();
    public bool DisableOnStart = true;

    [Header("Cache")]
    public bool Active = false;
    DialogItens.CameraActions c;
    public CharacterCamera CharCam;
    DialogItens.ConvoAction CurrentAction = DialogItens.ConvoAction.None;
    bool PathFound = false;
    float t;
    float t_line_rotate;
    float t_initial_rotate;
    float t_Pathfind;
    int LastLineIndex = -99;
    int PathfindingActorsReady = 0;
    bool doneChoiceAnim = false;
    Dialog.Line CurrentLine;
    Dialog.Choice Choice;
    CharacterInput Participant;
    float clampIndex = 0;

    private void Start()
    {
        t_initial_rotate = 1;
        if (DisableOnStart) { this.enabled = false; }
    }

    void Update()
    {
        // HOW TO ACTIVATE BELOW
        //if (Input.GetKeyDown(KeyCode.L)) { this.enabled = true; Active = true; }

        // INITIAL SETUP
        if(CharCam == null) { CharCam = CharacterCamera.Main; }

        // DO
        if (Active)
        {
            // GET BOX AND DO SCRIPT
            if (box != null)
            {
                if (t > 0.0f)
                {
                    // INITIALIZE CHARACTER ACTIONS
                    if(t < 0.05f) 
                    {
                        // CHARACTER SETUP
                        t = 0.06f;
                        for (int i = 0; i < DiaItens[Index].Participants.Count; i++)
                        {
                            // STOP PLAYER MOVING
                            DiaItens[Index].Participants[i].PlayerCheckInput = false;
                            if (DiaItens[Index].Participants[i].Char != null) { DiaItens[Index].Participants[i].Char.rigid.linearVelocity = Vector3.zero; }
                            DiaItens[Index].Participants[i].CheckForTargets = false;

                            // INITIAL TARGET
                            if(i < DiaItens[Index].ParticipantsTarget.Count)
                            { DiaItens[Index].Participants[i].CurrentTarget = DiaItens[Index].ParticipantsTarget[i]; }

                            // DO ACTION
                            if (DiaItens[Index].ParticipantActions[i] == DialogItens.ConvoAction.SnapToPoint)
                            {
                                DiaItens[Index].Participants[i].Actions.SwitchAction(-99);
                                ResetAnimations(DiaItens[Index].Participants[i], true);

                                Vector3 xypos = Vector3.ProjectOnPlane(DiaItens[Index].Positions[i].position, DiaItens[Index].Participants[i].Char.GravityDir);
                                xypos.y = DiaItens[Index].Participants[i].transform.position.y;
                                DiaItens[Index].Participants[i].transform.position = xypos;

                                //DiaItens[Index].Participants[i].anim.transform.rotation = DiaItens[Index].Positions[i].rotation;
                                CurrentAction = DialogItens.ConvoAction.SnapToPoint;
                            }
                            else if (DiaItens[Index].ParticipantActions[i] == DialogItens.ConvoAction.Pathfind)
                            {
                                CurrentAction = DialogItens.ConvoAction.Pathfind;
                                Active = false;
                                PathFound = false;
                            }
                            else if(DiaItens[Index].ParticipantActions[i] == DialogItens.ConvoAction.None)
                            {
                                DiaItens[Index].Participants[i].Actions.SwitchAction(-99);
                                ResetAnimations(DiaItens[Index].Participants[i], true);

                                CurrentAction = DialogItens.ConvoAction.None;
                            }

                            if (Index < DiaItens[Index].Positions.Count)
                            {
                                // TURN ON ITENS
                                DiaItens[Index].Positions[i].gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        t += Time.deltaTime;
                    }

                    // START BOX
                    if(t > DiaItens[Index].TimeToDialog && box.gameObject.activeSelf == false)
                    {
                        DiaItens[Index].dialog.Scripts[DiaIndex].LineIndex = 0;
                        box.StartWritter(DiaItens[Index].dialog, DiaItens[Index].dialog.Scripts[DiaIndex], false);
                    }

                    // INSIDE ACTIONS
                    if(t > DiaItens[Index].TimeToCamera)
                    {
                        CameraActions();
                    }

                    // CHANGED LINE CHECK
                    if(box.Active && box.CurrentScript != null && box.CurrentScript.Lines.Count > 0 && LastLineIndex != box.CurrentScript.LineIndex)
                    {
                        // GET CURRENT LINE
                        if (box.CurrentScript.LineIndex < box.CurrentScript.Lines.Count 
                            && box.CurrentScript.Lines[box.CurrentScript.LineIndex] != null)
                        {
                            CurrentLine = box.CurrentScript.Lines[box.CurrentScript.LineIndex];

                            // FLAG FUNCTIONS FOR LINE
                            if (CurrentLine.FlagChanges != null)
                            {
                                for (int i = 0; i < CurrentLine.FlagChanges.Length; i++)
                                {
                                    Debug.Log("Loading Line Flag Function " +
                                        "(If this is in the first line it will cause issues) !!! >> (" + CurrentLine.Text + ")");
                                    Dialog.FlagFunction.DoFlagAction(CurrentLine.FlagChanges[i], null);
                                }
                            }
                        }

                        // DO (this part only executes once due to LastLineIndex 
                        // (not true, happens TWICE, set values to half)
                        // (Seems to execute again when lines are over)
                        if (CurrentLine != null)
                        {
                            // FORCE ANIMATION
                            for (int i = 0; i < CurrentLine.Anim.Length; i++)
                            {
                                Participant = DiaItens[Index].Participants[i];
                                Participant.anim.SetTrigger("cut" + CurrentLine.Anim[i].ToString());
                            }

                            // FORCE TO IDLE IF NO ANIMS FOUND
                            if (CurrentLine.Anim.Length <= 0)
                            {
                                for (int i = 0; i < DiaItens[Index].Participants.Count; i++)
                                { ResetAnimations(DiaItens[Index].Participants[i], false); }
                            }


                            // PLAY SFX IF EXISTS
                            if (CurrentLine.LineSFX != null && DiaItens[Index].dialog.Done == false)
                            {
                                if (DiaItens[Index].CutsceneSFX != null)
                                {
                                    DiaItens[Index].CutsceneSFX.clip = CurrentLine.LineSFX;
                                    DiaItens[Index].CutsceneSFX.Play();
                                }
                            }

                            // DO LINE ACTION
                            if (CurrentLine.Actions != null)
                            { 
                                Dialog.LineAction.DoLineAction(CurrentLine.Actions);
                            }

                            // FINAL LINE SETS
                            t_line_rotate = 1;
                            LastLineIndex = box.CurrentScript.LineIndex;
                            Debug.Log("NEW LINE ALERT");

                        }
                    }

                    // CHOICE SELECTED
                    if (box.Active && box.MenuMode == 2 && box.CurrentScript != null)
                    {
                        if (doneChoiceAnim == false)
                        {
                            doneChoiceAnim = true;
                            Choice = box.CurrentScript.Choices[box.ChoiceIndex];
                            for (int i = 0; i < Choice.Anim.Length; i++)
                            {
                                Participant = DiaItens[Index].Participants[i];
                                Participant.anim.SetTrigger("cut" + Choice.Anim[i].ToString());
                            }

                            // ACTIVATE OBJECT
                            for (int i = 0; i < Choice.ActivateOnChoice.Length; i++)
                            {
                                Choice.ActivateOnChoice[i].SetActive(true);
                            }
                        }
                    }
                    else if (box.MenuMode != 2)
                    {
                        doneChoiceAnim = false;
                    }

                    // ROTATION INITIAL
                    if(t_initial_rotate < 0 && CurrentLine != null)
                    {
                        for (int i = 0; i < DiaItens[Index].Participants.Count; i++)
                        {
                            if (DiaItens[Index].Participants[i] != null)
                            {
                                // DIRECTION FIX
                                if(DiaItens[Index].SetPositionsToParticipantPos[i] == true) 
                                { DiaItens[Index].Positions[i].position = DiaItens[Index].Participants[i].transform.position; }

                                Participant = DiaItens[Index].Participants[i];
                                Participant.anim.transform.rotation = Quaternion.Slerp(Participant.anim.transform.rotation,
                                DiaItens[Index].Positions[i].rotation, Time.deltaTime * 10);
                            }
                        }
                        t_initial_rotate -= Time.deltaTime;
                        //Debug.Log("ROTATE INITIAL");
                    }

                    // ROTATION ON CHANGE LINE
                    if (t_line_rotate > 0 && CurrentLine != null)
                    {
                        // LINE ROTATION
                        t_line_rotate -= Time.deltaTime;
                        for (int i = 0; i < CurrentLine.RotateTowardsTarget.Length; i++)
                        {
                            if (CurrentLine.RotateTowardsTarget[i])
                            {
                                if (DiaItens[Index].Participants[i] != null)
                                {
                                    // DIRECTION FIX
                                    if (DiaItens[Index].SetPositionsToParticipantPos[i] == true)
                                    { 
                                        DiaItens[Index].Positions[i].position = DiaItens[Index].Participants[i].transform.position;
                                    }

                                    if (Index < DiaItens[Index].Positions.Count)
                                    {
                                        Participant = DiaItens[Index].Participants[i];
                                        Participant.anim.transform.rotation = Quaternion.Slerp(Participant.anim.transform.rotation,
                                        DiaItens[Index].Positions[i].rotation, Time.deltaTime * 10);
                                    }
                                }
                            }
                        }
                        //Debug.Log("ROTATE CUT");
                    }

                    // NEXT BOX OR END
                    if (DiaItens[Index].dialog.Done == true)
                    {
                        Index++;

                        // END
                        if (Index >= DiaItens.Count)
                        {
                            // RESTORE PLAYER AND END SEQUENCE
                            for (int i = 0; i < DiaItens[Index - 1].Participants.Count; i++)
                            {
                                if (DiaItens[Index - 1].Participants[i].Char != null)
                                { DiaItens[Index - 1].Participants[i].Char.rigid.linearVelocity = Vector3.zero; }
                                DiaItens[Index - 1].Participants[i].CheckForTargets = true;

                                if (DiaItens[Index - 1].Participants[i].Player)
                                {
                                    DiaItens[Index - 1].Participants[i].Actions.SwitchAction(0);
                                    DiaItens[Index - 1].Participants[i].PlayerCheckInput = true;
                                    DiaItens[Index - 1].Participants[i].CheckForTargets = false;
                                }
                                
                                // TURN OFF ITENS
                                if(Index - 1 < DiaItens[Index - 1].Positions.Count)
                                {
                                    DiaItens[Index - 1].Positions[i].gameObject.SetActive(false);
                                }
                            }

                            // RESTORE DIALOG
                            for (int i = 0; i < DiaItens[Index - 1].dialog.Scripts.Count; i++)
                            { DiaItens[Index - 1].dialog.Scripts[i].LineIndex = 0; }
                            DiaItens[Index - 1].dialog.Done = false;

                            // RESTORE CAM DATA
                            for (int i = 0; i < DiaItens[Index - 1].Cam.Count; i++)
                            {
                                DiaItens[Index - 1].Cam[i].t = 0;
                            }

                            // RESTORE VARIABLES
                            t = 0;
                            t_Pathfind = 0;
                            t_line_rotate = 0;
                            t_initial_rotate = 0;
                            Index = 0;
                            Active = false;
                            box.StopWritter();
                            CurrentLine = null;
                            Participant = null;
                            PathFound = false;
                            LastLineIndex = -99;
                            t_initial_rotate = 1;
                            if (CutsceneRepo.ConvoScene == false) { CharCam.enabled = true; }
                            CharCam.modeCounter = 0;
                            //CharCam.PreviousLerping = 0;
                            CharCam.TargetProxy = CharCam.transform.position;
                            CurrentAction = DialogItens.ConvoAction.None;


                        }
                        // NEXT
                        else
                        {
                            // RESTORE VARIABLES
                            t = 0;
                            t_Pathfind = 0;
                            t_line_rotate = 0;
                            t_initial_rotate = 0;
                            box.StopWritter();
                            CurrentLine = null;
                            Participant = null;
                            PathFound = false;
                            LastLineIndex = -99;
                            t_initial_rotate = 1;
                            if (CutsceneRepo.ConvoScene == false) { CharCam.enabled = true; }
                            CharCam.modeCounter = 0;
                            //CharCam.PreviousLerping = 0;
                            CharCam.TargetProxy = CharCam.transform.position;
                            CurrentAction = DialogItens.ConvoAction.None;

                            // PREPARE PLAYER
                            for (int i = 0; i < DiaItens[Index - 1].Participants.Count; i++)
                            {
                                DiaItens[Index - 1].Participants[i].Char.rigid.linearVelocity = Vector3.zero;
                                DiaItens[Index - 1].Participants[i].CheckForTargets = false;

                                if (DiaItens[Index - 1].Participants[i].Player)
                                {
                                    DiaItens[Index - 1].Participants[i].Actions.SwitchAction(0);
                                    DiaItens[Index - 1].Participants[i].PlayerCheckInput = false;
                                    DiaItens[Index - 1].Participants[i].CheckForTargets = false;
                                }

                                // TURN OFF ITENS
                                DiaItens[Index - 1].Positions[i].gameObject.SetActive(false);
                            }

                            // PREPARE DIALOG
                            for (int i = 0; i < DiaItens[Index - 1].dialog.Scripts.Count; i++)
                            { DiaItens[Index - 1].dialog.Scripts[i].LineIndex = 0; }
                            DiaItens[Index - 1].dialog.Done = false;

                            // RESTORE CAM DATA
                            for (int i = 0; i < DiaItens[Index - 1].Cam.Count; i++)
                            { DiaItens[Index - 1].Cam[i].t = 0; }

                            // NEXT
                            Active = true;
                        }
                    }
                }
                else
                {
                    t += Time.deltaTime;
                }
            }
            // GET BOX
            else
            {
                if (TextBox.Instance != null) { box = TextBox.Instance; }
            }
        }

        // PATHFIND
        if (CurrentAction == DialogItens.ConvoAction.Pathfind)
        {
            CameraActions();

            // SETUP
            if(t_Pathfind <= 0.0f)
            {
                t_Pathfind = 0.05f;
                for (int i = 0; i < DiaItens[Index].Participants.Count; i++)
                {
                    if (DiaItens[Index].Participants[i] != null)
                    {
                        Participant = DiaItens[Index].Participants[i];
                        StartCoroutine(StartMovementCoroutine(DiaItens[Index].MoveTime[i], Participant, i));
                    }
                }
            }
            else if(t_Pathfind > 0.05f)
            {
                // CHECK DISTANCE AND STOP (then rotate)
                PathfindingActorsReady = 0;
                if (t_Pathfind < 10)
                {
                    for (int i = 0; i < DiaItens[Index].Participants.Count; i++)
                    {
                        // DISTANCE
                        if (DiaItens[Index].Participants[i] != null)
                        {
                            Participant = DiaItens[Index].Participants[i];
                            if (Vector3.Distance(Participant.transform.position, DiaItens[Index].Positions[i].position)
                                < DiaItens[Index].PathfindDistance[i])
                            {
                                Participant.ai.UseCustomTarget = false;
                                Participant.LeftAnalogInput = Vector3.zero;
                                Participant.CamRelativeInput = Vector3.zero;
                                Participant.ai.dir = Vector3.zero;
                                Participant.ai.projecteddir = Vector3.zero;
                                Participant.ai.AI_Action = -2;
                                Participant.ai.AI_Subaction = 0;
                                Participant.Char.rigid.linearVelocity = Vector3.Lerp(Participant.Char.rigid.linearVelocity, Vector3.zero, Time.deltaTime * 10);
                                PathfindingActorsReady++;

                                // ROTATE & POS
                                if (Participant.Char.SpeedMagnitude <= 0.1f) { Participant.Char.rigid.linearVelocity = Vector3.zero; }
                                Participant.Actions.Basic.prevInput = DiaItens[Index].Positions[i].forward;
                               
                                Participant.anim.transform.rotation = Quaternion.Slerp(Participant.anim.transform.rotation,
                                    DiaItens[Index].Positions[i].rotation, Time.deltaTime * 10);

                                Vector3 xypos = Vector3.ProjectOnPlane(DiaItens[Index].Positions[i].position, Participant.Char.GravityDir);
                                xypos.y = Participant.transform.position.y;
                                Participant.transform.position = Vector3.Lerp(Participant.transform.position, xypos, Time.deltaTime * 10); 
                            }
                        }

                        // GO TO TIME WHEN CHARACTERS ARE ON POSITION
                        if (PathfindingActorsReady >= DiaItens[Index].Participants.Count)
                        {
                            t_Pathfind = 14;
                        }
                    }
                }

                // TIMEOUT
                if (t_Pathfind > 15) 
                {
                    for (int i = 0; i < DiaItens[Index].Participants.Count; i++)
                    {
                        if (DiaItens[Index].Participants[i] != null)
                        {
                            Participant = DiaItens[Index].Participants[i];
                            Participant.ai.PathFinding = false;
                            Participant.ai.UseCustomTarget = false;
                            Participant.Actions.SwitchAction(-99);
                            ResetAnimations(DiaItens[Index].Participants[i], true);
                            //Participant.transform.position = DiaItens[Index].Positions[i].position;
                            //Participant.transform.rotation = DiaItens[Index].Positions[i].rotation;
                            Participant.Char.rigid.linearVelocity = Vector3.zero;
                            Participant.ai.AI_Action = -2;
                            Participant.ai.AI_Subaction = -2;
                            Participant.ai.counter = 0;
                            Participant.ai.counter2 = 0;
                            t_Pathfind = 0;
                            CharacterInput inp = CharCam.Char.GetComponent<CharacterInput>();
                            inp.Player = true;
                            CurrentAction = DialogItens.ConvoAction.SnapToPoint;
                            Active = true;
                        }
                    }
                }

                // MAX SPEED
                for (int i = 0; i < DiaItens[Index].Participants.Count; i++)
                {
                    if (DiaItens[Index].Participants[i] != null)
                    {
                        Participant = DiaItens[Index].Participants[i];
                        Participant.Char.rigid.linearVelocity = Vector3.ClampMagnitude(Participant.Char.rigid.linearVelocity, DiaItens[Index].MaxSpeed[i]);
                    }
                }

                // FINAL SET
                t_Pathfind += Time.deltaTime;
            }
            else
            {
                t_Pathfind += Time.deltaTime;
            }
        }
    }

    void CameraActions()
    {
        // CAMERA ACTIONS
        c = DiaItens[Index].Cam[0];
        if (c != null)
        {
            if (c.Type == DialogItens.CameraType.None)
            {
                CharCam.enabled = true;
            }
            else if (c.Type == DialogItens.CameraType.Cut)
            {
                CharCam.enabled = false;
                CharCam.transform.position = c.CamPos.position;
                CharCam.transform.rotation = c.CamPos.rotation;
                c.t = 0;
            }
            else if (c.Type == DialogItens.CameraType.Pan)
            {
                CharCam.enabled = false;
                CharCam.transform.position = Vector3.Lerp(c.CamPos.position, c.FinalCamPos.position, c.t);
                CharCam.transform.rotation = Quaternion.Slerp(c.CamPos.rotation, c.FinalCamPos.rotation, c.t);
                c.t += Time.deltaTime * c.Speed;
            }
            else if (c.Type == DialogItens.CameraType.Smooth)
            {
                CharCam.enabled = false;
                CharCam.transform.position = Vector3.Lerp(CharCam.transform.position, c.FinalCamPos.position, c.Speed);
                CharCam.transform.rotation = Quaternion.Slerp(CharCam.transform.rotation, c.FinalCamPos.rotation, c.Speed);
                c.t = 0;
            }
        }

    }

    void ResetAnimations(CharacterInput c, bool initial)
    {
        c.anim.SetInteger("Action", -999);
        c.anim.ResetTrigger("Dash");
        c.anim.ResetTrigger("Wall");
        c.anim.ResetTrigger("WallJump");
        c.anim.SetBool("Skid", false);
        c.anim.SetBool("Sliding", false);
        if (initial) { c.anim.SetTrigger("cut0"); }
    }

    IEnumerator StartMovementCoroutine(float time, CharacterInput participant, int i)
    {
        yield return new WaitForSeconds(time);
        participant.Actions.SwitchAction(0);
        participant.ai.PathFinding = true;
        participant.ai.UseCustomTarget = true;
        participant.ai.Target = DiaItens[Index].Positions[i];
        participant.Player = false;
        participant.ai.Agressive = false;
        participant.ai.AI_Action = 0;
        participant.ai.AI_Subaction = 0;
        participant.ai.counter = 1;
        participant.ai.counter2 = 0;
        participant.ai.PathMinDistanceToWaypoint = 0;
        participant.ai.PathfindingMoveSpeed = DiaItens[Index].MoveSpeed[i];
        participant.ai.NormalTargetDistance = 0;
    }

}
