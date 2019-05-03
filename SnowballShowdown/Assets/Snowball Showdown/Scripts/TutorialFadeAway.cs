using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialFadeAway : MonoBehaviour {

    public float timeToFade = 20;
    public float fadeSpeed = .01f;

    public Image[] imagesToFade;

    bool canFade = false;
    Color imageCol;

	// Use this for initialization
	void Start () {
        StartCoroutine(StartFadeTimer());	
	}

    IEnumerator StartFadeTimer()
    {
        yield return new WaitForSeconds(timeToFade);

        canFade = true; 
    }
	
	// Update is called once per frame
	void Update () {
		if (canFade)
        {
            foreach (Image im in imagesToFade)
            {
                imageCol = im.color;
                imageCol.a -= fadeSpeed;
                im.color = imageCol;

                //Works assuming all objects start at the same alpha value
                if (im.color.a <= 0)
                    canFade = false;
            }
        }
	}
}
