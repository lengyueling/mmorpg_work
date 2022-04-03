using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    /// <summary>
    /// 每一个玩具角色都有一个ItemManager
    /// 所以不做单例
    /// </summary>
    class ItemManager
    {
        Character Owner;

        /// <summary>
        /// 角色身上的道具字典
        /// </summary>
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        /// <summary>
        /// 构造函数
        /// 将当前角色的道具信息加到角色身上的道具字典中
        /// </summary>
        /// <param name="owner"></param>
        public ItemManager(Character owner)
        {
            this.Owner = owner;
            foreach (var item in owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        /// <summary>
        /// 使用道具后移除用过的道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool UseItem(int itemId, int count = 1)
        {
            Log.InfoFormat("[{0}]UseItem[{1}：{2}]", this.Owner.Data.ID, itemId, count);
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                if (item.Count < count)
                {
                    return false;
                }
                //还没加使用的逻辑
                item.Remove(count);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否有这个道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool HasItem(int itemId)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                return item.Count > 0;
            }
            return false;
        }
        
        /// <summary>
        /// 获取这个道具的信息
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public Item GetItem(int itemId)
        {
            Item item = null;
            this.Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}]GetItem[{1}：{2}]", this.Owner.Data.ID, itemId, item);
            return item;
        }

        /// <summary>
        /// 增加道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool AddItem(int itemId, int count)
        {
            Item item = null;
            //道具id对应的道具存在则直接添加，不存在则再创建一个
            if (this.Items.TryGetValue(itemId, out item))
            {
                item.Add(count);
            }
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                this.Items.Add(itemId, item);
            }
            Log.InfoFormat("[{0}]AddItem[{1}] addCount:{2}", this.Owner.Data.ID, itemId, count);
            //DBService.Instance.Save();
            return true;
        }

        /// <summary>
        /// 移除道具
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool RemoveItem(int ItemId, int count)
        {
            if (!this.Items.ContainsKey(ItemId))
            {
                return false;
            }
            Item item = this.Items[ItemId];
            if (item.Count < count)
            {
                return false;
            }
            item.Remove(count);
            Log.InfoFormat("[{0}]AddItem[{1}] removeCount:{2}", this.Owner.Data.ID, item, count);
            //DBService.Instance.Save();
            return true;
        }

        /// <summary>
        /// 将道具内存数据转换为道具协议的列表中
        /// </summary>
        /// <param name="list"></param>
        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo()
                {
                    Id = item.Value.ItemID,
                    Count = item.Value.Count
                });
            }
        }
    }
}
