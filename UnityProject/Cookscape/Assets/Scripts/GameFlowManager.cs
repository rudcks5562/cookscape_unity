using ARPGFX;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityProject.Cookscape
{
    public class GameFlowManager : MonoBehaviourPunCallbacks
    {
        //Game Start
        //get Chef, Runners, Items Information

        //Method
        //endGame : game End -> SceneChange

        public static GameFlowManager instance;

        //Game Is Start??
        public bool m_IsStart = false;

        //Is Timer Active?
        public bool m_IsTimerActive = false;

        public GameObject m_Recipe;

        //
        float m_Timer;

        float m_StartTime;
        float m_EndTime;
        float m_ChefReadingTime = 15.0f;
        float m_ExitLimitTime = 60.0f;

        int m_BreakedPot;
        int m_CatchedMan;

        //Game Is End??
        bool m_IsEnd = false;
        [SerializeField] float m_EscapeTime = 30f;
        
        //Players
        public int m_PlayerNumber = 5;
        public List<GameObject> m_Players = new();
        public HashSet<GameObject> m_CapturedPlayers = new();
        public List<GameObject> m_Valve = new();
        public List<GameObject> m_Pot = new();
        public List<GameObject> m_BreakPot = new();
        public List<GameObject> m_Exit = new();

        //Items
        int m_ItemNumber;
        //Some Array
        PhotonView m_PhotonView;
        private AudioSource m_AudioSource;

        public PlayInfo m_MyPlayInfo;

        #region Monobehaviors
        private void Awake()
        {
            instance = this;
            m_PhotonView = GetComponent<PhotonView>();
            m_AudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            GameManager.instance.OpenGameUI();
            GameManager.instance.UnlockControll();
            StartCoroutine(nameof(WaitUntillGameStart));
        }

        private void Update()
        {
            SetTimerUI();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
        #endregion

        #region METHODS

        public void SetTimerUI()
        {
            if (!m_IsTimerActive) return;
        }

        //add new player
        public void AddPlayer(GameObject player)
        {
            if (m_Players.Contains(player))
            {
                m_Players.Add(player);
            }
        }

        //search player
        public void SearchPlayer()
        {
            PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();

            foreach (var item in playerControllers)
            {
                if (!m_Players.Contains(item.gameObject))
                {
                    if (!m_IsStart)
                    {
                        item.gameObject.SetActive(false);
                    }

                    m_Players.Add(item.gameObject);
                }
            }
        }

        //�� ����
        public void OpenPot(GameObject valve)
        {
            Valve _valve = valve.GetComponent<Valve>();
            //Debug.LogError($"VALVE IS CLOSED!!!!!! {_valve.GetObjIndex()}");

            GameManager.instance.CloseValveUI(_valve.GetObjIndex());

            foreach (var item in m_Pot)
            {
                if ( _valve.GetObjIndex() == item.GetComponent<Pot>().GetObjIndex())
                {
                    item.GetComponent<Pot>().enabled = true;
                    item.transform.Find("FX_Fire").gameObject.SetActive(false);
                    break;
                }
            }

            //RPC�� UI �Ѹ���
            AlertOpenPot();
        }

        public void BreakPot(GameObject pot)
        {
            foreach (var item in m_Pot)
            {

                if (item.Equals(pot) && !m_BreakPot.Contains(item))
                {
                    m_BreakPot.Add(item);
                    break;
                }
            }

            // RPC POT SPOILED UI UPDATE
            int potIdx = pot.GetComponent<MapObject>().GetObjIndex();
            GameManager.instance.UI_UpdatePot(potIdx);

            if ( m_BreakPot.Count >= 4)
                StartCoroutine(nameof(ActiveEscape));
            //foreach (var item in m_Pot){
            //  if ( item. !=  )
            //  return;
            //}
        }

        public void AlertOpenPot()
        {
            GameManager.instance.AlertInfoMsg(true, "알림", "솥의 잠금이 해제되었습니다");
        }

        public void AlertBreakPot()
        {
            Debug.Log("POT IS CLOSED!!!!!!");
            //GameManager.instance -> alert
        }

        public void CatchedMan(GameObject catchedMan)
        {
            

            if (m_Players.Contains(catchedMan) && 
                !m_CapturedPlayers.Contains(catchedMan))
            {
                //UI Status Trans
                m_CapturedPlayers.Add(catchedMan);
                m_CatchedMan++;

                GameManager.instance.CheckCharacter(catchedMan.GetComponent<PlayerController>().GetCharacterName(), true);

                if (m_CapturedPlayers.Count >= m_PlayerNumber - 1)
                {
                    Debug.LogError("Chef Win!");

                    ChefIsWin(true);

                    m_IsEnd = true;
                }
            }
            else
            {
                //Already Captured
            }
        }

        [PunRPC]
        public void ChefIsWin(bool flag)
        {
            foreach(var p in m_Players)
            {
                //is Me?
                if (p.GetComponent<PhotonView>().IsMine) { 
                    //is Chef?
                    if (p.GetComponent<Chef>())
                    {
                        p.GetComponent<PlayInfo>().IsWin = flag;
                    }
                    else
                    {
                        p.GetComponent<PlayInfo>().IsWin = !flag;
                    }
                }
                break;
            };
        }

        public void SavedMan(GameObject catchedMan)
        {
            
            if (m_Players.Contains(catchedMan) &&
                m_CapturedPlayers.Contains(catchedMan))
            {
                //UI Status Trans
                m_CapturedPlayers.Remove(catchedMan);
                m_CatchedMan--;

                GameManager.instance.CheckCharacter(catchedMan.GetComponent<PlayerController>().GetCharacterName(), false);

                if (m_CapturedPlayers.Count == 4)
                {
                    Debug.LogError("Chef Win!");
                    m_IsEnd = true;
                }
            }
        }

        #endregion

        #region COROUTINES

        IEnumerator WaitUntillGameStart()
        {
            // GameManager.instance.DayTimeControllerOff();

            //check GameStart by 1second
            while( !m_IsStart )
            {
                //����� ��ٸ��� ��....

                if ( m_Players.Count >= m_PlayerNumber)
                {
                    break;
                }

                yield return new WaitForSeconds( 1f );
            }

            if (GameManager.instance.m_IsChef)
            {
                Debug.Log("Do Spawn");
                Spawner.instance.DoSpawn();
            }

            Pot[] pots = null;
            Valve[] valves = null;
            GameObject exitsParent = GameObject.Find("Exit");
            ARPGFXPortalScript[] tmp = exitsParent.GetComponentsInChildren<ARPGFXPortalScript>(true);

            foreach (ARPGFXPortalScript t in tmp )
            {
                if ( t != null )
                {
                    m_Exit.Add(t.gameObject);
                }
            }

            //wait for spawn
            while ( m_Valve.Count < Spawner.instance.m_Valves.Length || 
                    m_Pot.Count < Spawner.instance.m_Pots.Length)
            {
                //�丮 �غ� ��....
                pots = FindObjectsOfType<Pot>();

                foreach (var item in pots)
                {
                    if (!m_Pot.Contains(item.gameObject))
                    {
                        item.enabled = false;
                        m_Pot.Add(item.gameObject);
                    }
                }

                valves = FindObjectsOfType<Valve>();

                foreach (var item in valves)
                {
                    if (!m_Valve.Contains(item.gameObject))
                    {
                        m_Valve.Add(item.gameObject);
                    }
                    
                }

                yield return new WaitForSeconds( 1f );
            }

            Debug.Log("Game Start....");

            GetComponent<InGameBGM>().Play();
            GetComponent<SinkBubblingSound>().Play();

            m_IsStart = true;
            m_StartTime = Time.time;

            StartCoroutine("WaitUntillGameEnd");
        }

        IEnumerator WaitUntillGameEnd()
        {
            //all player character is active
            Chef chefController = null;
            foreach (var item in m_Players)
            {
                item.SetActive(true);

                Chef tmpChef = item.GetComponent<Chef>();
                if (tmpChef != null)
                {
                    chefController = tmpChef;
                    chefController.ReadRecipe();
                } else {
                    if (item.GetComponent<PhotonView>().IsMine) {
                        item.GetComponent<PlayerController>().StartUpdateMyNickname();
                    }
                }
            }

            //Chef Read Book 15~30
            yield return null;
            Debug.Log("Chef Read Book....");
            GameManager.instance.SetTimer("요리시작", "초 후 요리가 시작됩니다", m_ChefReadingTime);

            if (GameManager.instance.m_IsChef)
            {
                m_Recipe.SetActive(true);

                float cnt = m_ChefReadingTime;
                TextMeshProUGUI timer = m_Recipe.transform.Find("Frame").Find("Page Title").Find("Timer").GetComponent<TextMeshProUGUI>();
                while(cnt > 0)
                {
                    if (cnt >= 10)
                    {
                        timer.text = "" + (int)cnt;
                    }
                    else
                    {
                        timer.text = "0" + (int)cnt;
                    }

                    yield return new WaitForSeconds( 1 );
                    cnt--;
                }

                m_Recipe.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(m_ChefReadingTime);
            }

            if (chefController != null)
            {
                chefController.StopReadRecipe();
            }
            else
            {
                Debug.LogError("Error Is Happening!! Chef Isn't Here");
            }

            while( !m_IsEnd )
            {

                yield return new WaitForSeconds( 2f );
            }

            //record Time
            m_EndTime = Time.time;

            //SetData To GameManager
            GameManager.instance.CloseGameUI();
            m_PhotonView.RPC(nameof(DoSaveData), RpcTarget.All);
            SceneLoadingManager.LoadScene("Result");
        }

        IEnumerator ActiveEscape()
        {

            if ( GameManager.instance.m_IsChef )
            {
                int cnt = 0;
                bool[] check = new bool[m_Exit.Count];

                int number1 = -1, number2 = -1;

                while ( cnt < 2)
                {
                    int idx = Random.Range(0, m_Exit.Count);

                    if (!check[idx])
                    {
                        check[idx] = true;
                        cnt++;
                        if ( number1 == -1)
                        {
                            number1 = idx;
                        }
                        else
                        {
                            number2 = idx;
                        }
                    }
                }

                m_PhotonView.RPC(nameof(OpenExit), RpcTarget.All, number1, number2);
            }


            // �� �������� Ÿ�̸� Ȱ��ȭ
            m_IsTimerActive = true;
            m_Timer = m_EscapeTime;

            Debug.Log("Run!!!");

            // PLAY EXIT SOUND
            m_AudioSource.Stop();
            GetComponent<ExitStartSound>().Play();

            yield return null;

            m_ExitLimitTime = 20; // 발표용 코드
            GameManager.instance.SetTimer("탈출시작", "초 남았습니다", m_ExitLimitTime);
            yield return new WaitForSeconds( m_ExitLimitTime + 2.0f );

            if ( m_CapturedPlayers.Count >= m_PlayerNumber - 1 )
                ChefIsWin(true);
            else
            {
                ChefIsWin(false);
            }

            m_IsEnd = true;
            m_AudioSource.Stop();
            // GameManager.instance.DayTimeControllerOn();
        }

        #endregion

        [PunRPC]
        public void DoSaveData()
        {
            if (m_MyPlayInfo)
                m_MyPlayInfo.SaveData();
        }

        [PunRPC]
        public void OpenExit(int number1, int number2)
        {
            m_Exit[number1].SetActive(true);
            m_Exit[number2].SetActive(true);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            string nickName = otherPlayer.NickName;

            foreach( var el in m_Players)
            {
                if (el.GetComponent<PlayInfo>().NickName.Equals(nickName))
                {
                    //he is left guy
                    var pc = el.GetComponent<PlayerController>();

                    var character = pc.GetCharacterName();

                    if (character.Equals("Basic_Chef"))
                    {
                        ChefIsWin(false);
                        GameManager.instance.AlertInfoMsg(true, "알림", "셰프가 게임에서 나갔습니다!");
                        m_MyPlayInfo.IsEscape = true;
                        m_IsEnd = true;
                    }
                    else
                    {
                        GameManager.instance.AlertInfoMsg(true, "알림", "음식 1명이 게임에서 나갔습니다!");
                        CatchedMan(el);
                        GameManager.instance.UI_UpdateCharacter(character ,5);
                    }

                    pc.gameObject.SetActive(false);

                    break;
                };
            }
        }

    }

}
