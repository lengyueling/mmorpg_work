using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using SkillBridge.Message;

namespace Managers
{
    class FriendManager : Singleton<FriendManager>
    {
        /// <summary>
        /// 管理所有好友的列表
        /// </summary>
        public List<NFriendInfo> allFriends;

        /// <summary>
        /// 初始化好友列表
        /// </summary>
        /// <param name="friends"></param>
        public void Init(List<NFriendInfo> friends)
        {
            this.allFriends = friends;
        }
    }
}
