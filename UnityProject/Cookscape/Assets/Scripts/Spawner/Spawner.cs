using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance = null;

    public GameObject[] m_Pots;
    public GameObject[] m_Valves;
    public GameObject[] m_Item;
    public int[] m_ItemCnt;

    #region LIFECYCLE
    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            if (instance != this) {
                Destroy(this.gameObject);
            }
        }
    }
    #endregion

    IEnumerator WaitForConnect()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Hey Im Wait...");
            yield return new WaitForSeconds(2f);
        }

        PotSpawn();
        ValveSpawn();
        ItemSpawn();
    }

    public void DoSpawn() {
        StartCoroutine("WaitForConnect");
    }

    #region METHODS
    public void PotSpawn()
    {
        Transform[] points = GameObject.Find("PotSpawnPointGroup").GetComponentsInChildren<Transform>();
        Spawn("Pot", m_Pots, points);
    }

    public void ValveSpawn()
    {
        Transform[] points = GameObject.Find("ValveSpawnPointGroup").GetComponentsInChildren<Transform>();
        Spawn("Valve", m_Valves, points);
    }

    public void ItemSpawn()
    {
        Transform[] points = GameObject.Find("ItemSpawnPointGroup").GetComponentsInChildren<Transform>();
        SpawnAll(m_Item, m_ItemCnt, points);
    }

    void Spawn(string type, GameObject[] obj, Transform[] spawnPoints)
    {
        int pointCnt = spawnPoints.Length;
        int ObjCnt = obj.Length;
        if (pointCnt < ObjCnt) return;

        int count = 0;
        int[] seq = new int[pointCnt];
        bool[] isSelected = new bool[pointCnt];
        
        int[] pos = { 1, 2, 3, 4 };
        // 랜덤한 수열 생성
        while (count < ObjCnt) {
            // int number = Random.Range(1, pointCnt);
            // if (isSelected[number]) continue;
            // isSelected[number] = true;
            // seq[count++] = number;

            // 스폰 위치 고정(발표용)
            seq[count] = pos[count];
            count++;
        }

        for (int i = 0; i < ObjCnt; i++) {
            Quaternion ObjRotation = Quaternion.Euler(0, 0, 0);
            int posIdx = seq[i];
            if (type.Equals("Valve")) {
                switch (posIdx) {
                    case 5:
                        ObjRotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case 6:
                        ObjRotation = Quaternion.Euler(0, -90, 0);
                        break;
                    case 7:
                        ObjRotation = Quaternion.Euler(0, 90, 0);
                        break;
                    default:
                        ObjRotation = Quaternion.Euler(0, 0, 0);
                        break;
                }
            } else if (type.Equals("Pot")) {
                switch (posIdx) {
                    case 2:
                        ObjRotation = Quaternion.Euler(0, 117, 0);
                        break;
                    case 3:
                        ObjRotation = Quaternion.Euler(0, 116, 0);
                        break;
                    case 4:
                        ObjRotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case 6:
                        ObjRotation = Quaternion.Euler(0, 45, 0);
                        break;
                    case 8:
                        ObjRotation = Quaternion.Euler(0, 117, 0);
                        break;
                    default:
                        ObjRotation = Quaternion.Euler(0, 0, 0);
                        break;
                }
            }
            PhotonNetwork.Instantiate("Prefabs/Interacterable/" + obj[i].name, spawnPoints[seq[i]].transform.position, ObjRotation);
        }
    }

    void SpawnAll(GameObject[] obj, int[] spawnCnt, Transform[] spawnPoints)
    {
        int pointCnt = spawnPoints.Length;
        int spawnCntAll = 0;
        foreach (int cnt in spawnCnt) {
            spawnCntAll += cnt;
        }
        if (pointCnt < spawnCntAll) return;

        int count = 0;
        int[] seq = new int[pointCnt];
        bool[] isSelected = new bool[pointCnt];
        
        int[] pos = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        // 랜덤한 수열 생성
        while (count < spawnCntAll) {
            // int number = Random.Range(1, pointCnt);
            // if (isSelected[number]) continue;
            // isSelected[number] = true;
            // seq[count++] = number;
            seq[count] = pos[count];
            count++;
        }

        int itemNo = 0;
        int setCnt = 0;
        while (setCnt < spawnCntAll) {
            for (int i=0; i<spawnCnt[itemNo]; i++) {
                GameObject iii = PhotonNetwork.Instantiate("Prefabs/Item/" + obj[itemNo].name, spawnPoints[seq[setCnt++]].transform.position, Quaternion.identity);
                if (setCnt >= spawnCntAll) break;
            }
            itemNo++;
        }
    }
    #endregion
}
