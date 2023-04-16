using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityProject.Cookscape;

public class NetworkForLJWTest : MonoBehaviourPunCallbacks
{
    PhotonView m_PhotonView;
    [SerializeField] bool IsChef = true;

    private void Awake()
    {
        Screen.SetResolution(640, 480, false);
        PhotonNetwork.ConnectUsingSettings();
        m_PhotonView = GetComponent<PhotonView>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect To Master...");
        PhotonNetwork.JoinOrCreateRoom("LJWTestRoom", new Photon.Realtime.RoomOptions { MaxPlayers = 5 }, null);
    }

    public override void OnJoinedRoom()
    {
        int PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"Connet... And {PlayerCount}");

        if (IsChef && PlayerCount == 1)
        {
            PhotonNetwork.Instantiate("Prefabs/Player/Basic_Chef", GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
        }
        else
        {
            switch (PlayerCount + (IsChef ? 0 : 1))
            {
                case 2:
                    PhotonNetwork.Instantiate("Prefabs/Player/Runner_Tomato", GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
                    break;
                case 3:
                    PhotonNetwork.Instantiate("Prefabs/Player/Runner_Coke", GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
                    break;
                case 4:
                    PhotonNetwork.Instantiate("Prefabs/Player/Runner_Meat", GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
                    break;
                case 5:
                    PhotonNetwork.Instantiate("Prefabs/Player/Runner_Bell Pepper", GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);
                    break;
            }
        }

        StartCoroutine(WaitForGetAllPlayerInfo(PlayerCount));
    }

    IEnumerator WaitForGetAllPlayerInfo(int PlayerCount)
    {
        while (GameFlowManager.instance.m_Players.Count < PlayerCount)
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
}
