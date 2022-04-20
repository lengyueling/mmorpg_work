using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class StatusManager
    {
        Character Owner;

        /// <summary>
        /// 状态列表
        /// 记录一个会话中的多个状态
        /// 合并发送减少SendResponse的次数
        /// </summary>
        private List<NStatus> Status { get; set; }

        public bool HasStatus
        {
            get
            {
                return this.Status.Count > 0;
            }
        }

        public StatusManager(Character owner)
        {
            this.Owner = owner;
            this.Status = new List<NStatus>();
        }

        /// <summary>
        /// 状态改变
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="action"></param>
        public void AddStatus(StatusType type,int id,int value,StatusAction action)
        {
            this.Status.Add(new NStatus()
            {
                Type = type,
                Id = id,
                Value = value,
                Action = action
            });
        }

        /// <summary>
        /// 金币变化
        /// 增加或者减小
        /// </summary>
        /// <param name="goldDelta"></param>
        public void AddGoldChange(int goldDelta)
        {
            if (goldDelta > 0)
            {
                this.AddStatus(StatusType.Money, 0, goldDelta, StatusAction.Add);
            }
            if (goldDelta < 0)
            {
                this.AddStatus(StatusType.Money, 0, -goldDelta, StatusAction.Delete);
            }
        }

        /// <summary>
        /// 增加道具
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="action"></param>
        public void AddItemChange(int id,int count,StatusAction action)
        {
            this.AddStatus(StatusType.Item, id, count, action);
        }

        /// <summary>
        /// 应用当前会话的状态
        /// 追加新的传输请求
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (message.StatusNotify == null)
            {
                message.StatusNotify = new StatusNotify();
            }
            foreach (var status in this.Status)
            {
                message.StatusNotify.Status.Add(status);
            }
            this.Status.Clear();
        }
    }
}
