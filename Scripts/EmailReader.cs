using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailReader : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;
    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayEmail(int selectedEmail) {
        if (active) {
            audioSource.clip = clips[selectedEmail];
            audioSource.Play();
        }
    }

    public void StopEmail() {
        audioSource.Stop();
    }
}
