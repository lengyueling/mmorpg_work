using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class QuestManager
    {
        Character Owner;

        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// 获取通过db中所有的任务信息
        /// </summary>
        /// <param name="list"></param>
        public void GetQuestsInfos(List<NQuestInfo> list)
        {
            foreach (var quest in Owner.Data.Quests)
            {
                list.Add(GetQuestsInfo(quest));
            }
        }

        private NQuestInfo GetQuestsInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };
        }

        /// <summary>
        /// 接受任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;
            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                var dbquest = DBService.Instance.Entities.CharacterQuests.Create();
                dbquest.QuestID = quest.ID;
                if (quest.Target1 == QuestTarget.None)
                {
                    //无目标直接完成
                    dbquest.Status = (int)QuestStatus.Complated;
                }
                else
                {
                    //有目标则进入进行中状态
                    dbquest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAccept.Quest = this.GetQuestsInfo(dbquest);
                character.Data.Quests.Add(dbquest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;
            QuestDefine quest;

            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                //在db中查找该任务
                var dbquest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if (dbquest != null)
                {
                    if (dbquest.Status != (int)QuestStatus.Complated)
                    {
                        //还未完成任务
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    dbquest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestsInfo(dbquest);
                    DBService.Instance.Save();

                    //处理任务奖励
                    if (quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }
                    if (quest.RewardExp > 0)
                    {
                        //还没加升级系统
                        //character.Exp += quest.RewardExp;
                    }
                    if (quest.RewardItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem1Count);
                    }
                    DBService.Instance.Save();
                    return Result.Success;
                }
                sender.Session.Response.questSubmit.Errormsg = "[数据库]任务不存在";
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questSubmit.Errormsg = "[配置表]任务不存在";
                return Result.Failed;
            }
        }
    }
}
