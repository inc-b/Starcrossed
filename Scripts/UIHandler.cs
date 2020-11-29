using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Image mapBG;

    public Image dione;
    public Image enceladus;
    public Image iapetus;
    public Image mimas;
    public Image tethys;
    public Image titan;

    public Image dioneS;
    public Image enceladusS;
    public Image iapetusS;
    public Image mimasS;
    public Image tethysS;
    public Image titanS;

    public Image ship;

    public Transform dioT;
    public Transform encT;
    public Transform iapT;
    public Transform mimT;
    public Transform tetT;
    public Transform titT;

    public Transform dioST;
    public Transform encST;
    public Transform iapST;
    public Transform mimST;
    public Transform tetST;
    public Transform titST;

    public Transform shipT;

    public const float mapWidth = 350f;
    public const float mapHeight = 350f;

    public const float worldWidth = 12800;
    public const float worldHeight = 12800f;

    float mapZoomLevel = .5f;
    float maxZoomLevel = 32;

    float offsetX = 0f;
    float offsetY = 0f;

    public bool mapVisible = false;
    public GameObject minimap;

    bool menuActive = false;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public GameObject helpMenu;
    public GameObject confirmMenu;
    public GameObject uiText;
    public GameObject bgImage;
    public Text startButton;
    public GameObject menusObject;

    public ShipController shipController;

    bool newGame = false;

    bool menusActive = true;

    public SaveLoad saveLoad;

    bool ongoingGame = false;

    public Button continueButton;

    public Text emailReaderToggle;
    bool emailReaderActive = false;
    public EmailReader emailReader;

    public JukeboxHandler jukeboxHandler;
    public Text musicToggle;
    bool musicOn = true;

    public QuestSystem questSystem;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        bgImage.SetActive(false);
        helpMenu.SetActive(false);
        confirmMenu.SetActive(false);

        if(PlayerPrefs.GetInt("SaveDataPresent") == 1) {
            saveLoad.LoadGame();
            startButton.text = "Continue";
            ongoingGame = true;
            uiText.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (menuActive) {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                ToggleMenus();
            }

            if (emailReaderActive) {
                emailReaderToggle.text = "Emails will be read aloud";
            } else {
                emailReaderToggle.text = "Emails won't be read aloud";
            }

            if (musicOn) {
                musicToggle.text = "Music is on";
            } else {
                musicToggle.text = "Music is off";
            }

            if (!mapVisible) {
                minimap.SetActive(false);

            } else {
                minimap.SetActive(true);

                MoveImage(dione, dioT);
                MoveImage(enceladus, encT);
                MoveImage(iapetus, iapT);
                MoveImage(mimas, mimT);
                MoveImage(tethys, tetT);
                MoveImage(titan, titT);

                MoveImage(dioneS, dioST);
                MoveImage(enceladusS, encST);
                MoveImage(iapetusS, iapST);
                MoveImage(mimasS, mimST);
                MoveImage(tethysS, tetST);
                MoveImage(titanS, titST);

                MoveImage(ship, shipT);

                // Zoom the map
                if (Input.GetKeyDown("=")) {
                    mapZoomLevel += mapZoomLevel;
                    questSystem.ZoomQuestDone();
                }
                if (Input.GetKeyDown("-")) {
                    mapZoomLevel = mapZoomLevel / 2;
                    questSystem.ZoomQuestDone();
                }
                mapZoomLevel = Mathf.Clamp(mapZoomLevel, .5f, maxZoomLevel);
            }
        }
    }

    void MoveImage(Image image, Transform imaT) {
        float x = imaT.position.x;
        float y = imaT.position.z;

        if (image == ship) {
            float rot = -imaT.transform.eulerAngles.y - 180f;
            Vector3 newRot = new Vector3(0f, 0f, rot);
            image.transform.localRotation = Quaternion.Euler(newRot);
            offsetX = -x;
            offsetY = -y;
        }

        float mapX = (((x + offsetX) / worldWidth) * mapWidth) * mapZoomLevel;
        float mapY = (((y + offsetY)/ worldHeight) * mapHeight) * mapZoomLevel;

        Vector3 newPos = new Vector3(mapX, mapY, 0f);

        image.transform.localPosition = newPos;  
    }

    public void MainMenu() {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        helpMenu.SetActive(false);
        confirmMenu.SetActive(false);
    }

    public void OptionsMenu() {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        creditsMenu.SetActive(false);
        helpMenu.SetActive(false);
        confirmMenu.SetActive(false);
    }

    public void CreditsMenu() {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(true);
        helpMenu.SetActive(false);
        confirmMenu.SetActive(false);
    }

    public void HelpMenu() {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        helpMenu.SetActive(true);
        confirmMenu.SetActive(false);
    }

    public void ConfirmMenu() {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        helpMenu.SetActive(false);
        confirmMenu.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ToggleMenus() {
        if (shipController.gameRunning) {
            MainMenu();
            if (menusActive) {
                menusObject.SetActive(false);
                menusActive = false;
                mainMenu.SetActive(false);
                uiText.SetActive(true);
                shipController.MenusOff();
                saveLoad.SaveGame();
            } else {
                menusObject.SetActive(true);
                menusActive = true;
                uiText.SetActive(false);
                shipController.MenusOn();
            }
        }
    }

    public void BgImage() {
        bgImage.SetActive(true);
    }

    public void GameStarted() {
        startButton.text = "Continue";
    }

    public void StartGame() {
        if (ongoingGame) {
            shipController.ContinueGame();
            ToggleMenus();
            ongoingGame = false;
        } else if (!shipController.gameRunning) {
            shipController.StartGame();
        } else {
            ToggleMenus();
        }
    }

    public void Ready(bool cutScene) {
        newGame = !cutScene;
        menuActive = true;
        mainMenu.SetActive(true);
    }

    public void GameOver() {
        menusActive = true;
        MainMenu();
        continueButton.interactable = false;
        shipController.MenusOn();
    }

    public void ToggleEmailReader() {
        if (emailReaderActive) {
            emailReaderActive = false;
            emailReader.active = false;
        } else {
            emailReaderActive = true;
            emailReader.active = true;
        }
    }

    public void ToggleMusic() {
        musicOn = jukeboxHandler.MusicToggle();
    }
}
