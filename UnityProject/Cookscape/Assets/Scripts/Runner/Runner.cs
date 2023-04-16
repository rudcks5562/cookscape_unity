using Photon.Pun;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityProject.Cookscape;

public class Runner : PlayerController
{
    [SerializeField] const float DEAD_LINE = 180f;

    #region EXTERNAL COMPONENTS
    // EXTERNAL COMPONENTS
    public GameObject m_PlayerEquipPoint;
    public enum m_EnumFootprint
    {
        Left,
        Right
    }
    public GameObject m_FootprintLeft;
    public GameObject m_FootprintRight;
    #endregion

    #region PRIVATE VARIABLES
    // PRIVATE VARIABLES
    RunnerTriggerVolume m_RunnerTriggerVolume;

    private bool m_IsEquiped = false;
    private Rigidbody m_CurrentInteractingObj;
    private Vector3 LastFootprint;
    private m_EnumFootprint WhichFoot;
    private Rigidbody m_Rigidbody;
    private bool m_IsValveClosing = false;
    private bool m_IsPotPushing = false;
    private float m_PotPushingTime = 0f;

    private bool m_WasCaptured = false;
    private int m_CapturedLevel = -1;

    private float m_CapturedTime = 0;
    #endregion

    #region LIFECYCLE
    protected override void Awake()
    {
        base.Awake();

        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("EquipPoint"))
            {
                // Do something with the child object
                m_PlayerEquipPoint = child.gameObject;
                break;
            }
        }

        //.FindGameObjectWithTag("EquipPoint");
        m_RunnerTriggerVolume = GetComponent<RunnerTriggerVolume>();
    }

    protected override void Start()
    {
        base.Start();
        m_Rigidbody = this.GetComponent<Rigidbody>();
        LastFootprint = this.transform.position;

        if (m_PhotonView != null && m_PhotonView.IsMine)
        {
            GameManager.instance.player = gameObject;
            SetMyMapPointerColor(Color.green);
            StartCoroutine(FadeMyPointerToLowerGreen());
            ShowFriendPointer();
        }
        GameManager.instance.HideChefNickname();

        if (GameFlowManager.instance != null) {
            GetComponent<Detector>().enabled = true;
        }
        GetAvatarStatus(m_NameOfAvatar);
    }

    protected override void Update()
    {
        base.Update();

        if ( m_IsDead)
        {
            return;
        }

        // CHECK INTERACTABLE OBJECT
        RaycastHit hitData = m_CommonRaycast.ShootRay(m_InteractionMinDist, m_InteractableMask);

        if (m_IsCaptured)
        {
            m_WasCaptured = true;
            m_CapturedTime += Time.deltaTime;

            UpdateRunnerStatusUI();
            return;
        }

        if ( m_WasCaptured && !m_IsCaptured)
        {
            m_WasCaptured = false;
            GetComponent<SinkTicTokSound>().Stop();
            m_CapturedTime = 0;
            m_CapturedLevel = -1;
            m_PhotonView.RPC(nameof(UpdateUIStatus), RpcTarget.All, -1);
        }

        if (m_IsStuned) {
            StunProcess();
            return;
        }

        // PRINT FOOTPRINT WHEN YOU ARE ON GROUND
        if (m_IsGround && m_IsWet) {
            float DistanceSinceLastFootprint = Vector3.Distance(LastFootprint, this.transform.position);
            if (DistanceSinceLastFootprint >= m_FootprintSpace) {
                if (WhichFoot == m_EnumFootprint.Left) {
                    PrintDecal(m_FootprintLeft);
                    WhichFoot = m_EnumFootprint.Right;
                } else if (WhichFoot == m_EnumFootprint.Right) {
                    PrintDecal(m_FootprintRight);
                    WhichFoot = m_EnumFootprint.Left;
                }
                LastFootprint = this.transform.position;
            }
        }

        ChangePlayerCameraPerspective();

        // YOU ARE INTERACTING
        if (m_IsInteracting && !m_IsAnimating && !m_InputHandler.GetEKeyHeldDown()) {

            MapObject targetIneracterable = m_CurrentInteractingObj.GetComponent<Rigidbody>().GetComponent<MapObject>();
            StopObjectInteractHandler(targetIneracterable);

        } else if (m_IsInteracting && m_InputHandler.GetEKeyHeldDown()) {
            MapObject targetIneracterable = m_CurrentInteractingObj.GetComponent<Rigidbody>().GetComponent<MapObject>();
            if (!m_CurrentInteractingObj.GetComponent<Rigidbody>().GetComponent<MapObject>().enabled)
                targetIneracterable = null;

            // INCRESING INTERACTING TIME
            m_InteractingTime += Time.deltaTime;

            if (targetIneracterable != null && m_InteractingTime > m_InteractionReadyTime) {
                ObjectInteractingHandler(targetIneracterable);
            }
        }

        // THERE IS SOMETHING OBJECT
        if (hitData.rigidbody) {
            GameObject targetObj = hitData.rigidbody.gameObject;
            StartObjectInteractionHandler(hitData, targetObj);
        } else {
            // YOU HAVE AN EQUIPTMENT & PRESS 'F'
            if (m_IsEquiped && m_InputHandler.GetFKeyInputDown()) {
                DoDrop();
            }

            m_GameManager.HideKeyGuide();

            if (!m_IsInteracting) {
                m_GameManager.HideGaugeInfo();
            }
        }

        // USE EQUIPMENT -> LATER WE WILL ADD A STATE FOR USING EQUIPMENT
        if (!m_IsStuned && !m_IsCaptured && !m_IsInteracting)
        {
            UseItemHandler();
        }

        EmotionHandler();
    }
    #endregion

    #region Emotions
    void EmotionHandler()
    {
        if (m_IsRunningEmotion || m_IsInteracting) return;
        if (m_InputHandler.GetBackNumber1KeyInputDown()) {
            StartCoroutine("Incite");
        } else if (m_InputHandler.GetBackNumber2KeyInputDown()) {
            StartCoroutine("Fart");
        }
    }

    IEnumerator HitSoundHandler()
    {
        m_IsHitSoundPlaying = true;
        AudioClip[] clips = SoundsManager.instance.ChickHitSounds;
        int randomIndex = UnityEngine.Random.Range(0, clips.Length);

        m_PhotonView.RPC("HitSoundRPC", RpcTarget.All, randomIndex);
        yield return new WaitForSeconds(3.0f);
        m_IsHitSoundPlaying = false;
    }

    [PunRPC]
    void HitSoundRPC(int idx)
    {
        GetComponent<PlayerHitSounds>().Play(idx);
    }

    IEnumerator Incite()
    {
        m_IsRunningEmotion = true;

        // SELECT LAUGHING CLIP
        AudioClip[] clips = SoundsManager.instance.RunnerInciteSounds;
        int randomIndex = UnityEngine.Random.Range(0, clips.Length);

        m_PhotonView.RPC("InciteRPC", RpcTarget.All, randomIndex);
        yield return new WaitForSeconds(3.0f);
        m_IsRunningEmotion = false;
    }

    [PunRPC]
    void InciteRPC(int idx)
    {
        GetComponent<RunnerInciteSound>().Play(idx);
    }

    IEnumerator Fart()
    {
        m_IsRunningEmotion = true;

        // SELECT LAUGHING CLIP
        AudioClip[] clips = SoundsManager.instance.RunnerFartSounds;
        int randomIndex = UnityEngine.Random.Range(0, clips.Length);

        m_PhotonView.RPC("FartRPC", RpcTarget.All, randomIndex);
        yield return new WaitForSeconds(3.0f);
        m_IsRunningEmotion = false;
    }

    [PunRPC]
    void FartRPC(int idx)
    {
        GetComponent<RunnerFartSound>().Play(idx);
    }
    #endregion

    #region METHODS
    void StartObjectInteractionHandler(RaycastHit hitData, GameObject targetObj)
    {
        if (m_IsChefTakeMe) return;

        // IT IS EQUIPTMENT
        if (hitData.rigidbody.tag == "Equipment") {
            if (!m_IsEquiped) {
                // SHOW GUIDE TEXT
                m_GameManager.ShowKeyGuide("Pick");

                // YOU DON'T HAVE AN EQUIPTMENT & PRESS 'F'
                if (m_InputHandler.GetFKeyInputDown()) {
                    // IF YOU HAS AN EQUIPMENT
                    if (m_IsEquiped) return;

                    m_PhotonView.RPC(nameof(EquipRPC), RpcTarget.All, targetObj.GetComponent<PhotonView>().ViewID, m_PhotonView.ViewID);
                    m_GameManager.AlertInfoMsg(false, "정보", "일부 아이템은 플레이어의 이동속도를 감소시킵니다");
                    m_GameManager.HideKeyGuide();
                }
            } else {
                // SHOW GUIDE TEXT
                m_GameManager.ShowKeyGuide("Change");

                // YOU HAVE AN EQUIPTMENT & PRESS 'F'
                if (m_IsEquiped && m_InputHandler.GetFKeyInputDown()) {
                    //m_PhotonView.RPC("Drop", RpcTarget.All, m_PhotonView.ViewID);
                    DoDrop();

                    GameObject equipment = hitData.rigidbody.gameObject;
                    m_PhotonView.RPC(nameof(EquipRPC), RpcTarget.All, equipment.GetComponent<PhotonView>().ViewID, m_PhotonView.ViewID);
                    m_GameManager.AlertInfoMsg(false, "정보", "일부 아이템은 플레이어의 이동속도를 감소시킵니다");
                    m_GameManager.HideKeyGuide();
                }
            }
        } else if (hitData.rigidbody.tag == "InteractiveObject") { // IT IS INTERACTIVE OBJECT
            if (!m_IsInteracting) {
                // SHOW GUIDE TEXT
                MapObject mapObj = hitData.rigidbody.GetComponent<MapObject>();

                string objType = mapObj.GetObjectName();
                switch (objType) {
                    case "밸브":
                        m_GameManager.ShowKeyGuide("Valve");
                        break;
                    case "냄비":
                        m_GameManager.ShowKeyGuide("Push");
                        break;
                    default:
                        break;
                }

                // START INTERACTING
                if (m_InputHandler.GetEKeyHeldDown()) {
                    m_IsInteracting = true;
                    m_CurrentInteractingObj = hitData.rigidbody;
                }
            }
        }
    }

    void ObjectInteractingHandler(MapObject targetIneracterable)
    {
        GameObject targetObj = m_CurrentInteractingObj.gameObject;
        
        targetIneracterable.Interact();
        float gauge = targetIneracterable.GetGauge();
        bool isClose = targetIneracterable.isClose;
        if (gauge <= 100) {
            m_GameManager.SetGauge(gauge);
            // SHOW GAUGE INFO
            m_GameManager.ShowGaugeInfo(targetObj);

            if (m_IsInteracting && !m_IsAnimating) {
                if (isClose) {
                    m_IsInteracting = false;
                    m_InteractingTime = 0f;
                    if (targetIneracterable.GetObjectName() == "냄비" && m_IsPotPushing) {
                        m_IsPotPushing = false;
                        m_IsAnimating = false;
                        m_Animator.SetBool("IsPotPushing", m_IsPotPushing);
                        //Record this
                        gameObject.GetComponent<PlayInfo>().CountBreakPot++;
                    } else if (targetIneracterable.GetObjectName() == "밸브" && m_IsValveClosing) {
                        m_IsValveClosing = false;

                        Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                        {
                            valve.StopSpin();
                        });

                        m_Animator.SetBool("IsValveClosing", m_IsValveClosing);
                        
                        //Record this
                        gameObject.GetComponent<PlayInfo>().CountCloseValve++;
                    }
                } else {
                    // INTERACTABLE OBJ = VALVE, ANIMATE CLOSING VALVE
                    if (targetIneracterable.GetObjectName() == "밸브" && !m_IsValveClosing) {
                        m_IsValveClosing = true;

                        Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                        {
                            valve.StartSpin();
                        });

                        m_Animator.SetBool("IsValveClosing", m_IsValveClosing);
                    } else if (targetIneracterable.GetObjectName() == "냄비" && !m_IsPotPushing) {
                        m_IsPotPushing = true;
                        m_Animator.SetFloat("PotPushingTime", m_PotPushingTime);
                        m_Animator.SetBool("IsPotPushing", m_IsPotPushing);
                    } else if (targetIneracterable.GetObjectName() == "냄비" && m_IsPotPushing) {
                        m_PotPushingTime += Time.deltaTime;
                        m_Animator.SetFloat("PotPushingTime", m_PotPushingTime);
                    }
                }
            } else {
                if (gauge < 100) {
                    // INTERACTABLE OBJ = VALVE, ANIMATE CLOSING VALVE
                    if (targetIneracterable.GetObjectName() == "밸브" && !m_IsValveClosing) {
                        m_IsValveClosing = true;

                        Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                        {
                            valve.StartSpin();
                        });

                        m_Animator.SetBool("IsValveClosing", m_IsValveClosing);
                    } else if (targetIneracterable.GetObjectName() == "냄비" && !m_IsPotPushing && !m_IsAnimating) {
                        m_IsPotPushing = true;
                        m_Animator.SetBool("IsPotPushing", m_IsPotPushing);
                    } else if (targetIneracterable.GetObjectName() == "냄비" && m_IsPotPushing) {
                        m_PotPushingTime += Time.deltaTime;
                        m_Animator.SetFloat("PotPushingTime", m_PotPushingTime);
                    }
                }
            }

        } else {
            m_IsInteracting = false;
            m_InteractingTime = 0f;
        }

        // SHOW GAUGE VALUE ON THE GUIDE TEXT
        m_GameManager.HideKeyGuide();
    }

    void StopObjectInteractHandler(MapObject targetIneracterable)
    {
        if (targetIneracterable != null) {
            if (targetIneracterable.GetObjectName() == "밸브") {
                m_IsValveClosing = false;
                Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                {
                    valve.StopSpin();
                });
                m_Animator.SetBool("IsValveClosing", m_IsValveClosing);
            } else if (targetIneracterable.GetObjectName() == "냄비") {
                m_IsPotPushing = false;
                m_PotPushingTime = 0f;
                m_Animator.SetBool("IsPotPushing", m_IsPotPushing);
                m_Animator.SetFloat("PotPushingTime", m_PotPushingTime);
            }
        }
        m_IsInteracting = false;
        m_InteractingTime = 0f;
        m_CurrentInteractingObj = null;

        // HIDE GAUGE INFO
        m_GameManager.HideGaugeInfo();
    }

    void PrintDecal(GameObject prefab)
    {
        // FOOT POSITION CO_VALANCE VALUE
        Vector3 coValue = new Vector3(0, 0.2f, 0);
        Ray ray = new Ray(transform.position + coValue, -transform.up);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.5f, m_FloorMask)) {
            // GameObject decal = Instantiate(prefab);
            GameObject decal = PhotonNetwork.Instantiate("Prefabs/" + prefab.name, hit.point, Quaternion.identity);
            if (decal != null) {
                decal.transform.position = hit.point;
                decal.transform.Rotate(Vector3.up, this.transform.eulerAngles.y);
            }
        }
    }

    void StunProcess()
    {
        if (!m_IsHitSoundPlaying) {
            StartCoroutine(HitSoundHandler());
        }

        if (m_CurrentInteractingObj != null) {
            MapObject targetIneracterable = m_CurrentInteractingObj.GetComponent<Rigidbody>().GetComponent<MapObject>();
            if (targetIneracterable != null) {
                if (targetIneracterable.GetObjectName() == "밸브") {
                    m_IsValveClosing = false;
                    Array.ForEach(targetIneracterable.GetComponentsInChildren<ValveRotation>(), valve =>
                    {
                        valve.StopSpin();
                    });
                    m_Animator.SetBool("IsValveClosing", m_IsValveClosing);
                } else if (targetIneracterable.GetObjectName() == "냄비") {
                    m_IsPotPushing = false;
                    m_PotPushingTime = 0f;
                }
            }
        }

        if (GameManager.instance.m_IsMapExpanded) {
            GameManager.instance.ToggleGameMap();
        }

        m_IsInteracting = false;
        m_InteractingTime = 0f;
        m_CurrentInteractingObj = null;

        m_GameManager.HideGaugeInfo();
        m_GameManager.HideKeyGuide();

        CameraMovement.instance.ChangeThirdPerspective();
    }

    IEnumerator Equip(int myID)
    {
        GameObject pickingPlayer = PhotonView.Find(myID).gameObject;
        Runner tmp = pickingPlayer.GetComponent<Runner>();

        if (!tmp.m_IsEquiped) {
            m_Animator.SetTrigger("Picking Up");
            yield return new WaitForSeconds(0.8f);
            // Equipment's Physics System Off
            PhysicSystemToggle(tmpEquipment, false);

            // Equipment On My Hand
            tmpEquipment.transform.SetParent(tmp.m_PlayerEquipPoint.transform);
            tmpEquipment.transform.localPosition = Vector3.zero;
            tmpEquipment.transform.rotation = new Quaternion(0, 0, 0, 0);

            // Equiped State: true
            tmp.m_IsEquiped = true;

            // EQUIPMENT'S INTERACTABLE LAYER OFF
            int equipmentViewID = tmpEquipment.GetComponent<PhotonView>().ViewID;
            GameObject targetEuipment = PhotonView.Find(equipmentViewID).gameObject;
            // targetEuipment.layer = 6;

            // MY EQUIPMENT SET
            tmp.m_EquipedObj = tmpEquipment;
            tmpEquipment = null;

            // WHEN YOU GRAP HEAVY ITEM
            float itemWeight = tmp.m_EquipedObj.GetComponent<Item>().GetWeight();
            if (itemWeight >= 2.0f) {
                tmp.m_IsHeavy = true;
            }
        }
    }

    [PunRPC]
    void Drop(int myID)
    {
        if (myID != m_PhotonView.ViewID) return;

        if (m_IsEquiped) {

            // Get Equipment On My Hand
            GameObject equipment = m_PlayerEquipPoint.GetComponentInChildren<Rigidbody>().gameObject;
            Rigidbody rigidbody = equipment.GetComponent<Rigidbody>();

            int equipmentViewID = equipment.GetComponent<PhotonView>().ViewID;
            GameObject targetEuipment = PhotonView.Find(equipmentViewID).gameObject;
            targetEuipment.layer = 6;

            DetachEquipment();
            PhysicSystemToggle(equipment, true);
            StartCoroutine(nameof(ThrowEquipment), rigidbody);

            m_IsEquiped = false;
            m_IsHeavy = false;
        }
    }

    void PhysicSystemToggle(GameObject target, bool isOn)
    {
        Collider[] objectColliders = target.GetComponents<Collider>();
        Rigidbody objectRigidbody = target.GetComponent<Rigidbody>();
        foreach(Collider collider in objectColliders) {
            collider.enabled = isOn;
        }
        objectRigidbody.isKinematic = !isOn;
    }

    void DetachEquipment()
    {
        m_PlayerEquipPoint.transform.DetachChildren();
    }

    void UseItemHandler()
    {
        if (!m_IsEquiped)
            return;

        Item item = m_EquipedObj.GetComponent<Item>();
        
        bool seeGuideInfo = false;
        string guideName = "";
        
        if (item.GetItemName().Equals("키친타올") && m_IsWet && InputHandler.instance.GetQKeyInputDown())
        {
            m_IsInteracting = true;
            item.Use(gameObject);
            m_IsInteracting = false;
            return;
        }

        //need hitdata
        RaycastHit hitData = m_CommonRaycast.ShootRay(m_InteractionMinDist, m_PlayerMask);
        Rigidbody rb = hitData.rigidbody;

        if (rb == null)
            return;

        //Player And Captured?
        if (rb.CompareTag("Runner") && rb.gameObject.GetComponent<PlayerController>().m_IsCaptured && !rb.gameObject.GetComponent<PlayerController>().m_IsDead)
        {
            if ( m_RunnerTriggerVolume.CanSaveCatchee && item.GetItemName().Equals("나무젓가락"))
            {
                if (!seeGuideInfo)
                {
                    seeGuideInfo = true;
                    guideName = "Save";
                }

                if (InputHandler.instance.GetQKeyInputDown())
                {
                    //Use ChopStick
                    item.Use(rb.gameObject);
                    return;
                }
            }
            else
            {
                //show not have chopstick UI
            }
        }

        if (seeGuideInfo)
        {
            GameManager.instance.ShowKeyGuide(guideName);
        }
        else
        {
            GameManager.instance.HideKeyGuide();
        }
    }

    IEnumerator ThrowEquipment(Rigidbody rigidbody)
    {
        // CONSTRAINTS ON ALL
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        // THROW EQUIPMENT
        float throwPower = 0.5f;
        Vector3 throwAngle = transform.forward * 10f;
        throwAngle.y = 10f;
        rigidbody.AddForce(throwAngle * throwPower, ForceMode.Impulse);

        // CONSTRAINTS OFF
        yield return new WaitForSeconds(0.05f);
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    void UseItem(GameObject item)
    {
        if (item == null) return;
        Item itemObj = item.GetComponent<Item>();
        itemObj.Use();
    }

    public void DoDrop()
    {
        m_PhotonView.RPC("Drop", RpcTarget.All, m_PhotonView.ViewID);
    }

    public bool IsPlayerEquiped()
    {
        return this.m_IsEquiped;
    }
    #endregion

    public void UpdateRunnerStatusUI()
    {
        if (m_CapturedTime > DEAD_LINE)
        {
            GetComponent<SinkTicTokSound>().Stop();

            //DEAD
            m_IsDead = true;
            m_IsCaptured = false;
            m_PhotonView.RPC(nameof(UpdateUIStatus), RpcTarget.All, 5);
        }
        else if (m_CapturedTime > DEAD_LINE / 5 * 4 && m_CapturedLevel < 4)
        {
            //Water 80
            GetComponent<SinkTicTokSound>().Play(0.6f);
            m_CapturedLevel = 4;
            m_PhotonView.RPC(nameof(UpdateUIStatus), RpcTarget.All, 4);
        }
        else if (m_CapturedTime > DEAD_LINE / 5 * 3 && m_CapturedLevel < 3)
        {
            //Water 60
            GetComponent<SinkTicTokSound>().Play(0.8f);
            m_CapturedLevel = 3;
            m_PhotonView.RPC(nameof(UpdateUIStatus), RpcTarget.All, 3);
        }
        else if (m_CapturedTime > DEAD_LINE / 5 * 2 && m_CapturedLevel < 2)
        {
            //Water 40
            GetComponent<SinkTicTokSound>().Play(1f);
            m_CapturedLevel = 2;
            m_PhotonView.RPC(nameof(UpdateUIStatus), RpcTarget.All, 2);
        }
        else if  (m_CapturedTime > DEAD_LINE / 5 * 1 && m_CapturedLevel < 1)
        {
            //Water 20
            GetComponent<SinkTicTokSound>().Play(1.2f);
            m_CapturedLevel = 1;
            m_PhotonView.RPC(nameof(UpdateUIStatus), RpcTarget.All, 1);
        }
        else if ( m_CapturedLevel < 0 )
        {
            //Water 0
            m_CapturedLevel = 0;
            m_PhotonView.RPC(nameof(UpdateUIStatus), RpcTarget.All, 0);
        }
    }

    #region RPC

    GameObject tmpEquipment;

    [PunRPC]
    void EquipRPC(int targetID, int myID)
    {
        if (myID != m_PhotonView.ViewID)
        {
            return;
        }

        tmpEquipment = PhotonView.Find(targetID).gameObject;

        StopCoroutine(nameof(Equip));
        StartCoroutine(nameof(Equip), myID);
    }

    [PunRPC]
    void UpdateUIStatus(int level)
    {
        m_GameManager.UI_UpdateCharacter(GetCharacterName(), level);
    }

    #endregion
}

