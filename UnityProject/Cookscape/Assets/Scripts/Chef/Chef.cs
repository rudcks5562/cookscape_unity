using UnityEngine;
using UnityProject.Cookscape;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using System;

public class Chef : PlayerController
{
    [Header("References")]
    [Tooltip("Shef's weapon")]
    [SerializeField] GameObject m_ShefWeapon;

    [Tooltip("This is Jail")]
    [SerializeField] LayerMask m_JailMask;

    ChefTriggerVolume m_TriggerVolume;

    Weapon m_Weapon;

    CatchScript m_CatchScript;

    GameObject m_CurrentInteractingObj;

    bool IsAttackReady;
    bool IsCatching;
    float AttackDelay;

    protected override void Awake()
    {
        base.Awake();

        m_TriggerVolume = GetComponent<ChefTriggerVolume>();
        m_Weapon = GetComponentInChildren<Weapon>();
        m_CatchScript = GetComponent<CatchScript>();
    }

    protected override void Start()
    {
        base.Start();
        if (m_PhotonView != null && m_PhotonView.IsMine)
        {
            GameManager.instance.player = gameObject;
            SetMyMapPointerColor(Color.green);
            StartCoroutine(FadeMyPointerToLowerGreen());
            ShowChefPointer();
            GameManager.instance.HideRunnerNickname();
        }
        GetAvatarStatus(m_NameOfAvatar);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if ( m_IsStuned )
        {
            return;
        }

        EmotionHandle();

        ChangePlayerCameraPerspective();

        InteractingHandler();

        AttackHandler();

        RaycastInteractiveHandler();

        ImprisonHandler();
    }

    #region Emotions
    void EmotionHandle()
    {
        if (m_IsRunningEmotion || m_IsInteracting) return;
        if (m_InputHandler.GetBackNumber1KeyInputDown()) {
            StartCoroutine("Laugh");
        }
    }

    IEnumerator Laugh()
    {
        m_IsRunningEmotion = true;

        // SELECT LAUGHING CLIP
        AudioClip[] clips = SoundsManager.instance.ChefLaughSounds;
        int randomIndex = UnityEngine.Random.Range(0, clips.Length);

        m_PhotonView.RPC("LaughRPC", RpcTarget.All, randomIndex);
        yield return new WaitForSeconds(3.0f);
        m_IsRunningEmotion = false;
    }

    [PunRPC]
    void LaughRPC(int idx)
    {
        GetComponent<ChefLaughingSound>().Play(idx);
    }
    #endregion

    void ImprisonHandler()
    {
        if (IsCatching &&
            m_TriggerVolume.CanImprisonCatchee)
        {
            GameManager.instance.ShowKeyGuide("Throw");

            if (m_InputHandler.GetEKeyInputDown()){
                //throw
                m_CatchScript.Imprison(                //throw
                m_CatchScript.GetM_PhotonView());

                m_PhotonView.RPC(nameof(ImprisonEventRPC), RpcTarget.All);

                m_Animator.SetBool("IsCarrying", false);
            }
        }
    }

    void RaycastInteractiveHandler()
    {
        RaycastHit hitData = m_CommonRaycast.ShootRay(m_InteractionMinDist);

        Rigidbody seekObjectsBody = hitData.rigidbody;

        //hitData nothing
        if (seekObjectsBody == null )
        {
            m_GameManager.HideKeyGuide();
            return;
        }

        GameObject seekObject = seekObjectsBody.gameObject;

        //is Player?
        if ( seekObjectsBody.CompareTag("Runner") && seekObject.transform != this.transform )
        {
            Runner runnerScript = seekObject.GetComponent<Runner>();
            //is Catchable?
            if (runnerScript != null && runnerScript.m_IsStuned && !runnerScript.m_IsCaptured && !IsCatching)
            {
                Debug.Log("Search Catchable Player!");
                m_GameManager.ShowKeyGuide("Catch");

                //And You Click E????
                if (m_InputHandler.GetEKeyInputDown())
                {
                    //set Animation
                    m_Animator.SetBool("IsCarrying", true);

                    //set other status
                    m_PhotonView.RPC("CatchEventRPC", RpcTarget.All);

                    //do catch coroutine
                    m_CatchScript.DoCatch(seekObject);

                    m_GameManager.HideKeyGuide();
                }
            }
        }
        else if ( seekObjectsBody.CompareTag("InteractiveObject"))
        {
            MapObject mapObj = seekObjectsBody.GetComponent<MapObject>();
            string objType = mapObj.GetObjectName();
            switch (objType) {
                case "밸브":
                    m_GameManager.ShowKeyGuide("Valve");
                    break;
                case "냄비":
                    break;
                default:
                    break;
            }

            // START INTERACTING
            if (string.Equals(objType, "밸브") && m_InputHandler.GetEKeyHeldDown()) {
                m_IsInteracting = true;
                m_CurrentInteractingObj = seekObject;
            }
        }
        else
        {
            m_GameManager.HideKeyGuide();
        }
    }

