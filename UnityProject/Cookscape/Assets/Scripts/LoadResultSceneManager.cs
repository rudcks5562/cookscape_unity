using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityProject.Cookscape;
using UnityProject.Cookscape.Api;

public class LoadResultSceneManager : MonoBehaviourPunCallbacks
{
    GameManager gameManager;

    public GameObject ProfileBox;
    public GameObject TierGold;
    public GameObject TierSilver;
    public GameObject TierBronze;

    public Slider RankSlider;
    public TMP_Text RankPoint;
    public TMP_Text AddRankPoint;

    public GameObject VictoryParticle;
    public GameObject DefeatParticle;

    public GameObject FoodCharacterBox;
    public List<GameObject> FoodCharacterList;

    public GameObject ChefCharacterBox;
    public List<GameObject> ChefCharacterList;

    public GameObject FoodResult;
    public GameObject ChefResult;

    public GameObject FoodResultText;
    public GameObject ChefResultText;

    const string SCORE_LIST_TEXT = "Score List";

    const string CATCH_FOOD_SCORE = "Catch Food";
    const string OPEN_VALVE_SCORE = "Open Valve";

    const string SAVE_FOOD_SCORE = "Save Food";
    const string USE_TOWEL_SCORE = "Use Towel";
    const string CLOSE_VALVE_SCORE = "Close Valve";
    const string PUSH_POT_SCORE = "Push Pot";

    const string POT_STATUS = "Pot Status";
    const string FOOD_STATUS = "Food Status";

    const int SILVER_SCORE = 1500;
    const int GOLD_SCORE = 2000;

    private int m_CalculatedRankPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.CloseGameUI();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Transform nameBox = ProfileBox.transform.Find("Name");
        nameBox.GetComponent<TextMeshProUGUI>().text = gameManager.NickName;
        nameBox.Find("Nickname").GetComponent<TextMeshProUGUI>().text = gameManager.reward[gameManager.user.title].name;

        m_CalculatedRankPoint = 0;

