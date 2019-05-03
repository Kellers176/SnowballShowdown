using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScript : MonoBehaviour {

    public bool isTopClosed;
   

    void Start()
    {
        isTopClosed = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        isTopClosed = true;
        //GameObject.Find("Snowblower Top").transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
    }

    private void OnCollisionExit(Collision collision)
    {
        isTopClosed = false;
        //GameObject.Find("Snowblower Top").transform.eulerAngles = new Vector3(0.0f, -90.0f, -60.0f);

    }
}
