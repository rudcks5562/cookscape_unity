using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityProject.Cookscape;
using UnityProject.Cookscape.Api;

public class NicknameCardController : MonoBehaviour
{
    public TitlePrefabScript nickname;
    public Toggle setNicknameToggle;

    public void CardAnimation()
    {
        if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.y < -30 )
        {
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
        }
        else
        {
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -42);
        }
    }

    public void NicknameChange()
    {
        string changeNickname = nickname.m_Title.text;
        UnityProject.Cookscape.User user = GameManager.instance.user;
        if (setNicknameToggle.isOn)
        {
            Debug.Log(changeNickname + "���� Īȣ�� �����մϴ�");
            user.title = nickname.m_KeyValue;
            MetabusManager.instance.SetUserTitle();
            StartCoroutine(UnityProject.Cookscape.Api.User.instance.UpdateUser(new UserUpdateForm(user.avatarName, user.title, user.hat)));
        } 
        else
        {
            Debug.Log("�������� Īȣ " + changeNickname + "�� �����մϴ�");
            user.title = "NONE";
            MetabusManager.instance.SetUserTitle();
            StartCoroutine(UnityProject.Cookscape.Api.User.instance.UpdateUser(new UserUpdateForm(user.avatarName, user.title, user.hat)));
        }
    }
}
