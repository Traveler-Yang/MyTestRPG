using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Common.Data
{
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum QuestType
    {
        [Description("主线")]
        Main,
        [Description("支线")]
        Branch,
    }

    /// <summary>
    /// 任务目标类型
    /// </summary>
    public enum QuestTraget
    {
        None,
        Kill,
        Item,
    }

    public class QuestDefine
    {
        public int ID { get; set; } //任务ID
        public string Name { get; set; } //任务名称
        public int LimitLevel { get; set; } //任务限制等级
        public CharacterClass LimitClass { get; set; } //任务限制职业
        public int PreQuest { get; set; } //前置任务ID
        public QuestType Type { get; set; } //任务类型
        public int AcceptNPC { get; set; } //接受任务NPC ID
        public int SubmitNPC { get; set; } //提交任务NPC ID
        public string Overview { get; set; } //任务概述
        public string Dialog { get; set; } //任务对话
        public string DialogAccept { get; set; } //任务接受对话
        public string DialogDeny { get; set; } //任务拒绝对话
        public string DialogIncomplete { get; set; } //任务未完成对话
        public string DialogFinish { get; set; } //任务完成对话

        public QuestTraget Target1 { get; set; } //任务目标1类型
        public int Target1ID { get; set; } //任务目标1ID
        public int Target1Num { get; set; } //任务目标1数量
        public QuestTraget Target2 { get; set; } //任务目标2类型
        public int Target2ID { get; set; } //任务目标2ID
        public int Target2Num { get; set; } //任务目标2数量
        public QuestTraget Target3 { get; set; } //任务目标3类型
        public int Target3ID { get; set; } //任务目标3ID
        public int Target3Num { get; set; } //任务目标3数量
        public int RewardGold { get; set; } //任务奖励金币
        public int RewardExp { get; set; } //任务奖励经验
        public int RewardItem1 { get; set; } // 任务奖励物品1ID
        public int RewardItem1Count { get; set; } // 任务奖励物品1数量
        public int RewardItem2 { get; set; } // 任务奖励物品2ID
        public int RewardItem2Count { get; set; } // 任务奖励物品2数量
        public int RewardItem3 { get; set; } // 任务奖励物品3ID
        public int RewardItem3Count { get; set; } // 任务奖励物品3数量
    }
}
