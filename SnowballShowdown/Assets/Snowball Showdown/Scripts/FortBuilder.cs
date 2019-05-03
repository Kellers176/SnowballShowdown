using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FortBuilder : MonoBehaviour {

    float scaleNeeded;

    public GameObject fortCube;

    int delay;

	// Sets the necessary scale to change a snowball into a fort block
	void Start () {
        scaleNeeded = 0.35f;
        delay = 0;
    }

    // Update
    private void Update()
    {
        // Delay timer to keep multiple blocks from being created
        if (delay != 0)
        {
            delay--;
        }

        //Deletes snowballs that should be deleted
        //PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));
    }

    // Checks if the scale of the snowball on it is scaleNeeded
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Snowball")
        {
            if (delay == 0)
            {
                if (other.transform.localScale.x >= scaleNeeded)
                {
                    other.tag = "DeleteSnowball";
                    GameObject fortBlock = (GameObject)PhotonNetwork.Instantiate(fortCube.name, other.transform.position, Quaternion.identity);
                    delay = 10;
                }
            }
        }
    }
}
