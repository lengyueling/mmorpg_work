using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;

public class UIQuestDialog : UIWindow
{
    public UIQuestInfo questInfo;

    public Quest quest;

    public GameObject openButtons;
    public GameObject submitButtons;

    /// <summary>
    /// 设置与NPC交互框的ui
    /// </summary>
    /// <param name="quest"></param>
    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        //网络没有信息说明还没有接任务
        if (this.quest.Info == null)
        {
            openButtons.SetActive(true);
            submitButtons.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status == QuestStatus.Complated)
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(true);
            }
            else
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 更新任务信息
    /// </summary>
    private void UpdateQuest()
    {
        if (this.quest != null)
        {
            if (this.questInfo != null)
            {
                questInfo.SetQuestInfo(quest);
            }
        }
    }
}
