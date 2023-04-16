using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityProject.Cookscape
{


    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        const string CustomPropertyRoomName = "RoomName";
        const string CustomPropertyJobNumber = "JobNumber";

        // version
        private readonly string version = "1.0f";

        // User Id
        private string userId = "";

        public static PhotonManager instance;

        public string metabusName="meta_bus";

        public string NextRoom = "meta_bus";
        public string BeforeRoom = "";

        public byte max_player = 5;


        // Awake is called before start script
        private void Awake()
        {
            //singleton pattern 
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(this);
            }
            else
            {
                if (instance != this)
                {
                    Destroy(gameObject);
                }
            }

            // Auto scene loading for same room users
            //PhotonNetwork.AutomaticallySyncScene = true;

            // Permission for same version users



            // Set user id



            // Set communictaion count with photoncloud server, default : 30/s


            // Join server


            //Screen.SetResolution(960, 540, false);
        }

        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.GameVersion = version;

            PhotonNetwork.LocalPlayer.NickName = GameManager.instance.user.nickname;

            userId = PhotonNetwork.LocalPlayer.NickName;

            if ( !PhotonNetwork.IsConnected )
                PhotonNetwork.ConnectUsingSettings();
        }

        public IEnumerator DisconnectAndGoLobby()
        {
            PhotonNetwork.LeaveRoom();
            RoomOptions rs = new RoomOptions();
            rs.MaxPlayers= 20;
            rs.IsOpen= true;
            rs.IsVisible= true; 
            while (!PhotonNetwork.CurrentRoom.Name.Equals("meta_bus"))
            {
                PhotonNetwork.JoinOrCreateRoom("meta_bus", rs, null);
                yield return new WaitForSeconds(1);
            }
        }

        public void CompleteDisconnect()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            Debug.Log("Complete disconn complete!");
        }
        [PunRPC]
        public void ConnectionHanging(string ToRoomName,string FromRoomName)// lobby(metabus map)  game start -> move scene sequence  
        {
            NextRoom = ToRoomName;
            BeforeRoom = FromRoomName;

            PhotonNetwork.LeaveRoom();
        }

        public void ConnectStart(string RoomName)// gamemanager ref 
        {
            Debug.Log("Hi Connet Start");

            metabusName = RoomName;

            _ = StartCoroutine(nameof(TryJoinRoom));
        }

        // [callback] called when join photon server
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master!");

            // Connected lobby : true
            Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
           
            // Join lobby
            PhotonNetwork.JoinLobby();
        }

        // [callback] called when join lobby
        public override void OnJoinedLobby()
        {
            Debug.Log("I Joined Lobby");
        }

        // [callback] called when created room
        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room");
            Debug.Log($"Room Name : {PhotonNetwork.CurrentRoom.Name}");
        }

        // [callback] called when joined room
        public override void OnJoinedRoom()
        {
            Debug.Log("Hey im comming");
            SceneLoadingManager.LoadScene("Metaverse");
            
        }

        // [callback] called when failed join room
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Join Room Failed {returnCode} : {message}");

            RoomOptions ro = new()
            {
                MaxPlayers = 100,      // maximum user
                IsOpen = true,       // open or close room
                IsVisible = true    // public or private room
            };
            PhotonNetwork.CreateRoom(metabusName, ro);
        }

        IEnumerator TryJoinRoom()
        {
            WaitForSeconds _waits = new(0.1f);
            while(!PhotonNetwork.InLobby)
            {
                yield return _waits;
            }

            yield return _waits;

            PhotonNetwork.JoinRoom(metabusName);
        }
    }
}
