using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CombineSnowball : MonoBehaviour
{
    //CURRENT OBJECTS IN GAME:
    //      Snowball
    //      Rock
    //      Knife
    //      Grenade
    //      Cat

    // Scale Modifiers
    float rockScale = 36;
    float grenadeScale = 6;
    float knifeScale = 270;
    float catScale = 6;

    //VR Hands
    public GameObject leftHand;
    public GameObject rightHand;



    //Distance required between the hands for combining
    public float handsTogetherDistance;

    //Snowball Prefab
    public GameObject snowballPrefab;
    
    //Check if the two objects being held should be combined
    private void Update()
    {
        //Check if both hands are holding something
        if (leftHand.GetComponent<NewtonVR.NVRHand>().IsInteracting && rightHand.GetComponent<NewtonVR.NVRHand>().IsInteracting)
        {
            //Ups the distance necessary to combine snowballs so that larger snowballs can be made
            //Left Hand's snowball is larger
            if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x >= rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x)
            {
                if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Rock")
                {
                    handsTogetherDistance = (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x / rockScale);
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Knife")
                {
                    handsTogetherDistance = (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x / knifeScale);
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Grenade")
                {
                    handsTogetherDistance = (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x / grenadeScale);
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Cat")
                {
                    handsTogetherDistance = (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x / catScale);
                }
                else
                {
                    handsTogetherDistance = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x;
                }
            }
            //Right Hand's snowball is larger
            else
            {
                if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Rock")
                {
                    handsTogetherDistance = (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x / rockScale);
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Knife")
                {
                    handsTogetherDistance = (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x / knifeScale);
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Grenade")
                {
                    handsTogetherDistance = (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x / grenadeScale);
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Cat")
                {
                    handsTogetherDistance = (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x / catScale);
                }
                else
                {
                    handsTogetherDistance = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x;
                }
            }

            //Check distance between hands
            if (Vector3.Distance(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.position,
                     rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.position)
                     <= handsTogetherDistance)
            {
                //L-Snowball, R-Snowball
                if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left snowball to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale;
                    //Adds mass
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Rigidbody>().mass +=
                        leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Rigidbody>().mass;

                    //Change tag of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's snowball
                    //PhotonNetwork.Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));
                }

                //L-Rock, R-Snowball
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Rock" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left rock to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / rockScale);
                    //Adds mass
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Rigidbody>().mass +=
                        leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Rigidbody>().mass;

                    //Change tag of left Hand's rock
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's rock
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsRock = true;
                }
                //L-Snowball, R-Rock
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Rock")
                {
                    //Adds right rock to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / rockScale);
                    //Adds mass
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Rigidbody>().mass +=
                        rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Rigidbody>().mass;

                    //Change tag of right Hand's rock
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's rock
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsRock = true;
                }

                //L-Knife, R-Snowball
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Knife" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left knife to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / knifeScale);

                    //Change tag of left hand's knife
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's knife
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsKnife = true;
                }
                //L-Snowball, R-Knife
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Knife")
                {
                    //Adds right knife to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / knifeScale);

                    //Change tag of right hand's knife
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's knife
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsKnife = true;
                }

                //L-Grenade, R-Snowball
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Grenade" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left grenade to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / grenadeScale);

                    //Change tag of left hand's grenade
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's grenade
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsGrenade = true;
                }
                //L-Snowball, R-Grenade
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Grenade")
                {
                    //Adds right grenade to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / grenadeScale);

                    //Change tag of right hand's grenade
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's grenade
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsGrenade = true;
                }

                //L-Cat, R-Snowball
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Cat" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left cat to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / catScale);

                    //Change tag of left hand's cat
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's cat
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsCat = true;
                }
                //L-Snowball, R-Cat
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Cat")
                {
                    //Adds right cat to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / catScale);

                    //Change tag of right hand's cat
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's cat
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsCat = true;
                }

                //Check for other objects being held here
                // --- HERE ---
            }
        }
    }
}
