using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroCutsceneHandler : MonoBehaviour
{
    public bool skipcutScene = true;
    bool cutsceneActive = true;

    public GameObject cutsceneUI;

    public ShipController ship;

    public Image frameImage;
    public Text frameText;
    float frameLength;

    int frameCount = 4;

    public Sprite frame01;
    public Sprite frame02;
    public Sprite frame03;
    public Sprite frame04;

    public string text01;
    public string text02;
    public string text03;
    public string text04;

    const float frame01length = 5f;
    const float frame02length = 4f;
    const float frame03length = 4f;
    const float frame04length = 4f;

    const float textFadeInSpeed = 2f;
    const float textDelay = 1f;
    const float imageFadeInSpeed = 2f;
    const float frameFadeOutSpeed = 1f;
    const float interFrameDelay = 0.5f;
    const float cutsceneFadeSpeed = 5f;

    public UIHandler uiHandler;

    bool textFadeIn = false;
    bool imageFadeIn = false;
    bool fadeOut = false;
    bool cutsceneEnd = false;

    float timer = 0f;

    int currentFrame = 1;

    bool drawFrame = true;

    Color imageColor;
    Color textColor;
    float imageFade = 0f;
    float textFade = 0f;

    public GameObject menuButtons;
    public GameObject UIText;

    // Start is called before the first frame update
    void Start()
    {
        menuButtons.SetActive(false);
        UIText.SetActive(false);
        cutsceneUI.SetActive(true);
        imageColor = new Color(0f, 0f, 0f);
        textColor = new Color(0f,0f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (cutsceneActive) {
            if (drawFrame) {
                drawFrame = false;
                switch (currentFrame) {
                    case 1:
                        PlayFrame(frame01, text01, frame01length);
                        break;
                    case 2:
                        PlayFrame(frame02, text02, frame02length);
                        break;
                    case 3:
                        PlayFrame(frame03, text03, frame03length);
                        break;
                    case 4:
                        PlayFrame(frame04, text04, frame04length);
                        break;
                }
            } else if (!cutsceneEnd) {
                FadeFrame();
            } else if (cutsceneEnd) {
                EndCutScene();
            } else {
                Debug.Log("error");
            }
        }
    }

    void PlayFrame(Sprite frame, string text, float lengthOfFrame) {
        frameImage.sprite = frame;
        frameText.text = text;
        frameLength = lengthOfFrame;

        imageColor = new Color(0f, 0f, 0f);
        textColor = new Color(0f,0f,0f,0f);
        imageFade = 0f;
        textFade = 0f;

        frameImage.color = imageColor;
        frameText.color = textColor;

        timer = 0f;

        imageFadeIn = true;
    }

    void FadeFrame() {
        timer += Time.deltaTime;
        if (skipcutScene) {
            timer = Mathf.Infinity;
        }
        imageFadeIn = false;
        textFadeIn = false;
        fadeOut = false;

        if (timer < imageFadeInSpeed) {
            imageFadeIn = true;
        }

        if (timer > textDelay && timer < textDelay + textFadeInSpeed) {
            textFadeIn = true;
        }

        if (timer > imageFadeInSpeed + frameLength && timer < imageFadeInSpeed + frameLength + frameFadeOutSpeed) {
            fadeOut = true;
        }


        if (imageFadeIn) {
            imageFade = Mathf.Lerp(0f, 1f, timer / imageFadeInSpeed);
        }

        if (fadeOut) {
            imageFade = Mathf.Lerp(1f, 0f, (timer - imageFadeInSpeed - frameLength) / frameFadeOutSpeed);
            textFade = imageFade;
        }

        if (textFadeIn) {
            textFade = Mathf.Lerp(0f, 1f, (timer - textDelay) / textFadeInSpeed);
        }

        imageColor = new Color(imageFade, imageFade, imageFade);
        textColor = new Color(1, 1, 1, textFade);
        frameImage.color = imageColor;
        frameText.color = textColor;

        if (timer > imageFadeInSpeed + frameLength + frameFadeOutSpeed + interFrameDelay) {
            currentFrame++;

            if (currentFrame > frameCount) {
                timer = 0f;
                cutsceneEnd = true;
            } else {
                drawFrame = true;
            }
        }
    }

    void EndCutScene() {
        // call the main menu to fade in (send it the cutscene fade speed as well)
        timer += Time.deltaTime;
        if (timer < cutsceneFadeSpeed) {
            imageColor = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, (timer / cutsceneFadeSpeed)));
            frameImage.color = imageColor;
            frameText.color = imageColor;
        } else {
            uiHandler.Ready(skipcutScene);
            menuButtons.SetActive(true);
            UIText.SetActive(true);
            ship.cutsceneFinished = true;
            cutsceneActive = false;
            cutsceneUI.SetActive(false);
        }
    }
}
