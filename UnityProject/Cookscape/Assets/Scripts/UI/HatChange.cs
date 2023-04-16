using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityProject.Cookscape;

public class HatChange : MonoBehaviour
{
    public Toggle setHatToggle;

    public void EquipedHat()
    {
        User user = GameManager.instance.user;
        if (setHatToggle.isOn)
        {
            user.hat = RewardBookScript.instance.NowKeyValue;
        }
        else
        {
            RewardBookScript.instance.NowKeyValue = "NONE";
            user.hat = "NONE";
        }

        GameManager.instance.player.GetComponent<PlayerController>().SetHat();
        
        Debug.Log($"{user.hat} ¿Â¬¯«ÿ¡¶");
        StartCoroutine(UnityProject.Cookscape.Api.User.instance.UpdateUser(new UserUpdateForm(user.avatarName, user.title, user.hat)));
    }
}
