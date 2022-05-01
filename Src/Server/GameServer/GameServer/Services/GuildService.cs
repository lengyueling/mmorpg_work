﻿using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);

        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        /// <summary>
        /// 服务端当玩家创建工会
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate::GuildName:{0} character:[{1}]{2}", request.GuildName, character.Id, character.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "您已经有公会了";
                sender.SendResponse();
                return;
            }
            if (GuildManager.Instance.CheckNameExisted(request.GuildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名称已经存在";
                sender.SendResponse();
                return;
            }
            GuildManager.Instance.CreateGuild(request.GuildName, request.GuildNotice, character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }

        /// <summary>
        /// 服务器请求获取公会列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList:character:[{0}]{1}",character.Id,character.Name);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        /// <summary>
        /// 服务器收到加公会请求
        /// 有申请人申请加入公会
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRequest::GuildId:{0} characterId:[{1}]{2}",request.Apply.GuildId,request.Apply.characterId,request.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "公会不存在";
                sender.SendResponse();
                return;
            }
            //根据客户端发来的ID填写对应的信息
            request.Apply.characterId = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;

            if (guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                //给会长发送申请加入请求
                if (leader != null)
                {
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "请勿重复申请";
                sender.SendResponse();
            }
        }

        /// <summary>
        /// 收到加公会响应
        /// 管理员同意或者拒绝了请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinResponse::GuildId:{0} characterId:[{1}]{2}", response.Apply.GuildId, response.Apply.characterId, response.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            //管理员同意了加入请求
            if (response.Result == Result.Success)
            {
                guild.JoinAppove(response.Apply);

            }

            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            if (requester != null)
            {
                requester.Session.Character.Guild = guild;

                requester.Session.Response.guildJoinRes = response;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                requester.SendResponse();
                //设定加入公会角色的公会id
                requester.Session.Character.Data.GuildId = response.Apply.GuildId;
                DBService.Instance.Save();
            }
        }

        /// <summary>
        /// 服务器收到离开公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave::character{0}",character.Id);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            character.Guild.Leave(character);
            sender.Session.Response.guildLeave.Result = Result.Success;
            DBService.Instance.Save();
            sender.SendResponse();
        }

    }
}
