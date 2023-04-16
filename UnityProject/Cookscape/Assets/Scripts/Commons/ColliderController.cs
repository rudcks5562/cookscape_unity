using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    [SerializeField] float slidingSpeed = 5f; // 미끄러지는 속도를 설정합니다. 값을 조절하여 원하는 속도를 설정할 수 있습니다.

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        {
            // 충돌한 벽의 표면 법선을 가져옵니다.
            Vector3 wallNormal = collision.GetContact(0).normal;

            // 벽의 표면에 수직인 방향으로 속도를 조절합니다.
            Vector3 parallelToWall = Vector3.Cross(wallNormal, Vector3.up);
            m_Rigidbody.velocity += parallelToWall * slidingSpeed;
        }
    }
}
