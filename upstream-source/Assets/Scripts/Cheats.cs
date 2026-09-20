using UnityEngine;

public class Cheats : MonoBehaviour
{
    public bool EnableUnlockAllCheat = false;
    public AudioSource ConfirmSound;

    // CACHE
    //public Rewired.Player Inp;
    ProgressIten p;

    private void Start()
    {
        //Inp = Rewired.ReInput.players.GetPlayer(0);
    }

    void Update()
    {
        if (EnableUnlockAllCheat)
        {
            //if (Inp.GetButton("X") && Inp.GetButton("Y") && Inp.GetButton("L1") && Inp.GetButton("Start"))
            //{
            //    for (int i = 0; i < SaveData.Data.Arcade.Count; i++)
            //    {
            //        SaveData.Data.Arcade[i].unlocked = true;
            //    }

            //    if (ConfirmSound) { ConfirmSound.Play(); }
            //    EnableUnlockAllCheat = false;
            //}

            //if(Inp.GetButton("X") && Inp.GetButton("L1") && Inp.GetButton("L2"))
            //{
            //    // CHECK IF PG UNLOCKED SOMETHING
            //    ProgressIten p2 = SaveData.Data.FindIten(SaveData.Data.Arcade, "Mercury Cup");
            //    if (p2 != null && !p2.unlocked)
            //    {
            //        if (ConfirmSound) { ConfirmSound.Play(); }
            //        p2.unlocked = true;
            //    }
            //}

            // KEYBOARD

            if (Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.K))
            {
                for (int i = 0; i < SaveData.Data.Arcade.Count; i++)
                {
                    SaveData.Data.Arcade[i].unlocked = true;
                }

                if (ConfirmSound) { ConfirmSound.Play(); }
                EnableUnlockAllCheat = false;
            }

            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.U))
            {
                // CHECK IF PG UNLOCKED SOMETHING
                ProgressIten p2 = SaveData.Data.FindIten(SaveData.Data.Arcade, "Mercury Cup");
                if (p2 != null && !p2.unlocked)
                {
                    if (ConfirmSound) { ConfirmSound.Play(); }
                    p2.unlocked = true;
                }
            }
        }
    }
}
