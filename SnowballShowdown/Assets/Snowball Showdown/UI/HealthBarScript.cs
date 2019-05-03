using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
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
    float currentHealth;

    [SerializeField]
    Photon.Pun.Demo.PunBasics.PlayerManager PM;
    

    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private GameObject healthFader;

    Vector3 newScale;
    float origonalScale;

    Vector3 temp;
    float scaleSpeed = 0.00001f;
    float scaleStartSpeed = 0.00001f;


	void Start () {
        PM = gameObject.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<Photon.Pun.Demo.PunBasics.PlayerManager>();
        MaxHealth = PM.Health;
        
        if(MaxHealth != null){
            currentHealth = MaxHealth;
            newScale = healthBar.transform.localScale;
            origonalScale = newScale.x;       
        } else {
            Debug.Log("very broken");
        }
       
	}
	
	void Update () {
        UpdateHealth();         // delete once updatehealth gets called on hit
        ScaleFaderBar(newScale.x);
    }

    void UpdateHealth(/*float DMG*/) {
        currentHealth = PM.Health;
        newScale.x = origonalScale * (currentHealth / MaxHealth);
        healthBar.transform.localScale = newScale;
    }

    void ScaleFaderBar(float targetScale) {
        if (healthFader.transform.localScale.x > targetScale)
        {
            temp = healthFader.transform.localScale;
            temp.x -= temp.x * scaleSpeed;
            temp.x = Mathf.Clamp(temp.x, targetScale, healthFader.transform.localScale.x);
            healthFader.transform.localScale = temp;
            scaleSpeed *= 1.25f;

        }
        else {
            healthFader.transform.localScale = new Vector3 (targetScale, healthFader.transform.localScale.y, healthFader.transform.localScale.z);
            scaleSpeed = scaleStartSpeed;
        }

    }


}
