using UnityEngine;

public class ResetGiver : MonoBehaviour
{
    public bool GiveReset = true;
    public string Message = "Obtained Reset!";
    public int ResetAmm = 3;
    public bool GiveMoney = false;
    public string MoneyMessage = "Obtained Reset!";
    public float MoneyAmm = 19;
    public bool GiveVXP = false;
    public string VxpMessage = "Obtained Reset!";
    public float VXPamm = 19;

    // CACHE
    public GameProgress Data;
    ProgressIten p;
    ProgressIten p2;

    void Start()
    {
        Data = SaveData.Data;
        p = Data.FindIten(Data.StoryItens, "Iten_Reset");
        if(p != null)
        {
            if (GiveReset)
            {
                LoadingScreen.Messages.Add(Message + " (" + ResetAmm + "x)");
                p.ammount += ResetAmm;
            }
            if (GiveMoney)
            {
                p2 = Data.FindIten(Data.StoryItens, "Money");
                p2.ammount += MoneyAmm;
                LoadingScreen.Messages.Add(MoneyMessage + " (" + MoneyAmm + ")");
            }
            if (GiveVXP)
            {
                p2 = Data.FindIten(Data.StoryItens, "VXP");
                p2.ammount += VXPamm;
                LoadingScreen.Messages.Add(VxpMessage + " (" + VXPamm + ")");
            }
        }
        else
        {
            Debug.Log("VALUE WAS: Null, Cannot add Reset, " + gameObject.name);
        }
    }
}
