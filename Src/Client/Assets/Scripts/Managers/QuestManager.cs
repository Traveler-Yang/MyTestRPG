using Models;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using Services;
using UnityEngine.Events;

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
        //任务列表缓存（从服务器接收来的）
        public List<NQuestInfo> questInfos;
        //所有有效的任务
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public UnityAction<Quest> onQuestStatusChanged;

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
                //初始化所有任务时，实例化一个任务对象
                Quest quest = new Quest(info);
                this.allQuests[quest.Info.QuestId] = quest;
            }

            this.CheckAvailableQuests();

            foreach (var kv in this.allQuests)
            {
                //并将任务添加到对应的 NPC 的任务列表中
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        /// <summary>
        /// 初始化可用任务
        /// </summary>
        void CheckAvailableQuests()
        {
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
                        if (preQuest.Info.Status != QuestStatus.Finished)
                            continue; //前置任务未完成
                    }
                    else
                        continue; //前置任务还没接
                }
                Quest quest = new Quest(kv.Value);
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        /// <summary>
        /// 添加任务到 NPC
        /// </summary>
        /// <param name="npcId"></param>
        /// <param name="quest"></param>
        private void AddNpcQuest(int npcId, Quest quest)
        {
            //如果这个 NPC 没有任务列表，则创建一个新的任务列表
            if (!this.npcQuests.ContainsKey(npcId))
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

            List<Quest> availables;
            List<Quest> compltes;
            List<Quest> incompletes;

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out compltes))
            {
                compltes = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = compltes;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incompletes))
            {
                incompletes = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incompletes;
            }

            //如果这个是新任务（Info 为空），则添加给接取任务的npc
            if (quest.Info == null)
            {
                //如果这个npc是接收任务的npc
                //查找接收任务的这个npc，如果没有，则Add
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                //进行中任务，添加给接任务的 NPC
                if (npcId == quest.Define.AcceptNPC && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }

                //如果是 交任务的npc，并且判断任务状态是否是已经完成的
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                //状态是进行中的任务，则添加到未完成的任务列表中
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }
            }
        }

        public NpcQuestStatus GetNpcQuestStatus(int npcID)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (npcQuests.TryGetValue(npcID, out status))//得到这个 NPC 的任务列表
            {
                //如果有已完成的任务，则返回已完成状态
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                //如果有可接取的任务，则返回可接取状态
                if (status[NpcQuestStatus.Available].Count > 0)
                    return NpcQuestStatus.Available;
                //如果有未完成的任务，则返回未完成状态
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int iD)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            //判断这个 NPC是否有任务
            //如果有，则取出来，并判断这个 NPC 的任务状态
            if (npcQuests.TryGetValue(iD, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
            }
            return false;
        }

        /// <summary>
        /// 显示任务对话框
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        bool ShowQuestDialog(Quest quest)
        {
            //判断任务是空（新任务）或者任务状态为已完成
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                //设置对话框
                UIQuestDialog dialog = UIManager.Instance.Show<UIQuestDialog>();
                //设置对话框的任务信息
                dialog.SetQuest(quest);
                //添加对话框关闭事件
                dialog.OnClose += OnQuestDialogClose;
                return true;
            }
            //如果任务状态是已接受未完成，则显示任务未完成对话框
            if (quest.Info != null && quest.Info.Status == QuestStatus.InProgress)
            {
                if (string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        /// <summary>
        /// 任务对话框关闭事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void OnQuestDialogClose(UIWindow sender, UIWindow.UIWindowResult result)
        {
            UIQuestDialog dialog = sender as UIQuestDialog;
            //判断点击的是yes还是no
            if (result == UIWindow.UIWindowResult.Yes)
            {
                //如果点击的是接受任务或者提交任务，则发送任务接受或提交请求
                //如果任务信息为空，则表示是新任务（表示点击的是接受任务）
                if (dialog.quest.Info == null)
                    QuestService.Instance.SendQuestAccept(dialog.quest);
                //如果任务信息不为空，且任务状态为已完成，则表示点击的是提交任务
                else if(dialog.quest.Info.Status == QuestStatus.Complated)
                    QuestService.Instance.SendQuestSubmit(dialog.quest);
            }
            else if (result == UIWindow.UIWindowResult.No)
            {
                //如果点击的是no，则显示任务拒绝对话框
                MessageBox.Show(dialog.quest.Define.DialogDeny);
            }
        }

        Quest RefreshQuestStatus(NQuestInfo quest)
        {
            //当任务状态发生变化时，刷新任务状态
            this.npcQuests.Clear();
            Quest result;
            if (allQuests.ContainsKey(quest.QuestId))
            {
                //如果任务已经存在，则更新任务状态
                this.allQuests[quest.QuestId].Info = quest;
                result = this.allQuests[quest.QuestId];
            }
            else
            {
                //如果任务不存在，则创建一个新的任务对象
                result = new Quest(quest);
                this.allQuests[quest.QuestId] = result;
            }

            this.CheckAvailableQuests();

            foreach (var kv in this.allQuests)
            {
                //并将任务添加到对应的 NPC 的任务列表中
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }

            //任务更新后，让NPC的任务状态发生变化
            if (onQuestStatusChanged != null)
                onQuestStatusChanged.Invoke(result);
            return result;
        }

        internal void OnQuestAccepted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            //显示任务接受对话框
            MessageBox.Show(quest.Define.DialogAccept);
        }


        internal void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            //显示任务完成对话框
            MessageBox.Show(quest.Define.DialogFinish);
        }

    }
}
