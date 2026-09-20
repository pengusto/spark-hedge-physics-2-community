using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Networking;
//using Unity.Netcode;

//public class NetworkDebug : NetworkBehaviour
//{
//    public Text txt;
//    string s;

//    NetworkManager n;

//    void Update()
//    {
//        n = NetworkManager.Singleton;

//        s = "Network Enabled: " + n.enabled + " \n";
//        s += "Server: (" + n.IsServer + "), Server is host? (" + n.ServerIsHost + ")";
//        s += "\n";
//        s += "Host: (" + n.IsHost + ", " + n.ConnectedHostname + ")";
//        s += "\n";
//        s += "Clients: (" + n.ConnectedClients.Count + ")";
//        s += "\n";
//        for (int i = 0; i < n.ConnectedClients.Count; i++)
//        {
//            s += "(" + n.ConnectedClientsIds[i] + ") > ";
//            if (n.ConnectedClientsList[i].PlayerObject) { s += n.ConnectedClientsList[i].PlayerObject.name; }
//            s += "Is Host: (" + n.ConnectedClientsList[i].DAHost + ")";
//            s += "\n";
//        }



//        // DEBUG FUNCTIONS

//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            //GameObject g = Instantiate(n.pre)
//        }

//        // FINALLT SET TEXT

//        txt.text = s;
//    }
//}
