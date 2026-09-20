using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;

//public class AsyncSpawnerTest : MonoBehaviour
//{

//    public GameObject ObjectToInstance;
//    public int ammount = 20;

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.T))
//        {
//            for (int i = 0; i < ammount; i++)
//            {
//                Addressables.LoadAssetAsync<GameObject>("Cars/Base").Completed += (LoadObject) =>
//                {
//                    if (LoadObject.Status == AsyncOperationStatus.Succeeded)
//                    {
//                        Instantiate(LoadObject.Result);
//                    }
//                    else
//                    {
//                        Debug.Log("FAILED TO LOAD!");
//                    }
//                };
//            }
//        }

//        if (Input.GetKeyDown(KeyCode.U))
//        {
//            for (int i = 0; i < ammount; i++)
//            {
//                Instantiate(ObjectToInstance);
//            }
//        }
//    }
//}
