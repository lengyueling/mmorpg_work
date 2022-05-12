using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        public void Init()
        {

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
        /// 当客户端玩家购买商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show("购买结果：" + message.Result + "\n" + message.Erromessage, "购买完成");
        }

        /// <summary>
        /// 当前操作的装备
        /// </summary>
        Item pendingEquip = null;
        /// <summary>
        /// 是穿装备还是脱装备
        /// </summary>
        bool isEquip;
        /// <summary>
        /// 客户端发送玩家装备信息给服务端
        /// </summary>
        /// <param name="equip"></param>
        /// <param name="isEquip"></param>
        /// <returns></returns>
        public bool SendEquipItem(Item equip, bool isEquip)
        {
            if (pendingEquip != null)
            {
                return false;
            }
            Debug.Log("SendEquipItem");
            pendingEquip = equip;
            this.isEquip = isEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.isEquip = isEquip;
            NetClient.Instance.SendMessage(message);

            return true;
        }

        /// <summary>
        /// 当客户端玩家穿脱装备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result == Result.Success)
            {
                if (pendingEquip != null)
                {
                    if (this.isEquip)
                    {
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    }
                    else
                    {
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    }
                    pendingEquip = null;
                }
            }
        }
    }
}
