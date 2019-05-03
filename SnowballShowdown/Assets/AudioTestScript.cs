using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTestScript : MonoBehaviour {

    List<AudioSource> music = new List<AudioSource>();
    List<AudioSource> flyInAirSounds = new List<AudioSource>();
    List<AudioSource> hitSounds = new List<AudioSource>();
    List<AudioSource> scoopSounds = new List<AudioSource>();
    List<AudioSource> throwSounds = new List<AudioSource>();

    bool musicIsPlaying = false;
    bool flyInAirIsPlaying = false;
    bool hitIsPlaying = false;
    bool scoopIsPlaying = false;
    bool throwIsPlaying = false;

    void Start()
    {
        music.Add(GetComponents<AudioSource>()[5]);
        flyInAirSounds.Add(GetComponents<AudioSource>()[0]);
        for (int i = 1; i <= 3; i++)
        {
            hitSounds.Add(GetComponents<AudioSource>()[i]);
        }
        scoopSounds.Add(GetComponents<AudioSource>()[4]);
        throwSounds.Add(GetComponents<AudioSource>()[6]);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!musicIsPlaying)
            {
                AudioManager.instance.PlaySound(music, 0.033f, true, false, false, false);
                musicIsPlaying = true;
            }
            else
            {
                AudioManager.instance.StopSound(music, false, true);
                musicIsPlaying = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!flyInAirIsPlaying)
            {
                AudioManager.instance.PlaySound(flyInAirSounds, 1.0f, true, true, true, true);
                flyInAirIsPlaying = true;
            }
            else
            {
                AudioManager.instance.StopSound(flyInAirSounds, true, true);
                flyInAirIsPlaying = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!hitIsPlaying)
            {
                AudioManager.instance.PlaySound(hitSounds, 0.5f, true, false, true, false);
                hitIsPlaying = true;
            }
            else
            {
                AudioManager.instance.StopSound(hitSounds, false, false);
                hitIsPlaying = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!scoopIsPlaying)
            {
                AudioManager.instance.PlaySound(scoopSounds, 0.75f, true, true, true, true);
                scoopIsPlaying = true;
            }
            else
            {
                AudioManager.instance.StopSound(scoopSounds, true, true);
                scoopIsPlaying = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (!throwIsPlaying)
            {
                AudioManager.instance.PlaySound(throwSounds, 0.5f, true, false, true, false);
                throwIsPlaying = true;
            }
            else
            {
                AudioManager.instance.StopSound(throwSounds, false, false);
                throwIsPlaying = false;
            }
        }
    }
}
