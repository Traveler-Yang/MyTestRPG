using Models;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None = 0, //没有任务
        Complete, //拥有已完成可提交任务
        Available, //拥有可接取任务
        Incomplete, //拥有未完成任务
    }

    public class QuestManager : Singleton<QuestManager>
    {
        //所有有效的任务
        public List<NQuestInfo> questInfos;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public void Init(List<NQuestInfo> quests)
        {
             this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuest();
        }

        void InitQuest()
        {
            //初始化已有任务
            foreach (var info in this.questInfos)
            {
                Quest quest = new Quest(info);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubitNPC, quest);
                this.allQuests[quest.Info.QuestId] = quest;
            }
            //初始化可用任务
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                    continue;//如果任务的限制职业不符合当前角色的职业，则跳过

                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                    continue; //如果任务的限制等级大于当前角色的等级，则跳过

                if (this.allQuests.ContainsKey(kv.Key))
                    continue; //如果任务已经存在，则跳过

                if (kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))//取出前置任务
                    {
                        if (preQuest.Info == null)
                            continue; //前置任务未接取
                        if(preQuest.Info.Status != QuestStatus.Finished)
                            continue; //前置任务未完成
                    }
                    else
                        continue; //前置任务还没接
                }
                Quest quest = new Quest(kv.Value);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                //this.AddNpcQuest(quest.Define.SubitNPC, quest);
                this.allQuests[quest.Info.QuestId] = quest;
            }
        }

        private void AddNpcQuest(int npcId, Quest quest)
        {
            if(!this.npcQuests.ContainsKey(npcId))
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();


        }

        internal NpcQuestStatus GetNpcQuestStatus(int iD)
        {
            throw new NotImplementedException();
        }
    }
}
