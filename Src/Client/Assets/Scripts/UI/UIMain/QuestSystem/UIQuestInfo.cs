using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;


public class UIQuestInfo : MonoBehaviour
{
    public Text title;

    public Text[] targets;

    public Text description;

    public UIIconItem rewardItems;

    public Text rewardMoney;
    public Text rewardExp;

    /// <summary>
    /// 设置任务信息
    /// </summary>
    /// <param name="quest"></param>
    public void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        if (quest.Info == null)
        {
            this.description.text = quest.Define.Dialog;
        }
        else
        {
            if (quest.Info.Status == QuestStatus.Complated)
            {
                this.description.text = quest.Define.DialogFinish;
            }
        }
        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        //强制将布局刷新一次，防止内容变了不刷新
        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    /// <summary>
    /// 点击放弃任务
    /// </summary>
    public void OnClickAbandon()
    {

    }
}
