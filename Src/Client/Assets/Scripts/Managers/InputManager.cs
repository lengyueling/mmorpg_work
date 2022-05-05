using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using UnityEngine;

namespace Managers
{
    class InputManager : MonoSingleton<InputManager>
    {
        /// <summary>
        /// 是否处于输入模式
        /// </summary>
        public bool IsInputMode = false;
    }
}
