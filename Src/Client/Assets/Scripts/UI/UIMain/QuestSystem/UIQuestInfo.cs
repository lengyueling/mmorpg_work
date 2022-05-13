using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;

public class UIQuestInfo : MonoBehaviour
{
    public Text title;

    public Text[] targets;

    public Text description;

    public Text overview;

    public UIIconItem rewardItems;

    public Text rewardMoney;
    public Text rewardExp;

    public GameObject goalItem;

    public GameObject[] goalItemTarget;

    private GameObject[] goalItemTargetSlot = new GameObject[3];

    public Button navButton;
    private int npc = 0;

    /// <summary>
    /// 设置任务信息
    /// </summary>
    /// <param name="quest"></param>
    public void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);

        //任务概况
        if (this.overview != null)
        {
            this.overview.text = quest.Define.Overview;
        }

        if (this.description != null)
        {
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
        }
        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        if (quest.Info == null)
        {
            this.npc = quest.Define.AcceptNPC;
        }
        else if(quest.Info.Status == QuestStatus.Complated)
        {
            this.npc = quest.Define.SubmitNPC;
        }
        this.navButton.gameObject.SetActive(this.npc > 0);

        //强制将布局刷新一次，防止内容变了不刷新
        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }

        for (int i = 0; i < 3; i++)
        {
            if (goalItemTargetSlot[i] != null)
            {
                Destroy(goalItemTargetSlot[i]);
            }
        }

        //设置目标道具
        if (quest.Define.RewardItem1 <= 0)
        {
            return;
        }
        else
        {

            goalItemTargetSlot[0] = Instantiate(goalItem, goalItemTarget[0].transform);
            var def = DataManager.Instance.Items[quest.Define.RewardItem1];
            var ui = goalItemTargetSlot[0].GetComponent<UIIconItem>();
            ui.SetMainIcon(def.Icon, quest.Define.RewardItem1Count.ToString());
        }
        

        if (quest.Define.RewardItem2 <= 0)
        {
            return;
        }
        else
        {
            goalItemTargetSlot[1] = Instantiate(goalItem, goalItemTarget[1].transform);
            var def = DataManager.Instance.Items[quest.Define.RewardItem2];
            var ui = goalItemTargetSlot[1].GetComponent<UIIconItem>();
            ui.SetMainIcon(def.Icon, quest.Define.RewardItem2Count.ToString());
        }
        

        if (quest.Define.RewardItem3 <= 0)
        {
            return;
        }
        else
        {
            goalItemTargetSlot[2] = Instantiate(goalItem, goalItemTarget[2].transform);
            var def = DataManager.Instance.Items[quest.Define.RewardItem3];
            var ui = goalItemTargetSlot[2].GetComponent<UIIconItem>();
            ui.SetMainIcon(def.Icon, quest.Define.RewardItem3Count.ToString());
        }
    }

    /// <summary>
    /// 点击放弃任务
    /// </summary>
    public void OnClickAbandon()
    {

    }

    public void OnClickNav()
    {
        Vector3 pos = NpcManager.Instance.GetNpcPosition(this.npc);
        User.Instance.CurrentCharacterObject.StartNav(pos);
        UIManager.Instance.Close<UIQuestSystem>();
    }
}
