using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.Events;

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
        Incomplete,
    }

    class QuestManager : Singleton<QuestManager>
    {
        /// <summary>
        /// 管理所有可用任务(可接和已接)
        /// </summary>
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        public List<NQuestInfo> questInfos;

        /// <summary>
        /// 以npcid为分类的npc列表
        /// </summary>
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        /// <summary>
        /// 任务状态改变 委托
        /// </summary>
        public UnityAction<Quest> onQuestStatusChanged;

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
                this.allQuests[quest.Info.QuestId] = quest;
            }

            this.CheckAvailableQuests();

            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }
        /// <summary>
        /// 初始化可用任务
        /// </summary>
        void CheckAvailableQuests()
        {
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

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incomplates;
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
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
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
            //获取Npc任务，顺序不能变，因为有优先级
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
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                {
                    return NpcQuestStatus.Incomplete;
                }
            }
            return NpcQuestStatus.None;
        }

        /// <summary>
        /// 与Npc进行任务交互
        /// 如果存在已完成、进行中、待接任务
        /// 返回status中当前状态中的第一个任务
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            //获取Npc任务
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
                    
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
                }
            }
            return false;
        }

        /// <summary>
        /// 展示npc任务对话框
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        bool ShowQuestDialog(Quest quest)
        {
            //接任务或者交任务的情况
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            //任务进行中的情况
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
                if (dlg.quest.Info == null)
                {
                    QuestService.Instance.SendQuestAccept(dlg.quest);
                }
                else if (dlg.quest.Info.Status == QuestStatus.Complated)
                {
                    QuestService.Instance.SendQuestSubmit(dlg.quest);
                }
            }
            else if (result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }

        /// <summary>
        /// 服务器返回任务信息后刷新任务状态
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        private Quest RefreshQuestStatus(NQuestInfo quest)
        {
            this.npcQuests.Clear();
            //将服务器发来的信息同步到本地
            Quest result;
            if (this.allQuests.ContainsKey(quest.QuestId))
            {
                //更新当前新的任务状态
                this.allQuests[quest.QuestId].Info = quest;
                result = this.allQuests[quest.QuestId];
            }
            else
            {
                result = new Quest(quest);
                this.allQuests[quest.QuestId] = result;
            }
            //重新初始化npcQuests
            this.CheckAvailableQuests();
            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }

            if (onQuestStatusChanged != null)
            {
                onQuestStatusChanged(result);
            }
            return result;
        }

        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        public void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
