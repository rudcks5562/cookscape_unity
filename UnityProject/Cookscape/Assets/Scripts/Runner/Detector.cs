using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public LayerMask m_DetectLayer;
    bool m_IsDetecting = false;
    float m_DetectingTime = 0f;
    float m_DetectingRange = 10.0f;
    bool m_IsEncounterSoundPlaying = false;

    private void Update()
    {
        ShootDetectingRaycast();
        if (m_IsDetecting) {
            //Debug.LogError($"Chef Detected!!! : {m_DetectingTime}");
            m_DetectingTime += Time.deltaTime;
        } else {
            //Debug.LogError("Chef NOT Detected...");
            m_DetectingTime = 0f;
        }

        if (m_DetectingTime > 6.0f) {
            // transform.parent.GetComponent<ChefChaseSound>().Play();
        }
    }

    private void ShootDetectingRaycast()
    {
        bool isDetected = false;
        #region RAYCAST SHOOT
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, m_DetectingRange, m_DetectLayer))
        {
            if (hit.rigidbody.CompareTag("Chef")) {
                isDetected = true;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, m_DetectingRange, m_DetectLayer))
        {
            if (hit.rigidbody.CompareTag("Chef")) {
                isDetected = true;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, m_DetectingRange, m_DetectLayer))
        {
            if (hit.rigidbody.CompareTag("Chef")) {
                isDetected = true;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, m_DetectingRange, m_DetectLayer))
        {
           if (hit.rigidbody.CompareTag("Chef")) {
                isDetected = true;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 45, 0) * Vector3.forward), out hit, m_DetectingRange, m_DetectLayer))
        {
            if (hit.rigidbody.CompareTag("Chef")) {
                isDetected = true;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 45, 0) * (-Vector3.forward)), out hit, m_DetectingRange, m_DetectLayer))
        {
            if (hit.rigidbody.CompareTag("Chef")) {
                isDetected = true;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 45, 0) * Vector3.left), out hit, m_DetectingRange, m_DetectLayer))
        {
            if (hit.rigidbody.CompareTag("Chef")) {
                isDetected = true;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 45, 0) * Vector3.right), out hit, m_DetectingRange, m_DetectLayer))
        {
           if (hit.rigidbody.CompareTag("Chef")) {
                isDetected = true;
            }
        }
        #endregion

        if (isDetected && !m_IsEncounterSoundPlaying) {
            // 셰프 조우 사운드 재생
            StartCoroutine("PlayEncounterSound");
            m_IsDetecting = true;
        }
        m_IsDetecting = false;
    }

    private IEnumerator PlayEncounterSound()
    {
        if (!m_IsEncounterSoundPlaying) {
            m_IsEncounterSoundPlaying = true;
            transform.GetComponent<ChefEncounterSound>().Play();
            yield return new WaitForSeconds(10.0f);
            m_IsEncounterSoundPlaying = false;
        }
    }
}
