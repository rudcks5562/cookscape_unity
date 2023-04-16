using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Toggle tg;


    public void ToggleActive()
    {
        if (tg.isOn) { 
            TextMeshProUGUI input = RoomManager.rm.RoomTitle;
            Debug.Log(input.text);
            RoomManager.rm.ToggleActive(input);
        }

    }
}
