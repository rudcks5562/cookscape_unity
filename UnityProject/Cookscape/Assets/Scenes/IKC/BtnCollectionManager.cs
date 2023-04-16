using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityProject.Cookscape;

public class BtnCollectionManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update


    // Update is called once per frame
    // RoomManager roomManager= roomManager;

    RoomManager roomManager;
    public void Start()
    {
        roomManager = RoomManager.rm;
    }

    public void CreateBtnActive(TextMeshProUGUI Roomname)// �����?��ư 
    {
       // GameObject RB= GameObject.Find("Room Info");
      //  RB.SetActive(true);

        roomManager.CreateNewRoom();
        //CreateBtnEnd(Roomname.text);

    }
    //public void CreateBtnEnd(string Roomname)// after create enter auto
    //{
    //   // GameObject RB = GameObject.Find("Room Info");
    //   // RB.SetActive(true);
        
    //    roomManager.EnteredRoom(Roomname);
        
    //  //  roomManager.EnteredRoom();
    //}

    public void BookExitBtnActive(TextMeshProUGUI RoomName)// ����?�ݱ���?@@@@ �����ʿ� 
    {
        if(RoomName == null)
        {
            return;
        }

        //GameObject.Find("Room Book").SetActive(false);


        roomManager.LeftRoom(RoomName.text);
        
        

        //PhotonManager.instance.DisconnectAndGoLobby();
        
    }
    public void CoompleteOut()// just log off
    {

        if(!roomManager.pannel_right_room_name.Equals(null))
        {
            BookExitBtnActive(GameObject.Find("Room Info").transform.Find("Room Title").transform.GetComponent<TextMeshProUGUI>());
            return;
        }

        GameObject.Find("Room Book").SetActive(false);

        PhotonManager.instance.CompleteDisconnect();

    }

    public void SelectListEntered(TextMeshProUGUI CurrentRoomName,GameObject Pannel)// just select list btn at lobby = active
    {


        
        roomManager.EnteredRoom(CurrentRoomName.text);

        Pannel.SetActive(true);


    }
     


    public void MinSizeBtnActive(TextMeshProUGUI roomname)// �ּ�ȭ ��ư 
    {
        //roomManager.pannel_left.SetActive(false);
        //roomManager.pannel_right.SetActive(false);

        //roomManager.MinSizeRoomStatusBarPannel.SetActive(true);
        //GameObject.Find("MinSize Room Status Bar").transform.Find("False").gameObject.SetActive(true);

        //int temp = roomManager.CheckGameStatus(roomname.text);


        //if (temp.Equals(11))// detect how game's status? 
        //{
            

        //    GameObject.Find("MinSize Room Status Bar").transform.Find("True").gameObject.SetActive(true);// ���ӽ��۽� 

        //}




    }
    public void RollSelectToggleFoodActive(ToggleGroup tg)// toggle 
    {
        tg.SetAllTogglesOff();

        GameObject.Find("Chef Toggle").SetActive(false);
        GameObject.Find("Food Toggle").SetActive(true); 


    }
    public void RollSelectToggleCookerActive(ToggleGroup tg)// toggle 
    {

        tg.SetAllTogglesOff();

        GameObject.Find("Chef Toggle").SetActive(true);
        GameObject.Find("Food Toggle").SetActive(false);


    }


    public void NextScrollBtnActive()// ���� ��ũ�� ���� 
    {
        GameObject.Find("Scroll View").GetComponentInChildren<ScrollRect>().verticalNormalizedPosition+= 2.0f;




    }
    public void FromMinSizeToMax()
    {
        //roomManager.pannel_left.SetActive(true);
        //roomManager.pannel_right.SetActive(true);

        

        //roomManager.MinSizeRoomStatusBarPannel.SetActive(false);



    }
    





    public void EnteredRoomActive()
    {
        TextMeshProUGUI currentRoomName = GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(currentRoomName + "ROOOMNAME");

        roomManager.EnteredRoom(currentRoomName.text);
    }

    public void EnterRandomRoom()
    {
        Debug.Log("Try Enter Random Room....");

        roomManager.EnterRandomRoom();
    }














}
