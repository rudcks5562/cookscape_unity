using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityProject.Cookscape;

public class KitchenTowel : Item
{
    public float m_DryingTime;

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
        ItemData itemData = GameManager.instance.item[nameof(ItemData.ITEM.키친타올)];
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
        m_PhotonView.RPC(nameof(DryBody), RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
    }
    #endregion

    #region RPC
    [PunRPC]
    void DryBody(int targetViewID)
    {
        m_TargetViewID = targetViewID;
        
        GameObject me = PhotonView.Find(m_TargetViewID).gameObject;
        StopCoroutine(nameof(Dry));
        StartCoroutine(nameof(Dry), me);
    }
    #endregion

    #region coroutine
    IEnumerator Dry(GameObject me)
    {
        // SET WET STATE, FALSE
        PlayerController pc = me.GetComponent<PlayerController>();
        m_OwnerAnimator = GetComponentInParent<Animator>();

        //닦는 애니메이션 시작
        m_OwnerAnimator.SetBool("IsDrying", true);

        yield return new WaitForSeconds(6f);
        pc.SetWetState(false);

        //닦는 애니메이션 끝
        m_OwnerAnimator.SetBool("IsDrying", false);

        //record this
        m_OwnerAnimator.gameObject.GetComponent<PlayInfo>().CountUseTowel++;

        if (this.DecreaseUseCount() == 0) {
            Destroy(gameObject);
        }
    }
    #endregion
}
