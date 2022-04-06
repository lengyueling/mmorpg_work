using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        /// <summary>
        /// 当前角色购买商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shopId"></param>
        /// <param name="shopItemId"></param>
        /// <returns>是否成功</returns>
        public Result BuyItem(NetConnection<NetSession> sender, int shopId, int shopItemId)
        {
            if (!DataManager.Instance.Shops.ContainsKey(shopId))
            {
                return Result.Failed;
            }
            ShopItemDefine shopItem;
            if (DataManager.Instance.ShopItems[shopId].TryGetValue(shopItemId, out shopItem))
            {
                Log.InfoFormat("BuyItem::Character:{0}:Item:{1} Count:{2} Price:{3}", sender.Session.Character.Id, shopItem.ItemID, shopItem.Count, shopItem.Price);
                if (sender.Session.Character.Gold > shopItem.Price)
                {
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    sender.Session.Character.Gold -= shopItem.Price;
                    DBService.Instance.Save();
                    return Result.Success;
                }
            }
            return Result.Failed;
        }
    }
}
