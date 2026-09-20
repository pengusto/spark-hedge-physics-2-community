using System;
using UnityEngine;

public class WorkBonus : MonoBehaviour
{
    [Header("Parameters")]
    public GameObject WorkObject;
    public TextMesh BonusText;
    public int Rate = 50;
    public float StandardBonus = 2;
    public float BigBonus = 4;

    [Header("Cache")]
    public int Seed;
    public int Result;
    public float BonusMultiplier;
    public float FinalBonus;
    System.Random random;


    private void Update()
    {
        // RANDOM
        FinalBonus = 1;
        Seed = Mathf.RoundToInt(SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Day").ammount +
                                       SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Month").ammount);
        Seed += 100;
        random = new System.Random(Seed);
        Result = random.Next(0, 100);

        // MULTIPLIER
        if(Result >= Rate)
        {
            WorkObject.SetActive(true);
            BonusMultiplier = random.Next(60, 100);
            BonusMultiplier /= 100;
            //BonusMultiplier++;

            if(Result >= 80)
            {
                FinalBonus = (float)Math.Round(BigBonus * BonusMultiplier, 1);
                FinalBonus = Mathf.Clamp(FinalBonus, 1.1f, 999f);
                BonusText.text = FinalBonus + "X";
            }
            else
            {
                FinalBonus = (float)Math.Round(StandardBonus * BonusMultiplier, 1);
                FinalBonus = Mathf.Clamp(FinalBonus, 1.1f, 999f);
                BonusText.text = FinalBonus + "X";
            }
        }
        else
        {
            WorkObject.SetActive(false);
        }

        this.enabled = false;
    }
}
