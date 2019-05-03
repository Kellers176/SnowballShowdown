using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    // Seconds it takes for the sound to fade in and out
    public float fadeTime = 0.25f;

    private IEnumerator coroutine;

    void Awake()
    {
        instance = this;
    }

    // AudioManager.instance.PlaySound()
    // vol: The volume of the sound
    // fade: Does the sound fade in and out?
    // pitchVariance: Does the sound vary in pitch each time it plays?
    // savesPosition: Does the sound pause until it is played again? If false, the sound will start over from the beginning.

    public void PlaySound(List<AudioSource> sounds, float vol, bool isLooping, bool fade, bool pitchVariance, bool savesPosition)
    {
        foreach (AudioSource s in sounds)
        {
            if (s.isPlaying)
            {
                if (savesPosition)
                {
                    if (fade)
                    {
                        if (coroutine != null)
                        {
                            StopCoroutine(coroutine);
                        }
                        coroutine = FadeOut(s, true);
                        StartCoroutine(coroutine);
                    }
                    else
                    {
                        s.Pause();
                    }
                }
                else
                {
                    if (fade)
                    {
                        if (coroutine != null)
                        {
                            StopCoroutine(coroutine);
                        }
                        coroutine = FadeOut(s, false);
                        StartCoroutine(coroutine);
                    }
                    else
                    {
                        s.Stop();
                    }
                }
            }
        }

        AudioSource sound = sounds[Random.Range(0, sounds.Count)];
        
        if (fade)
        {
            if (!sound.isPlaying)
            {
                sound.volume = 0;
            }
        }
        else
        {
            sound.volume = vol;
        }

        if (pitchVariance)
        {
            sound.pitch = Random.Range(0.9f, 1.1f);
        }
        else
        {
            sound.pitch = 1.0f;
        }

        sound.loop = isLooping;
        sound.Play();

        if (fade)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = FadeIn(sound, vol);
            StartCoroutine(coroutine);
        }
    }

    // SoundPlayer.instance.StopSound()
    // NOTE: This script is used for pausing AND stopping sounds, which is determined by the boolean parameter.

    public void StopSound(List<AudioSource> sounds, bool fade, bool pausesInsteadOfStopping)
    {
        foreach (AudioSource s in sounds)
        {
            if (fade)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = FadeOut(s, pausesInsteadOfStopping);
                StartCoroutine(coroutine);
            }
            else
            {
                if (pausesInsteadOfStopping)
                {
                    s.Pause();
                }
                else
                {
                    s.Stop();
                }
            }
        }
    }

    private IEnumerator FadeOut(AudioSource sound, bool pausesInsteadOfStopping)
    {
        float initVolume = sound.volume;
        float t = 0.0f;

        while (t < 1.0f)
        {
            if (t < 0.90f)
            {
                sound.volume = Mathf.Lerp(initVolume, 0.0f, t);

                if (initVolume != 0)
                {
                    t += (Time.deltaTime / fadeTime) / initVolume;
                }
                else
                {
                    t += Time.deltaTime / fadeTime;  
                }
            }
            else
            {
                sound.volume = 0.0f;
                break;
            }

            yield return null;
        }

        if (pausesInsteadOfStopping)
        {
            sound.Pause();
        }
        else
        {
            sound.Stop();
        }

        yield return null;
    }

    private IEnumerator FadeIn(AudioSource sound, float newVolume)
    {
        float initVolume = sound.volume;
        float t = 0.0f;

        while (t < 1.0f)
        {
            if (t < 0.95f)
            {
                sound.volume = Mathf.Lerp(initVolume, newVolume, t);

                if (initVolume != 0)
                {
                    t += (Time.deltaTime / fadeTime) / initVolume;
                }
                else
                {
                    t += Time.deltaTime / fadeTime;
                }
            }
            else
            {
                sound.volume = newVolume;
            }

            yield return null;
        }
    }
}