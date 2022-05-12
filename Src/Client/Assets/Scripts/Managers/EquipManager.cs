using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using SkillBridge.Message;
using Services;
using UnityEngine;
using Managers;

namespace Managers
{
    public class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();

        public event OnEquipChangeHandler OnEquipChanged;

        /// <summary>
        /// EquipManager仅维护了Equips这个之前在协议中定义好的固定大小的装备数组
        /// </summary>
        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];

        byte[] Data;

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }


        /// <summary>
        /// 是否穿上了指定id的装备
        /// </summary>
        /// <param name="equipId"></param>
        /// <returns></returns>
        public bool Contains(int equipId)
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取装备
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }

        /// <summary>
        /// 装备槽位
        /// 将字节数组解析为装备的数组
        /// </summary>
        /// <param name="data"></param>
        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                    {
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    }
                    else
                    {
                        Equips[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// 装备槽位
        /// 将装备数组解析为字节数组
        /// </summary>
        /// <returns></returns>
        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemid = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                    {
                        *itemid = 0;
                    }
                    else
                    {
                        *itemid = Equips[i].Id;
                    }
                }
            }
            return this.Data;
        }

        /// <summary>
        /// 客户端发送穿装备请求
        /// </summary>
        /// <param name="equip"></param>
        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        /// <summary>
        /// 客户端发送脱装备请求
        /// </summary>
        /// <param name="equip"></param>
        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }

        /// <summary>
        /// 当客户端穿装备
        /// </summary>
        /// <param name="equip"></param>
        public void OnEquipItem(Item equip)
        {
            //当请求装备的装备已经被装备上时，retrun
            if (this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
            {
                return;
            }
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];
            if (OnEquipChanged != null)
            {
                OnEquipChanged();
            }
        }

        /// <summary>
        /// 当客户端拖装备
        /// </summary>
        /// <param name="slot"></param>
        public void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equips[(int)slot] != null)
            {
                this.Equips[(int)slot] = null;
                if (OnEquipChanged != null)
                {
                    OnEquipChanged();
                }
            }
        }
    }
}
