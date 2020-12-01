using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    public UIHandler uiHandler;
    public int wallet = 154; // How much money do we start with?

    Rigidbody rb;
    const float rotateSpeed = 9000f; // How fast the ship will rotate
    const float thrustSpeed = 500f; // How fast the ship will accelerate
    const float maxSpeed = 80f; // Max speed in m/s (ends up x10)
    const float maxSpin = 1.5f; // Max roatation
    float thrustMulti = 1f; // Used to kill thrusters when max speed is hit
    float revThrustMulti = .5f; // Modifier for braking speed
    bool mainThrustOn = false;

    public ParticleSystem leftThruster;
    public ParticleSystem rightThruster;
    public ParticleSystem revThrusters;

    const float defaultDrag = .01f;
    const float brakingDrag = 1f; // Applied when inside a moon's influence
    const float dangerDrag = 2f; // Applied when too close to a moon

    public List<GameObject> moons;
    GameObject closestMoon;
    float closestMoonDistance;

    public List<GameObject> stations;
    GameObject closestStation;
    float closestStationDistance;

    const float stationConnectDistance = 50f;

    const float speedLimitRadiusMulti = 5f; // The radius of a moon is multiplied by this to calculate where the moon's sphere of influence starts
    const float speedLimit = 10f; // Max speed in m/s while inside the moon's sphere of influence (ends up x10)
    float speedLimitRadius; // Used to hold the closest moon's radius of influence
    const float dangerLimitRadiusMulti = 1f; // Similar to above but for when you get too close to a moon
    const float dangerLimit = 1f;
    float dangerLimitRadius = .1f;
    const float maxLimitRadius = 700f;
    const float minLimitRadius = 350f;

    public Text speedReadout;
    public Text speedLimitText;
    public Text moonNameText;
    public Text dangerText;
    public Text warningText;
    public Text stationText;
    public Text fuelReadout;
    public Text outOfFuel;

    bool lastEmail = false;

    float fuel = 45f;
    const float fuelBurnSpeed = .1f;
    const float fuelStockValue = 10f;

    // Tracks which space we're in for changes
    enum Location { intersatellary, satellary, station, undef};
    Location myLocation;

    const float hitIceChance = .01f; // How likely we are to have ice explode off us
    public ParticleSystem hitIce;
    const float hitIceTime = .1f; // How long the ice system fires for
    float hitIceCounter = 0f; // Counter for the timer

    public const float dangerBlinkSpeed = 0.5f; // Blinks the danger text when too close to a moon
    float dangerBlinkCounter = 0f;

    public Light engineGlow;

    bool takeInput = true; // When false this will stop the ship engines from taking input (allows for controlling the shop menus)

    public SpaceStationHandler spaceStationHandler;
    bool isConnected = false;
    public GameObject TomsGarage;

    public int[] stock;
    const int initialStock = 0;

    public JukeboxHandler jukebox;

    bool playIntro = true;
    public bool cutsceneFinished = false;
    public Transform camTransform;
    Vector3 camTarget;
    Vector3 camRot;
    bool movingCamera = false;
    float camMoveSpeed = 5f;
    float camMoveTimer = 0f;
    Vector3 camStartPos;
    Vector3 camStartRot;
    public bool startGame = false;

    public EmailHandler emailHandler;
    public bool emailWaiting = false;
    bool emailAlert = false;

    public GameObject menusObject;
    public bool gameRunning = false;

    public SoundEffects soundEffects;

    public AudioSource engineSound;
    bool engineActive = false;

    const float moneyThreshold = 75f;
    float prevThreshold = 0f;
    

    public OutroMusic outroMusic;
    public OutroCutscene outroCutscene;

    public IceSpawner boxSpawner;

    public SaveLoad saveLoad;

    public BoxCollectedController boxCollectedController;

    public QuestSystem questSystem;

    public GameObject escNotification;

    // Start is called before the first frame update
    void Start()
    {
        escNotification.SetActive(false);
        boxSpawner.spawning = false;
        prevThreshold = wallet;
        menusObject.SetActive(true);
        speedLimitText.enabled = false;
        moonNameText.enabled = false;
        stationText.enabled = false;
        warningText.enabled = false;
        speedReadout.enabled = false;
        fuelReadout.enabled = false;
        outOfFuel.enabled = false;

        camStartPos = new Vector3(6.1f, 6.69f, -3.19f);
        camStartRot = new Vector3(24.31f, -44.9f, 0f);
        camTarget = new Vector3(0f, 6.77f, -6.91f);
        camRot = new Vector3(24.31f, 0f, 0f);

        myLocation = Location.undef;

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxSpin;
        closestMoonDistance = Mathf.Infinity; // Set to max so all moons are closer
        closestStationDistance = Mathf.Infinity; // Set to max so all stations are closer
        dangerText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playIntro) {
            IntroSequence();
            mainThrustOn = true;

            if (rb.velocity.magnitude > speedLimit) {
                thrustMulti = 0f;
            } else {
                thrustMulti = 1f;
            }
        } else if (startGame) {
            if (!isConnected) {
                // Reset speed multi, drag, cargo spawning, and ui elements - they'll be set by tests below
                thrustMulti = 1f;
                rb.drag = defaultDrag;
                speedLimitText.enabled = false;
                moonNameText.enabled = false;
                stationText.enabled = false;
                outOfFuel.enabled = false;
                
                // deactivate cargo spawning (but leave cargo despawning on)

                // Turn off the warning text unless we're getting out of bounds
                warningText.enabled = false;
                if (transform.position.magnitude > 9999f) {
                    warningText.enabled = true;
                }

                // Display the ship's speed
                speedReadout.text = Mathf.Round(rb.velocity.magnitude * 100f).ToString() + "m/s";

                // Display remaining fuel
                fuelReadout.text = "Fuel: " + Mathf.Round(fuel).ToString();

                // Update the closest important objects
                ClosestMoon();
                ClosestSpaceStation();

                // Check our distance from those objects and do things if we cross thresholds
                CheckMoon();
                CheckStation();

                // Check for any boxes nearby that we can collect
                CheckBoxes();

                // Keep the ship under the max speed limit at all times
                if (rb.velocity.magnitude > maxSpeed) {
                    thrustMulti = 0f;
                }

                IceCollide(); // Occasionally makes ice explode off the ship (explains the drag in open space)
                
                // If we earn a threshold amount of money then trigger a new email (and update the threshold counter)
                if (wallet > prevThreshold + moneyThreshold) {
                    prevThreshold += moneyThreshold;
                    bool readyForNewEmail = emailHandler.NewEmail();
                    if (readyForNewEmail) {
                        lastEmail = emailHandler.CheckIfLastEmail();
                        emailAlert = true;
                    }
                }
            }
        }
        

        if (mainThrustOn) {
            ForwardThrusters();
            BurnFuel();
            if (engineActive) {
                engineSound.volume = 1f;
            }
        } else {
            engineSound.volume = 0f;
        }

        if (fuel < 1) {
            soundEffects.Alarm();
            mainThrustOn = false;
            // OUT OF FUEL
            outOfFuel.enabled = true;
            if (Input.GetKeyUp("1") && !stationText.enabled) {
                spaceStationHandler.LoadTom();
                ConnectToTom();
            }
        }

        if (movingCamera) {
            MoveCamera();
        }

        if (takeInput) {
            GetInput();
        }

        if (emailAlert) {
            soundEffects.Fanfare();
            emailAlert = false;
        }
    }

    private void CheckBoxes() {
        List<GameObject> boxes = boxSpawner.GetBoxes();
        List<GameObject> hitBox = new List<GameObject>();
        float boxDistance = 0f;

        foreach (GameObject box in boxes) {
            boxDistance = Vector3.Distance(transform.position, box.transform.position);
            if (boxDistance < 5f) {
                hitBox.Add(box);
                boxCollectedController.HitBox();
                stock[8]++;
            }
        }

        if (hitBox.Count > 0) {
            foreach(GameObject box in hitBox) {
                boxSpawner.RemoveIce(box);
                Destroy(box);
            }
        }
    }

    private void ClosestMoon() {
        if (closestMoon != null) {
            closestMoonDistance = Vector3.Distance(transform.position, closestMoon.transform.position);
        } else {
            closestMoonDistance = Mathf.Infinity;
        }
        // Find the closest moon and set the speed limit radius from it
        foreach (GameObject moon in moons) {
            float moonDistance = Vector3.Distance(transform.position, moon.transform.position);
            if (moonDistance < closestMoonDistance) {
                closestMoonDistance = moonDistance;
                if (closestMoon != moon) {
                    closestMoon = moon;

                    speedLimitRadius = moon.transform.localScale.x * speedLimitRadiusMulti;
                    if (speedLimitRadius > maxLimitRadius) {
                        speedLimitRadius = maxLimitRadius;
                    }
                    if (speedLimitRadius < minLimitRadius) {
                        speedLimitRadius = minLimitRadius;
                    }

                    // dangerLimitRadius = moon.transform.localScale.x * dangerLimitRadiusMulti;


                }
            }
        }
    }

    // Do things when we're close to a moon
    private void CheckMoon() {
        if (Vector3.Distance(transform.position, closestMoon.transform.position) < speedLimitRadius) {
            questSystem.MoonQuestDone();
            // If we're not already in satellary space (or station space) then trigger a music fade based on the zone we're leaving and mark down that we're now in satellary space
            if (myLocation != Location.satellary) {
                boxSpawner.spawning = true;
                if (Vector3.Distance(closestStation.transform.position, transform.position) > stationConnectDistance) {
                    if (myLocation == Location.station) {
                        jukebox.CrossFade("station", "satellary");
                    } else {
                        jukebox.CrossFade("intersatellary", "satellary");
                    }
                    myLocation = Location.satellary;
                }
            }


            // TO DO
            //Activate cargo spawning
            // TO DO

            // Turn on UI elements
            speedLimitText.enabled = true;
            moonNameText.text = closestMoon.name;
            moonNameText.enabled = true;

            // Keep the ship under the speed limit by increasing drag
            if (rb.velocity.magnitude > speedLimit) {
                rb.drag = brakingDrag;
            }

            // If we're too close to a moon then turn on danger signs and limit speed even further
            if (Vector3.Distance(transform.position, closestMoon.transform.position) < dangerLimitRadius) {
                dangerBlinkCounter += Time.deltaTime;
                if (dangerBlinkCounter > dangerBlinkSpeed) {
                    dangerText.enabled = !dangerText.enabled;
                    dangerBlinkCounter = 0f;
                }

                if (rb.velocity.magnitude > dangerLimit) {
                    rb.drag = dangerDrag;
                }
            }
        } else {
            // We're too far away from a moon and entering intersatellary space - mark that and trigger a music fade based on whatever location we were last in
            closestStationDistance = Mathf.Infinity;
            if (myLocation == Location.satellary) {
                jukebox.CrossFade("satellary", "intersatellary");
            } else if (myLocation == Location.station) {
                jukebox.CrossFade("station", "intersatellary");
            }
            boxSpawner.spawning = false;
            myLocation = Location.intersatellary;
        }
    }

    // Do things when we're close to a station
    void CheckStation() {
        if (Vector3.Distance(closestStation.transform.position, transform.position) < stationConnectDistance) {
            questSystem.SpaceStationQuestDone();
            // If we're not already in station space then mark that down and trigger a music fade
            if (myLocation != Location.station) {
                spaceStationHandler.LoadNewStation(closestStation.GetComponent<StationData>());
                boxSpawner.spawning = false;
                myLocation = Location.station;
                jukebox.CrossFade("satellary", "station");
            }

            stationText.text = "Welcome to " + closestStation.name + "\nPress 1 to connect to a salescat";
            stationText.enabled = true;
            if (Input.GetKeyUp("1")) {
                ConnectToStation();
            }
        } else {
            // If we're currently in station space then we're now most likely in satellary space - mark that and trigger a music fade
            if (myLocation == Location.station) {
                boxSpawner.spawning = true;
                myLocation = Location.satellary;
                jukebox.CrossFade("station", "satellary");
               
            }
        }
    }

    private void IceCollide() {
        // Collide with ice effect
        hitIceCounter += Time.deltaTime;
        if (hitIceCounter > hitIceTime) {
            hitIceCounter = 0f;
            hitIce.Stop();

            float hitChance = Random.Range(0f, 1f);
            if (hitChance < hitIceChance * rb.velocity.magnitude / 10f) {
                hitIce.Play();
            }
        }
    }

    // Find the closest space station
    private void ClosestSpaceStation() {
        // Check each station to find which is the closest if our last station is out of range
        foreach (GameObject station in stations) {
            float stationDistance = Vector3.Distance(transform.position, station.transform.position);
            if (stationDistance < closestStationDistance) {
                closestStationDistance = stationDistance;
                // if this isn't the current closest station then update our records and switch out the music
                if (closestStation != station) {
                    closestStation = station;
                    jukebox.SetStation(station.name);
                }
            }
        }

    }

    // Physics stuff
    private void FixedUpdate() {

    }

    void GetInput() {
        if (Input.GetKeyUp("f")) {
            PauseShip();
            isConnected = true;
            emailHandler.ShowEmails();
        }

        // Rotate ship
        rb.AddTorque(Vector3.up * Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime);
        if (Input.GetAxis("Horizontal") != 0) {
            questSystem.SteerQuestDone();
        }

        // Thrusters on/off
        if (Input.GetButtonUp("Jump")) {
            questSystem.SpaceToggleQuestDone();
            mainThrustOn = !mainThrustOn;
        } else {
            if (!mainThrustOn) {
                leftThruster.Stop();
                rightThruster.Stop();
                engineGlow.intensity = 0f;
            }
        }

        // Brakes on/off
        if (Input.GetKey("s")) {
            if (fuel > 1) {
                rb.AddRelativeForce(-Vector3.forward * thrustSpeed * Time.deltaTime * thrustMulti * revThrustMulti);
                revThrusters.Play();
            }
        } else {
            revThrusters.Stop();
        }
    }

    // Pause physics
    void PauseShip() {
        rb.isKinematic = true;
        takeInput = false;
        mainThrustOn = false;
    }

    // Unpauses physics
    void UnpauseShip() {
        rb.isKinematic = false;
        takeInput = true;
    }

    void ConnectToStation() {
        spaceStationHandler.ConnectToStation(closestStation.GetComponent<StationData>());
        PauseShip();
        isConnected = true;
    }

    public void DisconnectFromStation() {
        UnpauseShip();
        saveLoad.SaveGame();
        isConnected = false;
    }

    public void CloseEmails() {
        if (lastEmail) {
            outroMusic.StartFade();
            outroCutscene.StartCutscene();
        } else {
            UnpauseShip();
            isConnected = false;
            emailWaiting = false;
            emailAlert = false;
        }
    }

    public void BuyItem(int itemIndex, GameObject item, StationData stationData, float favMulti, float hatedMulti, float fuelMulti, float pyramidMulti, float moneyMulti) {
        bool fuelBought = false;
        float valueOfItem = Mathf.Infinity;
        ShopItem shopItem = item.GetComponent<ShopItemUIHandler>().shopItem;
        bool boughtSomething = false;

        // Check if we're buying from Tom
        if (stationData.isTom) {
            if (shopItem.name == "Fuel") {
                if (stationData.stock[0] > 0) {
                    // Player can always buy fuel but may go into debt
                    valueOfItem = shopItem.itemValue * fuelMulti;
                    wallet -= (int)valueOfItem;
                    fuelBought = true;
                    soundEffects.Positive();
                    stationData.stock[0]--;
                    stock[0]++;
                }

            } else {
                // Tom won't sell anything else
            }
        } else {
            // Calculate the item's value
            valueOfItem = shopItem.itemValue;

            if (shopItem == stationData.favItem) {
                valueOfItem = valueOfItem * favMulti;
            }
            if (shopItem == stationData.hatedItem) {
                valueOfItem = valueOfItem * hatedMulti;
            }

            if (shopItem.name == "Pyramids" && stationData.isBlackCat) {
                valueOfItem = valueOfItem * pyramidMulti;
            }

            if (shopItem.name == "Money" && stationData.isBusinessCat) {
                valueOfItem = valueOfItem * moneyMulti;
            }

            // If we can afford that and the station has the item in stock then update our cargo/the station's cargo and our wallet/the station's wallet
            if (wallet > valueOfItem && stationData.stock[itemIndex] > 0) {
                wallet -= (int)valueOfItem;
                stationData.wallet += (int)valueOfItem;

                stationData.stock[itemIndex]--;
                stock[itemIndex]++;
                soundEffects.Positive();
                boughtSomething = true;
                if (shopItem.name == "Fuel") {
                    fuelBought = true;
                }
            }
        }

        if (fuelBought) {
            fuel = fuel + fuelStockValue;
            stock[itemIndex]--;
        }
        if (boughtSomething) {
            questSystem.BuySellQuestDone();
        }
    }

    public void SellItem(int itemIndex, GameObject item, StationData stationData, float favMulti, float hatedMulti) {
        bool soldSomething = false;
        // No one buys fuel
        if (item.name == "Fuel") {
            Debug.Log("Cats will not buy fuel");
        } else if (stationData.isTom) {
            // Tom doesn't buy things
        } else {
            // Calculate the item's value
            float valueOfItem = item.GetComponent<ShopItemUIHandler>().shopItem.itemValue;
            if (item == stationData.favItem) {
                valueOfItem = valueOfItem * favMulti;
            }
            if (item == stationData.hatedItem) {
                valueOfItem = valueOfItem * hatedMulti;
            }

            // If we have the item in stock and the station can afford it then update our cargo/the station's cargo and our wallet/the station's wallet
            if (stock[itemIndex] > 0 && stationData.wallet > valueOfItem) {
                wallet += (int)valueOfItem;
                stationData.wallet -= (int)valueOfItem;

                stationData.stock[itemIndex]++;
                stock[itemIndex]--;
                soundEffects.Negative();
                soldSomething = true;
            }
        }

        if (soldSomething) {
            questSystem.BuySellQuestDone();
        }
    }

    void IntroSequence() {
        if (cutsceneFinished) {
            playIntro = false;
        }
    }

    void MoveCamera() {
        camMoveTimer += Time.deltaTime;
        if (camMoveTimer > camMoveSpeed) {
            movingCamera = false;
            emailHandler.StartEmails();
            emailAlert = true;
            emailWaiting = true;
            GameReady();
        } else {
            float smoothTime = Mathf.SmoothStep(0f, 1f, (camMoveTimer / camMoveSpeed));
            camTransform.localPosition = Vector3.Lerp(camStartPos, camTarget, smoothTime);
            camTransform.localRotation = Quaternion.Euler(Vector3.Lerp(camStartRot, camRot, smoothTime));
        }

    }

    void ForwardThrusters() {
        if (fuel > 1) {
            rb.AddRelativeForce(Vector3.forward * thrustSpeed * Time.deltaTime * thrustMulti);
            leftThruster.Play();
            rightThruster.Play();
            engineGlow.intensity = 1f;
        }
    }

    void BurnFuel() {
        fuel -= fuelBurnSpeed * Time.deltaTime;
    }

    void ConnectToTom() {
        spaceStationHandler.ConnectToStation(TomsGarage.GetComponent<StationData>());
        PauseShip();
        isConnected = true;
    }

    public void StartGame() {
        stock = new int[9];
        for (int i = 0; i < 8; i++) {
            stock[i] = initialStock;
        }
        stock[0] = (int)Mathf.Round(fuel / fuelStockValue);
        camStartPos = new Vector3(6.1f, 6.69f, -3.19f);
        camStartRot = new Vector3(24.31f, -44.9f, 0f);
        camTransform.localPosition = camStartPos;
        camTransform.localRotation = Quaternion.Euler(camStartRot);
        menusObject.SetActive(false);
        gameRunning = false;
        engineActive = true;
        transform.position = Vector3.zero;
        uiHandler.mapVisible = false;
        speedReadout.enabled = false;
        fuelReadout.enabled = false;
        startGame = false;
        movingCamera = true;
    }

    public void GameReady() {
        uiHandler.mapVisible = true;
        speedReadout.enabled = true;
        fuelReadout.enabled = true;
        engineActive = true;
        gameRunning = true;
        uiHandler.BgImage();
        startGame = true;
        uiHandler.GameStarted();
        questSystem.DisplayQuests();
        escNotification.SetActive(true);
    }

    public void MenusOn() {
        PauseShip();
        mainThrustOn = false;
    }

    public void MenusOff() {
        GameReady();
        UnpauseShip();
    }

    public float[] SaveGame() {
        float[] returnArray = new float[14];
        returnArray[0] = transform.position.x;
        returnArray[1] = transform.position.y;
        returnArray[2] = transform.position.z;
        returnArray[3] = (float)wallet;
        for (int i = 0; i < 9; i++) {
            returnArray[4 + i] = (float)stock[i];
        }
        returnArray[13] = fuel;
        escNotification.GetComponent<AutoSaved>().autoSaved = true;
        return returnArray;
    }

    public void LoadGame(float[] gameData) {
        Vector3 playerPos = new Vector3(gameData[0], gameData[1], gameData[2]);
        transform.position = playerPos;
        wallet = (int)gameData[3];

        int[] saveStock = new int[9];
        for (int i = 0; i < 9; i++) {
            saveStock[i] = (int)gameData[4 + i];
        }

        stock = saveStock;

        fuel = gameData[13];
        camTarget = new Vector3(0f, 6.77f, -6.91f);
        camRot = new Vector3(24.31f, 0f, 0f);
        camTransform.localPosition = camTarget;
        camTransform.localRotation = Quaternion.Euler(camRot);
        playIntro = false;
        GameReady();
    }

    public void ContinueGame() {
        saveLoad.LoadGame();
    }

    public void HitABox() {
        stock[8]++;
    }
}
