using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static GameProgress Data;
    ProgressIten p;
    public string GameVersion = "0.3b";
    public string InternalGameVersion = "EaBeta2";
    public bool DebugSaveOnExit = true;

    private void Awake()
    {
        // LOAD DATA
        if (Data == null)
        {
            Debug.Log("Loading Data From SaveData.cs (Awake)");
            Data = LoadProgress();
        }

        // CHECK IF WRONG VERSION AND RESTORE
        if (Data.GameVersion != GameVersion)
        {
            Debug.LogError("Incompatible Save Data: " + Data.GameVersion + "/" + GameVersion + ", Deleting everything.");
            ResetData();
            Data = LoadProgress();
        }

        // INITIAL SETUP
        if (Data.FirstTime)
        {
            Debug.Log("Save, First time setup.");
            Data.GameVersion = GameVersion;
            Data.ActualGameVersion = InternalGameVersion;
            Data.FirstTime = false;
            ArcadeSaveDataSetup();
        }

    }

    private void OnDestroy()
    {
        if(StoryData.Instance.StoryMode == false)
        {
            if (DebugSaveOnExit)
            {
                Debug.LogError("Saving Data On Quit, Disable this on the [Input] global object");
                SaveProgress(Data);
            }
        }
        else
        {
            if (DebugSaveOnExit)
            {
                Debug.LogError("Saving Data On Quit, Disable this on the [Input] global object");
                SaveProgress(Data);
            }
        }
    }

    void ArcadeSaveDataSetup()
    {
        // XF ARCADE ITENS
        p = new ProgressIten();
        p.name = "Mercury Cup";
        p.UnlockMessage = "Mercury Cup, UNLOCKED!";
        p.UnlockCriteriaText = "Complete at least 12 challenges in the Proving Grounds.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "Venus Cup";
        p.UnlockMessage = "Venus Cup, UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Mercury Cup in 3rd place or above.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "Earth Cup";
        p.UnlockMessage = "Earth Cup, UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Mercury Cup in 3rd place or above.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "Moon Cup";
        p.UnlockMessage = "Moon Cup, UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Earth or Venus Cup in 3rd place or above.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "Mars Cup";
        p.UnlockMessage = "Mars Cup, UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Moon Cup in 3rd place or above.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "Jupiter Cup";
        p.UnlockMessage = "Jupiter Cup, UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Mars Cup in 3rd place or above.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "Saturn Cup";
        p.UnlockMessage = "Saturn Cup, UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Jupiter Cup in 3rd place or above.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "Outer Cup";
        p.UnlockMessage = "Outer Cup, UNLOCKED!";
        p.UnlockCriteriaText = " Complete the Saturn Cup in 3rd place or above.";
        Data.Arcade.Add(p);

        // XF TIME TRIAL STUFF
        p = new ProgressIten();
        p.name = "TA - Mercury Tracks";
        p.UnlockMessage = "Time Attack: Mercury Tracks UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Mercury Cup.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "TA - Venus Tracks";
        p.UnlockMessage = "Time Attack: Venus Tracks UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Venus Cup.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "TA - Earth Tracks";
        p.UnlockMessage = "Time Attack: Earth Tracks UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Earth Cup.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "TA - Moon Tracks";
        p.UnlockMessage = "Time Attack: Moon Tracks UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Moon Cup.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "TA - Mars Tracks";
        p.UnlockMessage = "Time Attack: Mars Tracks UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Mars Cup.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "TA - Jupiter System Tracks";
        p.UnlockMessage = "Time Attack: Jupiter Tracks UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Jupiter Cup.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "TA - Saturn System Tracks";
        p.UnlockMessage = "Time Attack: Saturn Tracks UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Saturn Cup.";
        Data.Arcade.Add(p);
        p = new ProgressIten();
        p.name = "TA - Outer System Tracks";
        p.UnlockMessage = "Time Attack: Outer Tracks UNLOCKED!";
        p.UnlockCriteriaText = "Complete the Outer Cup.";
        Data.Arcade.Add(p);

        // XF ARCADE MODE SETUP
        AddIten(Data.Arcade, "PG_Chal1", 
            "Reach a score of 100 on the proving grounds. ", 
            "Proving Grounds - More challenges unlocked!");
        AddIten(Data.Arcade, "PG_Chal2", 
            "Reach a score of 150 on the proving grounds. ", 
            " Proving Grounds - More challenges unlocked!");

        AddIten(Data.Arcade, "A_CR_BlueA",
            "Score 100 on the Proving Grounds or complete the Jupiter Cup on Intense (Top 3) or above.",
            "- Vehicle: Blue Auron, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_Corss",
            "Score 200 on the Proving Grounds or Complete the Mars Cup on Intense (Top 3) or above.",
            "- Vehicle: Crosswinds, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_Puni",
            "Complete the Earth Cup on Intense  (Top 3)",
            "- Vehicle: Pink Punisher, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_Haul",
            "Score 150 on the Proving Ground or Complete the Moon Cup on Intense (Top 3) or above.",
            "- Vehicle: Long Haul, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_Forg",
            "Complete the Saturn Cup.",
            "- Vehicle: Forgery, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_Centau",
            "Score 300 on the Proving Grounds or Complete the Mars cup on Extreme (Top 3) or above.",
            "- Vehicle: Never Centauri, unlocked for arcade mode!");

        AddIten(Data.Arcade, "A_CR_Ferro",
            "Complete the Saturn Cup on Extreme (Top 3) or above.",
            "- Vehicle: Ferronaut, unlocked for arcade mode!");

        AddIten(Data.Arcade, "A_CR_eWave",
            "Complete the Jupiter Cup on Extreme (Top 3) or above.",
            "- Vehicle: eWave, unlocked for arcade mode!");

        AddIten(Data.Arcade, "A_CR_Mk2",
            "Finish the Outer Cup in 3rd place or above.",
            "- Vehicle: Red Kestrel, unlocked for arcade mode!");


        AddIten(Data.Arcade, "A_CR_NPC_Edge",
            "Complete the Earth Cup on Beyond (Top 3) or above.",
            "- Bonus Vehicle: Edge Taurus V, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_NPC_Fata",
            "Complete the Mars Cup on Beyond (Top 3) or above.",
            "- Bonus Vehicle: Tristar Fata GT, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_NPC_Avro",
            "Complete the Venus Cup on Beyond (Top 3) or above.",
            "- Bonus Vehicle: Avronaut Monton, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_NPC_Gar",
            "Complete the Jupiter Cup on Beyond (Top 3) or above.",
            "- Bonus Vehicle: Garcion GT500, unlocked for arcade mode!");
        AddIten(Data.Arcade, "A_CR_NPC_Niz",
            "Complete the Outer Cup on Beyond (Top 3) or above.",
            "- Bonus Vehicle: Nizza R45, unlocked for arcade mode!");

        AddIten(Data.Arcade, "Difi_Special1",
            "Complete the Outer Cup (Top 3) or above.",
            "- New Difficulty: Special Difficulty Unlocked!");



        // XF STORY SETUP
    }

    ProgressIten AddIten(List<ProgressIten> list, string name, string unlockCriteria, string unlockMessage)
    {
        if (Data.FindIten(list, name) == null)
        {
            p = new ProgressIten();
            p.name = name;
            p.UnlockMessage = unlockMessage;
            p.UnlockCriteriaText = unlockCriteria;
            list.Add(p);
        }
        else
        {
            //Debug.LogWarning("Progress iten: " + name + " exists.");
            return null;
        }
        return null;
    }

    // SAVE // LOAD
    public static void SaveProgress(GameProgress prog)
    {
        Debug.Log("Saving Data...");
        System.IO.File.WriteAllText(Application.dataPath + "/Save.gs", JsonUtility.ToJson(prog, true));
    }

    public static GameProgress LoadProgress()
    {
        Debug.Log("LOADING SAVE DATA");
        string s;
        GameProgress g;
        if (System.IO.File.Exists(Application.dataPath + "/Save.gs"))
        {
            // LOAD SAVE FILE
            s = System.IO.File.ReadAllText(Application.dataPath + "/Save.gs");
            if(s == string.Empty) 
            { 
                Debug.LogError("DEBUG: Save data file was empty"); 
                g = new GameProgress(); 
            }

            g = JsonUtility.FromJson<GameProgress>(s);
            return g;
        }
        else
        {
            Debug.Log("NEW SAVE CREATED AT: " + Application.dataPath);
            g = new GameProgress();
            return g;
        }
    }

    public void ResetData()
    {
        // SAVE
        GameProgress g = new GameProgress();
        SaveProgress(g);
    }

    // VEHICLE SAVE
}

