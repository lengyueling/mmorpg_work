using Common;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        /// <summary>
        /// 穿脱装备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="slot"></param>
        /// <param name="itemId"></param>
        /// <param name="isEquip"></param>
        /// <returns></returns>
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            Character character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemId))
            {
                return Result.Failed;
            }
            UpdateEquip(character.Data.Equips, slot, itemId, isEquip);
            DBService.Instance.Save();
            return Result.Success;
        }


        /// <summary>
        /// 船脱装备到角色的各个部位
        /// </summary>
        /// <param name="equipData"></param>
        /// <param name="slot"></param>
        /// <param name="itemId"></param>
        /// <param name="isEquip"></param>
        unsafe void UpdateEquip(byte[] equipData,int slot,int itemId, bool isEquip)
        {
            fixed(byte* pt = equipData)
            {
                int* slotid = (int*)(pt + slot * sizeof(int));
                if (isEquip)
                {
                    *slotid = itemId;
                }
                else
                {
                    *slotid = 0;
                }
            }
        }
    }
}
