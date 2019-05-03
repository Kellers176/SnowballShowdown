using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangePosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(PhotonNetwork.LocalPlayer.ActorNumber %2 == 0)
        {
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            this.gameObject.transform.position = new Vector3(0.2f, 0.29f, -21.69f);
            Debug.Log("Player 2");
        }
        if(PhotonNetwork.LocalPlayer.ActorNumber %2 == 1)
        {
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            this.gameObject.transform.position = new Vector3(0.2f, 0.417f, -11.17f);
            Debug.Log("Player 1");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
