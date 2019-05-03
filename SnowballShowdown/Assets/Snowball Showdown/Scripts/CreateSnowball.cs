using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CreateSnowball : MonoBehaviour
{

    // hand objects
    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject leftParticleEffect;
    public GameObject rightParticleEffect;

    private bool left, right;

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

    // the distance necessary to reset the scooping when in collider
    float handsResetDistance;

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

    AudioObjectScript soundEffects;

    // initialize some stuff
    void Awake()
    {
        startDistance = 0.0f;
        handsTogetherDistance = 0.2f;
        handsResetDistance = 0.35f;
        snowballScalar = 0.0f;
    }

    private void Start()
    {
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
        soundEffects = GameObject.Find("SoundEffects").GetComponent<AudioObjectScript>();
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

            GameObject.Find("LeftHand").GetComponent<TrailRenderer>().enabled = true;
            GameObject.Find("RightHand").GetComponent<TrailRenderer>().enabled = true;
            if (!left)
            {
                leftParticleEffect = VFXManagerScript.instance.ParentParticle(3, leftHand, leftHand.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                left = true;
            }
            if (!right)
            {
                rightParticleEffect = VFXManagerScript.instance.ParentParticle(3, rightHand, rightHand.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
                right = true;
            }

            soundEffects.PlayScoopSound(leftHand.transform.position);
        }
        else
        {
            soundEffects.StopScoopSound();
        }
    }

    // while the player's hands remain in the snow, check if they
    // put their hands together.
    // if they do, create a snowball and prevent the loop from running (or you get infinite snowballs)
    void OnTriggerStay(Collider hand)
    {
        if (leftInSnow && rightInSnow)
        {
            // Increases size of startDistance if hands are moved futher apart
            // Checks if hands are farther apart then when they entered the snowbank collider
            if (Vector3.Distance(leftHand.transform.position, rightHand.transform.position) > startDistance)
            {
                // Changes startDistance to the new max start distance
                startDistance = Vector3.Distance(leftHand.transform.position, rightHand.transform.position);
                snowballScalar = startDistance;
            }
            //  Makes the snowball if hands are close enough together
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

                // Create a particle effect to show that the player has built a snowball
                VFXManagerScript.instance.PlayParticles(2, snowball.transform.position, new Vector3(1.0f, 1.0f, 1.0f));

                // Pause scooping sound
               // AudioManager.instance.StopSound(scoopSounds, true, true);
            }
        }
        // Resets hasMadeSnowball if hands are far enough apart
        // If a snowball has been made while in the snowbank collider
        if (hasMadeSnowball)
        {
            // Checks if the hands are far enough apart from each other to scoop again safely
            if (Vector3.Distance(leftHand.transform.position, rightHand.transform.position) >= handsResetDistance)
            {
                // Resets all variables
                hasMadeSnowball = false;
                leftInSnow = true;
                rightInSnow = true;

                // StartDistance is the new distance apart
                startDistance = Vector3.Distance(leftHand.transform.position, rightHand.transform.position);
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

        if (leftParticleEffect != null && rightParticleEffect != null)
        {
            VFXManagerScript.instance.DestroyParticle(leftParticleEffect);
            VFXManagerScript.instance.DestroyParticle(rightParticleEffect);
        }

        //left = right = false;

        hasMadeSnowball = false;
        GameObject.Find("LeftHand").GetComponent<TrailRenderer>().enabled = false;
        GameObject.Find("RightHand").GetComponent<TrailRenderer>().enabled = false;
    }

}
