using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using UnityEngine;

namespace Managers
{
    class NpcManager : Singleton<NpcManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();
        Dictionary<int, Vector3> npcPosition = new Dictionary<int, Vector3>();

        /// <summary>
        /// 注册Npc事件
        /// </summary>
        /// <param name="function"></param>
        /// <param name="action"></param>
        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }
        }

        /// <summary>
        /// 从DataManager获取NpcDefine
        /// </summary>
        /// <param name="npcID"></param>
        /// <returns></returns>
        public NpcDefine GetNpcDefine(int npcID)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(npcID, out npc);
            return npc;
        }

        /// <summary>
        /// 交互校验
        /// 校验npcId是否存在
        /// 存在则进行下一步校验
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public bool Interactive(int npcId)
        {
            if (DataManager.Instance.Npcs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.Npcs[npcId];
                return Interactive(npc);
            }
            return false;
        }

        /// <summary>
        /// 交互校验
        /// 如果是任务npc则进行任务交互
        /// 如果是功能npc则进行功能交互
        /// 以此类推
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public bool Interactive(NpcDefine npc)
        {
            if (DoTaskInteractive(npc))
            {
                return true;
            }
            else if (npc.Type == NpcType.Functional)
            {
                return DoFunctionInteractive(npc);
            }
            return false;
        }
        /// <summary>
        /// 功能Npc交互
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional)
            {
                return false;
            }
            if (!eventMap.ContainsKey(npc.Function))
            {
                return false;
            }
            //执行委托函数
            return eventMap[npc.Function](npc);
        }

        /// <summary>
        /// 任务Npc交互
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private bool DoTaskInteractive(NpcDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
            {
                return false;
            }
            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        /// <summary>
        /// 设置npc位置
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="pos"></param>
        public void UpdateNpcPosition(int npc,Vector3 pos)
        {
            this.npcPosition[npc] = pos;
        }

        /// <summary>
        /// 获取npc位置
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public Vector3 GetNpcPosition(int npc)
        {
            return this.npcPosition[npc];
        }
    }
}
