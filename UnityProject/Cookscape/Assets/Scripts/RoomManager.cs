using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityProject.Cookscape;

public class RoomManager : MonoBehaviourPunCallbacks
{
    //instance
    public static RoomManager rm;

    //cloning room pannel inside objects
    public GameObject m_Content;// ������ Ŭ�п� content
    public GameObject m_ContentText;// content text

    public GameObject m_RoomInfoCloseBtn;

    // left info pannel objects
    public GameObject pannel_left;//���� �ǳ� ����Ʈ �غ�// list
    public GameObject RoomInfoTitle;
    public GameObject RoomBannerBtn;// ��ư �ޱ����� ������Ʈ 

    public GameObject FoodStatus;
    public GameObject ChefStatus;
    public GameObject ReadyStatus;
    // creation init values 

    public TMP_Text CreationRoomNameText;// createRoomTitleName
    public TMP_Text pannel_right_room_name;//roominfo title name
    public GameObject JobToggle;// job selection

    // ETC from first left side objects
    public GameObject ScrollRect;// ScrollRect.verticalnormalizedposition

    // room info GomeObjects 
    public GameObject pannel_right;// ������ ����Ʈ �� �������ͽ� info 

    public TextMeshProUGUI pageTitle;

    public ToggleGroup JobToggleGroup;

    public TextMeshProUGUI RoomTitle;
    public TextMeshProUGUI[] RoomPlayerPannel = new TextMeshProUGUI[5];// player nick info pannel 
    public GameObject[] RoomPlayerJobPannel = new GameObject[5]; // player job value pannel

    public TextMeshProUGUI[] RoomStatusPannel = new TextMeshProUGUI[2]; // LEFT RIGHT -> 2 
    public TextMeshProUGUI HelpScriptText;// room status show

    public TextMeshProUGUI TimerText;// �ʸ��� ��ȭ�� Ÿ�̸�

    public string selectedRoomName = "";// ���õ� ���� ����

    // create pannel 

    public TextMeshProUGUI PageTitle_cr;
    public TextMeshProUGUI RoomTitle_cr;

    // minsizedpannel part 

    public GameObject MinSizeRoomStatusBarPannel;

    public Dictionary<string, Rooms> CheckRoomSet = lobbys.lb.RoomSet;

    public PhotonView m_PhotonView;// RPC����� ���� photonview 

    public PlayInfo[] m_PlayerList;

    public PlayInfo m_LocalPlayer;

    public lobbys lbs = lobbys.lb;

    private static RoomPlayer ClientPlayer;
    private string NextRoomName = string.Empty;

    //const string CustomPropertyRoomName = "RoomName";
    //const string CustomPropertyJobNumber = "JobNumber";

    bool m_TimerActive = false;

    private void Awake()
    {

        rm = this;
        m_PhotonView = GetComponent<PhotonView>();
        //PhotonNetwork.ConnectUsingSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        // PhotonNetwork.LeaveRoom(true);// getout from metabus or diffrent room 

        //RP(string nickName, string jobNumber, string CurrentRoom, bool isOnline)

        // for test photon code needed 

        PhotonNetwork.LocalPlayer.NickName = GameManager.instance.user.nickname;


        //ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        //customProperties.Add(CustomPropertyRoomName, "");
        //customProperties.Add(CustomPropertyJobNumber, "-1");
        //PhotonNetwork.LocalPlayer.NickName = "TESTMAN" + UnityEngine.Random.Range(0, 987654321);
        //PhotonNetwork.AutomaticallySyncScene = true;
        //String PublisherName = PhotonNetwork.MasterClient.NickName;// 접속자 데이터 동기화.. 

        Debug.Log("Set Custom Properties");

        ClientPlayer = new RoomPlayer();
        ClientPlayer.NickName = PhotonNetwork.LocalPlayer.NickName;
        ClientPlayer.IsOnline = true;
        ClientPlayer.CurrentRoom = "";
        // init 

        pannel_right.SetActive(false);// at start, right side cant see

        // test code!!!

        Cursor.lockState = CursorLockMode.Confined;

        // 로비에서 룸을 고르도록한다. 
        // 고르면 룸 인포를 받아와 룸에대한 세부정보와 현재 룸 안의 유저들과 각종 세부정보를 옆에 표기한다.
        // 이 때 방에서 나가기를 누르면 방에서만 LEAVE된다.
        // 이를 동기화해주는 것도 필요하다.
        // 중요한 시작 기준 5명이 차면 3초후 시작이 되도록 로딩스피너나 또는 타이머가 필요하다.
        // 게임 시작이 되면 그 해당룸의 이름을 참조하여 새로운 룸을 만들어서 그 방의 인원들을 그 룸에 배속시킨후 (RPC를 이 때 사용하면됨) 이 후 마스터의 씬 동기화를 받고 스폰 및 초기화를 진행
        // 입장 퇴장시 
        // 현재의 룸 관련 클래스들은 대기열에 속한 룸으로 생각해야되며 실제 게임 실행시 포톤의 룸을 다시 배정해주는 형태로 진행해보도록 하겠음.

        StartCoroutine(WaitForInstantiateCharacter());

        //PhotonNetwork.ConnectUsingSettings();   TEST CODE 
        //RoomListRefresh();

        //m_PhotonView.RPC(nameof(RedisScript.RedisInstance.SetRoomset2Redis), RpcTarget.All, null);// find lastman and setting his roomset to redis
        //RedisScript.RedisInstance.GetRoomSet();// then take it  

        GameManager.instance.MetabusUI.gameObject.SetActive(true);
    }


