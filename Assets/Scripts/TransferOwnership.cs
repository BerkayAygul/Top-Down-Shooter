using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TransferOwnership : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        /*if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.TransferOwnership(newMasterClient);
        }*/
        OnOwnershipRequest(photonView, newMasterClient);
    }

    // This method will call on any object which utilizes the ownership callbacks. So it is important to make sure the target view matches up with the view on this object.
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if(targetView != base.photonView)
        {
            return;
        }

        base.photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != base.photonView)
        {
            return;
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        if (targetView != base.photonView)
        {
            return;
        }
    }
}
