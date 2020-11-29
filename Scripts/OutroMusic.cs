using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutroMusic : MonoBehaviour
{
    public Jukebox satellary;
    public Jukebox intersatellary;
    public Jukebox station;
    public AudioSource audioSource;

    float timer = 0f;
    float fadeSpeed = 5f;
    bool fading = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fading) {
            timer += Time.deltaTime;
            if (timer > fadeSpeed) {
                fading = false;
                audioSource.Play();
            }
        }   
    }

    public void StartFade() {
        fading = true;
        satellary.FadeOut(fadeSpeed);
        intersatellary.FadeOut(fadeSpeed);
        station.FadeOut(fadeSpeed);
    }
}
