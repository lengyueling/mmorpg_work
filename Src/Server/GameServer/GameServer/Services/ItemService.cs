using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class ItemService : Singleton<ItemService>
    {
        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);

        }

        public void Init()
        {

        }

        /// <summary>
        /// 服务端购买道具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemBuy::Character:{0}:Shop:{1} ShopItem:{2}", character.Id, request.shopId, request.shopItemId);
            var result = ShopManager.Instance.BuyItem(sender, request.shopId, request.shopItemId);
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResponse();
        }

        /// <summary>
        /// 服务器穿脱装备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemEquip::Character:{0}:Item:{1} Equip:{2}", character.Id, request.Slot, request.itemId);
            var result = EquipManager.Instance.EquipItem(sender, request.Slot, request.itemId, request.isEquip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();
        }
    }
}
