using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SnowballGeneral : MonoBehaviour {

    public bool isCombined;
    public bool shouldDestroy; // checks when the snowball should be destroyed when it's thrown or dropped
    GameObject leftHand;
    GameObject rightHand;

    // Bools to check what items are in this snowball
    public bool containsRock;
    public bool containsKnife;
    public bool containsGrenade;
    public bool containsCat;
    public GameObject rockPrefab;
    public GameObject knifePrefab;
    public GameObject grenadePrefab;
    public GameObject catPrefab;

    int delay;
	
	void Start () {
        isCombined = false;
        shouldDestroy = false;
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");

        // Bools for items
        containsRock = false;
        containsKnife = false;
        containsGrenade = false;
        containsCat = false;

        delay = 0;
	}

    private void Update()
    {
        if (GetComponent<NewtonVR.NVRInteractableItem>().AttachedHands.Count > 0)
        {
            shouldDestroy = true;
        }

        // Delay timer to keep multiple items from being created
        if (delay != 0)
        {
            delay--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if both hands aren't holding snowballs, we want the snowball to be able to be destroyed once it collides with something
        // if both hands are holding snowballs, deletion of the objects will be handled by the CombineSnowball script
        if (!leftHand.GetComponent<NewtonVR.NVRHand>().IsInteracting && !rightHand.GetComponent<NewtonVR.NVRHand>().IsInteracting)

            // check if the snowball is set to be destroyed
            if (shouldDestroy)
            {
                // if it collides with anything that isn't a hand or another snowball, destroy it
                if (collision.gameObject.tag != "Snowball" || collision.gameObject.tag != "LeftHand" || collision.gameObject.tag != "RightHand")
                {
                    // if the delay is 0
                    if (delay == 0)
                    {
                        // check the velocity to determine when it should be destroyed. mainly to get around issue of picking up and dropping snowballs
                        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > 2.0f)
                        {
                            if (containsRock == true)
                            {
                                GameObject rock = (GameObject)PhotonNetwork.Instantiate(rockPrefab.name, gameObject.transform.position, Quaternion.identity);
                                rock.tag = "Rock";
                            }
                            if (containsKnife == true)
                            {
                                GameObject knife = (GameObject)PhotonNetwork.Instantiate(knifePrefab.name, gameObject.transform.position, Quaternion.identity);
                                knife.tag = "Knife";
                            }
                            if (containsGrenade == true)
                            {
                                GameObject grenade = (GameObject)PhotonNetwork.Instantiate(grenadePrefab.name, gameObject.transform.position, Quaternion.identity);
                                grenade.tag = "Grenade";

                                ExplodeGrenade(grenade);
                            }
                            if (containsCat == true)
                            {
                                GameObject cat = (GameObject)PhotonNetwork.Instantiate(catPrefab.name, gameObject.transform.position, Quaternion.identity);
                                cat.tag = "Cat";
                            }

                            delay = 10;
                            PhotonNetwork.Destroy(gameObject);
                        }
                    }
                }
            }
    }

    void ExplodeGrenade(GameObject grenade)
    {
        grenade.GetComponent<SphereCollider>().radius *= 50.0f;
        PhotonNetwork.Destroy(grenade);
    }
}
