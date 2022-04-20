using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Models;
using GameServer.Entities;
using SkillBridge.Message;
using Common;
using Network;

namespace GameServer.Managers
{

    class SessionManager : Singleton<SessionManager>
    {
        /// <summary>
        /// 管理当前会话
        /// </summary>
        public Dictionary<int, NetConnection<NetSession>> Sessions = new Dictionary<int, NetConnection<NetSession>>();

        /// <summary>
        /// 按照characterId增加会话
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="session"></param>
        public void AddSession(int characterId, NetConnection<NetSession> session)
        {
            this.Sessions[characterId] = session;
        }

        /// <summary>
        /// 按照characterId移除会话
        /// </summary>
        /// <param name="characterId"></param>
        public void RemoveSession(int characterId)
        {
            this.Sessions.Remove(characterId);
        }

        /// <summary>
        /// 按照characterId获取会话
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public NetConnection<NetSession> GetSession(int characterId)
        {
            NetConnection<NetSession> session = null;
            this.Sessions.TryGetValue(characterId, out session);
            return session;
        }
    }
}
