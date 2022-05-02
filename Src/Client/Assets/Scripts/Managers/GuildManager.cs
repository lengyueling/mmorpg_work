using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using SkillBridge.Message;

namespace Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        /// <summary>
        /// 当前公会的信息
        /// </summary>
        public NGuildInfo guildInfo;

        /// <summary>
        /// 当前公会成员的信息
        /// </summary>
        public NGuildMemberInfo myMemberInfo;

        /// <summary>
        /// 是否加入了公会
        /// </summary>
        public bool HasGuild
        {
            get { return this.guildInfo != null; }
        }

        public void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
            if (guild == null)
            {
                myMemberInfo = null;
                return;
            }
            foreach (var men in guild.Members)
            {
                if (men.characterId == User.Instance.CurrentCharacter.Id)
                {
                    myMemberInfo = men;
                    break;
                }
            }
        }

        /// <summary>
        /// UIMain入口
        /// 展示工会信息
        /// </summary>
        public void ShowGuild()
        {
            if (this.HasGuild)
            {
                UIManager.Instance.Show<UIGuild>();
            }
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        /// <summary>
        /// 当关闭工会创建/加入窗口时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            //点了创建公会
            if (result == UIWindow.WindowResult.Yes)
            {
                UIManager.Instance.Show<UIGuildPopCreate>();
            }
            //点了加入公会
            else if (result == UIWindow.WindowResult.No)
            {
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
