using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    [SerializeField] float slidingSpeed = 5f; // �̲������� �ӵ��� �����մϴ�. ���� �����Ͽ� ���ϴ� �ӵ��� ������ �� �ֽ��ϴ�.

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        {
            // �浹�� ���� ǥ�� ������ �����ɴϴ�.
            Vector3 wallNormal = collision.GetContact(0).normal;

            // ���� ǥ�鿡 ������ �������� �ӵ��� �����մϴ�.
            Vector3 parallelToWall = Vector3.Cross(wallNormal, Vector3.up);
            m_Rigidbody.velocity += parallelToWall * slidingSpeed;
        }
    }
}
