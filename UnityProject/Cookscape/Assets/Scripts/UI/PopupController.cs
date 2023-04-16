using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public void AlertExit()
    {
        GameManager.instance.Alert("Danger", "종료하시겠습니까?");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void PopupOpen()
    {
        GameManager.instance.LockControll();
    }

    public void PopupOpen(GameObject target)
    {
        target.SetActive(true);
        GameManager.instance.LockControll();
    }

    public void PopupClose()
    {
        GameManager.instance.UnlockControll();
    }

    public void PopupClose(GameObject target)
    {
        target.SetActive(false);
        GameManager.instance.UnlockControll();
    }

    public void ToggleScreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
