using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailHandler : MonoBehaviour
{
    public GameObject emailUI;
    public string[] emails;
    int currentEmail;
    bool emailWaiting = false;

    bool isActive = false;

    int selectedEmail = 0;

    public ShipController ship;

    public Text emailNotification;

    public Text[] emailSubjects;
    public Text emailContent;

    public Color fadeColor;
    public Color selectedColor;

    public EmailReader emailReader;
    bool emailReading = false;

    bool prevEmailRead = true;

    // Start is called before the first frame update
    void Start()
    {
        emailUI.SetActive(false);
        currentEmail = 0;
        emailNotification.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (emailWaiting) {
            emailNotification.enabled = true;
        } else {
            emailNotification.enabled = false;
        }

        if (isActive) {
            GetInput();
            EmailSelector();
            ReadEmail();
        }
    }

    // Returns true if we've already sent the last email (triggers the final cutscene)
    public bool NewEmail() {

        bool returnValue = false;

        if (prevEmailRead) {

            returnValue = true;

            currentEmail++;
            emailWaiting = true;
            ship.emailWaiting = true;
            currentEmail = Mathf.Clamp(currentEmail, 0, emails.Length - 1);
            PlayerPrefs.SetInt("CurrentEmail", currentEmail);
            prevEmailRead = false;
        }

        return returnValue;
    }

    public bool CheckIfLastEmail() {
        bool returnBool = false;

        if (currentEmail >= emails.Length -1) {
            returnBool = true;
        }

        return returnBool;
    }

    public void StartEmails() {
        emailWaiting = true;
    }

    public void ShowEmails() {
        // open up the email UI and start taking input
        isActive = true;
        emailUI.SetActive(true);
        selectedEmail = 0;

        emailReading = false;
        emailReader.StopEmail();

        for (int i = 0; i < emails.Length; i++) {
            if (i <= currentEmail) {
                emailSubjects[i].enabled = true;
            } else {
                emailSubjects[i].enabled = false;
            }
        }
    }

    void CloseEmails() {
        // Hide the email UI and stop taking input
        isActive = false;

        emailReader.StopEmail();

        emailUI.SetActive(false);
        emailNotification.enabled = false;
        ship.CloseEmails();

        emailWaiting = false;

        if (!prevEmailRead) {
            prevEmailRead = true;
        }
    }

    void GetInput() {
        for (int i = 0; i < emailSubjects.Length; i++) {
            emailSubjects[i].color = fadeColor;
        }

        if (Input.GetKeyUp("w")) {
            // Move to prev email
            selectedEmail--;
            emailReading = false;
        }
        if (Input.GetKeyUp("s")) {
            // Move to next email
            selectedEmail++;
            emailReading = false;
        }

        if (Input.GetKeyUp(KeyCode.Tab)) {
            CloseEmails();
        }

        selectedEmail = Mathf.Clamp(selectedEmail, 0, currentEmail);
    }

    void EmailSelector() {
        // Move the email selector ui around based on selectedEmail
        emailSubjects[selectedEmail].color = selectedColor;
    }

    void ReadEmail() {
        // Display email text based on selectedEmail
        emailContent.text = emails[selectedEmail];
        if (!emailReading) {
            emailReader.PlayEmail(selectedEmail);
            emailReading = true;
        }
    }

    public int SaveGame() {
        return currentEmail;
    }

    public void LoadGame(int gameData) {
        currentEmail = gameData;
    }
}
