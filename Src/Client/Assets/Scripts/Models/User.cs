using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using Common.Data;
using UnityEngine;
using Services;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;


        public NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(NUserInfo info)
        {
            this.userInfo = info;
        }

        public NCharacterInfo CurrentCharacter { get; set; }
        public MapDefine CurrentMapData { get; set; }
        public PlayerInputController CurrentCharacterObject { get; set; }
        public NTeamInfo TeamInfo { get; set; }

        /// <summary>
        /// 加减金币数量
        /// </summary>
        /// <param name="gold"></param>
        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
        }

        public int CurrentRide = 0;

        /// <summary>
        /// 骑乘坐骑
        /// </summary>
        /// <param name="id"></param>
        internal void Ride(int id)
        {
            if (CurrentRide != id)
            {
                CurrentRide = id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
            else
            {
                CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }
    }
}
