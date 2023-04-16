using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityProject.Cookscape;

public class PlayInfo : MonoBehaviourPun, IPunObservable
{
    #region Common

    public bool GameIsEnd = false;
    public bool IsWin = false;

    public string NickName;
    public string CharacterName;

    public string StartNickName;

    public string RoomName;
    public string JobNumber;

    public bool IsPlaying = true;

    PhotonView m_PhotonView;

    #endregion

    #region Runner
    //escape?
    public bool IsEscape = false;
    //cnt save other
    public int CountSaveOther = 0;
    //cnt captured
    public int CountCaptured = 0;
    //cnt valve
    public int CountCloseValve = 0;
    //cnt pot
    public int CountBreakPot = 0;
    //cnt use towel
    public int CountUseTowel = 0;
    //cnt hitted
    public int CountBeHitted = 0;

    //walk slowly
    public float CountWalkSlowly = 0f;

    //doNotWalk
    public float CountNotWalk = 0f;

    #endregion

    #region Chef

    public int CountCaptureOther = 0;

    public int CountOpenValve = 0;

    public int CountHitOther = 0;

    #endregion

    #region mono behavior

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!GameFlowManager.instance)
        {
            m_PhotonView.ObservedComponents.Add(this);
        }

        if (m_PhotonView.IsMine)
        {
            RoomName = string.Empty;
            JobNumber = "-1";
            NickName = GameManager.instance.user.nickname;
          //  NickName = "TESTMAN5";

            m_PhotonView.RPC(nameof(SetNickName), RpcTarget.AllBuffered, NickName);
        }
        else
        {
            NickName = StartNickName;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (m_PhotonView.IsMine && stream.IsWriting)
        {
            // 로컬 플레이어가 데이터를 보낼 때
            stream.SendNext(RoomName);
            stream.SendNext(JobNumber);
            stream.SendNext(NickName);
        }
        else
        {
            // 원격 플레이어의 데이터를 수신할 때
            RoomName = (string)stream.ReceiveNext();
            JobNumber = (string)stream.ReceiveNext();
            NickName = (string)stream.ReceiveNext();
        }
    }
    #endregion

    [PunRPC]
    public void SetNickName(string nickName)
    {
        StartNickName = nickName;
    }

    public void SaveData()
    {
        GameManager.instance.IsWin = IsWin;
        GameManager.instance.NickName = NickName;
        GameManager.instance.CharacterName = CharacterName;
        GameManager.instance.StartNickName = StartNickName;
        GameManager.instance.RoomName = RoomName;
        GameManager.instance.JobNumber = JobNumber;
        GameManager.instance.IsEscape = IsEscape;
        GameManager.instance.CountSaveOther = CountSaveOther;
        GameManager.instance.CountCaptured = CountCaptured;
        GameManager.instance.CountCloseValve = CountCloseValve;
        GameManager.instance.CountBreakPot = CountBreakPot;
        GameManager.instance.CountUseTowel = CountUseTowel;
        GameManager.instance.CountCaptureOther = CountCaptureOther;
        GameManager.instance.CountOpenValve = CountOpenValve;
        GameManager.instance.CountBeHitted = CountBeHitted;
        GameManager.instance.CountHitOther = CountHitOther;
        GameManager.instance.CountNotWalk = CountNotWalk;
        GameManager.instance.CountWalkSlowly = CountWalkSlowly;
    }
}
