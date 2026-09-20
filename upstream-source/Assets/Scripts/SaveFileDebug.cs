using UnityEngine;

public class SaveFileDebug : MonoBehaviour
{

    public UnityEngine.UI.Text txt;
    GameProgress g;
    //Rewired.Player Inp;
    string s;
    public bool VisualiseArcade;
    public bool VisualiseStoryItens;
    public bool VisualiseStoryFlags;
    public bool VisualiseCar;
    public bool VisualiseMisc;
    public float MoveSpeed = 5;

    [Header(" EXTRA DEBUG ")]
    public bool UnlockEveryCarPart = false;

    private void Update()
    {
        if(g == null)
        {
            g = SaveData.Data;
            //Inp = Rewired.ReInput.players.GetPlayer(0);
            InvokeRepeating("UpdateText", 0.05f, 0.5f);
        }
        else
        {
            //if (Inp.GetButton("R1") || Inp.GetButton("L1"))
            //{
            //    txt.rectTransform.position += Vector3.up * Inp.GetAxis("RightAnalogY") * MoveSpeed;
            //    if (Inp.GetButton("B")) { gameObject.SetActive(false); }
            //}

        }

        if (UnlockEveryCarPart)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                Debug.Log("DEBUG ACTION: Unlocked every car part!");
                for (int i = 0; i < g.CarVariations.Count; i++)
                {
                    for (int j = 0; j < g.CarVariations[i].Parts.Count; j++)
                    {
                        g.CarVariations[i].Parts[j].Accquired = true;
                        g.CarVariations[i].Parts[j].Unlocked = true;
                    }
                }

                g.FindIten(g.StoryFlags, "Kes_Sponsor01_01").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor01_02").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor01_03").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor02_01").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor02_02").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor02_03").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor03_01").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor03_02").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor03_03").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor04_01").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor04_02").unlocked = true;
                g.FindIten(g.StoryFlags, "Kes_Sponsor04_03").unlocked = true;

            }
        }
    }

    void UpdateText()
    {
        s = "[ SAVE DATA DEBUG VIEW ]";
        s += "Version:" + g.GameVersion + " / Real Version: " + g.ActualGameVersion + "\n";

        s += "--- \n";

        if (VisualiseArcade) 
        {
            if (g.Arcade.Count <= 0) { s += "[ No Arcade Data ] + \n"; }
            for (int i = 0; i < g.Arcade.Count; i++)
            {
                s += g.Arcade[i].name + " [ Unlocked: " + Checkbool(g.Arcade[i].unlocked) +
                    "]|[ Notify: " + Checkbool(g.Arcade[i].notify) + " ]\n";
            }
        }

        s += "--- \n";

        if (VisualiseStoryFlags)
        {
            if(g.StoryFlags.Count <= 0) { s += "[ No Story Flag Data ] + \n"; }
            for (int i = 0; i < g.StoryFlags.Count; i++)
            {
                s += g.StoryFlags[i].name + " [ Unlocked: " + Checkbool(g.StoryFlags[i].unlocked) +
                    "]|[ Notify: " + Checkbool(g.StoryFlags[i].notify) + " ]\n";
            }
        }

        s += "--- \n";

        if (VisualiseStoryItens)
        {
            if (g.StoryItens.Count <= 0) { s += " [ No Story Iten Data ] + \n"; }
            for (int i = 0; i < g.StoryItens.Count; i++)
            {
                s += g.StoryItens[i].name + " [ Unlocked: " + Checkbool(g.StoryItens[i].unlocked) +
                    " ]|[ Ammount: " + g.StoryItens[i].ammount + " ]\n";
            }
        }

        s += "--- \n";
        // CAR STUFF

        if (VisualiseCar)
        {
            if (g.CarVariations.Count <= 0) { s += " [ No Car Data ] + \n"; }
            for (int i = 0; i < g.CarVariations.Count; i++)
            {
                s += g.CarVariations[i].ID + " |" + g.CarVariations[i].VehicleName + "\n";
                s += "Paint ID: " + g.CarVariations[i].CurrentPaint + "\n";

                if (g.CarVariations[i].Parts.Count <= 0) { s += " [ No Car Data ] + \n"; }
                for (int j = 0; j < g.CarVariations[i].Parts.Count; j++)
                {
                    s += g.CarVariations[i].Parts[j].ID +
                        " [ " + g.CarVariations[i].Parts[j].Name + " ]" +
                        "|[ Unlocked: " + Checkbool(g.CarVariations[i].Parts[j].Unlocked) + " ]" +
                        "|[ Got: " + Checkbool(g.CarVariations[i].Parts[j].Accquired) + " ]" +
                        "|[ Equip: " + Checkbool(g.CarVariations[i].Parts[j].Equiped) + " ] \n";
                }

                s += "--- \n";
            }
        }

        // SET TXT
        txt.text = s;
    }

    public string Checkbool(bool b)
    {
        if (b)  { return "X"; }
        else    { return "-"; }
    }
}
