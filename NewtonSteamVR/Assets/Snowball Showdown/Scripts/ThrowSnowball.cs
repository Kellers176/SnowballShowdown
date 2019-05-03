using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSnowball : MonoBehaviour {

    public GameObject snowball;
    public GameObject playerBase; // player base rotates, so we can find its orientation and use that to apply force to the snowballs

    public GameObject hand;

    [SerializeField]
    public float bonusForce;
    public float bonusForce2;
    public float bonusForce3;

    private void Awake()
    {

    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Snowball")
        {
            snowball = other.gameObject;

            if (snowball.GetComponent<OVRGrabbable>().isGrabbed == true)
            {
               // handScript.isHoldingObject = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Snowball")
        {
            //handScript.isHoldingObject = false;
        }
    }


    private void Update()
    {
        // player must first be holding a snowball
       // if (handScript.isHoldingObject)
        {
            // check for left hand
            //if (hand.name == "hand_left")
            //{
            // if the player's hand moves at a certain speed, and lets go of the snowball
            // we want the snowball to fly forward at high speeds


            if ((snowball.GetComponent<Rigidbody>().velocity.magnitude) > 2f && (snowball.GetComponent<Rigidbody>().velocity.magnitude) < 5f && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) < 0.1f)
            {
                Rigidbody snowRB = snowball.GetComponent<Rigidbody>();
                Vector3 newVec = snowRB.velocity;
                newVec.x *= bonusForce;
                newVec.y *= bonusForce;
                newVec.z *= bonusForce;
                snowRB.velocity = newVec;
                //handScript.isHoldingObject = false;
                Debug.Log(1.2);
            }

            if ((snowball.GetComponent<Rigidbody>().velocity.magnitude) > 5f && (snowball.GetComponent<Rigidbody>().velocity.magnitude) < 10f && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) < 0.1f)
            {
                // takes the velocity and multiplies it by a bonus float, then sets the velocity equal to the multiplied version
                Rigidbody snowRB = snowball.GetComponent<Rigidbody>();
                Vector3 newVec = snowRB.velocity;
                newVec.x *= bonusForce2;
                newVec.y *= bonusForce2;
                newVec.z *= bonusForce2;
                snowRB.velocity = newVec;
               // handScript.isHoldingObject = false;
                Debug.Log(1.3);
            }
            if ((snowball.GetComponent<Rigidbody>().velocity.magnitude) > 10f && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) < 0.1f)
            {
                // takes the velocity and multiplies it by a bonus float, then sets the velocity equal to the multiplied version
                Rigidbody snowRB = snowball.GetComponent<Rigidbody>();
                Vector3 newVec = snowRB.velocity;
                newVec.x *= bonusForce3;
                newVec.y *= bonusForce3;
                newVec.z *= bonusForce3;
                snowRB.velocity = newVec;
                //handScript.isHoldingObject = false;
                Debug.Log(1.4);
            }

        }
    }
}
