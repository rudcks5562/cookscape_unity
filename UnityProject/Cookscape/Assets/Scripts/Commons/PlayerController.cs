using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityProject.Cookscape
{
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour, IPlayer
    {
        #region EXTERNAL VARIABLES
        [Header("References")]
        [Tooltip("Player Camera Arm")]
        public Transform m_PlayerCamera;

        [Tooltip("Players rigidbody")]
        public Rigidbody m_PlayerBody;

        [Tooltip("Feet Position")]
        public Transform m_FeetTransform;

        [Tooltip("Check FloorLayer")]
        public LayerMask m_FloorMask;

        [Tooltip("Check Target Player")]
        public LayerMask m_PlayerMask;

        [Tooltip("Check Target Object")]
        public LayerMask m_InteractableMask;

        //public Animator m_PlayerAnimator;
        //CharacterController characterController;

        //Player is ground
        public bool m_IsGround { get; private set; }
        //Player jump this time
        public bool m_HasJumpedThisFrame { get; private set; }
        #endregion

        #region PROTECTED VARIABLES
        // PROTECTED VARIABLES
        protected string m_UserTitle;
        protected string m_UserNickname;
        protected GameManager m_GameManager;
        protected CommonRaycast m_CommonRaycast;
        protected InputHandler m_InputHandler;
        protected NetworkedAnimation m_NetworkedAnimation;
        protected PhotonView m_PhotonView;
        protected GameObject m_EquipedObj;
        protected Animator m_Animator;
        [SerializeField] protected string m_NameOfAvatar;

        // CONSTANT STATUS
        protected long m_AvatarId;
        protected string m_AvatarName;
        [SerializeField] protected float m_MovementSpeed = 11f;
        protected float m_RotationSpeed = 200f;
        [SerializeField] protected float m_JumpForce = 20f;
        protected float m_Stamina = 100.0f;
        protected float m_StaminaDecreasingFactor = 10.0f;
        protected float m_StaminaIncreasingFactor = 1.0f;
        protected float m_InteractionMinDist = 5f;
        protected float m_InteractionReadyTime = 0.2f;
        protected float m_FootprintSpace = 1.0f;
        protected float m_SpeedCoFactor = 1.0f;
        protected float m_JumpForceCoFactor = 1.0f;

        protected PlayInfo m_PlayInfo;

        // STATUS
        public bool m_IsStuned = false;
        public bool m_IsChefTakeMe = false;
        public bool m_IsCaptured = false;
        protected bool m_IsRunning = false;
        protected bool m_IsCrouched = false;
        protected bool m_IsWet = false;
        protected bool m_IsHeavy = false;
        protected bool m_IsInteracting = false;
        protected float m_InteractingTime = 0f;
        protected bool m_IsAnimating = false;
        protected bool m_IsRunningEmotion = false;
        protected bool m_IsHitSoundPlaying = false;
        public bool m_IsDead = false;

        public List<GameObject> m_HatList;
        #endregion

        #region PRIVATE VARIABLES

        [SerializeField] string m_CharacterName = "Default";


        private IInteractable m_TargetInteractable;
        Vector3 m_GroundNormal;
        
        //Last jump time
        float m_LastJumpedTime = -10;

        //ground check distance in air
        const float m_GroundCheckDistanceInAir = 0.25f;
        //not use now
        const float m_JumpGroundingPreventionTime = 0.2f;

        float verticalSpeed = 0f;
        float horizonalSpeed = 0f;
        [SerializeField] float lerpSpeed = 100f;

        float notWalkTime = 0f;
        float slowWalkTime = 0f;

        bool cameraFollowMe = false;

        #endregion
        
        #region CALCULATED VARIABLES
        //real movement speed by status
        float m_Modified_MovementSpeed
        {
            get
            {
                if (m_IsRunning) return m_MovementSpeed * 2f;
                if (m_IsCrouched) return m_MovementSpeed * 0.5f;
                return m_MovementSpeed;
            }
        }
        //For Animator
        float m_GravityForAnimator
        {
            get
            {
                if (m_IsRunning) return 2.0f;
                if (m_IsCrouched) return 0.5f;
                return 1.0f;
            }
        }

        //rotation speed by status
        public float m_RotationMultiplier
        {
            get
            {
                return 1f;
            }
        }
        #endregion

        #region LIFECYCLE
        protected virtual void Awake()
        {
            m_NetworkedAnimation = GetComponent<NetworkedAnimation>();
            m_Animator = GetComponent<Animator>();
            m_CommonRaycast = GetComponent<CommonRaycast>();
            m_PhotonView = GetComponent<PhotonView>();
            m_PlayInfo = GetComponent<PlayInfo>();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            m_IsGround = true;
            m_InputHandler = InputHandler.instance;
            m_GameManager = GameManager.instance;
            

            // Set My Info
            // ??게임 ?�브?�트???�네???�기
            Transform userInfo = transform.Find("Text_UserInfo");
            TextMesh nickname = userInfo.Find("Text_Nickname").GetComponent<TextMesh>();
            TextMesh title = userInfo.Find("Text_Title").GetComponent<TextMesh>();

            title.text = GameManager.instance.user.title;
            nickname.text = GameManager.instance.user.nickname;

            // Dictionary<string, string> dictionary = new Dictionary<string, string>();
            // dictionary["viewID"] = m_PhotonView.ViewID + "";
            // dictionary["title"] = GameManager.instance.user.title;
            // dictionary["nickname"] = GameManager.instance.user.nickname;

            //It's not me
            if (m_PhotonView == null || !m_PhotonView.IsMine)
            {
                this.enabled = false;
            }
            //It's me!
            else
            {
                GameManager.instance.player = gameObject;
                CameraFollowMe();
                StartUpdateMyNickname();
                GameManager.instance.m_MyGameAvatar = m_CharacterName;
            }
        }

        float _deltaTime = 0f;
        // Update is called once per frame
        protected virtual void Update()
        {
            if (m_PhotonView == null || !m_PhotonView.IsMine)
            {
                return;
            }

            if (transform.position.y < -10)
            {
                this.gameObject.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
            }

            _deltaTime = Time.deltaTime;

            m_HasJumpedThisFrame = false;

            bool wasGrounded = m_IsGround;
            CheckGround();
            //m_PlayerAnimator.SetBool(GameConstants.playerOnGround, m_IsGround);

            //land now
            if (m_IsGround && !wasGrounded)
            {
                m_Animator.SetBool(GameConstants.playerJumpNow, false);
                //SetAnimationValue(AnimatorType.Bool, GameConstants.playerJumpNow, false);
            }

            //If you Stun Or Captured??
            if ( m_IsStuned || m_IsCaptured || m_IsDead)
            {

                if ( (m_IsCaptured || m_IsDead) && m_InputHandler.GetTabKeyInputDown())
                {
                    CameraFollowOther();
                }

                //Never Move
                return;
            }
            else if (enabled && !cameraFollowMe)
            {
                CameraFollowMe();
            }

            //run or crouched?
            if (m_InputHandler.GetRunKeyInputDown())
            {
                m_IsRunning = true;
                m_IsCrouched = false;
            }
            else if (m_InputHandler.GetCrouchKeyInputDown())
            {
                m_IsRunning = false;
                m_IsCrouched = true;
            }

            if (m_IsRunning && (!m_InputHandler.GetRunKeyInputHeld() || m_InputHandler.GetMoveInput() == Vector3.zero) || m_Stamina <= 0)
            {
                m_IsRunning = false;
            }
            if (m_IsCrouched && !m_InputHandler.GetCrouchKeyInputHeld())
            {
                m_IsCrouched = false;
            }

            if (!m_IsRunning)
            {
                // INCREASE STAMINA WHEN NOT RUNNING
                this.StaminaIncrease();
                if (m_Stamina >= 100) {
                    m_GameManager.HideStaminaBar();
                }
            } else {
                // DECREASE STAMINA WHEN RUNNING
                m_GameManager.ShowStaminaBar();
                this.StaminaDecrease();
            }
            m_GameManager.SetStamina(this.GetStamina());
            HandleMovement();

            // CHECK INTERACTABLE OBJECT
            RaycastHit hitData = m_CommonRaycast.ShootRay(m_InteractionMinDist, m_InteractableMask);
            if (hitData.rigidbody) {
                m_TargetInteractable = hitData.rigidbody.GetComponent<IInteractable>();
                if (m_TargetInteractable != null) {
                    m_TargetInteractable.DrawOutline();
                }
            } else {
                if (m_TargetInteractable != null) {
                    m_TargetInteractable.RemoveOutline();
                    m_TargetInteractable = null;
                }
            }
        }
        #endregion

        public void SetHat()
        {

            
            string KEYWORD = m_GameManager.user.hat;

            m_PhotonView.RPC(nameof(I_HAVE_HAT), RpcTarget.AllBuffered, m_PhotonView.ViewID, KEYWORD);

            
        }

        [PunRPC]
        public void I_HAVE_HAT(int viewID, string KEYWORD)
        {
            if (viewID != m_PhotonView.ViewID)
                return;

            if (KEYWORD == null || KEYWORD.Equals("NONE") || KEYWORD.Equals(String.Empty))
            {
                foreach (var item in m_HatList)
                {
                    item.SetActive(false);
                }
                return;
            }

            string targetPrefab = null;

            switch (KEYWORD)
            {
                case nameof(RewardData.REWARD.GOD):
                    targetPrefab = "m_ElephantHat01";
                    break;
                case nameof(RewardData.REWARD.요리사모자):
                    targetPrefab = "m_ChefHat01";
                    break;
                case nameof(RewardData.REWARD.왕관):
                    targetPrefab = "m_CrownHat01";
                    break;
                case nameof(RewardData.REWARD.보글보글모자):
                    targetPrefab = "m_PirateHat01";
                    break;
                case nameof(RewardData.REWARD.혹):
                    targetPrefab = "m_MushroomHat01";
                    break;
                case nameof(RewardData.REWARD.물음표의모자):
                    targetPrefab = "m_PartyHat01";
                    break;
                default:
                    Debug.Log($"{KEYWORD}");
                    break;
            }

            foreach (var item in m_HatList)
            {
                if (item.name.Equals(targetPrefab))
                {
                    item.SetActive(true);
                }
                else
                {
                    item.SetActive(false);
                }
            }
        }

        #region GET AVATAR STATUS
        protected void GetAvatarStatus(string p_name)
        {
            // string name = nameof(AvatarData.AVATAR.p_name);
            AvatarData avatarData = GameManager.instance.avatar[p_name];

            if (m_PhotonView.IsMine)
            {
                SetHat();
            }

            m_AvatarId = avatarData.avatarId;
            m_AvatarName = avatarData.name;
            m_AvatarName = avatarData.desc;
            m_MovementSpeed = avatarData.movementSpeed;
            m_RotationSpeed = avatarData.rotationSpeed;
            m_JumpForce = avatarData.jumpForce;
            m_Stamina = avatarData.stamina;
            m_StaminaDecreasingFactor = avatarData.staminaDecreasingFactor;
            m_StaminaIncreasingFactor = avatarData.staminaIncreasingFactor;
            m_InteractionMinDist = avatarData.interactionMinDist;
            m_InteractionReadyTime = avatarData.interactionReadyTime;
            m_FootprintSpace = avatarData.footprintSpace;
            m_SpeedCoFactor = avatarData.speedCoFactor;
            m_JumpForceCoFactor = avatarData.jumpForceCoFactor;
        }
        #endregion

        #region METHODS
        public void CheckGround()
        {
            m_IsGround = false;

            if (m_LastJumpedTime + 0.1 > Time.time)
            {
                return;
            }

            //Feet�� Floor Layer�� ����ִ°�??
            if (Physics.CheckSphere(m_FeetTransform.position, m_GroundCheckDistanceInAir, m_FloorMask))
            // if (Physics.BoxCast(m_FeetTransform.position + new Vector3(0, 0.1f, 0), transform.lossyScale, Quaternion.identity, -transform.up, m_GroundCheckDistanceInAir, m_FloorMask))
            {
                //SetAnimationValue(AnimatorType.Bool, GameConstants.playerJumpNow, false);
                //m_Animator.SetBool(GameConstants.playerJumpNow, false);
                //m_PlayerAnimator.SetBool(GameConstants.playerJum
                m_IsGround = true;
                return;
            }

            return;
        }

        public void CameraFollowMe()
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCameraArm");
            GameObject firstCamera = camera.transform.Find("First Camera").gameObject;


            camera.transform.position = transform.position;
            camera.GetComponent<CameraMovement>().m_FollowObject = m_PlayerCamera;
            camera.transform.SetParent(transform);
            GameManager.instance.playerCamera = camera;

            firstCamera.GetComponent<AudioListener>().enabled = true;

            cameraFollowMe = true;

            GameObject airViewCamera = GameObject.FindGameObjectWithTag("AirViewCamera");
            if (airViewCamera != null) {
                airViewCamera.GetComponent<AirViewCameraMovement>().m_FollowObject = gameObject;
            }
        }
        
        public void CameraFollowOther()
        {
            List<GameObject> players = GameFlowManager.instance.m_Players.Where(r => {
                return r.GetComponent<Chef>() == null; 
            }).ToList();

            GameObject camera = GameObject.FindGameObjectWithTag("MainCameraArm");
            GameObject nowFollowedPlayer = camera.GetComponentInParent<Rigidbody>().gameObject;

            for (int i = 0; i < players.Count; i++)
            {
                GameObject player = players[i];

                if (player.Equals(nowFollowedPlayer))
                {
                    if ( i == players.Count - 1 )
                    {
                        players[0].GetComponent<PlayerController>().CameraFollowMe();
                    }
                    else
                    {
                        players[i+1].GetComponent<PlayerController>().CameraFollowMe();
                    }

                    break;
                }
            }
            cameraFollowMe = false;
        }

        protected void SetMyMapPointerColor(Color color)
        {
            transform.Find("PlayerPointer").GetComponent<MeshRenderer>().material.color = color;
        }

        protected void ShowFriendPointer()
        {
            if (GameFlowManager.instance == null) return;
            GameObject.Find("MapCamera").GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("RunnerPointer");
        }

        protected void ShowChefPointer()
        {
            if (GameFlowManager.instance == null) return;
            GameObject.Find("MapCamera").GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("ChefPointer");
        }

        protected void HideFriendPointer()
        {
            if (GameFlowManager.instance == null) return;
            // GameObject.Find("MapCamera").GetComponent<Camera>().cullingMask = GameObject.Find("MapCamera").GetComponent<Camera>().cullingMask & LayerMask.NameToLayer("Invisible");
        }

        protected IEnumerator FadeMyPointerToFullGreen()
        {
            MeshRenderer playerPointer = transform.Find("PlayerPointer").GetComponent<MeshRenderer>();
            Color color = new Color(playerPointer.material.color.r, playerPointer.material.color.g, playerPointer.material.color.b, playerPointer.material.color.a);
            playerPointer.material.color = color;
            while (playerPointer.material.color.g < 1f)
            {
                color = new Color(playerPointer.material.color.r, playerPointer.material.color.g + (Time.deltaTime / 1.0f), playerPointer.material.color.b, playerPointer.material.color.a);
                playerPointer.material.color = color;
                yield return null;
            }
            StartCoroutine(FadeMyPointerToLowerGreen());
        }

        protected IEnumerator FadeMyPointerToLowerGreen()
        {
            MeshRenderer playerPointer = transform.Find("PlayerPointer").GetComponent<MeshRenderer>();
            Color color = new Color(playerPointer.material.color.r, playerPointer.material.color.g, playerPointer.material.color.b, playerPointer.material.color.a);
            playerPointer.material.color = color;
            while (playerPointer.material.color.g > 0.6f)
            {
                color = new Color(playerPointer.material.color.r, playerPointer.material.color.g - (Time.deltaTime / 1.0f), playerPointer.material.color.b, playerPointer.material.color.a);
                playerPointer.material.color = color;
                yield return null;
            }
            StartCoroutine(FadeMyPointerToFullGreen());
        }

        protected void ChangePlayerCameraPerspective()
        {
            if (m_IsInteracting && m_InteractingTime > m_InteractionReadyTime)
            {
                if (CameraMovement.instance)
                    CameraMovement.instance.ChangeFirstPerspective();
            }
            else
            {
                if (CameraMovement.instance)
                    CameraMovement.instance.ChangeThirdPerspective();
            }
        }

        private void StaminaDecrease()
        {
            if (this.m_Stamina <= 0) return;

            if (SceneManager.GetActiveScene().name.Equals("Metaverse"))
            {
                return;
            }

            this.m_Stamina -= Time.deltaTime * m_StaminaDecreasingFactor;
        }

        private void StaminaIncrease()
        {
            if (this.m_Stamina >= 100) return;
            this.m_Stamina += Time.deltaTime * m_StaminaIncreasingFactor;
        }

        public int GetStamina()
        {
            if (this.m_Stamina > 99) return 100;
            return (int)this.m_Stamina;
        }

        public bool GetStunned()
        {
            if (m_IsStuned) return false;

            //status change
            this.m_IsStuned = true;

            //run stunAnimation
            //SetAnimationValue(AnimatorType.Bool, "BeStunned", true);
            m_Animator.SetBool("BeStunned", true);

            return true;
        }

        public void StopStunned()
        {
            if (!m_IsStuned) return;

            m_IsStuned = false;
            //SetAnimationValue(AnimatorType.Bool, "BeStunned", false);
            m_Animator.SetBool("BeStunned", false);

            return;
        }

        public void BeCaptured()
        {
            if (m_IsCaptured) return;

            m_IsCaptured = true;

            m_Animator.SetBool("IsCaptured", true);

            //Record this
            m_PlayInfo.CountCaptured++;
        }

        public void BeWasted()
        {
            m_IsWet = true;
            m_IsChefTakeMe = false;

            //Camera can Follow Other user

            m_PhotonView.RPC(nameof(BeImprisoned), RpcTarget.All, m_PhotonView.ViewID);
        }

        public void BeSave()
        {
            m_IsCaptured = false;

            //SetAnimationValue(AnimatorType.Bool, "IsCaptured", false);
            m_Animator.SetBool("IsCaptured", false);
            m_PhotonView.RPC(nameof(BeFreeMan), RpcTarget.All, m_PhotonView.ViewID);
        }

        public void SetWetState(bool state)
        {
            this.m_IsWet = state;
        }

        public void SetIsAnimating(bool state)
        {
            this.m_IsAnimating = state;
        }

        public void DelaySetIsAnimatingOff()
        {
            this.m_IsAnimating = false;
        }

        public string GetCharacterName()
        {
            return this.m_CharacterName;
        }

        public void HandleMovement()
        {
            // IF PLAYER IS INTERACTING OR ANIMATING, CAN'T MOVE
            if (m_IsInteracting || m_IsAnimating || m_IsChefTakeMe) return;
            if (GameManager.instance.m_MovementLock || GameManager.instance.m_ChatInputFocused) {
                m_Animator.SetFloat(GameConstants.playerVerticalVelocity, 0f);
                m_Animator.SetFloat(GameConstants.playerHorizonalVelocity, 0f);
                return;
            }

            //horizonal camera movement
            {
                //Alt ��ư�� ������ �ֳ���?
                if (!m_InputHandler.GetAltInputHeld())
                {
                    m_PlayerBody.MoveRotation(m_PlayerBody.rotation * Quaternion.Euler(new Vector3(0f, (m_InputHandler.GetLookInputHorizontal() * m_RotationSpeed * m_RotationMultiplier), 0f)));
                    m_PlayerCamera.rotation = m_PlayerBody.rotation;
                }
            }

            {

                //Is Ground?
                if (m_IsGround)
                {

                    //Get Input
                    Vector3 inputVector = m_InputHandler.GetMoveInput();

                    // WHEIGHT MAN
                    // WHEN YOU HAVE HEAVY ITEM, SLOW SPEED
                    float weightFactor = m_IsHeavy ? 0.5f : 1.0f;

                    verticalSpeed = Mathf.Lerp(verticalSpeed, inputVector.z, lerpSpeed * Time.deltaTime);
                    horizonalSpeed = Mathf.Lerp(horizonalSpeed, inputVector.x, lerpSpeed * Time.deltaTime );
                    if (Math.Abs(verticalSpeed) < 0.001) verticalSpeed = 0;
                    if (Math.Abs(horizonalSpeed) < 0.001) horizonalSpeed = 0;

                    inputVector.z = verticalSpeed;
                    inputVector.x = horizonalSpeed;
                    //not walk
                    if ( inputVector.z == 0 && inputVector.x == 0)
                    {
                        notWalkTime += _deltaTime;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;

                        if (  notWalkTime < m_PlayInfo.CountNotWalk )
                        {
                            m_PlayInfo.CountNotWalk = notWalkTime;
                        }
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        notWalkTime = 0;
                    }

                    if ( m_Stamina >= 100 )
                    {
                        slowWalkTime += _deltaTime;

                        if ( slowWalkTime < m_PlayInfo.CountWalkSlowly)
                        {
                            m_PlayInfo.CountWalkSlowly = slowWalkTime;
                        }
                    }
                    else
                    {
                        slowWalkTime = 0;
                    }

                    // WHEN YOU ARE WET, SLOW SPEED
                    weightFactor = m_IsWet ? weightFactor * 0.8f : weightFactor;
                    Vector3 moveVector = transform.TransformDirection(inputVector) * m_Modified_MovementSpeed * weightFactor;

                    horizonalSpeed = Mathf.Clamp(horizonalSpeed, -0.5f, 0.5f);
                    verticalSpeed = Mathf.Clamp(verticalSpeed, -0.5f, 0.5f);

                    //SetAnimationValue(AnimatorType.Float, GameConstants.playerVerticalVelocity, verticalSpeed * m_GravityForAnimator);
                    //SetAnimationValue(AnimatorType.Float, GameConstants.playerHorizonalVelocity, horizonalSpeed * m_GravityForAnimator);
                    
                    //animation setting
                    m_Animator.SetFloat(GameConstants.playerVerticalVelocity, verticalSpeed * m_GravityForAnimator);
                    m_Animator.SetFloat(GameConstants.playerHorizonalVelocity, horizonalSpeed * m_GravityForAnimator);

                    //set velocity
                    m_PlayerBody.velocity = new Vector3(moveVector.x, m_PlayerBody.velocity.y, moveVector.z);

                    //click jump button?
                    if (m_InputHandler.GetJumpInputDown())
                    { 
                        //jump
                        m_PlayerBody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);

                        //m_PlayerAnimator.SetTrigger(GameConstants.playerJumpTrigger);
                        //m_PlayerAnimator.SetBool(GameConstants.playerJumpNow, true);
                        
                        //set jump animation
                        //SetAnimationValue(AnimatorType.Trigger, GameConstants.playerJumpTrigger);
                        //SetAnimationValue(AnimatorType.Bool, GameConstants.playerJumpNow, true);
                        m_Animator.SetBool(GameConstants.playerJumpNow, true);

                        //not crouched now
                        if (m_IsCrouched)
                        {
                            m_IsCrouched = false;
                        }

                        //record jump time
                        m_LastJumpedTime = Time.time;
                        m_HasJumpedThisFrame = true;

                        //m_IsGround => false
                        m_IsGround = false;
                    }
                    else
                    {
                        //do not jump
                        
                    }
                }
                //in air
                else
                {
                    //... just falling?
                }
            }
        }

        protected void SetAnimationValue(AnimatorType type, String animationParameter, object value = null)
        {
            String aniType;
            switch (type)
            {
                case AnimatorType.Bool:
                    aniType = "Bool";
                    break;
                case AnimatorType.Float:
                    aniType = "Float";
                    break;
                case AnimatorType.Int:
                    aniType = "Int";
                    break;
                case AnimatorType.Trigger:
                    aniType = "Trigger";
                    break;
                default:
                    aniType = "error man";
                    break;
            }

            m_NetworkedAnimation.SendPlayAnimationEvent(m_PhotonView.ViewID, animationParameter, aniType, value);
        }

        public void StartUpdateMyNickname()
        {
            StartCoroutine(nameof(UpdateMyNicknameInterval), m_PhotonView.ViewID);
            GameManager.instance.ShowViewNickname();
            if (gameObject.GetComponent<Runner>() != null) {
                GameManager.instance.HideChefNickname();
            }
        }
        
        IEnumerator UpdateMyNicknameInterval(int viewID)
        {
            while (PhotonNetwork.InRoom)
            {
                m_PhotonView.RPC(nameof(MyPlayerInfoRPC), RpcTarget.All, viewID, GameManager.instance.user.title, GameManager.instance.user.nickname);
                yield return new WaitForSeconds(5f);
            }
            yield return null;
        }


        #endregion

        #region RPC
        [PunRPC]
        public void MyPlayerInfoRPC(int viewId, string title, string nickname)
        {
            if (m_PhotonView.ViewID != viewId) return;

            string titleGrade = "NORMAL";
            foreach(RewardData data in GameManager.instance.reward.Values) {
                if (title.Equals(data.keyValue)) {
                    titleGrade = data.grade;
                    title = data.name;
                    break;
                }
            }

            Color titleColor = Color.green;
            switch (titleGrade) {
                case "NORMAL":
                    titleColor = Color.green;
                    break;
                case "RARE":
                    titleColor = Color.blue;
                    break;
                case "UNIQUE":
                    titleColor = Color.red;
                    break;
            }

            GameObject me = PhotonView.Find(viewId).gameObject;
            Transform userInfo = me.transform.Find("Text_UserInfo");
            userInfo.Find("Text_Title").GetComponent<TextMesh>().text = title;
            userInfo.Find("Text_Title").GetComponent<TextMesh>().color = titleColor;
            userInfo.Find("Text_Nickname").GetComponent<TextMesh>().text = nickname;
        }

        [PunRPC]
        public void BeImprisoned(int ViewID)
        {
            if (ViewID != m_PhotonView.ViewID)
                return;

            if( GameFlowManager.instance != null)
            {
                GameFlowManager.instance.CatchedMan(gameObject);
            }
        }

        [PunRPC]
        public void BeFreeMan(int ViewID)
        {
            if (ViewID != m_PhotonView.ViewID)
                return;

            if (GameFlowManager.instance != null)
            {
                GameFlowManager.instance.SavedMan(gameObject);
            }
        }

        #endregion

        #region enums

        public enum AnimatorType
        {
            Bool = 0,
            Float = 1,
            Int = 2,
            Trigger = 3,
        }

        #endregion
    }
}
