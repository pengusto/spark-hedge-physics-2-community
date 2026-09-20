using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public enum SpringType { Set, SetUp,  Add };
    public enum SecondarySpringType { SetPosition, NoSet }

    [Header("References")]
    public Rigidbody rigid;
    public AudioSource SpringAudio;
    public Animator anim;
    public GameObject particles;

    [Header("Parameters")]
    public SpringType type;
    public SecondarySpringType secondaryType;
    public float ActivationInterval = 0.032f;
    public bool ForceAirAnimation = true;
    public bool SetRotation = false;
    public float SpringForce = 10;
    public Transform SpringDirection;
    public Transform SprungPosition;
    public float ControlLockTime = 0.1f;
    public float AdditionMagnitude = 60;

    [Header("Spring Misc")]
    public bool UseSpringColor = false;
    public Gradient SpringLightColor;
    public float SpringColorRemap = 0.1f;
    public MeshRenderer[] SpringMaterials;
    Color EvaluatedColor;

    [Header("Debug / Cache")]
    public bool EnableDebug = true;
    public Vector3 DebugGravity = new Vector3(0, -3.7f, 0);
    public int DebugIterations = 128;
    public float DebugTime = 0.1f;
    Rigidbody playerRigid;
    CharacterActions playerActions;
    public bool Usable = true;

    private void Start()
    {
        if (UseSpringColor)
        {
            EvaluatedColor = SpringLightColor.Evaluate(SpringForce * SpringColorRemap);
            for (int i = 0; i < SpringMaterials.Length; i++)
            {
                SpringMaterials[i].material.SetColor("_EmissiveColor", EvaluatedColor);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (Usable && col.attachedRigidbody != null)
        {
            if (col.attachedRigidbody.CompareTag("Player"))
            {
                col.attachedRigidbody.TryGetComponent<Rigidbody>(out playerRigid);
                col.attachedRigidbody.TryGetComponent<CharacterActions>(out playerActions);
                if (playerRigid != null && playerActions != null)
                {
                    // STOP INPUT LOCK IF ALREADY RUNNING SO IT DOES NOT HAVE TO PLAY TWO AT A TIME
                    if (playerActions.Inp.DisableInputCoroutine != null) { StopCoroutine(playerActions.Inp.DisableInputCoroutine); }
                    // SET COROUTINE AND DISABLE INPUT FOR A TIME
                    playerActions.Inp.DisableInputCoroutine = playerActions.Inp.DisableInputForATime(ControlLockTime);
                    playerActions.Inp.StartCoroutine(playerActions.Inp.DisableInputCoroutine);
                    OnSpringAction(playerRigid, playerActions);
                }

                playerRigid = null;
                playerActions = null;
                StartCoroutine(DelayedActivation(ActivationInterval));
            }
        }
    }

    public void OnSpringAction(Rigidbody player, CharacterActions actions)
    {
        // DO SPRING
        if (type == SpringType.Set)
        {
            player.linearVelocity = SpringDirection.forward * SpringForce;
            actions.SwitchAction(0);
        }
        else if (type == SpringType.SetUp)
        {
            Vector3 s = rigid.transform.InverseTransformDirection(player.linearVelocity);
            s.y = SpringForce;
            s = rigid.transform.TransformDirection(s);
            player.linearVelocity = s;
            actions.SwitchAction(0);
        }
        else if (type == SpringType.Add)
        {
            player.linearVelocity = Vector3.ProjectOnPlane(player.linearVelocity, -SpringDirection.forward);
            player.linearVelocity += SpringDirection.forward * SpringForce;
            player.linearVelocity = Vector3.ClampMagnitude(player.linearVelocity, AdditionMagnitude);
        }

        // SUBTYPE
        if(secondaryType == SecondarySpringType.SetPosition) { player.position = SprungPosition.position; }

        // CHARACTER CHANGES
        if (actions.Inp.Player)
        {
            actions.SwitchAction(0);
            actions.Basic.SubAction = 0;
            actions.Basic.DashAvailable = true;
            actions.Basic.AirDashAvailable = true;
            if (actions.Spc) { actions.Spc.ChargedJesterDashAvailable = true; }
        }

        if (SetRotation)
        {
            Vector3 d = Vector3.ProjectOnPlane(SpringDirection.forward, -actions.Char.GravityDir);
            actions.Basic.SkinRotation(d, -actions.Char.GravityDir, 9999999, 0);
            actions.Basic.prevInput = d;
        }

        // EXTRA
        if (ForceAirAnimation)
        {
            actions.Basic.airTime = actions.Basic.CoyoteTime + 0.01f;
            actions.Char.CheckGroundTime = -0.1f;
            actions.Basic.anim.SetTrigger("Dash");
        }
        if (SpringAudio) { SpringAudio.Play(); }
        if (anim) { anim.SetTrigger("spring"); }
        if (particles) { IHB.StartIHB(particles); }

    }

    private void OnDrawGizmosSelected()
    {
        if (EnableDebug)
        {
            Gizmos.color = Color.red;
            Vector3 speed = SpringDirection.forward * SpringForce;
            Vector3 grav = DebugGravity;
            Vector3 pos = SprungPosition.position;
            float delta = DebugTime;
            float currentdelta = 0;
            Gizmos.DrawSphere(pos, 0.1f);
            for (int i = 0; i < DebugIterations; i++)
            {
                currentdelta = delta * i;
                speed += grav * delta;
                pos += speed * delta;
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
    }

    IEnumerator DelayedActivation(float time)
    {
        Usable = false;
        yield return new WaitForSeconds(time);
        Usable = true;
    }
}
