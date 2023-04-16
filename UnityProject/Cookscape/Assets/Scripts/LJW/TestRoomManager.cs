using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityProject.Cookscape;

public class TestRoomManager : MonoBehaviourPunCallbacks
{
    //instance
    public static TestRoomManager rm;

    //cloning room pannel inside objects
    public GameObject m_Content;// ������ Ŭ�п� content
    public GameObject m_ContentText;// content text


    // left info pannel objects
    public GameObject pannel_left;//���� �ǳ� ����Ʈ �غ�// list
    public GameObject RoomInfoTitle;
    public GameObject RoomBannerBtn;// ��ư �ޱ����� ������Ʈ 

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

    //public Dictionary<string, Rooms> CheckRoomSet = lobbys.lb.RoomSet;

    public PhotonView PhotonView;// RPC����� ���� photonview 

    public Dictionary<string, RoomInfoRecorder> m_CustomRoomList;


    public lobbys lbs = lobbys.lb;


    private static RoomPlayer ClientPlayer;
    private string NextRoomName = string.Empty;

    private void Awake()
    {

        rm = this;
        m_CustomRoomList = new Dictionary<string, RoomInfoRecorder>();
        PhotonNetwork.LocalPlayer.CustomProperties = new ExitGames.Client.Photon.Hashtable() { { "RoomName", "" }, { "JobNumber", "-1"} };

        PhotonNetwork.ConnectUsingSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.LocalPlayer.NickName = GameManager.instance.user.nickname;
        PhotonNetwork.LocalPlayer.NickName = "User" + UnityEngine.Random.Range(0, 99999);
        //PhotonNetwork.AutomaticallySyncScene = true;

        //String PublisherName = PhotonNetwork.MasterClient.NickName;// 접속자 데이터 동기화.. 

        Debug.Log(PhotonView + "@@@!");

        ClientPlayer = new RoomPlayer();
        ClientPlayer.NickName = PhotonNetwork.LocalPlayer.NickName;
        ClientPlayer.IsOnline = true;
        ClientPlayer.CurrentRoom = "";
        // init 

        pannel_right.SetActive(false);// at start, right side cant see

        // test code!!!

        // 로비에서 룸을 고르도록한다. 
        // 고르면 룸 인포를 받아와 룸에대한 세부정보와 현재 룸 안의 유저들과 각종 세부정보를 옆에 표기한다.
        // 이 때 방에서 나가기를 누르면 방에서만 LEAVE된다.
        // 이를 동기화해주는 것도 필요하다.
        // 중요한 시작 기준 5명이 차면 3초후 시작이 되도록 로딩스피너나 또는 타이머가 필요하다.
        // 게임 시작이 되면 그 해당룸의 이름을 참조하여 새로운 룸을 만들어서 그 방의 인원들을 그 룸에 배속시킨후 (RPC를 이 때 사용하면됨) 이 후 마스터의 씬 동기화를 받고 스폰 및 초기화를 진행
        // 입장 퇴장시 
        // 현재의 룸 관련 클래스들은 대기열에 속한 룸으로 생각해야되며 실제 게임 실행시 포톤의 룸을 다시 배정해주는 형태로 진행해보도록 하겠음.

        //PhotonNetwork.ConnectUsingSettings();   TEST CODE 
        RoomListRefresh();
    }


    public override void OnConnectedToMaster()
    {

        Debug.Log($"Current_Room_Name_Before_Load_Level : {(PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "Nothing")}");

        //PhotonNetwork.LoadLevel("GAME_SCENE1");

        PhotonNetwork.JoinOrCreateRoom("meta_verse", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Success Join Toom" + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Fail Join room: \n{returnCode}: {message}");
    }

    //void Update()//
    //{
    //    if (Input.GetKey(KeyCode.Backslash))
    //    {// TEST CODE
    //        StandBy();
    //        // timer and start 
    //    }
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

        Debug.Log(m_ContentText.GetComponent<TextMeshProUGUI>().text);

        RoomInfoRecorder rir = m_CustomRoomList[roomname];

        pannel_right_room_name.text = rir.RoomInsideName;// Left to right move the name string
        pannel_right.SetActive(true);// show right side pannel
        selectedRoomName = rir.RoomName;

