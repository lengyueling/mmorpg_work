using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Services;
using Managers;
using System;

namespace Managers
{
    public class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        /// <param name="shopId"></param>
        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId, out shop))
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if (uiShop != null)
                {
                    uiShop.SetShop(shop);
                }
            }
        }

        /// <summary>
        /// 购买商品
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="shopItemId"></param>
        /// <returns></returns>
        public bool BuyItem(int shopId, int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }
    }
}

