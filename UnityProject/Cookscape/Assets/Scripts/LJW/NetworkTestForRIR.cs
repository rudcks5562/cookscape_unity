using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;

public class NetworkTestForRIR : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public Dictionary<string, RoomInfo> m_Rooms = new Dictionary<string, RoomInfo>();
    public static NetworkTestForRIR Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Screen.SetResolution(720, 540, false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect Master...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Come In...");

        Hashtable customRoomProperties = new()
        {
            { "status", "Metaverse" }
        };
        PhotonNetwork.JoinOrCreateRoom("FIRST_ROOM", new RoomOptions { MaxPlayers = 20, CustomRoomProperties = customRoomProperties }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Connect.... And {PhotonNetwork.CurrentRoom.PlayerCount} Player in hear");

        PhotonNetwork.Instantiate("Prefabs/Player/Runner_Beef", Vector3.zero, Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"See New Player {newPlayer}");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomInfo tmpRoom = null;

        roomList.ForEach(r =>
        {
            if ( r.RemovedFromList)
            {
                m_Rooms.TryGetValue(r.Name, out tmpRoom);
                //Destroy(tmpRoom);
                m_Rooms.Remove(r.Name);
            }
            else
            {
                if ( !m_Rooms.ContainsKey(r.Name) )
                {
                    //�߰� �ʿ�
                    m_Rooms[r.Name] = r;
                }
                else
                {
                    //���� �ʿ�
                    m_Rooms[r.Name] = r;
                }
            }
        });
    }

    public void PrintRoomList()
    {
        Debug.Log("================ ROOM LIST ================");

        string[] ri = m_Rooms.Keys.ToArray();

        foreach (var item in ri)
        {
            Debug.Log(item);
        }

        Debug.Log("===========================================");
    }
}
