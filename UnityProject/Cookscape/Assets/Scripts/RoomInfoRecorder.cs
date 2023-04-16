using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomInfoRecorder : MonoBehaviour, IPunObservable
{
    PhotonView m_PhotonView;

    public string RoomName { get; set; }
    //public DateTime GenDate { get; set; }
    public bool Expire { get; set; }
    public int currentPlayerNum { get; set; }

    public int playerCount = 0;

    public List<Player> ManList = new();// array idx =5 and 
    public bool FoodSetting { get; set; }
    public bool CookerSetting { get; set; }// green red 
    public string helpScript { get; set; }
    public string RoomInsideName { get; set; }

    TMP_Text m_TitleText; 

    private void Start()
    {
        m_PhotonView = GetComponent<PhotonView>();
        m_TitleText = GetComponentInChildren<TMP_Text>();
        
        FoodSetting = false;
        CookerSetting = false;
    }

    private void Update()
    {
        m_TitleText.text = RoomName;
    }

    public void AddMe()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어의 데이터를 보냅니다.
            stream.SendNext(RoomName);
            //stream.SendNext(GenDate);
            stream.SendNext(Expire);
            stream.SendNext(currentPlayerNum);
            stream.SendNext(FoodSetting);
            stream.SendNext(CookerSetting);
            stream.SendNext(helpScript);
            stream.SendNext(RoomInsideName);
            //stream.SendNext(ManList);
            stream.SendNext(playerCount);
        }   
        else
        {
            // 원격 플레이어의 데이터를 받습니다.
            RoomName = (string)stream.ReceiveNext();
            //GenDate = (DateTime)stream.ReceiveNext();
            Expire = (bool)stream.ReceiveNext();
            currentPlayerNum = (int)stream.ReceiveNext();
            FoodSetting = (bool)stream.ReceiveNext();
            CookerSetting = (bool)stream.ReceiveNext();
            helpScript = (string)stream.ReceiveNext();
            RoomInsideName = (string)stream.ReceiveNext();
            //ManList = (List<Player>)stream.ReceiveNext();
            playerCount = (int)stream.ReceiveNext();
        }
    }

    //protected class RoomPlayer
    //{
    //    public string NickName { get; set; }
    //    public string JobNumber { get; set; }
    //    public string CurrentRoom { get; set; }

    //    public bool IsOnline { get; set; }

    //    public RoomPlayer(string nickName, string jobNumber, string CurrentRoom)
    //    {
    //        NickName = nickName;
    //        JobNumber = jobNumber;
    //        this.CurrentRoom = CurrentRoom;
    //        IsOnline = true;
    //    }

    //    public RoomPlayer(string nickName, string jobNumber, string CurrentRoom, bool isOnline) : this(nickName, jobNumber, CurrentRoom)
    //    {
    //        IsOnline = isOnline;
    //    }

    //    public RoomPlayer()
    //    {
    //    }
    //}
}
