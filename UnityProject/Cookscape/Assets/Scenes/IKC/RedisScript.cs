using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StackExchange.Redis;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;




public class RedisScript : MonoBehaviourPunCallbacks
{
    // Redis 연결

    public static RedisScript RedisInstance = new RedisScript();
    public static ConnectionMultiplexer redis = null;
    //ConnectionMultiplexer.Connect("localhost:6379");

    public static IDatabase db = null;
        //redis.GetDatabase();



    public int MaxRoomNum = 5;// limit game room manpower


    public string roomSetKey = "roomset";// redis string key -> value is object roomset

    public string LastMan = "";// redis string key  -> value is UserNickname



    void Update()// 마지막으로 업데이트를 하는 인원을 수시로 체크한다.  check persistantly Last updated man's record 
    {

        LastMan = db.StringGet("LastMan");// LastMan is Last RPC behavior man's nickname 


    }

    [PunRPC]
    public void SetRoomset2Redis()// Using singleton pattern , this function will be used another script or RPC?  
    {
        /*
         * 
         * 
        Dictionary<string,RoomManager.Rooms> temp=RoomManager.rm.lbs.RoomSet;// redis input elements
       
        foreach(var item in temp.Values)
        {
            RoomManager.Rooms temprooms = new RoomManager.Rooms();// redis input elements

            temprooms.RoomName = item.RoomName;
            temprooms.ManList = item.ManList;
            List<RoomPlayer> rp = new List<RoomPlayer>();// redis input elements

            for (int k=0;k<item.ManList.Count; k++)
            {
               RoomPlayer tempRP= new RoomPlayer();// redis input elements

                tempRP.NickName= temprooms.ManList[k].NickName;
                tempRP.JobNumber= temprooms.ManList[k].JobNumber;

                rp.Add(tempRP);

            }
        }
        */
        // read one more parts but not need?


        //string roomSetValues = JsonConvert.SerializeObject(temp);

        if (!LastMan.Equals(PhotonNetwork.LocalPlayer.NickName))// RPC received man delete
        {
            Debug.Log(LastMan + "not lastman you!");
            return;
        }


        string roomSetValues = JsonConvert.SerializeObject(RoomManager.rm.lbs.RoomSet);
        Debug.Log(roomSetValues + "CHECK ROOMSET VALUE#####");
        db.StringSet(roomSetKey, roomSetValues);


    }


    public void GetRoomSet()// recent roomset data moved  from redis to client 
    {

        Dictionary<string, RoomManager.Rooms> RoomSetter = JsonConvert.DeserializeObject<Dictionary<string, RoomManager.Rooms>>(db.StringGet(roomSetKey));
        RoomManager.rm.lbs.RoomSet = RoomSetter;

    }
    [PunRPC]
    public void RoomsetBefore()
    {
        if (LastMan.Equals(""))// Master get in metabus 
        {
            return;
        }
        else
        {
            foreach (var man in PhotonNetwork.CurrentRoom.Players)
            {
                if (man.Equals(LastMan) && PhotonNetwork.LocalPlayer.NickName.Equals(LastMan))//find lastman 
                {
                    SetRoomset2Redis();
                    break;

                }

            }
        }

    }

    public void LastManMarked(string ClientNickname)// RPC Caller must be done ..?
    {


        db.StringSet("LastMan", ClientNickname);


    }

}










