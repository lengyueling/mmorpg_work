using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        /// <summary>
        /// 当前显示频道
        /// </summary>
        public LocalChannel displayChannel;

        /// <summary>
        /// 当前本地频道
        /// </summary>
        public LocalChannel sendChannel;

        public int PrivateID = 0;
        public string PrivateName = "";

        /// <summary>
        /// 当前远程频道
        /// 通过本地频道进行自动转换
        /// </summary>
        public ChatChannel SendChannel
        {
            get
            {
                switch (sendChannel)
                {
                    case LocalChannel.Local:
                        return ChatChannel.Local;
                    case LocalChannel.World:
                        return ChatChannel.World;
                    case LocalChannel.Team:
                        return ChatChannel.Team;
                    case LocalChannel.Guild:
                        return ChatChannel.Guild;
                    case LocalChannel.Private:
                        return ChatChannel.Private;
                }
                return ChatChannel.Local;
            }
        }

        public Action OnChat { get; internal set; }

        public void Init()
        {
            foreach (var message in this.Messages)
            {
                message.Clear();
            }
        }

        /// <summary>
        /// 本地频道
        /// 与服务器中的远程频道有区别
        /// </summary>
        public enum LocalChannel
        {
            ALL = 0,
            Local = 1,
            World = 2,
            Team = 3,
            Guild = 4,
            Private = 5,
        }

        /// <summary>
        /// 频道过滤器
        /// 与本地频道一一对应
        /// </summary>
        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            ChatChannel.Local|ChatChannel.World|ChatChannel.Guild|ChatChannel.Team|ChatChannel.Private|ChatChannel.System,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private
        };

        /// <summary>
        /// 进行私聊
        /// </summary>
        /// <param name="targetId">私聊对象Id</param>
        /// <param name="targetName">私聊对象名</param>
        public void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateID = targetId;
            this.PrivateName = targetName;

            this.sendChannel = LocalChannel.Private;
            if (this.OnChat != null)
            {
                this.OnChat();
            }
        }
        /// <summary>
        /// 分频道管理要发送的信息
        /// </summary>
        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
        };

        /// <summary>
        /// 发送聊天
        /// </summary>
        /// <param name="content"></param>
        /// <param name="toId"></param>
        /// <param name="toName"></param>
        public void SendChat(string content, int toId = 0, string toName = "")
        {
            ChatService.Instance.SendChat(this.SendChannel, content, toId, toName);
        }

        /// <summary>
        /// 返回是否能够使用该频道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool SetSendChannel(LocalChannel channel)
        {
            if (channel == LocalChannel.Team)
            {
                if (User.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍，不能使用队伍频道");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)
            {
                if (User.Instance.CurrentCharacter.Guild == null)
                {
                    this.AddSystemMessage("你没有加入任何公会，不能使用公会频道");
                    return false;
                }
            }
            this.sendChannel = channel;
            Debug.LogFormat("Set Channel:{0}", this.sendChannel);
            return true;
        }

        /// <summary>
        /// 从服务器返回到的消息各频道中增加消息
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="messages"></param>
        public void AddMessages(ChatChannel channel, List<ChatMessage> messages)
        {
            for (int ch = 0; ch < 6; ch++)
            {
                ///&:按位与,仅当左右为1是值为1,否则为0
                if ((this.ChannelFilter[ch] & channel) == channel)
                {
                    this.Messages[ch].AddRange(messages);
                }
            }
            if (this.OnChat != null)
            {
                this.OnChat();
            }
        }

        /// <summary>
        /// 增加系统消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="from"></param>
        public void AddSystemMessage(string message, string from = "")
        {
            this.Messages[(int)LocalChannel.ALL].Add(new ChatMessage()
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName = from
            });
            if (this.OnChat != null)
            {
                this.OnChat();
            }
        }

        /// <summary>
        /// 获取当前所有信息
        /// </summary>
        /// <returns></returns>
        public string GetCurrentMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var message in this.Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(message));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 按照确定格式编辑消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string FormatMessage(ChatMessage message)
        {
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("[本地]{0}{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[世界]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("<color=yellow>[系统]{0}</color>", message.Message);
                case ChatChannel.Private:
                    return string.Format("<color=magenta>[私聊]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Team:
                    return string.Format("<color=green>[队伍]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=blue>[公会]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            }
            return "";
        }

        private string FormatFromPlayer(ChatMessage message)
        {
            if (message.FromId == User.Instance.CurrentCharacter.Id)
            {
                return "<a name=\"\" class=\"player\">[我]</a>";
            }
            else
            {
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId, message.FromName);
            }
        }
    }
}
