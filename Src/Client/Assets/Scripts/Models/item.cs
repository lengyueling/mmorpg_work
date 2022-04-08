using SkillBridge.Message;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Item
    {
        public int Id;
        public int Count;
        public EquipDefine EquipInfo;
        public ItemDefine Define;

        /// <summary>
        /// 构造函数
        /// this重载了Item类的构造函数
        /// </summary>
        /// <param name="item"></param>
        public Item(NItemInfo item) : this(item.Id, item.Count)
        {
        }

        public Item(int id, int count)
        {
            Id = id;
            Count = count;
            DataManager.Instance.Items.TryGetValue(this.Id, out this.Define);
            DataManager.Instance.Equips.TryGetValue(this.Id, out this.EquipInfo);
        }

        public override string ToString()
        {
            return string.Format("Id:{0},Count:{1}", this.Id, this.Count);
        }
    }
}
