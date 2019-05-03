using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonTransferOwner : MonoBehaviour
{
    public void TransferOwnership()
    {
        Debug.Log("Transfer Ownership");
        gameObject.GetComponent<Photon.Pun.PhotonView>().TransferOwnership(Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);
    }
}