using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamController : MonoBehaviour {

    [SerializeField]
    Transform[] cameraViews;
    [SerializeField]
    Transform[] lookPos;
    [SerializeField]
    GameObject lookTarget;

    [SerializeField]
    float speed;

    float lerpTime;
    float lookDist;
    Transform target;
    Transform target2;

    [SerializeField]
    bool debugMode;
    [SerializeField]
    Camera specCam;
    bool on = true;
    

	// Use this for initialization
	void Start () {
        target = cameraViews[0];
        target2 = lookPos[0];
        lerpTime = 0;
        gameObject.transform.position = cameraViews[0].position;
        lookTarget.transform.position = lookPos[0].position;
        
    }
	
	// Update is called once per frame
	void Update () {
        CheckInput();

        if (!debugMode)
        {
            if (lerpTime < 1)
            {
                lerpTime += Time.deltaTime * speed;
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target.position, lerpTime);
                lookTarget.transform.position = Vector3.Lerp(lookTarget.transform.position, target2.position, lerpTime);
            }
        }
        else {
            gameObject.transform.position = target.transform.position;
            lookTarget.transform.position = target2.transform.position;
        }


    }

    void CheckInput()
    {

        if ((Input.GetKey(KeyCode.Keypad0)) || (Input.GetKey(KeyCode.Alpha0)))
        {            
            lerpTime = 0;
            target = cameraViews[0];
            target2 = lookPos[0];
        }
        else if ((Input.GetKey(KeyCode.Keypad1)) || (Input.GetKey(KeyCode.Alpha1)))
        {            
            lerpTime = 0;
            target = cameraViews[1];
            target2 = lookPos[1];            
        }
        else if ((Input.GetKey(KeyCode.Keypad2)) || (Input.GetKey(KeyCode.Alpha2)))
        {
            lerpTime = 0;
            target = cameraViews[2];
            target2 = lookPos[2];
        }
        else if ((Input.GetKey(KeyCode.Keypad3)) || (Input.GetKey(KeyCode.Alpha3)))
        {
            lerpTime = 0;
            target = cameraViews[3];
            target2 = lookPos[3];
        }
        else if ((Input.GetKey(KeyCode.Keypad4)) || (Input.GetKey(KeyCode.Alpha4)))
        {
            lerpTime = 0;
            target = cameraViews[4];
            target2 = lookPos[4];
        }
        else if ((Input.GetKey(KeyCode.Keypad5)) || (Input.GetKey(KeyCode.Alpha5)))
        {
            lerpTime = 0;
            target = cameraViews[5];
            target2 = lookPos[5];
        }
        else if ((Input.GetKey(KeyCode.Keypad6)) || (Input.GetKey(KeyCode.Alpha6)))
        {
            lerpTime = 0;
            target = cameraViews[6];
            target2 = lookPos[6];
        }
        else if ((Input.GetKey(KeyCode.Keypad7)) || (Input.GetKey(KeyCode.Alpha7)))
        {
            lerpTime = 0;
            target = cameraViews[7];
            target2 = lookPos[7];
        }
        else if ((Input.GetKey(KeyCode.Keypad8)) || (Input.GetKey(KeyCode.Alpha8)))
        {
            lerpTime = 0;
            target = cameraViews[8];
            target2 = lookPos[8];
        }
        else if ((Input.GetKey(KeyCode.Keypad9)) || (Input.GetKey(KeyCode.Alpha9)))
        {
            lerpTime = 0;
            target = cameraViews[9];
            target2 = lookPos[9];
        }

        if ((Input.GetKeyDown(KeyCode.Return)) || (Input.GetKeyDown(KeyCode.KeypadEnter))) {
            if (on)
            {
                specCam.depth = -1;
                on = false;
            }
            else
            {
                specCam.depth = 1;
                on = true;
            }

        }

    }
}
