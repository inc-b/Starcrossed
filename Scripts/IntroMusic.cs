using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip mainMusic;
    bool introMusic = true;
    public bool intro = true;
    float timer = 0f;
    float fadeSpeed = 5f;
    bool fading = false;

    bool musicOff = false;

    float currentVolume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && introMusic) {
            introMusic = false;
            audioSource.loop = true;
            audioSource.clip = mainMusic;
            audioSource.Play();
        }
        if (fading) {
            timer += Time.deltaTime;
            currentVolume -= Time.deltaTime / fadeSpeed;
        }

        if (timer > fadeSpeed) {
            fading = false;
            audioSource.Stop();
            currentVolume = 0f;
        }

        audioSource.volume = currentVolume;

        if (musicOff) {
            audioSource.volume = 0f;
        }
    }

    public void FadeOut() {
        intro = false;
        fading = true;
    }

    public void MusicOff() {
        musicOff = true;
    }

    public void MusicOn() {
        musicOff = false;
    }
}
