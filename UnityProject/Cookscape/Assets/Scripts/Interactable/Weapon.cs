using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Weapon : Item
{

    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    [SerializeField] private float AttackWaitMotionTime = 0.3f;
    [SerializeField] private float AttackingMotionTime = 0.5f;

    Animator m_OwnerAnimator;

    public override void Use(GameObject gameObject = null)
    {
        m_PhotonView.RPC(nameof(HitBoxToggle), RpcTarget.All);
    }

    IEnumerator Swing()
    {
        m_OwnerAnimator = GetComponentInParent<Animator>();
        m_OwnerAnimator.SetBool("IsAttack", true);

        yield return new WaitForSeconds(AttackWaitMotionTime);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(AttackingMotionTime);
        meleeArea.enabled = false;
        trailEffect.enabled = false;

        yield break;
    }

    //Use() 메인 루틴 => Swing() 서브루틴 => Use() 다음줄...;
    //Use() 메인루틴 + Swing() 코루틴 -> 즉 비동기

    [PunRPC]
    void HitBoxToggle()
    {
        StopCoroutine(nameof(Swing));
        StartCoroutine(nameof(Swing));
    }
}
