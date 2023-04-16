using Photon.Pun;
using UnityEngine;
using UnityProject.Cookscape;

public class MovementRPC : MonoBehaviourPun, IPunObservable
{
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private Rigidbody rb;
    private PlayerController pc;
    private RunnerTriggerVolume rtv;
    [SerializeField] float lerpSpeed = 20f;

    PhotonView m_PhotonView;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();

        TryGetComponent<RunnerTriggerVolume>(out rtv);

        m_PhotonView = GetComponent<PhotonView>();
        m_PhotonView.ObservedComponents.Add(this);
    }

    void FixedUpdate()
    {
        if (!m_PhotonView.IsMine)
        {
            float lerpValue = Time.deltaTime * lerpSpeed;

            targetRotation.Normalize();
            if ( transform.parent != null )
            {
                rb.position = targetPosition;
                rb.rotation = targetRotation;
            }
            else
            {
                rb.position = Vector3.Lerp(rb.position, targetPosition, lerpValue);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, lerpValue);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);

            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        {
            targetPosition = (Vector3)stream.ReceiveNext();
            targetRotation = (Quaternion)stream.ReceiveNext();

            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();
        }
    }
}