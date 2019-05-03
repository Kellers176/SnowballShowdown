using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControl : MonoBehaviour {

    public GameObject thisHand;
    float timer;

	// Use this for initialization
	void Start () {
        timer = 10f;
        if (gameObject.tag == "LeftGlove")
        {
            thisHand = GameObject.Find("LeftHand");
        }
        else
        {
            thisHand = GameObject.Find("RightHand");
        }
	}
	
	// Update is called once per frame
	void Update () {
        // Hand transform position and rotation
        gameObject.transform.position = thisHand.transform.position;
        gameObject.transform.rotation = thisHand.transform.rotation;

        // Removes default hands from being rendered
        if (timer < 0.0f)
        {
            GameObject.Find("Render Model for RightHand").SetActive(false);
            GameObject.Find("Render Model for LeftHand").SetActive(false);
            timer = 100.0f;
        }
        else if (timer != 100.0f)
        {
            timer -= 0.5f;
        }
        /*
        if (GameObject.Find("LeftHand").GetComponent<NewtonVR.NVRHand>().HoldButtonPressed)
        {
            if (gameObject.tag == "LeftGlove")
            {
                //activate LeftGloveClosed
                gameObject.SetActive(false);
            }
        }
        if (GameObject.Find("RightHand").GetComponent<NewtonVR.NVRHand>().HoldButtonPressed)
        {
            if (gameObject.tag == "RightGlove")
            {
                //activate RightGloveClosed
                gameObject.SetActive(false);
            }
        }
        if (GameObject.Find("LeftHand").GetComponent<NewtonVR.NVRHand>().HoldButtonUp)
        {
            if (gameObject.tag == "LeftGloveClosed")
            {
                GameObject.Find("LeftGlove").SetActive(true);
                gameObject.SetActive(false);
            }
        }
        if (GameObject.Find("RightHand").GetComponent<NewtonVR.NVRHand>().HoldButtonUp)
        {
            if (gameObject.tag == "RightGloveClosed")
            {
                GameObject.Find("RightGlove").SetActive(true);
                gameObject.SetActive(false);
            }
        }
        */
	}
}
