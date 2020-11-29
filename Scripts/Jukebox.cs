using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    AudioSource audioSource;

    float currentVolume = 0f;
    bool fadeIn = false;
    bool fadeOut = false;
    float fadeSpeed;
    float fadeDelay;

    bool musicOff = false;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        // If we're fading in then start increasing the volume
        if (fadeIn) {
            currentVolume += (1f / fadeSpeed) * Time.deltaTime;
            if (currentVolume > 1f) {
                // If the volume is high enough then stop fading
                fadeIn = false;
                currentVolume = 1f;
            }
        }
        // If we're fading out then start decreasing the volume
        if (fadeOut) {
            currentVolume -= (1f / fadeSpeed) * Time.deltaTime;
            // if the volume is low enough then stop fading
            if (currentVolume < 0f) {
                fadeOut = false;
                currentVolume = 0f;
            }
        }

        // Clamp the volume and apply it to the audiosource
        currentVolume = Mathf.Clamp(currentVolume, 0f, 1f);
        audioSource.volume = currentVolume;

        if (musicOff) {
            audioSource.volume = 0f;
        }
    }

    // Called to start a fade out
    public void FadeOut(float speed) {
        fadeSpeed = speed;
        fadeIn = false;
        fadeOut = true;
        if (currentVolume > 1f) {
            currentVolume = 1f;
        }
    }

    // start a fade in
    public void FadeIn(float speed) {
        if (currentVolume < 0f) {
            currentVolume = 0f;
        }
        fadeSpeed = speed;
        fadeIn = true;
        fadeOut = false;
    }

    public void SetClip(AudioClip newClip) {
        audioSource.clip = newClip;
        if (!audioSource.isPlaying) {
            audioSource.Play();
        }   
    }

    public void Restart() {
        if (currentVolume < 0.1f) {
            audioSource.Stop();
            currentVolume = 0f;
            audioSource.volume = 0f;
            audioSource.Play();
        }
    }

    public void MusicOff() {
        musicOff = true;
    }

    public void MusicOn() {
        musicOff = false;
    }
}


