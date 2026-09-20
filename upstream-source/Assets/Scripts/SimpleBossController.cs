using Unity.VisualScripting;
using UnityEngine;

public class SimpleBossController : MonoBehaviour
{
    [Header("References")]
    public string NextScene;
    public string CurrentScene;
    public static SimpleBossController Instance;

    [Header("Characters")]
    public CharacterActions Player;
    public CharacterActions BossCharacter;

    [Header("Special")]
    public bool UseMultipleHealthBars = false;

    [Header("End Conditions")]
    public bool TimeLimit = false;
    public float SecondsLeft = 120f;
    public GameObject[] ToActivateOnTimer;
    public bool EndsWhenBossHP_ReachesZero = false;
    public float BossFightStartTime = 2;
    public GameObject[] ToActivateOnEnd;
    public bool EndFightWhenBossHpIsBelowAmm = false;
    public float EndFightAmm = -1f;

    [Header("Cache")]
    public bool EndTrigger = false;
    public bool Fighting = false;
    public float Counter = 0;

    private void Start()
    {
        if (Player) { Player.Basic.Inp.InputEnabled = false; }
        if (BossCharacter) 
        {
            BossCharacter.Basic.Inp.InputEnabled = false;
            BossCharacter.Basic.Inp.ai.Agressive = false;
            BossCharacter.Basic.Inp.ai.PathFinding = false;
        }

        if (UseMultipleHealthBars)
        {
            Player.Interactions.MultipleHPBars = true;
            Player.Interactions.MultipleHPBarsAmmount = 
                (int)SaveData.Data.FindIten(SaveData.Data.StoryItens, "Iten_Reset").ammount;
        }

        Instance = this;
    }

    public void Update()
    {
        // PRE FIGHT
        if(Counter < BossFightStartTime)
        {
            Counter += Time.deltaTime;
        }
        else
        {
            // START FIGHT
            Counter += 0.1f;
            Fighting = true;
            if (Player) { Player.Basic.Inp.InputEnabled = true; }
            if (BossCharacter)
            {
                BossCharacter.Basic.Inp.InputEnabled = true;
                BossCharacter.Basic.Inp.ai.Agressive = true;
                BossCharacter.Basic.Inp.ai.PathFinding = true;
            }
        }

        if (Fighting)
        {
            // END CONDITIONS
            if (EndsWhenBossHP_ReachesZero)
            {
                if (BossCharacter)
                {
                    if(BossCharacter.Interactions.Hp < 0.0f)
                    {
                        EndTrigger = true;
                        Fighting = false;
                        for (int i = 0; i < ToActivateOnEnd.Length; i++)
                        {
                            ToActivateOnEnd[i].SetActive(true);
                        }
                    }
                }
            }
            if (TimeLimit)
            {
                if(Player.Interactions.Hp > 0.0f)
                {
                    SecondsLeft -= Time.deltaTime;
                    if (SecondsLeft < 0.0f)
                    {
                        EndTrigger = true;
                        Fighting = false;
                        for (int i = 0; i < ToActivateOnTimer.Length; i++)
                        {
                            ToActivateOnTimer[i].SetActive(true);
                        }
                    }
                }
            }
            if (EndFightWhenBossHpIsBelowAmm)
            {
                if (BossCharacter.Interactions.Hp < EndFightAmm)
                {
                    EndTrigger = true;
                    Fighting = false;
                    for (int i = 0; i < ToActivateOnEnd.Length; i++)
                    {
                        ToActivateOnEnd[i].SetActive(true);
                    }
                }
            }
        }

        if (EndTrigger)
        {

        }
    }

}
