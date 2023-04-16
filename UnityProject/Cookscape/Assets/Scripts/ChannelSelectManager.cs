using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityProject.Cookscape;

public class ChannelSelectManager : MonoBehaviour
{

    public string protocol_channelName;
    public TextMeshProUGUI origin_channelName;
    string BeforeConvertName;

    void Start()
    {
        GameManager.instance.DayTimeControllerOn();
        PlaySceneBGM();

    }

    public void Activate()
    {
        Debug.Log(origin_channelName.text);
        BeforeConvertName = "verse";
        string temp = "meta_";
        
        protocol_channelName = temp;

        StopSceneBGM();
        PhotonManager.instance.ConnectStart(temp+BeforeConvertName);
        // photon conn meta_FFC(fastfoodclub)

    }

    public void PlaySceneBGM()
    {
        if (GetComponent<ChannelSceneBGM>() != null) {
            GetComponent<ChannelSceneBGM>().Play();
        }
    }

    public void StopSceneBGM()
    {
        if (GetComponent<ChannelSceneBGM>() != null) {
            GetComponent<ChannelSceneBGM>().Stop();
        }
    }


}
