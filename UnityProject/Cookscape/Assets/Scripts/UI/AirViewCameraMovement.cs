using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirViewCameraMovement : MonoBehaviour
{
    public GameObject m_FollowObject;

    void Update()
    {
        if (m_FollowObject != null) {
            transform.position = new Vector3(m_FollowObject.transform.position.x, m_FollowObject.transform.position.y + 100f, m_FollowObject.transform.position.z);
        }
    }
}
