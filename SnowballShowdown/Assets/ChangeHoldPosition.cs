using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHoldPosition : MonoBehaviour
{
    //VR Hands
    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject leftHandHoldPosition;
    public GameObject rightHandHoldPosition;

    
    // get the scale of the object in the hand
    private float GetBaseScale(GameObject hand)
    {
        float totalVal;

        totalVal = hand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Collider>().transform.localScale.x;
        //totalVal += hand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Collider>().transform.localScale.y;
        //totalVal += hand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.GetComponent<Collider>().transform.localScale.z;
        //totalVal /= 3f;
        return totalVal;
    }
    void Start()
    {
        //originalHandPositionLeft = new Vector3(0,0,0.21f);
        //originalHandPositionRight = new Vector3(0, 0, 0.21f);
        ////Debug.Log("========================================================================");
        ////Debug.Log(originalHandPositionLeft);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // when we're not interacting with something, reset where the hold positions are
        leftHandHoldPosition.transform.localPosition = rightHandHoldPosition.transform.localPosition = new Vector3(0f, 0f, 0.2f);

        // left hand interactions, 
        if (leftHand.GetComponent<NewtonVR.NVRHand>().IsInteracting && leftHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
        {
            //Debug.Log("Is interacting");
            float leftHandScale = GetBaseScale(leftHand);

            // offset the hold position by however big the object we're holding is
            // since objects don't scale quite as fast as expected, we can shorten the distance by subtracting the value of the hold position from the scale
            leftHandHoldPosition.transform.localPosition = new Vector3(0f, 0f, leftHandHoldPosition.transform.localPosition.z + (Mathf.Abs(leftHandScale - leftHandHoldPosition.transform.localPosition.z)));
            /*
            if (leftHandScale >= 0f && leftHandScale <= 0.2f)
            {
                holdPos = leftHandHoldPosition.transform.localPosition = new Vector3(0, 0, 0.21f);
            else if (leftHandScale >= 0.2f && leftHandScale < 0.4f)
            {
                holdPos = leftHandHoldPosition.transform.localPosition = new Vector3(0, 0, 0.61f);
            }

            }
            else if (leftHandScale >= 0.4f)
            {
                holdPos = leftHandHoldPosition.transform.localPosition = new Vector3(0, 0, 1.0f);
            }*/

        }
      
        // right hand interactions
        else if (rightHand.GetComponent<NewtonVR.NVRHand>().IsInteracting && rightHand.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.tag == "Snowball")
        {
            //Debug.Log("Is interacting");

            float rightHandScale = GetBaseScale(rightHand);

            // offset the hold position by however big the object is
            rightHandHoldPosition.transform.localPosition = new Vector3(0f, 0f, rightHandHoldPosition.transform.localPosition.z + (Mathf.Abs(rightHandScale - rightHandHoldPosition.transform.localPosition.z)));

            /*
            if (rightHandScale >= 0f && rightHandScale < 0.2f)
            {
                holdPos = rightHandHoldPosition.transform.localPosition = new Vector3(0, 0, 0.21f);
            }
            else if (rightHandScale >= 0.2f && rightHandScale < 0.4f)
            {
                holdPos = rightHandHoldPosition.transform.localPosition = new Vector3(0, 0, 0.41f);
            }
            else if (rightHandScale >= 0.4f)
            {
                holdPos = rightHandHoldPosition.transform.localPosition = new Vector3(0, 0, 0.61f);
            }
            */
        }
 

    }
    
}

