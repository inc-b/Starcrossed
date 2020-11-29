using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollectedController : MonoBehaviour
{
    public GameObject boxCollected;
    bool messageDisplayed = false;
    float timer = 0f;
    float displayTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        boxCollected.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (messageDisplayed) {
            timer += Time.deltaTime;
            if (timer > displayTime) {
                timer = 0f;
                messageDisplayed = false;
                boxCollected.SetActive(false);
            }
        }
    }

    public void HitBox() {
        boxCollected.SetActive(true);
        messageDisplayed = true;
        timer = 0f;
    }
}
