using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class VehicleAnimationController : MonoBehaviourPunCallbacks
{
    public Animator animator;
    public PhotonView m_PhotonView;
    private float syncInterval = 1f;
    private float timeSinceLastSync = 0f;

    private void Start()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (animator == null) return;

        if (m_PhotonView.IsMine)
        {
            timeSinceLastSync += Time.deltaTime;
            if (timeSinceLastSync >= syncInterval)
            {
                timeSinceLastSync = 0f;
                m_PhotonView.RPC(nameof(SyncAnimationTimeRPC), RpcTarget.Others, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
        }
    }

    [PunRPC]
    private void SyncAnimationTimeRPC(float normalizedTime)
    {
        animator.Play("Idle", 0, normalizedTime);
    }
}
