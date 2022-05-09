using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Models;
using GameServer.Entities;
using SkillBridge.Message;
using Common;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        /// <summary>
        /// 系统聊天信息列表
        /// </summary>
        public List<ChatMessage> System = new List<ChatMessage>();

        /// <summary>
        /// 世界聊天信息列表
        /// </summary>
        public List<ChatMessage> World = new List<ChatMessage>();

        /// <summary>
        /// 本地聊天信息列表
        /// 每个地图一个id
        /// </summary>
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();

        /// <summary>
        /// 队伍聊天信息列表
        /// 每个队伍一个id
        /// </summary>
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();

        /// <summary>
        /// 公会聊天信息列表
        /// 每个公会一个id
        /// </summary>
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();

        public void Init()
        {

        }

        /// <summary>
        /// 有玩家发消息时添加消息
        /// </summary>
        /// <param name="from"></param>
        /// <param name="message"></param>
        public void AddMessage(Character from, ChatMessage message)
        {
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(from.Info.mapId, message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
                case ChatChannel.Team:
                    this.AddTeamMessage(from.Team.Id, message);
                    break;
                case ChatChannel.Guild:
                    this.AddGuildMessage(from.Guild.Id, message);
                    break;
            }
        }
        /// <summary>
        /// 增加本地消息
        /// 若传入的地图id所在的地图还没有对应的本地消息列表则新建一个
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="message"></param>
        private void AddLocalMessage(int mapId, ChatMessage message)
        {
            if (!this.Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Local[mapId] = messages;
            }
            messages.Add(message);
        }

        private void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        private void AddGuildMessage(int guildId, ChatMessage message)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Guild[guildId] = messages;
            }
            messages.Add(message);
        }

        private void AddTeamMessage(int teamId, ChatMessage message)
        {
            if (!this.Team.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Team[teamId] = messages;
            }
            messages.Add(message);
        }

        /// <summary>
        /// 获取本地信息
        /// </summary>
        /// <param name="mapId">地图id</param>
        /// <param name="idx">获取到第几条信息了</param>
        /// <param name="result">结果</param>
        /// <returns></returns>
        public int GetLocalMessages(int mapId,int idx,List<ChatMessage> result)
        {
            if (!this.Local.TryGetValue(mapId,out List<ChatMessage> message))
            {
                return 0;
            }
            return GetNewMessage(idx, result, message);
        }

        public int GetWorldMessages(int idx, List<ChatMessage> result)
        {
            return GetNewMessage(idx, result, this.World);
        }

        public int GetSystemMessages(int idx, List<ChatMessage> result)
        {
            return GetNewMessage(idx, result, this.System);
        }

        public int GetTeamMessages(int teamId, int idx, List<ChatMessage> result)
        {
            if (!this.Team.TryGetValue(teamId, out List<ChatMessage> message))
            {
                return 0;
            }
            return GetNewMessage(idx, result, message);
        }

        public int GetGuildMessages(int guildId, int idx, List<ChatMessage> result)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> message))
            {
                return 0;
            }
            return GetNewMessage(idx, result, message);
        }

        /// <summary>
        /// 拉取对应频道已经存在的消息
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private int GetNewMessage(int idx, List<ChatMessage> result, List<ChatMessage> message)
        {
            if (idx == 0)
            {
                if (message.Count > GameDefine.MaxChatRecoredNums)
                {
                    idx = message.Count - GameDefine.MaxChatRecoredNums;
                }
            }
            for (; idx < message.Count; idx++)
            {
                result.Add(message[idx]);
            }
            return idx;
        }
    }
}
