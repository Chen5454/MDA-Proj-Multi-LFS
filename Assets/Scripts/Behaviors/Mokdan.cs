using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Mokdan : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PhotonView => gameObject.GetPhotonView();

    [SerializeField] public bool isMainMokdan;

    private void Start()
    {
        PhotonView.ObservedComponents.Add(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isMainMokdan);
        }
        else
        {
            isMainMokdan = (bool)stream.ReceiveNext();
        }
    }
}
