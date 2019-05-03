using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameObject head = GameObject.Find("Player(Clone)");

        if (head != null)
        {
            Vector3 headPosition = GameObject.Find("Head").transform.position;
            headPosition.y -= 2.0f;
            head.transform.position = headPosition;
        }
	}
}
