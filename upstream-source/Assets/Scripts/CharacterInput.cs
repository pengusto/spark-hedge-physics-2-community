using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{
    [Header("Unity Input System")]
    // Note, this "engine" was originally designed to work with "Rewired", a paid asset.
    // Rewired has been removed and *roughly* replaced with Unity's new input system.
    // To note, Rewired is rather complicated and I rather you stick with unity's new system instead for this engine.
    // If you choose to buy rewired, and put it into the project, this will not work as the input profile that I made was also deleted.
    public PlayerInput UniInp;
    public InputActionReference LeftAnalog;
    public InputActionReference RightAnalog;
    public InputActionReference A;
    public InputActionReference B;
    public InputActionReference X;
    public InputActionReference Y;
    public InputActionReference R1;
    public InputActionReference R2;
    public InputActionReference L1;
    public InputActionReference L2;
    public InputActionReference R3;
    public InputActionReference L3;
    public InputActionReference START;

    [Header("References")]
    public EntityInfo CharacterFaction;
    public bool Player = false;
    public bool PlayerCheckInput = true;
    public bool InputEnabled = true;
    public bool ButtonInputsEnabled = true;
    public CharacterPhysics Char;
    public CharacterCamera CharCam;
    public List<GameObject> CarUI; 
    public CharacterActions Actions;
    public GameObject CharacterUI;
    public Transform CameraTarget;

    [Header("Targeting System")]
    public bool CheckForTargets = true;
    public float TargetsDistanceToCheck = 50;
    public List<TargetData> NearTargets;
    public TargetData CurrentTarget;
    public float NearTargetCheckRate = 0.4f;
    public bool TargetDebug = false;
    float tgtDist;
    float tgtDistNear;
    int tgtIndex;
    int lockedtgtIndex;

    [Header("Corrected Input (Xinput issue)")]
    public bool EnabledCorrectedInput = false;
    public AnimationCurve CorrectedInputCurve;

    [Header("Targeting Itens")]
    public Animator TargetReticule;
    public float TargetReticuleSize = 0.1f;
    public float TargetReticuleSizeOffset = 0.1f;
    public float WeakTargetMinDistance = 10;
    public bool LockedOn = false;
    public bool InCombat = false;
    float reticuleSize;
    Quaternion reticuleRot;
    EntityInfo tgtLastFaction = null;

    [Header("AI Parameters")]
    public AiParameters ai;
    public Animator anim;

    [Header("Cache")]
    public float InputMag;
    public Vector3 LeftAnalogInput;
    public Vector3 CamRelativeInput;
    public Vector3 RightAnalogInput;
    public Vector3 CamRelativePreviousInput = Vector3.forward;
    public Vector3 CorrectedLeftAnalogInput;
    public Vector3 CorrectedCamRelativeInput;
    //public Rewired.Player Inp;
    public bool LightAttack = false;
    public bool LightAttackHold = false;
    public bool HeavyAttack = false;
    public bool HeavyAttackHold = false;
    public bool Jump = false;
    public bool JumpHold = false;
    public bool Dash = false;
    public bool DashHold = false;
    public bool Block = false;
    public bool BlockHold = false;
    public bool RightStickPress = false;
    public bool RightStickPressHold = false;
    public bool Special = false;
    public bool SpecialHold = false;
    public bool Shot = false;
    public bool ShotHold = false;
    public bool JDash = false;
    public bool JDashHold = false;
    public float NearestTargetDistance = 0;
    public bool WeakTarget = false;
    RaycastHit h;
    int i;
    Vector2 inp;

    void Start()
    {
        Initiate();
        InvokeRepeating("GetNearTargets", Random.Range(0.01f, 0.1f), NearTargetCheckRate + Random.Range(0, 0.1f));       
    }

    private void OnEnable()
    {
        //Initiate();
    }

    void Initiate()
    {
        if (Player) // PLAYER ACTIONS AND INPUTS
        {
            if (Camera.main != null)
            {
                if (Camera.main.GetComponent<CharacterCamera>() != null)
                {
                    Camera.main.GetComponent<CharacterCamera>().Target = CameraTarget;
                    CharCam = Camera.main.GetComponent<CharacterCamera>();
                    CharCam.enabled = true;
                    CharCam.Char = Char;
                }
            }

            if (CharacterUI) 
            { 
                CharacterUI.SetActive(true);
            }

            EnemySpawner.Characters.Add(transform);
            //Inp = Rewired.ReInput.players.GetPlayer(0);
            //Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 0).enabled = false;
            //Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 1).enabled = false;
            //Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 2).enabled = true;
            

            for (int i = 0; i < CarUI.Count; i++) { CarUI[i].SetActive(false); }
            if(TargetReticule != null) { TargetReticule.transform.parent = null; }



        }
        else // A.I ACTIONS
        {
            AiParameters.Instances.Add(ai);
            AiParameters.ClearNullInstances();
            ai.Seed = Mathf.RoundToInt(transform.position.x + transform.position.y + transform.position.z) * 2;
            ai.random = new System.Random(ai.Seed);
            ai.StartPosition = transform.position;
        }
    }

    // NOTE: this used to run in "Fixed Update" as rewired had support for inputs being checked during fixed update;
    // Due to this a new method is used to fix the "pressed this frame" inconsistencies in another script that is set to execute last.
    void Update()
    {
        // PLAYER INPUT
        if (Player)
        {
            // MAIN INPUT
            if (InputEnabled)
            {
                //LeftAnalogInput = new Vector3(Inp.GetAxis("LeftAnalogX"), 0, Inp.GetAxis("LeftAnalogY"));
                //RightAnalogInput = new Vector3(Inp.GetAxis("RightAnalogX"), 0, Inp.GetAxis("RightAnalogX"));

                inp = LeftAnalog.action.ReadValue<Vector2>();
                LeftAnalogInput = new Vector3(inp.x, 0, inp.y);
                inp = RightAnalog.action.ReadValue<Vector2>();
                RightAnalogInput = new Vector3(inp.x, 0, inp.y);               
            }
            else
            {
                LeftAnalogInput = Vector3.zero;
                RightAnalogInput = Vector3.zero;
            }

            CamRelativeInput = CharCam.Proxy.TransformDirection(LeftAnalogInput);
            CamRelativeInput = Vector3.ProjectOnPlane(CamRelativeInput, transform.up);
            CamRelativeInput = Vector3.ProjectOnPlane(CamRelativeInput, -transform.up);
            InputMag = LeftAnalogInput.magnitude;
            CamRelativeInput = CamRelativeInput.normalized * InputMag;

            if(InputMag > 0.01f)
            {
                CamRelativePreviousInput = CamRelativeInput;
            }

            // CORRECTED INPUT
            if (EnabledCorrectedInput)
            {
                CorrectedLeftAnalogInput = Vector3.zero;
                //CorrectedLeftAnalogInput.x = CorrectedInputCurve.Evaluate(Inp.GetAxis("LeftAnalogX"));
                //CorrectedLeftAnalogInput.z = CorrectedInputCurve.Evaluate(Inp.GetAxis("LeftAnalogY"));
                CorrectedLeftAnalogInput.x = CorrectedInputCurve.Evaluate(LeftAnalogInput.x);
                CorrectedLeftAnalogInput.z = CorrectedInputCurve.Evaluate(LeftAnalogInput.z);
                CorrectedCamRelativeInput = CharCam.Proxy.TransformDirection(CorrectedLeftAnalogInput);
                CorrectedCamRelativeInput = Vector3.ProjectOnPlane(CorrectedCamRelativeInput, transform.up);
            }

            // BUTTONS
            if (PlayerCheckInput)
            {
                if (ButtonInputsEnabled)
                {
                    //Jump = Inp.GetButtonDown("A");
                    //JumpHold = Inp.GetButton("A");
                    //LightAttack = Inp.GetButtonDown("X");
                    //LightAttackHold = Inp.GetButton("X");
                    //HeavyAttack = Inp.GetButtonDown("Y");
                    //HeavyAttackHold = Inp.GetButton("Y");
                    //JDash = Inp.GetButtonDown("B");
                    //JDashHold = Inp.GetButton("B");
                    //Shot = Inp.GetButtonDown("R1");
                    //ShotHold = Inp.GetButton("R1");
                    //Dash = Inp.GetButtonDown("R2");
                    //DashHold = Inp.GetButton("R2");
                    //Block = Inp.GetButtonDown("L1");
                    //BlockHold = Inp.GetButton("L1");
                    //Special = Inp.GetButtonDown("L2");
                    //SpecialHold = Inp.GetButton("L2")
                    //RightStickPress = Inp.GetButtonDown("RightAnalogPress");
                    //RightStickPressHold = Inp.GetButton("RightAnalogPress");

                    if (A.action.WasPressedThisFrame()) { Jump = true; }
                    JumpHold = A.action.IsPressed();

                    if (X.action.WasPressedThisFrame()) { LightAttack = true; }
                    LightAttackHold = X.action.IsPressed();

                    if (Y.action.WasPressedThisFrame()) { HeavyAttack = true; }
                    HeavyAttackHold = Y.action.IsPressed();

                    if (B.action.WasPressedThisFrame()) { JDash = true; }
                    JDashHold = B.action.IsPressed();

                    if (R1.action.WasPressedThisFrame()) { Shot = true; }
                    ShotHold = R1.action.IsPressed();

                    if (R2.action.WasPressedThisFrame()) { Dash = true; }
                    DashHold = R2.action.IsPressed();

                    if (L1.action.WasPressedThisFrame()) { Block = true; }
                    BlockHold = L1.action.IsPressed();

                    if (L2.action.WasPressedThisFrame()) { Special = true; }
                    SpecialHold = L2.action.IsPressed();

                    if (R3.action.WasPressedThisFrame()) { RightStickPress = true; }
                    RightStickPressHold = R3.action.IsPressed();
                }
                else
                {
                    Jump = false;
                    JumpHold = false;

                    Dash = false;
                    DashHold = false;

                    LightAttack = false;
                    LightAttackHold = false;

                    HeavyAttack = false;
                    HeavyAttackHold = false;

                    Block = false;
                    BlockHold = false;

                    RightStickPress = false;
                    RightStickPressHold = false;
                }
            }
            else
            {
                Jump = false;
                JumpHold = false;

                Dash = false;
                DashHold = false;

                LightAttack = false;
                LightAttackHold = false;

                HeavyAttack = false;
                HeavyAttackHold = false;

                Block = false;
                BlockHold = false;

                RightStickPress = false;
                RightStickPressHold = false;
            }
        }
        else // AI STATE MACHINE
        {
            AiStateMachine();
        }

        // TARGET MANAGER
        if (CheckForTargets)
        {
            // CALCULATE CLOSEST
            tgtIndex = 0;
            CurrentTarget = null;
            NearestTargetDistance = 9999;
            WeakTarget = false;
            for (int i = 0; i < NearTargets.Count; i++)
            {
                if (NearTargets[i] != null && NearTargets[i].gameObject.activeInHierarchy)
                {
                    // CHECK IF SAME NAME OR FACTION
                    if (NearTargets[i].AttachedCharacter)
                    {
                        if (NearTargets[i].TargetFaction.FactionName == CharacterFaction.FactionName) 
                        { continue; }
                    }

                    // SORT
                    tgtDistNear = Vector3.Distance(transform.position, NearTargets[i].transform.position);
                    if (NearestTargetDistance > tgtDistNear)
                    {
                        NearestTargetDistance = tgtDistNear;
                        tgtIndex = i;
                    }
                }
            }

            // FINAL SET
            if (!LockedOn)
            {
                if (NearTargets.Count > 0 && NearTargets[tgtIndex] != null) 
                {
                    CurrentTarget = NearTargets[tgtIndex];
                }

                if (RightStickPress && CurrentTarget != null && CurrentTarget.IsHostile)
                {
                    lockedtgtIndex = tgtIndex;
                    LockedOn = true;
                }
            }
            else
            {
                if (NearTargets.Count > 0 && NearTargets.Count > lockedtgtIndex)
                {
                    CurrentTarget = NearTargets[lockedtgtIndex];
                }
                else
                {
                    LockedOn = false;
                }

                if (RightStickPress) { LockedOn = false; }
            }

            // SET RETICULE & CAMERA (PLAYER ONLY)
            if (Player)
            {         
                if (TargetReticule && CurrentTarget != null && CharCam != null)
                {
                    if (CharCam)
                    {
                        if (!TargetReticule.gameObject.activeSelf && CurrentTarget.NoIcon == false) 
                        { TargetReticule.gameObject.SetActive(true); }

                        reticuleSize = (Vector3.Distance(CharCam.transform.position, CurrentTarget.transform.position) * TargetReticuleSize) + TargetReticuleSizeOffset;
                        reticuleRot = Quaternion.LookRotation((CharCam.transform.position - CurrentTarget.transform.position).normalized, -Char.GravityDir);

                        TargetReticule.transform.rotation = reticuleRot;
                        TargetReticule.transform.localScale = Vector3.one * reticuleSize;
                        TargetReticule.transform.position = CurrentTarget.transform.position;
                        CharCam.EnemyTarget = CurrentTarget;

                        // DIFFERENT TO PLAYER FACTION
                        if (CurrentTarget.TargetFaction.FactionName != CharacterFaction.FactionName)
                        {
                            if (CurrentTarget.IsHostile && !CurrentTarget.IsWeak)
                            {
                                if (!LockedOn)
                                {
                                    TargetReticule.SetTrigger("Enemy");
                                }
                                else
                                {
                                    TargetReticule.ResetTrigger("Enemy");
                                    TargetReticule.SetTrigger("Locked");
                                }
                                // CHECK IF IN COMBAT
                                if(Actions.Action == 1) { InCombat = true; }
                            }
                            else
                            {
                                TargetReticule.SetTrigger("Neutral");
                            }

                            // CHECK IF WEAK TARGET (Jester Dash)
                            if (CurrentTarget.IsWeak)
                            {
                                if (Vector3.Distance(transform.position, CurrentTarget.transform.position) < WeakTargetMinDistance)
                                {
                                    TargetReticule.ResetTrigger("Neutral");
                                    TargetReticule.SetTrigger("Weak");
                                    WeakTarget = CurrentTarget.IsWeak;
                                }
                                else
                                {
                                    WeakTarget = false;
                                }
                            }
                            else
                            {
                                WeakTarget = false;
                            }


                            // SET CAMERA & GAMEPLAY MODE
                            if (CharCam.Mode <= 1)
                            {
                                if (CharCam.TargetDistance < CurrentTarget.CameraRadius && CurrentTarget.IsHostile && InCombat)
                                {
                                    CharCam.Mode = 1;
                                }
                                else
                                {
                                    InCombat = false;
                                    CharCam.Mode = 0;
                                }
                            }
                        }
                        else if (CurrentTarget.TargetFaction.FactionName == CharacterFaction.FactionName) // SAME FACTION AS PLAYER
                        {
                            CharCam.Mode = 0;
                            TargetReticule.SetTrigger("Friendly");
                        }
                        else
                        {
                            InCombat = false;
                            CharCam.Mode = 0;
                        }

                        tgtLastFaction = CurrentTarget.TargetFaction;
                    }
                }
                else
                {
                    if (TargetReticule.gameObject.activeSelf) { TargetReticule.gameObject.SetActive(false); }
                }
            }
            else
            {
                if (TargetReticule.gameObject.activeSelf) { TargetReticule.gameObject.SetActive(false); }
            }

            if(CurrentTarget == null && CharCam != null)
            {
                CharCam.Mode = 0;
            }

            // DEBUG TARGETS
            if (TargetDebug)
            {

            }
        }
    }

    private void LateUpdate()
    {
        // SET CURSOR
        if (Player)
        {
            if (Cursor.visible || Cursor.lockState != CursorLockMode.Locked)
            {
                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // DEBUG
        //DebugText.AddDebugString("AI Action: " + ai.AI_Action + " | Subaction: " + ai.AI_Subaction);
    }

    // TARGETING SYSTEM
    public void GetNearTargets()
    {
        if (CheckForTargets)
        {
            NearTargets.Clear();
            for (int i = 0; i < TargetData.Targets.Count; i++)
            {
                if (TargetData.Targets[i] != null)
                {
                    tgtDist = Vector3.Distance(TargetData.Targets[i].transform.position, transform.position);
                    if (tgtDist < TargetsDistanceToCheck)
                    {
                        if(TargetData.Targets[i].AttachedCharacter != null) 
                        {
                            if (TargetData.Targets[i].AttachedCharacter.name != name) // DONT ADD IF SAME CHARACTER
                            {
                                NearTargets.Add(TargetData.Targets[i]);
                            }
                        }
                        else
                        {
                            NearTargets.Add(TargetData.Targets[i]);
                        }
                    }
                }
            }
        }
    }

    void AiStateMachine()
    {
        // INITIAL AI SETS
        anim.SetInteger("Action", (int)Actions.Action);
        CheckForTargets = true;
        LockedOn = false;
        if (CurrentTarget)
        {
            anim.SetFloat("TargetDistance", Vector3.Distance(transform.position, CurrentTarget.transform.position));
            DirSet();
        }

        // AI ALLY MANAGEMENT
        if (ai.IsAlly)
        {
            // CHECK IF ENEMIES NEARBY
            ai.allyEnemyNearby = false;
            for (int i = 0; i < NearTargets.Count; i++)
            {
                if (NearTargets[i] != null)
                {
                    if (Vector3.Distance(transform.position, NearTargets[i].transform.position) < ai.AllyEnemyMinDistance)
                    {
                        if (NearTargets[i].TargetFaction.FactionName == "Enemy")
                        {
                            ai.Target = NearTargets[i].transform;
                            ai.allyEnemyNearby = true;
                        }
                    }
                }
            }

            // PATHFIND TO PLAYER
            if (ai.allyEnemyNearby == false)
            {
                ai.Target = ai.AllyPlayer.transform;
                ai.allyDistance = Vector3.Distance(ai.Target.position, transform.position);
                if (ai.allyDistance > ai.AllyDistance)
                { ai.PathFinding = true; }
                else
                { ai.PathFinding = false; }
            }
            else // SEARCH AND FIGHT ENEMY
            {
                ai.Target = ai.AllyPlayer.transform;
                ai.allyDistance = Vector3.Distance(ai.Target.position, transform.position);
                if (ai.allyDistance > ai.AllyEnemyMinDistance)
                {
                    ai.AI_Action = 0;
                    ai.AI_Subaction = 0;
                    ai.PathFinding = true;
                }
                else
                {
                    if (ai.AI_Action != 1)
                    {
                        ai.AI_Action = 1;
                        ai.AI_Subaction = 0;
                        ai.PathFinding = false;
                        anim.SetBool("Idle", false);
                        ai.counter = 0;
                        ai.counter2 = 0;
                        Actions.SwitchAction(-1);
                    }
                }
            }

            // CHECK FOR CONDITIONS TO ALLY TELEPORT
            ai.allyDistance = Vector3.Distance(ai.AllyPlayer.transform.position, transform.position);

            if (ai.allyDistance > ai.AllyTeleportDistanceThreshold && Char.SpeedMagnitude < 0.5f && ai.AI_Action == 0 && ai.AI_Subaction <= 1)
            { ai.allyStuckCounter += Time.fixedDeltaTime; }
            else if (ai.allyDistance > ai.AllyTeleportDistanceThreshold && ai.AI_Action == 0)
            { ai.allyStuckCounter += Time.fixedDeltaTime; }
            else { ai.allyStuckCounter = Mathf.Clamp(ai.allyStuckCounter -= Time.deltaTime, 0, 99); }

            // DO THE TELEPORT
            if (ai.allyStuckCounter >= ai.AllyTeleportTime) { TeleportToPlayer(); }
            void TeleportToPlayer()
            {
                // FIND TELEPORT LOCATION
                RaycastHit r;
                Vector3 pos = ai.AllyPlayer.transform.position - (ai.AllyPlayer.Basic.Skin.forward * 0.5f);
                if(Physics.Raycast(pos, Char.GravityDir, out r, 3))
                {
                    // SET VARIABLES
                    ai.allyStuckCounter = 0;
                    ai.AI_Action = 0;
                    ai.AI_Subaction = 0;
                    ai.PathFinding = true;
                    IHB.StartIHB(ai.AllyTeleportIHB);
                    ai.allyEnemyNearby = false;
                    Char.Grounded = false;
                    Char.rigid.linearVelocity = ai.AllyPlayer.Char.rigid.linearVelocity;
                    Char.GravityDir = ai.AllyPlayer.Char.GravityDir;
                    transform.position = pos;
                }
            }
        }
        else // OTHER TARGET NPC
        {
            // AI TARGET MANAGEMENT
            if (ai.Agressive == false)
            {
                // REGULAR NPC
            }
            else if (ai.Agressive == true)
            {
                // ENEMY NPC
                // FIND NEXT TARGET
                if (ai.Agressive)
                {
                    if (ai.HitBy == null)
                    {
                        // LOOK FOR A TARGET
                        if (CurrentTarget)
                        {
                            ai.Target = CurrentTarget.transform;
                        }
                    }
                    else
                    {
                        // TARGET WHO HIT HIM
                        ai.Target = ai.HitBy.transform;
                    }
                }
            }
        }  

        // AI ACTIONS
        if (ai.AI_Action == 0) // MOVEMENT
        {
            if (ai.AI_Subaction == 0) // MOVE PATHFINDING TO FIND TARGET
            {
                if (ai.counter < 0.01f)
                {
                    ai.counter = 0.02f;
                    Actions.SwitchAction(0);
                    Actions.Attacks.RootMotion.EnableRootMotion = true;
                    LeftAnalogInput = Vector3.zero;
                    CamRelativeInput = Vector3.zero;
                    Actions.Basic.prevInput = Actions.Basic.Skin.transform.forward;
                }

                ai.counter += Time.fixedDeltaTime;
                if (ai.PathFinding && ai.Target != null && Actions.Action == 0 && ai.counter > 0.3f)
                {
                    Jump = false;
                    JumpHold = false;
                    Actions.Basic.SubAction = 0;

                    // FIND PATH
                    ai.counter2 -= Time.fixedDeltaTime;
                    if (ai.counter2 < 0.0f)
                    {
                        ai.counter2 = 0.25f + Random.Range(0, 0.25f);
                        ai.PathfindingStep = 1;
                        Pathfinding(ai.PathMaxSteps, ai.PathRadius, ai.PathMarch, Char.GravityDir, transform, ai.Target, ai.PathfindingMask);

                        // JUMP
                        if (Physics.Raycast(transform.position,
                            Vector3.Slerp(-transform.up, Actions.Basic.Skin.transform.forward, ai.JumpRayDir), ai.JumpRayDist, ai.PathfindingMask))
                        {
                            ai.counter2 = 0;
                            ai.counter = 0;
                            ai.AI_Subaction = 2;
                        }
                    }

                    // JUMP DEBUG
                    Debug.DrawRay(transform.position, Vector3.Slerp(-transform.up, Actions.Basic.Skin.transform.forward, ai.JumpRayDir) * ai.JumpRayDist, Color.red);

                    // GET WAYPOINT
                    if (ai.PathfindingStep < Pathfound.Waypoints.Count)
                    { ai.TargetPos = Pathfound.Waypoints[ai.PathfindingStep]; }

                    // GET DIST
                    ai.dir = ai.TargetPos - transform.position;
                    ai.distance = ai.dir.magnitude;
                    ai.dir /= ai.distance;

                    ai.projecteddir = ai.dir;
                    ai.projecteddir = Vector3.ProjectOnPlane(ai.projecteddir, Char.GravityDir);
                    ai.projecteddir = Vector3.ProjectOnPlane(ai.projecteddir, -Char.GravityDir);
                    ai.projecteddir = ai.projecteddir.normalized;

                    // MOVE TO NEXT WAYPOINT
                    if (ai.distance < ai.PathMinDistanceToWaypoint && ai.PathfindingStep < ai.PathMaxSteps)
                    { ai.PathfindingStep++; }

                    // STOP WHEN CLOSE, THEN ATTACK OR STRAFE
                    if (ai.Agressive)
                    {
                        if (ai.AttackWhenDistant == false && Vector3.Distance(transform.position, ai.Target.position) > ai.AgroTargetDistance) // CONTINUE MOVING
                        {
                            ai.atkCooldownCounter = 0;
                            LeftAnalogInput = ai.projecteddir * ai.PathfindingMoveSpeed;
                            CamRelativeInput = ai.projecteddir * ai.PathfindingMoveSpeed;
                        }
                        // IF ATTACK WHILE STILL MOVING
                        else if(ai.AttackWhenDistant == true && ai.atkCooldownCounter < ai.AttackInterval)
                        {
                            // ONLY MOVE IF FAR
                            if (Vector3.Distance(transform.position, ai.Target.position) > ai.AgroTargetDistance)
                            {
                                LeftAnalogInput = ai.projecteddir * ai.PathfindingMoveSpeed;
                                CamRelativeInput = ai.projecteddir * ai.PathfindingMoveSpeed;
                            }
                            else
                            {
                                LeftAnalogInput = Vector3.zero;
                                CamRelativeInput = Vector3.zero;
                            }
                            ai.atkCooldownCounter += Time.fixedDeltaTime;
                        }
                        else // STOP AND NEXT ( DO ATTACK )
                        {
                            LeftAnalogInput = Vector3.zero;
                            CamRelativeInput = Vector3.zero;
                            if (ai.Strafe && ai.counter > 0.5f && Actions.Action == 0 && Char.Grounded) // IF STRAFE
                            {
                                if (ai.AttackWhenDistant) { anim.SetTrigger("en_atk_misc"); }
                                ai.counter = 0;
                                ai.counter2 = 0;
                                ai.AI_Action = 0;
                                ai.AI_Subaction = 1;
                                Actions.SwitchAction(-1);
                            }
                            else if (ai.counter > 0.5f && Actions.Action == 0 && Char.Grounded) // IF NO STRAFE
                            {
                                // START ATTACK
                                ai.atkCooldownCounter += Time.deltaTime;
                                if (ai.atkCooldownCounter > ai.AttackInterval)
                                {
                                    if (ai.AttackWhenDistant) { anim.SetTrigger("en_atk_misc"); }
                                    Actions.SwitchAction(-1);
                                    ai.AI_Action = 1;
                                    ai.AI_Subaction = 0;
                                    ai.atkCooldownCounter = 0;
                                    ai.counter = 0;
                                    ai.counter2 = 0;
                                    anim.ResetTrigger("en_action_strafe");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, ai.Target.position) > ai.NormalTargetDistance)
                        {
                            if (ai.counter > 0.1f)
                            {
                                //ai.PathFinding = false;
                                LeftAnalogInput = ai.projecteddir * ai.PathfindingMoveSpeed;
                                CamRelativeInput = ai.projecteddir * ai.PathfindingMoveSpeed;
                            }
                        }
                        else
                        {
                            LeftAnalogInput = Vector3.zero;
                            CamRelativeInput = Vector3.zero;
                        }
                    }
                }
                else
                {
                    LeftAnalogInput = Vector3.zero;
                    CamRelativeInput = Vector3.zero;
                    ai.PathfindingStep = 0;
                    ai.distance = 0;
                    ai.counter2 = 0;
                }
            }
            else if (ai.AI_Subaction == 1) // STRAFE
            {
                // CHECK TARGET
                if(CurrentTarget == null) { ai.AI_Subaction = 0; return; }

                // INITIAL STRAFE
                Actions.Attacks.RootMotion.EnableRootMotion = true;
                ai.Target = CurrentTarget.transform;
                ai.TargetPos = ai.Target.position;
                ai.dir = ai.TargetPos - transform.position;
                ai.distance = ai.dir.magnitude;
                ai.dir /= ai.distance;
                ai.projecteddir = ai.dir;
                ai.projecteddir = Vector3.ProjectOnPlane(ai.projecteddir, Char.GravityDir);
                ai.projecteddir = Vector3.ProjectOnPlane(ai.projecteddir, -Char.GravityDir);
                ai.projecteddir = ai.projecteddir.normalized;
                Char.rigid.linearDamping = 10;
                anim.SetLayerWeight(Actions.Basic.LandingLayer, 0);

                // TAKEAWAYS
                LeftAnalogInput = ai.projecteddir;
                CamRelativeInput = ai.projecteddir;

                // STRAFE CYCLE
                if (ai.counter2 < 0.05f)
                {
                    Actions.SwitchAction(-1);
                    anim.SetTrigger("en_action_strafe");
                    ai.strafeLerpedDir.x = ((float)ai.random.Next(-255, 255) / 255f) * 2f;
                    ai.strafeLerpedDir.y = ((float)ai.random.Next(-255, 255) / 255f) * 0.01f;
                    if (ai.strafeLerpedDir.x > -0.75f && ai.strafeLerpedDir.x < 0.75f)
                    { ai.strafeLerpedDir.x = 0.75f * Mathf.Sign(ai.strafeLerpedDir.x); }

                    ai.randomGen = (float)ai.random.Next((int)ai.StrafeDirDuration.x * 255, (int)ai.StrafeDirDuration.y * 255) / 255f;
                    ai.counter = 0.05f;
                }

                anim.SetFloat("strafe_x", Mathf.Lerp(anim.GetFloat("strafe_x"), ai.strafeLerpedDir.x, Time.fixedDeltaTime * 5));
                anim.SetFloat("strafe_z", Mathf.Lerp(anim.GetFloat("strafe_z"), ai.strafeLerpedDir.y, Time.fixedDeltaTime * 5));
                ai.counter += Time.fixedDeltaTime;
                ai.counter2 += Time.fixedDeltaTime;

                if (ai.counter2 > ai.randomGen)
                { ai.counter2 = 0; }

                // SKIN ROT
                Debug.DrawRay(transform.position, ai.projecteddir * 2, Color.yellow);
                Actions.Basic.SkinRotation(ai.projecteddir, -Char.GravityDir, 10,0);
                LeftAnalogInput = Vector3.zero;
                CamRelativeInput = Vector3.zero;

                // BREAK OUT, UNLESS FLINCHED
                if (!Char.Grounded || Actions.Action != -1 || ai.distance > ai.AgroTargetDistance + 0.2f)
                {
                    ai.counter = 0;
                    ai.counter2 = 0;
                    ai.randomGen = 0;
                    ai.AI_Action = 0;
                    ai.AI_Subaction = 3;
                    anim.ResetTrigger("en_action_strafe");
                    if (Actions.Action == -1) { Actions.SwitchAction(0); }
                }

                // START ATTACK
                ai.atkCooldownCounter += Time.fixedDeltaTime;
                if (ai.atkCooldownCounter > ai.AttackInterval)
                {
                    Actions.SwitchAction(-1);
                    ai.AI_Action = 1;
                    ai.AI_Subaction = 0;
                    ai.atkCooldownCounter = 0;
                    ai.counter = 0;
                    ai.counter2 = 0;
                    anim.ResetTrigger("en_action_strafe");
                }

            }
            else if (ai.AI_Subaction == 2) // SIMPLE JUMP
            {
                if (ai.counter <= 0.0f)
                {
                    Actions.SwitchAction(0);
                    ai.counter = 0.01f;
                    if (Char.Grounded)
                    {
                        Jump = true;
                        JumpHold = true;
                        Actions.Basic.CoyoteActive = true;
                        Actions.Basic.CheckJump(false);
                    }
                }

                ai.counter += Time.fixedDeltaTime;
                Jump = false;
                if (ai.counter < 0.1f) { JumpHold = true; } else { JumpHold = false; }
                if (ai.counter > 0.5f)
                {
                    ai.counter = 0;
                    ai.counter2 = 0;
                    ai.AI_Subaction = 0;
                }
            }
            else if (ai.AI_Subaction == 3) // HOLDOVER TO ACTION ZERO
            {
                if (Actions.Action == 0)
                {
                    ai.counter = 0;
                    ai.counter2 = 0;
                    ai.AI_Action = 0;
                    ai.AI_Subaction = 0;
                    LeftAnalogInput = Vector3.zero;
                    CamRelativeInput = Vector3.zero;
                    Actions.Basic.prevInput = Actions.Basic.Skin.transform.forward;
                }
            }
            else if(ai.AI_Subaction == 4) // REVENGE
            {
                if (ai.counter < 0.01f)
                {
                    Actions.Interactions.RevengeTrigger = false;
                    Actions.Interactions.RevengeCounter = 0;
                    ai.counter = 0.02f;
                    Actions.Attacks.RootMotion.EnableRootMotion = true;
                    Actions.Interactions.Ap = Actions.Interactions.ApMax;
                    IHB.StartIHB(Actions.Interactions.RevengeIHB);
                    anim.SetInteger("Action", -1);
                    anim.SetTrigger("Revenge");
                    Actions.SwitchAction(-1);
                }

                LeftAnalogInput = Vector3.zero;
                CamRelativeInput = Vector3.zero;
                Actions.Char.rigid.linearVelocity = Vector3.zero;

                ai.counter += Time.fixedDeltaTime;
                if(ai.counter > 1f)
                {
                    if(ai.RevengeWaitForIdle == false)
                    {
                        Actions.SwitchAction(0);
                        ai.AI_Action = 0;
                        ai.AI_Subaction = 0;
                        ai.atkCooldownCounter = 0;
                        ai.counter = 0;
                        ai.counter2 = 0;
                    }
                    else
                    {
                        if (anim.GetBool("Idle"))
                        {
                            Actions.SwitchAction(0);
                            ai.AI_Action = 1;
                            ai.AI_Subaction = 0;
                            ai.atkCooldownCounter = 0;
                            ai.counter = 0;
                            ai.counter2 = 0;
                        }
                    }

                }

            }

            // AI, START REVENGE
            if (Actions.Attacks.SubAction == 2 && ai.counter > 0.01f 
                && Actions.Interactions.RevengeTrigger && Char.Grounded)
            {
                Char.rigid.linearVelocity = Vector3.Lerp(Char.rigid.linearVelocity, Vector3.zero, Time.deltaTime * 2);
                LeftAnalogInput = Vector3.zero;
                CamRelativeInput = Vector3.zero;
                ai.counter = 0;
                ai.AI_Subaction = 4;
                ai.AI_Action = 0;
                Actions.Interactions.LastDamageAmmTaken = 0;
                Actions.Interactions.RevengeTrigger = false;
                Actions.Interactions.RevengeCounter = 0;
                Actions.Interactions.Ap = Actions.Interactions.ApMax;
            }
        }
        else if (ai.AI_Action == 1) // COMBAT
        {
            if (ai.AI_Subaction == 0) // NORMAL ATTACK SEQUENCE
            {
                // SETUP
                DirSet();
                Char.rigid.linearDamping = 20;
                anim.SetLayerWeight(Actions.Basic.LandingLayer, 0);

                // TAKEAWAYS
                LeftAnalogInput = ai.projecteddir;
                CamRelativeInput = ai.projecteddir;

                // START ATTACK
                if (ai.counter2 < 0.01f)
                {
                    ai.counter2 = 0.02f;
                    ai.randomGen = ((float)ai.random.Next(0, 4096) / 4096f) * ai.AttacksAmmount;    /*Debug.Log("1: " + ai.randomGen);*/
                    ai.randomGen = Mathf.Floor(ai.randomGen);                                       /*Debug.Log("2: " + ai.randomGen);*/
                    ai.randomGen = Mathf.Clamp(ai.randomGen, 0, ai.AttacksAmmount);                 /*Debug.Log("3: " + ai.randomGen);*/
                    //Debug.Log("TRIGGERING ATTACK: " + "en_atk_" + ai.randomGen);
                    if (ai.IgnoreStandardAttackSystem == false) { anim.SetTrigger("en_atk_" + ai.randomGen); }
                }

                ai.counter += Time.fixedDeltaTime;
                ai.counter2 += Time.fixedDeltaTime;

                // MOVE FORWARD OR NOT
                if (CurrentTarget)
                {
                    if (ai.distance > CurrentTarget.SecondaryRadius)
                    { Actions.Attacks.RootMotion.EnableRootMotion = true; }
                    else
                    { Actions.Attacks.RootMotion.EnableRootMotion = false; }
                }

                // BREAK OUT
                if (Actions.Action != -1)
                {
                    ai.counter = 0;
                    ai.counter2 = 0;
                    ai.randomGen = 0;
                    ai.AI_Action = 0;
                    ai.AI_Subaction = 3;
                    if (Actions.Action == -1) { Actions.SwitchAction(0); }
                }

                // END ATTACK
                if (ai.counter2 > 0.2f)
                {
                    if (anim.GetBool("Idle"))
                    {
                        ai.AI_Action = 0;
                        ai.AI_Subaction = 3;
                        ai.counter = 0;
                        ai.counter2 = 0;
                        Actions.SwitchAction(0);
                        Pathfinding(ai.PathMaxSteps, ai.PathRadius, ai.PathMarch, Char.GravityDir, transform, ai.Target, ai.PathfindingMask);
                    }
                }
            }
            else if (ai.AI_Subaction == 1) // DEATH
            {
                if (Actions.Interactions)
                {
                    if (ai.DeathType == 0)
                    {
                        if (ai.counter <= 0.0f)
                        {
                            ai.Dead = true;
                            ai.counter += 0.01f;
                            anim.SetTrigger("Dead");
                            Actions.SwitchAction(-1);
                            Actions.Interactions.CombatInteractions = false;
                            Char.rigid.linearDamping = 20;

                            // GIVE POINTS & ENERGY
                            CharacterInteractions playerpoints;
                            if (CharacterCamera.Main.Char.TryGetComponent<CharacterInteractions>(out playerpoints))
                            {
                                playerpoints.SecondaryPenaltyCounter = ai.points.GracePeriod;
                                playerpoints.AddScore(ai.points.Score, ai.points.MultiplierAdd, ai.points.TimeAdded, ai.points.ItenName);
                                playerpoints.En += ai.EnergyOnDeath;
                            }
                        }

                        if (Char.Grounded) { anim.SetBool("Grounded", Char.Grounded); }

                        // KILL OBJECT
                        ai.counter += Time.fixedDeltaTime;
                        if (ai.counter > ai.DeathTime)
                        {
                            if (Char.PlatformReference) { Destroy(Char.PlatformReference.gameObject); }
                            if (TargetReticule) { Destroy(TargetReticule.gameObject); }

                            Destroy(gameObject);
                        }
                    }
                    else if(ai.DeathType == 1)
                    {
                        GameObject g = Instantiate(ai.DeathObject);
                        g.transform.position = transform.position;
                        g.transform.rotation = transform.rotation;
                        Rigidbody r;
                        if (g.TryGetComponent<Rigidbody>(out r))
                        {
                            g.GetComponent<Rigidbody>().linearVelocity = -transform.forward * 2;
                            g.GetComponent<Rigidbody>().AddTorque(transform.up * 2, ForceMode.VelocityChange);
                        }
                        Destroy(gameObject);
                    }
                }
            }

        }

        // AI DEATH
        if (Actions.Interactions)
        {
            if (
                Actions.Interactions.CombatInteractions && Actions.Interactions.Hp <= 0.0f 
                //&& ai.AI_Action != 1 && ai.AI_Subaction != 0
                )
            {
                Actions.SwitchAction(-1);
                Actions.Basic.enabled = false;
                Actions.Attacks.enabled = false;
                Actions.Interactions.enabled = false;
                ai.counter = 0;
                ai.counter2 = 0;
                ai.AI_Action = 1;
                ai.AI_Subaction = 1;
            }
        }

        // EXTRA TRACKING
        if (CurrentTarget && ai.trackingCounter > 0)
        {
            ai.trackingCounter -= Time.fixedDeltaTime;
            Actions.Basic.SkinRotation(ai.projecteddir, -Char.GravityDir, ai.trackingSpeed,0);
        }

        // SPECIAL ACTIONS
        if (ai.JumpToStart)
        {
            ai.JumpCounter += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(transform.position, ai.StartPosition, 
                Time.fixedDeltaTime * ai.JumpToStartSpeed);
            if(ai.JumpCounter > ai.JumpTime)
            {
                ai.JumpCounter = 0;
                ai.JumpToStart = false;
            }
        }

        // FINAL SETS
        InputMag = LeftAnalogInput.magnitude;

        void DirSet()
        {
            if(CurrentTarget != null)
            {
                if (ai.UseCustomTarget == false) { ai.Target = CurrentTarget.transform; }
            }

            if (ai.Target != null)
            {
                ai.TargetPos = ai.Target.position;
                ai.dir = ai.TargetPos - transform.position;
                ai.distance = ai.dir.magnitude;
                ai.dir /= ai.distance;
                ai.projecteddir = ai.dir;
                ai.projecteddir = Vector3.ProjectOnPlane(ai.projecteddir, Char.GravityDir);
                ai.projecteddir = Vector3.ProjectOnPlane(ai.projecteddir, -Char.GravityDir);
                ai.projecteddir = ai.projecteddir.normalized;
            }
        }

    }

    // PARTHFINDING
    Vector3 pathdir;
    Vector3 initialPathdir;
    Vector3 pathpos;
    float pathdist;
    float currentRadius;
    Path Pathfound = new Path();
    Vector3 path2;

    public void Pathfinding(int MaxSteps, float offset, float marchdist, Vector3 grav, Transform obj,  Transform tgt, LayerMask mask)
    {
        pathpos = obj.position;
        pathdir = (tgt.position - obj.position);
        pathdist = pathdir.magnitude;
        pathdir = pathdir / pathdist;
        initialPathdir = pathdir;
        Pathfound.Waypoints.Clear();

        for (i = 0; i < MaxSteps; i++)
        {
            if (Physics.Raycast(pathpos, pathdir, out h, pathdist, mask))
            {
                Pathfound.Waypoints.Add(pathpos);
                pathpos = h.point + (h.normal * offset);
                pathdir = (tgt.position - pathpos);
                pathdist = pathdir.magnitude;
                pathdir = Vector3.ProjectOnPlane(pathdir / pathdist, h.normal).normalized;
                Pathfound.Waypoints.Add(pathpos);
                Debug.DrawRay(pathpos, pathdir * h.distance);
            }
            else
            {
                // MARCH RAY ALONG DIRECTION 
                Pathfound.Waypoints.Add(pathpos);
                Debug.DrawRay(pathpos, pathdir * marchdist);
                pathpos += pathdir * marchdist;
                pathdir = (tgt.position - pathpos);
                pathdist = pathdir.magnitude;
                pathdir = pathdir / pathdir.magnitude;
                Pathfound.Waypoints.Add(pathpos);
            }

            if(pathdist < 1) { return; }
        }
    }

    [System.Serializable]
    public class Path
    {
        public List<Vector3> Waypoints = new List<Vector3>();
    }

    private void OnDrawGizmos()
    {
        for (int j = 0; j < Pathfound.Waypoints.Count; j++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(Pathfound.Waypoints[j], 0.05f);
        }
    }

    public IEnumerator DisableInputCoroutine;
    public IEnumerator DisableInputForATime(float time)
    {
        InputEnabled = false;
        yield return new WaitForSeconds(time);
        InputEnabled = true;
    }

    private void OnDestroy()
    {
        if (Player) 
        { 
            EnemySpawner.Characters.Clear();
            //if (Inp != null) { Inp.controllers.maps.GetMap(Rewired.ControllerType.Keyboard, 0, 2).enabled = false; }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }

        StopAllCoroutines();
    }
}

[System.Serializable]
public class AiParameters
{
    [Header ("Main")]
    public int AI_Action = 0;
    public int AI_Subaction = 0;
    public bool Agressive = false;
    public LayerMask PathfindingMask;

    [Header("Ally")]
    public bool IsAlly = false;
    public CharacterActions AllyPlayer;
    public float AllyDistance = 2;
    public float AllyEnemyMinDistance = 10;
    public GameObject AllyTeleportIHB;
    public float AllyTeleportCloseness = 0.25f;
    public float AllyTeleportDistanceThreshold = 5;
    public float AllyTeleportTime = 5;

    [Header("Misc References")]
    public HeadLook Head;

    [Header("Pathfinding")]
    public float PathfindingMoveSpeed = 1;
    public bool PathFinding = true;
    public int PathMaxSteps = 4;
    public float PathMarch = 0.5f;
    public float PathDistance = 0.5f;
    public float PathRadius = 0.05f;
    public float PathMinDistanceToWaypoint = 0.5f;
    [Range(0, 1)] public float JumpRayDir = 0.5f;
    public float JumpRayDist = 0.25f;
    public float AgroTargetDistance = 1;
    public float NormalTargetDistance = 0.1f;

    [Header("00 - Movement, Strafe")]
    public float MoveSpeed = 5;
    public bool Strafe = false;
    public float StrafeSpeed = 2;
    public Vector2 StrafeDirDuration = new Vector2(1, 2);
    public Vector3 TargetPos;

    [Header("01 - Combat")]
    public float AttackInterval = 1;
    public int AttacksAmmount = 1;
    public float atkCooldownCounter = 0;
    public float trackingCounter = 0;
    public float trackingSpeed = 60;
    public bool Dead = false;
    public int DeathType = 0;
    public float DeathTime = 3;
    public GameObject DeathObject;
    public float EnergyOnDeath = 10;
    public bool AttackWhenDistant = false;
    public bool RevengeWaitForIdle = false;
    public bool IgnoreStandardAttackSystem = false;

    [Header("Random Parameters")]
    public System.Random random;
    public float randomGen;
    public int Seed = 123456789;
    public int SeedIndex;

    [Header("Modifiers")]
    public bool UseCustomTarget = false;

    [Header("DEBUG & CACHE")]
    public Transform Target;
    public TargetData HitBy;
    public Vector3 dir;
    public Vector3 projecteddir;
    public Vector2 strafeLerpedDir;
    public float distance;
    public float counter = 0;
    public float counter2 = 0;
    public int PathfindingStep = 0;
    public static List<AiParameters> Instances = new List<AiParameters>();

    [Header("Special Actions")]
    public bool JumpToStart = false;
    public float JumpToStartSpeed = 0.5f;
    public float JumpToStartTime = 1;
    public float JumpCounter = 0;
    public float JumpTime = 1;
    public Vector3 StartPosition;

    [Header("Points")]
    public Collectable.Points points;

    [Header("Cache")]
    public float allyDistance;
    public bool allyEnemyNearby;
    public float allyStuckCounter;

    public static void ClearNullInstances()
    {
        for (int i = 0; i < Instances.Count; i++)
        {
            if(Instances == null) { Instances.RemoveAt(i); }
        }
    }
}

