﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Services;
using Common;
using Common.Data;
using Common.Utils;
using Network;
using GameServer.Managers;
using GameServer.Entities;

namespace GameServer.Models
{
    class Guild
    {
        public TGuild Data;

        public int Id { get { return this.Data.Id; } }

        public string Name { get { return this.Data.Name; } }

        public double timestamp;

        public Guild(TGuild guild)
        {
            this.Data = guild;
        }

        /// <summary>
        /// 加入工会申请
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        internal bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null)
            {
                return false;
            }
            //把公会申请信息加入到DB
            var dbApply = DBService.Instance.Entities.GuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);
            DBService.Instance.Save();
            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        /// <summary>
        /// 管理员进行加入公会审批
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        internal bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);
            if (oldApply == null)
            {
                return false;
            }

            oldApply.Result = (int)apply.Result;
            if (apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            }

            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }


        /// <summary>
        /// 审批通过，增加一个成员
        /// 创建公会，会长成为公会的一员
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="name"></param>
        /// <param name="class"></param>
        /// <param name="level"></param>
        /// <param name="title"></param>
        public void AddMember(int characterId,string name,int @class, int level ,GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now,
            };
            this.Data.Members.Add(dbMember);
            //将角色上的公会Id设置为对应的公会Id
            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character != null)
            {
                character.Data.GuildId = this.Id;
            }
            else
            {
                TCharacter dbChar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                dbChar.GuildId = this.Id;
            }
            timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("Leave Guild:{0}:{1}", member.Id, member.Info.Name);
            //TODO:离开公会
        }

        /// <summary>
        /// 公会后处理
        /// </summary>
        /// <param name="from"></param>
        /// <param name="message"></param>
        public void PostProcess(Character from, NetMessageResponse message)
        {
            if (message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(from);
            }
        }

        /// <summary>
        /// 获取公会信息
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count
            };

            if (from != null)
            {
                info.Members.AddRange(GetMemberInfos());
                //只有会长能看到加入申请
                if (from.Id == this.Data.LeaderID)
                {
                    info.Applies.AddRange(GetApplyInfos());
                }
            }
            return info;
        }

        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <returns></returns>
        List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();

            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };

                //在公会内刷新角色信息
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };
        }

        List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach (var apply in this.Data.Applies)
            {
                if (apply.Result != (int)ApplyResult.None)
                {
                    continue;
                }
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Class = apply.Class,
                    Level = apply.Level,
                    Name = apply.Name,
                    Result = (ApplyResult)apply.Result,
                });
            }
            return applies;
        }

        public TGuildMember GetDBMember(int characterId)
        {
            foreach (var member in this.Data.Members)
            {
                if (member.CharacterId == characterId)
                {
                    return member;
                }
            }
            return null;
        }

        /// <summary>
        /// 执行管理操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="targetId"></param>
        /// <param name="sourceId"></param>
        public void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(sourceId);
            switch (command)
            {
                case GuildAdminCommand.Kickout:
                    //TODO 踢人
                    break;
                case GuildAdminCommand.Promote:
                    target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    target.Title = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Transfer:
                    target.Title = (int)GuildTitle.President;
                    target.Title = (int)GuildTitle.None;
                    this.Data.LeaderID = targetId;
                    this.Data.LeaderName = target.Name;
                    break;
            }
            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
        }
    }
}