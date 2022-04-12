using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Common.Data;

public class UIQuestSystem : UIWindow
{

    public Text title;
    public GameObject itemPrefab;

    public TabView Tabs;
    public ListView listMain;
    public ListView listBranch;

    public UIQuestInfo questInfo;

    /// <summary>
    /// 已接任务为false
    /// 可接任务为true
    /// </summary>
    private bool showAvailableList = false;

    private void Start()
    {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        this.RefreshUI();
        //QuestManager.Instance.OnQuestChanged += RefreshUI;
    }

    /// <summary>
    /// 当选中可接任务或者已接任务时执行此代码
    /// </summary>
    /// <param name="idx">1为可接任务0为已接任务</param>
    void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;
        RefreshUI();
    }

    private void OnDestroy()
    {
        //QuestManager.Instance.OnQuestChanged -= RefreshUI;
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    private void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    /// <summary>
    /// 初始化所有任务列表
    /// </summary>
    void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.allQuests)
        {   
            //如果是可接任务或者已接任务则创建列表
            if (showAvailableList)
            {
                //还没有接的任务服务端一定没有，为空
                //如果有则说明一定是已接任务，不为空
                if (kv.Value.Info != null)
                {
                    continue;
                }
            }
            else
            {
                if (kv.Value.Info == null)
                {
                    continue;
                }
            }
            //判断是主线任务还是支线任务
            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? listMain.transform : listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);

            //增加到主线或者支线任务的列表中
            if (kv.Value.Define.Type == QuestType.Main)
            {
                this.listMain.AddItem(ui as ListView.ListViewItem);
            }
            else
            {
                this.listBranch.AddItem(ui as ListView.ListViewItem);
            }
        }
    }

    /// <summary>
    /// 清除当前所有的任务列表
    /// </summary>
    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    /// <summary>
    /// 当选中任务列表中的任务时
    /// 设置任务信息
    /// </summary>
    /// <param name="item"></param>
    public void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }
}
