using UnityProject.Cookscape;
using Photon.Pun;
using UnityEngine;

public class Valve : MapObject
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        m_Gauge = 0f;
        m_ObjectName = "Valve";
    }

    private void Start()
    {
        ObjectData objectData = GameManager.instance.mapObject[nameof(ObjectData.OBJECT.밸브)];
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
            m_PhotonView.RPC("ValveIsEnd", RpcTarget.All, m_PhotonView.ViewID);
            return;
        }

        ChargeGauge();
    }

    public override void Interact(int direction)
    {
        if (isClose) return;
        UnChargeGauge();
    }

    [PunRPC]
    public void ValveIsEnd(int ViewID)
    {
        if (m_PhotonView.ViewID != ViewID)
        {
            return;
        }

        //Go Animation
        if (GameFlowManager.instance != null )
            GameFlowManager.instance.OpenPot(gameObject);
    }

    #endregion
}
