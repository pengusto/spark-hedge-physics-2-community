using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using System.Text;

public class RandomActivator : MonoBehaviour
{
    public GameObject[] obj;
    System.Random random;
    int rand;
    int FinalDateSeed = 0;
    int i;
    int seed;
    bool firstTime = true;
    const int seedbias = 30;
    ActivateOnDistance act;
    public static int DateSeed = -1;

    private void Update()
    {
        if (firstTime == true)
        {
            if (DateSeed == -1)
            {
                DateSeed = Mathf.RoundToInt(SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Day").ammount +
                                        SaveData.Data.FindIten(SaveData.Data.StoryFlags, "Month").ammount);
                Debug.Log("Determening DATE SEED! (" + DateSeed + ")");
            }

            // POSSIBLE RANDOMS
            /*  GetInstanceID()... NO
             *  transform.childCount
             *  gameObject.name.Length
             */

            FinalDateSeed = Mathf.Abs(DateSeed) + GetExtraRandomParamsFromObject(gameObject);
            FinalDateSeed += Mathf.RoundToInt(transform.position.x + transform.position.y + transform.position.z);      
            SetRandom(FinalDateSeed);

            firstTime = false;
            this.enabled = false;
        }
    }

    public int GetExtraRandomParamsFromObject(GameObject g)
    {
        //using var shashy = SHA1.Create();
        //number = BitConverter.ToInt32(shashy.ComputeHash(Encoding.UTF8.GetBytes(g.name)));
        //Debug.Log(number);

        int number = 0;
        float crazy = (g.name.Length * (Mathf.Sin(g.name.Length * 1234567f) * 123f)) * 32f;
        number = Mathf.RoundToInt(crazy);
        return number;
    }

    void SetRandom(int day)
    {
        seed = seedbias + day;
        random = new System.Random(day);
        rand = random.Next(0, obj.Length);

        // DEACTIVATE
        for (i = 0; i < obj.Length; i++) 
        {
            if (i != rand)
            {
                obj[i].SetActive(false);
                if (obj[i].TryGetComponent<ActivateOnDistance>(out act))
                {
                    act.enabled = false;
                    act.CancelInvoke();
                }
            }
        }

        // ENABLE
        if (obj[rand] != null) 
        { 
            obj[rand].SetActive(true);
            if (obj[rand].TryGetComponent<ActivateOnDistance>(out act))
            {
                act.enabled = true;
                act.Start();
            }
        }
    }

    private void OnDestroy()
    {
        DateSeed = -1;
    }


}