// CLASSES

[System.Serializable]
public class GameProgress
{
    public bool FirstTime = true;
    public List<ProgressIten> Arcade = new List<ProgressIten>();
    public List<ProgressIten> StoryFlags = new List<ProgressIten>();
    public List<ProgressIten> StoryItens = new List<ProgressIten>();
    public string GameVersion = "";
    public string ActualGameVersion = "";
    public List<CarPartsData> CarVariations = new List<CarPartsData>();

    public ProgressIten FindIten(List<ProgressIten> p, string name)
    {
        for (int i = 0; i < p.Count; i++)
        {
            if (p[i].name == name) 
            { return p[i]; }
        }
        Debug.LogWarning("DEBUG: No progress iten found: [" + name + "] : if you are trying to " +
            "create a new item in the list and are checking if it still exists, please ignore this warning." +
            " AND MORE: if you're checking if a parameter exists for some other purpose like game progress, ignore this too.");
        return null;
    }
}

[System.Serializable]
public class ProgressIten
{
    public string name = "";
    public string Description = "";
    public string UnlockCriteriaText = "";
    public string UnlockMessage = "";
    public string FlavorText = "";
    public float ammount = 0;
    public bool active = false;
    public bool unlocked = false;
    public bool unChecked = false;
    public bool notify = false;
}

[System.Serializable]
public class CarPartsData
{
    public int ID = -1;
    public string VehicleName = "<insert vehicle name>";
    public string PilotName = "<insert pilot name>";
    public List<Part> Parts = new List<Part>();
    public List<Part> Sponsors = new List<Part>();
    public int CurrentPaint = 0;

    [System.Serializable]
    public class Part
    {
        public int ID = -1;
        public string Name = "unnamed part";
        public string Lore = "<lore>";
        public bool Unlocked = true;
        public bool Accquired = false;
        public bool Equiped = false;
        public float Value = 0;
    }

    public static bool GetCarPart(int ID, List<CarPartsData> c)
    {
        if(c == null) { return false; }
        for (int i = 0; i < c.Count; i++)
        {
            if(c[i].ID == ID) { return true; }
        }
        return false;
    }
}