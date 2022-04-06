using Common.Data;
using SkillBridge.Message;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        /// <summary>
        /// 初始化角色的道具
        /// 将在协议中的信息转到客户端内存中
        /// </summary>
        /// <param name="items"></param>
        internal void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);
                Debug.LogFormat("ItemManager:Init{0}", item);
            }
            StatusService.Instance.RegisterStatusNofity(StatusType.Item, OnItemNotify);
        }

        /// <summary>
        /// 注册状态通知（委托函数）
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }
            return true;
        }

        /// <summary>
        /// 增加道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        void AddItem(int itemId, int count)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                item.Count += count;
            }
            else
            {
                item = new Item(itemId, count);
                this.Items.Add(itemId, item);
            }
            BagManager.Instance.AddItem(itemId, count);
        }

        /// <summary>
        /// 移除道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        void RemoveItem(int itemId, int count)
        {
            if (!this.Items.ContainsKey(itemId))
            {
                return;
            }
            Item item = this.Items[itemId];
            if (item.Count < count)
            {
                return;
            }
            item.Count -= count;
            BagManager.Instance.RemoveItem(itemId, count);
        }

        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}