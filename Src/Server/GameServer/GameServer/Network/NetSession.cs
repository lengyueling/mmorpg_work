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
    class NetSession : INetSession
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
        /// 相应后处理器
        /// </summary>
        public IPostResponser PostResponser { get; set; }

        /// <summary>
        /// 断开连接，删除角色
        /// </summary>
        public void Disconnected()
        {
            this.PostResponser = null;
            if (Character != null)
            {
                UserService.Instance.CharacterLeave(Character);
            }
        }

        NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                {
                    response.Response = new NetMessageResponse();
                }
                return response.Response;
            }
        }

        /// <summary>
        /// 获取当前会话的打包数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (PostResponser != null)
                {
                    this.PostResponser.PostProcess(Response);
                }
                byte[] data = PackageHandler.PackMessage(response);
                response = null;
                return data;
            }
            return null;
        }
        
    }
}
