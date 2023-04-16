using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ListBtnScript : MonoBehaviour
{


    public TextMeshProUGUI currentRoomName;


    private void Start()
    {




    }







    public void active()
    {
        currentRoomName = GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(currentRoomName + "ROOOMNAME");

        RoomManager.rm.EnteredRoom(currentRoomName.text);




    }
}
