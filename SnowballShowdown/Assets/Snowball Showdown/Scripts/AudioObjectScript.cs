using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObjectScript : MonoBehaviour
{
    public GameObject scoopSoundsObject, hitSoundsObject, flyInAirSoundsObject, throwSoundsObject, magnetGrabSoundsObject,
    acornSoundObject, dynamiteSoundObject, grenadeSoundObject, NUT, potionSoundObject, snowblowerSoundObject, swordSoundObject, treeSoundObject,
    duckSoundObject, catSoundsObject, rockSoundObject, stickSoundObject, thudSoundObject;

    List<AudioSource> scoopSounds, hitSounds, flyInAirSounds, throwSounds, magnetGrabSounds,
    acornSound, dynamiteSound, grenadeSound, NUTsound, potionSound, snowblowerSound, swordSound, treeSound,
    duckSound, catSounds, rockSound, stickSound, thudSound;

    void Awake()
    {
        scoopSounds = new List<AudioSource>();
        hitSounds = new List<AudioSource>();
        flyInAirSounds = new List<AudioSource>();
        throwSounds = new List<AudioSource>();
        magnetGrabSounds = new List<AudioSource>();
        acornSound = new List<AudioSource>();
        dynamiteSound = new List<AudioSource>();
        grenadeSound = new List<AudioSource>();
        NUTsound = new List<AudioSource>();
        potionSound = new List<AudioSource>();
        snowblowerSound = new List<AudioSource>();
        swordSound = new List<AudioSource>();
        treeSound = new List<AudioSource>();
        duckSound = new List<AudioSource>();
        catSounds = new List<AudioSource>();
        rockSound = new List<AudioSource>();
        stickSound = new List<AudioSource>();
        thudSound = new List<AudioSource>();
    }

    void Start()
    {
        scoopSounds.Add(scoopSoundsObject.GetComponent<AudioSource>());
        for (int i = 0; i < 3; i++)
        {
            hitSounds.Add(hitSoundsObject.GetComponents<AudioSource>()[i]);
        }
        flyInAirSounds.Add(flyInAirSoundsObject.GetComponent<AudioSource>());
        throwSounds.Add(throwSoundsObject.GetComponent<AudioSource>());
        magnetGrabSounds.Add(magnetGrabSoundsObject.GetComponent<AudioSource>());
        acornSound.Add(acornSoundObject.GetComponent<AudioSource>());
        dynamiteSound.Add(dynamiteSoundObject.GetComponent<AudioSource>());
        grenadeSound.Add(grenadeSoundObject.GetComponent<AudioSource>());
        NUTsound.Add(NUT.GetComponent<AudioSource>());
        potionSound.Add(potionSoundObject.GetComponent<AudioSource>());
        snowblowerSound.Add(snowblowerSoundObject.GetComponent<AudioSource>());
        swordSound.Add(swordSoundObject.GetComponent<AudioSource>());
        treeSound.Add(treeSoundObject.GetComponent<AudioSource>());
        duckSound.Add(duckSoundObject.GetComponent<AudioSource>());
        for (int i = 0; i < 2; i++)
        {
            catSounds.Add(catSoundsObject.GetComponents<AudioSource>()[i]);
        }
        rockSound.Add(rockSoundObject.GetComponent<AudioSource>());
        stickSound.Add(stickSoundObject.GetComponent<AudioSource>());
        thudSound.Add(thudSoundObject.GetComponent <AudioSource>());
    }

    public void PlayFlyInAirSound(Vector3 position)
    {
        flyInAirSoundsObject.transform.position = position;
        AudioManager.instance.PlaySound(flyInAirSounds, 0.5f, true, true, true, true);
    }

    public void StopFlyInAirSound()
    {
        AudioManager.instance.StopSound(flyInAirSounds, true, true);
    }

    public void PlayHitSound(Vector3 position)
    {
        hitSoundsObject.transform.position = position;
        AudioManager.instance.PlaySound(hitSounds, 0.9f, false, false, true, false);
        //AudioManager.instance.StopSound(hitSounds, false, false);
    }

    public void PlayScoopSound(Vector3 position)
    {
        scoopSoundsObject.transform.position = position;
        AudioManager.instance.PlaySound(scoopSounds, 0.75f, true, true, true, true);
    }

    public void StopScoopSound()
    {
        AudioManager.instance.StopSound(scoopSounds, true, true);
    }

    public void PlayThrowSound(Vector3 position)
    {
        throwSoundsObject.transform.position = position;
        AudioManager.instance.PlaySound(throwSounds, 0.5f, false, false, true, false);
        //AudioManager.instance.StopSound(throwSounds, false, false);
    }

    public void PlayMagnetGrabSound(Vector3 position)
    {
        magnetGrabSoundsObject.transform.position = position;
        AudioManager.instance.PlaySound(magnetGrabSounds, 0.2f, false, false, false, false);
    }

    public void PlayAcornSound(Vector3 position)
    {
        acornSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(acornSound, 0.5f, false, false, false, false);
    }

    public void PlayDynamiteSound(Vector3 position)
    {
        dynamiteSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(dynamiteSound, 0.6f, false, false, false, false);
    }

    public void PlayGrenadeSound(Vector3 position)
    {
        grenadeSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(grenadeSound, 0.6f, false, false, false, false);
    }

    public void PlayNUTSound(Vector3 position)
    {
        NUT.transform.position = position;
        AudioManager.instance.PlaySound(NUTsound, 0.3f, false, false, false, false);
    }

    public void PlayPotionSound(Vector3 position)
    {
        potionSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(potionSound, 0.5f, false, false, false, false);
    }

    public void PlaySnowblowerSound(Vector3 position)
    {
        snowblowerSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(snowblowerSound, 0.3f, false, false, false, false);
    }

    public void PlaySwordSound(Vector3 position)
    {
        swordSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(swordSound, 0.4f, false, false, false, false);
    }

    public void PlayTreeSound(Vector3 position)
    {
        treeSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(treeSound, 0.65f, false, false, false, false);
    }

    public void PlayDuckSound(Vector3 position)
    {
        duckSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(duckSound, 0.6f, false, false, false, false);
    }

    public void PlayCatSound(Vector3 position)
    {
        catSoundsObject.transform.position = position;
        AudioManager.instance.PlaySound(catSounds, 0.4f, false, false, false, false);
    }

    public void PlayRockSound(Vector3 position)
    {
        rockSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(rockSound, 0.8f, false, false, false, false);
    }

    public void PlayStickSound(Vector3 position)
    {
        stickSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(stickSound, 1.0f, false, false, false, false);
    }

    public void PlayThudSound(Vector3 position)
    {
        treeSoundObject.transform.position = position;
        AudioManager.instance.PlaySound(thudSound, 1.0f, false, false, false, false);
    }
}
