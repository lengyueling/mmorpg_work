using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApplyItem : ListView.ListViewItem
{
    public Text nickname;
    public Text @class;
    public Text level;

    public NGuildApplyInfo Info;

    public void SetGuildApplyInfo(NGuildApplyInfo item)
    {
        Info = item;
        if (this.nickname != null)
        {
            this.nickname.text = this.Info.Name;
        }
        if (this.@class != null)
        {
            this.@class.text = this.Info.Class.ToString();
        }
        if (this.level != null)
        {
            this.level.text = this.Info.Level.ToString();
        }
    }

    /// <summary>
    /// 按钮同意加入
    /// </summary>
    public void OnAccept()
    {
        MessageBox.Show(string.Format("确定要通过[{0}]的公会申请吗？",this.Info.Name),"审批申请",MessageBoxType.Confirm, "同意加入", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }

    /// <summary>
    /// 按钮拒绝加入
    /// </summary>
    public void OnDecline()
    {
        MessageBox.Show(string.Format("确定要通过[{0}]的公会申请吗？", this.Info.Name), "审批申请", MessageBoxType.Confirm, "拒绝加入", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }
}
