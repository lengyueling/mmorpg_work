using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Item
    {
        TCharacterItem dbItem;
        public int ItemID;
        public int Count;

        /// <summary>
        /// 将道具从数据库中拉出来存为内存
        /// 之后不必频繁访问数据库
        /// </summary>
        /// <param name="item"></param>
        public Item(TCharacterItem item)
        {
            this.dbItem = item;
            this.ItemID = (short)item.ItemID;
            this.Count = (short)item.ItemCount;
        }

        /// <summary>
        /// 增加道具
        /// </summary>
        /// <param name="count"></param>
        public void Add(int count)
        {
            this.Count += count;
            dbItem.ItemCount = this.Count;
        }

        /// <summary>
        /// 移除道具
        /// </summary>
        /// <param name="count"></param>
        public void Remove(int count)
        {
            this.Count -= count;
            dbItem.ItemCount = this.Count;
        }

        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool Use(int count = 1)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.ItemID, this.Count);
        }
    }
}
