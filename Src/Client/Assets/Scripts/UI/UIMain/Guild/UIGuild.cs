using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using SkillBridge.Message;
using Common.Data;
using System;
using Managers;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildItem selectedItem;

	void Start ()
    {
        GuildService.Instance.OnGuildUpdate = UpdateUI;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
	}

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate = null;
    }

    /// <summary>
    /// 更新公会UI
    /// </summary>
    void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitItems();
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
    }

    /// <summary>
    /// 初始化公会成员Items
    /// </summary>
    void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    void ClearList()
    {
        this.listMain.RemoveAll();
    }

    public void OnClickAppliesList()
    {

    }

    public void OnClickLeave()
    {

    }

    public void OnClickChat()
    {

    }

    public void OnClickKickout()
    {

    }

    public void OnClickPromote()
    {

    }

    public void OnClickDepose()
    {

    }

    public void OnClickTransfer()
    {

    }

    public void OnClickSetNotice()
    {

    }
}
