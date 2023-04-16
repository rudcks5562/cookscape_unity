using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityProject.Cookscape;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public Transform m_ParentUI;
    public GameObject canvas_chat;
    public GameObject m_Content;
    public Transform m_Input_box;
    public TMP_InputField m_inputField;
    public PhotonView m_PhotonView;
    public ScrollRect scroll_rect;
    GameObject m_ContentText;
    public TMP_Text tempText;
    private Color m_InitInputColor;

    GameObject player = null;

    void Start()
    {
        ChatManagerInit();
    }

    void Update()
    {
        if (m_ParentUI == null) {
            ChatManagerInit();
        }

        if (player == null) {
            player = GameManager.instance.player;
        } else {
            ChatHandler();
        }
    }

    void ChatManagerInit()
    {
        m_ParentUI = GameManager.instance.transform.Find("UI_Metabus").gameObject.activeSelf ? 
                GameManager.instance.transform.Find("UI_Metabus") : GameManager.instance.transform.Find("UI_Game").gameObject.activeSelf ? 
                GameManager.instance.transform.Find("UI_Game") : null;
        if (m_ParentUI == null) return;

        canvas_chat = m_ParentUI.Find("Chatting").gameObject;

        Transform chat_box = canvas_chat.transform.Find("Chat Box");
        Transform scroll_view = chat_box.Find("Scroll View");
        Transform viewport = scroll_view.Find("Viewport");

        scroll_rect = chat_box.Find("Scroll View").GetComponent<ScrollRect>();
        m_Content = viewport.Find("Content").gameObject;
        m_ContentText = m_Content.transform.GetChild(0).gameObject;
        tempText = m_Content.transform.Find("test text").GetComponent<TMP_Text>();

        m_Input_box = canvas_chat.transform.Find("Input Box");
        m_InitInputColor = m_Input_box.GetComponent<Image>().color;
        m_inputField = m_Input_box.Find("InputField (TMP)").GetComponent<TMP_InputField>();

        m_PhotonView = GetComponent<PhotonView>();
    }

    void ChatHandler()
    {
        if (m_ParentUI == null) return;
        
        if (m_inputField.isFocused) {
            GameManager.instance.m_ChatInputFocused = true;
            m_Input_box.GetComponent<Image>().color = new Color(0, 25, 255);
        } else {
            GameManager.instance.m_ChatInputFocused = false;
            m_Input_box.GetComponent<Image>().color = m_InitInputColor;
        }

        if (Input.GetKeyDown(KeyCode.Return) && m_inputField.isFocused == false)// need check here 
        {
            m_inputField.ActivateInputField();
            OnEndEditEvent();
            scroll_rect.verticalNormalizedPosition = -0.2f;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected!");
     //   RoomOptions options = new RoomOptions();
     //   options.MaxPlayers = 5;

      //  int nRandomKey = Random.Range(0, 100);

     //   m_strUserName = "user" + nRandomKey;

        //PhotonNetwork.LocalPlayer.NickName = m_strUserName;
        //PhotonNetwork.JoinOrCreateRoom("roomTest1", options, null);

    }

    public override void OnJoinedRoom()
    {
        if (m_ParentUI != null) {
            RPC_Chat($"{PhotonNetwork.LocalPlayer.NickName}님이 입장하셨습니다.");
        }
    }

    public void OnEndEditEvent()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (m_inputField.text.Trim().Length == 0) return;
            string strMessage = GameManager.instance.user.nickname + " : " + m_inputField.text;

            m_PhotonView.RPC("RPC_Chat", RpcTarget.All, strMessage);
            m_inputField.text = "";
        }
    }

    void AddChatMessage(string message)
    {
        GameObject goText = Instantiate(m_ContentText, m_Content.transform);

        List<string> tests = new List<string>();

        int max_text_length = message.Length;

        goText.GetComponent<TextMeshProUGUI>().text = message;
        m_Content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

    }

    [PunRPC]
    void RPC_Chat(string message)
    {
        AddChatMessage(message);
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }

}

