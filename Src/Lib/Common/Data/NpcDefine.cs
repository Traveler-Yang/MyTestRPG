using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    public enum Npctype
    {
        None = 0,
        /// <summary>
        /// 功能型NPC
        /// </summary>
        Functional = 1,
        /// <summary>
        /// 任务型NPC
        /// </summary>
        Task = 2,
    }

    public enum NpcFunction
    {
        None = 0,
        /// <summary>
        /// 打开商店
        /// </summary>
        InvokeShop = 1,
        /// <summary>
        /// 打开副本
        /// </summary>
        InvokeInsrance = 2,
    }


    public class NpcDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Descript { get; set;}
        public NVector3 Postion { get; set; }
        public Npctype Type { get; set; }
        public NpcFunction Function { get; set; }
        public int Param { get; set; }
    }
}
