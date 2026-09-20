using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using System.Text;

public class MapIconClosed : MonoBehaviour
{
    [Header("References")]
    public Collider IconCollision;

    [Header("Parameters")]
    public bool IsPG = false;
    public string[] DaysClosed = new string[0];
    public bool IsNogo = false;
    public int Rate = 30;
    public int SeedModifier = 10;

    [Header("Cache")]
    public GameProgress Data;
    ProgressIten p;
    ProgressIten week;
    public int Seed;
    public int Result;
    System.Random random;

    private void Update()
    {
        // DATA
        Data = SaveData.Data;
        p = Data.FindIten(Data.StoryFlags, "Day");
        week = Data.FindIten(Data.StoryFlags, "Weekday");

        // DO
        if (IsPG)
        {
            gameObject.SetActive(false);
            for (int i = 0; i < DaysClosed.Length; i++)
            {
                if(week.FlavorText == DaysClosed[i])
                {
                    IconCollision.enabled = false;
                    gameObject.SetActive(true);
                }
            }
        }
        else if(IsNogo)
        {
            Seed = Mathf.RoundToInt(SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Day").ammount +
                                       SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Month").ammount);
            random = new System.Random(Seed + SeedModifier);
            Result = random.Next(0, 100);

            if(Result < Rate)
            {
                IconCollision.enabled = false;
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }       
        }

        this.enabled = false;
    }

}
