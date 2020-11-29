using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    AudioSource audioSource;

    public AudioClip alarm;
    public AudioClip alert;
    public AudioClip connect;
    public AudioClip disconnect;
    public AudioClip fanfare;
    public AudioClip negative;
    public AudioClip positive;
    public AudioClip shortBeep;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Alarm() {
        audioSource.clip = alarm;
        audioSource.Play();
    }

    public void Alert() {
        audioSource.clip = alert;
        audioSource.Play();
    }
    public void Connect() {
        audioSource.clip = connect;
        audioSource.Play();
    }
    public void ShortBeep() {
        audioSource.clip = shortBeep;
        audioSource.Play();
    }
    public void Disconnect() {
        audioSource.clip = disconnect;
        audioSource.Play();
    }
    public void Fanfare() {
        audioSource.clip = fanfare;
        audioSource.Play();
    }
    public void Negative() {
        audioSource.clip = negative;
        audioSource.Play();
    }
    public void Positive() {
        audioSource.clip = positive;
        audioSource.Play();
    }
}