        //Im Chef
        if (gameManager.m_IsChef)
        {
            ChefCharacterBox.SetActive(true);
            ChefResult.SetActive(true);
            ChefResultText.SetActive(true);

            FoodCharacterBox.SetActive(false);
            FoodResult.SetActive(false);
            FoodResultText.SetActive(false);

            //Set Chef Score
            Transform scoreList = ChefResult.transform.Find(SCORE_LIST_TEXT);
            scoreList.Find(CATCH_FOOD_SCORE).Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = "" + gameManager.CountCaptureOther;
            scoreList.Find(OPEN_VALVE_SCORE).Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = "" + gameManager.CountOpenValve;

            //Set Catched Food Status
            //Set Pot Status
            int catchedManData = gameManager.CatchedManData;
            Transform foodStatus = ChefResult.transform.Find(FOOD_STATUS);

            for (int i = 0; i < foodStatus.childCount; i++)
            {
                int check = 1 << i;
                if ((catchedManData & check) != 0)
                {
                    Transform food = foodStatus.Find($"Food{i + 1}");
                    food.Find("False").gameObject.SetActive(true);
                }
            }

            //Set Chef Character
            foreach (var now in ChefCharacterList)
            {
                now.SetActive(true);
                if (gameManager.IsWin)
                    now.GetComponent<Animator>().SetBool("Victory", true);
                else
                    now.GetComponent<Animator>().SetBool("Death", true);
            }

            if (gameManager.IsWin)
            {
                VictoryParticle.SetActive(true);
                DefeatParticle.SetActive(false);

                ChefResultText.transform.Find("Hungry").gameObject.SetActive(false);
                ChefCharacterBox.transform.Find("Hungry Background").gameObject.SetActive(false);
            }
            else
            {
                VictoryParticle.SetActive(false);
                DefeatParticle.SetActive(true);

                ChefResultText.transform.Find("Perfect").gameObject.SetActive(false);
                ChefCharacterBox.transform.Find("Perfect Background").gameObject.SetActive(false);
            }

            int calculatedExp = 0;
            if (gameManager.IsWin) {
                calculatedExp += 50;
                m_CalculatedRankPoint += 30;
            } else {
                calculatedExp += 20;
                m_CalculatedRankPoint -= 50;
            }
            calculatedExp += gameManager.CountCaptureOther * 20;
            calculatedExp += gameManager.CountOpenValve * 15;

            m_CalculatedRankPoint += gameManager.CountCaptureOther * 10;
            m_CalculatedRankPoint += gameManager.CountOpenValve * 5;

            // Chef Game Result Update
            GameResultForm chefResult = new GameResultForm(
                gameManager.currAvatar.avatarId,
                gameManager.IsWin,
                calculatedExp, // exp calculate need
                m_CalculatedRankPoint, // rank point
                1, // level
                100, // money
                0, // save count
                gameManager.CountCaptureOther, // catch count
                0, // catched count
                gameManager.CountOpenValve, // valve open count
                0, // valve close count
                0, // pot destroy count
                0, //maxNotUseStaminaTime { get; set; }
                0,// maxNotMoveTime { get; set; }
                gameManager.CountCaptureOther, // maxCatchCount { get; set; }
                0, //public int hitedCount
                gameManager.CountHitOther - gameManager.CountCaptureOther
            );
            
            CheckChefChallenge();

            StartCoroutine(
                        // Update
                        Data.instance.UpdateGameResult(chefResult, gameManager.currAvatar.name));
        }
        //Im Food
        else
        {
            ChefCharacterBox.SetActive(false);
            ChefResult.SetActive(false);
            ChefResultText.SetActive(false);

            FoodCharacterBox.SetActive(true);
            FoodResult.SetActive(true);
            FoodResultText.SetActive(true);

            //Set Food Score
            Transform scoreList = FoodResult.transform.Find(SCORE_LIST_TEXT);
            scoreList.Find(SAVE_FOOD_SCORE).Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = "" + gameManager.CountSaveOther;
            scoreList.Find(USE_TOWEL_SCORE).Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = "" + gameManager.CountUseTowel;
            scoreList.Find(CLOSE_VALVE_SCORE).Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = "" + gameManager.CountCloseValve;
            scoreList.Find(PUSH_POT_SCORE).Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = "" + gameManager.CountBreakPot;

            //Set Pot Status
            int breakPotData = gameManager.BreakPotData;
            Transform potStatus = FoodResult.transform.Find(POT_STATUS);

            for (int i = 0; i < potStatus.childCount; i++)
            {
                int check = 1 << i;
                if ( (breakPotData & check) != 0)
                {
                    Transform pot = potStatus.Find($"Pot{i + 1}");
                    pot.Find("True").gameObject.SetActive(false);
                    pot.Find("False").gameObject.SetActive(true);
                }
            }

            //Set Food Character
            foreach (var now in FoodCharacterList)
            {
                if (now.name.ToLower().Equals(gameManager.m_MyGameAvatar))
                {
                    now.SetActive(true);

                    if (gameManager.IsEscape)
                        now.GetComponent<Animator>().SetBool("Victory", true);
                    else
                        now.GetComponent<Animator>().SetBool("Death", true);


                    break;
                }
            }

            if (gameManager.IsEscape)
            {
                VictoryParticle.SetActive(true);
                DefeatParticle.SetActive(false);

                FoodResultText.transform.Find("Washed").gameObject.SetActive(false);
                FoodCharacterBox.transform.Find("Washed Background").gameObject.SetActive(false);
            }
            else
            {
                VictoryParticle.SetActive(false);
                DefeatParticle.SetActive(true);

                FoodResultText.transform.Find("Escape").gameObject.SetActive(false);
                FoodCharacterBox.transform.Find("Escape Background").gameObject.SetActive(false);
            }

            int calculatedExp = 0;
            if (gameManager.IsEscape) {
                calculatedExp += 50;
                m_CalculatedRankPoint += 30;
            } else {
                calculatedExp += 20;
                m_CalculatedRankPoint -= 50;
            }
            calculatedExp += gameManager.CountSaveOther * 20;
            calculatedExp -= gameManager.CountCaptured * 10;
            calculatedExp += gameManager.CountCloseValve * 10;
            calculatedExp += gameManager.CountBreakPot * 15;

            m_CalculatedRankPoint += gameManager.CountSaveOther * 20;
            m_CalculatedRankPoint -= gameManager.CountCaptured * 15;
            m_CalculatedRankPoint += gameManager.CountCloseValve * 5;
            m_CalculatedRankPoint += gameManager.CountBreakPot * 5;

            // Runner Game Result Update
            GameResultForm runnerResult = new GameResultForm(
                gameManager.currAvatar.avatarId,
                gameManager.IsEscape,
                calculatedExp, // exp calculate need
                m_CalculatedRankPoint, // rank point
                1, // level
                100, // money
                gameManager.CountSaveOther, // save count
                0, // catch count
                gameManager.CountCaptured, // catched count
                0, // valve open count
                gameManager.CountCloseValve, // valve close count
                gameManager.CountBreakPot, // pot destroy count
                
                gameManager.CountWalkSlowly, //maxNotUseStaminaTime { get; set; }
                gameManager.CountNotWalk, // maxNotMoveTime { get; set; }

                0, // maxCatchCount { get; set; }
                gameManager.CountBeHitted, // hitedCount
                0
            );

            CheckFoodChallenge();

            StartCoroutine(
                        // Update
                        Data.instance.UpdateGameResult(runnerResult, gameManager.currAvatar.name));
        }

