using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public List<Rigidbody> playerRigidbody = new();

    private Quaternion previousRotation;

    void Start()
    {
        // �ʱ� ȸ�� ���� �����մϴ�.
        previousRotation = transform.rotation;
    }
}