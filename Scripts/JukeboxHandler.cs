using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxHandler : MonoBehaviour
{
    float fadeSpeed = 1f;
    const float fadeDelay = 5f;
    const float stationFadeInSpeed = 5f;
    const float stationFadeOutSpeed = 10f;
    const float satellaryFadeInSpeed = 20f;
    const float satellaryFadeOutSpeed = 5f;
    const float intersatFadeInSpeed = 1f;
    const float intersatFadeOutSpeed = 20f;

    public Jukebox intersatellary;
    public Jukebox satellary;
    public Jukebox station;
    public IntroMusic introMusic;

    public AudioClip dioStation;
    public AudioClip encStation;
    public AudioClip iapStation;
    public AudioClip mimStation;
    public AudioClip tetStation;
    public AudioClip titstation;

    public AudioClip flightSong1;
    public AudioClip flightSong2;
    bool flightSong1Active = true;

    float timer;
    bool startFadeIn;
    bool waitForFadeIn;

    Jukebox nextJukebox;

    bool musicOn = true;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // If we're waiting to fade in then time it
        if (waitForFadeIn) {
            timer += Time.deltaTime;
            // If the timer limit is hit then stop waiting and start fading
            if (timer > fadeDelay || nextJukebox == intersatellary) {
                timer = 0f;
                waitForFadeIn = false;
                startFadeIn = true;
            }
        }

        // Start the fade in
        if (startFadeIn) {
            if (introMusic.intro) {
                introMusic.FadeOut();
            }
            nextJukebox.FadeIn(fadeSpeed);
            startFadeIn = false;
        }

        if (!musicOn) {
            satellary.MusicOff();
            intersatellary.MusicOff();
            station.MusicOff();
            introMusic.MusicOff();
        } else {
            satellary.MusicOn();
            intersatellary.MusicOn();
            station.MusicOn();
            introMusic.MusicOn();
        }
    }

    public void CrossFade(string currentJ, string upcoming) {
        // Fade out the current jukebox
        switch (currentJ) {
            case "station":
                station.FadeOut(stationFadeOutSpeed);
                break;
            case "satellary":
                satellary.FadeOut(satellaryFadeOutSpeed);
                break;
            case "intersatellary":
                intersatellary.FadeOut(intersatFadeOutSpeed); ;
                break;
        }

        // Set the next jukebox
        switch (upcoming) {
            case "station":
                nextJukebox = station;
                fadeSpeed = stationFadeInSpeed;
                nextJukebox.Restart();
                break;
            case "satellary":
                nextJukebox = satellary;
                fadeSpeed = satellaryFadeInSpeed;
                break;
            case "intersatellary":
                nextJukebox = intersatellary;
                fadeSpeed = intersatFadeInSpeed;
                if (flightSong1Active) {
                    nextJukebox.SetClip(flightSong2);
                    flightSong1Active = false;
                } else {
                    nextJukebox.SetClip(flightSong1);
                    flightSong1Active = true;
                }
                nextJukebox.Restart();
                break;
        }

        // Start the delay before fading in the next
        waitForFadeIn = true;
    }

    public void SetStation(string newStation) {
        switch (newStation) {
            case "Dione Dome":
                station.SetClip(dioStation);
                break;
            case "Princess Coco's Fairyland Feathers":
                station.SetClip(encStation);
                break;
            case "Iapetus Winter Palace":
                station.SetClip(iapStation);
                break;
            case "Mimas Acres":
                station.SetClip(mimStation);
                break;
            case "/// ERROR ///":
                station.SetClip(tetStation);
                break;
            case "Titan Inc.":
                station.SetClip(titstation);
                break;
        }
    }

    public bool MusicToggle() {
        if (musicOn) {
            musicOn = false;
        } else {
            musicOn = true;
        }
        return musicOn;
    }
}
