using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public List<Rigidbody> playerRigidbody = new();

    private Quaternion previousRotation;

    void Start()
    {
        // 초기 회전 값을 저장합니다.
        previousRotation = transform.rotation;
    }
}