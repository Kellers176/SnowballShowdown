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
    public GameObject stick;
    public GameObject acorn;
    public GameObject duck;
    public GameObject dynamite;

    AudioObjectScript soundEffects;

    int delay;

    int maxNumberOfItems = 8;

    public GameObject spawnPoint;

    private bool closed = true;
    public bool enableRandom = true;
    public bool triggerObject = true;

	// Sets the necessary scale to change a snowball into a random item
	void Start ()
    {
        //scaleNeeded = 0.25f;
        scaleNeeded = 0.1f;
        delay = 0;

        //GameObject.Find("Handle").GetComponent<NewtonVR.NVRLever>().enabled = true;

        soundEffects = GameObject.Find("SoundEffects").GetComponent<AudioObjectScript>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        closed = true;
        // Delay timer to keep multiple items from being created
        if (delay != 0)
        {
            delay--;
        }

        if (triggerObject)
        {
            triggerObject = false;
            Vector3 snowballValues = new Vector3(-1, -1, 0);
            string name = gameObject.GetComponent<SnowballItemTiers>().CheckNewObject((int)snowballValues.x, (int)snowballValues.y, (int)snowballValues.z).name;
            GameObject newObject = (GameObject)PhotonNetwork.Instantiate(name, spawnPoint.transform.position, Quaternion.identity);

            VFXManagerScript.instance.PlayParticles(2, newObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
            newObject.tag = name;
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
                        soundEffects.PlaySnowblowerSound(gameObject.transform.position);
                        soundEffects.StopFlyInAirSound();

                        // If random is enabled, output a totally random item
                        if (enableRandom)
                        {
                            int randomItem = Random.Range(0, (maxNumberOfItems - 1));

                            //Rock
                            if (randomItem == 0)
                            {
                                GameObject rockObject = (GameObject)PhotonNetwork.Instantiate(rock.name, spawnPoint.transform.position, Quaternion.identity);
                                VFXManagerScript.instance.PlayParticles(2, rockObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                                rockObject.tag = "Rock";
                            }
                            //Knife
                            else if (randomItem == 1)
                            {
                                GameObject knifeObject = (GameObject)PhotonNetwork.Instantiate(knife.name, spawnPoint.transform.position, Quaternion.identity);
                                VFXManagerScript.instance.PlayParticles(2, knifeObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                                knifeObject.tag = "Knife";
                            }
                            //Grenade
                            else if (randomItem == 2)
                            {
                                GameObject grenadeObject = (GameObject)PhotonNetwork.Instantiate(grenade.name, spawnPoint.transform.position, Quaternion.identity);
                                VFXManagerScript.instance.PlayParticles(2, grenadeObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                                grenadeObject.tag = "Grenade";
                            }
                            //Cat
                            else if (randomItem == 3)
                            {
                                GameObject catObject = (GameObject)PhotonNetwork.Instantiate(cat.name, spawnPoint.transform.position, Quaternion.identity);
                                VFXManagerScript.instance.PlayParticles(2, catObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                                catObject.tag = "Cat";
                            }
                            //stick
                            else if (randomItem == 4)
                            {
                                GameObject stickObject = (GameObject)PhotonNetwork.Instantiate(stick.name, spawnPoint.transform.position, Quaternion.identity);
                                VFXManagerScript.instance.PlayParticles(2, stickObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                                stickObject.tag = "Stick";
                            }
                            else if (randomItem == 5)
                            {
                                GameObject acornObject = (GameObject)PhotonNetwork.Instantiate(acorn.name, spawnPoint.transform.position, Quaternion.identity);
                                VFXManagerScript.instance.PlayParticles(2, acornObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                                acornObject.tag = "Acorn";
                            }
                            else if (randomItem == 6)
                            {
                                GameObject duckObject = (GameObject)PhotonNetwork.Instantiate(duck.name, spawnPoint.transform.position, Quaternion.identity);
                                VFXManagerScript.instance.PlayParticles(2, duckObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                                duckObject.tag = "Duck";
                            }
                            else if (randomItem == 7)
                            {
                                GameObject dynamiteObject = (GameObject)PhotonNetwork.Instantiate(dynamite.name, spawnPoint.transform.position, Quaternion.identity);
                                VFXManagerScript.instance.PlayParticles(2, dynamiteObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                                dynamiteObject.tag = "Dynamite";
                            }
                        }

                        // If random is disable, output an item based on the tier list
                        else
                        {
                            // Basic linking system
                            Vector3 snowballValues = other.GetComponent<SnowballGeneral>().combineValues;
                            string name = gameObject.GetComponent<SnowballItemTiers>().CheckNewObject((int)snowballValues.x, (int)snowballValues.y, (int)snowballValues.z).name;
                            GameObject newObject = (GameObject)PhotonNetwork.Instantiate(name, spawnPoint.transform.position, Quaternion.identity);

                            VFXManagerScript.instance.PlayParticles(2, newObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                            newObject.tag = name;
                        }
                        delay = 10;
                    }
                }
            }
        }
    }
}
