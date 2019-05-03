using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour {

    public AudioSource intro;
    public AudioSource mainTrack;

	void Update ()
    {
        if (!intro.isPlaying && !mainTrack.isPlaying)
        {
            mainTrack.Play();
        }
	}
}
