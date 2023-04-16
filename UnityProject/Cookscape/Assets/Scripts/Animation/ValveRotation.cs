using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveRotation : MonoBehaviour
{
    [SerializeField] float spinSpeed = 50f;
    public bool IsRotating = false;
    public bool IsReverseRotating = false;

    private void FixedUpdate()
    {
        if (IsRotating && IsReverseRotating)
        {
            StopSpin();
        }
        else if (IsRotating)
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        }
        else if (IsReverseRotating)
        {
            transform.Rotate(-Vector3.up, spinSpeed * Time.deltaTime);
        }
    }

    public void StartSpin()
    {
        if (IsRotating)
        {
            return;
        }

        IsRotating = true;
    }

    public void StartReverseSpin()
    {
        if (IsReverseRotating)
        {
            return;
        }

        IsReverseRotating = true;
    }

    public void StopSpin()
    {
        if (!IsRotating && !IsReverseRotating)
        {
            return;
        }

        IsRotating = false;
        IsReverseRotating = false;
    }
}
