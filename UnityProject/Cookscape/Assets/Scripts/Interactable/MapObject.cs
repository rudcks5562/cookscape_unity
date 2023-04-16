using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityProject.Cookscape;
using cakeslice;
using Photon.Pun;

public abstract class MapObject : MonoBehaviour, IInteractable
{
    // CHARGING SPEED PER ONE SECOND
    protected bool m_OutLined = false;
    protected PhotonView m_PhotonView;
    // IS THIS OBJ PROCESS FINISHED?
    protected bool isFinished = false;
    public bool isClose = false;
    public int m_ObjIndex; // CAN GET THIS WITH GETTER

    // CONSTANT STATUS
    protected long m_ObjectId;
    protected string m_ObjectName;
    protected float m_Gauge = 0;
    [SerializeField] protected float m_ChargingSpeed = 1f;
    [SerializeField] protected float m_DeChargingSpeed = 2f;
    protected string m_InteractableType;
    protected float m_ChargingSpeedCoFactor;
    protected float m_DeChargingSpeedCoFactor;

    Renderer renderers;
    List<Material> materialList = new List<Material>();

    protected virtual void Awake()
    {
        this.m_PhotonView = GetComponent<PhotonView>();
    }

    #region METHODS
    public void DrawOutline()
    {
        if (m_OutLined) return;
        m_OutLined = true;
        
        Transform[] allChildren = transform.GetComponentsInChildren<Transform>();
        foreach(Transform t in allChildren) {
            Outline ol = t.GetComponent<Outline>();
            if (ol != null) {
                ol.enabled = true;
            }
        }
    }

    public void RemoveOutline()
    {
        if (!m_OutLined) return;
        m_OutLined = false;

        Transform[] allChildren = transform.GetComponentsInChildren<Transform>();
        foreach(Transform t in allChildren) {
            Outline ol = t.GetComponent<Outline>();
            if (ol != null) {
                ol.enabled = false;
            }
        }
    }

    public void SetObjIndex(int idx)
    {
        this.m_ObjIndex = idx;
    }

    public int GetObjIndex()
    {
        return this.m_ObjIndex;
    }

    public string GetObjectName() {
        return this.m_ObjectName;
    }

    public float GetGauge() {
        if (this.m_Gauge >= 100) return 100;
        if (this.m_Gauge <= 0) return 0;
        return this.m_Gauge;
    }

    public void ChargeGauge()
    {
        float chargingValue = Time.deltaTime * m_ChargingSpeed * m_ChargingSpeedCoFactor;
        m_PhotonView.RPC("Charging", RpcTarget.All, chargingValue);
    }

    public void UnChargeGauge()
    {
        float unChargingValue = Time.deltaTime * m_DeChargingSpeed * m_DeChargingSpeedCoFactor;
        m_PhotonView.RPC("UnCharging", RpcTarget.All, unChargingValue);
    }

    [PunRPC]
    public void Charging(float _chargingValue)
    {
        if (m_Gauge >= 100) {
            Debug.Log("End Man");
            isFinished = true;
            return;
        }
        m_Gauge += _chargingValue;
    }

    [PunRPC]
    public void UnCharging(float _unChargingValue)
    {
        if (m_Gauge <= 0) return;
        m_Gauge -= _unChargingValue;
    }
   
    #endregion

    #region ABSTRACT METHODS
    public abstract void Interact();
    public abstract void Interact(int direction);
    #endregion
}
