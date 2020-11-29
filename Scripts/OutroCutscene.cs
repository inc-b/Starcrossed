using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutroCutscene : MonoBehaviour
{
    public Sprite[] frames;
    const float frameLength = 5f;
    const float fadeSpeed = 3f;
    float counter = 0f;
    bool running = false;
    bool fadeIn = false;
    bool framePlaying = false;
    bool fadeOut = false;
    public Image frameImage;
    int currentFrame = 0;
    float currentFade;
    bool fadeToBlack = false;
    float fadeToBlackSpeed = 5f;
    public Image blackBG;
    public GameObject outroCutsceneUI;
    float currentBlackFade = 0f;
    public UIHandler uiHandler;
    public GameObject menusObject;

    // Start is called before the first frame update
    void Start()
    {
        blackBG.color = new Color(0, 0, 0, 0);
        outroCutsceneUI.SetActive(false);
        frameImage.enabled = false;
        currentFade = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeToBlack) {
            counter += Time.deltaTime;
            if (counter > fadeToBlackSpeed) {
                fadeToBlack = false;
                running = true;
                fadeIn = true;
                framePlaying = false;
                fadeOut = false;
                frameImage.enabled = true;
                currentFade = 0f;
                frameImage.sprite = frames[0];
                currentFrame = 0;
                counter = 0;
            } else {
                currentBlackFade += Time.deltaTime / fadeToBlackSpeed;
                blackBG.color = new Color(0, 0, 0, currentBlackFade);
            }
        } else {
            if (running) {
                Color frameColor = Color.magenta;
                counter += Time.deltaTime;
                if (fadeIn) {
                    if (counter < fadeSpeed) {
                        currentFade += Time.deltaTime / fadeSpeed;
                    } else {
                        counter = 0;
                        fadeIn = false;
                        framePlaying = true;
                    }
                } else if (framePlaying) {
                    if (counter < frameLength) {
                        // Wait
                    } else {
                        counter = 0;
                        framePlaying = false;
                        fadeOut = true;
                    }
                } else if (fadeOut) {
                    if (counter < fadeSpeed) {
                        currentFade -= Time.deltaTime / fadeSpeed;
                    } else {
                        counter = 0;
                        currentFrame++;
                        if (currentFrame == frames.Length) {
                            // Finished
                            running = false;
                            menusObject.SetActive(true);
                            uiHandler.GameOver();
                            outroCutsceneUI.SetActive(false);
                        } else {
                            frameImage.sprite = frames[currentFrame];
                            fadeIn = true;
                            fadeOut = false;
                        }
                    }
                } else {
                    Debug.Log("error");
                }
                frameColor = new Color(currentFade, currentFade, currentFade, 1f);
                frameImage.color = frameColor;
            }
        }
    }

    public void StartCutscene() {
        outroCutsceneUI.SetActive(true);
        fadeToBlack = true;
    }
}
