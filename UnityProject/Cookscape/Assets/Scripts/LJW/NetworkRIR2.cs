using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityProject.Cookscape;

public class NetworkRIR2 : MonoBehaviourPunCallbacks
{
    PhotonView m_PhotonView;
    bool m_ChefIsOut = false;
    [SerializeField] bool IsChef = true;

    int MAX_PLAYER_NUMBER = 5;

    private void Awake()
    {
        

        //PhotonNetwork.ConnectUsingSettings();
        m_PhotonView = GetComponent<PhotonView>();
        Debug.Log($"Current Room : {(PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "nope")}");
    }

    private void Start()
    {
        Application.runInBackground = true;

        PhotonNetwork.AutomaticallySyncScene = true;

        RoomOptions rs = new();
        rs.MaxPlayers = 5;
        rs.IsOpen = true;
        rs.IsVisible = true;

        MAX_PLAYER_NUMBER = GameFlowManager.instance.m_PlayerNumber;

        if (PhotonNetwork.IsConnected) 
            PhotonNetwork.JoinOrCreateRoom(GameManager.instance.m_NextRoomName, rs, null);
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect To Master... In Game => this is mean now Testing....");

        RoomOptions rs = new();
        rs.MaxPlayers = 5;
        rs.IsOpen = true;
        rs.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom("_GameTestRoom", rs, null);
    }

    [PunRPC]
    public void ChefIsOut()
    {
        m_ChefIsOut = true;
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(nameof(WaitForAllPlayerEnter));
    }

    IEnumerator WaitForAllPlayerEnter()
    {
        int PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"Connet... And {PlayerCount}");
        GameObject me = null;
        
        //wait
        while (PlayerCount < MAX_PLAYER_NUMBER)
        {
            PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log($"Wait now...  Now Player Count is {PlayerCount}...");

            yield return new WaitForSeconds(1f);
        }

        int cnt = 1;
        if (GameManager.instance.m_IsChef)
        {
            me = PhotonNetwork.Instantiate("Prefabs/Player/Basic_Chef", GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
            m_PhotonView.RPC(nameof(ChefIsOut), RpcTarget.AllBuffered);
            GameFlowManager.instance.m_MyPlayInfo = me.GetComponent<PlayInfo>();

            foreach ( Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if ( p.IsLocal )
                {
                    //Do Nothing
                }
                else
                {
                    m_PhotonView.RPC(nameof(CreateCharacter), RpcTarget.All, p.ActorNumber, cnt++);
                }
            }
        }

        StartCoroutine(WaitForGetAllPlayerInfo(PlayerCount));
    }

    IEnumerator WaitForGetAllPlayerInfo(int PlayerCount)
    {
        while ( GameFlowManager.instance.m_Players.Count < PlayerCount)
        {
            GameFlowManager.instance.SearchPlayer();
            yield return new WaitForSeconds(1f);
        }

        m_PhotonView.RPC("AddMe", RpcTarget.All);
    }

    [PunRPC]
    void AddMe()
    {
        GameFlowManager.instance.SearchPlayer();
    }

    [PunRPC]
    void CreateCharacter(int actorNumber, int PrefabName)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
        {
            return;
        }

        GameObject me = null;
        switch (PrefabName)
        {
            case 1:
                me = PhotonNetwork.Instantiate("Prefabs/Player/Runner_Meat", GameObject.FindGameObjectWithTag("Respawn").transform.Find("Point1").position, Quaternion.identity);
                break;
            case 2:
                me = PhotonNetwork.Instantiate("Prefabs/Player/Runner_Coke", GameObject.FindGameObjectWithTag("Respawn").transform.Find("Point2").position, Quaternion.identity);
                break;
            case 3:
                me = PhotonNetwork.Instantiate("Prefabs/Player/Runner_Bell Pepper", GameObject.FindGameObjectWithTag("Respawn").transform.Find("Point3").position, Quaternion.identity);
                break;
            case 4:
                me = PhotonNetwork.Instantiate("Prefabs/Player/Runner_Tomato", GameObject.FindGameObjectWithTag("Respawn").transform.Find("Point4").position, Quaternion.identity);
                break;
            default:
                Debug.Log($"Bug!! Number is {PrefabName}");
                me = PhotonNetwork.Instantiate("Prefabs/Player/Runner_Tomato", GameObject.FindGameObjectWithTag("Respawn").transform.Find("Point4").position, Quaternion.identity);
                break;
        }
        GameFlowManager.instance.m_MyPlayInfo = me.GetComponent<PlayInfo>();
    }
}
