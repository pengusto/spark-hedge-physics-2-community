using NUnit.Framework;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RandomEventMapIcon : MonoBehaviour
{
    [Header("References")]
    public ValuesHolder Values;

    [Header("Parameters")]
    public int MaxRandomEvents = 24;
    public int MaxNonRepeatables = 12;
    public string RandomEventScene = "";
    public string NonRandomEventScene = "";
    public float NonRandomChance = 10;
    public int SeedModifier = 10;

    [Header("Cache")]
    public GameProgress g;
    public ProgressIten repeatIten;
    public ProgressIten day;
    public ProgressIten week;
    public ProgressIten monthIten;
    public System.Random r;
    public int finalRandom = 0;
    public int rand1;
    public int rand2;
    public int rand3;
    int randSeed;

    private void Update()
    {
        g = SaveData.Data;
        day = g.FindIten(g.StoryFlags, "Day");
        week = g.FindIten(g.StoryFlags, "Weekday");
        monthIten = g.FindIten(g.StoryFlags, "Month");

        randSeed = Mathf.RoundToInt(day.ammount + week.ammount + day.FlavorText.Length + monthIten.FlavorText.Length);
        randSeed = randSeed + Mathf.RoundToInt(Mathf.Abs(Mathf.Sin(randSeed * 33.12345f)) * 10000);
        rand1 = randSeed;
        r = new System.Random(rand1);
        rand3 = r.Next(0, 10000);

        rand2 = Mathf.RoundToInt
            (day.ammount + week.ammount + week.FlavorText.Length +
            monthIten.ammount + monthIten.FlavorText.Length
            ) * SeedModifier;
        r = new System.Random(rand1 + rand3);

        Debug.Log("Random day seed value: " + "(" + rand2 + ")" + rand1 + "+" + rand3 + "=" + (rand1 + rand3));
        Debug.Log("Random Seed For Random1: " + randSeed);

        // GET NON RANDOM EV
        if (g.FindIten(g.StoryFlags, "EV_RAND") != null) 
        {
            repeatIten = g.FindIten(g.StoryFlags, "EV_RAND");
        }
        else // CREATE RANDOM PROGRESS
        {
            ProgressIten rand;
            rand = new ProgressIten();
            rand.name = "EV_RAND";
            rand.unlocked = false; day.notify = false;
            rand.ammount = 0;
            rand.UnlockMessage = "";
            g.StoryFlags.Add(rand);
            repeatIten = g.FindIten(g.StoryFlags, "EV_RAND");
        }

        // SET TO GO TO
        finalRandom = r.Next(0, 100);
        Debug.Log("Random day value: " + finalRandom);

        this.enabled = false;
    }

    public void CheckNextScene()
    {
        if (finalRandom < NonRandomChance)
        {
            if (repeatIten.ammount <= MaxNonRepeatables)
            {
                Values.Strings[0] = NonRandomEventScene;
                rand1 = (int)repeatIten.ammount;
                CutsceneRepo.ConvoToPlay = rand1;
                Debug.Log("NonRandom Cutscene: " + rand1);
                repeatIten.ammount += 1;
            }
            else
            {
                SetRandomScene();
            }
        }
        else
        {
            SetRandomScene();
        }
    }

    public void SetRandomScene()
    {
        Values.Strings[0] = RandomEventScene;
        rand1 = r.Next(0, 1);
        rand1 = r.Next(0, 1);
        rand1 = r.Next(0, 999999);
        rand1 = Mathf.FloorToInt(rand1 % MaxRandomEvents + 1);
        rand1 = Mathf.Clamp(rand1, 0, MaxRandomEvents);
        Debug.Log("Random Cutscene: " + rand1);
        CutsceneRepo.ConvoToPlay = rand1;

        // DEBUG
        //float ad = 0;
        //string num = "RANDOM NUMBERS > ";
        //for (int i = 0; i < 1000; i++)
        //{
        //    rand1 = r.Next(0, 999999);
        //    rand1 = Mathf.FloorToInt(rand1 % MaxRandomEvents + 1);
        //    rand1 = Mathf.Clamp(rand1, 0, MaxRandomEvents);
        //    ad += rand1;
        //    num += "," + rand1;
        //}
        //ad = ad / 1000;
        //Debug.Log("DEBUG ADDER RANDOM CHECK: " + ad + ":: " + num);
    }
}
