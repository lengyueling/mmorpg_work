using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Models;
using GameServer.Entities;
using SkillBridge.Message;
using Common;
using GameServer.Services;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        /// <summary>
        /// 存储所有公会的公会名称
        /// </summary>
        private HashSet<string> GuildNames = new HashSet<string>();
        public void Init()
        {
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.Guilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }

        private void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id,guild);
            this.GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }

        public bool CheckNameExisted(string name)
        {
            return GuildNames.Contains(name);
        }

        /// <summary>
        /// 创建公会
        /// </summary>
        /// <param name="name"></param>
        /// <param name="notice"></param>
        /// <param name="leader"></param>
        public bool CreateGuild(string name, string notice, Character leader)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.Name = name;
            dbGuild.Notice = notice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.Guilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id,leader.Name, leader.Data.Class, leader.Data.Level, GuildTitle.President);
            leader.Guild = guild;
            DBService.Instance.Save();
            leader.Data.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);
            return true;

        }

        internal Guild GetGuild(int guildId)
        {
            if (guildId == 0)
            {
                return null;
            }
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        /// <summary>
        /// 获取公会清单
        /// </summary>
        /// <returns></returns>
        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result = new List<NGuildInfo>();
            foreach (var kv in this.Guilds)
            {
                result.Add(kv.Value.GuildInfo(null));
            }
            return result;
        }


    }
}
