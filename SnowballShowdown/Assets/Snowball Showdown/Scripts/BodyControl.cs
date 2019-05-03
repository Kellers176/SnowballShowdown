using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyControl : MonoBehaviour {

    bool bodyExists;

	// Use this for initialization
	void Start () {
        bodyExists = true;
	}
	
	// Update is called once per frame
	void Update () {
        // Body
        GameObject body = GameObject.Find("Body(Clone)");

        if (body != null)
        {
            Vector3 bodyPosition = GameObject.Find("Head").transform.position;
            Quaternion bodyRotation = GameObject.Find("Player(Clone)").transform.rotation;

            bodyPosition.y -= 2.25f;

            body.transform.position = bodyPosition;
            body.transform.rotation = bodyRotation;
        }

        if (bodyExists)
        {
            if (body.GetComponent<Photon.Pun.PhotonView>().OwnerActorNr == Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber)
            {
                //body.transform.GetChild(0).gameObject.SetActive(false);
                body.layer = 11;
                bodyExists = false;
            }
        }
	}
}
