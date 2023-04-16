using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class NetworkTestForCatch : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect Master...");
        PhotonNetwork.JoinOrCreateRoom("CatchTest", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Connect.... And {PhotonNetwork.CurrentRoom.PlayerCount} Player in hear");

        if ( PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.Instantiate("Prefabs/Player/Basic_Chef", Vector3.zero , Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Prefabs/Player/Runner_Beef", Vector3.zero, Quaternion.identity);
        }
    }
}
