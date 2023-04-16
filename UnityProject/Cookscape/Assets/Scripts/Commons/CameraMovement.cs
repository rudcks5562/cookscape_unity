using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityProject.Cookscape;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;
    
    public Transform m_FollowObject;

    [SerializeField] float followSpeed = 10f;
    [SerializeField] float sensitive = 1200f;
    [SerializeField] float verticalClampAngle = 70f;
    [SerializeField] float smoothness = 10f;

    [Tooltip("layerMask for camera position movement")]
    public LayerMask layerMask;

    private InputHandler m_InputHandler;
    private float rotateX;
    private float rotateY;

    public Transform realCamera;

    private bool perspectiveIsFirst = false;
    public GameObject m_MainCamera;
    public GameObject m_FirstPerspectiveCamera;

    Vector3 dirNormalize;
    Vector3 finalDir;

    public float minDistance;
    public float maxDistance;
    public float finalDistance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_InputHandler = InputHandler.instance;

        rotateX = transform.localRotation.eulerAngles.x;
        rotateY = transform.localRotation.eulerAngles.y;

        dirNormalize = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FollowObject == null)
            return;
        if (GameManager.instance.m_CameraLock || GameManager.instance.m_ChatInputFocused) return;

        rotateX += m_InputHandler.GetLookInputVertical() * sensitive * Time.deltaTime; 
        if ( m_InputHandler.GetAltInputHeld())
        {
            rotateY += m_InputHandler.GetLookInputHorizontal() * sensitive * Time.deltaTime;
        }

        if (m_InputHandler.GetAltInputUp())
        {
            rotateY = 0;
        }

        rotateX = Mathf.Clamp(rotateX, -verticalClampAngle, verticalClampAngle);

        //Quaternion rot = Quaternion.Euler(rotateX, rotateY, 0);
        transform.localEulerAngles = new Vector3(rotateX, rotateY, 0);

    }

    private void LateUpdate()
    {
        if (m_FollowObject == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, m_FollowObject.position, followSpeed * Time.deltaTime);

        finalDir = transform.TransformPoint(dirNormalize * maxDistance);

        if (Physics.Linecast(transform.position, finalDir, out RaycastHit hit, layerMask))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }

        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalize * finalDistance, Time.deltaTime * smoothness);
    }

    public void ChangeFirstPerspective()
    {
        if ( perspectiveIsFirst )
        {
            return;
        }

        // m_FirstPerspectiveCamera.SetActive(true);
        // m_MainCamera.SetActive(false);
        m_MainCamera.GetComponent<Camera>().enabled = false;
        //m_MainCamera.GetComponent<AudioListener>().enabled = false;
        m_FirstPerspectiveCamera.GetComponent<Camera>().enabled = true;
        perspectiveIsFirst = true;
    }

    public void ChangeThirdPerspective()
    {
        if (!perspectiveIsFirst)
        {
            return;
        }

        // m_MainCamera.SetActive(true);
        // m_FirstPerspectiveCamera.SetActive(false);
        m_FirstPerspectiveCamera.GetComponent<Camera>().enabled = false;
        //m_FirstPerspectiveCamera.GetComponent<AudioListener>().enabled = false;
        m_MainCamera.GetComponent<Camera>().enabled = true;
        perspectiveIsFirst = false;
    }
}
