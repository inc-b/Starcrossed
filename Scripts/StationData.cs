using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StationData : MonoBehaviour
{
    [TextArea(7,7)]
    public string welcomeText;
    public VideoClip videoClip;
    public Sprite shopUIBG;
    public ShopItem favItem;
    public ShopItem hatedItem;
    public Color textColour;
    public string catName;

    public bool isTom;
    public bool isBusinessCat;
    public bool isBlackCat;

    public int minStock;
    public int maxStock;

    public int wallet;
    const int minWallet = 150;
    const int maxWallet = 500;
    const int maxIncome = 20;

    public float refillTime;
    float refillCounter;


    public int[] stock;
    const int fuelIndex = 0; // which item in the stock array is fuel?
    const int pyramidIndex = 6;
    const int moneyIndex = 7;

    // Start is called before the first frame update
    void Start()
    {
        wallet = Random.Range(minWallet, maxWallet);

        refillCounter = 0f;

        stock = new int[9];
        for (int i = 0; i < 8; i++) {
            stock[i] = Random.Range(minStock, maxStock);
        }

        if (isTom) {
            stock[fuelIndex] = 3;
            wallet = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        refillCounter += Time.deltaTime;
        if (refillCounter > refillTime) {
            refillCounter = 0f;

            if (isTom) {
                stock[fuelIndex] = 3;
            } else {
                // Earn some money
                wallet += Random.Range(0, maxIncome);

                // Buy some stock
                for (int i = 0; i < 8; i++) {
                    stock[i]++;
                    if (stock[i] > maxStock) {
                        stock[i] = maxStock;
                    }
                }
            }

            if (isBlackCat) {
                stock[moneyIndex] += maxStock;
            }

            if (isBusinessCat) {
                stock[pyramidIndex] += maxStock;
            }
        }
    }

    public void ConnectToStation() {
        Debug.Log("Connected");
    }
}
