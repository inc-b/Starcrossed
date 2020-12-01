using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChaser : MonoBehaviour
{
    public Light[] lights;
    int currentLight = 0;
    float timer = 0f;
    public float blinkDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > blinkDelay) {
            currentLight++;
            timer = 0f;
        }

        if (currentLight > lights.Length - 1) {
            currentLight = 0;
        }

        foreach (Light light in lights) {
            light.enabled = false;
        }
        lights[currentLight].enabled = true;

    }
}
