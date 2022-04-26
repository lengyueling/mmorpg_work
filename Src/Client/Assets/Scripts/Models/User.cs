using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using Common.Data;
using UnityEngine;

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
        public GameObject CurrentCharacterObject { get; set; }
        public NTeamInfo TeamInfo { get; set; }

        /// <summary>
        /// 加减金币数量
        /// </summary>
        /// <param name="gold"></param>
        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
        }
    }
}
