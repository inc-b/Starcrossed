using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlinker : MonoBehaviour
{
    public float blinkSpeed;
    float counter = 0f;
    Light blinkLight;

    // Start is called before the first frame update
    void Start()
    {
        blinkLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > blinkSpeed) {
            counter = 0f;
            blinkLight.enabled = !blinkLight.enabled;
        }
    }
}
