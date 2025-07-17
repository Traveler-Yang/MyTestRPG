using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    class InputManager : MonoSingleton<InputManager>
    {
        public enum InputType
        {
            None,
            /// <summary>
            /// 角色
            /// </summary>
            Equip,
            /// <summary>
            /// 背包
            /// </summary>
            Bag,
            /// <summary>
            /// 任务
            /// </summary>
            Quest,
            /// <summary>
            /// 好友
            /// </summary>
            Friend,
            /// <summary>
            /// 技能
            /// </summary>
            Skills,
            /// <summary>
            /// 坐骑
            /// </summary>
            Ride,
            /// <summary>
            /// 设置
            /// </summary>
            Setting
        }

        /// <summary>
        /// 是否是输入模式
        /// </summary>
        public bool IsInputMode = false;

        public bool EnableGameplayInput = false; //是否启用快捷键监听

        private Dictionary<KeyCode, InputType> _keyBindings;

        private void Start()
        {
            // 初始化快捷键绑定
            _keyBindings = new Dictionary<KeyCode, InputType>
            {
                { KeyCode.C, InputType.Equip },
                { KeyCode.B, InputType.Bag },
                { KeyCode.Q, InputType.Quest },
                { KeyCode.F, InputType.Friend },
                { KeyCode.K, InputType.Skills },
                { KeyCode.R, InputType.Ride },
                { KeyCode.I, InputType.Setting }
            };
        }

        private void Update()
        {
            if (!EnableGameplayInput || IsInputMode) return;

            foreach (var kv in _keyBindings)
            {
                if (Input.GetKeyDown(kv.Key))
                {
                    OnUI(kv.Value);
                    break;
                }
            }
        }
        private void OnUI(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Equip:
                    UIMain.OnClickCharEquip();
                    break;
                case InputType.Bag:
                    UIMain.OnClickBag();
                    break;
                case InputType.Quest:
                    UIMain.OnClickQuest();
                    break;
                case InputType.Friend:
                    UIMain.OnClickFriend();
                    break;
                case InputType.Skills:
                    UIMain.OnClickSKill();
                    break;
                case InputType.Ride:
                    UIMain.OnClickRide();
                    break;
                case InputType.Setting:
                    UIMain.OnClickSetting();
                    break;
            }
        }
    }
}