    private void InteractingHandler()
    {
        if (!m_IsInteracting) {
            if (m_CurrentInteractingObj != null) {
                MapObject targetIneracterable = m_CurrentInteractingObj.GetComponent<Rigidbody>().GetComponent<MapObject>();
                if (targetIneracterable == null) return;
                Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                {
                    valve.StopSpin();
                });
            }
            return;
        }
        if (m_CurrentInteractingObj == null) return;

        if (m_InputHandler.GetEKeyHeldDown()) {
            MapObject targetIneracterable = m_CurrentInteractingObj.GetComponent<Rigidbody>().GetComponent<MapObject>();
            if (targetIneracterable == null) return;

            GameObject targetObj = m_CurrentInteractingObj.gameObject;
            // INCRESING INTERACTING TIME
            m_InteractingTime += Time.deltaTime;


            if (m_InteractingTime > m_InteractionReadyTime) {
                float gauge = targetIneracterable.GetGauge();
                if (gauge > 0) {
                    m_GameManager.SetGauge(gauge);
                    // SHOW GAUGE INFO
                    m_GameManager.ShowGaugeInfo(targetObj);

                    if (gauge <= 0) {
                        m_IsInteracting = false;
                        m_InteractingTime = 0f;

                        Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                        {
                            valve.StopSpin();
                        });

                        m_Animator.SetBool("IsValveOpening", false);

                    } else if (gauge < 100) {
                        if (!targetIneracterable.isClose) {
                            targetIneracterable.Interact(-1);
                            Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                            {
                                valve.StartSpin();
                            });

                            m_Animator.SetBool("IsValveOpening", true);
                        }
                    }

                } else {
                    m_IsInteracting = false;
                    m_InteractingTime = 0f;
                    Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                    {
                        valve.StopSpin();
                    });
                    m_Animator.SetBool("IsValveOpening", false);
                }
            }
        } else {
            m_IsInteracting = false;
            m_InteractingTime = 0f;
            m_Animator.SetBool("IsValveOpening", false);
            m_GameManager.HideGaugeInfo();
        }
        m_GameManager.HideKeyGuide();
    }

    public void ReadRecipe()
    {
        m_IsStuned = true;

        m_Animator.SetBool("ReadRecipe", true);
    }

    public void StopReadRecipe()
    {
        m_IsStuned = false;

        m_Animator.SetBool("ReadRecipe", false);
    }

    void AttackHandler()
    {
        AttackDelay += Time.deltaTime;

        if (m_InputHandler.GetAttackKeyInputDown())
        {
            if (m_Weapon == null || !m_Weapon.enabled)
            {
                Debug.Log("do not have weapon");
                return;
            }
            else
            {
                IsAttackReady = m_Weapon.rate < AttackDelay;

                if (IsAttackReady)
                {
                    m_Weapon.Use();
                    AttackDelay = 0;
                }
            }
        }
    }

    #region RPC

    [PunRPC]
    void CatchEventRPC()
    {
        //set Other Objects
        IsCatching = true;
        m_ShefWeapon.SetActive(false);
        m_Weapon.enabled = false;
    }

    [PunRPC]
    void ImprisonEventRPC()
    {
        IsCatching = false;
        m_ShefWeapon.SetActive(true);
        m_Weapon.enabled = true;
    }

    #endregion
}
