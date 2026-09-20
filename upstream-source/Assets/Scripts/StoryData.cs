using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryData : MonoBehaviour
{
    public static StoryData Instance;
    public bool StoryMode = false;
    public List<CharacterInfo> Characters = new List<CharacterInfo>();

    [Header("Debug")]
    public bool Debug = false;
    public string SceneToResetTo = "";
    public int ForcedDay = 0;
    public bool ForceStoryMode = false;

    [Header("Cache")]
    public int Day = 0;
    public static GameProgress Data;
    static ProgressIten p;

    public void Awake()
    {
        if (Instance == null) { Instance = this; }

        // INITIATE STORY MODE DATA
        InitiateStoryData();
        Day = Mathf.RoundToInt(Data.FindIten(Data.StoryFlags, "Day").ammount);

        // INITATE DEBUG
        if (Debug)
        {
            if (ForceStoryMode) { StoryMode = true; }
            Day = ForcedDay;
        }
    }

    private void Update()
    {
        if (Debug)
        {
            if (Input.GetKeyDown(KeyCode.R)) { SceneController.LoadStageLoading(SceneToResetTo); }
        }
    }

    public static CharacterInfo GetCharacterDetails(Dialog.StoryCharacter character)
    {
        switch (character)
        {
            case Dialog.StoryCharacter.Null:
                return FindCharacter("Null");
                break;
            case Dialog.StoryCharacter.Shell:
                return FindCharacter("Shell");
                break;
            case Dialog.StoryCharacter.Shoe:
                return FindCharacter("Shoe");
                break;
            case Dialog.StoryCharacter.DigiGirl:
                return FindCharacter("Digi");
                break;
            case Dialog.StoryCharacter.DigiBot:
                return FindCharacter("DigiBot");
                break;
            case Dialog.StoryCharacter.Cingul:
                return FindCharacter("Cingul");
                break;
            case Dialog.StoryCharacter.Narrator:
                return FindCharacter("Narrator");
                break;
            case Dialog.StoryCharacter.Information:
                return FindCharacter("Information");
                break;
            case Dialog.StoryCharacter.Individual:
                return FindCharacter("Individual");
                break;
            case Dialog.StoryCharacter.Vespa:
                return FindCharacter("Vespa");
                break;
            case Dialog.StoryCharacter.Wolfman:
                return FindCharacter("Wolfman");
                break;
            case Dialog.StoryCharacter.Chrome:
                return FindCharacter("Chrome");
                break;
            case Dialog.StoryCharacter.Hana:
                return FindCharacter("Hana");
                break;
            case Dialog.StoryCharacter.Clark:
                return FindCharacter("Clark");
                break;
            case Dialog.StoryCharacter.Survivor:
                return FindCharacter("The Survivor");
                break;
        }

        return Instance.Characters[0];

        static CharacterInfo FindCharacter(string name)
        {
            for (int i = 0; i < Instance.Characters.Count; i++)
            {
                if (name == Instance.Characters[i].Name)
                {
                    return Instance.Characters[i]; 
                }
            }

            return Instance.Characters[0];
        }
    }

    [System.Serializable]
    public class CharacterInfo
    {
        [Header("Names")]
        public string Name;
        public string RacerName;
        public string RealName;
        public string RealFullName;
        public string ExtraName;
        public string CarName;

        [Header("Other Params")]
        public Color CharacterColor;
        public string Description;

        [Header("Misc")]
        public string[] ExtraText;
    }

    public static void InitiateStoryData()
    {
        // BASIC DATA (flags)
        if (true)
        {
            Data = SaveData.Data;
            if (Data.FindIten(Data.StoryFlags, "RealDay") == null)
            {
                p = new ProgressIten();
                p.name = "RealDay";
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 0;
                Data.StoryFlags.Add(p);
            }

            if (Data.FindIten(Data.StoryFlags, "Day") == null)
            {
                p = new ProgressIten();
                p.name = "Day";
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 1;
                Data.StoryFlags.Add(p);
            }

            Data = SaveData.Data;
            if (Data.FindIten(Data.StoryFlags, "Month") == null)
            {
                p = new ProgressIten();
                p.name = "Month";
                p.FlavorText = "December";
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 12;
                Data.StoryFlags.Add(p);
            }

            Data = SaveData.Data;
            if (Data.FindIten(Data.StoryFlags, "Year") == null)
            {
                p = new ProgressIten();
                p.name = "Year";
                p.FlavorText = "2999";
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 2999;
                Data.StoryFlags.Add(p);
            }

            Data = SaveData.Data;
            if (Data.FindIten(Data.StoryFlags, "Weekday") == null)
            {
                p = new ProgressIten();
                p.name = "Weekday";
                p.FlavorText = "???";
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 0;
                Data.StoryFlags.Add(p);
            }
        }

        // CURRENCY (itens)
        if (true)
        {
            if (Data.FindIten(Data.StoryItens, "Money") == null)
            {
                p = new ProgressIten();
                p.name = "Money";
                p.unlocked = true;
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 100;
                Data.StoryItens.Add(p);
            }

            if (Data.FindIten(Data.StoryItens, "VXP") == null)
            {
                p = new ProgressIten();
                p.name = "VXP";
                p.unlocked = true;
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
        }

        // PLAYER MOVES (itens)
        if (true)
        {
            if (Data.FindIten(Data.StoryItens, "Move_Combo") == null)
            {
                p = new ProgressIten();
                p.name = "Move_Combo"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }

            if (Data.FindIten(Data.StoryItens, "Special_FireJump") == null)
            {
                p = new ProgressIten();
                p.name = "Special_FireJump"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Special_DashBlast") == null)
            {
                p = new ProgressIten();
                p.name = "Special_DashBlast"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Shoulder_Bash") == null)
            {
                p = new ProgressIten();
                p.name = "Shoulder_Bash"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Spin_Kick") == null)
            {
                p = new ProgressIten();
                p.name = "Spin_Kick"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Fireball") == null)
            {
                p = new ProgressIten();
                p.name = "Fireball"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Dash_Slide") == null)
            {
                p = new ProgressIten();
                p.name = "Dash_Slide"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Moment_Blast") == null)
            {
                p = new ProgressIten();
                p.name = "Moment_Blast"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Firespin_Kick") == null)
            {
                p = new ProgressIten();
                p.name = "Firespin_Kick"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Knockout_Punch") == null)
            {
                p = new ProgressIten();
                p.name = "Knockout_Punch"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Power_Kick") == null)
            {
                p = new ProgressIten();
                p.name = "Power_Kick"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Extreme_Ball") == null)
            {
                p = new ProgressIten();
                p.name = "Extreme_Ball"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Plasma_Blast") == null)
            {
                p = new ProgressIten();
                p.name = "Plasma_Blast"; p.unlocked = false; p.ammount = 0;
                Data.StoryItens.Add(p);
            }
        }

        // PLAYER UPGRADES & ITENS (itens)
        if (true)
        {
            if (Data.FindIten(Data.StoryItens, "Shell_HP") == null)
            {
                p = new ProgressIten();
                p.name = "Shell_HP"; p.ammount = 1; Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Shell_AP") == null)
            {
                p = new ProgressIten();
                p.name = "Shell_AP"; p.ammount = 1; Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Shell_POISE") == null)
            {
                p = new ProgressIten();
                p.name = "Shell_POISE"; p.ammount = 0; Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Shell_EN") == null)
            {
                p = new ProgressIten();
                p.name = "Shell_EN"; p.ammount = 1; Data.StoryItens.Add(p);
            }

            // ITENS
            if (Data.FindIten(Data.StoryItens, "Iten_Reset") == null)
            {
                p = new ProgressIten();
                p.name = "Iten_Reset"; p.ammount = 5; Data.StoryItens.Add(p);
            }
        }

        // NOGO STAGES DATA (Itens)
        if (true)
        {
            // UNLOCK ZONE
            if (Data.FindIten(Data.StoryItens, "Nogo_Unlocked") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Unlocked";
                p.unlocked = true; p.unChecked = true;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }

            // STAGES
            if (Data.FindIten(Data.StoryItens, "Nogo_Stage_0") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Stage_0";
                p.unlocked = true; p.unChecked = true;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Nogo_Stage_1") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Stage_1";
                p.unlocked = false; p.unChecked = false;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Nogo_Stage_2") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Stage_2";
                p.unlocked = false; p.unChecked = false;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Nogo_Stage_3") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Stage_3";
                p.unlocked = false; p.unChecked = false;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Nogo_Stage_4") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Stage_4";
                p.unlocked = false; p.unChecked = false;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Nogo_Stage_5") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Stage_5";
                p.unlocked = false; p.unChecked = false;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Nogo_Stage_6") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Stage_6";
                p.unlocked = false; p.unChecked = false;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
            if (Data.FindIten(Data.StoryItens, "Nogo_Stage_7") == null)
            {
                p = new ProgressIten();
                p.name = "Nogo_Stage_7";
                p.unlocked = false; p.unChecked = false;
                p.ammount = 0;
                Data.StoryItens.Add(p);
            }
        }

        // CAR SPONSORS (flags)
        if (true)
        {
            // PACK 1
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor01_01") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor01_01";
                p.UnlockMessage = " (Pack 1 - Level 1) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor01_02") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor01_02";
                p.UnlockMessage = " (Pack 1 - Level 2) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor01_03") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor01_03";
                p.UnlockMessage = " (Pack 1 - Level 3) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }

            // PACK 2
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor02_01") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor02_01";
                p.UnlockMessage = " (Pack 2 - Level 1) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor02_02") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor02_02";
                p.UnlockMessage = " (Pack 2 - Level 2) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor02_03") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor02_03";
                p.UnlockMessage = " (Pack 2 - Level 3) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }

            // PACK 3
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor03_01") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor03_01";
                p.UnlockMessage = " (Pack 3 - Level 1) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor03_02") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor03_02";
                p.UnlockMessage = " (Pack 3 - Level 2) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor03_03") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor03_03";
                p.UnlockMessage = " (Pack 3 - Level 3) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }

            // PACK 4
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor04_01") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor04_01";
                p.UnlockMessage = " (Pack 4 - Level 1) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor04_02") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor04_02";
                p.UnlockMessage = " (Pack 4 - Level 2) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
            if (Data.FindIten(Data.StoryFlags, "Kes_Sponsor04_03") == null)
            {
                p = new ProgressIten(); p.name = "Kes_Sponsor04_03";
                p.UnlockMessage = " (Pack 4 - Level 3) - Sponsor now available on the Parts Shop for free.";
                p.UnlockCriteriaText = "> Unlock Criteria: Gain more points in the Proving Grounds’ night races.";
                p.ammount = 0; p.unlocked = false; Data.StoryFlags.Add(p);
            }
        }

        // GENERAL RACE FLAGS (flags)
        if (true)
        {
            if (Data.FindIten(Data.StoryFlags, "Kes_Hull") == null)
            {
                p = new ProgressIten(); 
                p.name = "Kes_Hull";
                p.unlocked = true;
                p.notify = false;
                p.ammount = 0;
                p.FlavorText = "...";
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 1;
                Data.StoryFlags.Add(p);
            }

            if (Data.FindIten(Data.StoryFlags, "Kes_Fuel") == null)
            {
                p = new ProgressIten();
                p.name = "Kes_Fuel";
                p.unlocked = true;
                p.notify = false;
                p.ammount = 0;
                p.FlavorText = "...";
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 1;
                Data.StoryFlags.Add(p);
            }

            if (Data.FindIten(Data.StoryFlags, "NR_Rank") == null)
            {
                p = new ProgressIten(); 
                p.name = "NR_Rank";
                p.unlocked = true;
                p.notify = false;
                p.ammount = 0;
                p.FlavorText = "E";
                p.UnlockMessage = "...";
                p.UnlockCriteriaText = "...";
                p.ammount = 0; 
                Data.StoryFlags.Add(p);
            }
        }

        // GOALS (flags)
        if (true)
        {
            Data = SaveData.Data;
            if (Data.FindIten(Data.StoryFlags, "Sh_Goal0") == null)
            {
                p = new ProgressIten(); p.name = "Sh_Goal0"; p.active = false;
                p.unlocked = false; Data.StoryFlags.Add(p);
            }

            if (Data.FindIten(Data.StoryFlags, "Sh_Goal1") == null)
            {
                p = new ProgressIten(); p.name = "Sh_Goal1"; p.active = false;
                p.unlocked = false; Data.StoryFlags.Add(p);
            }

            if (Data.FindIten(Data.StoryFlags, "Sh_Goal2") == null)
            {
                p = new ProgressIten(); p.name = "Sh_Goal2"; p.active = false;
                p.unlocked = false; Data.StoryFlags.Add(p);
            }

            if (Data.FindIten(Data.StoryFlags, "Sh_Goal3") == null)
            {
                p = new ProgressIten(); p.name = "Sh_Goal3"; p.active = false;
                p.unlocked = false; Data.StoryFlags.Add(p);
            }

            if (Data.FindIten(Data.StoryFlags, "Sh_Goal4") == null)
            {
                p = new ProgressIten(); p.name = "Sh_Goal4"; p.active = false;
                p.unlocked = false; Data.StoryFlags.Add(p);
            }
        }

    }
}
