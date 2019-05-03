using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Snowblower : MonoBehaviour {

    float scaleNeeded;

    public GameObject rock;
    public GameObject knife;
    public GameObject grenade;
    public GameObject cat;

    int delay;

    int maxNumberOfItems = 4;

    public GameObject spawnPoint;

    bool closed = true;

	// Sets the necessary scale to change a snowball into a random item
	void Start ()
    {
        //scaleNeeded = 0.25f;
        scaleNeeded = 0.1f;
        delay = 0;
        GameObject.Find("Handle").GetComponent<NewtonVR.NVRLever>().enabled = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        closed = GameObject.Find("Lock1").GetComponent<LockScript>().isTopClosed;
        // Delay timer to keep multiple items from being created
        if (delay != 0)
        {
            delay--;
        }

        //Deletes snowballs that should be deleted
        PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));
    }

    // Checks if the scale of the snowball on it is scaleNeeded
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Snowball")
        {
            if (delay == 0)
            {
                if (closed)
                {
                    if (other.transform.localScale.x >= scaleNeeded)
                    {
                        other.tag = "DeleteSnowball";
                        int randomItem = Random.Range(0, (maxNumberOfItems - 1));

                        //Rock
                        if (randomItem == 0)
                        {
                            GameObject rockObject = (GameObject)PhotonNetwork.Instantiate(rock.name, spawnPoint.transform.position, Quaternion.identity);
                            rockObject.tag = "Rock";
                        }
                        //Knife
                        else if (randomItem == 1)
                        {
                            GameObject knifeObject = (GameObject)PhotonNetwork.Instantiate(knife.name, spawnPoint.transform.position, Quaternion.identity);
                            knifeObject.tag = "Knife";
                        }
                        //Grenade
                        else if (randomItem == 2)
                        {
                            GameObject grenadeObject = (GameObject)PhotonNetwork.Instantiate(grenade.name, spawnPoint.transform.position, Quaternion.identity);
                            grenadeObject.tag = "Grenade";
                        }
                        //Cat
                        else if (randomItem == 3)
                        {
                            GameObject catObject = (GameObject)PhotonNetwork.Instantiate(cat.name, spawnPoint.transform.position, Quaternion.identity);
                            catObject.tag = "Cat";
                        }

                        delay = 10;
                    }
                }
            }
        }
    }
}