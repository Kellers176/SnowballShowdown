using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyParticleScirpt : MonoBehaviour {

	
	void Start ()
    {
        WaitFiveSeconds();
	}
	
	IEnumerator WaitFiveSeconds()
    {
        yield return new WaitForSeconds(5.0f);
        PhotonNetwork.Destroy(gameObject);
    }
}
