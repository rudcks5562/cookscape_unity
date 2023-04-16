using System.Collections;
using System.Collections.Generic;
using UnityProject.Cookscape;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
     // Singleton
    public static GameManager instance = null;

    // Version Data
    public VersionData version = null;

    // Authorization(Token)
    private string authorization = "";

    // Item Data
    public Dictionary<string, ItemData> item = null;

    // Object Data
    public Dictionary<string, ObjectData> mapObject = null;

    // Avatar Data
    public Dictionary<string, AvatarData> avatar = null;

    // Challenge Data
    public Dictionary<string, ChallengeData> challenge = null;

    // Reward Data
    public Dictionary<string, RewardData> reward = null;

    // User Possession Reward Data
    public Dictionary<string, RewardData> userHaveReward = null;

    // User Not Possession Reward Data
    public Dictionary<string, RewardData> userNotHaveReward = null;

    // User Data
    public UserData userData = null;

    // User
    public User user = null;

    // User Usage Avater
    public Dictionary<string, UsageAvatarData> usageAvatar = null;
    public AvatarData currAvatar = null; // ingame avatar

    #region DEBUG
    public void PrintItem()
    {
        foreach(ItemData item in item.Values)
        {
            item.Print();
        }
    }

    public void PrintObject()
    {
        foreach(ObjectData mapObject in mapObject.Values)
        {
            mapObject.Print();
        }
    }

    public void PrintAvatar()
    {
        foreach (AvatarData avatar in avatar.Values)
        {
            avatar.Print();
        }
    }

    public void PrintUsageAvatar()
    {
        foreach (UsageAvatarData usageAvatar in usageAvatar.Values)
        {
            usageAvatar.Print();
        }
    }

    public void PrintChallenge()
    {
        foreach (ChallengeData challenge in challenge.Values)
        {
            challenge.Print();
        }
    }

    public void PrintReward()
    {
        foreach (RewardData reward in reward.Values)
        {
            reward.Print();
        }
    }

    public void PrintHaveReward()
    {
        foreach (RewardData reward in userHaveReward.Values)
        {
            reward.Print();
        }
    }

    public void PrintNotHaveReward()
    {
        foreach (RewardData reward in userNotHaveReward.Values)
        {
            reward.Print();
        }
    }
    #endregion

    public GameObject playerCamera; // USE FOR NICKNAME VIEW CAMERA
    public GameObject player;

    public Transform LoginUI;
    public Transform GameUI;
    public Transform CurrentDescription;
    public Transform CircularProgressBar;
    public Transform InteractionBundle;
    public Transform FoodStatus;
    public Transform[] FoodStatusWater;
    public Transform[] FoodStatusFalse;
    public Transform PotStatus;
    public Transform[] PotStatusTrue;
    public Transform[] PotStatusFalse;
    public Transform[] PotStatusValve;
    public Transform GameMap;

    public Transform MetabusUI;

    public Slider m_StickStaminaBar;
    public GameObject m_CircularProgressBar;

    // UI FOR INTERACTION WITH BOTH PLAY AND INTERACTABLE OBJECT
    public GameObject m_CurrentDesc;
    public GameObject m_InteractionBundle;
    public GameObject m_FoodStatus;
    public GameObject m_PotStatus;
    public GameObject m_GameMap;
    public GameObject m_GameMapCamera;
    public GameObject m_AlertScreen;

    public string m_NowRoomMyIn;
    public string m_NextRoomName;

    public bool m_IsChef;

    public string m_MyGameAvatar;

    public GameObject m_ExitMenu;

    #region PRIVATE VARIABLES
    private bool m_IsAlertShowing;
    public bool m_IsMapExpanded;
    private bool m_IsTimerActive;
    private InputHandler m_InputHandler;
    #endregion

    #region GAME END DATA

    public bool IsWin;

    public string NickName;
    public string CharacterName;

    public string StartNickName;

    public string RoomName;
    public string JobNumber;

    //escape?
    public bool m_ChatInputFocused = false;
    public bool m_CameraLock = false;
    public bool m_MovementLock = false;
    public bool IsEscape = false;
    //cnt save other
    public int CountSaveOther = 0;
    //cnt captured
    public int CountCaptured = 0;
    //cnt valve
    public int CountCloseValve = 0;
    //cnt pot
    public int CountBreakPot = 0;

    public int CountUseTowel = 0;

    public int CountCaptureOther = 0;

    public int CountOpenValve = 0;

    public int CountBeHitted = 0;

    public float CountWalkSlowly = 0f;

    public float CountNotWalk = 0f;

    //2222
    public int BreakPotData = 0;

    public int CatchedManData = 0;

    public int CountHitOther = 0;

    #endregion

    // Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            m_InputHandler = InputHandler.instance;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        m_IsAlertShowing = false;
        m_IsTimerActive = false;

        Screen.SetResolution(1600, 900, true);
        Application.runInBackground = true;
        PhotonNetwork.AutomaticallySyncScene = true;
        AlertBoxBtnAddEvent();
    }

    void Update()
    {
        if (m_InputHandler.GetMKeyInputDown()) {
            ToggleGameMap();
        }
    }

    #region AUTHORIZATION
    public void SetAuthorization(string token)
    {
        this.authorization = token;
    }

    public string GetAuthorization()
    {
        return this.authorization;
    }
    #endregion

    #region INGAME MAP
    public void ToggleGameMap()
    {
        if (!m_IsMapExpanded) {
            m_GameMap.SetActive(true);
        } else {
            m_GameMap.SetActive(false);
        }
        m_IsMapExpanded = !m_IsMapExpanded;
    }
    #endregion

    #region INGAME GAUGE
    public void SetGauge(float val)
    {
        Image loadingBar = m_CircularProgressBar.transform.Find("Center").gameObject.GetComponent<Image>();
        loadingBar.fillAmount = val / 100;

        TextMeshProUGUI progressIndicator = m_CircularProgressBar.transform.Find("ProgressIndicator").gameObject.GetComponent<TextMeshProUGUI>();
        progressIndicator.text = ((int)val + "");
    }

    public void ShowGaugeInfo(GameObject target)
    {
        string type = target.GetComponent<MapObject>().GetObjectName();
        // type: { Valve, Pot, Chopstick, Towel }
        m_CircularProgressBar.gameObject.SetActive(true);
        GameObject iconImage = m_CircularProgressBar.transform.Find("Icon Image").gameObject;

        switch (type) {
            case "밸브":
                iconImage.transform.Find("Valve").gameObject.SetActive(true);
                break;
            case "냄비":
                iconImage.transform.Find("Pot").gameObject.SetActive(true);
                break;
            case "젓가락":
                iconImage.transform.Find("Chopstick").gameObject.SetActive(true);
                break;
            case "키친타올":
                iconImage.transform.Find("Towel").gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void HideGaugeInfo()
    {
        m_CircularProgressBar.gameObject.SetActive(false);
        GameObject iconImage = m_CircularProgressBar.transform.Find("Icon Image").gameObject;
        foreach (Transform child in iconImage.transform) {
            child.gameObject.SetActive(false);
        }
    }
    #endregion

    #region INGAME STAMINA
    public void SetStamina(float val)
    {
        m_StickStaminaBar.value = val;
    }

    public void ShowStaminaBar()
    {
        m_StickStaminaBar.gameObject.SetActive(true);
    }

    public void HideStaminaBar()
    {
        m_StickStaminaBar.gameObject.SetActive(false);
    }
    #endregion

    #region TIMER
    public void SetTimer(string title, string msg, float time)
    {
        if (m_IsTimerActive) {
            HideInfoMsg();
        }

        m_IsTimerActive = true;
        ShowTimerMsg(time);
        SetInfoMsg(title, msg);
        StartCoroutine("TimerCount", time);
    }

    private IEnumerator TimerCount(float time)
    {
        while (time >= 0) {
            SetTimerText(time);
            time -= Time.deltaTime;
            yield return null;
        }
        if (time <= 0)
            m_IsTimerActive = false;
            StartCoroutine("HideTimerMsg");
    }
    #endregion

    #region INGAME KEY GUIDE
    public void ShowKeyGuide(string type)
    {
        // type { Catch, Throw, Valve, Push, Pick, Change, Dry, Save, Attack }
        m_InteractionBundle.transform.Find(type).gameObject.SetActive(true);
    }

    public void HideKeyGuide()
    {
        foreach(Transform child in m_InteractionBundle.transform) {
            child.gameObject.SetActive(false);
        }
    }
    #endregion

    #region INGAME INFO MESSAGE
    public void AlertInfoMsg(bool isForAll, string title, string msg)
    {
        // IF PREVIOUS ALERT MSG IS SHOWING
        if (m_IsAlertShowing || m_IsTimerActive) return;

        if (isForAll) {
            SetAlertMsg(title, msg);
            StartCoroutine("AlertInfoMsgToAll", msg);
        } else {
            SetInfoMsg(title, msg);
            StartCoroutine("ShowInfoMsg", 12.0f);
        }
        // StartCoroutine("FadeTextToFullAlpha");
        // yield return new WaitForSeconds(4.0f);
    }

    private void SetAlertMsg(string title, string msg)
    {
        TextMeshProUGUI mainText = m_CurrentDesc.transform.Find("Main Category Text").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI subText = m_CurrentDesc.transform.Find("Sub Category Text").gameObject.GetComponent<TextMeshProUGUI>();
        mainText.text = title;
        subText.text = msg;
    }

    private void SetInfoMsg(string title, string msg)
    {
        TextMeshProUGUI mainText = m_CurrentDesc.transform.Find("Main Category Text").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI subText = m_CurrentDesc.transform.Find("Sub Category Text").gameObject.GetComponent<TextMeshProUGUI>();
        mainText.text = title;
        subText.text = msg;
    }

    private void SetTimerText(float time)
    {
        TextMeshProUGUI timerText = m_CurrentDesc.transform.Find("Sub Category Text").gameObject.transform.Find("Timer Text").gameObject.GetComponent<TextMeshProUGUI>();
        timerText.text = (int)time + "";
    }

    private IEnumerator ShowInfoMsg(float duration)
    {
        m_CurrentDesc.SetActive(true);
        StartCoroutine("FadeTextToFullAlpha");
        yield return new WaitForSeconds(duration);

        // StartCoroutine("HideInfoMsg");
        HideInfoMsg();
    }
    

    private void HideInfoMsg()
    {
        // yield return null;
        m_CurrentDesc.SetActive(false);
        
        StopCoroutine(nameof(ShowInfoMsg));
        StopCoroutine(FadeTextToFullAlpha());
        StopCoroutine(FadeTextToZeroAlpha());
        if (m_IsAlertShowing) m_IsAlertShowing = false;
    }

    private void ShowTimerMsg(float duration)
    {
        TextMeshProUGUI mainText = m_CurrentDesc.transform.Find("Main Category Text").gameObject.GetComponent<TextMeshProUGUI>();
        GameObject timerText = m_CurrentDesc.transform.Find("Sub Category Text").gameObject.transform.Find("Timer Text").gameObject;
        m_CurrentDesc.SetActive(true);
        m_CurrentDesc.transform.Find("Main Background").GetComponent<Image>().color = new Color(0.4f, 0, 0);
        mainText.color = new Color(255, 255, 255);
        timerText.SetActive(true);

        StartCoroutine("FadeTextToFullAlpha");
    }
    

    private void HideTimerMsg()
    {
        TextMeshProUGUI mainText = m_CurrentDesc.transform.Find("Main Category Text").gameObject.GetComponent<TextMeshProUGUI>();
        GameObject timerText = m_CurrentDesc.transform.Find("Sub Category Text").gameObject.transform.Find("Timer Text").gameObject;
         m_CurrentDesc.transform.Find("Main Background").GetComponent<Image>().color = new Color(255, 255, 255);
        m_CurrentDesc.transform.Find("Main Background").GetComponent<Image>().color = new Color(0, 0, 0);
        m_CurrentDesc.SetActive(false);
        timerText.SetActive(false);

        StopCoroutine(FadeTextToFullAlpha());
        StopCoroutine(FadeTextToZeroAlpha());
    }

    public IEnumerator AlertInfoMsgToAll(string msg)
    {
        SetInfoMsg("알림", msg);
        if (m_IsAlertShowing) {
            StopCoroutine("ShowInfoMsg");
            // yield return StartCoroutine("HideInfoMsg");
            HideInfoMsg();
        }
        yield return null;
        m_IsAlertShowing = true;

        StartCoroutine("ShowInfoMsg", 16.0f);
    }

    public IEnumerator FadeTextToFullAlpha()
    {
        TextMeshProUGUI subText = m_CurrentDesc.transform.Find("Sub Category Text").gameObject.GetComponent<TextMeshProUGUI>();
        subText.color = new Color(subText.color.r, subText.color.g, subText.color.b, 0.4f);
        while (subText.color.a < 1.0f)
        {
            subText.color = new Color(subText.color.r, subText.color.g, subText.color.b, subText.color.a + (Time.deltaTime / 2.0f));
            yield return null;
        }
        StartCoroutine(FadeTextToZeroAlpha());
    }

    public IEnumerator FadeTextToZeroAlpha()
    {
        TextMeshProUGUI subText = m_CurrentDesc.transform.Find("Sub Category Text").gameObject.GetComponent<TextMeshProUGUI>();
        subText.color = new Color(subText.color.r, subText.color.g, subText.color.b, 1);
        while (subText.color.a > 0.6f)
        {
            subText.color = new Color(subText.color.r, subText.color.g, subText.color.b, subText.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }
        StartCoroutine(FadeTextToFullAlpha());
    }
    #endregion

    public void CheckCharacter(string character, bool flag)
    {
        int idx = -1;
        switch (character)
        {
            case "tomato":
                idx = 0;
                break;
            case "bell pepper":
                idx = 1;
                break;
            case "meat":
                idx = 2;
                break;
            case "coke":
                idx = 3;
                break;
        }

        int check = 1 << idx;

        if ( flag)
        {
            if ((CatchedManData & check) == 0)
            {
                CatchedManData += check;
                GameFlowManager.instance.m_MyPlayInfo.CountCaptureOther++;
            }
        }
        else
        {
            if ((CatchedManData & check) != 0)
            {
                CatchedManData -= check;
                GameFlowManager.instance.m_MyPlayInfo.CountCaptureOther--;
            }
        }
    }

    public void DayTimeControllerOn()
    {
        gameObject.GetComponent<DayTimeController>().ActiveDayTimeController();
    }

    public void DayTimeControllerOff()
    {
        gameObject.GetComponent<DayTimeController>().DisableDayTimeController();
    }

    #region OPEN UI
    public void OpenGameUI()
    {
        GameUI.gameObject.SetActive(true);
        m_GameMapCamera.gameObject.SetActive(true);
    }

    #endregion

    #region CLOSE UI
    public void CloseGameUI()
    {
        CurrentDescription.gameObject.SetActive(false);
        CircularProgressBar.gameObject.SetActive(false);
        foreach (Transform child in InteractionBundle) {
            child.gameObject.SetActive(false);
        }
        foreach (Transform water in FoodStatusWater) {
            foreach (Transform child in water) {
                child.gameObject.SetActive(false);
            }
        }
        foreach (Transform el in FoodStatusFalse) {
            el.gameObject.SetActive(false);
        }
        foreach (Transform el in PotStatusTrue) {
            el.gameObject.SetActive(true);
        }
        foreach (Transform el in PotStatusFalse) {
            el.gameObject.SetActive(false);
        }
        foreach (Transform el in PotStatusValve) {
            el.gameObject.SetActive(true);
        }
        GameMap.gameObject.SetActive(false);
        m_IsMapExpanded = false;
        m_GameMapCamera.gameObject.SetActive(false);
        GameUI.gameObject.SetActive(false);
    }

    public void CloseValveUI(int potIndex)
    {
        GameObject pot = m_PotStatus.transform.Find($"Pot{potIndex + 1}").gameObject;
        pot.transform.Find("Valve").gameObject.SetActive(false);
    }
    #endregion

    #region UPDATE UI
        public void UI_UpdatePot(int potIndex)
    {
        GameObject pot = m_PotStatus.transform.Find($"Pot{potIndex + 1}").gameObject;
        pot.transform.Find("True").gameObject.SetActive(false);
        pot.transform.Find("False").gameObject.SetActive(true);

        int check = 1 << potIndex;
        if ( (BreakPotData & check) == 0)
            BreakPotData += check;
    }

    public void UI_UpdateCharacter(string character, int level)
    {
        Transform portrait;
        Transform water;
        for ( int i = 1; i <= 4; i++ )
        {
            portrait = m_FoodStatus.transform.Find($"Pot{i}");
            water = portrait.Find("Water");
            if (portrait.Find("Portrait").Find(character).gameObject.activeInHierarchy)
            {
                

                if ( level != 5 && level != -1 )
                {
                    CheckCharacter(character, true);

                    water.Find($"{level * 20}").gameObject.SetActive(true);
                }
                else
                {
                    if (level == 5)
                    {
                        portrait.Find("False").gameObject.SetActive(true);
                    }
                    else
                    {
                        CheckCharacter(character, false);

                        for (int j = 0; j < 5; j++)
                        {
                            water.Find($"{j * 20}").gameObject.SetActive(false);
                        }                 
                    }
                }
                break;
            }
        }
    }
    #endregion

    #region TOGGLE NICKNAME UI
    public void ShowViewNickname()
    {
        if (playerCamera == null) return;
        Camera mainCamera = playerCamera.transform.Find("Main Camera").GetComponent<Camera>();
        mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("ChefNickname");
        mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("RunnerNickname");
    }

    public void HideViewNickname()
    {
        if (playerCamera == null) return;
        Camera mainCamera = playerCamera.transform.Find("Main Camera").GetComponent<Camera>();
        // playerCamera.GetComponent<Camera>().cullingMask = playerCamera.GetComponent<Camera>().cullingMask & ~(1 << LayerMask.NameToLayer("Nickname"));
    }

    public void HideChefNickname()
    {
        if (playerCamera == null) return;
        Camera mainCamera = playerCamera.transform.Find("Main Camera").GetComponent<Camera>();
        mainCamera.cullingMask = mainCamera.cullingMask & ~(1 << LayerMask.NameToLayer("ChefNickname"));
    }

    public void HideRunnerNickname()
    {
        if (playerCamera == null) return;
        Camera mainCamera = playerCamera.transform.Find("Main Camera").GetComponent<Camera>();
        mainCamera.cullingMask = mainCamera.cullingMask & ~(1 << LayerMask.NameToLayer("RunnerNickname"));
    }
    #endregion

    #region TOGGLE LOCK PLAYER CONTROLL
    public void LockControll()
    {
        m_CameraLock = true;
        m_MovementLock = true;
    }

    public void UnlockControll()
    {
        m_CameraLock = false;
        m_MovementLock = false;
    }
    #endregion

    #region ALERT BOX
    private void ShowAlertBox()
    {
        m_AlertScreen.SetActive(true);
    }

    private void HideAlertBox()
    {
        m_AlertScreen.SetActive(false);
    }

    private void AlertBoxBtnAddEvent()
    {
        Transform btn_bundle = m_AlertScreen.transform.Find("Button Bundle");

        Button btn_exit = btn_bundle.Find("Yes or No").Find("Exit").GetComponent<Button>();
        Button btn_No = btn_bundle.Find("Yes or No").Find("No").GetComponent<Button>();
        Button btn_confirm = btn_bundle.Find("Confirm").GetComponent<Button>();
        Button btn_close = btn_bundle.Find("Close").GetComponent<Button>();

        btn_exit.onClick.AddListener(ExitGame);
        btn_No.onClick.AddListener(HideAlertBox);
        btn_confirm.onClick.AddListener(HideAlertBox);
        btn_close.onClick.AddListener(HideAlertBox);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    public void Alert(string type, string msg)
    {
        Transform icon = m_AlertScreen.transform.Find("Icon");
        TextMeshProUGUI txt_msg = m_AlertScreen.transform.Find("Describe").gameObject.GetComponent<TextMeshProUGUI>();
        Transform btn_bundle = m_AlertScreen.transform.Find("Button Bundle");

        foreach (Transform child in icon) {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in btn_bundle) {
            child.gameObject.SetActive(false);
        }

        switch (type) {
            case "Alert":
                btn_bundle.Find("Confirm").gameObject.SetActive(true);
                break;
            case "Block":
                btn_bundle.Find("Close").gameObject.SetActive(true);
                break;
            case "Danger":
                btn_bundle.Find("Yes or No").gameObject.SetActive(true);
                break;
        }
        txt_msg.text = msg;
        icon.Find(type).gameObject.SetActive(true);
        ShowAlertBox();
    }
    #endregion
}
