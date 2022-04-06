using System;
using System.Collections;
using System.Collections.Generic;
using Common.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIShopItem : MonoBehaviour,ISelectHandler
{
    public Image icon;
    public Text title;
    public Text price;
    public Text count;
    public Image background;
    public Sprite normalBg;
    public Sprite selectBg;

    private bool selected;
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
            this.background.overrideSprite = selected ? selectBg : normalBg;
        }
    }
	
    public int ShopItemID { get; set; }
    private UIShop shop;
    private ItemDefine item;
    private ShopItemDefine ShopItem { get; set; }

	void Start () {
		
	}

	void Update () {
		
	}

    public void SetShopItem(int id, ShopItemDefine shopItem, UIShop owner)
    {
        this.shop = owner;
        this.ShopItemID = id;
        this.ShopItem = shopItem;
        this.item = DataManager.Instance.Items[this.ShopItem.ItemID];
        this.title.text = this.item.Name;
        this.count.text = ShopItem.Count.ToString();
        this.price.text = ShopItem.Price.ToString();
        this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
    }


    public void OnSelect(BaseEventData eventData)
    {
        this.selected = true;
        this.shop.SelectShopItem(this);
    }
}
