using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ButtonScript : MonoBehaviour {

    public Animator anim;

    public Mesh pressedMesh; // pressed state
    public Mesh normalMesh; // normal state

    bool canOpen;

	// Use this for initialization
	void Start () {
        canOpen = false;
        anim.SetBool("ButtonPressUp", true);

        // find the snowblower animators depending on which client you are
        if (PhotonNetwork.IsMasterClient)
        {
            anim = GameObject.Find("SnowblowerAnimation").GetComponent<Animator>();
        }
        else
        {
            anim = GameObject.Find("SnowblowerAnimation2").GetComponent<Animator>();
        }
    }

    // when the hand first touches the button, activate the snowblower and change the mesh of the button
    private void OnTriggerEnter(Collider other)
    {
        if((other.tag == "LeftHandCollider" || other.tag == "RightHandCollider") && canOpen == false)
        {
            canOpen = true;
            gameObject.GetComponent<MeshFilter>().mesh = pressedMesh;
            anim.SetBool("ButtonPressUp", true);
        }
        else if((other.tag == "LeftHandCollider" || other.tag == "RightHandCollider") && canOpen == true)
        {
            //do stuff
            canOpen = false;
            anim.SetBool("ButtonPressUp", false);
        }
    }

    // while the hand stays on the button, keep the meshed as the pressed one
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "LeftHandCollider" || other.tag == "RightHandCollider")
            gameObject.GetComponent<MeshFilter>().mesh = pressedMesh;
    }

    // once the hand leaves the button, change it back to its original state
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LeftHandCollider" || other.tag == "RightHandCollider")
            gameObject.GetComponent<MeshFilter>().mesh = normalMesh;
    }
}
