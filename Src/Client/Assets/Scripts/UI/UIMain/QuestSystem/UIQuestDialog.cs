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

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        if (this.quest.Info == null)
        {
            openButtons.SetActive(true);
            submitButtons.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status == QuestStatus.Complated)
            {
                openButtons.SetActive(true);
                submitButtons.SetActive(false);
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
