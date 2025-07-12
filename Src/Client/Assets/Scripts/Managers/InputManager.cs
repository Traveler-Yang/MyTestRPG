using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    class InputManager : MonoSingleton<InputManager>
    {
        /// <summary>
        /// 是否是输入模式
        /// </summary>
        public bool IsInputMode = false;
    }
}
