using UnityEngine;

public class DateActivator : MonoBehaviour
{
    [Header("Parameters")]
    public bool DayRangeCheck = false;
    public Vector2 Days = new Vector2(1, 10);
    public bool MonthNumberCheck = false;
    public int MonthNumber = 99;
    public bool MonthStringCheck = false;
    public string MonthString = "Feb?";
    public bool YearCheck = false;
    public int Year = 2012;

    // CACHE
    [Header("Cache")]
    public GameProgress Data;
    public ProgressIten ProgDay;
    public ProgressIten ProgMonth;
    public ProgressIten ProgYear;
    public int Checks;
    public int TrueChecks;

    public void Update()
    {
        // NOTE: do the update thick in another script
        // GET DATA
        Data = SaveData.Data;
        ProgDay = Data.FindIten(Data.StoryFlags, "Day");
        ProgMonth = Data.FindIten(Data.StoryFlags, "Month");
        ProgYear = Data.FindIten(Data.StoryFlags, "Year");

        Debug.Log("DAY CHECK: " + ProgDay.ammount + "/" + ProgMonth.FlavorText + "(" + ProgMonth.ammount + ") /" + ProgYear.ammount
            + "(" + ProgYear.Description + ")");

        Checks = 0;
        TrueChecks = 0;

        // CHECK
        if (DayRangeCheck)
        {
            Checks++;
            int d = Mathf.RoundToInt(ProgDay.ammount);
            if (d >= Days.x && d <= Days.y) { TrueChecks++; }
        }

        if (MonthNumberCheck)
        {
            Checks++;
            int m = Mathf.RoundToInt(ProgMonth.ammount);
            if(m == MonthNumber) { TrueChecks++; }
        }

        if (MonthStringCheck)
        {
            Checks++;
            string m = ProgMonth.FlavorText;
            if (m == MonthString) { TrueChecks++; }
        }

        if (YearCheck)
        {
            Checks++;
            int y = Mathf.RoundToInt(ProgYear.ammount);
            if(y == Year) { TrueChecks++; }
        }

        //END
        gameObject.SetActive(TrueChecks >= Checks);
        this.enabled = false;
    }
}
