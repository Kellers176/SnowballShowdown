using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHeadCreate : MonoBehaviour {

    Vector3 headPosition;
	// Use this for initialization
	void Start () {
        //headPosition = GameObject.Find("Head").transform.position;//GameObject.FindWithTag("MainCamera").transform.position;
        //headPosition.y -= 1.5f;
        //GameObject playerHead = (GameObject)PhotonNetwork.Instantiate("Headball", headPosition, Quaternion.identity, 0);
        //playerHead.tag = "Head";
        //playerHead.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        GameObject head = GameObject.Find("Headball(Clone)");
        headPosition = GameObject.Find("Head").transform.position; //GameObject.FindWithTag("MainCamera").transform.position;
        headPosition.y -= 1.5f;
        if (head == null)
        {
            GameObject playerHead = (GameObject)PhotonNetwork.Instantiate("Headball", headPosition, Quaternion.identity, 0);
            playerHead.tag = "Head";
            playerHead.SetActive(true);
        }
        else
        {
            head.transform.position = headPosition;
        }
    }
}
