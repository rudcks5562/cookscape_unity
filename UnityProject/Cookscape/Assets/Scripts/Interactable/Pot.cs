using Photon.Pun;
using UnityEngine;
using UnityProject.Cookscape;

public class Pot : MapObject
{
    protected override void Awake()
    {
        base.Awake();
        m_Gauge = 0f;
    }

    private void Start()
    {
        isClose = false;
        ObjectData objectData = GameManager.instance.mapObject[nameof(ObjectData.OBJECT.냄비)];
        m_ObjectId = objectData.objectId;
        m_ObjectName = objectData.name;
        m_Gauge = objectData.gauge;
        m_ChargingSpeed = objectData.chargingSpeed;
        m_DeChargingSpeed = objectData.dechargingSpeed;
        m_InteractableType = objectData.interactableType;
        m_ChargingSpeedCoFactor = objectData.chargingSpeedCoFactor;
        m_DeChargingSpeedCoFactor = objectData.dechargingSpeedCoFactor;
    }

    #region METHODS
    public override void Interact()
    {
        if (isClose) return;

        if (isFinished)
        {
            isClose = true;
            m_PhotonView.RPC("PotIsEnd", RpcTarget.All, m_PhotonView.ViewID);
            return;
        }

        ChargeGauge();
    }
    
    public override void Interact(int direction) {}
    #endregion

    [PunRPC]
    public void PotIsEnd(int ViewID)
    {
        if ( m_PhotonView.ViewID != ViewID)
        {
            return;
        }

        //Go Animation
        Animator animator = GetComponent<Animator>();

        if (GameFlowManager.instance != null)
            GameFlowManager.instance.BreakPot(gameObject);
        if (GameManager.instance != null)
            GameManager.instance.AlertInfoMsg(true, "알림", "솥이 파괴되었습니다");
        
        animator.SetBool("Break", true);
    }
}
