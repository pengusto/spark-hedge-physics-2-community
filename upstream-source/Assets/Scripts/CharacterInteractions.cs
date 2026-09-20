using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractions : MonoBehaviour
{
    [Header("References")]
    public CharacterActions Actions;
    public CharacterSpeedEffects Effects;

    [Header("Base Stats")]
    [HideInInspector] public EntityInfo CharacterFaction; // INPORTED FROM INPUT
    public bool HealthAndStats = false;
    public float BaseHp = 100;
    public float BaseAp = 100;
    public float BasePoise = 5;
    public float BaseEn = 30;

    [Header("Current Stats (Leave Exposed)")]
    public float HpMax;
    public float Hp;
    public float ApMax;
    public float Ap;
    public float EnMax;
    public float En;
    public float Combo;

    [Header("Stats UI Parents")]
    public TargetData CharTarget;
    public CharacterUiReferences Ref;
    public bool UseBarScale = false;
    public float BarScaleMultiplier = 2;
    public float MinBarScale = 30;
    public float BarMoveSpeed = 30;
    public float BarBelowMoveSpeed = 10;
    public float BarBelowTime = 2;

    [Header("Combat Interactions")]
    public bool CombatInteractions = false;
    public bool ComboEnabled = false;
    public Transform CharacterCenter;

    [Header("Combat Stats")]
    public float BaseAttack = 1;
    public float BasePhysDefense = 1;
    public float BaseEleDefense = 1;
    public float MinDamage = 0.1f;
    public float BaseApRegen = 1;
    public float BaseApCooldownTime = 1;
    public float BaseApHitTime = 1;
    public float BaseComboDrain = 1;
    public float MinInvencibilityOnHit = 0.032f;
    public bool RotateOnHit = true;
    public bool RotateOnStrongHit = true;
    public float EnergyGainOnHit = 0.5f;
    public float EnergyGainOnParry = 10f;

    [Header("Combo Stats")]
    public float ComboPenaltySize = 0.5f;
    public float ComboAddOnHit = 1f;
    public float ComboInterval = 0.1f;
    public float ComboAttackIDRemovalTime = 0.5f;
    public int AttacksMax = 30;
    public AnimationCurve ComboMultiplier;
    public AnimationCurve ComboVisibleMultiplier;
    public List<int> AttacksHit;
    float ComboListCounter = 0;
    int ID;

    [Header("Current Stats (Hide Later)")]
    public float AttackPower = 1;
    public float PhysicalDefense = 1;
    public float ElementalDefense = 1;
    public float ApRegen = 1;
    public float ComboDrain = 1;

    [Header("Revenge")]
    public bool RevengeEnabled = false;
    public float RevengeThresold = 60;
    public GameObject RevengeIHB;

    [Header("Multiple HP Bars")]
    public bool MultipleHPBars = false; 
    public int MultipleHPBarsAmmount = 10;

    [Header("FX")]
    public bool UseFx = false;
    public float FlashWhiteOnHitTime = 0.5f;
    public SkinnedMeshRenderer[] CharacterMeshes;
    public AudioSource PlayerDieAudio;
    public AudioSource PlayerHurtAudio;

    [Header("Score")]
    public int Score = 0;
    public float CurrentMultiplier = 1;
    public float MultiplierLossOverTime = 0.1f;
    public AnimationCurve CollectablePitchCurve;
    public float CollectablePitch = 0;
    public float CollectablePitchAddOnCollect = 0.01f;
    float CollectableHidenPitchCounter = 1;

    public float MultiplierColorMax = 100000000;
    public float ScorePenaltyCounter = 1;
    public float SecondaryPenaltyCounter = 1;
    public float ScorePenaltyBarSize = 3;

    [Header("UI")]
    public GameObject FloatUI_Interactions;

    [Header("Cache")]
    public Collider DummyCollider;
    public float HpDownBarCounter = 0;
    public float ApDownBarCounter = 0;
    public float EnDownBarCounter = 0;
    Vector3 s = Vector3.one;
    Vector2 r = Vector3.one;
    float ActiveTime;
    float ComboTime;
    [HideInInspector] public bool ApRecoverMode = false;
    float ApCounter;
    HitboxData h;
    Vector3 charOrigin;
    Vector3 hitDir;
    Vector3 parryHitDir;
    Vector3 upHitDir;
    public bool Invencible;
    public float InvencibilityCounter;
    public bool SuperArmour;
    public float SuperArmourCounter;
    public float PostDamageCounter = 99;
    public float BlockCooldown = 0;
    public float RevengeCounter = 0;
    public bool RevengeTrigger = false;
    float fx_hitcounter = 0;
    bool UsingFX;
    public float LastDamageAmmTaken;
    CharacterInteractions EnemyHP;
    float penaltybar;
    float fakescore;
    float fakemulti;
    Collectable c;
    GameObject g;
    float deadCounter = 0;
    public bool CompleteTrigger = false;
    CharacterInteractions WasHitByInteractions;
    public float ComboPenalty = 0;
    public TriggerEffect Trigger;

    // COLORS
    Color HpBarInitialColor;
    Color ApBarInitialColor;
    Color ComboBarInitialColor;

    private void Start()
    {
        if (HealthAndStats)
        {
            StatsInitialSetup(true);
        }
    }

    void StatsInitialSetup(bool ResetValues)
    {
        HpMax = BaseHp;
        ApMax = BaseAp;
        EnMax = BaseEn;

        AttackPower = BaseAttack;
        PhysicalDefense = BasePhysDefense;
        ElementalDefense = BaseEleDefense;
        ApRegen = BaseApRegen;
        ComboDrain = BaseComboDrain;

        if (ResetValues)
        {
            Hp = HpMax;
            Ap = ApMax;
            En = 0;
            Combo = 0;
        }

        if (Actions != null)
        {
            if(Actions.Inp != null)
            {
                CharacterFaction = Actions.Inp.CharacterFaction;
            }
        }

        // GET UI
        GetUi();

        // PLAY INTRO CARD IF EXITSTS
        if (CharacterStageDetails.Current != null && Ref != null)
        {
            if (CharacterStageDetails.Current.HasIntroCard)
            {
                Ref.Cards.StartIntroCard(Actions.Inp, this, CharacterStageDetails.Current.AreaName, CharacterStageDetails.Current.StageName);
            }
        }
    }

    void GetUi()
    {
        if (Actions && Actions.Inp)
        {
            if (Actions.Inp.Player) 
            { 
                Ref = Actions.Inp.CharCam.CharUiRefs;

                // INITIAL UI SETS
                HpBarInitialColor = Ref.Hp_Bar.color;
                ApBarInitialColor = Ref.Ap_Bar.color;
                ComboBarInitialColor = Ref.Combo_Bar.color;
            }
        }
    }

    string scoreFormat = "###,###,000,000,000";
    string multiplierFormat = "F2";

    private void Update()
    {
        // UI
        if (HealthAndStats && Ref)
        {
            // SET BAR SIZES
            if (Ref.Hp_Bar)
            {
                s.y = 1;
                s.x = Hp / HpMax;
                Ref.Hp_Bar.transform.localScale = Vector3.Lerp(Ref.Hp_Bar.transform.localScale, s, Time.deltaTime * BarMoveSpeed);
                if (Ref.Hp_Text) { Ref.Hp_Text.text = (int)Hp + "/" + (int)HpMax; }
                // BAR SIZE
                if (UseBarScale)
                {
                    r = Ref.Hp_BarScale.sizeDelta; r.x = Mathf.Clamp(HpMax * BarScaleMultiplier, MinBarScale, float.PositiveInfinity);
                    Ref.Hp_BarScale.sizeDelta = r;
                }

                // HP BAR FX
                if(Hp < (HpMax * 0.1f)) { Ref.Hp_Bar.color = Color.Lerp(HpBarInitialColor, HpBarInitialColor * 0.8f, Mathf.Sin(Time.unscaledTime * 60)); }
                else { Ref.Hp_Bar.color = HpBarInitialColor; }
            }
            if (Ref.Ap_Bar)
            {
                s.y = 1;
                s.x = Ap / ApMax;
                Ref.Ap_Bar.transform.localScale = Vector3.Lerp(Ref.Ap_Bar.transform.localScale, s, Time.deltaTime * BarMoveSpeed);
                // POISE THRESHOLD
                if (UseBarScale)
                {
                    r = Vector2.Lerp(Ref.Ap_PoiseEnd.position, Ref.Ap_Bar.transform.position, (BasePoise / ApMax) /*- (Ap / ApMax)*/);
                    Ref.Ap_PoiseIcon.position = r;
                    // BAR SIZE
                    r = Ref.Ap_BarScale.sizeDelta; r.x = Mathf.Clamp(ApMax * BarScaleMultiplier, MinBarScale, float.PositiveInfinity);
                    Ref.Ap_BarScale.sizeDelta = r;
                }

                // AP BAR FX
                if (ApRecoverMode) { Ref.Ap_Bar.color = Color.Lerp(ApBarInitialColor, ApBarInitialColor * 0.8f, Mathf.Sin(Time.unscaledTime * 60)); }
                else { Ref.Ap_Bar.color = ApBarInitialColor; }
            }
            if (Ref.En_Bar)
            {
                s.x = En / EnMax;
                Ref.En_Bar.transform.localScale = Vector3.Lerp(Ref.En_Bar.transform.localScale, s, Time.deltaTime * BarMoveSpeed);
                if (Ref.En_Text) { Ref.En_Text.text = (int)En + "/" + (int)EnMax; }
                // BAR SIZE
                if (UseBarScale)
                {
                    r = Ref.En_BarScale.sizeDelta; r.x = Mathf.Clamp(EnMax * BarScaleMultiplier, MinBarScale, float.PositiveInfinity);
                    Ref.En_BarScale.sizeDelta = r;
                }
            }
            if (Ref.Combo_Bar)
            {
                if (ComboEnabled)
                {
                    if(Ref.StatsComboBar.activeSelf == false) { Ref.StatsComboBar.SetActive(true); }
                    s.x = Combo / 100;
                    Ref.Combo_Bar.transform.localScale = Vector3.Lerp(Ref.Combo_Bar.transform.localScale, s, Time.deltaTime * BarMoveSpeed);
                    Ref.Combo_Text.text = Mathf.Clamp(ComboVisibleMultiplier.Evaluate(Combo) /*- 1*/, 0, 99).ToString("F1") + "X";

                    //FX
                    if (Combo > 50) { Ref.Combo_Bar.color = Color.Lerp(ComboBarInitialColor, ComboBarInitialColor * 0.8f, Mathf.Sin(Time.unscaledTime * 60)); }
                    else { Ref.Combo_Bar.color = ComboBarInitialColor; }
                }
                else
                {
                    if (Ref.StatsComboBar.activeSelf == true) { Ref.StatsComboBar.SetActive(false); }
                }
            }

            // UNDER BAR;
            if (HpDownBarCounter > 0) { HpDownBarCounter -= Time.deltaTime; }
            else if(Ref.Hp_BarBelow)
            {
                Ref.Hp_BarBelow.transform.localScale = 
                    Vector3.Lerp(Ref.Hp_BarBelow.transform.localScale, Ref.Hp_Bar.transform.localScale, Time.deltaTime * BarBelowMoveSpeed);
            }

            if (ApDownBarCounter > 0) { ApDownBarCounter -= Time.deltaTime; }
            else if(Ref.Ap_BarBelow)
            {
                Ref.Ap_BarBelow.transform.localScale =
                    Vector3.Lerp(Ref.Ap_BarBelow.transform.localScale, Ref.Ap_Bar.transform.localScale, Time.deltaTime * BarBelowMoveSpeed);
            }

            if (EnDownBarCounter > 0) { EnDownBarCounter -= Time.deltaTime; }
            else if(Ref.En_BarBelow)
            {
                Ref.En_BarBelow.transform.localScale =
                    Vector3.Lerp(Ref.En_BarBelow.transform.localScale, Ref.En_Bar.transform.localScale, Time.deltaTime * BarBelowMoveSpeed);
            }

            // ENEMY BAR
            if (Actions.Inp)
            {
                if (Actions.Inp.CurrentTarget != null && Actions.Inp.CurrentTarget.IsHostile)
                {
                    // IF THERES A TARGET, GET ATTACHED CHARACTER AND ENABLE          
                    if (Actions.Inp.CurrentTarget.AttachedCharacter) { EnemyHP = Actions.Inp.CurrentTarget.AttachedCharacter.Interactions; }
                    if (EnemyHP != null && Ref != null && Ref.Enemy_Bar != null)
                    {
                        if (Ref.EnemyBar_Object.activeSelf == false) { Ref.EnemyBar_Object.SetActive(true); }
                        s.y = 1;
                        s.x = EnemyHP.Hp / EnemyHP.HpMax + 0.001f;
                        Ref.Enemy_Bar.transform.localScale = Vector3.Lerp(Ref.Enemy_Bar.transform.localScale, s, Time.deltaTime * BarMoveSpeed);
                        Ref.Enemy_BarBelow.transform.localScale = Vector3.Lerp(Ref.Enemy_BarBelow.transform.localScale, s, Time.deltaTime * BarBelowMoveSpeed);
                        if(Actions.Inp.CurrentTarget.TargetFaction.EntityName != Ref.Enemy_Bar_Name.text) 
                        { Ref.Enemy_Bar_Name.text = Actions.Inp.CurrentTarget.TargetFaction.EntityName; }
                    }
                }
                else
                {
                    if (Ref.EnemyBar_Object.activeSelf == true) { Ref.EnemyBar_Object.SetActive(false); }
                }
            }

            // SCORE BAR 
            if (Ref.ScoreBar)
            {
                Ref.CurrentScoreColor = Ref.ColorOverTime.Evaluate(CurrentMultiplier / MultiplierColorMax);
                Ref.ScoreText.color = Ref.CurrentScoreColor;
                Ref.ScoreNumber.color = Ref.CurrentScoreColor;
                Ref.ScoreBar.color = Ref.CurrentScoreColor;
                Ref.ScoreMultiplierText.color = Ref.CurrentScoreColor;
                Ref.ScoreMultiplierNumber.color = Ref.CurrentScoreColor;

                // SCORE BAR & PENALTY
                if (SecondaryPenaltyCounter < 0)
                {
                    ScorePenaltyCounter -= Time.deltaTime;
                    penaltybar = ScorePenaltyCounter;
                    penaltybar = Mathf.Lerp(0, 1, penaltybar / ScorePenaltyBarSize);
                    Ref.ScoreBar.rectTransform.localScale = new Vector3(penaltybar, 1);
                    if (ScorePenaltyCounter <= 0.0f)
                    {
                        CurrentMultiplier -= MultiplierLossOverTime * Time.deltaTime;
                        CurrentMultiplier = Mathf.Clamp(CurrentMultiplier, 1, 500);
                        ScorePenaltyCounter = 0.0f;
                    }
                }
                else
                {
                    penaltybar = ScorePenaltyCounter;
                    penaltybar = Mathf.Lerp(0, 1, penaltybar / ScorePenaltyBarSize);
                    Ref.ScoreBar.rectTransform.localScale = new Vector3(penaltybar, 1);
                    SecondaryPenaltyCounter -= Time.deltaTime;
                }

                // SCORE TEXT AND MULTIPLIER
                fakescore = Mathf.Lerp(fakescore, Score, Time.deltaTime * 10);
                Ref.ScoreNumber.text = Mathf.RoundToInt(fakescore).ToString(scoreFormat) + "p";
                fakemulti = Mathf.Lerp(fakemulti, CurrentMultiplier, Time.deltaTime * 10);
                Ref.ScoreMultiplierNumber.text = fakemulti.ToString(multiplierFormat) + "x";

            }

            // COLLECTABLES PITCH
            if (CollectableHidenPitchCounter < 0) { CollectablePitch = Mathf.Lerp(CollectablePitch, 0, Time.deltaTime); }
            else { CollectableHidenPitchCounter -= Time.deltaTime; }

        }
        else if (Actions.Inp && Ref == null)
        {
            GetUi();
        }

        if (UseFx)
        {
            if(UsingFX)
            {
                fx_hitcounter -= Time.deltaTime;
                if (fx_hitcounter > 0f)
                {
                    for (int i = 0; i < CharacterMeshes.Length; i++)
                    { CharacterMeshes[i].material.SetFloat("_HitFX", 1); }
                }
                else
                {
                    UsingFX = false;
                    for (int i = 0; i < CharacterMeshes.Length; i++)
                    { CharacterMeshes[i].material.SetFloat("_HitFX", 0); }
                }
            }
        }

        // TRIGGER END FOR PLAYER
        if (Actions.Inp.Player)
        {
            if (CompleteTrigger)
            {
                StartCoroutine(Ref.Cards.StartCompleteCard(Actions.Inp, this, 1f));
                //Actions.SwitchAction(0);
                Actions.Inp.InputEnabled = false;
                Actions.Inp.LeftAnalogInput = Vector3.zero;
                Actions.Inp.CamRelativeInput = Vector3.zero;
                Actions.Inp.InputMag = 0;
                this.enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (CombatInteractions)
        {
            ActiveTime += Time.fixedDeltaTime;
            ApCounter += Time.fixedDeltaTime;
            BlockCooldown += Time.deltaTime;

            // AP MANAGEMENT
            if (Ap <= 0.0f && ApRecoverMode == false)
            {
                // AP DEPLETED RECOVERING
                if (ApCounter > BaseApCooldownTime)  { ApRecoverMode = true; }
            }
            else
            {
                if (ApRecoverMode) // STAGGERED AP RECOVERY
                {
                    if (Actions.Char.Grounded) { Ap += (ApRegen * Time.fixedDeltaTime) * 5f; }
                    if(Ap >= ApMax) { ApRecoverMode = false; }
                }
                else // NORMAL AP RECOVERY
                {
                    if (ApCounter > BaseApHitTime) { Ap += ApRegen * Time.fixedDeltaTime; }
                }
            }

            // INVENCIBILITY FRAMES MANAGEMENT
            if(InvencibilityCounter > 0.0f) { Invencible = true; InvencibilityCounter -= Time.fixedDeltaTime; }
            else { Invencible = false; }

            if(SuperArmourCounter > 0.0f) { SuperArmour = true; SuperArmourCounter -= Time.fixedDeltaTime; }
            else { SuperArmour = false; }

            PostDamageCounter += Time.fixedDeltaTime;

            // COMBO
            if (ComboEnabled)
            {
                if (ComboTime > 0.0f) { ComboTime -= Time.fixedDeltaTime; }
                else if (Combo > 0.0f)
                {
                    Combo -= Time.deltaTime * ComboDrain;
                    Combo = Mathf.Clamp(Combo, 0, 100);
                }

                ComboManager();
            }

            // FINAL SET
            Hp = Mathf.Clamp(Hp, 0, HpMax);
            Ap = Mathf.Clamp(Ap, 0, ApMax);
            En = Mathf.Clamp(En, 0, EnMax);

            if(Actions.Inp.Player == true)
            {
                // CHECK PLAYER DEATH AND FORCE FALL
                if(Hp <= 0) 
                {
                    // CHECK IF MULTIPLE HEALTH BARS
                    if (MultipleHPBars == false || MultipleHPBarsAmmount <= 0)
                    {
                        if (deadCounter < 1)
                        {
                            // START FAIL CARD
                            deadCounter = 1;
                            if (PlayerDieAudio) { PlayerDieAudio.Play(); }
                            if (CharacterStageDetails.Current != null)
                            {
                                StartCoroutine(Ref.Cards.StartFailCard(Actions.Inp, Actions.Interactions, CharacterStageDetails.Current.AllowResetsAfterDeath, 1f));
                            }
                            else
                            {
                                StartCoroutine(Ref.Cards.StartFailCard(Actions.Inp, Actions.Interactions, false, 1f));
                            }

                            // SET TO A NULL ACTION AND FORCE ANIMATION
                            Actions.SwitchAction(-1);
                            Actions.Char.enabled = false;
                            Actions.Basic.anim.SetBool("Grounded", false);
                            Actions.Basic.anim.SetTrigger("Die");
                            Actions.Basic.anim.SetInteger("Action", -1);
                        }
                    }
                    else if (MultipleHPBars == true && MultipleHPBarsAmmount > 0)
                    {
                        if (Ref.UseMultipleHpBarSFX) // FX 
                        { 
                            Ref.UseMultipleHpBarSFX.Play();
                            Ref.UseMultipleHP_Animator.SetTrigger("Start");
                        }

                        Hp = HpMax;
                        MultipleHPBarsAmmount -= 1; 
                    }

                    Actions.Char.rigid.linearVelocity = Vector3.zero;
                    deadCounter += Time.fixedDeltaTime;
                    
                }

                // TRY TO DISABLE
                if (FloatUI_Interactions.activeSelf) { FloatUI_Interactions.SetActive(false); }
            }
        }

        if (MultipleHPBars)
        {
            if(Ref.MultipleHP_Bars_Object.activeSelf == false)
            { Ref.MultipleHP_Bars_Object.SetActive(true); }

            Ref.MutipleHPBars_Amm.text = "x" + MultipleHPBarsAmmount;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (Actions.Inp.Player)
        {
            if (col.CompareTag("Trigger"))
            {
                if (col.TryGetComponent<TriggerEffect>(out Trigger))
                {
                    if (Trigger.TrigEffect == TriggerEffect.Effect.CameraDirection)
                    {
                        Actions.Inp.CharCam.LookAtAngle
                            (Trigger.transform.localEulerAngles, Trigger.Look_Time, 
                            Trigger.Look_Speed, Trigger.transform.rotation, Trigger.transform, Trigger.Look_HeightOffset, Vector3.zero, Vector3.zero);
                        if (Effects) { Effects.camrotcounter = Trigger.Look_Time; }
                    }
                    else if(Trigger.TrigEffect == TriggerEffect.Effect.SceneChange)
                    {
                        if (Ref)
                        {
                            if(Trigger.CutsceneIndex >= 0)
                            {
                                CutsceneRepo.ConvoToPlay = Trigger.CutsceneIndex;                  
                            }

                            Ref.Cards.StartSceneTransition(Trigger.SceneToGoTo);
                            Trigger.enabled = false;
                            Actions.Inp.InputEnabled = false;
                        }
                    }
                    else if(Trigger.TrigEffect == TriggerEffect.Effect.Animation)
                    {
                        Trigger.TriggerAnimations(Trigger.Anim);
                    }
                    else if(Trigger.TrigEffect == TriggerEffect.Effect.Dialog)
                    {
                        if (Actions.Inp.PlayerCheckInput)
                        {
                            if (Trigger.DialogTriggerOnContact)
                            {
                                Trigger.StartDialog(Trigger.DialogObject);
                            }

                            // TRIGGER DIALOG IS IN STAY FUNCTION
                        }
                    }
                }
            }
        }
    }

    AudioSource source;
    private void OnTriggerStay(Collider col)
    {
        if (CombatInteractions)
        {
            if(Actions != null && Actions.Attacks != null)
            {
                if (ActiveTime > 0.2f && col.CompareTag("Hitbox"))
                {
                    h = null;
                    if (col.TryGetComponent<HitboxData>(out h))
                    {
                        if (CheckHit(h) && Actions.Action != 2) 
                        {
                            ApplyHit(h); 
                        }
                        else if(Actions.Action == 2)
                        {
                            ApplyHit(h);
                            Actions.Rail.RailDamage();
                        }
                    }
                }
            }
        }

        if (Actions.Inp.Player)
        {
            //COLLECTABLES
            if (col.CompareTag("Collectable"))
            {
                if(col.TryGetComponent<Collectable>(out c))
                {

                    if (c.ObjectType == Collectable.CollectableType.Points || c.ObjectType == Collectable.CollectableType.PointsAndHP)
                    {
                        SecondaryPenaltyCounter = c.points.GracePeriod;
                        AddScore(c.points.Score, c.points.MultiplierAdd, c.points.TimeAdded, c.points.ItenName);
                    }
                    else if(c.ObjectType == Collectable.CollectableType.HP || c.ObjectType == Collectable.CollectableType.PointsAndHP)
                    {
                        Hp += c.Hp;
                    }

                    // FX
                    CharacterCamera.ShakeCameraAddtive(c.points.FX.ShakeDuration, c.points.FX.ShakeAmplitude, transform.position);
                    CharacterCamera.SlowDown(c.points.FX.SlowDownDuration, c.points.FX.SlowDownTime,
                            c.points.FX.SlowDownRestoreSpeed, transform.position);           

                    // OBJECT
                    if (c.points.GetEffect != null)
                    {
                        g = Instantiate(c.points.GetEffect);
                        g.transform.position = c.points.MainObject.transform.position;
                        g.transform.rotation = transform.rotation;

                        // CHECK IF VIB AND APPLY COLLECTABLE PICTH EFFECT
                        AudioSource source;
                        if(g.TryGetComponent<AudioSource>(out source))
                        {
                            source.pitch += CollectablePitchCurve.Evaluate(CollectablePitch);
                            CollectablePitch += CollectablePitchAddOnCollect;
                            CollectableHidenPitchCounter = 1;
                        }
                    }

                    if(c.ObjectType != Collectable.CollectableType.None) { Destroy(c.gameObject); }
                }
            }

            // TRIGGERS
            if (col.CompareTag("Trigger"))
            {
                if (col.TryGetComponent<TriggerEffect>(out Trigger))
                {
                    if (Trigger.TrigEffect == TriggerEffect.Effect.Dialog)
                    {
                        // SET PROMPT TO TRUE IF NOT IN DIALOG
                        if (FloatUI_Interactions.activeSelf == false) 
                        {
                            if (TextBox.Instance == null)
                            {
                                FloatUI_Interactions.SetActive(true);
                            }
                            else
                            {
                                if (TextBox.Instance.gameObject.activeSelf)
                                {
                                    FloatUI_Interactions.SetActive(false);
                                }
                                else
                                {
                                    FloatUI_Interactions.SetActive(true);
                                }
                            }
                        }


                        if (Actions.Inp.JDashHold)
                        {
                            Trigger.StartDialog(Trigger.DialogObject);
                            Actions.Inp.JDashHold = false;
                        }
                    }
                    else
                    {
                        if (FloatUI_Interactions.activeSelf == true) { FloatUI_Interactions.SetActive(false); }
                    }
                }
            }
        }
    }

    public bool CheckHit(HitboxData hit)
    {
        if (hit != null) 
        {
            // RETURN IF ALREADY DEAD
            if(Hp <= 0) { return true; }

            // SAME FACTION BUT NOT SAME CHARACTER
            if (hit.FriendlyFire && hit.ParentName != name)
            {
                return true;
            }

            // IF DIFFERENT FACTION
            if (hit.BoxFaction.FactionName != CharacterFaction.FactionName)
            {
                if (hit.ParentName == name) { return false; }
                else { return true; }
            }

            return false;
        }
        else
        {
            return false;
        }
    }

    float damage;
    public void ApplyHit(HitboxData hit)
    {
        // RETURN IF ALREADY DEAD
        if (Hp <= 0) { return; }

        //LastDamageAmmTaken = 0;
        if (Invencible) { return; }

        // CHECK FOR PARRY BEFORE DOING ANYTHING
        if (Ap > 0.01f && hit.DamageType == HitboxData.DamageValueType.Normal && Actions.Attacks)
        {
            if(BlockCooldown > 0.1f && Actions.Attacks.ParryStart != null && Actions.Action == 1 && Actions.Attacks.SubAction == 1)
            {
                if(Actions.Attacks.ParryCounter < Actions.Attacks.ParryPerfectThreshold)
                {
                    HitDirection();
                    BlockCooldown = 0;
                    InvencibilityCounter = MinInvencibilityOnHit;
                    IHB.StartIHB(Actions.Attacks.ParryParry);
                    Actions.Attacks.anim.SetTrigger("BlockParry");
                    Actions.Attacks.SubActionTime = 1 + Actions.Attacks.BlockingSkinRotationThreshold;

                    if (hit.Parent) { parryHitDir = -(transform.position - hit.Parent.position).normalized; }
                    Actions.Basic.prevInput = parryHitDir;
                    parryHitDir = Vector3.ProjectOnPlane(parryHitDir, -Actions.Char.GravityDir);
                    Actions.Basic.SkinRotation(parryHitDir, -Actions.Char.GravityDir, 909, 0);
                    En += EnergyGainOnParry;
                    return;
                }
                else
                {
                    HitDirection();
                    BlockCooldown = 0;
                    InvencibilityCounter = MinInvencibilityOnHit;
                    IHB.StartIHB(Actions.Attacks.ParryBlock);
                    Ap -= hit.ApDamage * hit.ApDamageMultiplier;
                    Actions.Attacks.anim.SetTrigger("BlockBlocked");
                    Actions.Attacks.SubActionTime = 1 + Actions.Attacks.BlockingSkinRotationThreshold;

                    if (hit.Parent) { parryHitDir = -(transform.position - hit.Parent.position).normalized; }
                    Actions.Basic.prevInput = parryHitDir;
                    parryHitDir = Vector3.ProjectOnPlane(parryHitDir, -Actions.Char.GravityDir);
                    Actions.Basic.SkinRotation(parryHitDir, -Actions.Char.GravityDir, 909, 0);
                    return;
                }
            }
        }

        // BAR MOVEMENT
        ApDownBarCounter = BarBelowTime;
        HpDownBarCounter = BarBelowTime;

        // CALCULATE DAMAGE
        LastDamageAmmTaken = Hp;
        if (hit.Owner && hit.Owner.Interactions && hit.Owner.Interactions.ComboEnabled)
        {
            damage = hit.PhysicalDamage * hit.Owner.Interactions.ComboMultiplier.Evaluate(hit.Owner.Interactions.Combo);
            damage *= hit.Owner.Interactions.AttackPower;
            damage *= hit.PhysDamageMultiplier;
            Hp -= DefenseCalculation(damage, PhysicalDefense, hit.DamageType);

            damage = hit.ElementalDamage * hit.Owner.Interactions.ComboMultiplier.Evaluate(hit.Owner.Interactions.Combo);
            damage *= hit.Owner.Interactions.AttackPower;
            damage *= hit.ElemDamageMultiplier;
            Hp -= DefenseCalculation(damage, ElementalDefense, hit.DamageType);

            LastDamageAmmTaken = LastDamageAmmTaken - Hp;
        }
        else
        {
            damage = hit.PhysicalDamage;
            if (hit.Owner) { damage *= hit.Owner.Interactions.AttackPower; }
            damage *= hit.PhysDamageMultiplier;
            Hp -= DefenseCalculation(damage, PhysicalDefense, hit.DamageType);

            damage = hit.ElementalDamage;
            if (hit.Owner) { damage *= hit.Owner.Interactions.AttackPower; }
            damage *= hit.ElemDamageMultiplier;
            Hp -= DefenseCalculation(damage, ElementalDefense, hit.DamageType);

            LastDamageAmmTaken = LastDamageAmmTaken - Hp;
        }

        // ADD REVENGE
        if (RevengeEnabled && hit.DontBuildRevenge == false)
        {
            if(Ap < BasePoise && Actions.Char.Grounded) 
            { 
                RevengeCounter += LastDamageAmmTaken;
                if (RevengeCounter > RevengeThresold && RevengeTrigger == false) 
                { 
                    RevengeTrigger = true;
                    RevengeCounter = 0;
                }
            }
        }

        // CHARACTER CENTER
        if (CharacterCenter) { charOrigin = CharacterCenter.position; }
        else { charOrigin = transform.position; }

        // GET HIT DIRECTION
        HitDirection();

        // STAGGER
        ApCounter = 0;
        ApplyExtraEffects();
        ApRecoverMode = false;
        if (Actions.Action != 2)
        {
            if (!hit.ForceStagger)
            {
                Ap -= hit.ApDamage * hit.ApDamageMultiplier;
                if (Ap < (ApMax - BasePoise) && Ap > 0.0f) // FLINCH
                {
                    if (hit.OnlyUpwardsOnStagger) { upHitDir = Vector3.zero; }
                    if (hit.OnlyUpwardsIfInAir && Actions.Char.Grounded) { upHitDir = Vector3.zero; }

                    if (SuperArmour == false)
                    { Actions.Attacks.DoHurtPlayerAnim(0, false, 0, hitDir + upHitDir, upHitDir, hit, this); }
                }
                else if (Ap <= 0.0f) // STAGGERED
                {
                    if (hit.OnlyUpwardsIfInAir && Actions.Char.Grounded) { upHitDir = Vector3.zero; }

                    if (SuperArmour == false)
                    { Actions.Attacks.DoHurtPlayerAnim(0, false, 0, hitDir + upHitDir, upHitDir, hit, this); }

                    if (false)
                    {
                        if (hit.OnlyUpwardsIfInAir && Actions.Char.Grounded) { upHitDir = Vector3.zero; }
                        if (Actions.Char.Grounded)
                        {
                            // DO NOT SEND UP IF ATTACK ISNT STRONG ENOUGH
                            if (hit.AwayForce > 5f)
                            {
                                Actions.Attacks.DoHurtPlayerAnim(1, false, 0, hitDir + upHitDir, upHitDir, hit, this);
                            }
                            else
                            {
                                Actions.Attacks.DoHurtPlayerAnim(1, false, 0, hitDir + upHitDir, upHitDir, hit, this);
                            }
                        }
                        else
                        {
                            Actions.Attacks.DoHurtPlayerAnim(1, false, 0, hitDir + upHitDir, upHitDir, hit, this);
                        }
                    }
                }
            }
            else
            {
                Ap -= hit.ApDamage;
                Actions.Attacks.DoHurtPlayerAnim(0, hit.ForceStagger, hit.ForcedStaggerType, hitDir + upHitDir, upHitDir, hit, this);
            }
        }

        // INVENCIBILITY FRAMES (TIME)
        if (hit.ForceInvencibility) { InvencibilityCounter = hit.InvencibilityTime; }
        else
        {
            if (InvencibilityCounter < hit.InvencibilityTime) // ONLY ADD TIME IF MORE THEN WE ALREADY HAVE
            { InvencibilityCounter = hit.InvencibilityTime; }

            if(InvencibilityCounter < MinInvencibilityOnHit)
            { InvencibilityCounter = MinInvencibilityOnHit; }

        }

        // WHO DONE IT
        if(Actions.Interactions)
        {
            if(hit.Owner != null && hit.Owner.Interactions != null)
            {
                if (hit.Owner.Interactions.CharTarget != null)
                {
                    Actions.Inp.ai.HitBy = hit.Owner.Interactions.CharTarget;
                }
            }
        }

        // ADD COMBO
        if (hit.Owner != null && hit.Owner.Interactions)
        {
            WasHitByInteractions = hit.Owner.Interactions;
            if (hit.Owner.Inp.Player)
            {
                WasHitByInteractions.AddComboAttack(hit);
            }
        }

        // FX
        if (hit.HitObject)
        {
            g = Instantiate(hit.HitObject);
            if (CharacterCenter) { g.transform.position = CharacterCenter.position; }
            else { g.transform.position = transform.position; }
        }
        if (hit.UseFxWhenHit)
        {
            CharacterCamera.SlowDown(hit.SlowDownHitDuration, hit.SlowDownHitTime, hit.SlowDownHitRestore, transform.position);
            CharacterCamera.ShakeCameraAddtive(hit.ShakeHitDuration, hit.ShakeHitAmplitude, transform.position);
        }

        // ON PLAYER HIT
        if(Actions.Inp && Actions.Inp.Player)
        {
            if (Actions.Inp.CharCam)
            {
                Actions.Inp.CharCam.CameraFX(0,0);
                Actions.Inp.CharCam.CameraFX(1, 0.25f);
                CharacterCamera.SlowDown(hit.EffectsOnPlayerHit.SlowDownDuration, hit.EffectsOnPlayerHit.SlowDownTime, hit.EffectsOnPlayerHit.SlowDownRestoreSpeed, transform.position);
                CharacterCamera.ShakeCameraAddtive(hit.EffectsOnPlayerHit.ShakeDuration, hit.EffectsOnPlayerHit.ShakeAmplitude, transform.position);
                
            }
        }

        UsingFX = true;
        fx_hitcounter = FlashWhiteOnHitTime;

        // FUNCTIONS
        void HitDirection()
        {
            if (hit.FakeCenter != null)
            {
                hitDir = (charOrigin - hit.FakeCenter.position).normalized;
            }
            else if (hit.Parent)
            {
                hitDir = (charOrigin - hit.Parent.position).normalized;
            }
            else
            {
                hitDir = (charOrigin - hit.transform.position).normalized;
            }

            // DEFINE HIT DIRECTION AND UP HIT
            hitDir = Vector3.ProjectOnPlane(hitDir, -Actions.Char.GravityDir).normalized;
            hitDir = Vector3.ProjectOnPlane(hitDir, Actions.Char.GravityDir).normalized;
            parryHitDir = hitDir;
            hitDir *= hit.AwayForce;
            upHitDir = Actions.Char.GravityDir * -hit.UpwardForce;
        }

        void ApplyExtraEffects()
        {
            if (hit.NegateInvencibility) { InvencibilityCounter = 0; Invencible = false; }
            if (hit.NegateSuperArmour) { SuperArmourCounter = 0; SuperArmour = false; }
            if (PlayerHurtAudio) { PlayerHurtAudio.Play(); }
        }
    }

    public void AddScore(int score, float multiplierAdd, float counteradd, string name)
    {
        // STAGE STUFF
        if(CharacterStageDetails.Current != null)
        {
            score = Mathf.RoundToInt(score * CharacterStageDetails.Current.ScoreGainMultiplier);
            score = Mathf.RoundToInt(score * CharacterStageDetails.Current.DifficultyMultiplier);
        }

        // ADD POINTS
        ScorePenaltyCounter = Mathf.Clamp(ScorePenaltyCounter, 0, 99999999);
        ScorePenaltyCounter += counteradd;
        CurrentMultiplier += multiplierAdd;
        Score += Mathf.RoundToInt(score * CurrentMultiplier);
        Ref.AddNotifObject(name, Mathf.RoundToInt(score * CurrentMultiplier), multiplierAdd);
    }

    // DEFENSE CALC
    float def = 0;
    float dmg = 0;

    public float DefenseCalculation(float damage, float defense, HitboxData.DamageValueType damageType)
    {
        if(damageType == HitboxData.DamageValueType.Normal || damageType == HitboxData.DamageValueType.NormalUnblockable) // STANDARD
        {
            def = Mathf.Clamp(defense, 0.1f, float.PositiveInfinity);
            dmg = (damage / defense);
            if(dmg > MinDamage) { return dmg; }
            else { return 0; }
        }
        else if(damageType == HitboxData.DamageValueType.TrueDamage) // TRUE DAMAGE (BYPASS DEFENSE)
        {
            return damage;
        }
        else
        {
            return 0;
        }
    }

    // COMBO
    public void ComboManager()
    {
        // REMOVE ATTACK IDs OVER TIME
        ComboListCounter += Time.fixedDeltaTime;
        if(ComboListCounter > ComboAttackIDRemovalTime)
        {
            ComboListCounter = 0;
            if (AttacksHit.Count > 0) { AttacksHit.RemoveAt(0); }
        }

        if (AttacksHit.Count > AttacksMax) { AttacksHit.RemoveAt(0); }
    }

    public void AddComboAttack(HitboxData hit)
    {
        // CALCULATE PENALTY, CHECK IF SAME ID EXISTS
        ComboPenalty = 1;
        ID = hit.GetInstanceID();
        for (int i = 0; i < AttacksHit.Count; i++)
        {
            if(ID == AttacksHit[i]) { ComboPenalty += ComboPenaltySize; }
        }

        ComboTime = ComboInterval / ComboPenalty;
        Combo += ComboAddOnHit / ComboPenalty;
        ComboListCounter = 0;
        AttacksHit.Add(hit.GetInstanceID());
    }
}
