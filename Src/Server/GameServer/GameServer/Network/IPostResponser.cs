using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    /// <summary>
    /// 后处理接口
    /// </summary>
    public interface IPostResponser
    {
        void PostProcess(NetMessageResponse message);
    }
}

