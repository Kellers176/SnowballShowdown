using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedHealth : MonoBehaviour {

    // Networked variables
    public float nHP, nOtherHP;

	// Use this for initialization
	void Start () {
        nHP = 100.0f;
        nOtherHP = 100.0f;
	}
	
	// Update is called once per frame
	void Update () {
        nHP = GameObject.Find("NVRPlayer").GetComponent<Photon.Pun.Demo.PunBasics.PlayerManager>().Health;
        nOtherHP = GameObject.Find("NVRPlayer").GetComponent<Photon.Pun.Demo.PunBasics.PlayerManager>().otherPlayerHealth;
	}
}
