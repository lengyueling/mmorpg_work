using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemService : Singleton<ItemService>,IDisposable
{
    public ItemService()
    {
        MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
    }

    public void Dispose()
    {
        MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);

    }

    /// <summary>
    /// 客户端发送购买商品信息给服务端
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="shopItemId"></param>
    public void SendBuyItem(int shopId, int shopItemId)
    {
        Debug.Log("SendBuyItem");

        NetMessage message = new NetMessage();
        message.Request = new NetMessageRequest();
        message.Request.itemBuy = new ItemBuyRequest();
        message.Request.itemBuy.shopId = shopId;
        message.Request.itemBuy.shopItemId = shopItemId;
        NetClient.Instance.SendMessage(message);
    }

    /// <summary>
    /// 当客户端购买商品
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    private void OnItemBuy(object sender, ItemBuyResponse message)
    {
        MessageBox.Show("购买结果：" + message.Result + "\n" + message.Erromessage,"购买完成");
    }
}
