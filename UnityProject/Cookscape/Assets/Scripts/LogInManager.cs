using Photon.Pun;
using TMPro;
using UnityEngine;

public class LogInManager : MonoBehaviour
{
    // Start is called before the first frame update

    private string version = "1.0";
    public string Nicknames = "USER_GUEST";

    public TMP_InputField input_id;
    public TMP_InputField input_pw;// by unity ediotr , it is registed

    public GameObject gm;
    public GameObject m_Alert;

    void Start()
    {
        GameManager.instance.LoginUI.gameObject.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = true;
        if (version == "0.1")// after version update 1.1 over.. user should be login and get data from spring DB server for nickname update 
        {  
            Nicknames += Random.Range(1, 21);
        }
        // PLAY SCENE BGM
        PlaySceneBGM();

        //PhotonNetwork.LocalPlayer.NickName = Nicknames;
        // PhotonNetwork.ConnectUsingSettings();
    }
    /*
    public override void OnConnectedToMaster()// master entered with no error
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()// entered lobby with no errr
    {
        RoomOptions opt = new RoomOptions();
        opt.MaxPlayers = 20;
        PhotonNetwork.JoinOrCreateRoom("metabus", opt, null);
    }

    public override void OnJoinedRoom()// entered metaRoom with no errr
    {
        Debug.Log("WELCOME TO COOKSCAPE!");



        //            Debug.Log($"Room Name : {PhotonNetwork.CurrentRoom.Name}");

        // need player instantiate code
        // player spawn code and scene change code? 

    }

    */

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) )
        {
            Login();
            // DB  AUTH CHECK CODE NEED!
        }

        if(GameManager.instance.user!= null)
        {
            StopSceneBGM();
            GameManager.instance.LoginUI.gameObject.SetActive(false);
            SceneLoadingManager.LoadScene("ChannelChangeScene");
        }


    }


    public void Login()
    {

        float localVersion = float.Parse(Application.version);
        float serverVersion = GameManager.instance.version.version;

        if (localVersion == serverVersion) {
            if (input_id.text.Trim().Length == 0 || input_pw.text.Trim().Length == 0) return;
            
            StartCoroutine(UnityProject.Cookscape.Api.User.instance.SignIn(input_id.text, input_pw.text));
        } else {
            // VERSION ALERT!!
            GameManager.instance.Alert("Block", "업데이트된 버전이 있습니다. 홈페이지에서 새로운 버전을 다운받아 주세요.");
        }
    }

    public void PlaySceneBGM()
    {
        if (GetComponent<LoginSceneBGM>() != null) {
            GetComponent<LoginSceneBGM>().Play();
        }
    }

    public void StopSceneBGM()
    {
        if (GetComponent<LoginSceneBGM>() != null) {
            GetComponent<LoginSceneBGM>().Stop();
        }
    }
}