        SetUserRank();
    }

    public void SetUserRank()
    {
        int rankPoint = GameManager.instance.userData.rankPoint;
        if (m_CalculatedRankPoint < 0) {
            RankPoint.text = $"{rankPoint} {m_CalculatedRankPoint}";
            AddRankPoint.text = $" {m_CalculatedRankPoint}";
            AddRankPoint.color = Color.red;
        } else {
            RankPoint.text = $"{rankPoint} +{m_CalculatedRankPoint}";
            AddRankPoint.text = $" +{m_CalculatedRankPoint}";
            AddRankPoint.color = Color.blue;
        }

        if (rankPoint < SILVER_SCORE)
        {
            TierBronze.SetActive(true);
            TierSilver.SetActive(false);
            TierGold.SetActive(false);

            RankSlider.value = Mathf.Lerp(0, 100, 1f * (rankPoint + m_CalculatedRankPoint) / SILVER_SCORE);
        }
        else if (rankPoint < GOLD_SCORE)
        {
            TierBronze.SetActive(false);
            TierSilver.SetActive(true);
            TierGold.SetActive(false);

            RankSlider.value = Mathf.Lerp(0, 100, 1f * ((rankPoint + m_CalculatedRankPoint) - SILVER_SCORE) / (GOLD_SCORE - SILVER_SCORE));
        }
        else
        {
            TierBronze.SetActive(false);
            TierSilver.SetActive(false);
            TierGold.SetActive(true);

            RankSlider.value = 100;
        }
    }

    void CheckChefChallenge()
    {
        //check COOKING
        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.COOKING)) && gameManager.CountCaptureOther >= 3)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.COOKING)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.왕관)) && gameManager.CountCaptureOther >= 5)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.왕관)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.THECOOKSLAYER)) && gameManager.CountCaptureOther >= 8)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.THECOOKSLAYER)]));
        }

        //check Clean Is First
        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.아이깨끗해)) && gameManager.CountCaptureOther + gameManager.userData.catchedCount >= 10)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.아이깨끗해)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.연쇄세척마)) && gameManager.CountCaptureOther + gameManager.userData.catchedCount >= 50)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.연쇄세척마)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.보글보글모자)) && gameManager.CountCaptureOther + gameManager.userData.catchedCount >= 100)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.보글보글모자)]));
        }

        //check Mercy
        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.주방장)) && gameManager.CountHitOther - gameManager.CountCaptureOther >= 3)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.주방장)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.자비로운주방장)) && gameManager.CountHitOther - gameManager.CountCaptureOther >= 5)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.자비로운주방장)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.요리사모자)) && gameManager.CountHitOther - gameManager.CountCaptureOther >= 8)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.요리사모자)]));
        }
    }

    void CheckFoodChallenge()
    {
        //check Why are you doing here
        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.반죽)) && gameManager.userData.catchCount + gameManager.CountCaptureOther >= 10)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.반죽)]));
            //GetReward(RewardData.REWARD.반죽);
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.빨래)) && gameManager.userData.catchCount + gameManager.CountCaptureOther >= 50)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.빨래)]));
            //GetReward(RewardData.REWARD.빨래);
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.혹)) && gameManager.userData.catchCount + gameManager.CountCaptureOther >= 100)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.혹)]));
        }

        //check Walk Slowly
        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.양반)) && gameManager.CountWalkSlowly >= 180f)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.양반)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.대감)) && gameManager.CountCaptureOther >= 300f)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.대감)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.GOD)) && gameManager.CountCaptureOther >= 600f)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.GOD)]));
        }

        //check not Walk
        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.중급닌자)) && gameManager.CountWalkSlowly >= 60f)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.중급닌자)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.상급닌자)) && gameManager.CountCaptureOther >= 180f)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.상급닌자)]));
        }

        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.호카게)) && gameManager.CountCaptureOther >= 300f)
        {
            StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.호카게)]));
        }

        //check I AM LEGEND
        if (!gameManager.userHaveReward.ContainsKey(nameof(RewardData.REWARD.전설적인)))
        {
            int check;
            int cnt = 0;
            for (int i = 0; i < 4;  i++)
            {
                check = 1 << i;
                if ( (gameManager.CatchedManData & check) != 0 )
                {
                    cnt++;
                }
            }

            if ( gameManager.IsEscape && cnt == 3)
            {
                StartCoroutine(Data.instance.RegistReward(gameManager.reward[nameof(RewardData.REWARD.전설적인)]));
            }
        }
    }

    public void GoToChannelSelectScene()
    {
        //Application.Quit();

        StartCoroutine(nameof(EndAndRestartGame));
    }

    IEnumerator EndAndRestartGame()
    {
        PhotonNetwork.LeaveRoom();

        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }

        GameManager.instance.CloseGameUI();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneLoadingManager.LoadScene("ChannelChangeScene");
    }
}
