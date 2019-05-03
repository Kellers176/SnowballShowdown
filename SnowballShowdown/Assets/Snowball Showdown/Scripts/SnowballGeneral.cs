using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SnowballGeneral : MonoBehaviour {

    public bool isCombined;
    public bool shouldDestroy; // checks when the snowball should be destroyed when it's thrown or dropped
    GameObject leftHand;
    GameObject rightHand;

    AudioObjectScript soundEffects;

    // Bools to check what items are in this snowball
    public bool containsRock;
    public bool containsKnife;
    public bool containsGrenade;
    public bool containsCat;
    public bool containsStick;
    public bool containsAcorn;
    public bool containsDuck;
    public bool containsDynamite;
    public GameObject rockPrefab;
    public GameObject knifePrefab;
    public GameObject grenadePrefab;
    public GameObject catPrefab;
    public GameObject stickPrefab;
    public GameObject acornPrefab;
    public GameObject duckPrefab;
    public GameObject dynamitePrefab;
    public GameObject explosion;
    public bool isExploding = false;
    public GameObject grenade1;
    const float MAX_SIZE = 0.7f;
    int delay;

    [Header("Tier List Values")]
    // Values checked when a snowball goes into the snowblower
    public Vector3 combineValues = new Vector3(-1, -1, 0); // Line num; placement in line num; contained items
	
	void Start () {
        isCombined = false;
        shouldDestroy = false;
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");

        soundEffects = GameObject.Find("SoundEffects").GetComponent<AudioObjectScript>();

        // Bools for items
        containsRock = false;
        containsKnife = false;
        containsGrenade = false;
        containsCat = false;
        containsStick = false;
        containsDuck = false;

        // Values for combining
        combineValues = new Vector3(-1, -1, 0);

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

        if(this.gameObject.transform.localScale.x >= MAX_SIZE)
        {
            this.gameObject.transform.localScale = new Vector3(MAX_SIZE, MAX_SIZE, MAX_SIZE);
        }
        /*if(isExploding)
        {
            grenade1.GetComponent<SphereCollider>().radius *= 30.0f;
            //Debug.Log("SPHERE COLLIDER:" + grenade1.GetComponent<SphereCollider>().radius);
        }*/

       
    }

    public void PreMagnetGrabSound()
    {
        soundEffects.PlayMagnetGrabSound(transform.position);
    }

    public void PostThrowSound()
    {
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > 2.0f)
        {
            soundEffects.PlayThrowSound(transform.position);
            soundEffects.PlayFlyInAirSound(transform.position);
        }
        else
        {
            soundEffects.StopFlyInAirSound();
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
                        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1.0f)
                        {
                            if (containsRock == true)
                            {
                                GameObject rock = (GameObject)PhotonNetwork.Instantiate(rockPrefab.name, gameObject.transform.position, Quaternion.identity);
                                rock.tag = "Rock";
                                soundEffects.PlayRockSound(gameObject.transform.position);
                            }
                            if (containsKnife == true)
                            {
                                GameObject knife = (GameObject)PhotonNetwork.Instantiate(knifePrefab.name, gameObject.transform.position, Quaternion.identity);
                                knife.tag = "Knife";
                                soundEffects.PlaySwordSound(gameObject.transform.position);
                            }
                            if (containsGrenade == true)
                            {
                                //GameObject grenade = (GameObject)PhotonNetwork.Instantiate(grenadePrefab.name, gameObject.transform.position, Quaternion.identity);
                                //grenade.tag = "Grenade";

                                ExplodeGrenade(this.gameObject);
                                soundEffects.PlayGrenadeSound(gameObject.transform.position);
                            }
                            if (containsCat == true)
                            {
                                GameObject cat = (GameObject)PhotonNetwork.Instantiate(catPrefab.name, gameObject.transform.position, Quaternion.identity);
                                cat.tag = "Cat";
                                soundEffects.PlayCatSound(gameObject.transform.position);
                            }
                            if (containsStick == true)
                            {
                                GameObject stick = (GameObject)PhotonNetwork.Instantiate(stickPrefab.name, gameObject.transform.position, Quaternion.identity);
                                stick.tag = "Stick";
                                soundEffects.PlayStickSound(gameObject.transform.position);
                            }
                            if (containsAcorn == true)
                            {
                                GameObject acorn = (GameObject)PhotonNetwork.Instantiate(acornPrefab.name, gameObject.transform.position, Quaternion.identity);
                                acorn.tag = "Acorn";
                                soundEffects.PlayAcornSound(gameObject.transform.position);
                            }
                            if (containsDuck == true)
                            {
                                GameObject duck = (GameObject)PhotonNetwork.Instantiate(duckPrefab.name, gameObject.transform.position, Quaternion.identity);
                                duck.tag = "Duck";
                                soundEffects.PlayDuckSound(gameObject.transform.position);
                            }
                            if (containsDynamite == true)
                            {
                                GameObject dynamite = (GameObject)PhotonNetwork.Instantiate(dynamitePrefab.name, gameObject.transform.position, Quaternion.identity);
                                dynamite.tag = "Dynamite";
                                soundEffects.PlayDynamiteSound(gameObject.transform.position);
                            }
                            delay = 10;
                            //Debug.Log("===================================================================");
                            //Debug.Log("In script");
                            VFXManagerScript.instance.PlayParticles(1, gameObject.transform.position, gameObject.transform.localScale * 1.5f);
                            soundEffects.PlayHitSound(transform.position);
                            soundEffects.StopFlyInAirSound();
                            gameObject.GetComponent<MeshRenderer>().enabled = false;
                            //StartCoroutine(Wait(gameObject));
                            PhotonNetwork.Destroy(gameObject);
                        }
                    }
                }
            }
    }
    IEnumerator Wait(GameObject myGameObject)
    {
        //Debug.Log("Got into Wait");
        yield return new WaitForSeconds(0.2f);
        PhotonNetwork.Destroy(myGameObject);
        //Debug.Log("Leaving Wait");
    }
    void ExplodeGrenade(GameObject grenade)
    {
        grenade.GetComponent<SphereCollider>().radius = 10.0f;
        //Debug.Log("SPHERE COLLIDER:" + grenade.GetComponent<SphereCollider>().radius);

        VFXManagerScript.instance.PlayParticles(3, gameObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
        //Instantiate(explosion, grenade.transform.position, Quaternion.identity);
        //PhotonNetwork.Destroy(grenade);
    }
}
