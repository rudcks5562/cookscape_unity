using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerForRIR : MonoBehaviour
{
    [SerializeField] int me = 0;
    bool nowChanging = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.CompareTag("Chef") || other.attachedRigidbody.CompareTag("Runner"))
        {
            switch (me)
            {
                case 0:
                    NetworkTestForRIR rir = FindAnyObjectByType<NetworkTestForRIR>();
                    rir.PrintRoomList();
                    break;
                case 1:
                    if ( !nowChanging)
                    {
                        nowChanging = true;
                        StartCoroutine("JoinGame");
                    }
                    break;
            }
        }
    }

    IEnumerator JoinGame()
    {
        PhotonNetwork.LeaveRoom();
        while( PhotonNetwork.InRoom )
        {
            yield return null;
        }

        ExitGames.Client.Photon.Hashtable customRoomProperties = new()
        {
            { "status", "Game" }
        };
        PhotonNetwork.JoinRandomOrCreateRoom(customRoomProperties, 5);

        while( !PhotonNetwork.InRoom)
        {
            yield return null;
        }
        
        //if ( PhotonNetwork.LocalPlayer.IsMasterClient )
        //{
        SceneManager.LoadSceneAsync("THIS_IS_GAME_ROOM");
        //}
    }
}
