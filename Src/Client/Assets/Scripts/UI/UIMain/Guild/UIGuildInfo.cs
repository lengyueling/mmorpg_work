﻿using System.Collections;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;
using Common.Data;

public class UIGuildInfo : MonoBehaviour
{
    public Text guildName;
    public Text guildID;
    public Text leader;
    public Text notice;
    public Text memberNumber;
    private NGuildInfo info;
    public NGuildInfo Info
    {
        get { return this.info; }
        set { this.info = value; this.UpdateUI(); }
    }

    private void UpdateUI()
    {
        if (this.info == null)
        {
            this.guildName.text = "无";
            this.guildID.text = "ID:0";
            this.leader.text = "会长:无";
            this.notice.text = "";
            //TODO GameDefine.GuildMaxMemberCount 未增加该配置表
            this.memberNumber.text = string.Format("成员数量：0/{0}", 20);
        }
        else
        {
            this.guildName.text = this.Info.GuildName;
            this.guildID.text = "ID:" + this.Info.Id;
            this.leader.text = "会长:" + this.Info.leaderName;
            this.notice.text = this.Info.Notice;
            //TODO GameDefine.GuildMaxMemberCount 未增加该配置表
            this.memberNumber.text = string.Format("成员数量:{0}/{1}", this.Info.memberCount, 20);
        }
    }
}