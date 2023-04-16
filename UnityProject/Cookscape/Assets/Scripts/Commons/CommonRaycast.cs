using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonRaycast : MonoBehaviour
{
    public LayerMask interactable;
    
    Camera playerCamera;

    protected virtual void Start()
    {
        playerCamera = Camera.main;
    }

    public RaycastHit ShootRay(float p_RayDistance)
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitData;
        // if (Physics.Raycast(ray, out hitData, p_RayDistance, interactable)) {
        if (Physics.SphereCast(ray, 1f, out hitData, p_RayDistance, interactable)) {
            return hitData;
        }
        return hitData;
    }

    public RaycastHit ShootRay(float p_RayDistance, LayerMask p_LayerMask)
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitData;
        // if (Physics.Raycast(ray, out hitData, p_RayDistance, p_LayerMask))
        if (Physics.SphereCast(ray, 1f, out hitData, p_RayDistance, p_LayerMask))
        {
            return hitData;
        }
        return hitData;
    }
}