        string NowRoomName = (string)PhotonNetwork.LocalPlayer.CustomProperties["RoomName"];

        if (NowRoomName.Equals(roomname))// 똑같은 방에 입장하려고 하는 경우 
        {
            return;
        }
        else if (!NowRoomName.Equals(""))//현재 방에서 다른 방으로 이동하려고 하는 경우 
        {
            //현재 방에서 나가기
            PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 2, PhotonNetwork.LocalPlayer.CustomProperties["RoomName"], ClientPlayer.NickName, int.Parse(ClientPlayer.JobNumber));// before room out 

            ClientPlayer.CurrentRoom = roomname;
            
            //새 방에 들어가기
            PhotonView.RPC("RoomRPC", RpcTarget.All, 1, roomname, ClientPlayer.NickName, -1);// after room in
            return;                                                                             // 
        }
        else if (NowRoomName.Equals(""))
        {
            ClientPlayer.CurrentRoom = roomname;
            
            //새방 들어가기
            PhotonView.RPC(nameof(RoomRPC), RpcTarget.All, 1, roomname, ClientPlayer.NickName, -1);
            Debug.Log(ClientPlayer.CurrentRoom + "crt room check");
        }
    }

    public void RoomRefresh(string RoomName)// inside room    jobnum 변화 구현하기 
    {
        // manlist 정보값 읽어서 다시 텍스트 바꾸기 
        RoomInfoRecorder temp = m_CustomRoomList[RoomName];

        string nickname;
        string jobnum;

        int FoodsNum = 0;
        int CookerNum = 0;

        for (int s = 0; s < temp.ManList.Count; s++)
        {
            nickname = temp.ManList[s].NickName;
            jobnum = (string)temp.ManList[s].CustomProperties["JobNumber"];
            GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(true);
            if (jobnum.Equals("1"))
            {
                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(false);
                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(true);

                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Chef").gameObject.SetActive(true);
                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Food").gameObject.SetActive(false);
            }
            else if (jobnum.Equals("0"))
            {
                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(false);
                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(true);

                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Chef").gameObject.SetActive(false);
                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Food").gameObject.SetActive(true);
            }
            else if (jobnum.Equals("-1"))
            {
                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(false);

                GameObject.Find("Member" + (s + 1).ToString()).GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().Find("Roll").GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(true);
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

        GameObject.Find("Food Status").transform.GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        GameObject.Find("Food Status").transform.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);

        if (temp.FoodSetting)
        {


            GameObject.Find("Food Status").transform.GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(true);

        }
        else if (temp.FoodSetting == false)
        {

            GameObject.Find("Food Status").transform.GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(false);
            GameObject.Find("Food Status").transform.GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(true);

            GameObject.Find("Food Status").transform.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().Find("Less Count").GetComponent<TextMeshProUGUI>().text = (4 - FoodsNum)
                .ToString();



        }
        if (temp.CookerSetting)
        {

            GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(true);
            GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(false);

            // GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("Less Text").gameObject.SetActive(!true);

        }
        else if (temp.CookerSetting == false)
        {
            GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").gameObject.SetActive(true);
            GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("True").gameObject.SetActive(false);
            if (CookerNum == 0)
            {
                GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Much Text").gameObject.SetActive(false);

                GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Less Text").gameObject.SetActive(true);


            }
            else if (temp.CookerSetting == false && CookerFlag == 1)
            {

                GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Much Text").gameObject.SetActive(true);

                GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Less Text").gameObject.SetActive(false);

            }
            else if (temp.CookerSetting == false && CookerFlag == 0)
            {

                GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Much Text").gameObject.SetActive(false);

                GameObject.Find("Chef Status").transform.GetComponentInChildren<Transform>().Find("False").transform.GetComponentInChildren<Transform>().Find("Less Text").gameObject.SetActive(true);

            }



        }

        //GameObject.Find("Ready Status").transform.gameObject.SetActive(false);
        GameObject.Find("Ready Status").transform.GetComponentInChildren<Transform>().Find("Unselect").gameObject.SetActive(false);
        GameObject.Find("Ready Status").transform.GetComponentInChildren<Transform>().Find("Unable").gameObject.SetActive(false);
        GameObject.Find("Ready Status").transform.GetComponentInChildren<Transform>().Find("Ready").gameObject.SetActive(false);

        if (temp.CookerSetting && temp.FoodSetting)
        {
            GameObject.Find("Ready Status").transform.GetComponentInChildren<Transform>().Find("Unable").gameObject.SetActive(false);
            GameObject.Find("Ready Status").transform.GetComponentInChildren<Transform>().Find("Ready").gameObject.SetActive(true);

            StartCoroutine(StartTimer());// timer started and we prerpare next scene now 
            // timer and start 
        }

        else if (CookerNum == 0 || CookerNum > 1 || FoodsNum != 4)
        {
            GameObject.Find("Ready Status").transform.GetComponentInChildren<Transform>().Find("Ready").gameObject.SetActive(false);

            GameObject.Find("Ready Status").transform.GetComponentInChildren<Transform>().Find("Unable").gameObject.SetActive(true);

        }


        //if(Input.GetKey(KeyCode.Backslash)) {// TEST CODE
        //    StartCoroutine(StartTimer());// timer started and we prerpare next scene now 
        //    // timer and start 
        //}
    }

    [PunRPC]
    public void RoomRPC(int accident, string RoomName, string WhoName, int jobnum)// �� ���� �÷��̾� ��ȭ�� RPC���� ��   1- player in, 2- player out, 3- player select job change
    {

        // �÷��̾� ��ġ �г�, ������ , �� ���� ���¿��� üũ , ��ũ��Ʈ ��ȭ�� ���� (�̰� RPC�� ���Ҷ��� �Ҷ� ����)

        Debug.Log(RoomName + "roomname check for rpc?");
        RoomInfoRecorder temp = m_CustomRoomList[RoomName];// current  room info  here   room inside case only    
        switch (accident)
        {
            //방 입장하기
            case 1:
                if (temp.playerCount == 5)
                {
                    // event handler need!  �����ʰ� �̺�Ʈ �ʿ� 
                    Debug.Log("This Room Is Full");
                    
                    return;
                }

                temp.ManList.Add(PhotonNetwork.LocalPlayer);

                if (WhoName.Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    PhotonNetwork.LocalPlayer.CustomProperties["RoomName"] = RoomName;
                    ClientPlayer.CurrentRoom = RoomName;
                }
                //RPC 
                //(string nickName, string jobNumber, string CurrentRoom, bool isOnline)

                break;


            case 2:
                for (int s = 0; s < temp.ManList.Count; s++)
                {
                    if (temp.ManList[s].NickName.Equals(WhoName))
                    {
                        temp.ManList.RemoveAt(s);
                        if (WhoName.Equals(PhotonNetwork.LocalPlayer.NickName))
                        {
                            PhotonNetwork.LocalPlayer.CustomProperties["RoomName"] = "";
                            ClientPlayer.CurrentRoom = "";
                        }
                    }
                }
                break;

            case 3:

                for (int s = 0; s < temp.ManList.Count; s++)
                {
                    if (temp.ManList[s].NickName.Equals(WhoName))
                    {
                        if (temp.ManList[s].CustomProperties["JobNumber"].Equals("0") && jobnum == 0)
                        {
                            temp.ManList[s].CustomProperties["JobNumber"] = "0";
                            break;
                        }
                        else if (temp.ManList[s].CustomProperties["JobNumber"].Equals("0") && jobnum == 1)
                        {
                            temp.ManList[s].CustomProperties["JobNumber"] = "1";
                            break;
                        }
                        else if (temp.ManList[s].CustomProperties["JobNumber"].Equals("1") && jobnum == 1)
                        {
                            temp.ManList[s].CustomProperties["JobNumber"] = "1";
                            break;
                        }
                        else if (temp.ManList[s].CustomProperties["JobNumber"].Equals("1") && jobnum == 0)
                        {
                            temp.ManList[s].CustomProperties["JobNumber"] = "0";
                            break;
                        }
                        else if (temp.ManList[s].CustomProperties["JobNumber"].Equals("-1"))
                        {
                            if (jobnum == 1)
                            {
                                temp.ManList[s].CustomProperties["JobNumber"] = "1";
                            }
                            else if (jobnum == 0)
                            {
                                temp.ManList[s].CustomProperties["JobNumber"] = "0";
                            }
                        }

                    }
                }
                break;
        }

        if (ClientPlayer.CurrentRoom != "")
        {
            RoomRefresh(RoomName);
        }

        RoomListRefresh();
    }

    public void RoomListRefresh()// �� ����Ʈ ����  from lobby class , data updated 
    {
        // expire check need 
        //var children = m_Content.transform.GetComponentInChildren<Transform>();
        //foreach (Transform child in children)
        //{
        //    if (child.name.Contains("Content"))
        //    {
        //        continue;
        //    }
        //    else if (child.name.Contains("Clone"))
        //    {

        //        Destroy(child.gameObject);
        //    }
        //}

        //오토 클리너 작동
        //RoomManager.lobbys.lb.AutoCleaner();

        RoomInfoRecorder[] rec = FindObjectsOfType<RoomInfoRecorder>();

        if ( rec.Length == 0)
        {
            return;
        }

        foreach (var item in rec)
        {
            if ( item.RoomName == "")
            {
                continue;
            }

            if (item.Expire)
            {
                Destroy(item.gameObject);
                if (m_CustomRoomList.ContainsKey(item.RoomName))
                {
                    m_CustomRoomList.Remove(item.RoomName);
                }
                else
                {
                    //Do nothing
                }
            }
            else
            {
                if (m_CustomRoomList.ContainsKey(item.RoomName))
                {
                    //hehe
                }
                else
                {
                    //Add
                    m_CustomRoomList.Add(item.RoomName, item);
                }
            }
        }
        //foreach (Rooms roms in lobbys.lb.RoomSet.Values)
        //{
        //GameObject goText = PhotonNetwork.Instantiate("RoomBannerBtn", m_Content.transform.position);
        //goText.GetComponentInChildren<TMP_Text>().text = roms.RoomName;

        //List<RoomPlayer> temporaryList = lobbys.lb.RoomSet[roms.RoomName].ManList;
        //int foodCnt = 0;
        //int chefCnt = 0;
        //foreach (RoomPlayer rp in temporaryList)
        //{
        //    if (
        //    rp.JobNumber.Equals("0"))
        //    {
        //        foodCnt++;
        //    }
        //    else if (rp.JobNumber.Equals("1"))
        //    {
        //        chefCnt++;
        //    }

        //}

        //Debug.Log(foodCnt + "FOOD!CNT");
        //Debug.Log("CHEFCNT" + chefCnt);


        //goText.GetComponentInChildren<Transform>().Find("roll").Find("food count").GameObject().GetComponent<TextMeshProUGUI>().text = foodCnt.ToString();
        //goText.GetComponentInChildren<Transform>().Find("roll").Find("chef count").GameObject().GetComponent<TextMeshProUGUI>().text = chefCnt.ToString();
        //goText.GetComponentInChildren<Transform>().Find("Member Count").GameObject().GetComponent<TextMeshProUGUI>().text = (foodCnt + chefCnt).ToString() + "/5";

        //m_Content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        //}

    }

    [PunRPC]
    public void CreateNewRoom()// ���� �� ����� at this moment room create but man info are not added 
    {

        //goText.GetComponent<TextMeshProUGUI>().text = message;
        Debug.Log(CreationRoomNameText.text + " : 생성될 방 이름 로그 입니다.");
        // Debug.Log(goText.GetComponentInChildren<TMP_Text>().text);
        string temp = CreationRoomNameText.text;
        
        //Debug.Log(temp.Length);

        if (temp.Length == 1)
        {
            Debug.Log(CreationRoomNameText.text + "is no name err");
            return;
        }

        if (m_CustomRoomList.ContainsKey(CreationRoomNameText.text))
        {
            Debug.Log(CreationRoomNameText.text + "duplicate name err");
            return;
        }

        //방 생성
        GameObject goText = PhotonNetwork.Instantiate("Prefabs/UI/Room Button", m_Content.transform.position, Quaternion.identity);
        goText.transform.SetParent(m_Content.transform);

        //데이터 넣기

        Debug.Log($"방 제목: {temp}");
        RoomInfoRecorder rir = goText.GetComponent<RoomInfoRecorder>();
        rir.RoomName = temp;
        rir.RoomInsideName = temp;

        m_Content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        // we must think about when i use the RPC that added new room ?

        //PhotonView.RPC(nameof(AddRoom), RpcTarget.All, CreationRoomNameText.text);
        //AddRoom(CreationRoomNameText.text);

        //       public static void AddRoom(string RoomName,Rooms rooms )// ���ϱ� 
        //// photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);

        //방 목록 갱신
        RoomListRefresh();

        //lobbys.AddRoom(CreationRoo
        //mNameText.text, new Rooms());

        Debug.Log(CreationRoomNameText.text);
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
        PhotonView.RPC("RoomRPC", RpcTarget.All, 2, roomName, PhotonNetwork.LocalPlayer.NickName);

        pannel_right.SetActive(false);

        //RoomRPC(2, roomName, PhotonNetwork.LocalPlayer.NickName);


        // rpc + out

    }


    public void ChangeBtnActive(int idx)// ������ �гο� ���� ���� ��ư ���� �� �̹��� ����?
    {
        string MyNickName = PhotonNetwork.LocalPlayer.NickName;
        string MyRoomName = ClientPlayer.CurrentRoom;

        int tempLength = lobbys.lb.RoomSet[MyRoomName].ManList.Count;

        for (int s = 0; s < tempLength; s++)
        {
            if (lobbys.lb.RoomSet[MyRoomName].ManList[s].NickName.Equals(MyNickName))
            {
                if (s == idx)
                {

                    PhotonView.RPC("RoomRPC", RpcTarget.All, 3, MyRoomName, MyNickName);

                    //RoomRPC(3, MyRoomName, MyNickName);

                }

            }

        }



    }

    public IEnumerator StartTimer()// 5�� Ÿ�̸� ���� �Լ� �� �� RPC�� �� ��ȯ 
    {
        int value = 5;

        Debug.Log("타이머를 시작한다맨");

        if (GameObject.Find("Room").transform.Find("MinSize Room Status Bar").gameObject.activeSelf.Equals(true))
        {
            GameObject.Find("Room").transform.Find("MinSize Room Status Bar").gameObject.GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().gameObject.SetActive(true);
            GameObject.Find("Room").transform.Find("MinSize Room Status Bar").gameObject.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        }

        else if (GameObject.Find("Room").transform.Find("Room Book").transform.Find("Room Info").gameObject.activeSelf.Equals(true))
        {
            GameObject.Find("Room").transform.Find("Room Book").transform.Find("Room Info").transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Ready").GetComponentInChildren<Transform>().gameObject.SetActive(true);
            GameObject.Find("Room").transform.Find("Room Book").transform.Find("Room Info").transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Unable").GetComponentInChildren<Transform>().gameObject.SetActive(false);
            GameObject.Find("Room").transform.Find("Room Book").transform.Find("Room Info").transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Unselect").GetComponentInChildren<Transform>().gameObject.SetActive(false);
        }


        while (true)
        {
            if (value != 0)
            {
                if (GameObject.Find("Room").transform.Find("MinSize Room Status Bar").gameObject.activeSelf.Equals(true))
                {
                    GameObject.Find("Room").transform.Find("MinSize Room Status Bar").gameObject.GetComponentInChildren<Transform>().Find("True").GetComponentInChildren<Transform>().gameObject.SetActive(true);
                    GameObject.Find("Room").transform.Find("MinSize Room Status Bar").gameObject.GetComponentInChildren<Transform>().Find("False").GetComponentInChildren<Transform>().gameObject.SetActive(false);
                }

                else if (GameObject.Find("Room").transform.Find("Room Book").transform.Find("Room Info").gameObject.activeSelf.Equals(true))
                {
                    GameObject.Find("Room").transform.Find("Room Book").transform.Find("Room Info").transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Ready").GetComponentInChildren<Transform>().gameObject.SetActive(true);
                    GameObject.Find("Room").transform.Find("Room Book").transform.Find("Room Info").transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Unable").GetComponentInChildren<Transform>().gameObject.SetActive(false);
                    GameObject.Find("Room").transform.Find("Room Book").transform.Find("Room Info").transform.Find("Ready Status").GetComponentInChildren<Transform>().Find("Unselect").GetComponentInChildren<Transform>().gameObject.SetActive(false);
                }


                TimerText.text = "GAME START SET.." + value.ToString();


                GameObject.Find("Room").transform.Find("MinSize Room Status Bar").gameObject.transform.Find("True").GetComponentInChildren<TextMeshProUGUI>().text = "";
                GameObject.Find("Room").transform.Find("MinSize Room Status Bar").gameObject.transform.Find("True").GetComponentInChildren<TextMeshProUGUI>().text = TimerText.text;

                value--;
                yield return new WaitForSeconds(1);

            }
            else if (value == 0)
            {
                // RPC GAMESTART CODE NEED
                Debug.Log("야 발사한다.");


                PhotonView.RPC(nameof(RPC_GameStart), RpcTarget.All, pannel_right_room_name.text, PhotonView.ViewID);

                yield break;
                //pv.RPC("RPC_GameStart",RpcTarget.All);// ready function = load scene game room;

                // photonview.RPC("RPC_Chat", RpcTarget.All, strMessage); 
            }
        }

    }

    [PunRPC]
    public void RPC_GameStart(string roomName, int me)
    {


        if (me != PhotonView.ViewID)
        {
            return;
        }

        Debug.Log("와 시작한다!");

        if (!ClientPlayer.CurrentRoom.Equals(roomName))
        {
            return;
        }

        StringBuilder sb = new StringBuilder();
        //will change ROOM CODE in FUTURE 
        sb.Append("game_").Append(roomName);
        NextRoomName = sb.ToString();

        Debug.Log(ClientPlayer.JobNumber);
        GameManager.instance.m_NextRoomName = NextRoomName;

        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // CLASS @@@@@@@@@@@@@@@@@@@@@@@@@\

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



                    TestRoomManager.rm.PhotonView.RPC("DelRoom", RpcTarget.All, value.RoomName);
                    //// photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
                }


            }
        }
    }



    public class RoomPlayer
    {
        public string NickName { get; set; }
        public string JobNumber { get; set; }
        public string CurrentRoom { get; set; }

        public Boolean IsOnline { get; set; }



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

        public RoomPlayer()
        {
        }



    }

    [System.Serializable]
    public class Rooms : RoomInfo
    {
        public List<RoomPlayer> ManList = new List<RoomPlayer>();// array idx =5 and 
        public Boolean FoodSetting { get; set; }
        public Boolean CookerSetting { get; set; }// green red 
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
    public class RoomInfo
    {
        public String RoomName { get; set; }
        public DateTime GenDate { get; set; }
        public Boolean Expire { get; set; }
        public int currentPlayerNum { get; set; }

        public RoomInfo(string roomName, DateTime genDate, bool Expire, int currentPlayerNum)
        {
            RoomName = roomName;
            GenDate = genDate;
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
            return;
        }


        else if (rm.JobToggleGroup.ActiveToggles().FirstOrDefault().name.Equals("Chef Toggle"))
        {

            // RoomManager.rm.RoomRPC(3,RoomName.text,PhotonNetwork.LocalPlayer.NickName);

            PhotonView.RPC("RoomRPC", RpcTarget.All, 3, RoomName.text, PhotonNetwork.LocalPlayer.NickName, 1);


        }
        else
        {
            PhotonView.RPC("RoomRPC", RpcTarget.All, 3, RoomName.text, PhotonNetwork.LocalPlayer.NickName, 0);
        }
    }

    public void DeleteMan()
    {

        rm.RoomRPC(2, ClientPlayer.CurrentRoom, PhotonNetwork.LocalPlayer.NickName, int.Parse(ClientPlayer.JobNumber));

        PhotonView.RPC("RoomRPC", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, int.Parse(ClientPlayer.JobNumber));
    }
}

