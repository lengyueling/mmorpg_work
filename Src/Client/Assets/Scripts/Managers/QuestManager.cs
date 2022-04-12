using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using Models;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    public enum NpcQuestStatus
    {
        /// <summary>
        /// 无任务
        /// </summary>
        None = 0,
        /// <summary>
        /// 拥有已完成可提交任务
        /// </summary>
        Complete,
        /// <summary>
        /// 拥有可接受任务
        /// </summary>
        Available,
        /// <summary>
        /// 拥有未完成任务
        /// </summary>
        Incomolete,
    }

    class QuestManager : Singleton<QuestManager>
    {
        /// <summary>
        /// 管理所有可用任务
        /// </summary>
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        public List<NQuestInfo> questInfos;

        /// <summary>
        /// 以npcid为分类的npc列表
        /// </summary>
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            this.InitQuests();
        }

        /// <summary>
        /// 初始化可用任务（可接和已接）
        /// </summary>
        void InitQuests()
        {
            //初始化已接任务
            foreach (var info in this.questInfos)
            {
                Quest quest = new Quest(info);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Info.QuestId] = quest;
            }
            //初始化可用任务
            foreach (var kv in DataManager.Instance.Quests)
            {
                //不符合职业
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                {
                    continue;
                }
                //不符合等级
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                {
                    continue;
                }
                //任务已经存在
                if (this.allQuests.ContainsKey(kv.Key))
                {
                    continue;
                }
                if (kv.Value.PreQuest > 0)
                {
                    Quest PreQuest;
                    //获取前置任务
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out PreQuest))
                    {
                        //未领取前置任务
                        if (PreQuest.Info == null)
                        {
                            continue;
                        }
                        //未完成前置任务
                        if (PreQuest.Info.Status != QuestStatus.Finished)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        //未接前置任务
                        continue;
                    }
                }
                Quest quest = new Quest(kv.Value);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        /// <summary>
        /// 在Npc中增加任务
        /// </summary>
        /// <param name="npcId"></param>
        /// <param name="quest"></param>
        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcId))
            {
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();
            }
            //可接
            List<Quest> availables;
            //已完成
            List<Quest> complates;
            //未完成
            List<Quest> incomplates;

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomolete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomolete] = incomplates;
            }

            if (quest.Info == null)
            {
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomolete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Incomolete].Add(quest);
                    }
                }
            }
        }

        /// <summary>
        /// 获得Npc任务状态
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                {
                    return NpcQuestStatus.Complete;
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return NpcQuestStatus.Available;
                }
                if (status[NpcQuestStatus.Incomolete].Count > 0)
                {
                    return NpcQuestStatus.Incomolete;
                }
            }
            return NpcQuestStatus.None;
        }

        /// <summary>
        /// 与Npc进行任务交互
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                }
                    
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                }
                    
                if (status[NpcQuestStatus.Incomolete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Incomolete].First());
                }
            }
            return false;
        }

        /// <summary>
        /// 展示任务对话
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        bool ShowQuestDialog(Quest quest)
        {
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            if (quest.Info != null || quest.Info.Status == QuestStatus.Complated)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                {
                    MessageBox.Show(quest.Define.DialogIncomplete);
                }
            }
            return true;
        }

        /// <summary>
        /// 委托函数当任务对话关闭时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if (result == UIWindow.WindowResult.Yes)
            {
                MessageBox.Show(dlg.quest.Define.DialogAccept);
            }
            else if (result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }
    }
}
