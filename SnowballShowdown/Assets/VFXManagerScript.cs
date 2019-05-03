using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
public class VFXManagerScript : MonoBehaviour
{
    // GameObjects used to grab particle effects
    public GameObject lastEffect;
    public GameObject[] particleEffects;
    public bool call = false;

    public static VFXManagerScript instance = null;

    // Relevant Lines:
    // - Snowball Destruction: SnowballGeneral (104) - VFXManagerScript.instance.PlayParticles(1, gameObject.transform.position, gameObject.transform.localScale * 1.5f);
    // - Target Hit (NOT APPLIED): PlayerManager (140 / 145) - VFXManagerScript.instance.PlayParticles(0, [HIT PLAYER].transform.position, new Vector3(2.0f, 2.0f, 2.0f));
    // - Create Snowball: CreateSnowball (112) - VFXManagerScript.instance.PlayParticles(2, snowball.transform.position, snowball.transform.localScale * 2.0f);
    // - Combine Items: CombineSnowball (354) - VFXManagerScript.instance.PlayParticles(2, heldObject.transform.position, heldObject.transform.localScale * 2.0f);
    // - Create Item: Snowblower (64 / 71 / 78 / 85) - VFXManagerScript.instance.PlayParticles(2, [OBJECT_NAME].transform.position, heldObject.transform.localScale * 2.0f);

    // Awake is always called before any Start functions
    void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            instance = this;
        }

        // If the instance already exists, destroy this object
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Allows the user to check a box to automatically play a desired effect (debugging!)
        // if (call)
        // {
        //     PlayParticles(0, new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0.5f));
        //     call = false;
        // }
    }

    // Plays a particle effect at a specific location and scale
    public void PlayParticles(int index, Vector3 pos, Vector3 scale)
    {
        // Destroys the last GameObject
        // WARNING: Effect might not work well in a multiplayer context! Please redesign if possible!
        // Comment this out if it causes problems during or before testing!
        // if (lastEffect != null)
        // {
        //     Destroy(lastEffect);
        // }

        // Instantiates the GameObject
        // WARNING: Instantiating might be suboptimal! Might want to replace later!
        GameObject temp = Instantiate(particleEffects[index], pos, Quaternion.identity);
        temp.transform.localScale = new Vector3(temp.transform.localScale.x * scale.x, temp.transform.localScale.y * scale.y, temp.transform.localScale.z * scale.z);
        lastEffect = temp;

        // Checks if the object has a child and scales all children accordingly
        for (int i = 0; i < temp.transform.childCount; i++)
        {
            GameObject tempChild = temp.transform.GetChild(i).gameObject;
            tempChild.transform.localScale = new Vector3(tempChild.transform.localScale.x * scale.x, tempChild.transform.localScale.y * scale.y, tempChild.transform.localScale.z * scale.z);
        }

        // Plays the effect
        temp.GetComponent<ParticleSystem>().Play();
    }

    // Plays a particle effect at a specific location and scale
    public GameObject ParentParticle(int index, GameObject parent, Vector3 pos, Vector3 scale)
    {
        // Instantiates the GameObject
        GameObject temp = Instantiate(particleEffects[index], pos, Quaternion.identity);
        temp.transform.localScale = new Vector3(temp.transform.localScale.x * scale.x, temp.transform.localScale.y * scale.y, temp.transform.localScale.z * scale.z);
        lastEffect = temp;

        // Parents the effect to a parent
        temp.transform.SetParent(parent.transform);

        // Checks if the object has a child and scales all children accordingly
        for (int i = 0; i < temp.transform.childCount; i++)
        {
            GameObject tempChild = temp.transform.GetChild(i).gameObject;
            tempChild.transform.localScale = new Vector3(tempChild.transform.localScale.x * scale.x, tempChild.transform.localScale.y * scale.y, tempChild.transform.localScale.z * scale.z);
        }

        // Plays the effect
        temp.GetComponent<ParticleSystem>().Play();
        return temp;
    }

    public void DestroyParticle(GameObject particle)
    {
        PhotonNetwork.Destroy(particle);
    }
}