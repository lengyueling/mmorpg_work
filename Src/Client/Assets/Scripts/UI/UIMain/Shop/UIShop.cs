using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Data;
using Models;
using Managers;
using SkillBridge.Message;
using System;

public class UIShop : UIWindow
{
    public Text title;
    public Text money;
    public GameObject shopItem;
    ShopDefine shop;
    /// <summary>
    /// 几号商店放置商品的根节点
    /// </summary>
    public Transform[] itemRoot;

    void Start()
    {
        StartCoroutine(InitItem());
    }

    /// <summary>
    /// 初始化商店中的商品
    /// </summary>
    /// <returns></returns>
    IEnumerator InitItem()
    {
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, itemRoot[0]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);
            }
        }
        yield return null;
    }

    /// <summary>
    /// 设置商店信息
    /// </summary>
    /// <param name="shop"></param>
    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }
    private UIShopItem selectedItem;

    /// <summary>
    /// 选择了商品
    /// </summary>
    /// <param name="item"></param>
    public void SelectShopItem(UIShopItem item)
    {
        if (selectedItem != null)
        {
            selectedItem.Selected = false;
        }
        selectedItem = item;

    }

    /// <summary>
    /// 购买商品
    /// </summary>
    public void OnClickBuy()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        if (ShopManager.Instance.BuyItem(this.shop.ID,this.selectedItem.ShopItemID))
        {
            //刷新当前金币,由于此时获取的是服务器返回前的金币数值，所以需要手动减去商品价格来显示当前的金钱
            this.money.text = (User.Instance.CurrentCharacter.Gold - int.Parse(selectedItem.price.text)).ToString();
        }
    }
}