    public override void OnConnectedToMaster()
    {
        //RoomOptions rs = new();
        //rs.MaxPlayers = 5;
        //rs.IsOpen= true;
        //rs.IsVisible= true;

        Debug.Log($"Current_Room_Name_Before_Load_Level : {(PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "Nothing")}");

        GameManager.instance.MetabusUI.gameObject.SetActive(false);
        // PhotonNetwork.LoadLevel("GAME_SCENE1");
        SceneLoadingManager.LoadScene("GAME_SCENE1");

        //PhotonNetwork.JoinOrCreateRoom(NextRoomName, rs, TypedLobby.Default);
        //    PhotonNetwork.JoinLobby();
    }



    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Fail Join room: \n{returnCode}: {message}");
    }

    //   void Update()//
    //  {

    //   if (Input.GetKey(KeyCode.Backslash))
    //     {// TEST CODE

    //     RedisScript.SetMyRoomset2Redis();
    // timer and start 
    //   }
    //  }
    //    else if (Input.GetKey(KeyCode.Comma))
    //    {
    //        Subcribe()
    //    }
    //}

    //[PunRPC]
    //public void StandBy()
    //{
    //    if (PhotonView.IsMine.Equals(true))
    //    {
    //        StartCoroutine(StartTimer());// timer started and we prerpare next scene now 
    //    }
    //}

    //public void Subcribe()
    //{
    //    foreach(Rooms roms in lobbys.lb.RoomSet.Values)
    //    {
    //        Debug.Log(roms.RoomName+"방이름");
    //        Debug.Log(roms.ManList.Count+"방인원 수");
    //        foreach(RoomPlayer rp in roms.ManList)
    //        {
    //            Debug.Log(rp.NickName+"유저닉네임");
    //            Debug.Log(rp.JobNumber + "직업 번호");
    //        }
    //    }
    //    return;
    //}

    public void EnteredRoom(string roomname)// 룸에 잘 들어온 경우 오픈쪽 패널을 잘 보이게 해준다 rpc need -> left room list pannel on clicked >>
    {
        // entered room ... RPC act
        Debug.Log(m_ContentText.GetComponent<TextMeshProUGUI>().text + "ENTEREDROOM functions m_cttx text");
        pannel_right_room_name.text = roomname;// Left to right move the name string
        pannel_right.SetActive(true);// show right side pannel
        selectedRoomName = roomname;

        if (ClientPlayer.CurrentRoom.Equals(roomname))// 똑같은 방에 입장하려고 하는 경우 
        {
            return;
        }
        else if (!m_LocalPlayer.RoomName.Equals("") && !ClientPlayer.CurrentRoom.Equals(roomname))//현재 방에서 다른 방으로 이동하려고 하는 경우 (내가 생성한거 아님!)
        {
            if (!roomname.Equals(ClientPlayer.CurrentRoom))
            {
                //int.Parse(ClientPlayer.JobNumber
                Debug.Log("NNN" + ClientPlayer.JobNumber);
                m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 2, ClientPlayer.CurrentRoom, ClientPlayer.NickName, -1);// before room out 


                // RoomRPC(2, ClientPlayer.CurrentRoom, ClientPlayer.NickName,(int.Parse(ClientPlayer.JobNumber)));
                ClientPlayer.CurrentRoom = roomname;

                //  RoomRPC(1, ClientPlayer.CurrentRoom, ClientPlayer.NickName,-1);

                m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 1, roomname, ClientPlayer.NickName, -1);// after room in
                //RedisScript.RedisInstance.LastManMarked(PhotonNetwork.LocalPlayer.NickName);


                return;
            }
            //RoomRPC(1, ClientPlayer.CurrentRoom, ClientPlayer.NickName, -1);
            // RoomRPC(1, selectedRoomName, ClientPlayer.NickName);// and broadcast to everyone my data (it is updated data by me)
            //RoomRefresh(pannel_right_room_name.text);// inside of room information and layout must change but RoomRPC func has a roomrefrsh method at end point

        }
        else if (ClientPlayer.CurrentRoom.Equals(""))// lobby->room entering case 
        {
            ClientPlayer.CurrentRoom = roomname;
            m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 1, roomname, ClientPlayer.NickName, -1);
            //RedisScript.RedisInstance.LastManMarked(PhotonNetwork.LocalPlayer.NickName);

        }

        Debug.Log($"current room check : {m_LocalPlayer.RoomName}");
    }

    public void EnterRandomRoom()
    {
        if (!m_LocalPlayer)
        {
            Debug.Log("Before Loading...");
            return;
        }

        if (!m_LocalPlayer.RoomName.Equals(""))
            return;
        //m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 2, m_LocalPlayer.RoomName, m_LocalPlayer.NickName, -1);

        UpdateMap();

        foreach (var item in CheckRoomSet.Values)
        {
            if ( item.ManList.Count < 5 && !item.RoomName.Equals(""))
            {

                EnteredRoom(item.RoomName);
                return;
            }
        }

        //IF YOU START HERE => MEAN ROOM IS NOTHING
        GameObject RB = pannel_right;
        RB.SetActive(true);

        CreationRoomNameText.text = $"무작위 방 {UnityEngine.Random.Range(0, 999)}-{UnityEngine.Random.Range(0, 999)}";
        CreateNewRoom();
    }

    [PunRPC]
    public void RoomRefresh(string RoomName)// inside room jobnum 변화 구현하기 
    {
        //m_PhotonView.RPC(nameof(UpdateMap), RpcTarget.All);

        if (!m_LocalPlayer || !m_LocalPlayer.RoomName.Equals(RoomName))
        {
            return;
        }

        // manlist 정보값 읽어서 다시 텍스트 바꾸기 
        Rooms temp = lobbys.lb.RoomSet[RoomName];

        string nickname;
        string jobnum;

        int FoodsNum = 0;
        int CookerNum = 0;

        //방 안에 있는 놈들
        for (int s = 0; s < 5; s++)
        {
            Transform trs = pannel_right.transform.Find("Member List").Find($"Member{(s + 1)}").transform;
            if (s < temp.ManList.Count)
            {
                trs.Find("False").gameObject.SetActive(false);
                trs.Find("True").gameObject.SetActive(true);

                Transform tr = trs.Find("True").Find("Roll").GetComponentInChildren<Transform>();
                nickname = temp.ManList[s].NickName;
                jobnum = temp.ManList[s].JobNumber;
                tr.Find("False").gameObject.SetActive(true);
                if (jobnum.Equals("1"))
                {
                    tr.Find("False").gameObject.SetActive(false);
                    tr.Find("True").gameObject.SetActive(true);

                    tr.Find("True").GetComponentInChildren<Transform>().Find("Chef").gameObject.SetActive(true);
                    tr.Find("True").GetComponentInChildren<Transform>().Find("Food").gameObject.SetActive(false);
                }
                else if (jobnum.Equals("0"))
                {
                    tr.Find("False").gameObject.SetActive(false);
                    tr.Find("True").gameObject.SetActive(true);

                    tr.Find("True").GetComponentInChildren<Transform>().Find("Chef").gameObject.SetActive(false);
                    tr.Find("True").GetComponentInChildren<Transform>().Find("Food").gameObject.SetActive(true);
                }
                else if (jobnum.Equals("-1"))
                {
                    tr.Find("True").gameObject.SetActive(false);

                    tr.Find("False").gameObject.SetActive(true);
                }

                RoomPlayerPannel[s].text = nickname;

                if (RoomPlayerJobPannel[s].GetComponentInChildren<Transform>().GetComponentInChildren<Transform>().name.Equals("Food"))
                {
                    var items = RoomPlayerJobPannel[s].GetComponentInChildren<Transform>().transform.GetComponentInChildren<Transform>().transform;

                    foreach (Transform soil in items)
                    {
                        soil.gameObject.SetActive(false);
                        if (soil.name.Equals("Food") && jobnum.Equals("0"))
                        {
                            soil.gameObject.SetActive(true);

                        }
                    }
                }
                else if (RoomPlayerJobPannel[s].GetComponentInChildren<Transform>().GetComponentInChildren<Transform>().name.Equals("Chef"))
                {
                    var items = RoomPlayerJobPannel[s].GetComponentInChildren<Transform>().transform.GetComponentInChildren<Transform>().transform;

                    foreach (Transform soil in items)
                    {
                        soil.gameObject.SetActive(false);
                        if (soil.name.Equals("Chef") && jobnum.Equals("1"))
                        {
                            soil.gameObject.SetActive(true);
                        }
                    }
                }

                if (jobnum.Equals("0"))
                {
                    FoodsNum++;
                }
                else if (jobnum.Equals("1"))
                {
                    CookerNum++;
                }
            }
            else
            {
                trs.Find("True").gameObject.SetActive(false);
                trs.Find("False").gameObject.SetActive(true);
            }

        }// user status pannel info

        temp.FoodSetting = false;
        temp.CookerSetting = false;
        int CookerFlag = 0;

        if (CookerNum != 0)
        {
            temp.CookerSetting = true;
            if (CookerNum > 1)
            {
                CookerFlag = 1;
                temp.CookerSetting = false;

            }

        }

        if (FoodsNum == 4)
        {
            temp.FoodSetting = true;
        }

        FoodStatus.transform.GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        FoodStatus.transform.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        //ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);

        if (temp.FoodSetting)
        {


            FoodStatus.transform.GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(true);

        }
        else if (temp.FoodSetting == false)
        {

            FoodStatus.transform.GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(false);
            FoodStatus.transform.GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(true);

            FoodStatus.transform.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().Find("Less Count").GetComponent<TextMeshProUGUI>().text = (4 - FoodsNum)
                .ToString();



        }
        if (temp.CookerSetting)
        {

            ChefStatus.transform.GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(true);
            ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(false);

            // GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("Less Text").gameObject.SetActive(!true);

        }
        else if (temp.CookerSetting == false)
        {
            ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(true);
            ChefStatus.transform.GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(false);
            if (CookerNum == 0)
            {
                ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Much Text").gameObject.SetActive(false);

                ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Less Text").gameObject.SetActive(true);


            }
            else if (temp.CookerSetting == false && CookerFlag == 1)
            {

                ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Much Text").gameObject.SetActive(true);

                ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Less Text").gameObject.SetActive(false);

            }
            else if (temp.CookerSetting == false && CookerFlag == 0)
            {

                ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Much Text").gameObject.SetActive(false);

                ChefStatus.transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Less Text").gameObject.SetActive(true);

            }
        }

        //GameObject.Find("Ready Status").transform.gameObject.SetActive(false);
        ReadyStatus.transform.GetComponentInChildren<Transform>().Find("Unselect").gameObject.SetActive(false);
        ReadyStatus.transform.GetComponentInChildren<Transform>().Find("Unable").gameObject.SetActive(false);
        ReadyStatus.transform.GetComponentInChildren<Transform>().Find("Ready").gameObject.SetActive(false);

        if (temp.CookerSetting && temp.FoodSetting)
        {
            ReadyStatus.transform.GetComponentInChildren<Transform>().Find("Unable").gameObject.SetActive(false);
            ReadyStatus.transform.GetComponentInChildren<Transform>().Find("Ready").gameObject.SetActive(true);


            Rooms tmp = null;
            if (CheckRoomSet.TryGetValue(RoomName, out tmp))
            {
                foreach (var item in tmp.ManList)
                {
                    //m_PhotonView.RPC(nameof(DoStartTimer), RpcTarget.All, item.NickName);
                    if (!m_TimerActive)
                    {
                        StopCoroutine(StartTimer());
                        StartCoroutine(StartTimer());// timer started and we prerpare next scene now 
                    }
                }
            }
            // timer and start 
        }

        else if (CookerNum == 0 || CookerNum > 1 || FoodsNum != 4)
        {
            ReadyStatus.transform.GetComponentInChildren<Transform>().Find("Ready").gameObject.SetActive(false);

            ReadyStatus.transform.GetComponentInChildren<Transform>().Find("Unable").gameObject.SetActive(true);

        }


        //if(Input.GetKey(KeyCode.Backslash)) {// TEST CODE
        //    StartCoroutine(StartTimer());// timer started and we prerpare next scene now 
        //    // timer and start 
        //}
    }

    [PunRPC]
    void DoStartTimer(string nickName)
    {
        UpdateMap();
        if (!m_TimerActive)
        {
            StopCoroutine(StartTimer());
            StartCoroutine(StartTimer());// timer started and we prerpare next scene now 
        }
    }

    [PunRPC]
    void UpdateMap()
    {
        if (m_Refreshing)
        {
            return;
        }
        else
            m_Refreshing = true;

        Debug.Log("Start Update Map....");
        m_PlayerList = FindObjectsOfType<PlayInfo>();

        lbs.RoomSet.Clear();

        //Map Update
        foreach (var item in m_PlayerList)
        {
            string nickName = item.NickName;

            if (nickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                m_LocalPlayer = item;

            string roomName = item.RoomName;

            string jobNumber = item.JobNumber;

            Rooms tmp;

            if (roomName == null || roomName.Equals(""))
            {
                //do Nothing
            }
            else
            {
                if (lbs.RoomSet.TryGetValue(roomName, out tmp))
                {

                    //add
                    tmp.ManList.Add(new RoomPlayer(nickName, jobNumber, roomName));
                    //}
                }
                //Room Is Nope
                else
                {
                    Rooms rom = new Rooms();
                    rom.RoomName = roomName;
                    rom.RoomInsideName = roomName;
                    rom.ManList = new List<RoomPlayer>();
                    rom.CookerSetting = false;
                    rom.FoodSetting = false;

                    rom.ManList.Add(new RoomPlayer(nickName, jobNumber, roomName));

                    lbs.RoomSet.Add(roomName, rom);
                }
            }
        }

        //
        if (lbs.RoomSet.Count != 0)
            foreach (var item in lbs.RoomSet.Values)
            {
                m_PhotonView.RPC(nameof(RoomRefresh), RpcTarget.All, item.RoomName);
            }

        m_Refreshing = false;
    }

    [PunRPC]
    public void RoomRPC(int accident, string RoomName, string WhoName, int jobnum)// �� ���� �÷��̾� ��ȭ�� RPC���� ��   1- player in, 2- player out, 3- player select job change
    {

        // �÷��̾� ��ġ �г�, ������ , �� ���� ���¿��� üũ , ��ũ��Ʈ ��ȭ�� ���� (�̰� RPC�� ���Ҷ��� �Ҷ� ����)

        if (RoomName.Equals(""))
        {
            return;
        }

        Debug.Log(RoomName + " ... RPC Start" + accident);
        Rooms temp = null; // current  room info  here   room inside case only

        if (CheckRoomSet.TryGetValue(RoomName, out temp))
        {
            //DoNothing
            Debug.Log($"Yes Room is here {RoomName}");
        }
        else
        {
            UpdateMap();
            if (CheckRoomSet.TryGetValue(RoomName, out temp))
            {
                //DoNothing
            }
            else
            {
                return;
            }
        }

        switch (accident)
        {
            //Enter The Room
            case 1:
                if (temp.ManList.Count == 5)
                {
                    // event handler need!  �����ʰ� �̺�Ʈ �ʿ� 
                    return;
                }

                //Add Player
                temp.ManList.Add(new RoomPlayer(WhoName, "-1", RoomName));

                //Is Me?
                if (WhoName.Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    //PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyRoomName] = RoomName;

                    //foreach (var item in m_PlayerList)
                    //{
                    //    if (item.NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                    //    {
                    //        item.RoomName = RoomName;
                    //    }
                    //}
                    m_LocalPlayer.RoomName = RoomName;

                    ClientPlayer.CurrentRoom = RoomName;
                }
                //RPC 
                //(string nickName, string jobNumber, string CurrentRoom, bool isOnline)

                break;

            //Leave The Room
            case 2:
                for (int s = 0; s < temp.ManList.Count; s++)
                {
                    if (temp.ManList[s].NickName.Equals(WhoName))
                    {
                        temp.ManList.RemoveAt(s);

                        //Is ME?
                        if (WhoName.Equals(m_LocalPlayer.NickName))
                        {
                            //PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyRoomName] = "";
                            //PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyJobNumber] = "-1";

                            m_LocalPlayer.RoomName = "";
                            m_LocalPlayer.JobNumber = "-1";

                            ClientPlayer.CurrentRoom = "";
                            ClientPlayer.JobNumber = "-1";
                        }
                        break;
                    }
                }
                break;

            //Change Job
            case 3:

                for (int s = 0; s < temp.ManList.Count; s++)
                {

                    //Is He?
                    if (temp.ManList[s].NickName.Equals(WhoName))
                    {
                        if (jobnum == 0)
                        {
                            temp.ManList[s].JobNumber = "0";
                        }
                        else if (jobnum == 1)
                        {
                            temp.ManList[s].JobNumber = "1";
                        }
                        else
                        {
                            Debug.Log($"What Do you Want Man {jobnum}");
                        }

                        //Is ME?
                        if (WhoName.Equals(PhotonNetwork.LocalPlayer.NickName))
                        {
                            Debug.Log("My Job Change Man");
                            //PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyJobNumber] = temp.ManList[s].JobNumber;

                            foreach (var item in m_PlayerList)
                            {
                                if (item.NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                                {
                                    item.JobNumber = temp.ManList[s].JobNumber;
                                    break;
                                }
                            }
                        }
                        //else if (temp.ManList[s].JobNumber.Equals("1") && jobnum == 1)
                        //{
                        //    temp.ManList[s].JobNumber = "1";
                        //}
                        //else if (temp.ManList[s].JobNumber.Equals("1") && jobnum == 0 )
                        //{
                        //    temp.ManList[s].JobNumber = "0";
                        //}
                        //else if(temp.ManList[s].JobNumber.Equals("-1"))
                        //{
                        //    if (jobnum==1)
                        //    {
                        //        temp.ManList[s].JobNumber = "1";
                        //    }
                        //    else if(jobnum==0)
                        //    {
                        //        temp.ManList[s].JobNumber = "0";
                        //    }
                        //}
                        break;
                    }
                }
                break;
        }

        RoomListRefresh();

        //m_PhotonView.RPC(nameof(RoomRefresh), RpcTarget.All, RoomName);
    }

    bool m_Refreshing = false;
    public void RoomListRefresh()// �� ����Ʈ ����  from lobby class , data updated 
    {

        // expire check need 
        var children = m_Content.transform.GetComponentInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Contains("Content"))
            {
                continue;
            }
            else if (child.name.Contains("Clone"))
            {

                Destroy(child.gameObject);
            }
        }

        //RoomManager.lobbys.lb.AutoCleaner();
        m_PhotonView.RPC(nameof(UpdateMap), RpcTarget.All);
        // RedisScript.LastManMarked(PhotonNetwork.LocalPlayer.NickName);
        //this.lobbys.AutoCleaner();

        Debug.Log($"현재 방의 갯수 {CheckRoomSet.Count}");

        foreach (Rooms roms in lobbys.lb.RoomSet.Values)
        {
            GameObject goText = Instantiate(RoomBannerBtn, m_Content.transform);
            goText.GetComponentInChildren<TMP_Text>().text = roms.RoomName;

            List<RoomPlayer> temporaryList = lobbys.lb.RoomSet[roms.RoomName].ManList;
            int foodCnt = 0;
            int chefCnt = 0;
            foreach (RoomPlayer rp in temporaryList)
            {
                if (rp.JobNumber.Equals("0"))
                {
                    foodCnt++;
                }
                else if (rp.JobNumber.Equals("1"))
                {
                    chefCnt++;
                }

            }

            Debug.Log("FOOD_CNT" + foodCnt);
            Debug.Log("CHEF_CNT" + chefCnt);


            goText.GetComponentInChildren<Transform>().Find("roll").Find("food count").GameObject().GetComponent<TextMeshProUGUI>().text = foodCnt.ToString();
            goText.GetComponentInChildren<Transform>().Find("roll").Find("chef count").GameObject().GetComponent<TextMeshProUGUI>().text = chefCnt.ToString();
            goText.GetComponentInChildren<Transform>().Find("Member Count").GameObject().GetComponent<TextMeshProUGUI>().text = (temporaryList.Count).ToString() + "/5";

            m_Content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }
    }
    [PunRPC]
    public void CreateNewRoom()// ���� �� ����� at this moment room create but man info are not added 
    {

        if (!m_LocalPlayer || !m_LocalPlayer.RoomName.Equals(""))
        {
            return;
        }

        //goText.GetComponent<TextMeshProUGUI>().text = message;
        Debug.Log(CreationRoomNameText.text + "생성된 방 이름 로그 입니다.");
        // Debug.Log(goText.GetComponentInChildren<TMP_Text>().text);
        string temp = CreationRoomNameText.text;

        //Debug.Log(temp.Length);

        if (temp.Length <= 1)
        {
            Debug.Log(CreationRoomNameText.text + "is no name err");
            return;
        }

        if (lobbys.lb.RoomSet.ContainsKey(CreationRoomNameText.text))
        {
            Debug.Log(CreationRoomNameText.text + "duplicate name err");
            return;
        }

        GameObject goText = Instantiate(RoomBannerBtn, m_Content.transform);
        goText.GetComponentInChildren<TMP_Text>().text = CreationRoomNameText.text;
        m_Content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        // we must think about when i use the RPC that added new room ?

        //lobbys.AddRoom(CreationRoo
        //mNameText.text, new Rooms());
        //Rooms rms = new Rooms();
        //rms.RoomName = temp;
        //rms.RoomInsideName = CreationRoomNameText.text;

        Debug.Log(PhotonNetwork.CurrentRoom);

        //ClientPlayer.CurrentRoom = temp;
        m_PhotonView.RPC(nameof(AddRoom), RpcTarget.All, CreationRoomNameText.text);
        //RedisScript.RedisInstance.LastManMarked(PhotonNetwork.LocalPlayer.NickName);
        EnteredRoom(temp);

        //if (!ClientPlayer.CurrentRoom.Equals(""))// create another room -> user make room case
        //{
        //    m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 2, ClientPlayer.CurrentRoom, PhotonNetwork.LocalPlayer.NickName, -1);// out current room
        //    m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 1, CreationRoomNameText.text, ClientPlayer.NickName, -1);// get in create room
        //}
        //else// room status = empty   normally lobby-> room case 
        //{
        //    m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 1, CreationRoomNameText.text, ClientPlayer.NickName, -1);// create and room in
        //}


        //AddRoom(CreationRoomNameText.text);

        //RoomListRefresh();

        //       public static void AddRoom(string RoomName,Rooms rooms )// ���ϱ� 
        //// photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
    }


    [PunRPC]
    public void AddRoom(string roomname)
    {
        Rooms rom = new Rooms();
        rom.RoomName = roomname;
        rom.RoomInsideName = roomname;
        rom.ManList = new List<RoomPlayer>();
        rom.CookerSetting = false;
        rom.FoodSetting = false;
        lobbys.lb.RoomSet.Add(roomname, rom);

    }

    [PunRPC]
    public void DelRoom(string RoomName)// ���� 
    {

        lobbys.lb.RoomSet.Remove(RoomName);
    }

    [PunRPC]
    public void InitUpdateRoom()
    {
        //RoomManager.lobbys.lb.RoomSet;

    }

    public int CheckGameStatus(string RoomName)
    {
        int res = 0;

        if (lobbys.lb.RoomSet[RoomName].FoodSetting.Equals(true))
        {// number 10~00 cook=10  number 1~0 food 1,0

            if (lobbys.lb.RoomSet[RoomName].CookerSetting.Equals(true))
            {
                res = 11;
            }
            else
            {
                res = 1;//01
            }
        }
        else if (lobbys.lb.RoomSet[RoomName].FoodSetting.Equals(false))


            if (lobbys.lb.RoomSet[RoomName].CookerSetting.Equals(true))
            {
                res = 10;
            }
            else
            {
                res = 0;
            }
        return res;

    }

    public void LeftRoom(string roomName)// use disconnect func : pun ~callback �뿡 ���� ���� �ݿ� �ʿ� 
    {
        m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 2, roomName, PhotonNetwork.LocalPlayer.NickName, -1);
        //RedisScript.RedisInstance.LastManMarked(PhotonNetwork.LocalPlayer.NickName);
        //pannel_right.SetActive(false);

        //RoomRPC(2, roomName, PhotonNetwork.LocalPlayer.NickName);

        // rpc + out

    }


    //public void ChangeBtnActive(int idx)// ������ �гο� ���� ���� ��ư ���� �� �̹��� ����?
    //{
    //    string MyNickName = PhotonNetwork.LocalPlayer.NickName;
    //    string MyRoomName = m_LocalPlayer.RoomName;

    //    int tempLength = lobbys.lb.RoomSet[MyRoomName].ManList.Count;

    //    for (int s=0; s < tempLength; s++)
    //    {
    //        if (lobbys.lb.RoomSet[MyRoomName].ManList[s].NickName.Equals(MyNickName))
    //        {
    //            if (s == idx)
    //            {

    //                m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 3, MyRoomName, MyNickName);

    //                //RoomRPC(3, MyRoomName, MyNickName);
    //            }

    //        }

    //    }
    //}

    public IEnumerator StartTimer()// 5�� Ÿ�̸� ���� �Լ� �� �� RPC�� �� ��ȯ 
    {
        m_TimerActive = true;

        //나중에 특정시간 밑으로 내려가면 작동하게 바꾸고
        //특정 시간전에 직업이 변경된 경우 타이머가 튀소되게 하기
        JobToggleGroup.enabled = false;
        m_RoomInfoCloseBtn.SetActive(false);

        int value = 5;

        Debug.Log("타이머를 시작한다맨");

        if (MinSizeRoomStatusBarPannel.activeSelf.Equals(true))
        {
            MinSizeRoomStatusBarPannel.GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().gameObject.SetActive(true);
            MinSizeRoomStatusBarPannel.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        }

        else if (pannel_right.activeSelf.Equals(true))
        {
            pannel_right.transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Ready").GetComponentInChildren<Transform>().gameObject.SetActive(true);
            pannel_right.transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Unable").GetComponentInChildren<Transform>().gameObject.SetActive(false);
            pannel_right.transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Unselect").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        }


        while (true)
        {
            if (value != 0)
            {
                if (MinSizeRoomStatusBarPannel.activeSelf.Equals(true))
                {
                    MinSizeRoomStatusBarPannel.GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().gameObject.SetActive(true);
                    MinSizeRoomStatusBarPannel.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);
                }

                else if (pannel_right.activeSelf.Equals(true))
                {
                    pannel_right.transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Ready").GetComponentInChildren<Transform>().gameObject.SetActive(true);
                    pannel_right.transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Unable").GetComponentInChildren<Transform>().gameObject.SetActive(false);
                    pannel_right.transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Unselect").GetComponentInChildren<Transform>().gameObject.SetActive(false);
                }


                TimerText.text = "GAME START SET.." + value.ToString();


                MinSizeRoomStatusBarPannel.transform.Find("True").GetComponentInChildren<TextMeshProUGUI>().text = "";
                MinSizeRoomStatusBarPannel.transform.Find("True").GetComponentInChildren<TextMeshProUGUI>().text = TimerText.text;

                value--;
                yield return new WaitForSeconds(1);

            }
            else if (value == 0)
            {
                // RPC GAMESTART CODE NEED
                Debug.Log("야 발사한다.");

                m_PhotonView.RPC(nameof(RPC_GameStart), RpcTarget.All, pannel_right_room_name.text, m_PhotonView.ViewID);
                //RedisScript.RedisInstance.LastManMarked(PhotonNetwork.LocalPlayer.NickName);

                yield break;
                //pv.RPC("RPC_GameStart",RpcTarget.All);// ready function = load scene game room;

                // photonview.RPC("RPC_Chat", RpcTarget.All, strMessage); 
            }
        }

    }

    [PunRPC]
    public void RPC_GameStart(string roomName, int me)
    {


        if (me != m_PhotonView.ViewID)
        {
            return;
        }

        Debug.Log("와 시작한다!");

        if (!m_LocalPlayer.RoomName.Equals(roomName))
        {
            return;
        }

        StringBuilder sb = new StringBuilder();
        //will change ROOM CODE in FUTURE 
        sb.Append("game_").Append(roomName);
        NextRoomName = sb.ToString();

        Debug.Log("시작하기 전 나의 직업... : " + m_LocalPlayer.JobNumber);
        GameManager.instance.m_NextRoomName = NextRoomName;

        if (m_LocalPlayer.JobNumber.Equals("1"))
        {
            //Chef
            GameManager.instance.m_IsChef = true;
        }
        else
        {
            //Food
            GameManager.instance.m_IsChef = false;
        }

        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // CLASS @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\

    [PunRPC]
    public class lobbys // �뿡���� ��� ���� �Ѱ� 
    {

        public Dictionary<string, Rooms> RoomSet = new Dictionary<string, Rooms>();// ��� �뿡���� ���� ��Ƴ��� 

        public static lobbys lb = new lobbys();

        public lobbys()
        {

        }


        public void AddRoom(string RoomName, Rooms rooms)// ���ϱ� 
        {
            RoomSet.Add(RoomName, rooms);
        }

        public void DelRoom(string RoomName)// ���� 
        {

            RoomSet.Remove(RoomName);
        }

        public void AutoCleaner()// ����� 0���� �� �ڵ� ���� room expired delete always 
        {


            var keys = RoomSet.Keys.ToArray();
            var values = RoomSet.Values.ToArray();

            for (int index = 0; index < RoomSet.Count; index++)
            {
                String key = keys[index];
                Rooms value = values[index];
                if (value.ManList.Count.Equals(0))
                {
                    //RoomSet.Remove(roms.RoomName);

                    rm.m_PhotonView.RPC("DelRoom", RpcTarget.All, value.RoomName);
                    //// photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
                }


            }
        }
    }

    [System.Serializable]
    public class RoomPlayer
    {
        public string NickName { get; set; }
        public string JobNumber { get; set; }
        public string CurrentRoom { get; set; }

        public bool IsOnline { get; set; }



        public RoomPlayer(string nickName, string jobNumber, string CurrentRoom)
        {
            NickName = nickName;
            JobNumber = jobNumber;
            this.CurrentRoom = CurrentRoom;
            IsOnline = true;
        }

        public RoomPlayer(string nickName, string jobNumber, string CurrentRoom, bool isOnline) : this(nickName, jobNumber, CurrentRoom)
        {
            IsOnline = isOnline;
        }
        public RoomPlayer(string nickName, string jobNumber)
        {

            NickName = nickName;
            this.JobNumber = jobNumber;
        }

        public RoomPlayer()
        {
        }
    }

    [System.Serializable]
    public class Rooms : RoomInfo
    {
        public List<RoomPlayer> ManList = new List<RoomPlayer>();// array idx =5 and 
        public bool FoodSetting { get; set; }
        public bool CookerSetting { get; set; }// green red 
        public string helpScript { get; set; }
        public string RoomInsideName { get; set; }

        public Rooms(List<RoomPlayer> manList, bool foodSetting, bool cookerSetting, string helpScript, string roomInsideName)
        {
            ManList = manList;
            FoodSetting = foodSetting;
            CookerSetting = cookerSetting;
            this.helpScript = helpScript;
            RoomInsideName = roomInsideName;
        }
        public Rooms()
        {
            this.ManList = new List<RoomPlayer>();
            this.helpScript = "";
            this.CookerSetting = false;
            this.FoodSetting = false;
            this.RoomInsideName = "";
        }


    }

    [System.Serializable]
    public class RoomInfo
    {
        public string RoomName { get; set; }
        //public  DateTime GenDate { get; set; }
        public bool Expire { get; set; }
        public int currentPlayerNum { get; set; }

        public RoomInfo(string roomName, bool Expire, int currentPlayerNum)
        {
            RoomName = roomName;
            //GenDate = genDate;
            this.Expire = Expire;
            this.currentPlayerNum = currentPlayerNum;
        }
        public RoomInfo()
        {
        }
    }
    [PunRPC]
    public void ToggleActive(TextMeshProUGUI RoomName)
    {


        if (rm.JobToggleGroup.ActiveToggles().FirstOrDefault().Equals(null))
        {
            Debug.Log("WhatTheHell");
            return;
        }
        else if (rm.JobToggleGroup.ActiveToggles().FirstOrDefault().name.Equals("Chef Toggle"))
        {
            // RoomManager.rm.RoomRPC(3,RoomName.text,PhotonNetwork.LocalPlayer.NickName);
            Debug.Log("Change To Chef");
            Debug.Log(PhotonNetwork.LocalPlayer.NickName);
            m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 3, RoomName.text, PhotonNetwork.LocalPlayer.NickName, 1);
            //RedisScript.RedisInstance.LastManMarked(PhotonNetwork.LocalPlayer.NickName);

        }
        else
        {
            Debug.Log("Change To Food");
            Debug.Log(PhotonNetwork.LocalPlayer.NickName);
            m_PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 3, RoomName.text, PhotonNetwork.LocalPlayer.NickName, 0);
            //RedisScript.RedisInstance.LastManMarked(PhotonNetwork.LocalPlayer.NickName);

        }
    }

    public void DeleteMan()
    {

        //rm.RoomRPC( PhotonNetwork.LocalPlayer.NickName, int.Parse(  ClientPlayer.JobNumber));

        m_PhotonView.RPC("RoomRPC", RpcTarget.All, 2, ClientPlayer.CurrentRoom, PhotonNetwork.LocalPlayer.NickName, int.Parse(ClientPlayer.JobNumber));
        //RedisScript.RedisInstance.LastManMarked(PhotonNetwork.LocalPlayer.NickName);
    }

    IEnumerator WaitForInstantiateCharacter()
    {
        while (!PhotonNetwork.InRoom)
        {
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);

        int random = UnityEngine.Random.Range(0, 4);
        string avatarName = "Prefabs/Player/Runner_Meat";

        if (GameManager.instance.currAvatar != null) {
            switch (GameManager.instance.currAvatar.name)
            {
                case "고기맨":
                    avatarName = "Prefabs/Player/Runner_Meat";
                    break;
                case "토마토맨":
                    avatarName = "Prefabs/Player/Runner_Tomato";
                    break;
                case "피망맨":
                    avatarName = "Prefabs/Player/Runner_Bell Pepper";
                    break;
                case "콜라맨":
                    avatarName = "Prefabs/Player/Runner_Coke";
                    break;
                case "치즈맨":
                    avatarName = "Prefabs/Player/Runner_Cheese";
                    break;
                case "딸기맨":
                    avatarName = "Prefabs/Player/Runner_Strawberry";
                    break;
                case "우유맨":
                    avatarName = "Prefabs/Player/Runner_Milk";
                    break;
                case "에그맨":
                    avatarName = "Prefabs/Player/Runner_Egg";
                    break;
                case "양배추맨":
                    avatarName = "Prefabs/Player/Runner_Cabbage";
                    break;
                case "크로와상맨":
                    avatarName = "Prefabs/Player/Runner_Croissant";
                    break;
                case "토스트맨":
                    avatarName = "Prefabs/Player/Runner_Toast";
                    break;
                case "새우맨":
                    avatarName = "Prefabs/Player/Runner_Shrimp";
                    break;
                case "밀가루맨":
                    avatarName = "Prefabs/Player/Runner_Flour";
                    break;
                case "소세지맨":
                    avatarName = "Prefabs/Player/Runner_Sausage";
                    break;
                default:
                    avatarName = "Prefabs/Player/Runner_Meat";
                    break;
            }
        }

        GameObject me = PhotonNetwork.Instantiate(avatarName, GameObject.FindGameObjectWithTag("Respawn").transform.position, Quaternion.identity);

        m_PlayerList = FindObjectsOfType<PlayInfo>();
        m_LocalPlayer = me.GetComponent<PlayInfo>();

        StartCoroutine(StartUpdateRoomListByInterval());
        //RoomListRefresh();
    }

    float m_Interval = 4f;
    IEnumerator StartUpdateRoomListByInterval()
    {
        while (PhotonNetwork.InRoom)
        {
            Debug.Log("Do Update....");
            RoomListRefresh();
            yield return new WaitForSeconds(m_Interval);
        }
    }
}

