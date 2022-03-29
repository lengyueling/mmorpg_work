using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// 测试功能Npc协程
    /// </summary>
    class TestManager : Singleton<TestManager>
    {
        public void Init()
        {
            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnNpcInvokeShop);
            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeInsrance, OnNpcInvokeInsrance);
        }

        private bool OnNpcInvokeShop(NpcDefine npc)
        {
            Debug.LogFormat("TestManager.OnNpcInvokeShop :NPC[{0}: {1}] Type:{2} Func:{3}]", npc.ID, npc.Name, npc.Type, npc.Function);
            UITest uITest = UIManager.Instance.Show<UITest>();
            uITest.SetTitle(npc.Name);
            return true;
        }

        private bool OnNpcInvokeInsrance(NpcDefine npc)
        {
            Debug.LogFormat("TestManager.OnNpcInvokeInsrance :NPC[{0}: {1}] Type:{2} Func:{3}]", npc.ID, npc.Name, npc.Type, npc.Function);
            MessageBox.Show("点击了解Npc" + npc.Name, "NPC对话");
            return true;
        }

        
    }
}
