using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace UnityProject.Cookscape
{
    public class CatchScript : MonoBehaviourPun
    {
        [Tooltip("Chef's CatchPoint")]
        [SerializeField] Transform m_PlayerCatchPoint;

        PhotonView m_PhotonView;
        Rigidbody m_PlayerBody;
        BoxCollider m_Collider;

        int catchedManViewID;
        int catcherManViewID;
        bool m_Catchable;

        bool m_Hitted;

        private Transform catchedMan;

        bool m_BeThrowing;

        #region monobehaviors

        private void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();
            m_PlayerBody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<BoxCollider>();

            catchedManViewID = -1;
        }

        private void OnTriggerEnter(Collider other)
        {
            //Hit by weapon
            if (other.CompareTag(nameof(Catch)))
            {
                int HeatedManViewID = m_PhotonView.ViewID;
                m_PhotonView.RPC(nameof(DoStunned), RpcTarget.All, HeatedManViewID);
            }
        }

        #endregion

        public void DoCatch(GameObject _catchedMan)
        {
            //catchedMan = _catchedMan.transform;
            int me = m_PhotonView.ViewID;

            Runner _runner = _catchedMan.GetComponent<Runner>();
            if (_runner.IsPlayerEquiped()) {
                _runner.DoDrop();
            }

            int other = _catchedMan.GetComponent<PhotonView>().ViewID;
            m_PhotonView.RPC(nameof(CatchRPC), RpcTarget.All, me, other);
        }

        public PhotonView GetM_PhotonView()
        {
            return m_PhotonView;
        }

        public void Imprison(PhotonView m_PhotonView)
        {
            if (catchedManViewID == -1)
            {
                Debug.Log("not have catched foods");
                return;
            }

            m_PhotonView.RPC(nameof(ImprisonRPC), RpcTarget.All);
        }

        #region RPC

        [PunRPC]
        void CatchRPC(int me, int _catchedManViewID)
        {
            catcherManViewID = me;
            catchedManViewID = _catchedManViewID;
            catchedMan = PhotonView.Find(catchedManViewID).gameObject.transform;
            catchedMan.GetComponent<Runner>().m_IsChefTakeMe = true;
            StopCoroutine(nameof(Catch));
            _ = StartCoroutine(nameof(Catch), me);
        }

        [PunRPC]
        void ImprisonRPC()
        {
            m_PlayerCatchPoint.transform.DetachChildren();
            PhysicSystemToggle(catchedManViewID, true);

            if ( !m_BeThrowing )
            {
                m_BeThrowing = true;
                
                StopCoroutine(nameof(ThrowCatchee));
                _ = StartCoroutine(nameof(ThrowCatchee), catchedMan.GetComponent<Rigidbody>());
            }
        }

        [PunRPC]
        void DoStunned(int HeatedManViewID)
        {
            if (GameManager.instance.m_IsChef)
            {
                GetComponent<PlayInfo>().CountHitOther++;
            }

            //Not ME
            if (HeatedManViewID != m_PhotonView.ViewID)
                return;

            
            //StopCoroutine(nameof(Stunned));
            StartCoroutine(nameof(Stunned));
        }

        #endregion

        #region coroutine
        IEnumerator ThrowCatchee(Rigidbody rigidbody)
        {
            if ( m_PhotonView.ViewID != catcherManViewID )
            {
                yield break;
            }

            // CONSTRAINTS ON ALL
            m_PlayerBody.constraints = RigidbodyConstraints.FreezeAll;

            // THROW EQUIPMENT
            float throwPower = 0.3f;
            Vector3 throwAngle = transform.forward * 10f;
            throwAngle.y = 5f;
            rigidbody.AddForce(throwAngle * throwPower, ForceMode.Impulse);

            //return physics
            PhysicSystemToggle(catchedManViewID, true);
            rigidbody.gameObject.GetComponent<Runner>().BeWasted();

            // CONSTRAINTS OFF
            yield return new WaitForSeconds(0.05f);
            m_PlayerBody.constraints = RigidbodyConstraints.FreezeRotation;
            m_BeThrowing = false;

            // PLAY RUNNER SINK SOUND
            rigidbody.gameObject.GetComponent<Runner>().transform.GetComponent<RunnerSinkSound>().Play();
            rigidbody.gameObject.GetComponent<Runner>().BeCaptured();
        }

        IEnumerator Catch(int me)
        {
            int nowViewID = m_PhotonView.ViewID;

            //not chef
            if ( nowViewID != me )
            {
                yield break;
            }

            //���� �丮��
            Debug.Log("Catching....");

            //�������� ���� ����ȭ
            PhysicSystemToggle(catchedManViewID, false);

            //wait for pickup animation
            yield return new WaitForSeconds(0.9f);

            //position change to my front
            catchedMan.SetParent(m_PlayerCatchPoint.transform);
            catchedMan.localPosition = new Vector3(0f, 0f, 0f);
            catchedMan.rotation = Quaternion.Euler(0f, 0f, 0f);

            // catchedMan.GetComponent<Runner>().BeCaptured();

            //end
            yield break;
        }

        IEnumerator Stunned()
        {
            Runner playerController = GetComponent<Runner>();

            //if ( playerController.m_IsStuned )
            //{
            //    yield break;
            //}

            //Already Stun;
            playerController.GetStunned();

            yield return new WaitForSeconds(0.5f);

            GetComponent<PlayInfo>().CountBeHitted++;

            int timeCount = 0;
            while (timeCount++ < 14)
            {
                if (playerController.m_IsCaptured)
                {
                    playerController.StopStunned();
                    yield break;
                }

                yield return new WaitForSeconds(1.0f);
            }
            Debug.Log("unlock");

            //if not catching...
            playerController.StopStunned();
        }

        #endregion

        #region privateMethods

        void PhysicSystemToggle(int catchedManViewID, bool isOn)
        {

            CapsuleCollider[] catchedManColliders = catchedMan.GetComponentsInChildren<CapsuleCollider>();
            Rigidbody catchedManRigidbody = catchedMan.GetComponent<Rigidbody>();

            //catched man cant move
            foreach (Collider c in catchedManColliders)
            {
                c.enabled = isOn;
            }
            catchedManRigidbody.isKinematic = !isOn;
            //catchedManRigidbody.useGravity = isOn;
        }

        #endregion
    }
}