using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    public void SaveScreenShot()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string fileName = "COOKSCAPE-SCREENSHOT-" + timestamp + ".png";
        
        #if UNITY_IPHONE || UNITY_ANDROID
        CaptureScreenForMobile(fileName);
        #else
        StartCoroutine("CaptureScreenForPC", fileName);
        #endif
    }

    private IEnumerator CaptureScreenForPC(string fileName)
    {
        string savepath = $"{Application.dataPath}/ScreenShot/";
        if (!Directory.Exists(savepath)) {
            Directory.CreateDirectory(savepath);
        }
        ScreenCapture.CaptureScreenshot(savepath + fileName);
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.Alert("Alert", "스크린샷이 저장되었습니다.");
    }

    private void CaptureScreenForMobile(string fileName)
    {
        Debug.LogError("모바일은 지원하지 않는 게임입니다.");
    }
}
