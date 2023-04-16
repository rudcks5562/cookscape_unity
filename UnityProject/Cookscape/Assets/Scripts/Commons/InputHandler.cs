using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FPS Tutorial ����
//������ Input �ڵ鸵 ��
namespace UnityProject.Cookscape
{
    public class InputHandler : MonoBehaviour
    {
        #region  EXTERNAL VARIABLES
        public static InputHandler instance;

        // MEMBER VARIABLES
        [Tooltip("Sensitivity multiplier for moving the came" +
            "ra around")]
        public float m_LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float m_Webgl_LookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float m_TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool m_InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool m_InvertXAxis = false;

        float m_LastXInput = 0;
        float m_LastYInput = 0;
        #endregion

        #region LIFECYCLE
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        protected void Start()
        {
            //Corsur Status => Locked
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        private void Update()
        {
            GetESCInputDown();
        }

        #endregion

        #region METHODS
        //���� ��ǲ �޾Ƶ� �Ǵ��� üũ
        public bool CanProcessInput()
        {
            //Cursor Status is Locked ? True : False
            // if (GameFlowManager.instance)
            //     return Cursor.lockState == CursorLockMode.Locked;

            return true;
        }

        //�󸶳� �̵��ϴ��� �ޱ�
        public Vector3 GetMoveInput()
        {
            m_LastXInput = 0;
            m_LastYInput = 0;

            if (CanProcessInput())
            {
                m_LastXInput = Input.GetAxisRaw(GameConstants.axisNameHorizontal);
                m_LastYInput = Input.GetAxisRaw(GameConstants.axisNameVertical);

                Vector3 move = new Vector3(m_LastXInput, 0f, m_LastYInput);

                move = Vector3.ClampMagnitude(move, 1);
                return move;
            }

            //��ǲ ����
            return Vector3.zero;
        }

        public bool HasHorizonalPlusInput()
        {
            return m_LastXInput > 0;
        }

        public bool HasHorizonalMinusInput()
        {
            return m_LastXInput < 0;
        }

        public bool HasVerticalPlusInput()
        {
            return m_LastYInput > 0;
        }

        public bool HasVerticalMinusInput()
        {
            return m_LastYInput < 0;
        }

        float GetMouseLookAxis(string mouseInputName)
        {
            if (CanProcessInput())
            {
                float val = Input.GetAxisRaw(mouseInputName);

                if ((m_InvertYAxis && mouseInputName.Equals(GameConstants.mouseAxisNameVertical))
                   || (m_InvertXAxis && mouseInputName.Equals(GameConstants.mouseAxisNameHorizontal)))
                {
                    val *= -1f;
                }

                val *= m_LookSensitivity;

                val *= 0.01f;
                #if UNITY_WEBGL
                    // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                    val *= m_Webgl_LookSensitivityMultiplier;
                #endif
                return val;
            }

            return 0f;
        }

        //�¿� �����̵� �ޱ�
        public float GetLookInputHorizontal()
        {
            return GetMouseLookAxis(GameConstants.mouseAxisNameHorizontal);
        }

        //���� �����̵� �ޱ�
        public float GetLookInputVertical()
        {
            return GetMouseLookAxis(GameConstants.mouseAxisNameVertical);
        }

        //Ű Input Downs ================================================
        public bool GetJumpInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.buttonNameJump);
            }

            return false;
        }

        public bool GetRunKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.LeftShift);
            }
            return false;
        }

        public bool GetCrouchKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.LeftControl);
            }
            return false;
        }

        public bool GetAttackKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Mouse0);
            }
            return false;
        }

        public bool GetEKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.E);
            }
            return false;
        }

        public bool GetEKeyHeldDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKey(KeyCode.E);
            }
            return false;
        }

        public bool GetQKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Q);
            }
            return false;
        }

        public bool GetFKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.F);
            }
            return false;
        }

        public bool GetCKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.C);
            }
            return false;
        }

        public bool GetMKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.M);
            }
            return false;
        }

        public bool GetEnterKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Return);
            }
            return false;
        }

        public bool GetBackQuoteKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.BackQuote);
            }
            return false;
        }

        public bool GetBackNumber1KeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Alpha1);
            }
            return false;
        }

        public bool GetBackNumber2KeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Alpha2);
            }
            return false;
        }

        public bool GetBackNumber3KeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Alpha3);
            }
            return false;
        }

        public bool GetBackNumber4KeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Alpha4);
            }
            return false;
        }

        public bool GetBackNumber5KeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Alpha5);
            }
            return false;
        }

        public bool GetTabKeyInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyDown(KeyCode.Tab);
            }
            return false;
        }

        //Key Input Helds ======================================================
        public bool GetJumpInputHeld()
        {
            if (!CanProcessInput())
            {
                return Input.GetButton(GameConstants.buttonNameJump);
            }

            return false;
        }

        public bool GetCrouchKeyInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetKey(KeyCode.LeftControl);
            }

            return false;
        }

        public bool GetRunKeyInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetKey(KeyCode.LeftShift);
            }

            return false;
        }

        public bool GetAltInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetKey(KeyCode.LeftAlt);
            }

            return false;
        }

        //Key Input Ups ======================================================
        public bool GetAltInputUp()
        {
            if (CanProcessInput())
            {
                return Input.GetKeyUp(KeyCode.LeftAlt);
            }
            return false;
        }

        #endregion

        public void GetESCInputDown()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if ( CanProcessInput() )
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}
