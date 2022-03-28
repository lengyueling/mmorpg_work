using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;

namespace Managers
{
    class NpcManager : Singleton<NpcManager>
    {
        public NpcDefine GetNpcDefine(int npcID)
        {
            return DataManager.Instance.Npcs[npcID];
        }
    }
}
