using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityProject.Cookscape
{
    public class RoomData : MonoBehaviour
    {
        TMP_Text m_RoomText;
        private RoomInfo m_RoomInfo;

        public RoomInfo RoomInfo
        {
            get
            {
                return m_RoomInfo;
            }
            set
            {
                m_RoomInfo = value;
                m_RoomText.text = value.ToString();
                GetComponent<Button>().onClick.AddListener(() => { OnEnterRoom(RoomInfo.Name); });
            }
        }

        private void Awake()
        {
            m_RoomText = GetComponent<TMP_Text>();

        }

        void OnEnterRoom(string roomName)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = 5;

            PhotonNetwork.NickName = "¶Êºñ";
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }
}
