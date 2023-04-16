using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityProject.Cookscape;
using UnityProject.Cookscape.Api;

public class MetabusManager : MonoBehaviour
{

    public static MetabusManager instance;

    public Transform MetaverseUICanvas;
    public Transform MetaverseUICanvas_Profile;
    public Transform MetaverseUICanvas_MiniMap;
    public Transform MetaverseUICanvas_ActionList;
    public Transform MetaverseUICanvas_Chatting;
    public Transform MetaverseUICanvas_AlertScreen;
    public Transform MetaverseUICanvas_ExternalContent;

    public GameObject m_RoomBook;

    ProfilePrefabScript m_ProfileScript;
    TMP_Text m_TitleSpace;

    GameObject m_RewardBook;
    RewardBookScript rewardBook;

    GameObject m_NickNamePage;
    Transform m_NickNamePage_ContentBox;
    GameObject m_NickNamePage_Content;

    GameObject m_ClosetPage;
    Transform m_ClosetPage_ContentBox;
    GameObject m_ClosetPage_Content;

    SoloChallengeCardScript soloChallengeCardScript;

    GameObject m_ChallengeSoloPage;
    Transform m_ChallengeSoloPage_ContentBox;
    GameObject m_ChallengeSoloPage_Content;

    StepChallengeCardScript stepChallengeCardScript;

    GameObject m_ChallengeStepPage;
    Transform m_ChallengeStepPage_ContentBox;
    GameObject m_ChallengeStepPage_Content;

    List<GameObject> Titles = new();
    List<GameObject> Hats = new();
    List<GameObject> Challenges = new();

    const int SILVER_SCORE = 1500;
    const int GOLD_SCORE = 2000;

