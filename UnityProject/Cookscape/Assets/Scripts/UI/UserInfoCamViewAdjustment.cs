using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoCamViewAdjustment : MonoBehaviour
{
    GameObject m_Camera;
    Vector3 m_InitScale;
    public float distance = 3;

    void Start()
    {
        m_Camera = GameManager.instance.playerCamera;
        m_InitScale = transform.localScale; 
    }

    void Update()
    {
        if (m_Camera != null) {
            transform.rotation = m_Camera.transform.rotation;

            // float dist = Vector3.Distance(m_Camera.transform.position, transform.position);
            // Vector3 adjustScale = m_InitScale * dist / distance * 4.0f;
            // transform.localScale = adjustScale;
        }
    }
}
