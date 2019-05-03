using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanelScript : MonoBehaviour
{

    /* ////////////////////////////////////////////////////////////////////////////////////////
     * 
     * TRANSFORM COMPONENTS FOR HAND PANELS
     * 
     * lEFT (child of NVRPlayer - LeftHand):
     * 
     * Width: 4 Height: 1
     * 
     * pos (-0.049, -0.0133, 0.007)
     * rot (0, -90, -90)
     * scale (0.077772367, 0.077772367, 0.077772367)
     * 
     * 
     *RIGHT (child of NVRPlayer - RightHand):
     * 
     * Width: 4 Height: 1
     * 
     * pos (0.049, 0, 0.007)
     * rot (0, -90, -90)
     * scale (0.077772367, 0.077772367, 0.077772367)
     * 
     */


    
    float MaxHealth;
   // [SerializeField]
    float p1currentHealth;
   // [SerializeField]
    float p2currentHealth;
    bool healthFound = false;


    [SerializeField]
    GameObject HealthController;
    [SerializeField]
    NetworkedHealth NH;



    [SerializeField]
    private GameObject p1healthBar;
    [SerializeField]
    private GameObject p1healthFader;
    Vector3 p1newScale;
    float p1origonalScale;
    Vector3 p1temp;

    [SerializeField]
    private GameObject p2healthBar;
    [SerializeField]
    private GameObject p2healthFader;
    Vector3 p2newScale;
    float p2origonalScale;
    Vector3 p2temp;



    float scaleSpeed = 0.00001f;
    float scaleSpeedp2 = 0.00001f;
    float scaleStartSpeed = 0.00001f;


	void Start () {
        
        NH = HealthController.GetComponent<NetworkedHealth>();
        
       StartCoroutine(GetHealthValue());
       
	}

    private IEnumerator GetHealthValue(){
        yield return new WaitForFixedUpdate();

        if (NH.nHP != 0)
        {
            MaxHealth = NH.nHP;
        }

        if(MaxHealth != 0){
            p1currentHealth = MaxHealth;
            p1newScale = p1healthBar.transform.localScale;
            p1origonalScale = p1newScale.x;

            p2currentHealth = MaxHealth;
            p2newScale = p2healthBar.transform.localScale;
            p2origonalScale = p2newScale.x;
            healthFound = true;
        } else {
           StartCoroutine(GetHealthValue());
        }

    }
	
	void Update () {
        if (healthFound){
            UpdateHealth();         // delete once updatehealth gets called on hit
            ScaleP1FaderBar(p1newScale.x);
            ScaleP2FaderBar(p2newScale.x);
        }
    }

    void UpdateHealth(/*float DMG*/) {

        p1currentHealth = NH.nHP;
        p2currentHealth = NH.nOtherHP;

        p1newScale.x = p1origonalScale * (p1currentHealth / MaxHealth);
        p1healthBar.transform.localScale = p1newScale;
        p2newScale.x = p2origonalScale * (p2currentHealth / MaxHealth);
        p2healthBar.transform.localScale = p2newScale;
       
    }

    void ScaleP1FaderBar(float targetScale) {
        if (p1healthFader.transform.localScale.x > targetScale)
        {
            p1temp = p1healthFader.transform.localScale;
            p1temp.x -= p1temp.x * scaleSpeed;
            p1temp.x = Mathf.Clamp(p1temp.x, targetScale, p1healthFader.transform.localScale.x);
            p1healthFader.transform.localScale = p1temp;
            scaleSpeed *= 1.25f;

        }
        else {
            p1healthFader.transform.localScale = new Vector3 (targetScale, p1healthFader.transform.localScale.y, p1healthFader.transform.localScale.z);
            scaleSpeed = scaleStartSpeed;
        }

    }

    void ScaleP2FaderBar(float targetScale) {
        if (p2healthFader.transform.localScale.x > targetScale)
        {
            p2temp = p2healthFader.transform.localScale;
            p2temp.x -= p2temp.x * scaleSpeedp2;
            p2temp.x = Mathf.Clamp(p2temp.x, targetScale, p2healthFader.transform.localScale.x);
            p2healthFader.transform.localScale = p2temp;
            scaleSpeedp2 *= 1.25f;

        }
        else {
            p2healthFader.transform.localScale = new Vector3 (targetScale, p2healthFader.transform.localScale.y, p2healthFader.transform.localScale.z);
            scaleSpeedp2 = scaleStartSpeed;
        }

    }


}
