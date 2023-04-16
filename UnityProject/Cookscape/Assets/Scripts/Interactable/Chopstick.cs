using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityProject.Cookscape;

public class Chopstick : Item
{
    #region PRIVATE METHODS
    Animator m_OwnerAnimator;
    int m_TargetViewID;
    #endregion

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        ItemData itemData = GameManager.instance.item[nameof(ItemData.ITEM.나무젓가락)];
        m_ItemId = itemData.itemId;
        m_ItemName = itemData.name;
        m_UseCount = itemData.useCount;
        m_Weight = itemData.weight;
        m_Desc = itemData.desc;
    }

    #region METHODS
    public override void Use(GameObject gameObject)
    {
        Debug.Log(m_ItemName + "을(를) 사용");
        //...

        int ViewID = gameObject.GetComponent<PhotonView>().ViewID;

        m_PhotonView.RPC(nameof(SaveAction), RpcTarget.AllBuffered, ViewID);
    }
    #endregion

    #region RPC
    [PunRPC]
    void SaveAction(int ViewID)
    {
        m_TargetViewID = ViewID;
        StopCoroutine("Save");
        StartCoroutine("Save");
    }
    #endregion

    IEnumerator Save()
    {
        m_OwnerAnimator = GetComponentInParent<Animator>();

        //Start Animation
        Debug.Log("Start Save Animation....");
        m_OwnerAnimator.SetBool("IsSave", true);

        //Wait Time...
        yield return new WaitForSeconds(4.0f);

        //Save Him
        GameObject catchee = PhotonView.Find(m_TargetViewID).gameObject;
        
        PlayerController playerController = catchee.GetComponent<PlayerController>();
        playerController.BeSave();
        playerController.StopStunned();

        catchee.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        catchee.transform.rotation = Quaternion.identity;

        Runner runner = m_OwnerAnimator.gameObject.GetComponent<Runner>();
        runner.DoDrop();
        m_OwnerAnimator.gameObject.GetComponent<PlayInfo>().CountSaveOther++;

        //Stop Animation
        Debug.Log("Stop Save Animation....");

        //Lost Chopstick
        if (this.DecreaseUseCount() == 0) {
            gameObject.SetActive(false);
        }
    }
}
