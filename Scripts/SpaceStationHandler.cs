using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SpaceStationHandler : MonoBehaviour
{
    public StationData stationData;
    public GameObject stationUI;
    public ShipController ship;
    bool isDisplayed = false;
    public Text displayText;
    public Text catName;
    public Text buySell;
    public Text guideText;
    public VideoPlayer videoPlayer;

    public Text stationWallet;
    public Text playerWallet;

    public GameObject[] shopItems;

    public Image shopUIBG;

    public float favMulti = 1.5f;
    public float hatedMulti = .5f;
    public float fuelMulti = 3f;
    public float pyramidMulti = 3f;
    public float moneyMulti = 3f;

    bool sellMode = true;

    int selectedX = 0;
    int selectedY = 0;
    int itemGridWidth = 3;
    int ItemGridhHeight = 3;

    public GameObject fuelCross;

    // Start is called before the first frame update
    void Start()
    {
        fuelCross.SetActive(false);
        stationUI.SetActive(false);   
        foreach (GameObject shopItemObject in shopItems) {
            ShopItemUIHandler shopItemUI = shopItemObject.GetComponent<ShopItemUIHandler>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisplayed) {
            foreach (GameObject shopItem in shopItems) {
                SetColour(shopItem, Color.grey);
            }
            GetInput();
            UpdateStock();

            stationWallet.text = "¥" + stationData.wallet;
            playerWallet.text = "¥" + ship.wallet;
        }
    }

    void SetColour(GameObject shopItem, Color setColor) {
        shopItem.GetComponentInChildren<Image>().color = setColor;
    }

    void GetInput() {
        float oldX = selectedX;
        float oldY = selectedY;

        if (Input.GetKeyUp(KeyCode.Tab)) {
            stationUI.SetActive(false);
            ship.DisconnectFromStation();
            isDisplayed = false;
            ship.soundEffects.Disconnect();
        } else if (Input.GetKeyUp("d")) {
            // Move selection right
            selectedX++;
        } else if (Input.GetKeyUp("a")) {
            // Move selection left
            selectedX--;
        } else if (Input.GetKeyUp("s")) {
            // Move selection down
            selectedY++;
        } else if (Input.GetKeyUp("w")) {
            // Move selection up
            selectedY--;
        }

        selectedX = Mathf.Clamp(selectedX, 0, itemGridWidth - 1);
        selectedY = Mathf.Clamp(selectedY, 0, ItemGridhHeight -1);

        if (oldX != selectedX || oldY != selectedY) {
            ship.soundEffects.ShortBeep();
        }

        int selectedItem = itemGridWidth * selectedY + selectedX;

        SetColour(shopItems[selectedItem], Color.white);

        if (Input.GetButtonUp("Jump")) {
            if (sellMode) {
                // Buy
                ship.BuyItem(selectedItem, shopItems[selectedItem], stationData, favMulti, hatedMulti, fuelMulti, pyramidMulti, moneyMulti);
            } else {
                // Sell
                ship.SellItem(selectedItem, shopItems[selectedItem], stationData, favMulti, hatedMulti);
            }
        } else if (Input.GetKeyUp("t")) {
            if (stationData.isTom) {
                // Tom doesn't buy
                sellMode = true;
                buySell.text = "Emergency fuel for sale";
                guideText.text = "Press Space to buy";
            } else {
                // Switch buy/sell mode
                if (sellMode) {
                    sellMode = false;
                    buySell.text = "Your Stock (press T to switch)";
                    guideText.text = "Press Space to sell";
                    fuelCross.SetActive(true);
                } else {
                    sellMode = true;
                    buySell.text = "Seller's Stock (press T to switch)";
                    guideText.text = "Press Space to buy";
                    fuelCross.SetActive(false);
                }
            }
        }
    }

    void UpdateStock() {
        foreach (GameObject shopElement in shopItems) {
            ShopItemUIHandler itemUI = shopElement.GetComponent<ShopItemUIHandler>();
            int currentIndex = System.Array.IndexOf(shopItems, shopElement);
            if (sellMode) {
                itemUI.shopItemStock.text = "Stock: " + stationData.stock[currentIndex].ToString();
            } else {
                itemUI.shopItemStock.text = "Stock: " + ship.stock[currentIndex].ToString();
            }
        }
    }

    public void LoadNewStation(StationData newStationData) {
        videoPlayer.clip = newStationData.videoClip;
        stationData = newStationData;
    }

    public void ConnectToStation(StationData station) {

        ship.soundEffects.Connect();

        // Turn on the station UI
        stationUI.SetActive(true);
        isDisplayed = true;

        // Set the UI elements
        displayText.text = stationData.welcomeText;
        displayText.color = stationData.textColour;
        catName.text = stationData.catName;
        shopUIBG.sprite = stationData.shopUIBG;

        buySell.text = "Seller's Stock (press T to switch)";
        guideText.text = "Press Space to buy";
        if (station.isTom) {
            buySell.text = "Emergency fuel for sale";
        }
        fuelCross.SetActive(false);

        // Adjust item prices etc
        ShopItem favItem = stationData.favItem;
        ShopItem hatedItem = stationData.hatedItem;

        foreach (GameObject shopElement in shopItems) {
            ShopItemUIHandler itemUI = shopElement.GetComponent<ShopItemUIHandler>();

            itemUI.favIcon.SetActive(false);
            itemUI.hatedIcon.SetActive(false);

            if (itemUI.shopItem == favItem) {
                itemUI.shopItemValue.text = "¥" + (itemUI.shopItem.itemValue * favMulti).ToString();
                itemUI.favIcon.SetActive(true);
            } else if (itemUI.shopItem == hatedItem) {
                itemUI.shopItemValue.text = "¥" + (itemUI.shopItem.itemValue * hatedMulti).ToString();
                itemUI.hatedIcon.SetActive(true);
            } else {
                itemUI.shopItemValue.text = "¥" + itemUI.shopItem.itemValue.ToString();
            }

            // If the station is Tom's Garage and the item is fuel then the price is increased, otherwise the value is ¥0
            if (station.isTom) {
                if(itemUI.shopItem.name == "Fuel") {
                    itemUI.shopItemValue.text = "¥" + (itemUI.shopItem.itemValue * fuelMulti).ToString();
                } else {
                    itemUI.shopItemValue.text = "¥0";
                }
            }

            if (station.isBlackCat) {
                if (itemUI.shopItem.name == "Pyramids") {
                    itemUI.shopItemValue.text = "¥" + (itemUI.shopItem.itemValue * pyramidMulti).ToString();
                }
            }

            if (station.isBusinessCat) {
                if (itemUI.shopItem.name == "Money") {
                    itemUI.shopItemValue.text = "¥" + (itemUI.shopItem.itemValue * moneyMulti).ToString();
                }
            }

            int currentIndex = System.Array.IndexOf(shopItems, shopElement);
            itemUI.shopItemStock.text = stationData.stock[currentIndex].ToString();
        }

        sellMode = true;

        selectedX = 0;
        selectedY = 0;
    }
}
