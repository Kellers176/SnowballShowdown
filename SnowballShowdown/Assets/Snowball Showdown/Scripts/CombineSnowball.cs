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
    float rockScale = 35f;
    float grenadeScale = 5f;
    float knifeScale = 269f;
    float catScale = 5f;
    float stickScale = 5f;
    float acornScale = 5f;
    float duckScale = 5f;
    float dynamiteScale = 5f;

    //VR Hands
    public GameObject leftHand;
    public GameObject rightHand;

    //Distance required between the hands for combining
    public float handsTogetherDistance;

    //Snowball Prefab
    public GameObject snowballPrefab;

    private float GetBaseScale(GameObject hand)
    {
        float totalVal = 0.0f;

        totalVal += hand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Collider>().transform.localScale.x;
        totalVal += hand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Collider>().transform.localScale.y;
        totalVal += hand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Collider>().transform.localScale.z;
        totalVal /= 3f;
        return totalVal;
    }

    float comMul = 1.9f;

    //Check if the two objects being held should be combined
    private void Update()
    {

        PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));
        //Check if both hands are holding something
        if (leftHand.GetComponent<NewtonVR.NVRHand>().IsInteracting && rightHand.GetComponent<NewtonVR.NVRHand>().IsInteracting)
        {
            //Ups the distance necessary to combine snowballs so that larger snowballs can be made
            //Left Hand's snowball is larger
            if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x >= rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x)
            {
                if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Rock")
                {
                    handsTogetherDistance = (GetBaseScale(leftHand) / rockScale);
                    handsTogetherDistance *= comMul;
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Knife")
                {
                    handsTogetherDistance = (GetBaseScale(leftHand) / knifeScale);
                    handsTogetherDistance *= comMul * comMul;
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Grenade")
                {
                    handsTogetherDistance = (GetBaseScale(leftHand) / grenadeScale);
                    handsTogetherDistance *= comMul;
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Cat")
                {
                    handsTogetherDistance = (GetBaseScale(leftHand) / catScale);
                    handsTogetherDistance *= comMul;
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Stick")
                {
                    handsTogetherDistance = (GetBaseScale(leftHand) / stickScale);
                    handsTogetherDistance *= comMul;
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Acorn")
                {
                    handsTogetherDistance = (GetBaseScale(leftHand) / acornScale);
                    handsTogetherDistance *= comMul;
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Duck")
                {
                    handsTogetherDistance = (GetBaseScale(leftHand) / duckScale);
                    handsTogetherDistance *= comMul;
                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Dynamite")
                {
                    handsTogetherDistance = (GetBaseScale(leftHand) / dynamiteScale);
                    handsTogetherDistance *= comMul;
                }
                else
                {
                    handsTogetherDistance = GetBaseScale(leftHand);
                }
            }
            //Right Hand's snowball is larger
            else
            {
                if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Rock")
                {
                    handsTogetherDistance = (GetBaseScale(rightHand) / rockScale);
                    handsTogetherDistance *= comMul;
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Knife")
                {
                    handsTogetherDistance = (GetBaseScale(rightHand) / knifeScale);
                    handsTogetherDistance *= comMul * comMul;
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Grenade")
                {
                    handsTogetherDistance = (GetBaseScale(rightHand) / grenadeScale);
                    handsTogetherDistance *= comMul;
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Cat")
                {
                    handsTogetherDistance = (GetBaseScale(rightHand) / catScale);
                    handsTogetherDistance *= comMul;
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Stick")
                {
                    handsTogetherDistance = (GetBaseScale(rightHand) / stickScale);
                    handsTogetherDistance *= comMul;
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Acorn")
                {
                    handsTogetherDistance = (GetBaseScale(rightHand) / acornScale);
                    handsTogetherDistance *= comMul;
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Duck")
                {
                    handsTogetherDistance = (GetBaseScale(rightHand) / duckScale);
                    handsTogetherDistance *= comMul;
                }
                else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Dynamite")
                {
                    handsTogetherDistance = (GetBaseScale(rightHand) / dynamiteScale);
                    handsTogetherDistance *= comMul;
                }
                else
                {
                    handsTogetherDistance = GetBaseScale(rightHand);
                }
            }

            if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x >= 0.7f)
            {
                handsTogetherDistance *= comMul;
            }
            else if (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x >= 0.7f)
            {
                handsTogetherDistance *= comMul;
            }
            else
            {
                handsTogetherDistance *= 1.2f;
            }

            // Determines whether or not the snowball is added
            //Check distance between hands
            if (Vector3.Distance(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.position,
                 rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.position)
                 <= handsTogetherDistance)
            {
                NewtonVR.NVRInteractable heldObject = null;

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

                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

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

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of left Hand's rock
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's rock
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsRock = true;
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsRock(true);
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

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                  // leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                  //     new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                  //     rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                  //     (int)leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of right Hand's rock
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's rock
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsRock = true;
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsRock(true);
                }

                //L-Knife, R-Snowball
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Knife" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left knife to right snowball
                    //Creates proper proportions
                    Vector3 properSize = new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x,
                                                     leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x,
                                                     leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x);
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (properSize / knifeScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                  // rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                  //     new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                  //     leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                  //     (int)rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of left hand's knife
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's knife
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsKnife = true;
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsKnife(true);

                }
                //L-Snowball, R-Knife
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Knife")
                {
                    //Adds right knife to left snowball
                    //Creates proper proportions
                    Vector3 properSize = new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x,
                                                     rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x,
                                                     rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale.x);
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (properSize / knifeScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of right hand's knife
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's knife
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsKnife = true;
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsKnife(true);

                }

                //L-Grenade, R-Snowball
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Grenade" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left grenade to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / grenadeScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of left hand's grenade
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";
                    
                    //Destroys left Hand's grenade
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsGrenade = true;
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsGrenade(true);

                }
                //L-Snowball, R-Grenade
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Grenade")
                {
                    //Adds right grenade to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / grenadeScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of right hand's grenade
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's grenade
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsGrenade = true;
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsGrenade(true);

                }

                //L-Cat, R-Snowball
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Cat" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Debug.Log("---------------------------------------------------------------");
                    //Debug.Log("Combining Cat");
                    //Adds left cat to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / catScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of left hand's cat
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";
                    //Destroys left Hand's cat
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsCat = true;
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsCat(true);

                }
                //L-Snowball, R-Cat
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Cat")
                {
                    //Adds right cat to left snowball
                    //Adds size
                    //Debug.Log("---------------------------------------------------------------");
                    //Debug.Log("Combining Cat");
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / catScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                   // leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                   //     new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                   //     rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                   //     (int)leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of right hand's cat
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's cat
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsCat = true;
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsCat(true);

                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Stick" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left cat to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / stickScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of left hand's cat
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's cat
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsStick = true;
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsStick(true);

                }
                //L-Snowball, R-Cat
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Stick")
                {
                    //Adds right cat to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / stickScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of right hand's cat
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's cat
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsStick = true;
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsStick(true);

                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Acorn" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left cat to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / acornScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                   // rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                   //     new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                   //     leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                   //     (int)rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of left hand's cat
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's cat
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsAcorn = true;
                    //rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsStick(true);

                }
                //L-Snowball, R-Cat
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Acorn")
                {
                    //Adds right cat to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / acornScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);
                    //
                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of right hand's cat
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's cat
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsAcorn = true;
                    //leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsStick(true);

                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Duck" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left cat to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / duckScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                    //rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                    //    new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                    //    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                    //    (int)rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of left hand's cat
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's cat
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsDuck = true;
                    //rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsStick(true);

                }
                //L-Snowball, R-Cat
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Duck")
                {
                    //Adds right cat to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / duckScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                   //leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                   //    new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                   //    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                   //    (int)leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of right hand's cat
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's cat
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsDuck = true;
                    //leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsStick(true);

                }
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Dynamite" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
                {
                    //Adds left cat to right snowball
                    //Adds size
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / dynamiteScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                   // rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                   //     new Vector3(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                   //     leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                   //     (int)rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of left hand's cat
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys left Hand's cat
                    //Destroy(leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of right Hand's snowball
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsDynamite = true;
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsDynamite(true);

                }
                //L-Snowball, R-Cat
                else if (leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball" &&
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Dynamite")
                {
                    //Adds right cat to left snowball
                    //Adds size
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale +=
                        (rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.transform.localScale / dynamiteScale);

                    // Adds item values (Line ID, Tier Position, Increment items in snowball)
                   //leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues =
                   //    new Vector3(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetLineID(),
                   //    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<ItemDataScript>().GetTierNum(),
                   //    (int)leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().combineValues.z++);

                    // Sets the object to a specific interactable object
                    heldObject = rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting;

                    //Change tag of right hand's cat
                    rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag = "DeleteSnowball";

                    //Destroys right Hand's cat
                    //Destroy(rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting);
                    PhotonNetwork.Destroy(GameObject.FindWithTag("DeleteSnowball"));

                    //Change bool of left Hand's snowball
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<SnowballGeneral>().containsDynamite = true;
                    leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().setContainsDynamite(true);

                }
                //Check for other objects being held here
                // --- HERE ---

                // If the HeldObject exists, displays a particle effect for the fused object
                //if (heldObject != null)
                //{
                //    VFXManagerScript.instance.PlayParticles(2, heldObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                //}
            }   //
        }
    }
}
