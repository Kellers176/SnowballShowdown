using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundScript : MonoBehaviour {

    List<AudioSource> hitSounds;

    void Awake()
    {
        hitSounds = new List<AudioSource>();
        for (int i = 0; i <= 2; i++)
        {
            hitSounds.Add(GetComponents<AudioSource>()[i]);
        }
    }

    void Start ()
    {
        //AudioManager.instance.PlaySound(hitSounds, 0.5f, false, false, true, false);
        //hitSounds[0].Play();
        Destroy(gameObject, 1.0f);
    }
}
