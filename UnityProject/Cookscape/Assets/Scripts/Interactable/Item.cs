using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityProject.Cookscape;
using cakeslice;
using Photon.Pun;

public abstract class Item : MonoBehaviour, IInteractable
{
    // CONSTANT STATUS
    protected long m_ItemId;
    protected string m_ItemName;
    protected string m_Desc;
    protected int m_UseCount;
    protected float m_Weight = 2.0f;
    protected bool m_OutLined = false;
    protected PhotonView m_PhotonView;

    Material outline;
    Renderer renderers;
    List<Material> materialList = new List<Material>();

    protected virtual void Awake()
    {
        this.m_PhotonView = GetComponent<PhotonView>();
    }

    #region METHODS
    private void GetItemData()
    {

    }

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

    public string GetItemName() {
        return this.m_ItemName;
    }

    public float GetWeight() {
        return this.m_Weight;
    }

    public int DecreaseUseCount()
    {
        this.m_UseCount -= 1;
        return this.m_UseCount;
    }
    #endregion


    // ABSTRACT METHODS
    #region ABSTRACT METHODS
    public abstract void Use(GameObject gameObject = null);
    #endregion
}
