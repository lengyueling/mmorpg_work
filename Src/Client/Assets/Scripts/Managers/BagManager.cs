using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using SkillBridge.Message;

namespace Managers
{
    class BagManager : Singleton<BagManager>
    {
        public int Unlocked;
        public BagItem[] Items;
        NBagInfo info;

        /// <summary>
        /// 初始化背包
        /// </summary>
        /// <param name="info"></param>
        unsafe public void Init(NBagInfo info)
        {
            this.info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[this.Unlocked];
            if (info.Items != null && info.Items.Length >= this.Unlocked)
            {
                Analyze(info.Items);
            }
            else
            {
                info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }
        }

        /// <summary>
        /// 将字节数组解析为结构体数组
        /// </summary>
        /// <param name="items"></param>
        unsafe private void Analyze(byte[] data)
        {
            fixed(byte* pt = data)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;
                }
            }
        }

        /// <summary>
        /// 将结构体数组解析为字节数组
        /// </summary>
        /// <returns></returns>
        unsafe public NBagInfo GetBagInfo()
        {
            //使用fixed时地址不发生改变，使用指针需要被包含在fixed内
            fixed (byte* pt = info.Items)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    //通过平移bagitem单位指针来进行赋值
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return this.info;
        }

        /// <summary>
        /// 背包整理
        /// </summary>
        public void Reset()
        {
            int i = 0;
            //遍历持有的道具
            foreach (var kv in ItemManager.Instance.Items)
            {
                //堆叠判断
                if (kv.Value.Count <= kv.Value.Define.StackLimit)
                {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                //拆分超出堆叠限制的道具
                else
                {
                    int count = kv.Value.Count;
                    while (count > kv.Value.Define.StackLimit)
                    {
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        /// <summary>
        /// 增加背包道具 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        public void AddItem(int itemId, int count)
        {
            ushort addCount = (ushort)count;
            for (int i = 0; i < Items.Length; i++)
            {
                if (this.Items[i].ItemId == itemId)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - this.Items[i].Count);
                    if (canAdd >= addCount)
                    {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    else
                    {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if (addCount > 0)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (this.Items[i].ItemId == 0)
                    {
                        this.Items[i].ItemId = (ushort)itemId;
                        this.Items[i].Count = addCount;
                    }
                }
            }
        }

        /// <summary>
        /// 移除背包道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        public void RemoveItem(int itemId, int count)
        {

        }
    }
}
