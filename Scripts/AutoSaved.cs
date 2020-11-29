using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSaved : MonoBehaviour
{

    public bool autoSaved = false;
    public Text notificationText;
    float counter = 0f;
    float fadeDelay = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (autoSaved) {
            notificationText.text = "Autosaved...";
            counter += Time.deltaTime;
            if (counter > fadeDelay) {
                counter = 0;
                autoSaved = false;
            } else {
                float alphaAmount = notificationText.color.a -(Time.deltaTime / fadeDelay);
                notificationText.color = new Color(1f, 1f, 0f, alphaAmount);
            }
        } else {
            notificationText.text = "ESC (Main Menu)";
            notificationText.color = new Color(.3f, .3f, .3f, 1f);
        }

    }
}
