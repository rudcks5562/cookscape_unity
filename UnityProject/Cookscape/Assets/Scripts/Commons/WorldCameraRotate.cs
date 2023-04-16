using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCameraRotate : MonoBehaviour
{
    public float m_RotateSpeed;
    void Awake()
    {
        
    }

    void Update()
    {
        RotateAxis();
    }

    void RotateAxis()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * m_RotateSpeed);
    }
}
