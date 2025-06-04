using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    class NPCManager : Singleton<NPCManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        /// <summary>
        /// 注册 NPC 事件处理器
        /// </summary>
        /// <param name="function"></param>
        /// <param name="handler"></param>
        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler handler)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = handler;
            }
            else
            {
                eventMap[function] += handler;
            }
        }


        public NpcDefine GetNpcDefine(int npcId)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(npcId, out npc);
            return npc;
        }

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
        /// 与 NPC 交互处理
        /// </summary>
        /// <param name="npc"></param>
        /// <returns>返回交互结果是否成功</returns>
        public bool Interactive(NpcDefine npc)
        {
            //判断 是哪种 类型的 NPC
            if (npc.Type == Npctype.Task)// 任务型 NPC
            {
                return DoTaskInteractive(npc);
            }
            else if (npc.Type == Npctype.Functional)// 功能型 NPC
            {
                return DoFunctionalInteractive(npc);
            }
            return false;
        }

        /// <summary>
        /// 任务型 NPC 交互处理
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private bool DoTaskInteractive(NpcDefine npc)
        {
            MessageBox.Show("点击了NPC：" + npc.Name, "NPC对话");
            return true;
        }

        /// <summary>
        /// 功能性 NPC 交互处理
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private bool DoFunctionalInteractive(NpcDefine npc)
        {
            if (npc.Type != Npctype.Functional)
                return false;
            if (!eventMap.ContainsKey(npc.Function))
                return false;
            return eventMap[npc.Function](npc);
        }

    }
}
