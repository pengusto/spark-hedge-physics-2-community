using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PitRespawner : MonoBehaviour
{
    [Header("Reference")]
    public CharacterPhysics Char;
    public CharacterActions Actions;
    public CharacterInput Inp;
    public CharacterCombat Combat;
    public AudioSource FallSFX;

    [Header("Parameters")]
    public float PositionGetRate = 3; 
    public int PositionsMax = 8;
    public SimpleCameraFxParams FX;
    public bool Shake = false;
    public bool Slow = false;
    public float TimeToRestartAfterFalling = 1;

    // CACHE
    public bool Resetting = false;
    public List<Vector3> Positions = new List<Vector3>();
    Coroutine ResetCoroutine;
    public CheckpointPosition checkpoint;

    // POINTS
    private void Start()
    {
        Positions.Add(transform.position);
        InvokeRepeating("FakeUpdate", PositionGetRate, PositionGetRate);

        // CREATE CHECKPOINT
        checkpoint = new CheckpointPosition();
        checkpoint.position = transform.position;
        checkpoint.direction = transform.forward;
    }

    void FakeUpdate()
    {
        // CHECK IF MOVING PLATFORM & OTHER STUFF
        if (Inp.Player == false) { CancelInvoke(); this.enabled = false; }
        if (Char.Grounded == false) { return; }
        if (Char.Surface != null && Char.Surface.MovingPlatform == true) { return; }

        // CHECK IF NORMALLY MOVING, OR ELSE DONT ADD NEW POINTS
        if(Actions.Action != 0) { return; }

        if (Positions.Count > PositionsMax)
        {
            Positions.RemoveAt(0);
            Positions.Add(transform.position);
        }
        else
        {
            Positions.Add(transform.position);
        }
    }

    // PIT CHECK
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PitCollider")
        {
            if (Inp.Player == true)
            {
                FallOnPit(col);
            }
            else
            {
                Actions.Interactions.Hp = -10000;
            }
        }
    }

    public void FallOnPit(Collider col)
    {
        if (Resetting == false)
        {
            if (ResetCoroutine != null) { StopCoroutine(ResetCoroutine); }
            ResetCoroutine = StartCoroutine(ResetPlayer(TimeToRestartAfterFalling));

            // HITBOX
            Combat.anim.SetTrigger("Damage_Fly");
            Combat.HurtFlying = true;
            HitboxData hit = new HitboxData();
            if (col != null)
            {
                hit.HitboxCol = col;
                hit.Parent = col.transform;
            }
            hit.ForceStagger = false;
            hit.NegateSuperArmour = true;
            hit.FriendlyFire = true;
            hit.FakeCenter = Char.transform;
            hit.PhysicalDamage = 10;
            hit.ElementalDamage = 0;
            hit.ApDamage = 1000000000;
            hit.AwayForce = 0;
            hit.UpwardForce = 0;
            hit.ForceType = HitboxData.DamageForceType.Additive;
            hit.StaggerType = HitboxData.DamageStaggerType.Strong;
            hit.DamageType = HitboxData.DamageValueType.TrueDamage;
            hit.EffectsOnPlayerHit = FX;
            Actions.Interactions.ApplyHit(hit);

            // FX
            CharacterCamera.Main.CharUiRefs.FallFade.gameObject.SetActive(true);
            CharacterCamera.Main.CharUiRefs.FallFade.SetTrigger("Start");
            if (FallSFX) { FallSFX.Play(); }
            if (Shake)
            {
                CharacterCamera.ShakeCameraAddtive(FX.ShakeDuration, FX.ShakeAmplitude, transform.position);
            }
            if (Slow)
            {
                CharacterCamera.SlowDown(FX.SlowDownDuration, FX.SlowDownTime,
                    FX.SlowDownRestoreSpeed, transform.position);
            }
        }
    }

    public IEnumerator ResetPlayer(float time)
    {
        Resetting = true;
        yield return new WaitForSeconds(time);

        // RESETTING
        Char.rigid.linearVelocity = Vector3.zero;
        Char.transform.position = Positions[Positions.Count - 1];
        if(Positions.Count > 2) { Positions.RemoveAt(Positions.Count - 1); }
        Resetting = false; 
    }

    // DEBUG

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < Positions.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Positions[i], 0.1f);
        }
    }

    public class CheckpointPosition
    {
        public GameObject RestartReferenceObject;
        public Vector3 position;
        public Vector3 direction;
    }
}
