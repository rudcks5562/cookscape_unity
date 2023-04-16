using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityProject.Cookscape;

public class MetaChatManager : MonoBehaviour
{
    public GameObject m_Content;
    public TMP_InputField m_inputField;

    PhotonView m_Photonview;
    public ScrollRect scroll_rect;
    GameObject m_ContentText;
    InputHandler m_InputHandler;
    public TMP_Text tempText;

    public GameObject canvas_chat;
    string m_strUserName;


    void Start()
    {
        Screen.SetResolution(960, 600, false);
       // PhotonNetwork.ConnectUsingSettings();
        m_ContentText = m_Content.transform.GetChild(0).gameObject;
        m_Photonview = GetComponent<PhotonView>();
        //canvas_chat.SetActive(false);

        //PhotonManager.instance.ConnectStart("meta_bus");//TEST CODE
        m_InputHandler = InputHandler.instance;

        Debug.Log(PhotonNetwork.CurrentLobby);
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        //if (PhotonNetwork.CurrentRoom.Name.Equals("meta_bus"))
        //{
            canvas_chat.SetActive(true);
            m_strUserName = PhotonNetwork.LocalPlayer.NickName;
        //}
    }

    void Update()
    {
        
        if (m_InputHandler.GetEnterKeyInputDown() && m_inputField.isFocused == false)// need check here 
        {
            m_inputField.ActivateInputField();
            OnEndEditEvent();
            scroll_rect.verticalNormalizedPosition = 0.0f;

        }
    }

    public void OnEndEditEvent()
    {
        string strMessage = m_strUserName + " : " + m_inputField.text;

        m_Photonview.RPC(nameof(RPC_Chat), RpcTarget.All, strMessage);
        m_inputField.text = "";
    }

    void AddChatMessage(string message)
    {
        GameObject goText = Instantiate(m_ContentText, m_Content.transform);

        List<string> tests = new List<string>();

        int max_text_length = message.Length;

        //int tempp = tempText

        //Debug.Log(tempp+"@@@");

        goText.GetComponent<TextMeshProUGUI>().text = message;
        m_Content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

    }

    [PunRPC]
    void RPC_Chat(string message)
    {
        AddChatMessage(message);
        scroll_rect.verticalNormalizedPosition = -1.0f;
    }

}

