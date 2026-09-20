using UnityEngine;
//using Unity.Netcode;
//using Unity.Networking;

//public class OnlinePlayer : NetworkBehaviour
//{
//    [Header("Debug")]
//    public NetworkObject Vehicle;
//    public string VehicleName;

//    [Header("Cache")]
//    public NetworkManager n;
//    public NetworkObject obj;
//    public NetworkClient Client;
//    public bool VehicleFound = false;
//    public CarPhysics Car;
//    public CarInput Inp;
//    public CarObjectInteraction Interaction;
//    public bool isHost = false;
//    public bool isClient = false;


//    void Start()
//    {
//        n = NetworkManager.Singleton;
//        obj = GetComponent<NetworkObject>();
//    }

//    // Script is active for all players, in every instance there are copies of this script

//    void Update()
//    {
//        // LOOK FOR DESIRED VEHICLE AND STOP LOOKING
//        if (VehicleFound == false)
//        {
//            GameObject[] found = GameObject.FindGameObjectsWithTag("Player");
//            for (int i = 0; i < found.Length; i++)
//            {
//                if (found[i].name == VehicleName) { Vehicle = found[i].GetComponent<NetworkObject>(); }
//            }

//            if (Vehicle != null)
//            {
//                Car = Vehicle.GetComponent<CarPhysics>();
//                Inp = Vehicle.GetComponent<CarInput>();
//                Interaction = Vehicle.GetComponent<CarObjectInteraction>();
//                VehicleFound = true;
//            }
//        }

//        // NETWORK LOOP
//        if (VehicleFound)
//        {
//            if (IsOwner)
//            {

//            }
//            else
//            {
//                Inp.Inp = Rewired.ReInput.players.GetPlayer("Network");
//                Inp.NetWorkObject = true;
//            }
//        }
//    }
//}
