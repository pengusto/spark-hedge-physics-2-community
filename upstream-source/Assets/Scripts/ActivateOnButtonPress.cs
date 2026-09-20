using UnityEngine;

public class ActivateOnButtonPress : MonoBehaviour
{
    public string ButtonToPress = "Y";
    public GameObject[] ToActivate;
    public GameObject[] ToDeactivate;
    public Animator[] AnimatorsToDeactivate;
    //public Rewired.Player Inp;

    private void Start()
    {
        //Inp = Rewired.ReInput.players.GetPlayer(0);
    }

    void Update()
    {
        //if (Inp.GetButtonDown(ButtonToPress))
        //{
        //    ActivateObjects();
        //}
    }

    void ActivateObjects()
    {
        for (int i = 0; i < ToActivate.Length; i++)
        {
            ToActivate[i].SetActive(true);
        }

        for (int i = 0; i < ToDeactivate.Length; i++)
        {
            ToDeactivate[i].SetActive(false);
        }
        for (int i = 0; i < AnimatorsToDeactivate.Length; i++)
        {
            AnimatorsToDeactivate[i].enabled = false;
        }
    }
}
