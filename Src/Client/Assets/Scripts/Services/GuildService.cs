﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using SkillBridge.Message;
using UnityEngine.Events;
using Models;
using Managers;

namespace Services
{
    class GuildService : Singleton<GuildService>, IDisposable
    {
        /// <summary>
        /// 公会更新委托
        /// </summary>
        public UnityAction OnGuildUpdate;
        /// <summary>
        /// 公会创建委托
        /// </summary>
        public UnityAction<bool> OnGuildCreateResult;
        /// <summary>
        /// 列表刷新委托
        /// </summary>
        public UnityAction<List<NGuildInfo>> OnGuildListResult;

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 发送创建公会
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        public void SendGuildCreate(string guildName, string notice)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到公会创建响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("OnGuildCreateResponse:{0}", response.Result);
            if (OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(response.Result == Result.Success);
            }
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("{0}公会创建成功", response.guildInfo.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(string.Format("{0}公会创建失败", response.guildInfo.GuildName), "公会");
            }
        }

        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="guildId"></param>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 发送加入公会请求
        /// 会长等有管理权限的人决定是否同意玩家加入公会
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendGuildJoinResponse(bool accept,GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = request.Apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到加入公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}申请加入公会", request.Apply.Name), "公会申请", MessageBoxType.Confirm, "同意", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinResponse(false, request);
            };
        }

        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinResponse:{0}", response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show("加入公会成功", "公会");
            }
            else
            {
                MessageBox.Show("加入公会失败", "公会");
            }
        }

        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild:{0}{1}{2}", message.Result, message.guildInfo.Id, message.guildInfo.GuildName);
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate != null)
            {
                OnGuildUpdate();
            }
        }

        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "公会");
            }
            else
            {
                MessageBox.Show("离开公会成功", "公会", MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 发送列表刷新请求
        /// </summary>
        public void SendGuildListResponse()
        {
            Debug.Log("SendGuildListResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 客户端响应列表刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if (OnGuildListResult != null)
            {
                this.OnGuildListResult(response.Guilds);
            }
        }

        /// <summary>
        /// 发送离开公会请求
        /// </summary>
        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        
    }
}