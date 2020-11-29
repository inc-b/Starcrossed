using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUIHandler : MonoBehaviour
{
    public ShopItem shopItem;
    public Text shopItemValue;
    public Text shopItemStock;
    public Image shopItemImage;
    public bool hatedItem;
    public bool favItem;

    public GameObject favIcon;
    public GameObject hatedIcon;

    // Start is called before the first frame update
    void Start()
    {
        shopItemImage.sprite = shopItem.itemImage;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
