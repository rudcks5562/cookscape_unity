using Photon.Pun;
using UnityEngine;

public class NetworkTest : MonoBehaviourPunCallbacks
{
    public GameObject m_Pot;
    public int m_PotSpawnCnt;
    public GameObject m_Valve;
    public int m_ValveSpawnCnt;
    public GameObject[] m_Item;
    public int[] m_ItemCnt;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect Master...");
        PhotonNetwork.JoinOrCreateRoom("MetaFaker", new Photon.Realtime.RoomOptions { MaxPlayers = 5 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Connect....");
        if ( PhotonNetwork.CurrentRoom.PlayerCount == 1 )
        {
            PhotonNetwork.Instantiate("Prefabs/Player/Basic_Chef", GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Prefabs/Player/Runner_Beef", GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Spawner.instance.DoSpawn();
        }
    }
}
