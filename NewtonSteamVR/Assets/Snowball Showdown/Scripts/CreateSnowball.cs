using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CreateSnowball : MonoBehaviour
{

    // hand objects
    public GameObject leftHand;
    public GameObject rightHand;

    // check if either hand is inside the snow to create snowballs
    bool leftInSnow = false;
    bool rightInSnow = false;

    // when the hands first enter the snow, we need to calculate 
    // the distance between them to generate different sized
    // snowballs
    float startDistance;
    
    // we need to have a set distance in place for when the
    // hands come together
    float handsTogetherDistance;

    // the snowball object itself
    // having this allows us to scale the snowball's size and weight
    public GameObject snowballPrefab;

    // checks if the player has already created a snowball when their
    // hands are in the snow.
    // allows us to limit the number of snowballs to one per action
    bool hasMadeSnowball = false;

    // this scalar is calculated by finding the distance between 
    // the player's hands when placed in snow. we use this to scale
    // the snowball's weight and size
    float snowballScalar;

    // initialize some stuff
    void Awake()
    {
        startDistance = 0.0f;
        handsTogetherDistance = 0.2f;
        snowballScalar = 0.0f;
    }

    private void Start()
    {
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
    }

    // when hands enter the snow, calculate their distance to find
    // how big the snowball will be
    void OnTriggerEnter(Collider hand)
    {
        if (hand.gameObject.name == "LeftHand")
        {
            leftInSnow = true;
        }

        if (hand.gameObject.name == "RightHand")
        {
           rightInSnow = true;
        }

        // if both hands are in the snow, calculate the distance
        // we haven't made a snowball yet, since we just placed our
        // hands in snow

        if (leftInSnow && rightInSnow)
        {
            startDistance = Vector3.Distance(leftHand.transform.position, rightHand.transform.position);
            snowballScalar = startDistance;
            hasMadeSnowball = false;
        }
    }

    // while the player's hands remain in the snow, check if they
    // put their hands together.
    // if they do, create a snowball and prevent the loop from running (or you get infinite snowballs)
    void OnTriggerStay(Collider hand)
    {
        if (leftInSnow && rightInSnow)
        {
            if (hasMadeSnowball == false && Vector3.Distance(leftHand.transform.position, rightHand.transform.position) <= handsTogetherDistance)
            {
                // find the center point between the hands. this allows us to spawn the snowballs
                // right in front of the player
                Vector3 centerBetweenHands = leftHand.transform.position + ((rightHand.transform.position - leftHand.transform.position) * 0.5f);

                // create the snowball and scale it appropriately
                //GameObject snowball = (GameObject)Instantiate(snowballPrefab, centerBetweenHands, Quaternion.identity);
                GameObject snowball = (GameObject)PhotonNetwork.Instantiate("Snowball", centerBetweenHands, Quaternion.identity);
                snowball.tag = "Snowball";
                snowball.transform.localScale *= snowballScalar;
                snowball.GetComponent<Rigidbody>().mass *= 1 + snowballScalar;

                hasMadeSnowball = true;
               leftInSnow = false;
               rightInSnow = false;
            }
        }
    }

    // when the player remove their hands from the snow, just reset values
    void OnTriggerExit(Collider hand)
    {
        if (hand.gameObject.name == "LeftHand")
        {
            leftInSnow = false;
        }

        if (hand.gameObject.name == "RightHand")
        {
            rightInSnow = false;
        }

        hasMadeSnowball = false;

    }

}
