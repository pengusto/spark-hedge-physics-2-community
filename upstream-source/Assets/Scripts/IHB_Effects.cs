using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IHB_Effects : MonoBehaviour
{
    [Header("Particle Related")]
    public ParticleSystem[] Particles;

    [Header("Camera")]
    public bool Shake = false;
    public bool Slow = false;
    public SimpleCameraFxParams ShakeAndSlow;

    [Header("Audio Related")]
    public AudioSource[] Sources;
    public bool RandomPitch = false;
    public Vector2 RandomPitchRange = new Vector2(0.8f, 0.95f);
    public bool StopAfterTime = false;
    public float StopTime = 1;

    [Header("Emission")]
    public bool InstantiateObject = false;
    public GameObject[] ObjectsToInstantiate;
    public Transform[] InstantiatedPositions;
    public ProjectileInfo[] ObjInfo;
    GameObject g;
    Projectile p;
    EntityInfo f;

    [Header("EnableDisable")]
    public bool EnableOrDisable = false;
    public GameObject[] ObjectsToEnableOrDisable;
    public bool[] ObjectsToEnableBool;

    [Header("IHB")]
    public bool OtherIHB = false;
    public float IHB_TimeInterval = 0.1f;
    public GameObject OtherIHB_ToPlay;

    [Header("Teleport")]
    public bool Teleport = false;
    public bool TeleportToTarget = false;
    public float TeleportTargetDistance = 0.4f;
    public float TeleportQuickDelay = 0.1f;
    public GameObject TeleportIHB;
    public int TeleportCurrentIndex = 0;
    public Transform[] TeleportPositions;
    public bool TeleportKeepY = true;

    [Header("Optional References")]
    public GameObject ParentObject;
    public CharacterActions Actions;

    [Header("Special")]
    public bool CarDashAttackAfterimage = false;
    public ParticleSystem CarDashParticle;

    public void PlayIHBEffects()
    {
        // CAMERA STUFF
        if (Shake)
        {
            CharacterCamera.ShakeCamera(ShakeAndSlow.ShakeDuration, ShakeAndSlow.ShakeAmplitude, transform.position);
        }

        if (Slow)
        {
            CharacterCamera.SlowDown(ShakeAndSlow.SlowDownDuration, ShakeAndSlow.SlowDownTime, 
                ShakeAndSlow.SlowDownRestoreSpeed, transform.position);
        }

        // AUDIO
        if (RandomPitch)
        {
            for (int i = 0; i < Sources.Length; i++)
            { Sources[i].pitch = Random.Range(RandomPitchRange.x, RandomPitchRange.y); }
        }
        if (StopAfterTime) { StartCoroutine(StopAudioAfterTime(Sources[0], StopTime)); }

        // OBJECT CREATOR
        if (InstantiateObject)
        {
            for (int i = 0; (i < ObjectsToInstantiate.Length) && (i < InstantiatedPositions.Length); i++)
            {
                g = GameObject.Instantiate(ObjectsToInstantiate[i]);
                g.transform.position = InstantiatedPositions[i].transform.position;
                g.transform.rotation = InstantiatedPositions[i].transform.rotation;

                if (ObjInfo.Length > 0)
                {
                    if (g.TryGetComponent<Projectile>(out p))
                    {
                        p.MovementSpeed += ObjInfo[i].ForwardSpeed * InstantiatedPositions[i].transform.forward;
                        p.MovementSpeed += ObjInfo[i].RightSpeed * InstantiatedPositions[i].transform.right;
                        p.MovementSpeed += ObjInfo[i].UpSpeed * InstantiatedPositions[i].transform.up;
                        p.Gravity *= ObjInfo[i].Gravity;
                        for (int j = 0; j < ObjInfo[i].TargetFactions.Length; j++)
                        {
                            f = new EntityInfo();
                            f.FactionName = ObjInfo[i].TargetFactions[j];
                            p.TargetFactions.Add(f);
                        }
                    }
                }
            }
        }

        // OBJECT ENABLED
        if(EnableOrDisable == true)
        {
            for (int i = 0; i < ObjectsToEnableOrDisable.Length; i++)
            {
                ObjectsToEnableOrDisable[i].SetActive(ObjectsToEnableBool[i]);
            }
        }

        // SEQUENCE IHB
        if (OtherIHB)
        {
            StartCoroutine(PlayIHB_AfterTime(IHB_TimeInterval));
        }

        // TELEPORT
        if (Teleport)
        {
            Vector3 position = Vector3.zero;
            Vector3 dir = Vector3.zero;
            if (TeleportCurrentIndex >= TeleportPositions.Length) { TeleportCurrentIndex = 0; }

            if (TeleportToTarget)
            {
                if(Actions.Inp.CurrentTarget != null) 
                { 
                    dir = (ParentObject.transform.position - Actions.Inp.CurrentTarget.transform.position).normalized;
                    position = Actions.Inp.CurrentTarget.transform.position + (dir * -TeleportTargetDistance);
                }
            }
            else
            {
                position = TeleportPositions[TeleportCurrentIndex].position;     
            }

            if (TeleportKeepY) { position.y = ParentObject.transform.position.y; }

            StartCoroutine(TeleportRoutine(TeleportQuickDelay, position, ParentObject));
            TeleportCurrentIndex++;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator StopAudioAfterTime(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        source.Stop();
    }

    [System.Serializable]
    public class ProjectileInfo
    {
        public float ForwardSpeed = 0;
        public float RightSpeed = 0;
        public float UpSpeed = 0;
        public float Gravity = 1;
        public string[] TargetFactions;
    }

    IEnumerator PlayIHB_AfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        IHB.StartIHB(OtherIHB_ToPlay);
    }

    IEnumerator TeleportRoutine(float delay, Vector3 position, GameObject telepotedobject)
    {
        IHB.StartIHB(TeleportIHB);
        yield return new WaitForSeconds(delay);
        IHB.StartIHB(TeleportIHB);
        telepotedobject.transform.position = position;
    }
}