    void Awake()
    {
        GameObject mainCameraArm = GameObject.FindGameObjectWithTag("MainCameraArm");
        GameManager.instance.playerCamera = mainCameraArm;

        if  (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        GameManager.instance.DayTimeControllerOn();
        GameManager.instance.m_IsChef = false;

        if (GetComponent<MetabusSceneBGM>()) {
            GetComponent<MetabusSceneBGM>().Play();
        }

        if ( PhotonNetwork.IsMasterClient )
        {
            PhotonNetwork.InstantiateRoomObject("Prefabs/Map/MetaverseDynamics/Ferris_wheel_01", new Vector3(-152.801f, -1.200001f, 227.3993f), Quaternion.Euler(new Vector3(0, -0.291f, 0)));
            PhotonNetwork.InstantiateRoomObject("Prefabs/Map/MetaverseDynamics/free-fall-ride", new Vector3(-160.6f, 2.1f, 183.9f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("Prefabs/Map/MetaverseDynamics/HotAirBalloon_Red", new Vector3(-121.7428f, 0.8942776f, 244.3848f), Quaternion.Euler(new Vector3(0, -0.291f, 0)));
            PhotonNetwork.InstantiateRoomObject("Prefabs/Map/MetaverseDynamics/RollerCosters", new Vector3(1568.926f, 555.1f, -1005.345f), Quaternion.Euler(new Vector3(0, -0.291f, 0)));
            PhotonNetwork.InstantiateRoomObject("Prefabs/Map/MetaverseDynamics/TeaCups", new Vector3(2420.304f, 554.9f, -902.8892f), Quaternion.Euler(new Vector3(0, -0.291f, 0)));
            PhotonNetwork.InstantiateRoomObject("Prefabs/Map/MetaverseDynamics/Yacht_02 (6)", new Vector3(-23.57403f, -0.1999989f, 538.5999f), Quaternion.Euler(new Vector3(0, -0.291f, 0)));
            PhotonNetwork.InstantiateRoomObject("Prefabs/Map/MetaverseDynamics/Marry_Go_Around", new Vector3(2454.843f, 554.8f, -868.44f), Quaternion.Euler(new Vector3(0, -0.291f, 0)));
        }

        MetaverseUICanvas = GameManager.instance.transform.Find("UI_Metabus");
        MetaverseUICanvas_Profile = MetaverseUICanvas.Find("Profile");
        MetaverseUICanvas_MiniMap = MetaverseUICanvas.Find("MiniMap");
        MetaverseUICanvas_ActionList = MetaverseUICanvas.Find("Action List");
        MetaverseUICanvas_Chatting = MetaverseUICanvas.Find("Chatting");
        MetaverseUICanvas_AlertScreen = MetaverseUICanvas.Find("Alert Screen");
        MetaverseUICanvas_ExternalContent = MetaverseUICanvas.Find("External Content");

        //set profile
        m_ProfileScript = MetaverseUICanvas_Profile.GetComponent<ProfilePrefabScript>();

        m_ProfileScript.NICKNAME.text = GameManager.instance.user.nickname;
        m_TitleSpace = m_ProfileScript.USER_TITLE;
        SetUserTitle();
        SetUserRank();

        //set rewards
        m_RewardBook = MetaverseUICanvas_ExternalContent.Find("Reward Book").gameObject;

        rewardBook = m_RewardBook.GetComponent<RewardBookScript>();

        //titles
        m_NickNamePage = rewardBook.NicknamePage;
        m_NickNamePage_ContentBox = rewardBook.NicknamePage_ContentBox.transform;
        m_NickNamePage_Content = rewardBook.NicknamePage_Content;

        //hats
        m_ClosetPage = rewardBook.ClosetPage;
        m_ClosetPage_ContentBox = rewardBook.ClosetPage_ContentBox.transform;
        m_ClosetPage_Content = rewardBook.ClosetPage_Content;

        MakeTitleList(3);
        MakeHatList(3);

        //set challenges

        m_ChallengeSoloPage = rewardBook.ChallengeSoloPage;
        m_ChallengeSoloPage_ContentBox = rewardBook.ChallengeSoloPage_ContentBox.transform;
        m_ChallengeSoloPage_Content = rewardBook.ChallengeSoloPage_Content;

        m_ChallengeStepPage = rewardBook.ChallengeStepPage;
        m_ChallengeStepPage_ContentBox = rewardBook.ChallengeStepPage_ContentBox.transform;
        m_ChallengeStepPage_Content = rewardBook.ChallengeStepPage_Content;

        MakeChallengeList();
    }

    public void SetUserTitle()
    {
        string title = GameManager.instance.user.title;

        if ( title.Equals("NONE") || title.Equals(string.Empty))
        {
            m_TitleSpace.text = string.Empty;
            return;
        }

        m_TitleSpace.text = GameManager.instance.reward[title].name;
    }

    public void SetUserRank()
    {
        int rankPoint = GameManager.instance.userData.rankPoint;

        m_ProfileScript.RANK_POINT.text = "" + rankPoint;

        if ( rankPoint < SILVER_SCORE)
        {
            m_ProfileScript.SetBronzeTier();
            m_ProfileScript.RANK_SLIDER.value = Mathf.Lerp(0, 100, 1f * rankPoint / SILVER_SCORE);
        }
        else if (rankPoint < GOLD_SCORE)
        {
            m_ProfileScript.SetSilverTier();
            m_ProfileScript.RANK_SLIDER.value = Mathf.Lerp(0, 100, 1f * (rankPoint - SILVER_SCORE) / (GOLD_SCORE - SILVER_SCORE));
        }
        else
        {
            m_ProfileScript.SetGoldTier();
            m_ProfileScript.RANK_SLIDER.value = 100;
        }
    }

    void MakeChallengeList()
    {
        var challenges = GameManager.instance.challenge;

        GameObject tmp;
        SoloChallengeCardScript solo;
        StepChallengeCardScript step;

        var Rewards = GameManager.instance.reward;

        foreach( var item in Challenges)
        {
            item.SetActive(false);
            Destroy(item);
        }

        Challenges.Clear();

        RewardData reward;
        foreach (var item in challenges.Values)
        {
            if ( item.singleAchievementFlag)
            {
                tmp = Instantiate(m_ChallengeSoloPage_Content, m_ChallengeSoloPage_ContentBox);
                solo = tmp.GetComponent<SoloChallengeCardScript>();
                solo.m_Title.text = item.name;
                solo.m_Describe.text = item.desc;

                reward = Rewards[item.firstLevel];

                solo.m_RewardName.text = reward.name;

                if (reward.type.Equals(nameof(RewardData.TYPE.TITLE)))
                {
                    solo.m_NicknameIcon.SetActive(true);
                    solo.m_ClosetIcon.SetActive(false);
                }
                else
                {
                    solo.m_ClosetIcon.SetActive(true);
                    solo.m_NicknameIcon.SetActive(false);
                }

                if ( item.firstAchievementFlag)
                {
                    solo.m_FalseButton.SetActive(false);
                    solo.m_TrueButton.SetActive(false);
                    solo.m_FinishButton.SetActive(true);
                }
                else
                {
                    solo.m_FalseButton.SetActive(true);
                    solo.m_TrueButton.SetActive(false);
                    solo.m_FinishButton.SetActive(false);
                }

                tmp.SetActive(true);
                Challenges.Add(tmp);
            }
            else
            {
                tmp = Instantiate(m_ChallengeStepPage_Content, m_ChallengeStepPage_ContentBox);
                step = tmp.GetComponent<StepChallengeCardScript>();

                step.m_Title.text = item.name;
                step.m_Describe.text = item.desc;

                SetStepChallengeValue(step, item);

                reward = Rewards[item.firstLevel];

                if (reward.type.Equals(nameof(RewardData.TYPE.TITLE)))
                {
                    step.m_ClosetIcon_1.SetActive(false);
                    step.m_TitleIcon_1.SetActive(true);
                }
                else
                {
                    step.m_ClosetIcon_1.SetActive(true);
                    step.m_TitleIcon_1.SetActive(false);
                }

                if (item.firstAchievementFlag)
                {
                    step.m_TrueButton_1.SetActive(false);
                    step.m_FalseButton_1.SetActive(false);
                    step.m_FinishButton_1.SetActive(true);

                    step.m_Step1Slider.value = 100;
                }
                else
                {
                    step.m_TrueButton_1.SetActive(false);
                    step.m_FalseButton_1.SetActive(true);
                    step.m_FinishButton_1.SetActive(false);

                    step.m_Step1Slider.value = 0;
                }

                step.m_Name_1.text = reward.name;

                //step.m_Step1Count;
                //step.m_Step1Slider;

                reward = Rewards[item.secondLevel];

                if (reward.type.Equals(nameof(RewardData.TYPE.TITLE)))
                {
                    step.m_ClosetIcon_2.SetActive(false);
                    step.m_TitleIcon_2.SetActive(true);
                }
                else
                {
                    step.m_ClosetIcon_2.SetActive(true);
                    step.m_TitleIcon_2.SetActive(false);
                }

                if (item.secondAchievementFlag)
                {
                    step.m_TrueButton_2.SetActive(false);
                    step.m_FalseButton_2.SetActive(false);
                    step.m_FinishButton_2.SetActive(true);

                    step.m_Step2Slider.value = 100;
                }
                else
                {
                    step.m_TrueButton_2.SetActive(false);
                    step.m_FalseButton_2.SetActive(true);
                    step.m_FinishButton_2.SetActive(false);

                    step.m_Step2Slider.value = 0;
                }

                step.m_Name_2.text = reward.name;

                //step.m_Step1Count;
                //step.m_Step1Slider;

                reward = Rewards[item.thirdLevel];

                if (reward.type.Equals(nameof(RewardData.TYPE.TITLE)))
                {
                    step.m_ClosetIcon_3.SetActive(false);
                    step.m_TitleIcon_3.SetActive(true);
                }
                else
                {
                    step.m_ClosetIcon_3.SetActive(true);
                    step.m_TitleIcon_3.SetActive(false);
                }

                if (item.thirdAchievementFlag)
                {
                    step.m_TrueButton_3.SetActive(false);
                    step.m_FalseButton_3.SetActive(false);
                    step.m_FinishButton_3.SetActive(true);

                    step.m_Step3Slider.value = 100;
                }
                else
                {
                    step.m_TrueButton_3.SetActive(false);
                    step.m_FalseButton_3.SetActive(true);
                    step.m_FinishButton_3.SetActive(false);

                    step.m_Step3Slider.value = 0;
                }

                step.m_Name_3.text = reward.name;

                //step.m_Step1Count;
                //step.m_Step1Slider;
                tmp.SetActive(true);

                Challenges.Add(tmp);
            }
        }
    }

    void SetStepChallengeValue(StepChallengeCardScript step, ChallengeData item)
    {
        switch (item.keyValue)
        {
            case nameof(ChallengeData.CHALLENGE.THECOOKSLAYER) :
                step.m_TotalCount.text = 8.ToString();
                step.m_UserCount.text = GameManager.instance.userData.maxCatchCount.ToString();

                step.m_Step1Count.text = 3.ToString();
                step.m_Step2Count.text = 5.ToString();
                step.m_Step3Count.text = 8.ToString();

                break;

            case nameof(ChallengeData.CHALLENGE.느긋하게걷기):
                step.m_TotalCount.text = 600.ToString();
                step.m_UserCount.text = GameManager.instance.userData.maxNotUseStaminaTime.ToString();

                step.m_Step1Count.text = 180.ToString();
                step.m_Step2Count.text = 300.ToString();
                step.m_Step3Count.text = 600.ToString();
                
                break;

            case nameof(ChallengeData.CHALLENGE.대체왜이래요):
                step.m_TotalCount.text = 100.ToString();
                step.m_UserCount.text = GameManager.instance.userData.hitedCount.ToString();

                step.m_Step1Count.text = 10.ToString();
                step.m_Step2Count.text = 50.ToString();
                step.m_Step3Count.text = 100.ToString();

                break;

            case nameof(ChallengeData.CHALLENGE.나어디있게):
                step.m_TotalCount.text = 300.ToString();
                step.m_UserCount.text = GameManager.instance.userData.maxNotMoveTime.ToString();

                step.m_Step1Count.text = 60.ToString();
                step.m_Step2Count.text = 180.ToString();
                step.m_Step3Count.text = 300.ToString();

                break;

            case nameof(ChallengeData.CHALLENGE.다음엔친구랑같이오거라):
                step.m_TotalCount.text = 8.ToString();
                step.m_UserCount.text = GameManager.instance.userData.maxNotCatchCount.ToString();

                step.m_Step1Count.text = 3.ToString();
                step.m_Step2Count.text = 5.ToString();
                step.m_Step3Count.text = 8.ToString();

                break;

            case nameof(ChallengeData.CHALLENGE.청결이최우선):
                step.m_TotalCount.text = 100.ToString();
                step.m_UserCount.text = GameManager.instance.userData.catchCount.ToString();

                step.m_Step1Count.text = 10.ToString();
                step.m_Step2Count.text = 50.ToString();
                step.m_Step3Count.text = 100.ToString();

                break;
        }
    }

    public void MakeTitleList(int status)
    {
        //status 0, 1, 2, 3
        //1 = have , 2 = have not
        int haveTitle = 0;
        int notHaveTitle = 0;

        var haveReward = GameManager.instance.userHaveReward;

        GameObject tmp;
        foreach(var item in Titles)
        {
            item.SetActive(false);
            Destroy(item);
        }

        Titles.Clear();

        TitlePrefabScript titleScript;

        foreach (var item in haveReward.Values)
        {
            if (item.type.Equals(nameof(RewardData.TYPE.TITLE)) && ((status & 1) != 0))
            {
                tmp = Instantiate(m_NickNamePage_Content, m_NickNamePage_ContentBox);
                titleScript = tmp.GetComponent<TitlePrefabScript>();
                titleScript.m_Title.text = item.name;
                titleScript.m_Description.text = item.desc;
                titleScript.m_TrueBackground.SetActive(true);
                titleScript.m_FalseBackground.SetActive(false);

                titleScript.m_TrueButton.SetActive(true);
                titleScript.m_FalseButton.SetActive(false);

                titleScript.m_KeyValue = item.keyValue;

                tmp.SetActive(true);
                
                haveTitle++;
                Titles.Add(tmp);
            }
        }

        var notHaveReward = GameManager.instance.userNotHaveReward;

        foreach (var item in notHaveReward.Values)
        {
            if (item.type.Equals(nameof(RewardData.TYPE.TITLE)) && ((status & 2) != 0))
            {
                tmp = Instantiate(m_NickNamePage_Content, m_NickNamePage_ContentBox);
                titleScript = tmp.GetComponent<TitlePrefabScript>();
                titleScript.m_Title.text = item.name;
                titleScript.m_Description.text = item.desc;
                titleScript.m_FalseBackground.SetActive(true);
                titleScript.m_TrueBackground.SetActive(false);

                titleScript.m_FalseButton.SetActive(true);
                titleScript.m_TrueButton.SetActive(false);

                titleScript.m_KeyValue = item.keyValue;

                tmp.SetActive(true);

                notHaveTitle++;
                Titles.Add(tmp);
            }
        }

        rewardBook.NicknamePage_Slider.value = Mathf.Lerp(0, 100, 1f * haveTitle / (haveTitle + notHaveTitle));
        rewardBook.NicknamePage_Percentage.text = $"{haveTitle}/{(haveTitle + notHaveTitle)}";
    }

    public void MakeHatList(int status)
    {
        int haveHat = 0;
        int notHaveHat = 0;

        var haveReward = GameManager.instance.userHaveReward;

        GameObject tmp;
        foreach(var item in Hats)
        {
            item.SetActive(false);
            Destroy(item);
        }

        Hats.Clear();

        HatPrefabScript hatScript;
        foreach (var item in haveReward.Values)
        {
            if (item.type.Equals(nameof(RewardData.TYPE.HAT)) && ((status & 1) != 0))
            {
                tmp = Instantiate(m_ClosetPage_Content, m_ClosetPage_ContentBox);
                hatScript = tmp.GetComponent<HatPrefabScript>();
                hatScript.m_HatName = item.name;
                hatScript.m_Description = item.desc;
                hatScript.m_IsHave = true;
                hatScript.m_KeyValue = item.keyValue;

                //icon???
                string iconName = "";
                switch (item.keyValue)
                {
                    case nameof(RewardData.REWARD.요리사모자):
                        iconName = "chef";
                        break;
                    case nameof(RewardData.REWARD.왕관):
                        iconName = "crown";
                        break;
                    case nameof(RewardData.REWARD.보글보글모자):
                        iconName = "pirate";
                        break;
                    case nameof(RewardData.REWARD.혹):
                        iconName = "mushroom";
                        break;
                    case nameof(RewardData.REWARD.GOD):
                        iconName = "elephant";
                        break;
                    case nameof(RewardData.REWARD.물음표의모자):
                        iconName = "party";
                        break;
                    default:
                        Debug.Log($"{item.keyValue}");
                        break;
                }

                foreach(var txt in hatScript.m_IconList)
                {
                    if (txt.name.Equals(iconName))
                    {
                        hatScript.m_Icon.sprite = Sprite.Create(txt, new Rect(0, 0, txt.width, txt.height), Vector2.one * 0.5f);
                        break;
                    }
                }
                tmp.SetActive(true);
                haveHat++;
                Hats.Add(tmp);
            }
        }
        var notHaveReward = GameManager.instance.userNotHaveReward;

        foreach (var item in notHaveReward.Values)
        {
            if (item.type.Equals(nameof(RewardData.TYPE.HAT)))
            {
                tmp = Instantiate(m_ClosetPage_Content, m_ClosetPage_ContentBox);

                hatScript = tmp.GetComponent<HatPrefabScript>();
                hatScript.m_HatName = item.name;
                hatScript.m_Description = item.desc;
                hatScript.m_IsHave = false;
                hatScript.m_KeyValue = item.keyValue;

                //icon???
                string iconName = "";
                switch (item.keyValue)
                {
                    case nameof(RewardData.REWARD.요리사모자):
                        iconName = "chef";
                        break;
                    case nameof(RewardData.REWARD.왕관):
                        iconName = "crown";
                        break;
                    case nameof(RewardData.REWARD.보글보글모자):
                        iconName = "pirate";
                        break;
                    case nameof(RewardData.REWARD.혹):
                        iconName = "mushroom";
                        break;
                    case nameof(RewardData.REWARD.GOD):
                        iconName = "elephant";
                        break;
                    case nameof(RewardData.REWARD.물음표의모자):
                        iconName = "party";
                        break;
                    default:
                        Debug.Log($"{item.keyValue}");
                        break;
                }

                foreach (var txt in hatScript.m_IconList)
                {
                    if (txt.name.Equals(iconName))
                    {
                        hatScript.m_Icon.sprite = Sprite.Create(txt, new Rect(0, 0, txt.width, txt.height), Vector2.one * 0.5f);
                        break;
                    }
                }

                tmp.SetActive(true);
                
                notHaveHat++;
                Hats.Add(tmp);
            }

        }

        rewardBook.ClosetPage_Slider.value = Mathf.Lerp(0, 100, 1f * haveHat / (haveHat + notHaveHat));
        rewardBook.ClosetPage_Percentage.text = $"{haveHat}/{(haveHat + notHaveHat)}";
    }
}
