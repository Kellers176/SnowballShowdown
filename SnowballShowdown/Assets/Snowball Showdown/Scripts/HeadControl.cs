using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadControl : MonoBehaviour
{

    bool headExists;

    // Use this for initialization
    void Start()
    {
        headExists = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Head
        GameObject head = GameObject.Find("Player(Clone)");
        // Create Player(Clone) as head, separate from NVRPlayer
        if (head != null)
        {
            Vector3 headPosition = GameObject.Find("Head").transform.position;
            Quaternion headRotation = GameObject.Find("Head").transform.rotation;
            headPosition.y -= 2.0f;
            head.transform.position = headPosition;
            head.transform.rotation = headRotation;
        }

        // Remove Red Player Head
        if (headExists)
        {
            if (head.GetComponent<Photon.Pun.PhotonView>().OwnerActorNr == Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber)
            {
                //head.transform.GetChild(0).gameObject.SetActive(false);
                head.layer = 11;
                headExists = false;
            }
        }
        //      // Right Hand
        //      GameObject rightHand = GameObject.Find("RightHand(Clone)");
        //      if (rightHand != null)
        //      {
        //          Vector3 rightHandPosition = GameObject.FindWithTag("RightHand").transform.position;
        //          Quaternion rightHandRotation = GameObject.FindWithTag("RightHand").transform.rotation;
        //          rightHand.transform.position = rightHandPosition;
        //          rightHand.transform.rotation = rightHandRotation;
        //      }
        //
        //      // Left Hand
        //      GameObject leftHand = GameObject.Find("LeftHand(Clone)");
        //      if (leftHand != null)
        //      {
        //          Vector3 leftHandPosition = GameObject.FindWithTag("LeftHand").transform.position;
        //          Quaternion leftHandRotation = GameObject.FindWithTag("LeftHand").transform.rotation;
        //          leftHand.transform.position = leftHandPosition;
        //          leftHand.transform.rotation = leftHandRotation;
        //      }
    }
}
