using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession
    {
        /// <summary>
        /// 用户登录时进行赋值
        /// 可以获取当前登录的用户
        /// </summary>
        public TUser User { get; set; }
        /// <summary>
        /// 用户进入游戏时进行赋值
        /// 可以获取当前操作的对象
        /// </summary>
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        /// <summary>
        /// 断开连接，删除角色
        /// </summary>
        internal void Disconnected()
        {
            if (Character != null)
            {
                UserService.Instance.CharacterLeave(Character);
            }
        }
    }
}
